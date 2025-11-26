using System.Diagnostics;
using Azure.Core;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Broker;
using Microsoft.Identity.Client.Extensions.Msal;

namespace Scotec.Identity.AzureActiveDirectory;

/// <summary>
///     Represents an Azure Active Directory authentication session, managing token acquisition, refresh, and
///     sign-in/sign-out operations.
/// </summary>
/// <remarks>
///     Provides a high-level abstraction for Azure AD authentication in desktop applications. Utilizes MSAL for token
///     management,
///     supports both synchronous and asynchronous sign-in/sign-out flows, and exposes a <see cref="TokenCredential" /> for
///     Azure SDK clients.
///     Optionally, can automatically sign out the user when disposed to ensure security and resource cleanup.
/// </remarks>
internal sealed class AadAuthSession : IAadAuthSession
{
    private static readonly SemaphoreSlim PcaLock = new(1, 1);
    private readonly AadAuthOptions _options;
    private readonly string[] _scopes;
    private AuthenticationResult? _authenticationResult;
    private IPublicClientApplication? _pca;

    internal AadAuthSession(AadAuthOptions options)
    {
        _options = options;
        _scopes = options.Scopes;
    }

    public bool IsSignedIn => Account is not null;

    public Task<AuthenticationResult?> SignInAsync(Func<AcquireTokenInteractiveParameterBuilder, AcquireTokenInteractiveParameterBuilder>? configure = null,
                                                   CancellationToken cancellationToken = default)
    {
        return SignInAsync(Prompt.ForceLogin, configure, cancellationToken);
    }

    public async Task<AuthenticationResult?> SignInAsync(
        Prompt prompt, Func<AcquireTokenInteractiveParameterBuilder, AcquireTokenInteractiveParameterBuilder>? configure = null,
        CancellationToken cancellationToken = default)
    {
        var currentAccount = Account;

        try
        {
            var pca = await GetPublicClientApplicationAsync(cancellationToken);
            var builder = pca.AcquireTokenInteractive(_scopes)
                             //.WithParentActivityOrWindow(handle)
                             .WithPrompt(prompt)
                             .WithAccount(null); // No specific account, let user choose

            if (configure is not null)
            {
                builder = configure(builder);
            }

            _authenticationResult = await builder.ExecuteAsync(cancellationToken);
        }
        catch
        {
            _authenticationResult = null;
        }
        finally
        {
            await RaiseEvents(currentAccount);
        }
        return _authenticationResult;
    }

    public async Task<AuthenticationResult?> SignInSilentAsync(CancellationToken cancellationToken)
    {
        return await InternalSignInSilentAsync(null, cancellationToken);
    }

    public async Task<AuthenticationResult?> SignInSilentAsync(IAccount account, CancellationToken cancellationToken)
    {
        return await InternalSignInSilentAsync(account, cancellationToken);
    }

    public Task<AuthenticationResult?> SignInAsync(IAccount? account,
                                                   Func<AcquireTokenInteractiveParameterBuilder, AcquireTokenInteractiveParameterBuilder>? configure = null,
                                                   CancellationToken cancellationToken = default)
    {
        return SignInAsync(account, Prompt.NoPrompt, configure, cancellationToken);
    }

    public async Task<AuthenticationResult?> SignInAsync(IAccount? account, Prompt prompt,
                                                         Func<AcquireTokenInteractiveParameterBuilder, AcquireTokenInteractiveParameterBuilder>? configure =
                                                             null, CancellationToken cancellationToken = default)
    {
        if (account is null)
        {
            return await SignInAsync(prompt, configure, cancellationToken);
        }

        var currentAccount = Account;

        var pca = await GetPublicClientApplicationAsync(cancellationToken);
        AuthenticationResult? result = null;
        try
        {
            _authenticationResult = await pca.AcquireTokenSilent(_scopes, account)
                              .ExecuteAsync(cancellationToken);
        }
        catch (MsalUiRequiredException)
        {
            var builder = pca.AcquireTokenInteractive(_scopes)
                             .WithPrompt(prompt)
                             .WithAccount(account);

            if (configure is not null)
            {
                builder = configure(builder);
            }

            result = await builder.ExecuteAsync(cancellationToken);
        }
        catch
        {
            result = null;
        }
        finally
        {
            _authenticationResult = result?.Account is not null ? result : null;
            await RaiseEvents(currentAccount);
        }

        return _authenticationResult;
    }

    public async Task<IEnumerable<IAccount>> GetAccountsAsync()
    {
        var pca = await GetPublicClientApplicationAsync(CancellationToken.None);
        var accounts = (await pca.GetAccountsAsync()).ToList();

        accounts.Insert(0, PublicClientApplication.OperatingSystemAccount);

        return accounts;
    }

    public event EventHandler<EventArgs>? SignedOut;
    public event EventHandler<EventArgs>? SignedIn;

    /// <summary>
    ///     Gets the Azure AD account associated with this session.
    /// </summary>
    /// <remarks>
    ///     The account represents the signed-in user for this authentication session.
    /// </remarks>
    public IAccount? Account => _authenticationResult?.Account;

    /// <summary>
    ///     Gets or sets a value indicating whether to automatically sign out when the session is disposed.
    /// </summary>
    /// <remarks>
    ///     If set to <c>true</c>, the session will sign out the user and clear cached tokens upon disposal.
    ///     This helps prevent unauthorized access and ensures session termination.
    /// </remarks>
    public bool AutoSignOut { get; set; }

    /// <summary>
    ///     Signs out of the current Azure AD session synchronously.
    /// </summary>
    /// <remarks>
    ///     Removes all cached accounts and tokens, effectively signing out the user from the application.
    ///     This method blocks until the operation is complete.
    /// </remarks>
    public void SignOut()
    {
        SignOutAsync().ConfigureAwait(false).GetAwaiter().GetResult();
    }

    /// <summary>
    ///     Asynchronously signs out of the current Azure AD session and clears the cached authentication result.
    /// </summary>
    /// <returns>A task representing the asynchronous sign-out operation.</returns>
    /// <remarks>
    ///     Clears the cached authentication result and removes all accounts from the token cache, ensuring the user is fully
    ///     signed out.
    ///     Use this method in asynchronous workflows for non-blocking sign-out.
    /// </remarks>
    public async Task SignOutAsync()
    {
        var currentAccount = Account;
        if (currentAccount is not null)
        {
            var pca = await GetPublicClientApplicationAsync(CancellationToken.None);

            await pca.RemoveAsync(currentAccount);
        }

        _authenticationResult = null;
        await RaiseEvents(currentAccount);
    }

    public async Task<AuthenticationResult?> GetTokenSilentAsync(CancellationToken cancellationToken)
    {
        if (_options.ClientId is null || _options.TenantId is null)
        {
            return null;
        }
        // If we have a valid token and the account is still in the cache, return it
        if (_authenticationResult is not null && _authenticationResult.ExpiresOn > DateTimeOffset.UtcNow.AddMinutes(5)
                                              && Account != null)
        {
            return _authenticationResult;
        }

        // Try to acquire token silently.
        var currentAccount = Account;
        try
        {
            
            if (Account is null)
            {
                return null;
            }

            var pca = await GetPublicClientApplicationAsync(cancellationToken);

            var result = await pca
                               .AcquireTokenSilent(_scopes, Account)
                               .ExecuteAsync(cancellationToken);

            _authenticationResult = result.Account is not null ? result : null;

            return _authenticationResult;
        }
        catch (MsalUiRequiredException)
        {
            _authenticationResult = null;
            await RaiseEvents(currentAccount);
            throw;
        }
        catch
        {
            _authenticationResult = null;
            await RaiseEvents(currentAccount);
            throw;
        }
        finally
        {
            await RaiseEvents(currentAccount);
        }
    }

    private Task RaiseEvents(IAccount? currentAccount)
    {
        if ((currentAccount is null && Account is null) || currentAccount != Account)
        {
            OnSignedOut();
        }
        
        if(Account is not null&& currentAccount != Account)
        {
            OnSignedIn();
        }
        return Task.CompletedTask;
    }

    private void OnSignedIn()
    {
        SignedIn?.Invoke(this, EventArgs.Empty);
    }

    private void OnSignedOut()
    {
        SignedOut?.Invoke(this, EventArgs.Empty);
    }

    public TokenCredential? GetTokenCredential()
    {
        return IsSignedIn ? new MsalTokenCredential(this) : null;
    }

    public async ValueTask DisposeAsync()
    {
        if (AutoSignOut)
        {
            await SignOutAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     Disposes the session and releases resources.
    /// </summary>
    /// <remarks>
    ///     If <see cref="AutoSignOut" /> is enabled, this method will sign out the user and clear cached tokens.
    ///     This method blocks until the operation is complete.
    /// </remarks>
    public void Dispose()
    {
        if (AutoSignOut)
        {
            SignOut();
        }
    }

    private async Task<IPublicClientApplication> GetPublicClientApplicationAsync(CancellationToken cancellationToken)
    {
        await PcaLock.WaitAsync(cancellationToken);
        try
        {
            if (_pca is not null)
            {
                return _pca;
            }

            var brokerOptions = new BrokerOptions(BrokerOptions.OperatingSystems.Windows)
            {
                Title = "BIM Family Manager - Azure Storage Sign-in"
            };

            _pca = PublicClientApplicationBuilder
                   .Create(_options.ClientId!.Value.ToString("D"))
                   .WithAuthority(AzureCloudInstance.AzurePublic, _options.TenantId!.Value.ToString("D"))
                   .WithBroker(brokerOptions)
                   .WithRedirectUri("https://login.microsoftonline.com/common/oauth2/nativeclient")
                   //.WithDefaultRedirectUri()
                   .Build();

            var cacheHelper = await CreateCacheHelperAsync();

            // Let the cache helper handle MSAL's cache, otherwise the user will be prompted to sign in every time.
            cacheHelper.RegisterCache(_pca.UserTokenCache);

            _options.TokenCache?.Enable(_pca.UserTokenCache);

            return _pca;
        }
        finally
        {
            PcaLock.Release();
        }
    }

    private async Task<MsalCacheHelper> CreateCacheHelperAsync()
    {
        // Since this is for WPF application, only Windows storage is configured
        var storageProperties = new StorageCreationPropertiesBuilder(
                "BIM.FamilyManager", MsalCacheHelper.UserRootDirectory)
            .Build();

        var cacheHelper = await MsalCacheHelper.CreateAsync(storageProperties, new TraceSource("MSAL.CacheTrace"))
                                               .ConfigureAwait(false);

        return cacheHelper;
    }

    private async Task<AuthenticationResult?> InternalSignInSilentAsync(IAccount? account, CancellationToken cancellationToken)
    {
        // Take the specified account, or the current account, or all accounts
        var accounts = account is not null ? [account] : Account is not null ? [Account] : (await GetAccountsAsync()).ToList();

        // If we have a valid token and the account is still in the cache, return it
        if (_authenticationResult is not null && _authenticationResult.ExpiresOn > DateTimeOffset.UtcNow.AddMinutes(5)
                                              && Account != null && accounts.Contains(Account))
        {
            return _authenticationResult;
        }

        _authenticationResult = null;
        var pca = await GetPublicClientApplicationAsync(cancellationToken);
        foreach (var testAccount in accounts)
        {
            var result = await pca.AcquireTokenSilent(_scopes, testAccount)
                                  .ExecuteAsync(cancellationToken);

            if (result.Account is not null)
            {
                _authenticationResult = result;
                break;
            }
        }

        return _authenticationResult;
    }

    ~AadAuthSession()
    {
        if (AutoSignOut)
        {
            SignOut();
        }
    }
}

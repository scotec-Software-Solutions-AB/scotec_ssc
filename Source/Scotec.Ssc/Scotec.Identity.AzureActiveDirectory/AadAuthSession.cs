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
    // Guards _authenticationResult for concurrent reads/writes across async sign-in paths
    private static readonly object AuthResultLock = new();
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
        return SignInAsync(Prompt.SelectAccount, configure, cancellationToken);
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

            AuthenticationResult = await builder.ExecuteAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            AuthenticationResult = null;
            throw;
        }
        catch (MsalException)
        {
            AuthenticationResult = null;
        }
        return AuthenticationResult;
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
            AuthenticationResult = await pca.AcquireTokenSilent(_scopes, account)
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
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (MsalException)
        {
            result = null;
        }
        finally
        {
            AuthenticationResult = result?.Account is not null ? result : null;
        }

        return AuthenticationResult;
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
    public IAccount? Account => AuthenticationResult?.Account;

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

        AuthenticationResult = null;
    }

    public async Task<AuthenticationResult?> GetTokenSilentAsync(CancellationToken cancellationToken)
    {
        // If we have a valid token and the account is still in the cache, return it
        if (AuthenticationResult is not null && AuthenticationResult.ExpiresOn > DateTimeOffset.UtcNow.AddMinutes(5)
                                              && Account != null)
        {
            return AuthenticationResult;
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

            AuthenticationResult = result.Account is not null ? result : null;

            return AuthenticationResult;
        }
        catch (MsalUiRequiredException)
        {
            AuthenticationResult = null;
            throw;
        }
        catch
        {
            AuthenticationResult = null;
            throw;
        }
    }

    private AuthenticationResult? AuthenticationResult
    {
        get
        {
            lock (AuthResultLock)
            {
                return _authenticationResult;
            }
        }
        set
        {
            IAccount? currentAccount;
            lock (AuthResultLock)
            {
                currentAccount = _authenticationResult?.Account;
                _authenticationResult = value;
            }

            RaiseEvents(currentAccount);
        }
    }

    private void RaiseEvents(IAccount? currentAccount)
    {
        var newAccount = Account;

        if (currentAccount is not null && newAccount is null)
        {
            OnSignedOut();
        }

        if (newAccount is not null && currentAccount != newAccount)
        {
            OnSignedIn();
        }
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
                Title = _options.BrokerTitle
            };

            _pca = PublicClientApplicationBuilder
                   .Create(_options.ClientId.ToString("D"))
                   .WithAuthority(AzureCloudInstance.AzurePublic, _options.TenantId.ToString("D"))
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
        var cacheName = _options.TokenCacheName ?? _options.ClientId.ToString("D");
        var storageProperties = new StorageCreationPropertiesBuilder(
                cacheName, MsalCacheHelper.UserRootDirectory)
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
        if (AuthenticationResult is not null && AuthenticationResult.ExpiresOn > DateTimeOffset.UtcNow.AddMinutes(5)
                                              && Account != null && accounts.Contains(Account))
        {
            return AuthenticationResult;
        }

        AuthenticationResult = null;
        var pca = await GetPublicClientApplicationAsync(cancellationToken);
        foreach (var testAccount in accounts)
        {
            try
            {
                var result = await pca.AcquireTokenSilent(_scopes, testAccount)
                                      .ExecuteAsync(cancellationToken);

                if (result.Account is not null)
                {
                    AuthenticationResult = result;
                    break;
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (MsalUiRequiredException)
            {
                // Silent acquisition not possible for this account; try the next one.
            }
        }

        return AuthenticationResult;
    }
    
    

    }

using Azure.Core;
using Microsoft.Identity.Client;

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
public sealed class AadAuthSession : IAadAuthSession
{
    private readonly AadAuthOptions _options;
    private readonly IPublicClientApplication _pca;
    private readonly string[] _scopes;

    internal AadAuthSession(AadAuthOptions options)
    {
        _options = options;
        _scopes = options.Scopes;

        _pca = PublicClientApplicationBuilder
               .Create(options.ClientId)
               .WithAuthority(AzureCloudInstance.AzurePublic, options.TenantId)
               .WithDefaultRedirectUri()
               .Build();

        options.TokenCache?.Enable(_pca.UserTokenCache);
    }

    public bool IsSignedIn => Account is not null;

    public Task<IAccount?> SignInAsync()
    {
        return SignInAsync(Prompt.ForceLogin);
    }

    public async Task<IAccount?> SignInAsync(Prompt prompt)
    {
        var result = await _pca.AcquireTokenInteractive(_scopes)
                               .WithPrompt(prompt)
                               .WithAccount(null) // No specific account, let user choose
                               .ExecuteAsync();

        Account = result.Account;

        return result.Account;
    }

    public async Task<IAccount?> SignInSilentAsync()
    {
        var accounts = await GetAccountsAsync();

        foreach (var account in accounts)
        {
            var result = await _pca.AcquireTokenSilent(_scopes, account)
                           .ExecuteAsync();

            if (result.Account is not null)
            {
                Account = result.Account;

                return result.Account;
            }
        }

        return null;
    }
    public Task<IAccount?> SignInSilentAsync(IAccount account)
    {
        throw new NotImplementedException();
    }

    public Task<AuthenticationResult> GetTokenSilentAsync(IAccount account)
    {
        throw new NotImplementedException();
    }

    public Task<IAccount?> SignInAsync(IAccount? account)
    {
        return SignInAsync(account, Prompt.NoPrompt);
    }

    public async Task<IAccount?> SignInAsync(IAccount? account, Prompt prompt)
    {
        if (account is null)
        {
            return await SignInAsync(prompt);
        }

        AuthenticationResult result;
        try
        {
            result = await _pca.AcquireTokenSilent(_scopes, account)
                               .ExecuteAsync();
        }
        catch (MsalUiRequiredException)
        {
            result = await _pca.AcquireTokenInteractive(_scopes)
                               .WithPrompt(prompt)
                               .WithAccount(account)
                               .ExecuteAsync();
        }

        Account = result.Account;

        return Account;
    }


    public async Task<IEnumerable<IAccount>> GetAccountsAsync()
    {
        return await _pca.GetAccountsAsync();
    }

    /// <summary>
    ///     Gets the Azure AD account associated with this session.
    /// </summary>
    /// <remarks>
    ///     The account represents the signed-in user for this authentication session.
    /// </remarks>
    public IAccount? Account { get; private set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to automatically sign out when the session is disposed.
    /// </summary>
    /// <remarks>
    ///     If set to <c>true</c>, the session will sign out the user and clear cached tokens upon disposal.
    ///     This helps prevent unauthorized access and ensures session termination.
    /// </remarks>
    public bool AutoSignOut { get; set; }

    /// <summary>
    ///     Gets the <see cref="TokenCredential" /> for Azure SDK authentication.
    /// </summary>
    /// <remarks>
    ///     Exposes a credential compatible with Azure SDK clients, allowing integration with Azure services that require
    ///     authentication.
    /// </remarks>
    public TokenCredential TokenCredential { get; }

    /// <summary>
    ///     Disposes the session and releases resources.
    /// </summary>
    /// <remarks>
    ///     If <see cref="AutoSignOut" /> is enabled, this method will sign out the user and clear cached tokens.
    ///     This method blocks until the operation is complete.
    /// </remarks>
    public void Dispose()
    {
        DisposeAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    ///     Asynchronously disposes the session and releases resources.
    /// </summary>
    /// <returns>A task representing the asynchronous dispose operation.</returns>
    /// <remarks>
    ///     If <see cref="AutoSignOut" /> is enabled, this method will sign out the user and clear cached tokens
    ///     asynchronously.
    ///     Use this method in asynchronous scenarios to avoid blocking the calling thread.
    /// </remarks>
    public async Task DisposeAsync()
    {
        if (AutoSignOut)
        {
            await SignOutAsync();
        }
    }

    /// <summary>
    ///     Signs out of the current Azure AD session synchronously.
    /// </summary>
    /// <remarks>
    ///     Removes all cached accounts and tokens, effectively signing out the user from the application.
    ///     This method blocks until the operation is complete.
    /// </remarks>
    public void SignOut()
    {
        SignOutAsync().GetAwaiter().GetResult();
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
        if (Account is null)
        {
            await _pca.RemoveAsync(Account);
        }
    }


    public async Task<AuthenticationResult?> GetTokenSilentAsync()
    {
        // Try to acquire token silently.
        try
        {
            if (Account is null)
            {
                return null;
            }

            return await _pca
                         .AcquireTokenSilent(_scopes, Account)
                         .ExecuteAsync();
        }
        catch (MsalUiRequiredException)
        {
            Account = null;
            throw;
        }
    }

    ~AadAuthSession()
    {
        Dispose();
    }
}

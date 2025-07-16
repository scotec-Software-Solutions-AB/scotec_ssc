using Azure.Core;
using Microsoft.Identity.Client;

namespace Scotec.Identity.AzureActiveDirectory;

/// <summary>
///     Provides authentication services using MSAL for acquiring Azure AD tokens in desktop applications.
/// </summary>
/// <remarks>
///     This service encapsulates the MSAL public client application and exposes methods for acquiring and managing tokens,
///     including silent and interactive flows, as well as sign-out functionality. It also exposes a
///     <see cref="TokenCredential" />
///     for use with Azure SDK clients.
/// </remarks>
internal sealed class AuthService
{
    private readonly IPublicClientApplication _pca;
    private readonly string[] _scopes;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AuthService" /> class.
    /// </summary>
    /// <param name="clientId">The Azure AD application (client) ID.</param>
    /// <param name="tenantId">The Azure AD tenant ID.</param>
    /// <param name="scopes">The scopes to request for the token.</param>
    /// <param name="tokenCache">Optional. A <see cref="TokenCache" /> instance for persistent token caching.</param>
    /// <remarks>
    ///     The constructor configures the MSAL public client application and enables token caching if a
    ///     <paramref name="tokenCache" /> is provided.
    /// </remarks>
    public AuthService(string clientId, string tenantId, string[] scopes, TokenCache? tokenCache = null)
    {
        _scopes = scopes;

        _pca = PublicClientApplicationBuilder
               .Create(clientId)
               .WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
               //.WithWindowsDesktopFeatures(new BrokerOptions(BrokerOptions.OperatingSystems.Windows){Title = "XXXXXXXXX"})
               .WithDefaultRedirectUri()
               .Build();

        tokenCache?.Enable(_pca.UserTokenCache);
        TokenCredential = new MsalTokenCredential(_pca, _scopes);
    }

    /// <summary>
    ///     Gets the <see cref="TokenCredential" /> for use with Azure SDK clients.
    /// </summary>
    /// <remarks>
    ///     This credential uses MSAL to acquire tokens for the configured scopes.
    /// </remarks>
    public TokenCredential TokenCredential { get; }

    /// <summary>
    ///     Acquires an authentication token for the configured scopes.
    /// </summary>
    /// <returns>
    ///     An <see cref="AuthenticationResult" /> containing the access token and related information.
    /// </returns>
    /// <remarks>
    ///     Attempts to acquire a token silently using cached accounts. If user interaction is required,
    ///     falls back to an interactive prompt.
    /// </remarks>
    public async Task<AuthenticationResult> GetTokenAsync()
    {
        try
        {
            // Try to acquire token silently first
            return await _pca
                         .AcquireTokenSilent(_scopes, (await _pca.GetAccountsAsync()).FirstOrDefault())
                         .ExecuteAsync();
        }
        catch (MsalUiRequiredException)
        {
            // If silent acquisition fails, fall back to interactive acquisition
            return await _pca.AcquireTokenInteractive(_scopes)
                             .WithPrompt(Prompt.SelectAccount)
                             //.WithUseEmbeddedWebView(true)
                             .ExecuteAsync();
        }
    }

    /// <summary>
    ///     Signs out the user by removing all accounts from the token cache.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method removes all cached accounts, effectively signing out the user from the application.
    /// </remarks>
    public async Task SignOutAsync()
    {
        // Get all accounts in the cache
        var accounts = await _pca.GetAccountsAsync();

        // Remove each account
        foreach (var account in accounts)
        {
            await _pca.RemoveAsync(account);
        }
    }
}

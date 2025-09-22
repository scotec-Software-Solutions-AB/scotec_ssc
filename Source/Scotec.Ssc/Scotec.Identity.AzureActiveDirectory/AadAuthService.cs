using System.Collections.Concurrent;
using Azure.Core;
using Microsoft.Identity.Client;

namespace Scotec.Identity.AzureActiveDirectory;

/// <summary>
///     Provides authentication services using MSAL for acquiring Azure AD tokens in desktop applications.
/// </summary>
/// <remarks>
///     This service encapsulates the MSAL public client application and exposes methods for acquiring and managing tokens,
///     including silent and interactive flows, as well as sign-out functionality. It also exposes a
///     <see cref="TokenCredential" /> for use with Azure SDK clients.
/// </remarks>
public sealed class AadAuthService : IAadAuthService, IDisposable
{
    private readonly AadAuthOptions _options;
    private readonly IPublicClientApplication _pca;
    private readonly string[] _scopes;
    private readonly ConcurrentDictionary<IAccount, IAadAuthSession> _sessionCache = [];

    /// <summary>
    ///     Initializes a new instance of the <see cref="AadAuthService" /> class using explicit parameters.
    /// </summary>
    /// <param name="clientId">The Azure AD application (client) ID.</param>
    /// <param name="tenantId">The Azure AD tenant ID.</param>
    /// <param name="scopes">The scopes to request for the token.</param>
    /// <param name="tokenCache">Optional. A <see cref="TokenCache" /> instance for persistent token caching.</param>
    /// <remarks>
    ///     The constructor configures the MSAL public client application and enables token caching if a
    ///     <paramref name="tokenCache" /> is provided.
    /// </remarks>
    public AadAuthService(string clientId, string tenantId, string[] scopes, TokenCache? tokenCache = null)
        : this(new AadAuthOptions
        {
            ClientId = clientId,
            TenantId = tenantId,
            Scopes = scopes,
            TokenCache = tokenCache
        })
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AadAuthService" /> class using <see cref="AadAuthOptions"/>.
    /// </summary>
    /// <param name="options">The authentication options to use for configuration.</param>
    /// <remarks>
    ///     This constructor configures the MSAL public client application and enables token caching if specified in options.
    /// </remarks>
    public AadAuthService(AadAuthOptions options)
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

    /// <summary>
    ///     Determines whether a user is currently signed in.
    /// </summary>
    /// <param name="account">The account to check for an active session.</param>
    /// <returns><c>true</c> if the user is signed in; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     Returns <c>true</c> if there is a session for the specified account in the session cache.
    /// </remarks>
    public bool IsSignedIn(IAccount account)
    {
        return _sessionCache.ContainsKey(account);
    }

    /// <summary>
    ///     Acquires an authentication token for the configured scopes using a silent flow.
    /// </summary>
    /// <param name="account">The account for which to acquire the token.</param>
    /// <returns>
    ///     An <see cref="AuthenticationResult" /> containing the access token and related information.
    /// </returns>
    /// <remarks>
    ///     Attempts to acquire a token silently using cached accounts. If user interaction is required,
    ///     an exception is thrown and interactive authentication should be used.
    /// </remarks>
    public async Task<AuthenticationResult> GetTokenSilentAsync(IAccount account)
    {
        // Try to acquire token silently.
        try
        {
            return await _pca
                .AcquireTokenSilent(_scopes, account)
                .ExecuteAsync();
        }
        catch(MsalUiRequiredException)
        {
            _sessionCache.TryRemove(account, out _);
            throw;
        }
    }

    /// <summary>
    ///     Signs out the user by removing the account from the token cache.
    /// </summary>
    /// <param name="session">The authentication session to sign out.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method removes the specified account from the token cache, effectively signing out the user from the application.
    /// </remarks>
    public async Task SignOutAsync(IAadAuthSession session)
    {
        //TODO: Remove session from cache if needed.
        await _pca.RemoveAsync(session.Account);
    }

    /// <summary>
    ///     Signs in a user using the specified account, attempting silent authentication first.
    /// </summary>
    /// <param name="account">The account to sign in, or <c>null</c> to prompt for account selection.</param>
    /// <returns>An <see cref="AadAuthSession"/> representing the authenticated session.</returns>
    /// <remarks>
    ///     Attempts to acquire a token silently for the specified account. If silent authentication fails,
    ///     falls back to interactive authentication.
    /// </remarks>
    public Task<IAadAuthSession> SignInAsync(IAccount? account )
    {
        return SignInAsync(account, Prompt.NoPrompt);
    }

    public async Task<IAadAuthSession> SignInAsync(IAccount? account, Prompt prompt)
    {
        if (account is null)
        {
            return await SignInAsync();
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
                .WithPrompt(Prompt.NoPrompt)
                .WithAccount(account)
                .ExecuteAsync();
        }

        var signedIn = result.Account;
        if (!_sessionCache.TryGetValue(signedIn, out var session))
        {
            session = CreateSession(signedIn);
        }
        return session;
    }

    /// <summary>
    ///     Creates a new authentication session for the specified account.
    /// </summary>
    /// <param name="signedIn">The signed-in account.</param>
    /// <returns>An <see cref="AadAuthSession"/> for the account.</returns>
    /// <remarks>
    ///     This method creates and caches a new authentication session for the given account.
    /// </remarks>
    private AadAuthSession CreateSession(IAccount signedIn)
    {
        var tokenCredential = new MsalTokenCredential(this, signedIn);
        var session = new AadAuthSession(this, tokenCredential, signedIn)
        {
            AutoSignOut = _options.AutoSignOut
        };

        _sessionCache[signedIn] = session;
        return session;
    }

    /// <summary>
    ///     Signs in a user interactively, prompting for account selection.
    /// </summary>
    /// <returns>An <see cref="AadAuthSession"/> representing the authenticated session.</returns>
    /// <remarks>
    ///     This method always prompts the user to sign in, regardless of any cached accounts.
    /// </remarks>
    public async Task<IAadAuthSession> SignInAsync()
    {
        try
        {
            var result = await _pca.AcquireTokenInteractive(_scopes)
                                   .WithPrompt(Prompt.ForceLogin)
                                   .WithAccount(null) // No specific account, let user choose
                                   .ExecuteAsync();

            var signedIn = result.Account;
            if (!_sessionCache.TryGetValue(signedIn, out var session))
            {
                session = CreateSession(signedIn);
            }
            return session;
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    ///     Gets all accounts currently available in the token cache.
    /// </summary>
    /// <returns>An enumerable of <see cref="IAccount"/> objects.</returns>
    /// <remarks>
    ///     Returns all accounts that are present in the MSAL token cache.
    /// </remarks>
    public async Task<IEnumerable<IAccount>> GetAccountsAsync()
    {
        return await _pca.GetAccountsAsync();
    }

    /// <summary>
    ///     Disposes the authentication service and releases resources.
    /// </summary>
    /// <remarks>
    ///     If <see cref="AadAuthOptions.AutoSignOut"/> is enabled, this method signs out all active sessions.
    /// </remarks>
    public void Dispose()
    {
        DisposeAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    ///     Asynchronously disposes the authentication service and releases resources.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous dispose operation.</returns>
    /// <remarks>
    ///     If <see cref="AadAuthOptions.AutoSignOut"/> is enabled, this method signs out all active sessions asynchronously.
    /// </remarks>
    public async Task DisposeAsync()
    {
        if (_options.AutoSignOut)
        {
            foreach (var session in _sessionCache)
            {
                await SignOutAsync(session.Value);
            }
        }
    }
}
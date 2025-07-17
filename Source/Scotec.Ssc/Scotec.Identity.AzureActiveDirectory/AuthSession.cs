using Azure.Core;
using Microsoft.Identity.Client;

namespace Scotec.Identity.AzureActiveDirectory;

/// <summary>
///     Manages an Azure Active Directory authentication session, including token acquisition, refresh, and sign-in/sign-out operations.
/// </summary>
/// <remarks>
///     This class provides a high-level abstraction for handling Azure AD authentication in desktop applications.
///     It leverages MSAL for token management, supports both synchronous and asynchronous sign-in/sign-out flows,
///     and exposes a <see cref="TokenCredential"/> for use with Azure SDK clients. Optionally, it can automatically
///     sign out the user when disposed, ensuring security and proper resource cleanup.
/// </remarks>
public sealed class AadAuthSession : IDisposable
{
    private readonly AuthService _authService;

    /// <summary>
    ///     Initializes a new <see cref="AadAuthSession"/> with the specified authentication options.
    /// </summary>
    /// <param name="options">Authentication options containing client ID, tenant ID, scopes, and optional token cache.</param>
    /// <remarks>
    ///     This constructor allows for flexible configuration of the authentication session, including persistent token caching
    ///     via the <see cref="TokenCache"/>. It is recommended to use this overload when advanced options are required.
    /// </remarks>
    public AadAuthSession(AuthOptions options)
    {
        _authService = new AuthService(options.ClientId, options.TenantId, options.Scopes, options.TokenCache);
    }

    /// <summary>
    ///     Initializes a new <see cref="AadAuthSession"/> with explicit client ID, tenant ID, and scopes.
    /// </summary>
    /// <param name="clientId">The Azure AD application (client) ID.</param>
    /// <param name="tenantId">The Azure AD tenant ID.</param>
    /// <param name="scopes">The scopes required for authentication.</param>
    /// <remarks>
    ///     This constructor is a convenience overload for scenarios where only basic authentication parameters are needed.
    ///     It does not enable persistent token caching unless configured separately.
    /// </remarks>
    public AadAuthSession(string clientId, string tenantId, string[] scopes)
        : this(new AuthOptions { ClientId = clientId, TenantId = tenantId, Scopes = scopes })
    {
    }

    /// <summary>
    ///     Gets or sets a value indicating whether to automatically sign out when the session is disposed.
    /// </summary>
    /// <remarks>
    ///     When set to <c>true</c>, the session will sign out the user and clear cached tokens upon disposal,
    ///     helping to prevent unauthorized access and ensuring session termination.
    /// </remarks>
    public bool AutoSignOut { get; set; }

    /// <summary>
    ///     Gets the <see cref="TokenCredential"/> for Azure SDK authentication.
    /// </summary>
    /// <remarks>
    ///     This property exposes a credential compatible with Azure SDK clients, allowing seamless integration
    ///     with Azure services that require authentication.
    /// </remarks>
    public TokenCredential TokenCredential => _authService.TokenCredential;

    private AuthenticationResult? MsalResult { get; set; }

    /// <summary>
    ///     Disposes the session and releases resources.
    ///     Signs out from Azure AD if <see cref="AutoSignOut"/> is enabled.
    /// </summary>
    /// <remarks>
    ///     This method synchronously disposes the session. If <see cref="AutoSignOut"/> is set, it will also
    ///     sign out the user and clear cached tokens, ensuring proper cleanup.
    /// </remarks>
    public void Dispose()
    {
        DisposeAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    ///     Asynchronously disposes the session and releases resources.
    ///     Signs out from Azure AD if <see cref="AutoSignOut"/> is enabled.
    /// </summary>
    /// <returns>A task representing the asynchronous dispose operation.</returns>
    /// <remarks>
    ///     This method should be used in asynchronous scenarios to ensure that sign-out and resource cleanup
    ///     are performed without blocking the calling thread.
    /// </remarks>
    public async Task DisposeAsync()
    {
        if (AutoSignOut)
        {
            await _authService.SignOutAsync();
        }
    }

    /// <summary>
    ///     Signs out of the current Azure AD session synchronously.
    /// </summary>
    /// <remarks>
    ///     This method removes all cached accounts and tokens, effectively signing out the user from the application.
    ///     It is recommended to call this method before disposing the session if manual sign-out is required.
    /// </remarks>
    public void SignOut()
    {
        _authService.SignOutAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    ///     Asynchronously signs out of the current Azure AD session and clears the cached authentication result.
    /// </summary>
    /// <returns>A task representing the asynchronous sign-out operation.</returns>
    /// <remarks>
    ///     This method clears the cached authentication result and removes all accounts from the token cache,
    ///     ensuring the user is fully signed out. Use this in asynchronous workflows for non-blocking sign-out.
    /// </remarks>
    public async Task SignOutAsync()
    {
        MsalResult = null;
        await _authService.SignOutAsync();
    }

    /// <summary>
    ///     Signs in to Azure AD synchronously and acquires a token.
    /// </summary>
    /// <remarks>
    ///     This method initiates the sign-in process, acquiring an authentication token for the configured scopes.
    ///     It blocks until the operation completes. Use <see cref="SignInAsync"/> for asynchronous scenarios.
    /// </remarks>
    public void SignIn()
    {
        SignInAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    ///     Asynchronously signs in to Azure AD and acquires a token.
    /// </summary>
    /// <returns>A task representing the asynchronous sign-in operation.</returns>
    /// <remarks>
    ///     This method attempts to acquire a token interactively, prompting the user if necessary.
    ///     The result is cached for subsequent silent authentication attempts.
    /// </remarks>
    public async Task SignInAsync()
    {
        try
        {
            MsalResult = await _authService.GetTokenInteractiveAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    ///     Gets a value indicating whether the user is currently signed in.
    /// </summary>
    /// <remarks>
    ///     This property checks if a valid token can be acquired silently, indicating an active authenticated session.
    ///     If silent token acquisition fails, the user is considered signed out.
    /// </remarks>
    public bool IsSignedIn => _authService.IsSignedIn;

    /// <summary>
    ///     Finalizer for <see cref="AadAuthSession"/>. Ensures resources are released and sign-out occurs if <see cref="AutoSignOut"/> is enabled.
    /// </summary>
    /// <remarks>
    ///     The finalizer calls <see cref="Dispose"/> to ensure that resources are cleaned up and the user is signed out
    ///     if <see cref="AutoSignOut"/> is set. This helps prevent token leakage and ensures session termination.
    /// </remarks>
    ~AadAuthSession()
    {
        Dispose();
    }
}

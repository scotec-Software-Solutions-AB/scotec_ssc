using Azure.Core;
using Microsoft.Identity.Client;
using Timer = System.Timers.Timer;

namespace Scotec.Identity.AzureActiveDirectory;

/// <summary>
///     Represents an Azure Active Directory authentication session for acquiring and refreshing tokens.
/// </summary>
/// <remarks>
///     This class manages the authentication lifecycle using MSAL, including token acquisition, automatic refresh, and
///     sign-in/sign-out operations.
///     It supports both synchronous and asynchronous patterns and can optionally sign out automatically upon disposal.
/// </remarks>
public sealed class AadAuthSession : IDisposable
{
    private readonly AuthService _authService;
    private readonly Timer _tokenTimer;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AadAuthSession" /> class using the specified authentication options.
    /// </summary>
    /// <param name="options">The authentication options containing client ID, tenant ID, scopes, and optional token cache.</param>
    /// <remarks>
    ///     Sets up the authentication service and configures a timer to refresh the token 5 minutes before it expires.
    /// </remarks>
    public AadAuthSession(AuthOptions options)
    {
        _authService = new AuthService(options.ClientId, options.TenantId, options.Scopes, options.TokenCache);

        // 1-minute polling timer
        _tokenTimer = new Timer { AutoReset = true };
        _tokenTimer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
        _tokenTimer.Elapsed += async (sender, e) =>
        {
            if (MsalResult != null &&
                DateTime.UtcNow >= MsalResult.ExpiresOn.UtcDateTime.AddSeconds(-options.TokenRefreshTime))
            {
                try
                {
                    MsalResult = await _authService.GetTokenAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error refreshing token: {ex.Message}");
                }
            }
        };
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AadAuthSession" /> class using explicit client ID, tenant ID, and
    ///     scopes.
    /// </summary>
    /// <param name="clientId">The client (application) ID.</param>
    /// <param name="tenantId">The Azure Active Directory tenant ID.</param>
    /// <param name="scopes">The scopes required for authentication.</param>
    /// <remarks>
    ///     This constructor creates an <see cref="AuthOptions" /> object internally.
    /// </remarks>
    public AadAuthSession(string clientId, string tenantId, string[] scopes)
        : this(new AuthOptions { ClientId = clientId, TenantId = tenantId, Scopes = scopes, TokenRefreshTime = 300 })
    {
    }

    /// <summary>
    ///     Gets or sets a value indicating whether to automatically sign out when the session is disposed.
    /// </summary>
    /// <remarks>
    ///     If set to <c>true</c>, the session will sign out from Azure AD when disposed.
    /// </remarks>
    public bool AutoSignOut { get; set; }

    /// <summary>
    ///     Gets the <see cref="TokenCredential" /> used for Azure SDK authentication.
    /// </summary>
    /// <remarks>
    ///     This property exposes the credential for use with Azure SDK clients.
    /// </remarks>
    public TokenCredential TokenCredential => _authService.TokenCredential;

    private AuthenticationResult? MsalResult { get; set; }

    /// <summary>
    ///     Disposes the session and releases resources.
    /// </summary>
    /// <remarks>
    ///     If <see cref="AutoSignOut" /> is enabled, this method will also sign out from Azure AD.
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
    ///     Stops the token refresh timer and, if <see cref="AutoSignOut" /> is enabled, signs out from Azure AD.
    /// </remarks>
    public async Task DisposeAsync()
    {
        _tokenTimer.Stop();
        _tokenTimer.Dispose();

        if (AutoSignOut)
        {
            await _authService.SignOutAsync();
        }
    }

    /// <summary>
    ///     Signs out of the current Azure AD session synchronously.
    /// </summary>
    /// <remarks>
    ///     This method blocks until sign-out is complete.
    /// </remarks>
    public void SignOut()
    {
        _authService.SignOutAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    ///     Asynchronously signs out of the current Azure AD session.
    /// </summary>
    /// <returns>A task representing the asynchronous sign-out operation.</returns>
    /// <remarks>
    ///     Stops the token refresh timer and clears the cached authentication result.
    /// </remarks>
    public async Task SignOutAsync()
    {
        _tokenTimer.Stop();
        MsalResult = null;
        await _authService.SignOutAsync();
    }

    /// <summary>
    ///     Signs in to Azure AD synchronously and acquires a token.
    /// </summary>
    /// <remarks>
    ///     This method blocks until sign-in is complete.
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
    ///     Acquires a new authentication token using the configured authentication service.
    /// </remarks>
    public async Task SignInAsync()
    {
        MsalResult = await _authService.GetTokenAsync();
    }

    /// <summary>
    ///     Finalizer for the <see cref="AadAuthSession" /> class.
    /// </summary>
    /// <remarks>
    ///     Ensures that resources are released and sign-out occurs if <see cref="AutoSignOut" /> is enabled.
    /// </remarks>
    ~AadAuthSession()
    {
        Dispose();
    }
}

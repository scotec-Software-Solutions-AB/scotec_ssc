using Azure.Core;
using Microsoft.Identity.Client;

namespace Scotec.Identity.AzureActiveDirectory;

/// <summary>
///     Represents an Azure Active Directory authentication session, managing token acquisition, refresh, and sign-in/sign-out operations.
/// </summary>
/// <remarks>
///     Provides a high-level abstraction for Azure AD authentication in desktop applications. Utilizes MSAL for token management,
///     supports both synchronous and asynchronous sign-in/sign-out flows, and exposes a <see cref="TokenCredential"/> for Azure SDK clients.
///     Optionally, can automatically sign out the user when disposed to ensure security and resource cleanup.
/// </remarks>
public sealed class AadAuthSession : IDisposable
{
    private readonly AadAuthService _authService;
    private readonly IAccount _account;

    internal AadAuthSession(AadAuthService authService, TokenCredential tokenCredential, IAccount account)
    {
        _authService = authService;
        TokenCredential = tokenCredential;
        _account = account;
    }

    /// <summary>
    ///     Gets the Azure AD account associated with this session.
    /// </summary>
    /// <remarks>
    ///     The account represents the signed-in user for this authentication session.
    /// </remarks>
    public IAccount Account => _account;

    /// <summary>
    ///     Gets or sets a value indicating whether to automatically sign out when the session is disposed.
    /// </summary>
    /// <remarks>
    ///     If set to <c>true</c>, the session will sign out the user and clear cached tokens upon disposal.
    ///     This helps prevent unauthorized access and ensures session termination.
    /// </remarks>
    public bool AutoSignOut { get; set; }

    /// <summary>
    ///     Gets the <see cref="TokenCredential"/> for Azure SDK authentication.
    /// </summary>
    /// <remarks>
    ///     Exposes a credential compatible with Azure SDK clients, allowing integration with Azure services that require authentication.
    /// </remarks>
    public TokenCredential TokenCredential { get; }

    /// <summary>
    ///     Disposes the session and releases resources.
    /// </summary>
    /// <remarks>
    ///     If <see cref="AutoSignOut"/> is enabled, this method will sign out the user and clear cached tokens.
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
    ///     If <see cref="AutoSignOut"/> is enabled, this method will sign out the user and clear cached tokens asynchronously.
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
    /// <param name="account">The account to sign out. (Parameter is ignored; the session's account is used.)</param>
    /// <remarks>
    ///     Removes all cached accounts and tokens, effectively signing out the user from the application.
    ///     This method blocks until the operation is complete.
    /// </remarks>
    public void SignOut(IAccount account)
    {
        SignOutAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    ///     Asynchronously signs out of the current Azure AD session and clears the cached authentication result.
    /// </summary>
    /// <returns>A task representing the asynchronous sign-out operation.</returns>
    /// <remarks>
    ///     Clears the cached authentication result and removes all accounts from the token cache, ensuring the user is fully signed out.
    ///     Use this method in asynchronous workflows for non-blocking sign-out.
    /// </remarks>
    public async Task SignOutAsync()
    {
        await _authService.SignOutAsync(this);
    }

    /// <summary>
    ///     Gets a value indicating whether the user is currently signed in.
    /// </summary>
    /// <remarks>
    ///     Returns <c>true</c> if a valid token can be acquired silently for the account, indicating an active authenticated session.
    ///     If silent token acquisition fails, the user is considered signed out.
    /// </remarks>
    public bool IsSignedIn => _authService.IsSignedIn(_account);

    /// <summary>
    ///     Finalizer for <see cref="AadAuthSession"/>. Ensures resources are released and sign-out occurs if <see cref="AutoSignOut"/> is enabled.
    /// </summary>
    /// <remarks>
    ///     Invokes <see cref="Dispose"/> to ensure that resources are cleaned up and the user is signed out if <see cref="AutoSignOut"/> is set.
    ///     Helps prevent token leakage and ensures session termination.
    /// </remarks>
    ~AadAuthSession()
    {
        Dispose();
    }
}

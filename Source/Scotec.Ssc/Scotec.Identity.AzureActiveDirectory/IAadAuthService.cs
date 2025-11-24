using System.Diagnostics.CodeAnalysis;
using Microsoft.Identity.Client;

namespace Scotec.Identity.AzureActiveDirectory;

/// <summary>
///     Defines the contract for Azure Active Directory authentication services.
/// </summary>
/// <remarks>
///     This interface provides methods for managing authentication sessions, including interactive and silent sign-in flows.
///     It supports session retrieval and ensures proper resource management through synchronous and asynchronous disposal.
///     Implementations should handle session caching and token management for desktop applications using MSAL.
/// </remarks>
public interface IAadAuthService : IDisposable, IAsyncDisposable
{
    Task<IAadAuthSession> RegisterAppAsync(AadAuthOptions options, bool trySignIn, CancellationToken cancellationToken);


    /// <summary>
    ///     Attempts to retrieve an existing authentication session for the specified tenant and client.
    /// </summary>
    /// <param name="tenantId">The Azure AD tenant ID.</param>
    /// <param name="clientId">The Azure AD client (application) ID.</param>
    /// <param name="session">
    ///     When this method returns, contains the session associated with the specified tenant and client, if found; otherwise, <c>null</c>.
    /// </param>
    /// <returns>
    ///     <c>true</c> if a session exists for the specified tenant and client; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    ///     Use this method to check if a session is already available before attempting to sign in or create a new session.
    ///     This helps avoid redundant authentication flows and improves performance by reusing existing sessions.
    /// </remarks>
    public bool TryGetSession(string tenantId, string clientId, [NotNullWhen(true)] out IAadAuthSession? session);

    /// <summary>
    ///     Signs in a user interactively using the provided authentication options.
    /// </summary>
    /// <param name="options">The authentication options containing tenant ID, client ID, scopes, and cache settings.</param>
    /// <param name="configure">An optional delegate to further configure the interactive parameter builder.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A <see cref="Task{IAadAuthSession}"/> representing the asynchronous operation, with the authenticated session if successful; otherwise, <c>null</c>.
    /// </returns>
    /// <remarks>
    ///     This method initiates an interactive sign-in flow for the specified tenant and client.
    ///     If no session exists, a new session is created and stored.
    ///     The <paramref name="configure"/> parameter allows customization of the interactive sign-in experience.
    ///     Use this method when user interaction is required for authentication.
    /// </remarks>
    public Task<IAadAuthSession?> SignInAsync(
        AadAuthOptions options,
        Func<AcquireTokenInteractiveParameterBuilder, AcquireTokenInteractiveParameterBuilder>? configure,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Signs in a user silently using the provided authentication options.
    /// </summary>
    /// <param name="options">The authentication options containing tenant ID, client ID, scopes, and cache settings.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A <see cref="Task{IAadAuthSession}"/> representing the asynchronous operation, with the authenticated session if successful; otherwise, <c>null</c>.
    /// </returns>
    /// <remarks>
    ///     This method attempts to acquire a token silently for the specified tenant and client.
    ///     If no session exists, a new session is created and stored.
    ///     If silent authentication fails, the returned session may not be authenticated.
    ///     Use this method to avoid user interaction when a valid token is available in the cache.
    /// </remarks>
    public Task<IAadAuthSession?> SignInSilentAsync(AadAuthOptions options, CancellationToken cancellationToken);
}

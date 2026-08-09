using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Identity.Client;

namespace Scotec.Identity.AzureActiveDirectory;

/// <summary>
///     Provides authentication services using MSAL for acquiring Azure AD tokens in desktop applications.
/// </summary>
/// <remarks>
///     This service manages authentication sessions for multiple Azure AD tenants and clients.
///     It encapsulates session creation, token acquisition (silent and interactive), and session lifecycle management.
///     The service also supports both synchronous and asynchronous disposal of sessions.
/// </remarks>
public sealed class AadAuthService : IAadAuthService
{
    // Use ConcurrentDictionary for thread-safe session management
    private readonly ConcurrentDictionary<string, IAadAuthSession> _sessions = new();

    public async Task<IAadAuthSession> RegisterAppAsync(AadAuthOptions options, bool trySignIn, CancellationToken cancellationToken)
    {
        var key = CreateKey(options.TenantId, options.ClientId);

        // Atomically get or add the session
        var session = _sessions.GetOrAdd(key, _ => new AadAuthSession(options));

        if (trySignIn && !session.IsSignedIn)
        {
            await session.SignInSilentAsync(cancellationToken);
        }

        return session;
    }

    /// <summary>
    ///     Attempts to retrieve an existing authentication session for the specified tenant and client.
    /// </summary>
    /// <param name="tenantId">The Azure AD tenant ID.</param>
    /// <param name="clientId">The Azure AD client (application) ID.</param>
    /// <param name="session">
    ///     When this method returns, contains the session associated with the specified tenant and client,
    ///     if found; otherwise, <c>null</c>.
    /// </param>
    /// <returns><c>true</c> if a session exists for the specified tenant and client; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     Use this method to check if a session is already available before attempting to sign in or create a new session.
    /// </remarks>
    public bool TryGetSession(Guid tenantId, Guid clientId, [NotNullWhen(true)] out IAadAuthSession? session)
    {
        var key = CreateKey(tenantId, clientId);        
        return _sessions.TryGetValue(key, out session);
    }

    /// <summary>
    ///     Signs in a user silently using the provided authentication options.
    /// </summary>
    /// <param name="options">The authentication options containing tenant ID, client ID, scopes, and cache settings.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A <see cref="Task{IAadAuthSession}" /> representing the asynchronous operation, with the authenticated session if
    ///     successful; otherwise, <c>null</c>.
    /// </returns>
    /// <remarks>
    ///     This method attempts to acquire a token silently for the specified tenant and client.
    ///     If no session exists, a new session is created and stored.
    ///     If silent authentication fails, the returned session may not be authenticated.
    /// </remarks>
    public async Task<IAadAuthSession?> SignInSilentAsync(AadAuthOptions options, CancellationToken cancellationToken)
    {
        var key = CreateKey(options.TenantId, options.ClientId);

        if (!_sessions.TryGetValue(key, out var session))
        {
            session = await RegisterAppAsync(options, true, cancellationToken);
        }

        return session;
    }

    /// <summary>
    ///     Signs in a user interactively using the provided authentication options.
    /// </summary>
    /// <param name="options">The authentication options containing tenant ID, client ID, scopes, and cache settings.</param>
    /// <param name="configure">An optional delegate to further configure the interactive parameter builder.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A <see cref="Task{IAadAuthSession}" /> representing the asynchronous operation, with the authenticated session if
    ///     successful; otherwise, <c>null</c>.
    /// </returns>
    /// <remarks>
    ///     This method initiates an interactive sign-in flow for the specified tenant and client.
    ///     If no session exists, a new session is created and stored.
    ///     The <paramref name="configure" /> parameter allows customization of the interactive sign-in experience.
    /// </remarks>
    public async Task<IAadAuthSession?> SignInAsync(
        AadAuthOptions options,
        Func<AcquireTokenInteractiveParameterBuilder, AcquireTokenInteractiveParameterBuilder>? configure = null,
        CancellationToken cancellationToken = default)
    {
        var key = CreateKey(options.TenantId, options.ClientId);

        if (!_sessions.TryGetValue(key, out var session))
        {
            session = await RegisterAppAsync(options, false, cancellationToken);
        }

        await session.SignInAsync(configure, cancellationToken);
        return session;
    }

    /// <summary>
    ///     Disposes all authentication sessions managed by this service.
    /// </summary>
    /// <remarks>
    ///     This method synchronously disposes each session, releasing resources and optionally signing out users if
    ///     configured.
    ///     Prefer calling <see cref="DisposeAsync" /> for asynchronous disposal in non-blocking scenarios.
    /// </remarks>
    public void Dispose()
    {
        foreach (var session in _sessions.Values)
        {
            session.Dispose();
        }
    }

    /// <summary>
    ///     Asynchronously disposes all authentication sessions managed by this service.
    /// </summary>
    /// <returns>A <see cref="ValueTask" /> representing the asynchronous dispose operation.</returns>
    /// <remarks>
    ///     This method asynchronously disposes each session, releasing resources and optionally signing out users if
    ///     configured.
    ///     Use this method in asynchronous workflows to avoid blocking the calling thread.
    /// </remarks>
    public async ValueTask DisposeAsync()
    {
        foreach (var session in _sessions.Values)
        {
            await session.DisposeAsync();
        }
    }

    /// <summary>
    ///     Creates a unique key for identifying authentication sessions based on tenant and client IDs.
    /// </summary>
    /// <param name="tenantId">The Azure AD tenant ID.</param>
    /// <param name="clientId">The Azure AD client (application) ID.</param>
    /// <returns>A string key in the format <c>{tenantId}/{clientId}</c>.</returns>
    /// <remarks>
    ///     This key is used internally to store and retrieve sessions in the session dictionary.
    /// </remarks>
    private static string CreateKey(Guid tenantId, Guid clientId)
    {
        var key = $"{tenantId}/{clientId}";
        return key;
    }
}

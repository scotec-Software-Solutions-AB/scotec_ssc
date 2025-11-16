using System.Reflection.Emit;
using Azure.Core;
using Microsoft.Identity.Client;

namespace Scotec.Identity.AzureActiveDirectory;

/// <summary>
///     Defines the contract for an Azure Active Directory authentication session, managing token acquisition, refresh, and sign-in/sign-out operations.
/// </summary>
/// <remarks>
///     This interface provides a high-level abstraction for Azure AD authentication in desktop applications.
///     It supports both synchronous and asynchronous sign-in/sign-out flows, token management, and exposes a <see cref="TokenCredential"/> for Azure SDK clients.
///     Implementations may optionally sign out the user when disposed to ensure security and resource cleanup.
/// </remarks>
public interface IAadAuthSession : IDisposable, IAsyncDisposable
{
    event Func<IAccount, Task> SignedOut;
    event Func<IAccount, Task> SignedIn;

    /// <summary>
    ///     Gets the Azure AD account associated with this session.
    /// </summary>
    /// <remarks>
    ///     The account represents the signed-in user for this authentication session.
    /// </remarks>
    IAccount? Account { get; }

    /// <summary>
    ///     Gets or sets a value indicating whether to automatically sign out when the session is disposed.
    /// </summary>
    /// <remarks>
    ///     If set to <c>true</c>, the session will sign out the user and clear cached tokens upon disposal.
    ///     This helps prevent unauthorized access and ensures session termination.
    /// </remarks>
    bool AutoSignOut { get; set; }

    /// <summary>
    ///     Signs out of the current Azure AD session synchronously.
    /// </summary>
    /// <remarks>
    ///     Removes all cached accounts and tokens, effectively signing out the user from the application.
    ///     This method blocks until the operation is complete.
    /// </remarks>
    void SignOut();

    /// <summary>
    ///     Asynchronously signs out of the current Azure AD session and clears the cached authentication result.
    /// </summary>
    /// <returns>A task representing the asynchronous sign-out operation.</returns>
    /// <remarks>
    ///     Clears the cached authentication result and removes all accounts from the token cache, ensuring the user is fully signed out.
    ///     Use this method in asynchronous workflows for non-blocking sign-out.
    /// </remarks>
    Task SignOutAsync();

    /// <summary>
    ///     Determines whether a user is currently signed in.
    /// </summary>
    /// <remarks>
    ///     Returns <c>true</c> if the user is signed in; otherwise, <c>false</c>.
    ///     This property can be used to check authentication state before performing operations that require a valid session.
    /// </remarks>
    bool IsSignedIn { get; }

    /// <summary>
    ///     Gets an authentication token for the current account using silent authentication.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A <see cref="Task{AuthenticationResult}"/> representing the asynchronous operation, with the authentication result.
    /// </returns>
    /// <remarks>
    ///     Attempts to acquire a token silently for the current account. If silent authentication fails, an exception may be thrown.
    /// </remarks>
    Task<AuthenticationResult?> GetTokenSilentAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Signs in a user using the specified account, attempting silent authentication first.
    /// </summary>
    /// <param name="account">The account to sign in, or <c>null</c> to prompt for account selection.</param>
    /// <param name="configure">An optional delegate to further configure the interactive parameter builder.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A <see cref="Task{AuthenticationResult}"/> representing the asynchronous operation, with the authentication result.
    /// </returns>
    /// <remarks>
    ///     Attempts silent authentication for the specified account. If silent authentication fails, interactive sign-in is performed.
    /// </remarks>
    Task<AuthenticationResult?> SignInAsync(IAccount account, Func<AcquireTokenInteractiveParameterBuilder, AcquireTokenInteractiveParameterBuilder>? configure, CancellationToken cancellationToken);

    /// <summary>
    ///     Signs in a user using the specified account and prompt behavior, attempting silent authentication first.
    /// </summary>
    /// <param name="account">The account to sign in, or <c>null</c> to prompt for account selection.</param>
    /// <param name="prompt">The prompt behavior for interactive sign-in.</param>
    /// <param name="configure">An optional delegate to further configure the interactive parameter builder.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A <see cref="Task{AuthenticationResult}"/> representing the asynchronous operation, with the authentication result.
    /// </returns>
    /// <remarks>
    ///     Attempts silent authentication for the specified account. If silent authentication fails, interactive sign-in is performed with the specified prompt.
    /// </remarks>
    Task<AuthenticationResult?> SignInAsync(IAccount account, Prompt prompt, Func<AcquireTokenInteractiveParameterBuilder, AcquireTokenInteractiveParameterBuilder>? configure, CancellationToken cancellationToken);

    /// <summary>
    ///     Signs in a user interactively, optionally configuring the interactive parameter builder.
    /// </summary>
    /// <param name="configure">An optional delegate to further configure the interactive parameter builder.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A <see cref="Task{AuthenticationResult}"/> representing the asynchronous operation, with the authentication result.
    /// </returns>
    /// <remarks>
    ///     Initiates an interactive sign-in flow, allowing the user to select an account and provide credentials.
    /// </remarks>
    Task<AuthenticationResult?> SignInAsync(Func<AcquireTokenInteractiveParameterBuilder, AcquireTokenInteractiveParameterBuilder>? configure, CancellationToken cancellationToken);

    /// <summary>
    ///     Signs in a user interactively with the specified prompt behavior, optionally configuring the interactive parameter builder.
    /// </summary>
    /// <param name="prompt">The prompt behavior for interactive sign-in.</param>
    /// <param name="configure">An optional delegate to further configure the interactive parameter builder.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A <see cref="Task{AuthenticationResult}"/> representing the asynchronous operation, with the authentication result.
    /// </returns>
    /// <remarks>
    ///     Initiates an interactive sign-in flow with the specified prompt, allowing the user to select an account and provide credentials.
    /// </remarks>
    Task<AuthenticationResult?> SignInAsync(Prompt prompt, Func<AcquireTokenInteractiveParameterBuilder, AcquireTokenInteractiveParameterBuilder>? configure, CancellationToken cancellationToken);

    /// <summary>
    ///     Signs in a user silently using the current account.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A <see cref="Task{AuthenticationResult}"/> representing the asynchronous operation, with the authentication result.
    /// </returns>
    /// <remarks>
    ///     Attempts to acquire a token silently for the current account. If silent authentication fails, <c>null</c> is returned.
    /// </remarks>
    Task<AuthenticationResult?> SignInSilentAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Signs in a user silently using the specified account.
    /// </summary>
    /// <param name="account">The account to sign in.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A <see cref="Task{AuthenticationResult}"/> representing the asynchronous operation, with the authentication result.
    /// </returns>
    /// <remarks>
    ///     Attempts to acquire a token silently for the specified account. If silent authentication fails, <c>null</c> is returned.
    /// </remarks>
    Task<AuthenticationResult?> SignInSilentAsync(IAccount account, CancellationToken cancellationToken);

    /// <summary>
    ///     Gets all accounts currently available in the token cache.
    /// </summary>
    /// <returns>An enumerable of <see cref="IAccount"/> objects.</returns>
    /// <remarks>
    ///     This method returns all accounts found in the token cache, including the operating system account if available.
    /// </remarks>
    Task<IEnumerable<IAccount>> GetAccountsAsync();

    /// <summary>
    ///     Gets a <see cref="TokenCredential"/> for use with Azure SDK clients.
    /// </summary>
    /// <returns>
    ///     A <see cref="TokenCredential"/> if the user is signed in; otherwise, <c>null</c>.
    /// </returns>
    /// <remarks>
    ///     This method provides a credential object for authenticating Azure SDK clients using the current session.
    /// </remarks>
    TokenCredential? GetTokenCredential();
}

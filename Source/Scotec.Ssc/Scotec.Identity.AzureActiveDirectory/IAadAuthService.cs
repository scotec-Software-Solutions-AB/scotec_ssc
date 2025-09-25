using Microsoft.Identity.Client;

namespace Scotec.Identity.AzureActiveDirectory;

/// <summary>
///     Defines the contract for Azure Active Directory authentication services.
/// </summary>
public interface IAadAuthService : IDisposable
{
    /// <summary>
    ///     Determines whether a user is currently signed in.
    /// </summary>
    /// <param name="account">The account to check for an active session.</param>
    /// <returns><c>true</c> if the user is signed in; otherwise, <c>false</c>.</returns>
    bool IsSignedIn(IAccount account);

    /// <summary>
    ///     Gets an authentication token for the specified account using silent authentication.
    /// </summary>
    /// <param name="account">The account for which to acquire the token.</param>
    /// <returns>
    ///     A <see cref="Task{AuthenticationResult}" /> representing the asynchronous operation, with the authentication
    ///     result.
    /// </returns>
    Task<AuthenticationResult> GetTokenSilentAsync(IAccount account);

    /// <summary>
    ///     Signs out the user by removing the account from the token cache.
    /// </summary>
    /// <param name="session">The authentication session to sign out.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    Task SignOutAsync(IAadAuthSession session);

    /// <summary>
    ///     Signs in a user using the specified account, attempting silent authentication first.
    /// </summary>
    /// <param name="account">The account to sign in, or <c>null</c> to prompt for account selection.</param>
    /// <returns>An <see cref="AadAuthSession" /> representing the authenticated session.</returns>
    Task<IAadAuthSession> SignInAsync(IAccount? account);

    Task<IAadAuthSession> SignInAsync(IAccount? account, Prompt prompt);

    Task<IAadAuthSession?> SignInSilentAsync(IAccount account);

    /// <summary>
    ///     Signs in a user interactively, prompting for account selection.
    /// </summary>
    /// <returns>An <see cref="AadAuthSession" /> representing the authenticated session.</returns>
    Task<IAadAuthSession> SignInAsync();

    Task<IAadAuthSession> SignInAsync(Prompt prompt);

    /// <summary>
    ///     Gets all accounts currently available in the token cache.
    /// </summary>
    /// <returns>An enumerable of <see cref="IAccount" /> objects.</returns>
    Task<IEnumerable<IAccount>> GetAccountsAsync();

    /// <summary>
    ///     Asynchronously disposes the authentication service and releases resources.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous dispose operation.</returns>
    Task DisposeAsync();
}

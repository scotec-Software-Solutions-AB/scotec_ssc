using Azure.Core;
using Microsoft.Identity.Client;

namespace Scotec.Identity.AzureActiveDirectory;

/// <summary>
///     Defines the contract for an Azure Active Directory authentication session, managing token acquisition, refresh, and sign-in/sign-out operations.
/// </summary>
public interface IAadAuthSession : IDisposable
{
    /// <summary>
    ///     Gets the Azure AD account associated with this session.
    /// </summary>
    IAccount? Account { get; }

    /// <summary>
    ///     Gets or sets a value indicating whether to automatically sign out when the session is disposed.
    /// </summary>
    bool AutoSignOut { get; set; }

    /// <summary>
    ///     Asynchronously disposes the session and releases resources.
    /// </summary>
    /// <returns>A task representing the asynchronous dispose operation.</returns>
    Task DisposeAsync();

    /// <summary>
    ///     Signs out of the current Azure AD session synchronously.
    /// </summary>
    void SignOut();

    /// <summary>
    ///     Asynchronously signs out of the current Azure AD session and clears the cached authentication result.
    /// </summary>
    /// <returns>A task representing the asynchronous sign-out operation.</returns>
    Task SignOutAsync();

    /// <summary>
    ///     Determines whether a user is currently signed in.
    /// </summary>
    /// <returns><c>true</c> if the user is signed in; otherwise, <c>false</c>.</returns>
    bool IsSignedIn { get; }

    /// <summary>
    ///     Gets an authentication token for the specified account using silent authentication.
    /// </summary>
    /// <returns>
    ///     A <see cref="Task{AuthenticationResult}" /> representing the asynchronous operation, with the authentication
    ///     result.
    /// </returns>
    Task<AuthenticationResult?> GetTokenSilentAsync();

    /// <summary>
    ///     Signs in a user using the specified account, attempting silent authentication first.
    /// </summary>
    /// <param name="account">The account to sign in, or <c>null</c> to prompt for account selection.</param>
    /// <returns>An <see cref="AadAuthSession" /> representing the authenticated session.</returns>
    Task<IAccount?> SignInAsync(IAccount account);

    Task<IAccount?> SignInAsync(IAccount account, Prompt prompt);

    Task<IAccount?> SignInAsync();

    Task<IAccount?> SignInAsync(Prompt prompt);
    
    Task<IAccount?> SignInSilentAsync();

    Task<IAccount?> SignInSilentAsync(IAccount account);

    /// <summary>
    ///     Gets all accounts currently available in the token cache.
    /// </summary>
    /// <returns>An enumerable of <see cref="IAccount" /> objects.</returns>
    Task<IEnumerable<IAccount>> GetAccountsAsync();

    public TokenCredential? GetTokenCredential();
}

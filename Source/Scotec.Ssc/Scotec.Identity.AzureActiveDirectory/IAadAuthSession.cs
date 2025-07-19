using Azure.Core;
using Microsoft.Identity.Client;
using System;
using System.Threading.Tasks;

namespace Scotec.Identity.AzureActiveDirectory;

/// <summary>
///     Defines the contract for an Azure Active Directory authentication session, managing token acquisition, refresh, and sign-in/sign-out operations.
/// </summary>
public interface IAadAuthSession : IDisposable
{
    /// <summary>
    ///     Gets the Azure AD account associated with this session.
    /// </summary>
    IAccount Account { get; }

    /// <summary>
    ///     Gets or sets a value indicating whether to automatically sign out when the session is disposed.
    /// </summary>
    bool AutoSignOut { get; set; }

    /// <summary>
    ///     Gets the <see cref="TokenCredential"/> for Azure SDK authentication.
    /// </summary>
    TokenCredential TokenCredential { get; }

    /// <summary>
    ///     Asynchronously disposes the session and releases resources.
    /// </summary>
    /// <returns>A task representing the asynchronous dispose operation.</returns>
    Task DisposeAsync();

    /// <summary>
    ///     Signs out of the current Azure AD session synchronously.
    /// </summary>
    /// <param name="account">The account to sign out. (Parameter is ignored; the session's account is used.)</param>
    void SignOut(IAccount account);

    /// <summary>
    ///     Asynchronously signs out of the current Azure AD session and clears the cached authentication result.
    /// </summary>
    /// <returns>A task representing the asynchronous sign-out operation.</returns>
    Task SignOutAsync();

    /// <summary>
    ///     Gets a value indicating whether the user is currently signed in.
    /// </summary>
    bool IsSignedIn { get; }
}

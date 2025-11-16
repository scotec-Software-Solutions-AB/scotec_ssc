namespace Scotec.Identity.AzureActiveDirectory;

/// <summary>
///     Represents authentication options for Azure Active Directory authentication.
/// </summary>
/// <remarks>
///     Encapsulates parameters required for authenticating with Azure AD, including client and tenant IDs,
///     required scopes, and an optional persistent token cache. The <see cref="TokenCache" /> property enables
///     secure token persistence between sessions.
/// </remarks>
public class AadAuthOptions
{
    /// <summary>
    ///     Gets or sets the client (application) ID used for authentication.
    /// </summary>
    /// <remarks>
    ///     This value is required and should correspond to the registered application's client ID in Azure AD.
    /// </remarks>
    public required string ClientId { get; set; }

    /// <summary>
    ///     Gets or sets the tenant ID associated with the Azure Active Directory.
    /// </summary>
    /// <remarks>
    ///     This value is required and should match the Azure AD tenant where the application is registered.
    /// </remarks>
    public required string TenantId { get; set; }

    /// <summary>
    ///     Gets or sets the scopes required for authentication.
    /// </summary>
    /// <remarks>
    ///     This array should contain all the scopes that the application needs to request access to.
    /// </remarks>
    public required string[] Scopes { get; set; }

    /// <summary>
    ///     Gets or sets the token cache used to persist authentication tokens.
    /// </summary>
    /// <remarks>
    ///     Optional. If provided, enables secure, persistent token caching to improve authentication performance
    ///     and reduce redundant requests.
    /// </remarks>
    public TokenCache? TokenCache { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether automatic sign-out is enabled.
    /// </summary>
    /// <remarks>
    ///     When set to <c>true</c>, the authentication provider will automatically sign out the user
    ///     under certain conditions, such as token expiration or explicit sign-out requests. This helps
    ///     ensure that user sessions are properly managed and reduces the risk of unauthorized access
    ///     due to stale authentication tokens.
    /// </remarks>
    public bool AutoSignOut { get; set; }
}

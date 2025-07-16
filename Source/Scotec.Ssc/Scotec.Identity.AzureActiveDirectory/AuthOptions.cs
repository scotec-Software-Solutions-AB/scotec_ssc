namespace Scotec.Identity.AzureActiveDirectory;

/// <summary>
///     Represents authentication options for Azure Storage access.
/// </summary>
/// <remarks>
///     This class encapsulates the necessary parameters for authenticating with Azure services,
///     including client and tenant identifiers, required scopes, and an optional token cache.
///     The <see cref="TokenCache" /> property can be used to persist tokens between sessions.
/// </remarks>
public class AuthOptions
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
    ///     This property is optional. If provided, it enables token caching to improve authentication performance and reduce
    ///     redundant requests.
    /// </remarks>
    public TokenCache? TokenCache { get; set; }

    /// <summary>
    ///     Gets or sets the time in seconds before token expiration when a refresh should be attempted.
    /// </summary>
    /// <remarks>
    ///     This value determines how soon before the token's actual expiration the system should attempt to refresh the
    ///     authentication token.
    ///     A typical value might be 300 (5 minutes) to ensure seamless authentication without interruption.
    /// </remarks>
    public int TokenRefreshTime { get; set; }
}

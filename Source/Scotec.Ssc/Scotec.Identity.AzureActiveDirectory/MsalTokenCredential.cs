using Azure.Core;
using Microsoft.Identity.Client;

namespace Scotec.Identity.AzureActiveDirectory;

/// <summary>
///     Implements <see cref="TokenCredential" /> using MSAL to acquire tokens for Azure SDK authentication.
/// </summary>
/// <remarks>
///     This credential attempts to acquire a token silently using cached accounts. If silent acquisition fails,
///     interactive authentication should be performed by the caller.
/// </remarks>
internal sealed class MsalTokenCredential : TokenCredential
{
    private readonly AuthService _authService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MsalTokenCredential" /> class.
    /// </summary>
    /// <param name="authService">The authentication service used to acquire tokens.</param>
    internal MsalTokenCredential(AuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    ///     Synchronously acquires an access token for the specified request context.
    /// </summary>
    /// <param name="context">The token request context containing scopes and other parameters.</param>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>An <see cref="AccessToken" /> for the requested scopes.</returns>
    /// <remarks>
    ///     This method blocks the calling thread. Prefer <see cref="GetTokenAsync(TokenRequestContext, CancellationToken)" /> for asynchronous scenarios.
    /// </remarks>
    public override AccessToken GetToken(TokenRequestContext context, CancellationToken token)
    {
        return GetTokenAsync(context, token).GetAwaiter().GetResult();
    }

    /// <summary>
    ///     Asynchronously acquires an access token for the specified request context.
    /// </summary>
    /// <param name="context">The token request context containing scopes and other parameters.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>
    ///     A <see cref="ValueTask{AccessToken}" /> representing the asynchronous operation, containing the access token for the requested scopes.
    /// </returns>
    /// <remarks>
    ///     Attempts to acquire a token silently using cached accounts. If silent acquisition fails, interactive authentication should be performed by the caller.
    /// </remarks>
    public override async ValueTask<AccessToken> GetTokenAsync(TokenRequestContext context, CancellationToken cancellationToken)
    {
        var result = await _authService.GetTokenSilentAsync();

        return new AccessToken(result.AccessToken, result.ExpiresOn);
    }
}

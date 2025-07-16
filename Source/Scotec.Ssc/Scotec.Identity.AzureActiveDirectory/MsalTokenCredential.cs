using Azure.Core;
using Microsoft.Identity.Client;

namespace Scotec.Identity.AzureActiveDirectory;

/// <summary>
///     A <see cref="TokenCredential" /> implementation that uses MSAL to acquire tokens for Azure SDK authentication.
/// </summary>
/// <remarks>
///     This credential attempts to acquire a token silently using cached accounts. If user interaction is required,
///     it falls back to an interactive prompt.
/// </remarks>
internal sealed class MsalTokenCredential : TokenCredential
{
    private readonly IPublicClientApplication _pca;
    private readonly string[] _scopes;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MsalTokenCredential" /> class.
    /// </summary>
    /// <param name="pca">The MSAL public client application instance.</param>
    /// <param name="scopes">The scopes to request for the token.</param>
    /// <remarks>
    ///     The <paramref name="pca" /> should be properly configured for the target Azure AD application.
    /// </remarks>
    public MsalTokenCredential(IPublicClientApplication pca, string[] scopes)
    {
        _pca = pca;
        _scopes = scopes;
    }

    /// <summary>
    ///     Synchronously acquires an access token for the specified context.
    /// </summary>
    /// <param name="context">The token request context.</param>
    /// <param name="token">A cancellation token.</param>
    /// <returns>An <see cref="AccessToken" /> for the requested scopes.</returns>
    /// <remarks>
    ///     This method blocks the calling thread. Prefer <see cref="GetTokenAsync" /> for asynchronous scenarios.
    /// </remarks>
    public override AccessToken GetToken(TokenRequestContext context, CancellationToken token)
    {
        return GetTokenAsync(context, token).GetAwaiter().GetResult();
    }

    /// <summary>
    ///     Asynchronously acquires an access token for the specified context.
    /// </summary>
    /// <param name="context">The token request context.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="ValueTask{AccessToken}" /> representing the asynchronous operation.</returns>
    /// <remarks>
    ///     Attempts to acquire a token silently. If user interaction is required, an interactive prompt is shown.
    /// </remarks>
    public override async ValueTask<AccessToken> GetTokenAsync(
        TokenRequestContext context, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _pca
                               .AcquireTokenSilent(_scopes, (await _pca.GetAccountsAsync()).FirstOrDefault())
                               .ExecuteAsync(cancellationToken);
            return new AccessToken(result.AccessToken, result.ExpiresOn);
        }
        catch (MsalUiRequiredException)
        {
            var result = await _pca
                               .AcquireTokenInteractive(_scopes)
                               .WithPrompt(Prompt.SelectAccount)
                               .ExecuteAsync(cancellationToken);
            return new AccessToken(result.AccessToken, result.ExpiresOn);
        }
    }
}

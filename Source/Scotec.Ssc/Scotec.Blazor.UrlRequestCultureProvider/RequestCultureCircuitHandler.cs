using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.WebUtilities;

namespace Scotec.Blazor.UrlRequestCultureProvider;

public class RequestCultureCircuitHandler : CircuitHandler
{
    private readonly string _connectionToken;

    public RequestCultureCircuitHandler(IHttpContextAccessor httpContextAccessor)
    {
        var components = QueryHelpers.ParseQuery(httpContextAccessor.HttpContext!.Request.QueryString.Value);
        _connectionToken = components["id"]!;
    }

    public override async Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        await base.OnCircuitClosedAsync(circuit, cancellationToken);
        if (string.IsNullOrEmpty(_connectionToken)) ;
        {
            CultureByConnectionTokens.RemoveToken(_connectionToken);
        }
    }
}
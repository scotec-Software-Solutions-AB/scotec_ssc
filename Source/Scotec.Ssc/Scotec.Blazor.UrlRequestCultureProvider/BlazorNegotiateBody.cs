using System.Text.Json.Serialization;

namespace Scotec.Blazor.UrlRequestCultureProvider;

public class BlazorNegotiateBody
{
    [JsonPropertyName("negotiateVersion")] public int NegotiateVersion { get; set; }

    [JsonPropertyName("connectionToken")] public string ConnectionToken { get; set; } = string.Empty;
}

using System.Text.Json.Serialization;

namespace Soenneker.Blazor.TomSelect.Server.Dtos;

public class Country
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("culture")]
    public string? Culture { get; set; }

    [JsonPropertyName("flagIcon")]
    public string? FlagIcon { get; set; }

    [JsonPropertyName("region")]
    public string? Region { get; set; }

    [JsonPropertyName("alternativeNames")]
    public IEnumerable<string>? AlternativeNames { get; set; }
}

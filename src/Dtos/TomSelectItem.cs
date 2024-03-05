using System.Text.Json.Serialization;

namespace Soenneker.Blazor.TomSelect.Dtos;

public class TomSelectItem
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = default!;
}
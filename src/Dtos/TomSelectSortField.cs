using System.Text.Json.Serialization;

namespace Soenneker.Blazor.TomSelect.Dtos;

/// <summary>
/// Represents how items are sorted in the dropdown.
/// </summary>
public class TomSelectSortField
{
    [JsonPropertyName("field")]
    public string Field { get; set; }

    [JsonPropertyName("direction")]
    public string Direction { get; set; } = "asc"; // Assuming default sorting direction is ascending.
}
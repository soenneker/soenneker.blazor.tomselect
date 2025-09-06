using System.Text.Json.Serialization;

namespace Soenneker.Blazor.TomSelect.Dtos;

/// <summary>
/// Represents how items are sorted in the dropdown.
/// </summary>
public sealed class TomSelectSortField
{
    /// <summary>
    /// Gets or sets the field used for sorting.
    /// </summary>
    [JsonPropertyName("field")]
    public string? Field { get; set; }

    /// <summary>
    /// Gets or sets the sorting direction. The default value is "asc".
    /// </summary>
    [JsonPropertyName("direction")]
    public string Direction { get; set; } = "asc";
}

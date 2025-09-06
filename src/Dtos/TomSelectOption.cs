using System.Text.Json.Serialization;

namespace Soenneker.Blazor.TomSelect.Dtos;

/// <summary>
/// Represents a single option in the configuration.
/// </summary>
public sealed class TomSelectOption
{
    /// <summary>
    /// The value of the option.
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; set; } = null!;

    /// <summary>
    /// The display text of the option.
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; } = null!;

    /// <summary>
    /// The underlying item of the option.
    /// </summary>
    [JsonPropertyName("item")]
    public object? Item { get; set; }
}

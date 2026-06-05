using Soenneker.Gen.EnumValues;

namespace Soenneker.Blazor.TomSelect.Enums;

/// <summary>
/// Represents the add item type.
/// </summary>
[EnumValue<string>]
public sealed partial class AddItemType
{
    /// <summary>
    /// The normal.
    /// </summary>
    public static readonly AddItemType Normal = new(nameof(Normal));
    /// <summary>
    /// The new option.
    /// </summary>
    public static readonly AddItemType NewOption = new(nameof(NewOption));
    /// <summary>
    /// The error.
    /// </summary>
    public static readonly AddItemType Error = new(nameof(Error));
}
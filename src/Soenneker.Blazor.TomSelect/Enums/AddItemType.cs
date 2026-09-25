using Soenneker.Gen.EnumValues;

namespace Soenneker.Blazor.TomSelect.Enums;

/// <summary>
/// Represents the add item type.
/// </summary>
[EnumValue<string>]
public sealed partial class AddItemType
{
    // Prevent JSON source generation from assuming an implicit public constructor.
    private AddItemType() => throw new System.NotSupportedException("Use a declared enum value.");

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
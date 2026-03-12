using Soenneker.Gen.EnumValues;

namespace Soenneker.Blazor.TomSelect.Enums;

[EnumValue<string>]
public sealed partial class AddItemType
{
    public static readonly AddItemType Normal = new(nameof(Normal));
    public static readonly AddItemType NewOption = new(nameof(NewOption));
    public static readonly AddItemType Error = new(nameof(Error));
}

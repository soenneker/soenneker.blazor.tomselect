using Ardalis.SmartEnum;

namespace Soenneker.Blazor.TomSelect.Enums;

public sealed class AddItemType : SmartEnum<AddItemType>
{
    public static readonly AddItemType Normal = new(nameof(Normal), 0);
    public static readonly AddItemType NewItem = new(nameof(NewItem), 1);
    public static readonly AddItemType Error = new(nameof(Error), 2);

    private AddItemType(string name, int value) : base(name, value)
    {
    }
}
using Ardalis.SmartEnum;
using Soenneker.Extensions.String;

namespace Soenneker.Blazor.TomSelect.Enums;

public sealed class TomSelectPluginType : SmartEnum<TomSelectPluginType>
{
    public static readonly TomSelectPluginType CaretPosition = new(nameof(CaretPosition).ToSnakeCaseFromPascal(), 0);
    public static readonly TomSelectPluginType ChangeListener = new(nameof(ChangeListener).ToSnakeCaseFromPascal(), 1);
    public static readonly TomSelectPluginType CheckboxOptions = new(nameof(CheckboxOptions).ToSnakeCaseFromPascal(), 2);
    public static readonly TomSelectPluginType ClearButton = new(nameof(ClearButton).ToSnakeCaseFromPascal(), 3);
    public static readonly TomSelectPluginType DragDrop = new(nameof(DragDrop).ToSnakeCaseFromPascal(), 4);
    public static readonly TomSelectPluginType DropdownHeader = new(nameof(DropdownHeader).ToSnakeCaseFromPascal(), 5);
    public static readonly TomSelectPluginType DropdownInput = new(nameof(DropdownInput).ToSnakeCaseFromPascal(), 6);
    public static readonly TomSelectPluginType InputAutogrow = new(nameof(InputAutogrow).ToSnakeCaseFromPascal(), 7);
    public static readonly TomSelectPluginType NoActiveItems = new(nameof(NoActiveItems).ToSnakeCaseFromPascal(), 8);
    public static readonly TomSelectPluginType NoBackspaceDelete = new(nameof(NoBackspaceDelete).ToSnakeCaseFromPascal(), 9);
    public static readonly TomSelectPluginType OptionGroupColumns = new(nameof(OptionGroupColumns).ToSnakeCaseFromPascal(), 10);
    public static readonly TomSelectPluginType RemoveButton = new(nameof(RemoveButton).ToSnakeCaseFromPascal(), 11);
    public static readonly TomSelectPluginType RestoreOnBackspace = new(nameof(RestoreOnBackspace).ToSnakeCaseFromPascal(), 12);
    public static readonly TomSelectPluginType VirtualScroll = new(nameof(VirtualScroll).ToSnakeCaseFromPascal(), 13);

    private TomSelectPluginType(string name, int value) : base(name, value)
    {
    }
}
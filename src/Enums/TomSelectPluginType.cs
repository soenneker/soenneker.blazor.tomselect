using Intellenum;

namespace Soenneker.Blazor.TomSelect.Enums;

[Intellenum<string>]
public sealed partial class TomSelectPluginType
{
    public static readonly TomSelectPluginType CaretPosition = new("caret_position");
    public static readonly TomSelectPluginType ChangeListener = new("change_listener");
    public static readonly TomSelectPluginType CheckboxOptions = new("checkbox_options");
    public static readonly TomSelectPluginType ClearButton = new("clear_button");
    public static readonly TomSelectPluginType DragDrop = new("drag_drop");
    public static readonly TomSelectPluginType DropdownHeader = new("dropdown_header");
    public static readonly TomSelectPluginType DropdownInput = new("dropdown_input");
    public static readonly TomSelectPluginType InputAutogrow = new("input_autogrow");
    public static readonly TomSelectPluginType NoActiveItems = new("no_active_items");
    public static readonly TomSelectPluginType NoBackspaceDelete = new("no_backspace_delete");
    public static readonly TomSelectPluginType OptionGroupColumns = new("option_group_columns");
    public static readonly TomSelectPluginType RemoveButton = new("remove_button");
    public static readonly TomSelectPluginType RestoreOnBackspace = new("restore_on_backspace");
    public static readonly TomSelectPluginType VirtualScroll = new("virtual_scroll");
}

using Soenneker.Gen.EnumValues;

namespace Soenneker.Blazor.TomSelect.Enums;

/// <summary>
/// Represents the tom select plugin type.
/// </summary>
[EnumValue<string>]
public sealed partial class TomSelectPluginType
{
    /// <summary>
    /// The caret position.
    /// </summary>
    public static readonly TomSelectPluginType CaretPosition = new("caret_position");
    /// <summary>
    /// The change listener.
    /// </summary>
    public static readonly TomSelectPluginType ChangeListener = new("change_listener");
    /// <summary>
    /// The checkbox options.
    /// </summary>
    public static readonly TomSelectPluginType CheckboxOptions = new("checkbox_options");
    /// <summary>
    /// The clear button.
    /// </summary>
    public static readonly TomSelectPluginType ClearButton = new("clear_button");
    /// <summary>
    /// The drag drop.
    /// </summary>
    public static readonly TomSelectPluginType DragDrop = new("drag_drop");
    /// <summary>
    /// The dropdown header.
    /// </summary>
    public static readonly TomSelectPluginType DropdownHeader = new("dropdown_header");
    /// <summary>
    /// The dropdown input.
    /// </summary>
    public static readonly TomSelectPluginType DropdownInput = new("dropdown_input");
    /// <summary>
    /// The input autogrow.
    /// </summary>
    public static readonly TomSelectPluginType InputAutogrow = new("input_autogrow");
    /// <summary>
    /// The no active items.
    /// </summary>
    public static readonly TomSelectPluginType NoActiveItems = new("no_active_items");
    /// <summary>
    /// The no backspace delete.
    /// </summary>
    public static readonly TomSelectPluginType NoBackspaceDelete = new("no_backspace_delete");
    /// <summary>
    /// The option group columns.
    /// </summary>
    public static readonly TomSelectPluginType OptionGroupColumns = new("option_group_columns");
    /// <summary>
    /// The remove button.
    /// </summary>
    public static readonly TomSelectPluginType RemoveButton = new("remove_button");
    /// <summary>
    /// The restore on backspace.
    /// </summary>
    public static readonly TomSelectPluginType RestoreOnBackspace = new("restore_on_backspace");
    /// <summary>
    /// The virtual scroll.
    /// </summary>
    public static readonly TomSelectPluginType VirtualScroll = new("virtual_scroll");
}

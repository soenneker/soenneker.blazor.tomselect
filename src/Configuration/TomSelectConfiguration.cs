using System.Collections.Generic;
using System.Text.Json.Serialization;
using Soenneker.Blazor.TomSelect.Dtos;
using Soenneker.Blazor.TomSelect.Enums;

namespace Soenneker.Blazor.TomSelect.Configuration;

/// <summary>
/// Represents the general configuration for the control.
/// </summary>
public class TomSelectConfiguration
{
    /// <summary>
    /// An array of the initial selected values, populated from the original input element.
    /// </summary>
    [JsonPropertyName("items")]
    public List<string> Items { get; set; } = [];

    [JsonPropertyName("plugins")]
    public List<TomSelectPluginType>? Plugins { get; set; }

    /// <summary>
    /// Determines if the user is allowed to create new items that aren't in the initial list of options.
    /// This setting can be true or false.
    /// </summary>
    [JsonPropertyName("create")]
    public bool Create { get; set; } = false;

    /// <summary>
    /// If true, when user exits the field, a new option is created and selected if the create setting is enabled.
    /// </summary>
    [JsonPropertyName("createOnBlur")]
    public bool CreateOnBlur { get; set; } = false;

    /// <summary>
    /// Specifies a RegExp, a string containing a regular expression, or a predicate function that the current search filter must match to be allowed to be created.
    /// </summary>
    [JsonPropertyName("createFilter")]
    public string? CreateFilter { get; set; } = null;

    /// <summary>
    /// The string to separate items by.
    /// </summary>
    [JsonPropertyName("delimiter")]
    public string Delimiter { get; set; } = ",";

    /// <summary>
    /// Toggles match highlighting within the dropdown menu.
    /// </summary>
    [JsonPropertyName("highlight")]
    public bool Highlight { get; set; } = true;

    /// <summary>
    /// If false, items created by the user will not show up as available options once they are unselected.
    /// </summary>
    [JsonPropertyName("persist")]
    public bool Persist { get; set; } = true;

    /// <summary>
    /// Show the dropdown immediately when the control receives focus.
    /// </summary>
    [JsonPropertyName("openOnFocus")]
    public bool OpenOnFocus { get; set; } = true;

    /// <summary>
    /// The max number of options to display in the dropdown. Null for unlimited.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("maxOptions")]
    public int? MaxOptions { get; set; } = null;

    /// <summary>
    /// The max number of items the user can select. Null for unlimited.
    /// </summary>
    [JsonPropertyName("maxItems")]
    public int? MaxItems { get; set; } = null;

    /// <summary>
    /// If true, currently selected items will not be shown in the dropdown list of available options.
    /// </summary>
    [JsonPropertyName("hideSelected")]
    public bool? HideSelected { get; set; } = null;

    /// <summary>
    /// After a selection is made, the dropdown's behavior is controlled by this property.
    /// </summary>
    [JsonPropertyName("closeAfterSelect")]
    public bool? CloseAfterSelect { get; set; } = null;

    /// <summary>
    /// Treats options with a "" value like normal options.
    /// </summary>
    [JsonPropertyName("allowEmptyOption")]
    public bool AllowEmptyOption { get; set; } = false;

    /// <summary>
    /// The number of milliseconds to wait before requesting options from the server. Null disables throttling.
    /// </summary>
    [JsonPropertyName("loadThrottle")]
    public int? LoadThrottle { get; set; } = 300;

    /// <summary>
    /// The class name added to the wrapper element while awaiting the fulfillment of load requests.
    /// </summary>
    [JsonPropertyName("loadingClass")]
    public string LoadingClass { get; set; } = "loading";

    /// <summary>
    /// The placeholder of the control.
    /// </summary>
    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; set; } = null;

    /// <summary>
    /// Controls the visibility of the placeholder based on the control's state.
    /// </summary>
    [JsonPropertyName("hidePlaceholder")]
    public bool? HidePlaceholder { get; set; } = null;

    /// <summary>
    /// Preloads options upon control initialization or when control receives focus.
    /// </summary>
    [JsonPropertyName("preload")]
    public bool Preload { get; set; } = false;

    /// <summary>
    /// The element the dropdown menu is appended to. Null appends it as a child of the control.
    /// </summary>
    [JsonPropertyName("dropdownParent")]
    public string? DropdownParent { get; set; } = null;

    /// <summary>
    /// Gives precedence to the "Add..." option in the dropdown.
    /// </summary>
    [JsonPropertyName("addPrecedence")]
    public bool AddPrecedence { get; set; } = false;

    /// <summary>
    /// If true, the tab key will choose the currently selected item.
    /// </summary>
    [JsonPropertyName("selectOnTab")]
    public bool SelectOnTab { get; set; } = true;

    /// <summary>
    /// Enable or disable international character support.
    /// </summary>
    [JsonPropertyName("diacritics")]
    public bool Diacritics { get; set; } = true;

    /// <summary>
    /// Supply a custom input element. Null disables the default functionality.
    /// </summary>
    [JsonPropertyName("controlInput")]
    public string? ControlInput { get; set; } = null;

    /// <summary>
    /// Allow selecting the same option more than once.
    /// </summary>
    [JsonPropertyName("duplicates")]
    public bool Duplicates { get; set; } = false;

    /// <summary>
    /// Option groups that options will be bucketed into.
    /// </summary>
    [JsonPropertyName("optgroups")]
    public List<TomSelectOption>? OptGroups { get; set; }

    /// <summary>
    /// The option attribute from which to read JSON data about the option.
    /// </summary>
    [JsonPropertyName("dataAttr")]
    public string? DataAttr { get; set; } = null;

    /// <summary>
    /// The name of the property to use as the value when an item is selected.
    /// </summary>
    [JsonPropertyName("valueField")]
    public string ValueField { get; set; } = "value";

    /// <summary>
    /// The name of the option group property that serves as its unique identifier.
    /// </summary>
    [JsonPropertyName("optgroupValueField")]
    public string OptgroupValueField { get; set; } = "value";

    /// <summary>
    /// The name of the property to render as an option / item label.
    /// </summary>
    [JsonPropertyName("labelField")]
    public string LabelField { get; set; } = "text";

    /// <summary>
    /// The name of the property to render as an option group label.
    /// </summary>
    [JsonPropertyName("optgroupLabelField")]
    public string OptgroupLabelField { get; set; } = "label";

    /// <summary>
    /// The name of the property to group items by.
    /// </summary>
    [JsonPropertyName("optgroupField")]
    public string OptgroupField { get; set; } = "optgroup";

    /// <summary>
    /// The name of the property to disabled option and optgroup.
    /// </summary>
    [JsonPropertyName("disabledField")]
    public string DisabledField { get; set; } = "disabled";

    /// <summary>
    /// Defines how items are sorted in the dropdown. Can be a string, array, or function.
    /// </summary>
    [JsonPropertyName("sortField")]
    public List<TomSelectSortField> SortField { get; set; } =
    [
        new TomSelectSortField {Field = "$score"},
        new TomSelectSortField {Field = "$order"}
    ];

    /// <summary>
    /// An array of property names to analyze when filtering options.
    /// </summary>
    [JsonPropertyName("searchField")]
    public List<string> SearchField { get; set; } = [ "text" ];

    /// <summary>
    /// The operator used when searching for multiple terms.
    /// </summary>
    [JsonPropertyName("searchConjunction")]
    public string SearchConjunction { get; set; } = "and";

    /// <summary>
    /// If true, all optgroups will be displayed in the order they were added.
    /// </summary>
    [JsonPropertyName("lockOptgroupOrder")]
    public bool LockOptgroupOrder { get; set; } = false;

    /// <summary>
    /// Copy the original input classes to the dropdown element.
    /// </summary>
    [JsonPropertyName("copyClassesToDropdown")]
    public bool CopyClassesToDropdown { get; set; } = true;
}
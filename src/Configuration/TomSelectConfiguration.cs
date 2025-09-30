using System.Collections.Generic;
using System.Text.Json.Serialization;
using Soenneker.Blazor.TomSelect.Dtos;
using Soenneker.Blazor.TomSelect.Enums;

namespace Soenneker.Blazor.TomSelect.Configuration;

/// <summary>
/// Configuration settings for the Tom Select control, mirroring the options passed
/// to the TomSelect constructor and accessible via its <c>settings</c> property.
/// </summary>
public sealed class TomSelectConfiguration
{
    /// <summary>
    /// Gets or sets the list of values for options initially selected when the control is rendered.
    /// </summary>
    /// <remarks>
    /// Maps to the <c>items</c> setting. Default is an empty list.
    /// </remarks>
    [JsonPropertyName("items")]
    public List<string> Items { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of plugins to load for additional Tom Select functionality.
    /// </summary>
    /// <remarks>
    /// Maps to the <c>plugins</c> setting. Null indicates no plugins.
    /// </remarks>
    [JsonPropertyName("plugins")]
    public List<TomSelectPluginType>? Plugins { get; set; }

    /// <summary>
    /// Determines whether users may create new options not present in the initial list.
    /// </summary>
    /// <remarks>
    /// Maps to the <c>create</c> setting. Default is <c>false</c>.
    /// </remarks>
    [JsonPropertyName("create")]
    public bool Create { get; set; } = false;

    /// <summary>
    /// When enabled, adding focus away from the control will create and select a new option
    /// if <c>Create</c> is <c>true</c>.
    /// </summary>
    /// <remarks>
    /// Maps to <c>createOnBlur</c>. Default is <c>false</c>.
    /// </remarks>
    [JsonPropertyName("createOnBlur")]
    public bool CreateOnBlur { get; set; } = false;

    /// <summary>
    /// Specifies a regular expression or filter function that new values must match
    /// before creation is allowed when <c>Create</c> is enabled.
    /// </summary>
    /// <remarks>
    /// Maps to <c>createFilter</c>. Default is <c>null</c> (no filter).
    /// </remarks>
    [JsonPropertyName("createFilter")]
    public string? CreateFilter { get; set; } = null;

    /// <summary>
    /// Gets or sets the delimiter string used to split and join values in a text input.
    /// </summary>
    /// <remarks>
    /// Maps to <c>delimiter</c>. Default is ",".
    /// </remarks>
    [JsonPropertyName("delimiter")]
    public string Delimiter { get; set; } = ",";

    /// <summary>
    /// Toggles highlighting of matching text in the dropdown options.
    /// </summary>
    /// <remarks>
    /// Maps to <c>highlight</c>. Default is <c>true</c>.
    /// </remarks>
    [JsonPropertyName("highlight")]
    public bool Highlight { get; set; } = true;

    /// <summary>
    /// Determines whether options created by the user persist in the list
    /// after they are deselected.
    /// </summary>
    /// <remarks>
    /// Maps to <c>persist</c>. Default is <c>true</c>.
    /// </remarks>
    [JsonPropertyName("persist")]
    public bool Persist { get; set; } = true;

    /// <summary>
    /// If true, the dropdown will automatically open when the control receives focus.
    /// </summary>
    /// <remarks>
    /// Maps to <c>openOnFocus</c>. Default is <c>true</c>.
    /// </remarks>
    [JsonPropertyName("openOnFocus")]
    public bool OpenOnFocus { get; set; } = true;

    /// <summary>
    /// Specifies the maximum number of dropdown options to display.
    /// </summary>
    /// <remarks>
    /// Maps to <c>maxOptions</c>. Null indicates no limit.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("maxOptions")]
    public int? MaxOptions { get; set; } = null;

    /// <summary>
    /// Specifies the maximum number of items the user may select.
    /// </summary>
    /// <remarks>
    /// Maps to <c>maxItems</c>. Null indicates no limit.
    /// </remarks>
    [JsonPropertyName("maxItems")]
    public int? MaxItems { get; set; } = null;

    /// <summary>
    /// Hides options that are currently selected from the dropdown.
    /// </summary>
    /// <remarks>
    /// Maps to <c>hideSelected</c>. Null will use Tom Select's default behavior.
    /// </remarks>
    [JsonPropertyName("hideSelected")]
    public bool? HideSelected { get; set; } = null;

    /// <summary>
    /// Controls whether the dropdown closes immediately after making a selection.
    /// </summary>
    /// <remarks>
    /// Maps to <c>closeAfterSelect</c>. Null will use Tom Select's default behavior.
    /// </remarks>
    [JsonPropertyName("closeAfterSelect")]
    public bool? CloseAfterSelect { get; set; } = null;

    /// <summary>
    /// Treats options with an empty string value as normal selectable options.
    /// </summary>
    /// <remarks>
    /// Maps to <c>allowEmptyOption</c>. Default is <c>false</c>.
    /// </remarks>
    [JsonPropertyName("allowEmptyOption")]
    public bool AllowEmptyOption { get; set; } = false;

    /// <summary>
    /// Throttles remote loading of options by waiting this many milliseconds before
    /// invoking the <c>load</c> callback.
    /// </summary>
    /// <remarks>
    /// Maps to <c>loadThrottle</c>. Null disables throttling; default is 300.
    /// </remarks>
    [JsonPropertyName("loadThrottle")]
    public int? LoadThrottle { get; set; } = 300;

    /// <summary>
    /// CSS class added to the control's wrapper while awaiting remote load results.
    /// </summary>
    /// <remarks>
    /// Maps to <c>loadingClass</c>. Default is "loading".
    /// </remarks>
    [JsonPropertyName("loadingClass")]
    public string LoadingClass { get; set; } = "loading";

    /// <summary>
    /// The placeholder text displayed when no items are selected.
    /// </summary>
    /// <remarks>
    /// Maps to <c>placeholder</c>. Defaults to the underlying input's placeholder if not set.
    /// </remarks>
    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; set; } = null;

    /// <summary>
    /// Determines whether the placeholder text remains visible when
    /// the control has selected items but is not focused.
    /// </summary>
    /// <remarks>
    /// Maps to <c>hidePlaceholder</c>. Null will defer to Tom Select defaults.
    /// </remarks>
    [JsonPropertyName("hidePlaceholder")]
    public bool? HidePlaceholder { get; set; } = null;

    /// <summary>
    /// Controls preloading of options on initialization or on focus.
    /// </summary>
    /// <remarks>
    /// Maps to <c>preload</c>. Default is <c>false</c>.
    /// </remarks>
    [JsonPropertyName("preload")]
    public bool Preload { get; set; } = false;

    /// <summary>
    /// CSS selector or DOM node reference where the dropdown menu will be appended.
    /// </summary>
    /// <remarks>
    /// Maps to <c>dropdownParent</c>. Null appends to the control element.
    /// </remarks>
    [JsonPropertyName("dropdownParent")]
    public string? DropdownParent { get; set; } = null;

    /// <summary>
    /// If true, new items will be added to the top of the dropdown list by precedence.
    /// </summary>
    /// <remarks>
    /// Maps to <c>addPrecedence</c>. Default is <c>false</c>.
    /// </remarks>
    [JsonPropertyName("addPrecedence")]
    public bool AddPrecedence { get; set; } = false;

    /// <summary>
    /// Enables selecting the highlighted option via the Tab key.
    /// </summary>
    /// <remarks>
    /// Maps to <c>selectOnTab</c>. Default is <c>true</c>.
    /// </remarks>
    [JsonPropertyName("selectOnTab")]
    public bool SelectOnTab { get; set; } = true;

    /// <summary>
    /// Enables support for international (diacritical) characters during search and filtering.
    /// </summary>
    /// <remarks>
    /// Maps to <c>diacritics</c>. Default is <c>true</c>.
    /// </remarks>
    [JsonPropertyName("diacritics")]
    public bool Diacritics { get; set; } = true;

    /// <summary>
    /// Allows supplying a custom input element selector or DOM reference.
    /// </summary>
    /// <remarks>
    /// Maps to <c>controlInput</c>. Null uses the default <input> element.
    /// </remarks>
    [JsonPropertyName("controlInput")]
    public string? ControlInput { get; set; } = null;

    /// <summary>
    /// When enabled, the same option may be selected multiple times.
    /// </summary>
    /// <remarks>
    /// Maps to <c>duplicates</c>. Default is <c>false</c>.
    /// </remarks>
    [JsonPropertyName("duplicates")]
    public bool Duplicates { get; set; } = false;

    /// <summary>
    /// Defines groups used to bucket options in the dropdown.
    /// </summary>
    /// <remarks>
    /// Maps to <c>optgroups</c>. Null indicates no grouping.
    /// </remarks>
    [JsonPropertyName("optgroups")]
    public List<TomSelectOption>? OptGroups { get; set; }

    /// <summary>
    /// The attribute name on option elements containing JSON data for that option.
    /// </summary>
    /// <remarks>
    /// Maps to <c>dataAttr</c>. Null disables reading JSON from attributes.
    /// </remarks>
    [JsonPropertyName("dataAttr")]
    public string? DataAttr { get; set; } = null;

    /// <summary>
    /// The property name of option objects used as the selected value.
    /// </summary>
    /// <remarks>
    /// Maps to <c>valueField</c>. Default is "value".
    /// </remarks>
    [JsonPropertyName("valueField")]
    public string ValueField { get; set; } = "value";

    /// <summary>
    /// The property name of optgroup objects serving as their unique identifier.
    /// </summary>
    /// <remarks>
    /// Maps to <c>optgroupValueField</c>. Default is "value".
    /// </remarks>
    [JsonPropertyName("optgroupValueField")]
    public string OptgroupValueField { get; set; } = "value";

    /// <summary>
    /// The property name of option objects used as the display label.
    /// </summary>
    /// <remarks>
    /// Maps to <c>labelField</c>. Default is "text".
    /// </remarks>
    [JsonPropertyName("labelField")]
    public string LabelField { get; set; } = "text";

    /// <summary>
    /// The property name of optgroup objects used as the display label.
    /// </summary>
    /// <remarks>
    /// Maps to <c>optgroupLabelField</c>. Default is "label".
    /// </remarks>
    [JsonPropertyName("optgroupLabelField")]
    public string OptgroupLabelField { get; set; } = "label";

    /// <summary>
    /// The property name of option objects indicating their optgroup membership.
    /// </summary>
    /// <remarks>
    /// Maps to <c>optgroupField</c>. Default is "optgroup".
    /// </remarks>
    [JsonPropertyName("optgroupField")]
    public string OptgroupField { get; set; } = "optgroup";

    /// <summary>
    /// The property name indicating whether an option or group is disabled.
    /// </summary>
    /// <remarks>
    /// Maps to <c>disabledField</c>. Default is "disabled".
    /// </remarks>
    [JsonPropertyName("disabledField")]
    public string DisabledField { get; set; } = "disabled";

    /// <summary>
    /// Defines how dropdown items are sorted before display.
    /// </summary>
    /// <remarks>
    /// Maps to <c>sortField</c>. Default sorts by score then original order.
    /// </remarks>
    [JsonPropertyName("sortField")]
    public List<TomSelectSortField> SortField { get; set; } =
    [
        new() {Field = "$score"},

        new() {Field = "$order"}
    ];

    /// <summary>
    /// The list of property names used when filtering options by search.
    /// </summary>
    /// <remarks>
    /// Maps to <c>searchField</c>. Default is ["text"].
    /// </remarks>
    [JsonPropertyName("searchField")]
    public List<string> SearchField { get; set; } = ["text"];

    /// <summary>
    /// The operator used when combining multiple search terms ("and" or "or").
    /// </summary>
    /// <remarks>
    /// Maps to <c>searchConjunction</c>. Default is "and".
    /// </remarks>
    [JsonPropertyName("searchConjunction")]
    public string SearchConjunction { get; set; } = "and";

    /// <summary>
    /// If true, preserves the original optgroup order regardless of search results.
    /// </summary>
    /// <remarks>
    /// Maps to <c>lockOptgroupOrder</c>. Default is <c>false</c>.
    /// </remarks>
    [JsonPropertyName("lockOptgroupOrder")]
    public bool LockOptgroupOrder { get; set; } = false;

    /// <summary>
    /// Copies CSS classes from the original input element to the dropdown menu.
    /// </summary>
    /// <remarks>
    /// Maps to <c>copyClassesToDropdown</c>. Default is <c>true</c>.
    /// </remarks>
    [JsonPropertyName("copyClassesToDropdown")]
    public bool CopyClassesToDropdown { get; set; } = true;

    /// <summary>
    /// When true, loads Tom Select assets from the CDN instead of local files.
    /// </summary>
    /// <remarks>
    /// Maps to <c>useCdn</c>. Default is <c>true</c>.
    /// </remarks>
    [JsonPropertyName("useCdn")]
    public bool UseCdn { get; set; } = true;

    /// <summary>
    /// When true, enables debug logging to the browser console.
    /// </summary>
    /// <remarks>
    /// Controls whether console.warn, console.log, and console.error statements are executed.
    /// Default is <c>false</c>.
    /// </remarks>
    [JsonPropertyName("debug")]
    public bool Debug { get; set; }

    /// <summary>
    /// When true, Tom Select will call back into .NET to load options via a C# function.
    /// </summary>
    /// <remarks>
    /// Custom flag handled by interop; not a native Tom Select setting.
    /// </remarks>
    [JsonPropertyName("useDotNetLoad")]
    public bool UseDotNetLoad { get; set; } = false;

    /// <summary>
    /// Minimum query length required before invoking the load function.
    /// </summary>
    /// <remarks>
    /// Custom flag handled by interop; not a native Tom Select setting. If null, no minimum enforced.
    /// </remarks>
    [JsonPropertyName("shouldLoadMinQueryLength")]
    public int? ShouldLoadMinQueryLength { get; set; }

    /// <summary>
    /// When true, option and item markup will be sourced from DOM nodes rendered by Blazor.
    /// </summary>
    /// <remarks>
    /// Custom flag handled by interop; not a native Tom Select setting.
    /// </remarks>
    [JsonPropertyName("templatesFromDom")]
    public bool TemplatesFromDom { get; set; } = false;
}

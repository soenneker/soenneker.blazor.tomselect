using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using System.Threading;
using Soenneker.Blazor.TomSelect.Configuration;
using Soenneker.Blazor.TomSelect.Dtos;
using System.Collections.Generic;

namespace Soenneker.Blazor.TomSelect.Abstract;

/// <summary>
/// Represents a generic interface for interacting with a TomSelect component in Blazor.
/// </summary>
/// <typeparam name="TItem">The type of the items in the TomSelect component.</typeparam>
/// <typeparam name="TSource">The type of the source data for the TomSelect component.</typeparam>
public interface ITomSelect<TItem, TSource>
{
    /// <summary>
    /// Gets or sets the data source for the TomSelect component.
    /// </summary>
    IEnumerable<TItem> Data { get; set; }

    /// <summary>
    /// The selected item(s) in the TomSelect component.
    /// </summary>
    List<TItem> Items { get; set; }

    #region Events

    /// <summary>
    /// Event triggered when the component is initialized.
    /// </summary>
    EventCallback OnInitialize { get; set; }

    /// <summary>
    /// Event triggered when the selected value changes.
    /// </summary>
    EventCallback<string> OnChange { get; set; }

    /// <summary>
    /// Event triggered when the component gains focus.
    /// </summary>
    EventCallback OnFocus { get; set; }

    /// <summary>
    /// Event triggered when the component loses focus.
    /// </summary>
    EventCallback OnBlur { get; set; }

    /// <summary>
    /// Event triggered when an item is added to the component.
    /// </summary>
    EventCallback<(string Value, TomSelectOption Item)> OnItemAdd { get; set; }

    /// <summary>
    /// Event triggered when an item is removed from the component.
    /// </summary>
    EventCallback<(string Value, TomSelectOption Item)> OnItemRemove { get; set; }

    /// <summary>
    /// Event triggered when an item is selected.
    /// </summary>
    EventCallback<TomSelectOption> OnItemSelect { get; set; }

    /// <summary>
    /// Event triggered when all selected items are cleared.
    /// </summary>
    EventCallback OnClear { get; set; }

    /// <summary>
    /// Event triggered when an option is added to the component.
    /// </summary>
    EventCallback<(string Value, TomSelectOption Data)> OnOptionAdd { get; set; }

    /// <summary>
    /// Event triggered when an option is removed from the component.
    /// </summary>
    EventCallback<string> OnOptionRemove { get; set; }

    /// <summary>
    /// Event triggered when all options are cleared from the component.
    /// </summary>
    EventCallback OnOptionClear { get; set; }

    /// <summary>
    /// Event triggered when an optgroup is added to the component.
    /// </summary>
    EventCallback<(string Id, TomSelectOption Data)> OnOptgroupAdd { get; set; }

    /// <summary>
    /// Event triggered when an optgroup is removed from the component.
    /// </summary>
    EventCallback<string> OnOptgroupRemove { get; set; }

    /// <summary>
    /// Event triggered when all optgroups are cleared from the component.
    /// </summary>
    EventCallback OnOptgroupClear { get; set; }

    /// <summary>
    /// Event triggered when the dropdown is opened.
    /// </summary>
    EventCallback<TomSelectOption> OnDropdownOpen { get; set; }

    /// <summary>
    /// Event triggered when the dropdown is closed.
    /// </summary>
    EventCallback<TomSelectOption> OnDropdownClose { get; set; }

    /// <summary>
    /// Event triggered when the user types into the input.
    /// </summary>
    EventCallback<string> OnType { get; set; }

    /// <summary>
    /// Event triggered when the component is loaded.
    /// </summary>
    EventCallback<object> OnLoad { get; set; }

    /// <summary>
    /// Event triggered when the component is destroyed.
    /// </summary>
    EventCallback OnDestroy { get; set; }

    #endregion Events

    /// <summary>
    /// Creates the TomSelect component with the specified configuration.
    /// </summary>
    /// <param name="configuration">The configuration for the TomSelect component. (Optional)</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask Create(TomSelectConfiguration? configuration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a single option to the TomSelect component.
    /// </summary>
    /// <param name="item">The item to add as an option.</param>
    /// <param name="userCreated">Indicates whether the item was created by the user. (Optional)</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask AddOption(TItem item, bool userCreated = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple options to the TomSelect component.
    /// </summary>
    /// <param name="items">The items to add as options.</param>
    /// <param name="userCreated">Indicates whether the items were created by the user. (Optional)</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask AddOptions(IEnumerable<TItem> items, bool userCreated = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the option in the TomSelect component with the specified value.
    /// </summary>
    /// <param name="value">The value of the option to update.</param>
    /// <param name="item">The new item to assign to the option.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask UpdateOption(string value, TItem item, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the option with the specified value from the TomSelect component.
    /// </summary>
    /// <param name="value">The value of the option to remove.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask RemoveOption(string value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes the options in the TomSelect component.
    /// </summary>
    /// <param name="triggerDropdown">Indicates whether to trigger the dropdown. (Optional)</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask RefreshOptions(bool triggerDropdown, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all options from the TomSelect component.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask ClearOptions(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds an item with the specified value to the TomSelect component.
    /// </summary>
    /// <param name="value">The value of the item to add.</param>
    /// <param name="silent">Indicates whether to suppress triggering any events. Default is false. (Optional)</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask AddItem(string value, bool silent = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all selected items from the TomSelect component.
    /// </summary>
    /// <param name="silent">Indicates whether to suppress triggering any events. Default is false. (Optional)</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask ClearItems(bool silent = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an item from the TomSelect component based on its value or HTML element.
    /// </summary>
    /// <param name="valueOrHTMLElement">The value or HTML element of the item to remove.</param>
    /// <param name="silent">Indicates whether to suppress triggering any events. Default is false. (Optional)</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask RemoveItem(string valueOrHTMLElement, bool silent = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes the items in the TomSelect component.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask RefreshItems(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds an option group with the specified id and data to the TomSelect component.
    /// </summary>
    /// <param name="id">The id of the option group.</param>
    /// <param name="data">The data associated with the option group.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask AddOptionGroup(string id, object data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the option group with the specified id from the TomSelect component.
    /// </summary>
    /// <param name="id">The id of the option group to remove.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask RemoveOptionGroup(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all option groups from the TomSelect component.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask ClearOptionGroups(CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens the dropdown of the TomSelect component.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask OpenDropdown(CancellationToken cancellationToken = default);

    /// <summary>
    /// Closes the dropdown of the TomSelect component.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask CloseDropdown(CancellationToken cancellationToken = default);

    /// <summary>
    /// Positions the dropdown of the TomSelect component.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask PositionDropdown(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets focus to the TomSelect component.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask Focus(CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes focus from the TomSelect component.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask Blur(CancellationToken cancellationToken = default);

    /// <summary>
    /// Locks the TomSelect component.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask Lock(CancellationToken cancellationToken = default);

    /// <summary>
    /// Unlocks the TomSelect component.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask Unlock(CancellationToken cancellationToken = default);

    /// <summary>
    /// Enables the TomSelect component.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask Enable(CancellationToken cancellationToken = default);

    /// <summary>
    /// Disables the TomSelect component.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask Disable(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the value of the TomSelect component.
    /// </summary>
    /// <param name="value">The value to set.</param>
    /// <param name="silent">Indicates whether to suppress triggering any events. Default is false. (Optional)</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask SetValue(TomSelectOption value, bool silent = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the value of the TomSelect component.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask<TomSelectOption> GetValue(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the caret position in the TomSelect component.
    /// </summary>
    /// <param name="index">The index to set the caret to.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask SetCaret(int index, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the TomSelect component is full.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask<bool> IsFull(CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears the cache of the TomSelect component.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask ClearCache(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the value of the textbox associated with the TomSelect component.
    /// </summary>
    /// <param name="str">The value to set.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask SetTextboxValue(string str, CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronizes the TomSelect component.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask Sync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Destroys the TomSelect component.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask Destroy(CancellationToken cancellationToken = default);
}
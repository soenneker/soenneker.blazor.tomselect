using Microsoft.AspNetCore.Components;
using Soenneker.Blazor.TomSelect.Dtos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Blazor.TomSelect.Base.Abstract;

/// <summary>
/// Represents the base object for TomSelect.
/// </summary>
/// <remarks>We need a base object because we have generic type parameters on TomSelect.</remarks>
public interface IBaseTomSelect : IAsyncDisposable
{
    #region Events

    /// <summary>
    /// Event triggered when the component is initialized.
    /// </summary>
    EventCallback OnInitialize { get; set; }

    /// <summary>
    /// Event triggered when the selected value changes.
    /// </summary>
    EventCallback<List<string>> OnChange { get; set; }

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

    EventCallback<(string Value, TomSelectOption Item)> OnItemCreated { get; set; }

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
    EventCallback OnClearItems { get; set; }

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

    /// <summary>
    /// For debugging to log messages
    /// </summary>
    bool Debug { get; set; }

    /// <summary>
    /// Placeholder text that TomSelect will display when nothing is selected.
    /// </summary>
    public string? Placeholder { get; set; }

    #endregion Events

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

    /// <summary>
    /// Removes an item from the TomSelect component based on its value or HTML element.
    /// </summary>
    /// <param name="valueOrHtmlElement">The value or HTML element of the item to remove.</param>
    /// <param name="silent">Indicates whether to suppress triggering any events. Default is false. (Optional)</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask RemoveItem(string valueOrHtmlElement, bool silent = false, CancellationToken cancellationToken = default);

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
}

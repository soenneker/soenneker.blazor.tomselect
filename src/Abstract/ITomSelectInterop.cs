using Microsoft.AspNetCore.Components;
using Soenneker.Blazor.Utils.EventListeningInterop.Abstract;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.JSInterop;
using System.Collections.Generic;
using Soenneker.Blazor.TomSelect.Configuration;
using Soenneker.Blazor.TomSelect.Dtos;
using Soenneker.Blazor.TomSelect.Base;
using System;

namespace Soenneker.Blazor.TomSelect.Abstract;

/// <summary>
/// A Blazor interop library for the select user control library, Tom Select
/// </summary>
public interface ITomSelectInterop : IEventListeningInterop, IAsyncDisposable
{
    /// <summary>
    /// Initializes the TomSelect interop by loading required scripts and styles.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the initialization operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a MutationObserver for the element with the specified identifier.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a TomSelect instance on the provided element.
    /// </summary>
    /// <param name="elementReference">A reference to the target element.</param>
    /// <param name="elementId">The unique identifier of the element.</param>
    /// <param name="dotNetObjectRef">A reference to a .NET object for callback handling.</param>
    /// <param name="configuration">Optional configuration settings for TomSelect.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask Create(ElementReference elementReference, string elementId, DotNetObjectReference<BaseTomSelect> dotNetObjectRef, TomSelectConfiguration? configuration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Destroys the TomSelect instance associated with the specified element.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask Destroy(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds an option to the TomSelect instance.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="tomSelectOption">The option to add.</param>
    /// <param name="userCreated">Indicates whether the option was created by the user.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask AddOption(string elementId, TomSelectOption tomSelectOption, bool userCreated = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple options to the TomSelect instance.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="data">A collection of options to add.</param>
    /// <param name="userCreated">Indicates whether the options were created by the user.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask AddOptions(string elementId, IEnumerable<TomSelectOption> data, bool userCreated = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing option in the TomSelect instance.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="value">The value identifying the option to update.</param>
    /// <param name="data">The updated option data.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask UpdateOption(string elementId, string value, TomSelectOption data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an option from the TomSelect instance.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="value">The value of the option to remove.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask RemoveOption(string elementId, string value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes the options in the TomSelect instance.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="triggerDropdown">Specifies whether to trigger the dropdown update.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask RefreshOptions(string elementId, bool triggerDropdown, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all options from the TomSelect instance.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask ClearOptions(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears the selected items from the TomSelect instance.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="silent">If set to true, suppresses any events triggered by the operation.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask ClearItems(string elementId, bool silent = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears current items and adds new items to the TomSelect instance.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="values">A collection of values to add as items.</param>
    /// <param name="silent">If set to true, suppresses any events triggered by the operation.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask ClearAndAddItems(string elementId, IEnumerable<string> values, bool silent = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears current options and adds new options to the TomSelect instance.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="data">A collection of options to add.</param>
    /// <param name="silent">If set to true, suppresses any events triggered by the operation.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask ClearAndAddOptions(string elementId, IEnumerable<TomSelectOption> data, bool silent = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a selected item to the TomSelect instance.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="value">The value of the item to add.</param>
    /// <param name="silent">If set to true, suppresses any events triggered by the operation.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask AddItem(string elementId, string value, bool silent = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple selected items to the TomSelect instance.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="values">A collection of item values to add.</param>
    /// <param name="silent">If set to true, suppresses any events triggered by the operation.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask AddItems(string elementId, IEnumerable<string> values, bool silent = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a selected item from the TomSelect instance.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="valueOrHtmlElement">The value or HTML element representing the item to remove.</param>
    /// <param name="silent">If set to true, suppresses any events triggered by the operation.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask RemoveItem(string elementId, string valueOrHtmlElement, bool silent = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes the selected items in the TomSelect instance.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask RefreshItems(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds an option group to the TomSelect instance.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="id">The identifier for the option group.</param>
    /// <param name="data">The data associated with the option group.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask AddOptionGroup(string elementId, string id, object data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an option group from the TomSelect instance.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="id">The identifier of the option group to remove.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask RemoveOptionGroup(string elementId, string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all option groups from the TomSelect instance.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask ClearOptionGroups(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens the dropdown for the TomSelect instance.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask OpenDropdown(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Closes the dropdown for the TomSelect instance.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask CloseDropdown(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Repositions the dropdown for the TomSelect instance.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask PositionDropdown(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets focus to the TomSelect instance.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask Focus(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes focus from the TomSelect instance.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask Blur(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Locks the TomSelect instance, preventing user interactions.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask Lock(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unlocks the TomSelect instance, allowing user interactions.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask Unlock(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enables the TomSelect instance.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask Enable(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disables the TomSelect instance.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask Disable(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the value of the TomSelect instance.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="value">The option to set as the current value.</param>
    /// <param name="silent">If set to true, suppresses any events triggered by the operation.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask SetValue(string elementId, TomSelectOption value, bool silent = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current value of the TomSelect instance.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask{TomSelectOption}"/> containing the current value.</returns>
    ValueTask<TomSelectOption> GetValue(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the caret position in the TomSelect input.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="index">The index at which to position the caret.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask SetCaret(string elementId, int index, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether the TomSelect instance has reached its selection limit.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask{Boolean}"/> that resolves to <c>true</c> if full; otherwise, <c>false</c>.</returns>
    ValueTask<bool> IsFull(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears the option cache for the TomSelect instance.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask ClearCache(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the value of the underlying textbox for the TomSelect instance.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="str">The text to set in the textbox.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask SetTextboxValue(string elementId, string str, CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronizes the TomSelect instance with its underlying data.
    /// </summary>
    /// <param name="elementId">The unique identifier of the target element.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask Sync(string elementId, CancellationToken cancellationToken = default);
}
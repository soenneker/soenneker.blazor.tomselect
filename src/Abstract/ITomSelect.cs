using Microsoft.AspNetCore.Components;
using Soenneker.Blazor.TomSelect.Base.Abstract;
using Soenneker.Blazor.TomSelect.Configuration;
using Soenneker.Blazor.TomSelect.Dtos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Blazor.TomSelect.Abstract;

/// <summary>
/// Defines the contract for a TomSelect component that provides a dynamic dropdown with selection capabilities.
/// </summary>
/// <typeparam name="TItem">The type of data item.</typeparam>
/// <typeparam name="TType">The type of value associated with the item.</typeparam>
public interface ITomSelect<TItem, TType> : IBaseTomSelect
{
    /// <summary>
    /// Gets or sets the data source for the TomSelect component.
    /// </summary>
    IEnumerable<TItem>? Data { get; set; }

    /// <summary>
    /// Gets or sets the function used to extract the display text from an item.
    /// </summary>
    Func<TItem, string?> TextField { get; set; }

    /// <summary>
    /// Gets or sets the function used to extract the value from an item.
    /// </summary>
    Func<TItem, string?> ValueField { get; set; }

    /// <summary>
    /// Gets or sets the synchronous function for creating a new item from a given string.
    /// </summary>
    Func<string, TItem>? CreateFuncSync { get; set; }

    /// <summary>
    /// Gets or sets the asynchronous function for creating a new item from a given string.
    /// </summary>
    Func<string, ValueTask<TItem>>? CreateFunc { get; set; }

    /// <summary>
    /// Gets or sets additional attributes to be applied to the component.
    /// </summary>
    Dictionary<string, object?>? Attributes { get; set; }

    /// <summary>
    /// Gets or sets the configuration for the TomSelect component.
    /// </summary>
    TomSelectConfiguration Configuration { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether multiple selections are allowed.
    /// </summary>
    bool Multiple { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether new items can be created.
    /// </summary>
    bool Create { get; set; }

    /// <summary>
    /// Gets or sets the currently selected items.
    /// </summary>
    List<TItem> Items { get; set; }

    /// <summary>
    /// Gets or sets the event callback that is invoked when the selected items change.
    /// </summary>
    EventCallback<List<TItem>> ItemsChanged { get; set; }

    /// <summary>
    /// Reinitializes the TomSelect component.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous reinitialization operation.</returns>
    ValueTask Reinitialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds an option based on the specified item.
    /// </summary>
    /// <param name="item">The item to be added as an option.</param>
    /// <param name="userCreated">Indicates whether the option is user created.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// A <see cref="ValueTask{TomSelectOption}"/> containing the added option; 
    /// returns null if the option could not be added.
    /// </returns>
    ValueTask<TomSelectOption?> AddOption(TItem item, bool userCreated = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple options based on the provided items.
    /// </summary>
    /// <param name="items">The items to be added as options.</param>
    /// <param name="userCreated">Indicates whether the options are user created.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous add operation.</returns>
    ValueTask AddOptions(IEnumerable<TItem> items, bool userCreated = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing option identified by its value.
    /// </summary>
    /// <param name="value">The value identifying the option to update.</param>
    /// <param name="item">The item containing updated data for the option.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous update operation.</returns>
    ValueTask UpdateOption(string value, TItem item, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds an item using the specified value.
    /// </summary>
    /// <param name="value">The value representing the item to add.</param>
    /// <param name="silent">If set to true, suppresses any events triggered by this addition.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous add operation.</returns>
    ValueTask AddItem(string value, bool silent = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds an item using the specified data item.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <param name="silent">If set to true, suppresses any events triggered by this addition.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous add operation.</returns>
    ValueTask AddItem(TItem item, bool silent = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple items using their string values.
    /// </summary>
    /// <param name="value">The values representing the items to add.</param>
    /// <param name="silent">If set to true, suppresses any events triggered by this addition.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous add operation.</returns>
    ValueTask AddItems(IEnumerable<string> value, bool silent = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple items.
    /// </summary>
    /// <param name="items">The items to add.</param>
    /// <param name="silent">If set to true, suppresses any events triggered by this addition.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous add operation.</returns>
    ValueTask AddItems(IEnumerable<TItem> items, bool silent = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initializes the TomSelect component with an optional configuration.
    /// </summary>
    /// <param name="configuration">Optional configuration settings.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous initialization.</returns>
    ValueTask Initialize(TomSelectConfiguration? configuration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invoked from JavaScript when the component is initialized.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task OnInitializedJs();

    /// <summary>
    /// Clears all selected items from the component.
    /// </summary>
    /// <param name="silent">If set to true, suppresses any events triggered by the clear operation.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous clear operation.</returns>
    ValueTask ClearItems(bool silent = false, CancellationToken cancellationToken = default);
}

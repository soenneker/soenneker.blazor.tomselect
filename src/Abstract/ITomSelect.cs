using System.Threading.Tasks;
using System.Threading;
using Soenneker.Blazor.TomSelect.Configuration;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using System;

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
    [EditorRequired]
    IEnumerable<TItem>? Data { get; set; }

    /// <summary>
    /// The selected item(s) in the TomSelect component.
    /// </summary>
    List<TItem> Items { get; set; }

    Func<TItem, string?> TextField { get; set; }

    Func<TItem, string?> ValueField { get; set; }

    Dictionary<string, object?>? Attributes { get; set; }

    TomSelectConfiguration Configuration { get; set; }

    bool Multiple { get; set; }

    bool Create { get; set; }

    /// <summary>
    /// Creates the TomSelect component with the specified configuration.
    /// </summary>
    /// <param name="configuration">The configuration for the TomSelect component. (Optional)</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask Initialize(TomSelectConfiguration? configuration = null, CancellationToken cancellationToken = default);

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
    /// Adds an item with the specified value to the TomSelect component.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <param name="silent">Indicates whether to suppress triggering any events. Default is false. (Optional)</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask AddItem(TItem item, bool silent = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds an item with the specified value to the TomSelect component.
    /// </summary>
    /// <param name="value">The value of the item to add.</param>
    /// <param name="silent">Indicates whether to suppress triggering any events. Default is false. (Optional)</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask AddItem(string value, bool silent = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds an item with the specified value to the TomSelect component.
    /// </summary>
    /// <param name="values">The values of the items to add.</param>
    /// <param name="silent">Indicates whether to suppress triggering any events. Default is false. (Optional)</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask AddItems(IEnumerable<string> values, bool silent = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds an item with the specified value to the TomSelect component.
    /// </summary>
    /// <param name="items">The items to add.</param>
    /// <param name="silent">Indicates whether to suppress triggering any events. Default is false. (Optional)</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation. (Optional)</param>
    ValueTask AddItems(IEnumerable<TItem> items, bool silent = false, CancellationToken cancellationToken = default);
}
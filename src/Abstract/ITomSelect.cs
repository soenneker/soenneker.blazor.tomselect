using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using System.Threading;
using Soenneker.Blazor.TomSelect.Configuration;
using Soenneker.Blazor.TomSelect.Dtos;
using System.Collections.Generic;

namespace Soenneker.Blazor.TomSelect.Abstract;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TSource"></typeparam>
public interface ITomSelect<TItem, TSource>
{
    IEnumerable<TItem> Data { get; set; }

    #region Events

    EventCallback OnInitialize { get; set; }

    EventCallback<string> OnChange { get; set; }

    EventCallback OnFocus { get; set; }

    EventCallback OnBlur { get; set; }

    EventCallback<(string Value, TomSelectOption Item)> OnItemAdd { get; set; }

    EventCallback<(string Value, TomSelectOption Item)> OnItemRemove { get; set; }

    EventCallback<object> OnItemSelect { get; set; }

    EventCallback OnClear { get; set; }

    EventCallback<(string Value, TomSelectOption Data)> OnOptionAdd { get; set; }

    EventCallback<string> OnOptionRemove { get; set; }

    EventCallback OnOptionClear { get; set; }

    EventCallback<(string Id, TomSelectOption Data)> OnOptgroupAdd { get; set; }

    EventCallback<string> OnOptgroupRemove { get; set; }

    EventCallback OnOptgroupClear { get; set; }

    EventCallback<object> OnDropdownOpen { get; set; }

    EventCallback<object> OnDropdownClose { get; set; }

    EventCallback<string> OnType { get; set; }

    EventCallback<object> OnLoad { get; set; }

    EventCallback OnDestroy { get; set; }

    #endregion Events

    ValueTask Create(TomSelectConfiguration? configuration = null, CancellationToken cancellationToken = default);

    ValueTask AddOption(TItem item, bool userCreated = true, CancellationToken cancellationToken = default);

    ValueTask AddOptions(IEnumerable<TItem> items, bool userCreated = true, CancellationToken cancellationToken = default);

    ValueTask UpdateOption(string value, TItem item, CancellationToken cancellationToken = default);

    ValueTask RemoveOption(string value, CancellationToken cancellationToken = default);

    ValueTask RefreshOptions(bool triggerDropdown, CancellationToken cancellationToken = default);

    ValueTask ClearOptions(CancellationToken cancellationToken = default);

    ValueTask AddItem(string value, bool silent = false, CancellationToken cancellationToken = default);

    ValueTask ClearItems(bool silent = false, CancellationToken cancellationToken = default);

    ValueTask RemoveItem(string valueOrHTMLElement, bool silent = false, CancellationToken cancellationToken = default);

    ValueTask RefreshItems(CancellationToken cancellationToken = default);

    ValueTask AddOptionGroup(string id, object data, CancellationToken cancellationToken = default);

    ValueTask RemoveOptionGroup(string id, CancellationToken cancellationToken = default);
    ValueTask ClearOptionGroups(CancellationToken cancellationToken = default);

    ValueTask OpenDropdown(CancellationToken cancellationToken = default);

    ValueTask CloseDropdown(CancellationToken cancellationToken = default);

    ValueTask PositionDropdown(CancellationToken cancellationToken = default);

    ValueTask Focus(CancellationToken cancellationToken = default);

    ValueTask Blur(CancellationToken cancellationToken = default);

    ValueTask Lock(CancellationToken cancellationToken = default);

    ValueTask Unlock(CancellationToken cancellationToken = default);

    ValueTask Enable(CancellationToken cancellationToken = default);

    ValueTask Disable(CancellationToken cancellationToken = default);

    ValueTask SetValue(object value, bool silent = false, CancellationToken cancellationToken = default);

    ValueTask<object> GetValue(CancellationToken cancellationToken = default);

    ValueTask SetCaret(int index, CancellationToken cancellationToken = default);

    ValueTask<bool> IsFull(CancellationToken cancellationToken = default);

    ValueTask ClearCache(CancellationToken cancellationToken = default);

    ValueTask SetTextboxValue(string str, CancellationToken cancellationToken = default);

    ValueTask Sync(CancellationToken cancellationToken = default);

    ValueTask Destroy(CancellationToken cancellationToken = default);
}
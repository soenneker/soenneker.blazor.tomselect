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
    ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default);

    ValueTask Initialize(CancellationToken cancellationToken = default);

    ValueTask Create(ElementReference elementReference, string elementId, DotNetObjectReference<BaseTomSelect> dotNetObjectRef, TomSelectConfiguration? configuration = null,  CancellationToken cancellationToken = default);

    ValueTask AddOption(string elementId, TomSelectOption tomSelectOption, bool userCreated = true, CancellationToken cancellationToken = default);

    ValueTask AddOptions(string elementId, IEnumerable<TomSelectOption> data, bool userCreated = true, CancellationToken cancellationToken = default);

    ValueTask UpdateOption(string elementId, string value, TomSelectOption data, CancellationToken cancellationToken = default);

    ValueTask RemoveOption(string elementId, string value, CancellationToken cancellationToken = default);

    ValueTask RefreshOptions(string elementId, bool triggerDropdown, CancellationToken cancellationToken = default);

    ValueTask ClearOptions(string elementId, CancellationToken cancellationToken = default);

    ValueTask ClearItems(string elementId, bool silent = false, CancellationToken cancellationToken = default);

    ValueTask ClearAndAddItems(string elementId, IEnumerable<string> value, bool silent = false, CancellationToken cancellationToken = default);

    ValueTask ClearAndAddOptions(string elementId, IEnumerable<TomSelectOption> data, bool silent = false, CancellationToken cancellationToken = default);

    ValueTask AddItem(string elementId, string value, bool silent = false, CancellationToken cancellationToken = default);

    ValueTask AddItems(string elementId, IEnumerable<string> value, bool silent = false, CancellationToken cancellationToken = default);

    ValueTask RemoveItem(string elementId, string valueOrHtmlElement, bool silent = false, CancellationToken cancellationToken = default);

    ValueTask RefreshItems(string elementId, CancellationToken cancellationToken = default);

    ValueTask AddOptionGroup(string elementId, string id, object data, CancellationToken cancellationToken = default);

    ValueTask RemoveOptionGroup(string elementId, string id, CancellationToken cancellationToken = default);

    ValueTask ClearOptionGroups(string elementId, CancellationToken cancellationToken = default);

    ValueTask OpenDropdown(string elementId, CancellationToken cancellationToken = default);

    ValueTask CloseDropdown(string elementId, CancellationToken cancellationToken = default);

    ValueTask PositionDropdown(string elementId, CancellationToken cancellationToken = default);

    ValueTask Focus(string elementId, CancellationToken cancellationToken = default);

    ValueTask Blur(string elementId, CancellationToken cancellationToken = default);

    ValueTask Lock(string elementId, CancellationToken cancellationToken = default);

    ValueTask Unlock(string elementId, CancellationToken cancellationToken = default);

    ValueTask Enable(string elementId, CancellationToken cancellationToken = default);

    ValueTask Disable(string elementId, CancellationToken cancellationToken = default);

    ValueTask SetValue(string elementId, TomSelectOption value, bool silent = false, CancellationToken cancellationToken = default);

    ValueTask<TomSelectOption> GetValue(string elementId, CancellationToken cancellationToken = default);

    ValueTask SetCaret(string elementId, int index, CancellationToken cancellationToken = default);

    ValueTask<bool> IsFull(string elementId, CancellationToken cancellationToken = default);

    ValueTask ClearCache(string elementId, CancellationToken cancellationToken = default);

    ValueTask SetTextboxValue(string elementId, string str, CancellationToken cancellationToken = default);

    ValueTask Sync(string elementId, CancellationToken cancellationToken = default);

    ValueTask Destroy(string elementId, CancellationToken cancellationToken = default);
}
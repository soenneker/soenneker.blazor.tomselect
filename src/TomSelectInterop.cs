using Soenneker.Blazor.TomSelect.Abstract;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using Soenneker.Blazor.Utils.EventListeningInterop;
using Microsoft.AspNetCore.Components;
using Soenneker.Utils.Json;
using System.Threading;
using System.Collections.Generic;
using Soenneker.Blazor.TomSelect.Configuration;
using Soenneker.Blazor.TomSelect.Dtos;
using Soenneker.Blazor.TomSelect.Base;

namespace Soenneker.Blazor.TomSelect;

/// <inheritdoc cref="ITomSelectInterop"/>
public class TomSelectInterop : EventListeningInterop, ITomSelectInterop
{
    private readonly ILogger<TomSelectInterop> _logger;

    public TomSelectInterop(IJSRuntime jSRuntime, ILogger<TomSelectInterop> logger) : base(jSRuntime)
    {
        _logger = logger;
    }

    public ValueTask Create(ElementReference elementReference, string elementId, DotNetObjectReference<BaseTomSelect> dotNetObjectRef, TomSelectConfiguration? configuration = null, CancellationToken cancellationToken = default)
    {
        string? json = null;

        if (configuration != null)
            json = JsonUtil.Serialize(configuration);

        return JsRuntime.InvokeVoidAsync("tomSelectInterop.create", cancellationToken, elementReference, elementId, json, dotNetObjectRef);
    }

    public ValueTask Destroy(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.destroy", cancellationToken, elementId);
    }

    public ValueTask AddOption(string elementId, TomSelectOption tomSelectOption, bool userCreated = false, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.addOption", cancellationToken, elementId, tomSelectOption, userCreated);
    }

    public ValueTask AddOptions(string elementId, IEnumerable<TomSelectOption> data, bool userCreated = false, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.addOptions", cancellationToken, elementId, data, userCreated);
    }

    public ValueTask UpdateOption(string elementId, string value, TomSelectOption data, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.updateOption", cancellationToken, elementId, value, data);
    }

    public ValueTask RemoveOption(string elementId, string value, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.removeOption", cancellationToken, elementId, value);
    }

    public ValueTask RefreshOptions(string elementId, bool triggerDropdown, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.refreshOptions", cancellationToken, elementId, triggerDropdown);
    }

    public ValueTask ClearOptions(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.clearOptions", cancellationToken, elementId);
    }

    public ValueTask ClearItems(string elementId, bool silent = false, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.clearItems", cancellationToken, elementId, silent);
    }

    public ValueTask ClearAndAddItems(string elementId, IEnumerable<string> values, bool silent = false, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.clearAndAddItems", cancellationToken, elementId, values, silent);
    }

    public ValueTask ClearAndAddOptions(string elementId, IEnumerable<TomSelectOption> data, bool silent = false, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.clearAndAddOptions", cancellationToken, elementId, data, silent);
    }

    public ValueTask AddItem(string elementId, string value, bool silent = false, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.addItem", cancellationToken, elementId, value, silent);
    }

    public ValueTask AddItems(string elementId, IEnumerable<string> values, bool silent = false, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.addItems", cancellationToken, elementId, values, silent);
    }

    public ValueTask RemoveItem(string elementId, string valueOrHtmlElement, bool silent = false, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.removeItem", cancellationToken, elementId, valueOrHtmlElement, silent);
    }

    public ValueTask RefreshItems(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.refreshItems", cancellationToken, elementId);
    }

    // Optgroup Methods
    public ValueTask AddOptionGroup(string elementId, string id, object data, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.addOptionGroup", cancellationToken, elementId, id, data);
    }

    public ValueTask RemoveOptionGroup(string elementId, string id, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.removeOptionGroup", cancellationToken, elementId, id);
    }

    public ValueTask ClearOptionGroups(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.clearOptionGroups", cancellationToken, elementId);
    }

    // Dropdown Methods
    public ValueTask OpenDropdown(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.open", cancellationToken, elementId);
    }

    public ValueTask CloseDropdown(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.close", cancellationToken, elementId);
    }

    public ValueTask PositionDropdown(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.positionDropdown", cancellationToken, elementId);
    }

    public ValueTask Focus(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.focus", cancellationToken, elementId);
    }

    public ValueTask Blur(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.blur", cancellationToken, elementId);
    }


    // Lock and Unlock Methods
    public ValueTask Lock(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.lock", cancellationToken, elementId);
    }

    public ValueTask Unlock(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.unlock", cancellationToken, elementId);
    }

    // Enable and Disable Methods
    public ValueTask Enable(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.enable", cancellationToken, elementId);
    }

    public ValueTask Disable(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.disable", cancellationToken, elementId);
    }

    // Value Management Methods
    public ValueTask SetValue(string elementId, TomSelectOption value, bool silent = false, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.setValue", cancellationToken, elementId, value, silent);
    }

    public ValueTask<TomSelectOption> GetValue(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeAsync<TomSelectOption>("tomSelectInterop.getValue", cancellationToken, elementId);
    }

    // Caret and Item Management Methods
    public ValueTask SetCaret(string elementId, int index, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.setCaret", cancellationToken, elementId, index);
    }

    public ValueTask<bool> IsFull(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeAsync<bool>("tomSelectInterop.isFull", cancellationToken, elementId);
    }

    // Cache and Textbox Value Methods
    public ValueTask ClearCache(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.clearCache", cancellationToken, elementId);
    }

    public ValueTask SetTextboxValue(string elementId, string str, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.setTextboxValue", cancellationToken, elementId, str);
    }

    // Synchronization Method
    public ValueTask Sync(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("tomSelectInterop.sync", cancellationToken, elementId);
    }
}
using Soenneker.Blazor.TomSelect.Abstract;
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
using Soenneker.Utils.AsyncSingleton;
using Soenneker.Blazor.Utils.ResourceLoader.Abstract;
using Soenneker.Extensions.ValueTask;
using System;

namespace Soenneker.Blazor.TomSelect;

/// <inheritdoc cref="ITomSelectInterop"/>
public class TomSelectInterop : EventListeningInterop, ITomSelectInterop
{
    private readonly IResourceLoader _resourceLoader;
    private readonly AsyncSingleton _scriptInitializer;

    public TomSelectInterop(IJSRuntime jSRuntime, IResourceLoader resourceLoader) : base(jSRuntime)
    {
        _resourceLoader = resourceLoader;

        _scriptInitializer = new AsyncSingleton(async (token, arr) =>
        {
            var useCdn = true;

            if (arr.Length > 0)
                useCdn = (bool) arr[0];

            await _resourceLoader.ImportModuleAndWaitUntilAvailable("Soenneker.Blazor.TomSelect/js/tomselectinterop.js", "TomSelectInterop", 100, token)
                                 .NoSync();

            if (useCdn)
            {
                await _resourceLoader.LoadStyle("https://cdn.jsdelivr.net/npm/tom-select@2.4.3/dist/css/tom-select.bootstrap5.min.css",
                                         "sha256-t5cAXPIzePs4RIuA3FejMxOlxXe4QXZXQ7sfKJxNU+Y=", cancellationToken: token)
                                     .NoSync();

                await _resourceLoader.LoadScriptAndWaitForVariable("https://cdn.jsdelivr.net/npm/tom-select@2.4.3/dist/js/tom-select.complete.min.js",
                                         "TomSelect", "sha256-lQMtfzgdbG8ufMCU5UThXG65Wsv5CIXGkHFGCHA68ME=", cancellationToken: token)
                                     .NoSync();
            }
            else
            {
                await _resourceLoader.LoadStyle("_content/Soenneker.Blazor.TomSelect/css/tom-select.bootstrap5.min.css", cancellationToken: token).NoSync();

                await _resourceLoader
                      .LoadScriptAndWaitForVariable("_content/Soenneker.Blazor.TomSelect/js/tom-select.complete.min.js", "TomSelect", cancellationToken: token)
                      .NoSync();
            }

            return new object();
        });
    }

    public ValueTask Initialize(bool useCdn = true, CancellationToken cancellationToken = default)
    {
        return _scriptInitializer.Init(cancellationToken, useCdn);
    }

    public ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.createObserver", cancellationToken, elementId);
    }

    public async ValueTask Create(ElementReference elementReference, string elementId, DotNetObjectReference<BaseTomSelect> dotNetObjectRef,
        TomSelectConfiguration? configuration = null, CancellationToken cancellationToken = default)
    {
        await _scriptInitializer.Init(cancellationToken, configuration?.UseCdn).NoSync();

        string? json = null;

        if (configuration != null)
            json = JsonUtil.Serialize(configuration);

        await JsRuntime.InvokeVoidAsync("TomSelectInterop.create", cancellationToken, elementReference, elementId, json, dotNetObjectRef).NoSync();
    }

    public ValueTask Destroy(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.destroy", cancellationToken, elementId);
    }

    public ValueTask AddOption(string elementId, TomSelectOption tomSelectOption, bool userCreated = false, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.addOption", cancellationToken, elementId, tomSelectOption, userCreated);
    }

    public ValueTask AddOptions(string elementId, IEnumerable<TomSelectOption> data, bool userCreated = false, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.addOptions", cancellationToken, elementId, data, userCreated);
    }

    public ValueTask UpdateOption(string elementId, string value, TomSelectOption data, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.updateOption", cancellationToken, elementId, value, data);
    }

    public ValueTask RemoveOption(string elementId, string value, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.removeOption", cancellationToken, elementId, value);
    }

    public ValueTask RefreshOptions(string elementId, bool triggerDropdown, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.refreshOptions", cancellationToken, elementId, triggerDropdown);
    }

    public ValueTask ClearOptions(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.clearOptions", cancellationToken, elementId);
    }

    public ValueTask ClearItems(string elementId, bool silent = false, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.clearItems", cancellationToken, elementId, silent);
    }

    public ValueTask ClearAndAddItems(string elementId, IEnumerable<string> values, bool silent = false, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.clearAndAddItems", cancellationToken, elementId, values, silent);
    }

    public ValueTask ClearAndAddOptions(string elementId, IEnumerable<TomSelectOption> data, bool silent = false, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.clearAndAddOptions", cancellationToken, elementId, data, silent);
    }

    public ValueTask AddItem(string elementId, string value, bool silent = false, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.addItem", cancellationToken, elementId, value, silent);
    }

    public ValueTask AddItems(string elementId, IEnumerable<string> values, bool silent = false, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.addItems", cancellationToken, elementId, values, silent);
    }

    public ValueTask RemoveItem(string elementId, string valueOrHtmlElement, bool silent = false, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.removeItem", cancellationToken, elementId, valueOrHtmlElement, silent);
    }

    public ValueTask RefreshItems(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.refreshItems", cancellationToken, elementId);
    }

    public ValueTask AddOptionGroup(string elementId, string id, object data, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.addOptionGroup", cancellationToken, elementId, id, data);
    }

    public ValueTask RemoveOptionGroup(string elementId, string id, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.removeOptionGroup", cancellationToken, elementId, id);
    }

    public ValueTask ClearOptionGroups(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.clearOptionGroups", cancellationToken, elementId);
    }

    public ValueTask OpenDropdown(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.open", cancellationToken, elementId);
    }

    public ValueTask CloseDropdown(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.close", cancellationToken, elementId);
    }

    public ValueTask PositionDropdown(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.positionDropdown", cancellationToken, elementId);
    }

    public ValueTask Focus(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.focus", cancellationToken, elementId);
    }

    public ValueTask Blur(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.blur", cancellationToken, elementId);
    }

    public ValueTask Lock(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.lock", cancellationToken, elementId);
    }

    public ValueTask Unlock(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.unlock", cancellationToken, elementId);
    }

    public ValueTask Enable(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.enable", cancellationToken, elementId);
    }

    public ValueTask Disable(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.disable", cancellationToken, elementId);
    }

    public ValueTask SetValue(string elementId, TomSelectOption value, bool silent = false, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.setValue", cancellationToken, elementId, value, silent);
    }

    public ValueTask<TomSelectOption> GetValue(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeAsync<TomSelectOption>("TomSelectInterop.getValue", cancellationToken, elementId);
    }

    public ValueTask SetCaret(string elementId, int index, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.setCaret", cancellationToken, elementId, index);
    }

    public ValueTask<bool> IsFull(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeAsync<bool>("TomSelectInterop.isFull", cancellationToken, elementId);
    }

    public ValueTask ClearCache(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.clearCache", cancellationToken, elementId);
    }

    public ValueTask SetTextboxValue(string elementId, string str, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.setTextboxValue", cancellationToken, elementId, str);
    }

    public ValueTask Sync(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("TomSelectInterop.sync", cancellationToken, elementId);
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        await _resourceLoader.DisposeModule("Soenneker.Blazor.TomSelect/tomselectinterop.js").NoSync();

        await _scriptInitializer.DisposeAsync().NoSync();
    }
}
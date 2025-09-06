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

namespace Soenneker.Blazor.TomSelect;

/// <inheritdoc cref="ITomSelectInterop"/>
public sealed class TomSelectInterop : EventListeningInterop, ITomSelectInterop
{
    private readonly IResourceLoader _resourceLoader;
    private readonly AsyncSingleton _scriptInitializer;

    private const string _module = "Soenneker.Blazor.TomSelect/js/tomselectinterop.js";
    private const string _moduleName = "TomSelectInterop";

    public TomSelectInterop(IJSRuntime jSRuntime, IResourceLoader resourceLoader) : base(jSRuntime)
    {
        _resourceLoader = resourceLoader;

        _scriptInitializer = new AsyncSingleton(async (token, arr) =>
        {
            var useCdn = true;

            if (arr.Length > 0)
                useCdn = (bool) arr[0];

            if (useCdn)
            {
                await _resourceLoader.LoadStyle("https://cdn.jsdelivr.net/npm/tom-select@2.4.3/dist/css/tom-select.bootstrap5.min.css",
                                         "sha256-lQMtfzgdbG8ufMCU5UThXG65Wsv5CIXGkHFGCHA68ME=", cancellationToken: token)
                                     ;

                await _resourceLoader.LoadScriptAndWaitForVariable("https://cdn.jsdelivr.net/npm/tom-select@2.4.3/dist/js/tom-select.complete.min.js",
                                         "TomSelect", "sha256-t5cAXPIzePs4RIuA3FejMxOlxXe4QXZXQ7sfKJxNU+Y=", cancellationToken: token)
                                     ;
            }
            else
            {
                await _resourceLoader.LoadStyle("_content/Soenneker.Blazor.TomSelect/css/tom-select.bootstrap5.min.css", cancellationToken: token);

                await _resourceLoader.LoadScriptAndWaitForVariable("_content/Soenneker.Blazor.TomSelect/js/tom-select.complete.min.js", "TomSelect",
                                         cancellationToken: token)
                                     ;
            }

            await _resourceLoader.ImportModuleAndWaitUntilAvailable(_module, _moduleName, 100, token);

            return new object();
        });
    }

    public ValueTask Initialize(bool useCdn = true, CancellationToken cancellationToken = default)
    {
        return _scriptInitializer.Init(cancellationToken, useCdn);
    }

    public ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.createObserver", cancellationToken, elementId);
    }

    public async ValueTask Create(ElementReference elementReference, string elementId, DotNetObjectReference<BaseTomSelect> dotNetObjectRef,
        TomSelectConfiguration? configuration = null, CancellationToken cancellationToken = default)
    {
        await _scriptInitializer.Init(cancellationToken, configuration?.UseCdn ?? true);

        string? json = null;

        if (configuration != null)
            json = JsonUtil.Serialize(configuration);

        await JsRuntime.InvokeVoidAsync($"{_moduleName}.create", cancellationToken, elementReference, elementId, json, dotNetObjectRef);
    }

    public ValueTask Destroy(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.destroy", cancellationToken, elementId);
    }

    public ValueTask AddOption(string elementId, TomSelectOption tomSelectOption, bool userCreated = false, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.addOption", cancellationToken, elementId, tomSelectOption, userCreated);
    }

    public ValueTask AddOptions(string elementId, IEnumerable<TomSelectOption> data, bool userCreated = false, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.addOptions", cancellationToken, elementId, data, userCreated);
    }

    public ValueTask UpdateOption(string elementId, string value, TomSelectOption data, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.updateOption", cancellationToken, elementId, value, data);
    }

    public ValueTask RemoveOption(string elementId, string value, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.removeOption", cancellationToken, elementId, value);
    }

    public ValueTask RefreshOptions(string elementId, bool triggerDropdown, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.refreshOptions", cancellationToken, elementId, triggerDropdown);
    }

    public ValueTask ClearOptions(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.clearOptions", cancellationToken, elementId);
    }

    public ValueTask ClearItems(string elementId, bool silent = false, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.clearItems", cancellationToken, elementId, silent);
    }

    public ValueTask ClearAndAddItems(string elementId, IEnumerable<string> values, bool silent = false, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.clearAndAddItems", cancellationToken, elementId, values, silent);
    }

    public ValueTask ClearAndAddOptions(string elementId, IEnumerable<TomSelectOption> data, bool silent = false, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.clearAndAddOptions", cancellationToken, elementId, data, silent);
    }

    public ValueTask AddItem(string elementId, string value, bool silent = false, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.addItem", cancellationToken, elementId, value, silent);
    }

    public ValueTask AddItems(string elementId, IEnumerable<string> values, bool silent = false, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.addItems", cancellationToken, elementId, values, silent);
    }

    public ValueTask RemoveItem(string elementId, string valueOrHtmlElement, bool silent = false, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.removeItem", cancellationToken, elementId, valueOrHtmlElement, silent);
    }

    public ValueTask RefreshItems(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.refreshItems", cancellationToken, elementId);
    }

    public ValueTask AddOptionGroup(string elementId, string id, object data, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.addOptionGroup", cancellationToken, elementId, id, data);
    }

    public ValueTask RemoveOptionGroup(string elementId, string id, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.removeOptionGroup", cancellationToken, elementId, id);
    }

    public ValueTask ClearOptionGroups(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.clearOptionGroups", cancellationToken, elementId);
    }

    public ValueTask OpenDropdown(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.open", cancellationToken, elementId);
    }

    public ValueTask CloseDropdown(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.close", cancellationToken, elementId);
    }

    public ValueTask PositionDropdown(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.positionDropdown", cancellationToken, elementId);
    }

    public ValueTask Focus(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.focus", cancellationToken, elementId);
    }

    public ValueTask Blur(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.blur", cancellationToken, elementId);
    }

    public ValueTask Lock(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.lock", cancellationToken, elementId);
    }

    public ValueTask Unlock(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.unlock", cancellationToken, elementId);
    }

    public ValueTask Enable(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.enable", cancellationToken, elementId);
    }

    public ValueTask Disable(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.disable", cancellationToken, elementId);
    }

    public ValueTask SetValue(string elementId, TomSelectOption value, bool silent = false, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.setValue", cancellationToken, elementId, value, silent);
    }

    public ValueTask<TomSelectOption> GetValue(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeAsync<TomSelectOption>($"{_moduleName}.getValue", cancellationToken, elementId);
    }

    public ValueTask SetCaret(string elementId, int index, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.setCaret", cancellationToken, elementId, index);
    }

    public ValueTask<bool> IsFull(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeAsync<bool>($"{_moduleName}.isFull", cancellationToken, elementId);
    }

    public ValueTask ClearCache(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.clearCache", cancellationToken, elementId);
    }

    public ValueTask SetTextboxValue(string elementId, string str, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.setTextboxValue", cancellationToken, elementId, str);
    }

    public ValueTask Sync(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{_moduleName}.sync", cancellationToken, elementId);
    }

    public async ValueTask DisposeAsync()
    {
        await _resourceLoader.DisposeModule(_module);

        await _scriptInitializer.DisposeAsync();
    }
}

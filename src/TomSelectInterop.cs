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
using Soenneker.Asyncs.Initializers;
using Soenneker.Blazor.Utils.ResourceLoader.Abstract;
using Soenneker.Extensions.CancellationTokens;
using Soenneker.Utils.CancellationScopes;

namespace Soenneker.Blazor.TomSelect;

/// <inheritdoc cref="ITomSelectInterop"/>
public sealed class TomSelectInterop : EventListeningInterop, ITomSelectInterop
{
    private readonly IResourceLoader _resourceLoader;
    private readonly AsyncInitializer<bool> _scriptInitializer;

    private const string _module = "Soenneker.Blazor.TomSelect/js/tomselectinterop.js";
    private const string _moduleName = "TomSelectInterop";

    private readonly CancellationScope _cancellationScope = new();

    public TomSelectInterop(IJSRuntime jSRuntime, IResourceLoader resourceLoader) : base(jSRuntime)
    {
        _resourceLoader = resourceLoader;

        _scriptInitializer = new AsyncInitializer<bool>(InitializeScript);
    }

    private async ValueTask InitializeScript(bool useCdn, CancellationToken token)
    {
        if (useCdn)
        {
            await _resourceLoader.LoadStyle("https://cdn.jsdelivr.net/npm/tom-select@2.4.3/dist/css/tom-select.bootstrap5.min.css",
                                     "sha256-lQMtfzgdbG8ufMCU5UThXG65Wsv5CIXGkHFGCHA68ME=", cancellationToken: token);
            await _resourceLoader.LoadScriptAndWaitForVariable("https://cdn.jsdelivr.net/npm/tom-select@2.4.3/dist/js/tom-select.complete.min.js",
                                     "TomSelect", "sha256-t5cAXPIzePs4RIuA3FejMxOlxXe4QXZXQ7sfKJxNU+Y=", cancellationToken: token);
        }
        else
        {
            await _resourceLoader.LoadStyle("_content/Soenneker.Blazor.TomSelect/css/tom-select.bootstrap5.min.css", cancellationToken: token);
            await _resourceLoader.LoadScriptAndWaitForVariable("_content/Soenneker.Blazor.TomSelect/js/tom-select.complete.min.js", "TomSelect",
                                     cancellationToken: token);
        }

        await _resourceLoader.ImportModuleAndWaitUntilAvailable(_module, _moduleName, 100, token);
    }

    public ValueTask Initialize(bool useCdn = true, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return _scriptInitializer.Init(useCdn, linked);
    }

    public ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.createObserver", linked, elementId);
    }

    public async ValueTask Create(ElementReference elementReference, string elementId, DotNetObjectReference<BaseTomSelect> dotNetObjectRef,
        TomSelectConfiguration? configuration = null, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            await _scriptInitializer.Init(configuration?.UseCdn ?? true, linked);

            string? json = null;

            if (configuration != null)
                json = JsonUtil.Serialize(configuration);

            await JsRuntime.InvokeVoidAsync("TomSelectInterop.create", linked, elementReference, elementId, json, dotNetObjectRef);
        }
    }

    public ValueTask Destroy(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.destroy", linked, elementId);
    }

    public ValueTask AddOption(string elementId, TomSelectOption tomSelectOption, bool userCreated = false, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.addOption", linked, elementId, tomSelectOption, userCreated);
    }

    public ValueTask AddOptions(string elementId, IEnumerable<TomSelectOption> data, bool userCreated = false, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.addOptions", linked, elementId, data, userCreated);
    }

    public ValueTask UpdateOption(string elementId, string value, TomSelectOption data, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.updateOption", linked, elementId, value, data);
    }

    public ValueTask RemoveOption(string elementId, string value, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.removeOption", linked, elementId, value);
    }

    public ValueTask RefreshOptions(string elementId, bool triggerDropdown, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.refreshOptions", linked, elementId, triggerDropdown);
    }

    public ValueTask ClearOptions(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.clearOptions", linked, elementId);
    }

    public ValueTask ClearItems(string elementId, bool silent = false, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.clearItems", linked, elementId, silent);
    }

    public ValueTask ClearAndAddItems(string elementId, IEnumerable<string> values, bool silent = false, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.clearAndAddItems", linked, elementId, values, silent);
    }

    public ValueTask ClearAndAddOptions(string elementId, IEnumerable<TomSelectOption> data, bool silent = false, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.clearAndAddOptions", linked, elementId, data, silent);
    }

    public ValueTask AddItem(string elementId, string value, bool silent = false, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.addItem", linked, elementId, value, silent);
    }

    public ValueTask AddItems(string elementId, IEnumerable<string> values, bool silent = false, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.addItems", linked, elementId, values, silent);
    }

    public ValueTask RemoveItem(string elementId, string valueOrHtmlElement, bool silent = false, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.removeItem", linked, elementId, valueOrHtmlElement, silent);
    }

    public ValueTask RefreshItems(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.refreshItems", linked, elementId);
    }

    public ValueTask AddOptionGroup(string elementId, string id, object data, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.addOptionGroup", linked, elementId, id, data);
    }

    public ValueTask RemoveOptionGroup(string elementId, string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.removeOptionGroup", linked, elementId, id);
    }

    public ValueTask ClearOptionGroups(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.clearOptionGroups", linked, elementId);
    }

    public ValueTask OpenDropdown(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.open", linked, elementId);
    }

    public ValueTask CloseDropdown(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.close", linked, elementId);
    }

    public ValueTask PositionDropdown(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.positionDropdown", linked, elementId);
    }

    public ValueTask Focus(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.focus", linked, elementId);
    }

    public ValueTask Blur(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.blur", linked, elementId);
    }

    public ValueTask Lock(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.lock", linked, elementId);
    }

    public ValueTask Unlock(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.unlock", linked, elementId);
    }

    public ValueTask Enable(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.enable", linked, elementId);
    }

    public ValueTask Disable(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.disable", linked, elementId);
    }

    public ValueTask SetValue(string elementId, TomSelectOption value, bool silent = false, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.setValue", linked, elementId, value, silent);
    }

    public ValueTask<TomSelectOption> GetValue(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeAsync<TomSelectOption>("TomSelectInterop.getValue", linked, elementId);
    }

    public ValueTask SetCaret(string elementId, int index, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.setCaret", linked, elementId, index);
    }

    public ValueTask<bool> IsFull(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeAsync<bool>("TomSelectInterop.isFull", linked, elementId);
    }

    public ValueTask ClearCache(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.clearCache", linked, elementId);
    }

    public ValueTask SetTextboxValue(string elementId, string str, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.setTextboxValue", linked, elementId, str);
    }

    public ValueTask Sync(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return JsRuntime.InvokeVoidAsync("TomSelectInterop.sync", linked, elementId);
    }

    public async ValueTask DisposeAsync()
    {
        await _resourceLoader.DisposeModule(_module);

        await _scriptInitializer.DisposeAsync();
        await _cancellationScope.DisposeAsync();
    }
}

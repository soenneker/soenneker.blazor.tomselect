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

    public async ValueTask Initialize(bool useCdn = true, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _scriptInitializer.Init(useCdn, linked);
    }

    public async ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.createObserver", linked, elementId);
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

    public async ValueTask Destroy(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.destroy", linked, elementId);
    }

    public async ValueTask AddOption(string elementId, TomSelectOption tomSelectOption, bool userCreated = false, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.addOption", linked, elementId, tomSelectOption, userCreated);
    }

    public async ValueTask AddOptions(string elementId, IEnumerable<TomSelectOption> data, bool userCreated = false, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.addOptions", linked, elementId, data, userCreated);
    }

    public async ValueTask UpdateOption(string elementId, string value, TomSelectOption data, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.updateOption", linked, elementId, value, data);
    }

    public async ValueTask RemoveOption(string elementId, string value, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.removeOption", linked, elementId, value);
    }

    public async ValueTask RefreshOptions(string elementId, bool triggerDropdown, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.refreshOptions", linked, elementId, triggerDropdown);
    }

    public async ValueTask ClearOptions(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.clearOptions", linked, elementId);
    }

    public async ValueTask ClearItems(string elementId, bool silent = false, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.clearItems", linked, elementId, silent);
    }

    public async ValueTask ClearAndAddItems(string elementId, IEnumerable<string> values, bool silent = false, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.clearAndAddItems", linked, elementId, values, silent);
    }

    public async ValueTask ClearAndAddOptions(string elementId, IEnumerable<TomSelectOption> data, bool silent = false, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.clearAndAddOptions", linked, elementId, data, silent);
    }

    public async ValueTask AddItem(string elementId, string value, bool silent = false, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.addItem", linked, elementId, value, silent);
    }

    public async ValueTask AddItems(string elementId, IEnumerable<string> values, bool silent = false, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.addItems", linked, elementId, values, silent);
    }

    public async ValueTask RemoveItem(string elementId, string valueOrHtmlElement, bool silent = false, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.removeItem", linked, elementId, valueOrHtmlElement, silent);
    }

    public async ValueTask RefreshItems(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.refreshItems", linked, elementId);
    }

    public async ValueTask AddOptionGroup(string elementId, string id, object data, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.addOptionGroup", linked, elementId, id, data);
    }

    public async ValueTask RemoveOptionGroup(string elementId, string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.removeOptionGroup", linked, elementId, id);
    }

    public async ValueTask ClearOptionGroups(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.clearOptionGroups", linked, elementId);
    }

    public async ValueTask OpenDropdown(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.open", linked, elementId);
    }

    public async ValueTask CloseDropdown(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.close", linked, elementId);
    }

    public async ValueTask PositionDropdown(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.positionDropdown", linked, elementId);
    }

    public async ValueTask Focus(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.focus", linked, elementId);
    }

    public async ValueTask Blur(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.blur", linked, elementId);
    }

    public async ValueTask Lock(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.lock", linked, elementId);
    }

    public async ValueTask Unlock(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.unlock", linked, elementId);
    }

    public async ValueTask Enable(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.enable", linked, elementId);
    }

    public async ValueTask Disable(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.disable", linked, elementId);
    }

    public async ValueTask SetValue(string elementId, TomSelectOption value, bool silent = false, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.setValue", linked, elementId, value, silent);
    }

    public async ValueTask<TomSelectOption> GetValue(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return await JsRuntime.InvokeAsync<TomSelectOption>("TomSelectInterop.getValue", linked, elementId);
    }

    public async ValueTask SetCaret(string elementId, int index, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.setCaret", linked, elementId, index);
    }

    public async ValueTask<bool> IsFull(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return await JsRuntime.InvokeAsync<bool>("TomSelectInterop.isFull", linked, elementId);
    }

    public async ValueTask ClearCache(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.clearCache", linked, elementId);
    }

    public async ValueTask SetTextboxValue(string elementId, string str, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.setTextboxValue", linked, elementId, str);
    }

    public async ValueTask Sync(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.sync", linked, elementId);
    }

    public async ValueTask DisposeAsync()
    {
        await _resourceLoader.DisposeModule(_module);

        await _scriptInitializer.DisposeAsync();
        await _cancellationScope.DisposeAsync();
    }
}

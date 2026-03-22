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
    private readonly HashSet<string> _loadedStyles = [];
    private readonly SemaphoreSlim _styleSemaphore = new(1, 1);

    private const string _module = "Soenneker.Blazor.TomSelect/js/tomselectinterop.js";
    private const string _bootstrap5CdnStylePath = "https://cdn.jsdelivr.net/npm/tom-select@2.5.2/dist/css/tom-select.bootstrap5.min.css";
    private const string _bootstrap5CdnStyleIntegrity = "sha256-Re4GjTaUjj1cCjvRlSe/GXl4eWsdaw9i6rGP3dZAz0U=";
    private const string _regularCdnStylePath = "https://cdn.jsdelivr.net/npm/tom-select@2.5.2/dist/css/tom-select.min.css";
    private const string _regularCdnStyleIntegrity = "sha256-Bz8grFpl3OF4fFRKz8HaIQ9TSBXis6YhioC7jFESW0c=";
    private const string _cdnScriptPath = "https://cdn.jsdelivr.net/npm/tom-select@2.5.2/dist/js/tom-select.complete.min.js";
    private const string _cdnScriptIntegrity = "sha256-gNr/eWjW3Y6HYubw6H8pd1rkFPbH1+5aU/F1hpjNIS4=";
    private const string _bootstrap5LocalStylePath = "_content/Soenneker.Blazor.TomSelect/css/tom-select.bootstrap5.min.css";
    private const string _regularLocalStylePath = "_content/Soenneker.Blazor.TomSelect/css/tom-select.min.css";
    private const string _localScriptPath = "_content/Soenneker.Blazor.TomSelect/js/tom-select.complete.min.js";

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
            await _resourceLoader.LoadScriptAndWaitForVariable(_cdnScriptPath, "TomSelect", _cdnScriptIntegrity, cancellationToken: token);
        }
        else
        {
            await _resourceLoader.LoadScriptAndWaitForVariable(_localScriptPath, "TomSelect", cancellationToken: token);
        }

        await _resourceLoader.ImportModule(_module, token);
    }

    private static (string Path, string? Integrity) GetStyleResource(bool useCdn, bool useBootstrap5Styling)
    {
        if (useCdn)
        {
            return useBootstrap5Styling
                ? (_bootstrap5CdnStylePath, _bootstrap5CdnStyleIntegrity)
                : (_regularCdnStylePath, _regularCdnStyleIntegrity);
        }

        return useBootstrap5Styling
            ? (_bootstrap5LocalStylePath, null)
            : (_regularLocalStylePath, null);
    }

    private async ValueTask EnsureStyleLoaded(bool useCdn, bool useBootstrap5Styling, CancellationToken cancellationToken)
    {
        (string path, string? integrity) = GetStyleResource(useCdn, useBootstrap5Styling);

        await _styleSemaphore.WaitAsync(cancellationToken);

        try
        {
            if (_loadedStyles.Contains(path))
                return;

            if (integrity != null)
                await _resourceLoader.LoadStyle(path, integrity, cancellationToken: cancellationToken);
            else
                await _resourceLoader.LoadStyle(path, cancellationToken: cancellationToken);

            _loadedStyles.Add(path);
        }
        finally
        {
            _styleSemaphore.Release();
        }
    }

    public ValueTask Initialize(bool useCdn = true, CancellationToken cancellationToken = default)
    {
        var configuration = new TomSelectConfiguration
        {
            UseCdn = useCdn
        };

        return Initialize(configuration, cancellationToken);
    }

    public async ValueTask Initialize(TomSelectConfiguration? configuration, CancellationToken cancellationToken = default)
    {
        bool useCdn = configuration?.UseCdn ?? true;
        bool useBootstrap5Styling = configuration?.UseBootstrap5Styling ?? true;

        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await _scriptInitializer.Init(useCdn, linked);
            await EnsureStyleLoaded(useCdn, useBootstrap5Styling, linked);
        }
    }

    public async ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.createObserver", linked, elementId);
    }

    public async ValueTask Create(ElementReference elementReference, string elementId, DotNetObjectReference<BaseTomSelect> dotNetObjectRef,
        TomSelectConfiguration? configuration = null, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            bool useCdn = configuration?.UseCdn ?? true;
            bool useBootstrap5Styling = configuration?.UseBootstrap5Styling ?? true;

            await _scriptInitializer.Init(useCdn, linked);
            await EnsureStyleLoaded(useCdn, useBootstrap5Styling, linked);

            string? json = null;

            if (configuration != null)
                json = JsonUtil.Serialize(configuration);

            await JsRuntime.InvokeVoidAsync("TomSelectInterop.create", linked, elementReference, elementId, json, dotNetObjectRef);
        }
    }

    public async ValueTask Destroy(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.destroy", linked, elementId);
    }

    public async ValueTask AddOption(string elementId, TomSelectOption tomSelectOption, bool userCreated = false, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.addOption", linked, elementId, tomSelectOption, userCreated);
    }

    public async ValueTask AddOptions(string elementId, IEnumerable<TomSelectOption> data, bool userCreated = false,
        CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.addOptions", linked, elementId, data, userCreated);
    }

    public async ValueTask UpdateOption(string elementId, string value, TomSelectOption data, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.updateOption", linked, elementId, value, data);
    }

    public async ValueTask RemoveOption(string elementId, string value, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.removeOption", linked, elementId, value);
    }

    public async ValueTask RefreshOptions(string elementId, bool triggerDropdown, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.refreshOptions", linked, elementId, triggerDropdown);
    }

    public async ValueTask ClearOptions(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.clearOptions", linked, elementId);
    }

    public async ValueTask ClearItems(string elementId, bool silent = false, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.clearItems", linked, elementId, silent);
    }

    public async ValueTask ClearAndAddItems(string elementId, IEnumerable<string> values, bool silent = false, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.clearAndAddItems", linked, elementId, values, silent);
    }

    public async ValueTask ClearAndAddOptions(string elementId, IEnumerable<TomSelectOption> data, bool silent = false,
        CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.clearAndAddOptions", linked, elementId, data, silent);
    }

    public async ValueTask AddItem(string elementId, string value, bool silent = false, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.addItem", linked, elementId, value, silent);
    }

    public async ValueTask AddItems(string elementId, IEnumerable<string> values, bool silent = false, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.addItems", linked, elementId, values, silent);
    }

    public async ValueTask RemoveItem(string elementId, string valueOrHtmlElement, bool silent = false, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.removeItem", linked, elementId, valueOrHtmlElement, silent);
    }

    public async ValueTask RefreshItems(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.refreshItems", linked, elementId);
    }

    public async ValueTask AddOptionGroup(string elementId, string id, object data, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.addOptionGroup", linked, elementId, id, data);
    }

    public async ValueTask RemoveOptionGroup(string elementId, string id, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.removeOptionGroup", linked, elementId, id);
    }

    public async ValueTask ClearOptionGroups(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.clearOptionGroups", linked, elementId);
    }

    public async ValueTask OpenDropdown(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.open", linked, elementId);
    }

    public async ValueTask CloseDropdown(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.close", linked, elementId);
    }

    public async ValueTask PositionDropdown(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.positionDropdown", linked, elementId);
    }

    public async ValueTask Focus(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.focus", linked, elementId);
    }

    public async ValueTask Blur(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.blur", linked, elementId);
    }

    public async ValueTask Lock(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.lock", linked, elementId);
    }

    public async ValueTask Unlock(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.unlock", linked, elementId);
    }

    public async ValueTask Enable(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.enable", linked, elementId);
    }

    public async ValueTask Disable(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.disable", linked, elementId);
    }

    public async ValueTask SetValue(string elementId, TomSelectOption value, bool silent = false, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.setValue", linked, elementId, value, silent);
    }

    public async ValueTask<TomSelectOption> GetValue(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            return await JsRuntime.InvokeAsync<TomSelectOption>("TomSelectInterop.getValue", linked, elementId);
    }

    public async ValueTask SetCaret(string elementId, int index, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.setCaret", linked, elementId, index);
    }

    public async ValueTask<bool> IsFull(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            return await JsRuntime.InvokeAsync<bool>("TomSelectInterop.isFull", linked, elementId);
    }

    public async ValueTask ClearCache(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.clearCache", linked, elementId);
    }

    public async ValueTask SetTextboxValue(string elementId, string str, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.setTextboxValue", linked, elementId, str);
    }

    public async ValueTask Sync(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await JsRuntime.InvokeVoidAsync("TomSelectInterop.sync", linked, elementId);
    }

    public async ValueTask DisposeAsync()
    {
        await _resourceLoader.DisposeModule(_module);

        await _scriptInitializer.DisposeAsync();
        await _cancellationScope.DisposeAsync();
        _styleSemaphore.Dispose();
    }
}
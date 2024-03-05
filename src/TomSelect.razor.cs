using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Soenneker.Blazor.TomSelect.Configuration;
using Soenneker.Blazor.TomSelect.Dtos;
using Soenneker.Extensions.String;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Blazor.TomSelect.Base;
using Soenneker.Extensions.Enumerable;
using Soenneker.Blazor.TomSelect.Abstract;
using Soenneker.Extensions.List;
using Microsoft.Extensions.Logging;

namespace Soenneker.Blazor.TomSelect;

///<inheritdoc cref="ITomSelect{TItem, TType}"/>
public partial class TomSelect<TItem, TType> : BaseTomSelect
{
    [Parameter, EditorRequired]
    public IEnumerable<TItem> Data { get; set; } = default!;

    public List<TItem> UserCreatedData { get; set; } = [];

    [Parameter, EditorRequired]
    public Func<TItem, string?> TextField { get; set; } = default!;

    [Parameter, EditorRequired]
    public Func<TItem, string?> ValueField { get; set; } = default!;

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? Attributes { get; set; }

    [Parameter]
    public TomSelectConfiguration Configuration { get; set; } = new();

    private readonly List<TomSelectOption> _options = [];

    #region Events

    [Parameter]
    public EventCallback OnInitialize { get; set; }

    [Parameter]
    public EventCallback<string> OnChange { get; set; }

    [Parameter]
    public EventCallback OnFocus { get; set; }

    [Parameter]
    public EventCallback OnBlur { get; set; }

    [Parameter]
    public EventCallback<(string Value, TomSelectOption Item)> OnItemAdd { get; set; }

    [Parameter]
    public EventCallback<(string Value, TomSelectOption Item)> OnItemRemove { get; set; }

    [Parameter]
    public EventCallback<object> OnItemSelect { get; set; }

    [Parameter]
    public EventCallback OnClear { get; set; }

    [Parameter]
    public EventCallback<(string Value, TomSelectOption Data)> OnOptionAdd { get; set; }

    [Parameter]
    public EventCallback<string> OnOptionRemove { get; set; }

    [Parameter]
    public EventCallback OnOptionClear { get; set; }

    [Parameter]
    public EventCallback<(string Id, TomSelectOption Data)> OnOptgroupAdd { get; set; }

    [Parameter]
    public EventCallback<string> OnOptgroupRemove { get; set; }

    [Parameter]
    public EventCallback OnOptgroupClear { get; set; }

    [Parameter]
    public EventCallback<object> OnDropdownOpen { get; set; }

    [Parameter]
    public EventCallback<object> OnDropdownClose { get; set; }

    [Parameter]
    public EventCallback<string> OnType { get; set; }

    [Parameter]
    public EventCallback<object> OnLoad { get; set; }

    [Parameter]
    public EventCallback OnDestroy { get; set; }

    #endregion Events

    [Parameter]
    public bool Multiple { get; set; } = true;

    [Parameter]
    public List<TItem> Items { get; set; } = [];

    private bool _isInitialized;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            InteropEventListener.Initialize(TomSelectInterop);
            await Create();
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!_isInitialized)
        {
            if (Data.Populated())
            {
                _isInitialized = true;

                await AddOptions(Data, false);
            }
        }

        await base.OnParametersSetAsync();
    }

    public async ValueTask Create(TomSelectConfiguration? configuration = null, CancellationToken cancellationToken = default)
    {
        if (configuration != null)
        {
            Configuration = configuration;
        }

        DotNetReference = DotNetObjectReference.Create((BaseTomSelect) this);

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);

        await TomSelectInterop.Create(ElementReference, ElementId, DotNetReference, Configuration, linkedCts.Token);

        await AddEventListeners();
    }

    private TomSelectOption? ToOptionFromItem(TItem item)
    {
        string? value = ValueField.Invoke(item);
        string? text = TextField.Invoke(item);

        if (text == null || value == null)
            return null;

        var tomSelectOption = new TomSelectOption {Text = text, Value = value};

        return tomSelectOption;
    }

    public ValueTask AddOption(TItem item, bool userCreated = true, CancellationToken cancellationToken = default)
    {
        UserCreatedData.Add(item);

        TomSelectOption? option = ToOptionFromItem(item);

        if (option == null)
        {
            Logger.LogWarning("Could not add item, option is null");
            return ValueTask.CompletedTask;
        }

        _options.Add(option);

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.AddOption(ElementId, option, userCreated, linkedCts.Token);
    }

    public async ValueTask AddOptions(IEnumerable<TItem> items, bool userCreated = true, CancellationToken cancellationToken = default)
    {
        var tomSelectOptions = new List<TomSelectOption>();

        foreach (TItem datum in items)
        {
            TomSelectOption? tomSelectOption = ToOptionFromItem(datum);

            if (tomSelectOption == null)
                continue;

            tomSelectOptions.Add(tomSelectOption);
        }

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        await TomSelectInterop.AddOptions(ElementId, tomSelectOptions, userCreated, linkedCts.Token);
    }

    public ValueTask UpdateOption(string value, TItem item, CancellationToken cancellationToken = default)
    {
        TomSelectOption? option = ToOptionFromItem(item);

        if (option == null)
        {
            Logger.LogWarning("Could not update option, option is null");
            return ValueTask.CompletedTask;
        }

        _options.Replace(c => c.Value == value, option);

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.UpdateOption(ElementId, value, option, linkedCts.Token);
    }

    public ValueTask RemoveOption(string value, CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.RemoveOption(ElementId, value, linkedCts.Token);
    }

    public ValueTask RefreshOptions(bool triggerDropdown, CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.RefreshOptions(ElementId, triggerDropdown, linkedCts.Token);
    }

    public ValueTask ClearOptions(CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.ClearOptions(ElementId, linkedCts.Token);
    }

    public ValueTask AddItem(string value, bool silent = false, CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.AddItem(ElementId, value, silent, linkedCts.Token);
    }

    public ValueTask ClearItems(bool silent = false, CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.ClearItems(ElementId, silent, linkedCts.Token);
    }

    public ValueTask RemoveItem(string valueOrHTMLElement, bool silent = false, CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.RemoveItem(ElementId, valueOrHTMLElement, silent, linkedCts.Token);
    }

    public ValueTask RefreshItems(CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.RefreshItems(ElementId, linkedCts.Token);
    }

    public ValueTask AddOptionGroup(string id, object data, CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.AddOptionGroup(ElementId, id, data, linkedCts.Token);
    }

    public ValueTask RemoveOptionGroup(string id, CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.RemoveOptionGroup(ElementId, id, linkedCts.Token);
    }

    public ValueTask ClearOptionGroups(CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.ClearOptionGroups(ElementId, linkedCts.Token);
    }

    public ValueTask OpenDropdown(CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.OpenDropdown(ElementId, linkedCts.Token);
    }

    public ValueTask CloseDropdown(CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.CloseDropdown(ElementId, linkedCts.Token);
    }

    public ValueTask PositionDropdown(CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.PositionDropdown(ElementId, linkedCts.Token);
    }

    public ValueTask Focus(CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.Focus(ElementId, linkedCts.Token);
    }

    public ValueTask Blur(CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.Blur(ElementId, linkedCts.Token);
    }

    public ValueTask Lock(CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.Lock(ElementId, linkedCts.Token);
    }

    public ValueTask Unlock(CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.Unlock(ElementId, linkedCts.Token);
    }

    public ValueTask Enable(CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.Enable(ElementId, linkedCts.Token);
    }

    public ValueTask Disable(CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.Disable(ElementId, linkedCts.Token);
    }

    public ValueTask SetValue(object value, bool silent = false, CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.SetValue(ElementId, value, silent, linkedCts.Token);
    }

    public ValueTask<object> GetValue(CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.GetValue(ElementId, linkedCts.Token);
    }

    public ValueTask SetCaret(int index, CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.SetCaret(ElementId, index, linkedCts.Token);
    }

    public ValueTask<bool> IsFull(CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.IsFull(ElementId, linkedCts.Token);
    }

    public ValueTask ClearCache(CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.ClearCache(ElementId, linkedCts.Token);
    }

    public ValueTask SetTextboxValue(string str, CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.SetTextboxValue(ElementId, str, linkedCts.Token);
    }

    public ValueTask Sync(CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.Sync(ElementId, linkedCts.Token);
    }

    public ValueTask Destroy(CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cTs.Token);
        return TomSelectInterop.Destroy(ElementId, linkedCts.Token);
    }

    private void OnItemAdd_internal(string value)
    {
        TItem? item = ToItemFromValue(value);

        if (item != null)
        {
            Items.Add(item);
        }
    }

    private void OnItemRemove_Internal(string value)
    {
        TItem? item = ToItemFromValue(value);

        if (item != null)
        {
            Items.Remove(item);
        }
    }

    private TItem? ToItemFromValue(string value)
    {
        foreach (TItem item in Data)
        {
            string? result = ValueField?.Invoke(item);

            if (result == value)
                return item;
        }

        foreach (TItem item in UserCreatedData)
        {
            string? result = ValueField?.Invoke(item);

            if (result == value)
                return item;
        }

        return default;
    }

    private void OnItemClear_internal()
    {
        Items.Clear();
    }

    public bool OnOptionAdd_internal(string value, TomSelectOption data)
    {
        TomSelectOption? option = _options.FirstOrDefault(c => c.Value == value);

        if (option != null)
        {
            Logger.LogWarning("Option with value {Value} already exists, skipping add", value);
            return false;
        }

        _options.Add(data);
        return true;
    }

    public void OnOptionRemove_internal(string value)
    {
        TomSelectOption? option = _options.FirstOrDefault(c => c.Value == value);

        if (option != null)
            _options.Remove(option);
    }

    public void OnOptionClear_internal()
    {
        _options.Clear();
    }

    private async ValueTask AddEventListeners()
    {
        // OPTIONS ---
        await AddEventListener<string>(
            GetJsEventName(nameof(OnOptionAdd)),
            async e =>
            {
                JsonDocument jsonDocument = JsonDocument.Parse(e);
                (string, TomSelectOption) parameters = (
                    jsonDocument.RootElement[0].Deserialize<string>(),
                    jsonDocument.RootElement[1].Deserialize<TomSelectOption>()!
                );

                OnOptionAdd_internal(parameters.Item1, parameters.Item2);

                if (OnOptionAdd.HasDelegate)
                    await OnOptionAdd.InvokeAsync(parameters);
            });

        await AddEventListener<string>(
            GetJsEventName(nameof(OnOptionRemove)),
            async e =>
            {
                // OnOptionRemove_Internal();

                if (OnOptionRemove.HasDelegate)
                    await OnOptionRemove.InvokeAsync(e);
            });


        await AddEventListener<string>(
            GetJsEventName(nameof(OnOptionClear)),
            async (e) =>
            {
                OnOptionClear_internal();

                if (OnOptionClear.HasDelegate)
                    await OnOptionClear.InvokeAsync();
            });

        // ITEMS ---

        await AddEventListener<string>(
            GetJsEventName(nameof(OnItemAdd)),
            async e =>
            {
                JsonDocument jsonDocument = JsonDocument.Parse(e);
                (string, TomSelectOption) parameters = (
                    jsonDocument.RootElement[0].Deserialize<string>(),
                    jsonDocument.RootElement[1].Deserialize<TomSelectOption>()!
                );

                OnItemAdd_internal(parameters.Item1);

                if (OnItemAdd.HasDelegate)
                    await OnItemAdd.InvokeAsync(parameters);
            });

        await AddEventListener<string>(
            GetJsEventName(nameof(OnItemRemove)),
            async e =>
            {
                JsonDocument jsonDocument = JsonDocument.Parse(e);
                (string, TomSelectOption) parameters = (
                    jsonDocument.RootElement[0].Deserialize<string>(),
                    jsonDocument.RootElement[1].Deserialize<TomSelectOption>()!
                );

                OnItemRemove_Internal(parameters.Item1);

                if (OnItemRemove.HasDelegate)
                    await OnItemRemove.InvokeAsync(parameters);
            });

        await AddEventListener<object>(
            GetJsEventName(nameof(OnItemSelect)),
            async e =>
            {
                if (OnItemSelect.HasDelegate)
                    await OnItemSelect.InvokeAsync(e);
            });

        await AddEventListener<string>(
            GetJsEventName(nameof(OnClear)),
            async e =>
            {
                OnItemClear_internal();

                if (OnClear.HasDelegate)
                    await OnClear.InvokeAsync();
            });

        if (OnChange.HasDelegate)
        {
            await AddEventListener<string>(
                GetJsEventName(nameof(OnChange)),
                async e => { await OnChange.InvokeAsync(e); });
        }

        if (OnFocus.HasDelegate)
        {
            await AddEventListener<string>(
                GetJsEventName(nameof(OnFocus)),
                async e => { await OnFocus.InvokeAsync(); });
        }

        if (OnBlur.HasDelegate)
        {
            await AddEventListener<string>(
                GetJsEventName(nameof(OnBlur)),
                async (e) => { await OnBlur.InvokeAsync(); });
        }

        if (OnOptgroupAdd.HasDelegate)
        {
            await AddEventListener<string>(
                GetJsEventName(nameof(OnOptgroupAdd)),
                async e =>
                {
                    JsonDocument jsonDocument = JsonDocument.Parse(e);
                    (string, TomSelectOption) parameters = (
                        jsonDocument.RootElement[0].Deserialize<string>(),
                        jsonDocument.RootElement[1].Deserialize<TomSelectOption>()!
                    );

                    await OnOptgroupAdd.InvokeAsync(parameters);
                });
        }

        if (OnOptgroupRemove.HasDelegate)
        {
            await AddEventListener<string>(
                GetJsEventName(nameof(OnOptgroupRemove)),
                async e => { await OnOptgroupRemove.InvokeAsync(e); });
        }

        if (OnOptgroupClear.HasDelegate)
        {
            await AddEventListener<string>(
                GetJsEventName(nameof(OnOptgroupClear)),
                async (e) => { await OnOptgroupClear.InvokeAsync(); });
        }

        if (OnDropdownOpen.HasDelegate)
        {
            await AddEventListener<object>(
                GetJsEventName(nameof(OnDropdownOpen)),
                async e => { await OnDropdownOpen.InvokeAsync(e); });
        }

        if (OnDropdownClose.HasDelegate)
        {
            await AddEventListener<object>(
                GetJsEventName(nameof(OnDropdownClose)),
                async e => { await OnDropdownClose.InvokeAsync(e); });
        }

        if (OnType.HasDelegate)
        {
            await AddEventListener<string>(
                GetJsEventName(nameof(OnType)),
                async e => { await OnType.InvokeAsync(e); });
        }

        if (OnLoad.HasDelegate)
        {
            await AddEventListener<object>(
                GetJsEventName(nameof(OnLoad)),
                async e => { await OnLoad.InvokeAsync(e); });
        }

        if (OnDestroy.HasDelegate)
        {
            await AddEventListener<string>(
                GetJsEventName(nameof(OnDestroy)),
                async (e) => { await OnDestroy.InvokeAsync(); });
        }
    }

    private static string GetJsEventName(string callback)
    {
        // Remove first two characters
        string subStr = callback[2..];
        string result = subStr.ToSnakeCaseFromPascal();
        return result;
    }

    private ValueTask AddEventListener<T>(string eventName, Func<T, ValueTask> callback)
    {
        return InteropEventListener.Add("tomSelectInterop.addEventListener", ElementId, eventName, callback, _cTs.Token);
    }

    private ValueTask AddEventListener<TInput, TOutput>(string eventName, Func<TInput, ValueTask<TOutput>> callback)
    {
        return InteropEventListener.Add("tomSelectInterop.addOutputEventListener", ElementId, eventName, callback, _cTs.Token);
    }

    [JSInvokable("OnInitializedJs")]
    public async Task OnInitializedJs()
    {
        if (OnInitialize.HasDelegate)
            await OnInitialize.InvokeAsync();
    }
}
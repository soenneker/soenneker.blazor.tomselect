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
using Soenneker.Extensions.ValueTask;

namespace Soenneker.Blazor.TomSelect;

///<inheritdoc cref="ITomSelect{TItem, TType}"/>
public partial class TomSelect<TItem, TType> : BaseTomSelect
{
    [Parameter, EditorRequired]
    public IEnumerable<TItem>? Data { get; set; }

    [Parameter, EditorRequired]
    public Func<TItem, string?> TextField { get; set; } = default!;

    [Parameter, EditorRequired]
    public Func<TItem, string?> ValueField { get; set; } = default!;

    [Parameter]
    public Func<string, TItem>? CreateFuncSync { get; set; }

    [Parameter]
    public Func<string, ValueTask<TItem>>? CreateFunc { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object?>? Attributes { get; set; }

    [Parameter]
    public TomSelectConfiguration Configuration { get; set; } = new();

    [Parameter]
    public bool Multiple { get; set; } = true;

    [Parameter]
    public bool Create { get; set; }

    [Parameter]
    public List<TItem> Items { get; set; } = default!;

    private int _itemsHash;
    private int _optionsHash;

    private bool _isCreated;
    private bool _isDataSet;

    private readonly List<TomSelectOption> _workingOptions = [];

    private TaskCompletionSource<bool> _onModificationTask = new TaskCompletionSource<bool>();

    protected override async Task OnInitializedAsync()
    {
        await TomSelectInterop.Initialize();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            Items ??= [];

            _isDataSet = false;
            InteropEventListener.Initialize(TomSelectInterop);
            await Initialize();
            _isCreated = true;
        }

        if (!_isDataSet && _isCreated)
        {
            _isDataSet = true;

            await InitializeInternal();
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!_isDataSet || !_isCreated)
            return;

        if (Data != null)
        {
            int optionsHashCode = Data.GetAggregateHashCode();

            if (_optionsHash != optionsHashCode)
            {
                //Logger.LogDebug("Setting new options");

                _optionsHash = optionsHashCode;

                List<TomSelectOption> tomSelectOptions = ConvertItemsToOptions(Data);

                await TomSelectInterop.ClearAndAddOptions(ElementId, tomSelectOptions, true);
            }
        }

        int itemsHashCode = Items.GetAggregateHashCode();

        if (_itemsHash != itemsHashCode)
        {
            //Logger.LogDebug("Setting new items");

            _itemsHash = itemsHashCode;

            List<string> values = ConvertItemsToListString(Items);
            await TomSelectInterop.ClearAndAddItems(ElementId, values, true);
        }
    }

    public override async ValueTask Reinitialize()
    {
        await ClearItems(true);

        await ClearOptions();

        await InitializeInternal();
    }

    private async ValueTask InitializeInternal()
    {
        _itemsHash = Items.GetAggregateHashCode();

        if (Data == null)
            return;

        _optionsHash = Data.GetAggregateHashCode();

        if (Data.Any())
        {
            await AddOptions(Data, false);

            if (Items.Populated())
                await AddItems(Items, true);
        }
    }

    public async ValueTask Initialize(TomSelectConfiguration? configuration = null, CancellationToken cancellationToken = default)
    {
        if (configuration != null)
            Configuration = configuration;

        if (Create)
            Configuration.Create = true;

        DotNetReference = DotNetObjectReference.Create((BaseTomSelect) this);

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, CTs.Token);
        await TomSelectInterop.Create(ElementReference, ElementId, DotNetReference, Configuration, linkedCts.Token);
        await TomSelectInterop.CreateObserver(ElementId, cancellationToken);

        await AddEventListeners().NoSync();
    }

    public async ValueTask<TomSelectOption?> AddOption(TItem item, bool userCreated = true, CancellationToken cancellationToken = default)
    {
        TomSelectOption? option = CreateOptionFromItem(item);

        if (option == null)
        {
            Logger.LogWarning("Could not add item, option is null");
            return null;
        }

        _workingOptions.Add(option);

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, CTs.Token);
        await TomSelectInterop.AddOption(ElementId, option, userCreated, linkedCts.Token);

        return option;
    }

    public async ValueTask AddOptions(IEnumerable<TItem> items, bool userCreated = true, CancellationToken cancellationToken = default)
    {
        List<TomSelectOption> tomSelectOptions = ConvertItemsToOptions(items);

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, CTs.Token);
        await TomSelectInterop.AddOptions(ElementId, tomSelectOptions, userCreated, linkedCts.Token);
    }

    public ValueTask UpdateOption(string value, TItem item, CancellationToken cancellationToken = default)
    {
        TomSelectOption? option = CreateOptionFromItem(item);

        if (option == null)
        {
            Logger.LogWarning("Could not update option, option is null");
            return ValueTask.CompletedTask;
        }

        _workingOptions.Replace(c => c.Value == value, option);

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, CTs.Token);
        return TomSelectInterop.UpdateOption(ElementId, value, option, linkedCts.Token);
    }

    public ValueTask AddItem(TItem item, bool silent = false, CancellationToken cancellationToken = default)
    {
        string? value = ToValueFromItem(item);

        return AddItem(value!, silent, cancellationToken);
    }

    public ValueTask AddItems(IEnumerable<TItem> items, bool silent = false, CancellationToken cancellationToken = default)
    {
        List<string> values = ConvertItemsToListString(items);

        return AddItems(values, silent, cancellationToken);
    }

    private List<TomSelectOption> ConvertItemsToOptions(IEnumerable<TItem> items)
    {
        var tomSelectOptions = new List<TomSelectOption>();

        foreach (TItem item in items)
        {
            TomSelectOption? tomSelectOption = CreateOptionFromItem(item);

            if (tomSelectOption == null)
                continue;

            tomSelectOptions.Add(tomSelectOption);
        }

        return tomSelectOptions;
    }

    private List<string> ConvertItemsToListString(IEnumerable<TItem> items)
    {
        HashSet<string> values = [];

        foreach (TItem item in items)
        {
            string? value = ToValueFromItem(item);

            if (value == null)
                continue;

            values.Add(value);
        }

        return values.ToList();
    }

    private TomSelectOption? CreateOptionFromItem(TItem item)
    {
        string? value = ToValueFromItem(item);
        string? text = ToTextFromItem(item);

        if (text == null || value == null)
            return null;

        var tomSelectOption = new TomSelectOption
        {
            Text = text, Value = value,
            Item = item
        };

        return tomSelectOption;
    }

    private TomSelectOption? GetOptionFromValue(string value)
    {
        return _workingOptions.FirstOrDefault(option => option.Value == value);
    }

    private TomSelectOption? GetOptionFromText(string text)
    {
        return _workingOptions.FirstOrDefault(option => option.Text == text);
    }

    private TItem? ToItemFromText(string text)
    {
        if (Data != null)
        {
            foreach (TItem item in Data)
            {
                string? result = ToTextFromItem(item);

                if (result == text)
                    return item;
            }
        }

        foreach (TomSelectOption option in _workingOptions)
        {
            if (option.Value == text)
                return (TItem) option.Item!;
        }

        return default;
    }


    private TItem? ToItemFromValue(string value)
    {
        if (Data != null)
        {
            foreach (TItem item in Data)
            {
                string? result = ToValueFromItem(item);

                if (result == value)
                    return item;
            }
        }

        foreach (TomSelectOption option in _workingOptions)
        {
            if (option.Value == value)
                return (TItem) option.Item!;
        }

        return default;
    }

    private string? ToValueFromItem(TItem item)
    {
        return ValueField.Invoke(item);
    }

    private string? ToTextFromItem(TItem item)
    {
        return TextField.Invoke(item);
    }

    private async ValueTask<TItem> CreateItemFromValue(string text)
    {
        if (CreateFunc == null && CreateFuncSync == null)
            throw new Exception("Cannot create a new item without `CreateFunc` or `CreatedFuncSync` being defined on the TomSelect");

        if (CreateFunc != null && CreateFuncSync != null)
            throw new Exception("`CreateFunc` and `CreatedFuncSync` cannot both be defined on the TomSelect");

        if (CreateFuncSync != null)
            return CreateFuncSync.Invoke(text);

        if (CreateFunc != null)
            return await CreateFunc.Invoke(text);

        return default!;
    }

    private async ValueTask OnItemAdd_internal(string valueOrText)
    {
        TItem? item = ToItemFromValue(valueOrText) ?? ToItemFromText(valueOrText);

        if (item == null)
        {
            await OnOptionCreated_internal(valueOrText);

            return;
        }

        if (!Items.Contains(item))
        {
            Items.Add(item);
            _itemsHash = Items.GetAggregateHashCode();
        }
    }

    private async ValueTask OnOptionCreated_internal(string value)
    {
        TItem item = await CreateItemFromValue(value);

        // Unfortunately we need to remove the option (stored via value) so we can re-add the properly built one from the component
        await RemoveOption(value);
        TomSelectOption? newOption = await AddOption(item, false);

        if (OnItemCreated.HasDelegate)
        {
            if (newOption != null)
                await OnItemCreated.InvokeAsync((value, newOption));
        }
    }

    private void OnItemRemove_Internal(string valueOrText)
    {
        TItem? item = ToItemFromValue(valueOrText) ?? ToItemFromText(valueOrText);

        if (item == null)
            return;

        if (Items.Contains(item))
        {
            Items.Remove(item);
            _itemsHash = Items.GetAggregateHashCode();
        }
    }

    private void OnClearItems_internal()
    {
        Items.Clear();
        _itemsHash = Items.GetAggregateHashCode();
    }

    private bool OnOptionAdd_internal(string value, TomSelectOption data)
    {
        TomSelectOption? option = _workingOptions.FirstOrDefault(c => c.Value == value);

        if (option != null)
        {
            Logger.LogWarning("Option with value {Value} already exists, skipping add", value);
            return false;
        }

        _workingOptions.Add(data);
        return true;
    }

    private void OnOptionRemove_internal(string value)
    {
        TomSelectOption? option = _workingOptions.FirstOrDefault(c => c.Value == value);

        if (option != null)
            _workingOptions.Remove(option);
    }

    private void OnOptionClear_internal()
    {
        _workingOptions.Clear();
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
                    jsonDocument.RootElement[0].Deserialize<string>()!,
                    jsonDocument.RootElement[1].Deserialize<TomSelectOption>()!
                );

                //  OnOptionAdd_internal(parameters.Item1, parameters.Item2);

                if (OnOptionAdd.HasDelegate)
                    await OnOptionAdd.InvokeAsync(parameters);
            });

        await AddEventListener<string>(
            GetJsEventName(nameof(OnOptionRemove)),
            async e =>
            {
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
                    jsonDocument.RootElement[0].Deserialize<string>()!,
                    jsonDocument.RootElement[1].Deserialize<TomSelectOption>()!
                );

                await OnItemAdd_internal(parameters.Item1);

                if (OnItemAdd.HasDelegate)
                    await OnItemAdd.InvokeAsync(parameters);

                _onModificationTask.TrySetResult(true);
            });

        await AddEventListener<string>(
            GetJsEventName(nameof(OnItemRemove)),
            async e =>
            {
                JsonDocument jsonDocument = JsonDocument.Parse(e);
                (string, TomSelectOption) parameters = (
                    jsonDocument.RootElement[0].Deserialize<string>()!,
                    jsonDocument.RootElement[1].Deserialize<TomSelectOption>()!
                );

                OnItemRemove_Internal(parameters.Item1);

                if (OnItemRemove.HasDelegate)
                    await OnItemRemove.InvokeAsync(parameters);

                _onModificationTask.TrySetResult(true);
            });

        await AddEventListener<string>(
            GetJsEventName(nameof(OnItemSelect)),
            async e =>
            {
                TItem? item = ToItemFromValue(e);

                if (item != null)
                {
                    TomSelectOption? option = CreateOptionFromItem(item);

                    if (OnItemSelect.HasDelegate)
                        await OnItemSelect.InvokeAsync(option);
                }
            });

        // TODO: There's a bug in the JS that raises the clear event when an item is selected 04/04/24
        //await AddEventListener<string>(
        //    "clear",
        //    async e =>
        //    {
        //        OnClearItems_internal();

        //        if (OnClearItems.HasDelegate)
        //            await OnClearItems.InvokeAsync();
        //    });

        if (OnChange.HasDelegate)
        {
            await AddEventListener<string>(
                GetJsEventName(nameof(OnChange)),
                async e =>
                {
                    await _onModificationTask.Task;

                    await OnChange.InvokeAsync(e);

                    _onModificationTask = new TaskCompletionSource<bool>();
                });
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
                        jsonDocument.RootElement[0].Deserialize<string>()!,
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
            await AddEventListener<TomSelectOption>(
                GetJsEventName(nameof(OnDropdownOpen)),
                async e => { await OnDropdownOpen.InvokeAsync(e); });
        }

        if (OnDropdownClose.HasDelegate)
        {
            await AddEventListener<TomSelectOption>(
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
                async e => { await OnDestroy.InvokeAsync(); });
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
        return InteropEventListener.Add("TomSelectInterop.addEventListener", ElementId, eventName, callback, CTs.Token);
    }

    public ValueTask ClearItems(bool silent = false, CancellationToken cancellationToken = default)
    {
        Items.Clear();
        _itemsHash = Items.GetAggregateHashCode();

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, CTs.Token);
        return TomSelectInterop.ClearItems(ElementId, silent, linkedCts.Token);
    }

    [JSInvokable("OnInitializedJs")]
    public async Task OnInitializedJs()
    {
        if (OnInitialize.HasDelegate)
            await OnInitialize.InvokeAsync();
    }
}
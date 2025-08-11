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
using Soenneker.Blazor.TomSelect.Enums;
using Soenneker.Extensions.ValueTask;
using Soenneker.Extensions.Task;
using Soenneker.Blazor.Extensions.EventCallback;
using Soenneker.Utils.Json;
using Soenneker.Extensions.CancellationTokens;

namespace Soenneker.Blazor.TomSelect;

///<inheritdoc cref="ITomSelect{TItem, TType}"/>
public partial class TomSelect<TItem, TType> : BaseTomSelect, ITomSelect<TItem, TType>
{
    [Parameter, EditorRequired]
    public IEnumerable<TItem>? Data { get; set; }

    [Parameter, EditorRequired]
    public Func<TItem, string?> TextField { get; set; } = null!;

    [Parameter, EditorRequired]
    public Func<TItem, string?> ValueField { get; set; } = null!;

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
    public List<TItem> Items { get; set; } = null!;

    [Parameter]
    public EventCallback<List<TItem>> ItemsChanged { get; set; }

    private int _itemsHash;
    private int _dataHash;

    private bool _isCreated;
    private bool _isDataSet;

    private readonly List<TomSelectOption> _workingOptions = [];

    private List<TItem> _workingItems = [];

    private TaskCompletionSource<bool>? _onModificationTask;

    protected override async Task OnInitializedAsync()
    {
        await TomSelectInterop.Initialize(Configuration.UseCdn, CancellationToken).NoSync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            Items ??= [];

            _isDataSet = false;
            InteropEventListener.Initialize(TomSelectInterop);
            await Initialize().NoSync();
            _isCreated = true;
        }

        if (!_isDataSet && _isCreated)
        {
            _isDataSet = true;

            await InitializeData().NoSync();
        }
    }

    protected override void OnParametersSet()
    {
        if (Placeholder.HasContent())
            Configuration.Placeholder = Placeholder!;

        base.OnParametersSet();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!_isDataSet || !_isCreated)
            return;

        if (Data.Populated())
        {
            int dataHashNew = Data.GetAggregateHashCode();

            if (_dataHash != dataHashNew)
            {
                LogDebug("OnParametersSet: Options hash differs, updating...");

                _dataHash = dataHashNew;

                await ClearOptions().NoSync();
                await AddOptions(Data, false).NoSync();
            }
        }

        int itemsHashCode = Items.GetAggregateHashCode(false);

        if (_itemsHash != itemsHashCode)
        {
            LogDebug("OnParametersSet: Items hash differs, updating...");

            List<TItem> oldWorkingItems = _workingItems.ToList(); // Save previous selections

            _workingItems = Items ?? [];

            await CleanItems().NoSync();

            _itemsHash = _workingItems.GetAggregateHashCode(false);

            List<string> values = ConvertItemsToListString(_workingItems);
            await TomSelectInterop.ClearAndAddItems(ElementId, values, true).NoSync();

            // Restore previously selected items if possible
            List<string?> preservedValues = oldWorkingItems.Select(ToValueFromItem).ToList();
            List<string?> newValues = _workingItems.Select(ToValueFromItem).ToList();
            List<string?> missingValues = preservedValues.Except(newValues).ToList();

            if (missingValues.Count > 0)
            {
                await TomSelectInterop.AddItems(ElementId, missingValues, true).NoSync();
            }
        }
    }

    public override async ValueTask Reinitialize(CancellationToken cancellationToken = default)
    {
        await ClearItems(true, cancellationToken).NoSync();

        await ClearOptions(cancellationToken).NoSync();

        await InitializeData(cancellationToken).NoSync();
    }

    private async ValueTask InitializeData(CancellationToken cancellationToken = default)
    {
        if (Data == null)
        {
            _onModificationTask = new TaskCompletionSource<bool>();
            return;
        }

        _dataHash = Data.GetAggregateHashCode();

        if (!Data.Empty())
        {
            await AddOptions(Data, false, cancellationToken).NoSync();

            _workingItems = Items;

            await CleanItems().NoSync();

            await AddItemsToDom(_workingItems, true, cancellationToken).NoSync();
        }

        _onModificationTask = new TaskCompletionSource<bool>();
    }

    public async ValueTask Initialize(TomSelectConfiguration? configuration = null, CancellationToken cancellationToken = default)
    {
        if (configuration != null)
            Configuration = configuration;

        if (Create)
            Configuration.Create = true;

        DotNetReference = DotNetObjectReference.Create<BaseTomSelect>(this);

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TomSelectInterop.Create(ElementReference, ElementId, DotNetReference, Configuration, linked).NoSync();
            await TomSelectInterop.CreateObserver(ElementId, linked).NoSync();
        }

        await AddEventListeners().NoSync();
    }

    public async ValueTask<TomSelectOption?> AddOption(TItem item, bool userCreated = true, CancellationToken cancellationToken = default)
    {
        TomSelectOption? option = CreateOptionFromItem(item);

        if (!TryAddOption(option))
            return null;

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
            await TomSelectInterop.AddOption(ElementId, option!, userCreated, linked).NoSync();

        return option;
    }

    public async ValueTask AddOptions(IEnumerable<TItem> items, bool userCreated = true, CancellationToken cancellationToken = default)
    {
        IEnumerable<TItem> dedupedItems = items.RemoveDuplicates();

        List<TomSelectOption> tomSelectOptions = CreateOptionsFromItems(dedupedItems);

        List<TomSelectOption> dedupedOptions = tomSelectOptions.RemoveDuplicates(c => c.Value).ToList();

        for (var i = 0; i < dedupedOptions.Count; i++)
        {
            TomSelectOption dedupedOption = dedupedOptions[i];
            TryAddOption(dedupedOption);
        }

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
            await TomSelectInterop.AddOptions(ElementId, dedupedOptions, userCreated, linked).NoSync();
    }

    public async ValueTask UpdateOption(string value, TItem item, CancellationToken cancellationToken = default)
    {
        TomSelectOption? option = CreateOptionFromItem(item);

        if (option == null)
        {
            Logger.LogWarning("Could not update option, option is null");
            return;
        }

        _workingOptions.Replace(c => c.Value == value, option);

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
            await TomSelectInterop.UpdateOption(ElementId, value, option, linked).NoSync();
    }

    public ValueTask AddItem(string value, bool silent = false, CancellationToken cancellationToken = default)
    {
        TItem? item = ToItemFromValue(value);

        return AddItem(item, silent, cancellationToken);
    }

    public async ValueTask AddItem(TItem item, bool silent = false, CancellationToken cancellationToken = default)
    {
        string? value = ToValueFromItem(item);

        AddItemType result = await TryAddItem(value, cancellationToken).NoSync();

        if (result != AddItemType.Normal)
            return;

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
            await TomSelectInterop.AddItem(ElementId, value!, silent, linked).NoSync();
    }

    public ValueTask AddItems(IEnumerable<string> value, bool silent = false, CancellationToken cancellationToken = default)
    {
        IEnumerable<TItem?> items = value.Select(ToItemFromValue);

        return AddItems(items, silent, cancellationToken);
    }

    private ValueTask AddItemsToDom(IEnumerable<string> values, bool silent, CancellationToken cancellationToken)
    {
        return TomSelectInterop.AddItems(ElementId, values, silent, cancellationToken);
    }

    private ValueTask AddItemsToDom(IEnumerable<TItem> items, bool silent, CancellationToken cancellationToken)
    {
        IEnumerable<string?> values = items.Select(ToValueFromItem);

        return TomSelectInterop.AddItems(ElementId, values!, silent, cancellationToken);
    }

    public async ValueTask AddItems(IEnumerable<TItem> items, bool silent = false, CancellationToken cancellationToken = default)
    {
        if (items.IsNullOrEmpty())
        {
            return;
        }

        List<string?> values = [];

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            foreach (TItem item in items)
            {
                string? value = ToValueFromItem(item);

                AddItemType result = await TryAddItem(value, linked).NoSync();

                if (result != AddItemType.Error)
                {
                    values.Add(value);
                }
            }

            await AddItemsToDom(values, silent, linked).NoSync();
        }
    }

    private List<string> ConvertItemsToListString(IEnumerable<TItem> items)
    {
        return items.Select(ToValueFromItem).Where(value => !value.IsNullOrEmpty()).ToList()!;
    }

    private List<TomSelectOption> CreateOptionsFromItems(IEnumerable<TItem> items)
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

    private TomSelectOption? CreateOptionFromItem(TItem item)
    {
        string? value = ToValueFromItem(item);
        string? text = ToTextFromItem(item);

        if (text == null || value == null)
            return null;

        var tomSelectOption = new TomSelectOption
        {
            Text = text,
            Value = value,
            Item = item
        };

        return tomSelectOption;
    }

    private TItem? ToItemFromText(string text)
    {
        foreach (TomSelectOption option in _workingOptions)
        {
            if (option.Value == text)
                return (TItem) option.Item!;
        }

        return default;
    }

    private TItem? ToItemFromValue(string value)
    {
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
            return await CreateFunc.Invoke(text).NoSync();

        return default!;
    }

    private async ValueTask OnItemAdd_internal(string valueOrText)
    {
        AddItemType addItemType = await TryAddItem(valueOrText).NoSync();

        if (addItemType == AddItemType.NewOption)
        {
            await OnOptionCreated_internal(valueOrText).NoSync();
        }
    }

    private async ValueTask OnOptionCreated_internal(string value)
    {
        LogDebug($"Adding new option ({value}) ...");

        TItem item = await CreateItemFromValue(value).NoSync();

        // Unfortunately we need to remove the option (stored via value) so we can re-add the properly built one from the component
        await RemoveOption(value).NoSync();
        TomSelectOption? newOption = await AddOption(item, false).NoSync();

        if (OnItemCreated.HasDelegate)
        {
            if (newOption != null)
                await OnItemCreated.InvokeAsync((value, newOption)).NoSync();
        }
    }

    private async ValueTask OnItemRemove_Internal(string valueOrText)
    {
        for (var i = 0; i < _workingItems.Count; i++)
        {
            TItem item = _workingItems[i];
            string? value = ToValueFromItem(item);

            if (value == valueOrText)
            {
                _workingItems.Remove(item);
                await SyncItems().NoSync();
                return;
            }
        }

        LogWarning($"Item ({valueOrText}) was not found in Items list, cannot remove");
    }

    private void OnOptionClear_internal()
    {
        _workingOptions.Clear();
    }

    private async ValueTask AddEventListeners()
    {
        // OPTIONS ---
        await AddEventListener<string>(GetJsEventName(nameof(OnOptionAdd)), async str =>
            {
                JsonDocument jsonDocument = JsonDocument.Parse(str);
                var parameters = (jsonDocument.RootElement[0].Deserialize<string>()!, jsonDocument.RootElement[1].Deserialize<TomSelectOption>()!);

                if (OnOptionAdd.HasDelegate)
                    await OnOptionAdd.InvokeAsync(parameters).NoSync();
            })
            .NoSync();

        await AddEventListener<string>(GetJsEventName(nameof(OnOptionRemove)), async str =>
            {
                if (OnOptionRemove.HasDelegate)
                    await OnOptionRemove.InvokeAsync(str).NoSync();
            })
            .NoSync();

        await AddEventListener<string>(GetJsEventName(nameof(OnOptionClear)), async _ =>
            {
                OnOptionClear_internal();

                if (OnOptionClear.HasDelegate)
                    await OnOptionClear.InvokeAsync().NoSync();
            })
            .NoSync();

        // ITEMS ---

        await AddEventListener<string>(GetJsEventName(nameof(OnItemAdd)), async str =>
            {
                JsonDocument jsonDocument = JsonDocument.Parse(str);
                (string, TomSelectOption) parameters = (jsonDocument.RootElement[0].Deserialize<string>()!,
                    jsonDocument.RootElement[1].Deserialize<TomSelectOption>()!);

                await OnItemAdd_internal(parameters.Item1).NoSync();

                if (OnItemAdd.HasDelegate)
                    await OnItemAdd.InvokeAsync(parameters).NoSync();

                _onModificationTask?.TrySetResult(true);
            })
            .NoSync();

        await AddEventListener<string>(GetJsEventName(nameof(OnItemRemove)), async str =>
            {
                JsonDocument jsonDocument = JsonDocument.Parse(str);
                (string, TomSelectOption) parameters = (jsonDocument.RootElement[0].Deserialize<string>()!,
                    jsonDocument.RootElement[1].Deserialize<TomSelectOption>()!);

                await OnItemRemove_Internal(parameters.Item1).NoSync();

                if (OnItemRemove.HasDelegate)
                    await OnItemRemove.InvokeAsync(parameters).NoSync();

                _onModificationTask?.TrySetResult(true);
            })
            .NoSync();

        await AddEventListener<string>(GetJsEventName(nameof(OnItemSelect)), async str =>
            {
                TItem? item = ToItemFromValue(str);

                if (item != null)
                {
                    TomSelectOption? option = CreateOptionFromItem(item);

                    if (OnItemSelect.HasDelegate)
                        await OnItemSelect.InvokeAsync(option).NoSync();
                }
            })
            .NoSync();

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
            await AddEventListener<string>(GetJsEventName(nameof(OnChange)), async str =>
                {
                    if (_onModificationTask != null)
                        await _onModificationTask.Task.NoSync();

                    var values = JsonUtil.Deserialize<List<string>>(str);

                    await OnChange.InvokeAsync(values).NoSync();

                    _onModificationTask = new TaskCompletionSource<bool>();
                })
                .NoSync();
        }

        if (OnFocus.HasDelegate)
        {
            await AddEventListener<string>(GetJsEventName(nameof(OnFocus)), async _ => { await OnFocus.InvokeAsync().NoSync(); }).NoSync();
        }

        if (OnBlur.HasDelegate)
        {
            await AddEventListener<string>(GetJsEventName(nameof(OnBlur)), async _ => { await OnBlur.InvokeAsync().NoSync(); }).NoSync();
        }

        if (OnOptgroupAdd.HasDelegate)
        {
            await AddEventListener<string>(GetJsEventName(nameof(OnOptgroupAdd)), async str =>
                {
                    JsonDocument jsonDocument = JsonDocument.Parse(str);
                    (string, TomSelectOption) parameters = (jsonDocument.RootElement[0].Deserialize<string>()!,
                        jsonDocument.RootElement[1].Deserialize<TomSelectOption>()!);

                    await OnOptgroupAdd.InvokeAsync(parameters).NoSync();
                })
                .NoSync();
        }

        if (OnOptgroupRemove.HasDelegate)
        {
            await AddEventListener<string>(GetJsEventName(nameof(OnOptgroupRemove)), async str => { await OnOptgroupRemove.InvokeAsync(str).NoSync(); })
                .NoSync();
        }

        if (OnOptgroupClear.HasDelegate)
        {
            await AddEventListener<string>(GetJsEventName(nameof(OnOptgroupClear)), async str => { await OnOptgroupClear.InvokeAsync().NoSync(); }).NoSync();
        }

        if (OnDropdownOpen.HasDelegate)
        {
            await AddEventListener<TomSelectOption>(GetJsEventName(nameof(OnDropdownOpen)), async str => { await OnDropdownOpen.InvokeAsync(str).NoSync(); })
                .NoSync();
        }

        if (OnDropdownClose.HasDelegate)
        {
            await AddEventListener<TomSelectOption>(GetJsEventName(nameof(OnDropdownClose)), async str => { await OnDropdownClose.InvokeAsync(str).NoSync(); })
                .NoSync();
        }

        if (OnType.HasDelegate)
        {
            await AddEventListener<string>(GetJsEventName(nameof(OnType)), async str => { await OnType.InvokeAsync(str).NoSync(); }).NoSync();
        }

        if (OnLoad.HasDelegate)
        {
            await AddEventListener<object>(GetJsEventName(nameof(OnLoad)), async str => { await OnLoad.InvokeAsync(str).NoSync(); }).NoSync();
        }

        if (OnDestroy.HasDelegate)
        {
            await AddEventListener<string>(GetJsEventName(nameof(OnDestroy)), async _ => { await OnDestroy.InvokeAsync().NoSync(); }).NoSync();
        }
    }

    private static string GetJsEventName(string callback)
    {
        // Remove first two characters
        string subStr = callback[2..];
        return subStr.ToSnakeCaseFromPascal();
    }

    private ValueTask AddEventListener<T>(string eventName, Func<T, ValueTask> callback)
    {
        return InteropEventListener.Add("TomSelectInterop.addEventListener", ElementId, eventName, callback, CancellationToken);
    }

    public async ValueTask ClearItems(bool silent = false, CancellationToken cancellationToken = default)
    {
        _workingItems.Clear();
        await SyncItems().NoSync();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
            await TomSelectInterop.ClearItems(ElementId, silent, linked).NoSync();
    }

    private bool TryAddOption(TomSelectOption? tomSelectOption)
    {
        if (tomSelectOption == null)
        {
            Logger.LogWarning("Option is null, skipping add");
            return false;
        }

        LogDebug($"Trying to add option: {tomSelectOption.Text}, {tomSelectOption.Value} ...");

        if (_workingOptions.Contains(c => c.Value == tomSelectOption.Value))
        {
            Logger.LogWarning("Option with value {Value} already exists, skipping add", tomSelectOption.Value);
            return false;
        }

        _workingOptions.Add(tomSelectOption);

        return true;
    }

    private async ValueTask<AddItemType> TryAddItem(string? valueOrText, CancellationToken cancellationToken = default)
    {
        if (valueOrText.IsNullOrEmpty())
        {
            Logger.LogWarning("Value or text is null or empty, skipping add");
            return AddItemType.Error;
        }

        LogDebug($"Trying to add item: {valueOrText} ...");

        TItem? item = default;

        for (var optionIndex = 0; optionIndex < _workingOptions.Count; optionIndex++)
        {
            TomSelectOption option = _workingOptions[optionIndex];
            if (option.Value == valueOrText || option.Text == valueOrText)
            {
                item = (TItem) option.Item!;
                break;
            }
        }

        if (item == null)
        {
            LogWarning($"Item ({valueOrText}) could not be found in existing options, must be a new option, skipping add");
            return AddItemType.NewOption;
        }

        for (var itemIndex = 0; itemIndex < Items.Count; itemIndex++)
        {
            TItem i = Items[itemIndex];
            string? value = ToValueFromItem(i);

            if (value == valueOrText)
            {
                LogWarning($"Item ({valueOrText}) already exists, skipping add");
                return AddItemType.Error;
            }
        }

        _workingItems.Add(item);
        await SyncItems().NoSync();

        return AddItemType.Normal;
    }

    [JSInvokable("OnInitializedJs")]
    public Task OnInitializedJs()
    {
        return OnInitialize.InvokeIfHasDelegate();
    }

    private async ValueTask CleanItems()
    {
        LogDebug("Cleaning Items...");

        var cleaned = new Dictionary<string, TItem>();

        var requiresSync = false;

        for (var workingItemIndex = 0; workingItemIndex < _workingItems.Count; workingItemIndex++)
        {
            TItem item = _workingItems[workingItemIndex];
            string? value = ToValueFromItem(item);

            if (value.IsNullOrEmpty())
            {
                LogWarning("Item value was null, removing from Items");
                requiresSync = true;
                continue;
            }

            if (!cleaned.TryAdd(value, item))
            {
                LogWarning($"Item with value {value} already exists, removing from Items");
                requiresSync = true;
            }
        }

        if (requiresSync)
        {
            LogDebug("Clean requires a resync of Items");
            _workingItems = cleaned.Values.ToList();
            await SyncItems().NoSync();
        }
    }

    private Task SyncItems()
    {
        _itemsHash = _workingItems.GetAggregateHashCode(false);
        return ItemsChanged.InvokeAsync(_workingItems);
    }
}
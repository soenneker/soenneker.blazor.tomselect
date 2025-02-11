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

namespace Soenneker.Blazor.TomSelect;

///<inheritdoc cref="ITomSelect{TItem, TType}"/>
public partial class TomSelect<TItem, TType> : BaseTomSelect
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
    private int _workingOptionsHash;
    private int _dataHash;

    private bool _isCreated;
    private bool _isDataSet;
    private bool _cleaned;

    private readonly List<TomSelectOption> _workingOptions = [];

    private List<TItem> _workingItems = [];

    private TaskCompletionSource<bool>? _onModificationTask = null;

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

            await InitializeData();
        }
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

                await ClearOptions();
                await AddOptions(Data, false);
            }
        }

        int itemsHashCode = Items.GetAggregateHashCode(false);

        if (_itemsHash != itemsHashCode)
        {
            LogDebug("OnParametersSet: Items hash differs, updating...");

            _workingItems = Items;

            await CleanItems();

            _itemsHash = Items.GetAggregateHashCode(false);

            List<string> values = ConvertItemsToListString(_workingItems);
            await TomSelectInterop.ClearAndAddItems(ElementId, values, true);
        }
    }

    public override async ValueTask Reinitialize()
    {
        await ClearItems(true).NoSync();

        await ClearOptions().NoSync();

        await InitializeData().NoSync();
    }

    private async ValueTask InitializeData()
    {
        if (Data == null)
        {
            _onModificationTask = new TaskCompletionSource<bool>();
            return;
        }

        _dataHash = Data.GetAggregateHashCode();

        if (Data.Any())
        {
            await AddOptions(Data, false);

            _workingItems = Items;

            await CleanItems();

            await AddItemsToDom(_workingItems, true, CancellationToken.None);
        }

        _onModificationTask = new TaskCompletionSource<bool>();
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

        if (!TryAddOption(option))
            return null;

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, CTs.Token);
        await TomSelectInterop.AddOption(ElementId, option!, userCreated, linkedCts.Token);

        return option;
    }

    public async ValueTask AddOptions(IEnumerable<TItem> items, bool userCreated = true, CancellationToken cancellationToken = default)
    {
        IEnumerable<TItem> dedupedItems = items.RemoveDuplicates();

        List<TomSelectOption> tomSelectOptions = CreateOptionsFromItems(dedupedItems);

        List<TomSelectOption> dedupedOptions = tomSelectOptions.RemoveDuplicates(c => c.Value).ToList();

        foreach (TomSelectOption dedupedOption in dedupedOptions)
        {
            TryAddOption(dedupedOption);
        }

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, CTs.Token);
        await TomSelectInterop.AddOptions(ElementId, dedupedOptions, userCreated, linkedCts.Token);
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
        SyncOptions();

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, CTs.Token);
        return TomSelectInterop.UpdateOption(ElementId, value, option, linkedCts.Token);
    }

    public ValueTask AddItem(string value, bool silent = false, CancellationToken cancellationToken = default)
    {
        TItem? item = ToItemFromValue(value);

        return AddItem(item, silent, cancellationToken);
    }

    public async ValueTask AddItem(TItem item, bool silent = false, CancellationToken cancellationToken = default)
    {
        string? value = ToValueFromItem(item);

        AddItemType result = await TryAddItem(value);

        if (result != AddItemType.Normal)
            return;

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, CTs.Token);
        await TomSelectInterop.AddItem(ElementId, value!, silent, linkedCts.Token);
    }

    public ValueTask AddItems(IEnumerable<string> value, bool silent = false, CancellationToken cancellationToken = default)
    {
        IEnumerable<TItem?> items = value.Select(ToItemFromValue);

        return AddItems(items, silent, cancellationToken);
    }

    private ValueTask AddItemsToDom(IEnumerable<string> values, bool silent, CancellationToken cancellationToken)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(CTs.Token, cancellationToken);
        return TomSelectInterop.AddItems(ElementId, values!, silent, linkedCts.Token);
    }

    private ValueTask AddItemsToDom(IEnumerable<TItem> items, bool silent, CancellationToken cancellationToken)
    {
        IEnumerable<string?> values = items.Select(ToValueFromItem);

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(CTs.Token, cancellationToken);
        return TomSelectInterop.AddItems(ElementId, values!, silent, linkedCts.Token);
    }

    public async ValueTask AddItems(IEnumerable<TItem> items, bool silent = false, CancellationToken cancellationToken = default)
    {
        if (items.IsNullOrEmpty())
        {
            return;
        }

        List<string?> values = [];

        foreach (TItem item in items)
        {
            string? value = ToValueFromItem(item);

            int result = await TryAddItem(value);

            if (result == 1)
            {
                values.Add(value);
            }
        }

        await AddItemsToDom(values, silent, cancellationToken);
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
        AddItemType addItemType = await TryAddItem(valueOrText);

        if (addItemType == AddItemType.NewOption)
        {
            await OnOptionCreated_internal(valueOrText);
        }
    }

    private async ValueTask OnOptionCreated_internal(string value)
    {
        LogDebug($"Adding new option ({value}) ...");

        TItem item = await CreateItemFromValue(value).NoSync();

        // Unfortunately we need to remove the option (stored via value) so we can re-add the properly built one from the component
        await RemoveOption(value);
        TomSelectOption? newOption = await AddOption(item, false);

        if (OnItemCreated.HasDelegate)
        {
            if (newOption != null)
                await OnItemCreated.InvokeAsync((value, newOption));
        }
    }

    private async ValueTask OnItemRemove_Internal(string valueOrText)
    {
        foreach (TItem item in _workingItems)
        {
            string? value = ToValueFromItem(item);

            if (value == valueOrText)
            {
                _workingItems.Remove(item);
                await SyncItems();
                return;
            }
        }

        LogWarning($"Item ({valueOrText}) was not found in Items list, cannot remove");
    }

    private void OnOptionClear_internal()
    {
        _workingOptions.Clear();
        SyncOptions();
    }

    private async ValueTask AddEventListeners()
    {
        // OPTIONS ---
        await AddEventListener<string>(
            GetJsEventName(nameof(OnOptionAdd)),
            async (str, _) =>
            {
                JsonDocument jsonDocument = JsonDocument.Parse(str);
                var parameters = (
                    jsonDocument.RootElement[0].Deserialize<string>()!,
                    jsonDocument.RootElement[1].Deserialize<TomSelectOption>()!
                );

                if (OnOptionAdd.HasDelegate)
                    await OnOptionAdd.InvokeAsync(parameters);
            });

        await AddEventListener<string>(
            GetJsEventName(nameof(OnOptionRemove)),
            async (str, _) =>
            {
                if (OnOptionRemove.HasDelegate)
                    await OnOptionRemove.InvokeAsync(str);
            });

        await AddEventListener<string>(
            GetJsEventName(nameof(OnOptionClear)),
            async (_, _) =>
            {
                OnOptionClear_internal();

                if (OnOptionClear.HasDelegate)
                    await OnOptionClear.InvokeAsync();
            });

        // ITEMS ---

        await AddEventListener<string>(
            GetJsEventName(nameof(OnItemAdd)),
            async (str, _) =>
            {
                JsonDocument jsonDocument = JsonDocument.Parse(str);
                (string, TomSelectOption) parameters = (
                    jsonDocument.RootElement[0].Deserialize<string>()!,
                    jsonDocument.RootElement[1].Deserialize<TomSelectOption>()!
                );

                await OnItemAdd_internal(parameters.Item1);

                if (OnItemAdd.HasDelegate)
                    await OnItemAdd.InvokeAsync(parameters);

                _onModificationTask?.TrySetResult(true);
            });

        await AddEventListener<string>(
            GetJsEventName(nameof(OnItemRemove)),
            async (str, _) =>
            {
                JsonDocument jsonDocument = JsonDocument.Parse(str);
                (string, TomSelectOption) parameters = (
                    jsonDocument.RootElement[0].Deserialize<string>()!,
                    jsonDocument.RootElement[1].Deserialize<TomSelectOption>()!
                );

                await OnItemRemove_Internal(parameters.Item1);

                if (OnItemRemove.HasDelegate)
                    await OnItemRemove.InvokeAsync(parameters);

                _onModificationTask?.TrySetResult(true);
            });

        await AddEventListener<string>(
            GetJsEventName(nameof(OnItemSelect)),
            async (str, _) =>
            {
                TItem? item = ToItemFromValue(str);

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
                async (str, _) =>
                {
                    if (_onModificationTask != null)
                        await _onModificationTask.Task.NoSync();

                    await OnChange.InvokeAsync(str);

                    _onModificationTask = new TaskCompletionSource<bool>();
                });
        }

        if (OnFocus.HasDelegate)
        {
            await AddEventListener<string>(
                GetJsEventName(nameof(OnFocus)),
                async (_, _) => { await OnFocus.InvokeAsync(); });
        }

        if (OnBlur.HasDelegate)
        {
            await AddEventListener<string>(
                GetJsEventName(nameof(OnBlur)),
                async (_, _) => { await OnBlur.InvokeAsync(); });
        }

        if (OnOptgroupAdd.HasDelegate)
        {
            await AddEventListener<string>(
                GetJsEventName(nameof(OnOptgroupAdd)),
                async (str, _) =>
                {
                    JsonDocument jsonDocument = JsonDocument.Parse(str);
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
                async (str, _) => { await OnOptgroupRemove.InvokeAsync(str); });
        }

        if (OnOptgroupClear.HasDelegate)
        {
            await AddEventListener<string>(
                GetJsEventName(nameof(OnOptgroupClear)),
                async (str, _) => { await OnOptgroupClear.InvokeAsync(); });
        }

        if (OnDropdownOpen.HasDelegate)
        {
            await AddEventListener<TomSelectOption>(
                GetJsEventName(nameof(OnDropdownOpen)),
                async (str, _) => { await OnDropdownOpen.InvokeAsync(str); });
        }

        if (OnDropdownClose.HasDelegate)
        {
            await AddEventListener<TomSelectOption>(
                GetJsEventName(nameof(OnDropdownClose)),
                async (str, _) => { await OnDropdownClose.InvokeAsync(str); });
        }

        if (OnType.HasDelegate)
        {
            await AddEventListener<string>(
                GetJsEventName(nameof(OnType)),
                async (str, _) => { await OnType.InvokeAsync(str); });
        }

        if (OnLoad.HasDelegate)
        {
            await AddEventListener<object>(
                GetJsEventName(nameof(OnLoad)),
                async (str, _) => { await OnLoad.InvokeAsync(str); });
        }

        if (OnDestroy.HasDelegate)
        {
            await AddEventListener<string>(
                GetJsEventName(nameof(OnDestroy)),
                async (_, _) => { await OnDestroy.InvokeAsync(); });
        }
    }

    private static string GetJsEventName(string callback)
    {
        // Remove first two characters
        string subStr = callback[2..];
        return subStr.ToSnakeCaseFromPascal();
    }

    private ValueTask AddEventListener<T>(string eventName, Func<T, CancellationToken, ValueTask> callback)
    {
        return InteropEventListener.Add("TomSelectInterop.addEventListener", ElementId, eventName, callback, CTs.Token);
    }

    public async ValueTask ClearItems(bool silent = false, CancellationToken cancellationToken = default)
    {
        _workingItems.Clear();
        await SyncItems();

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, CTs.Token);
        await TomSelectInterop.ClearItems(ElementId, silent, linkedCts.Token);
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
        SyncOptions();

        return true;
    }

    private async ValueTask<AddItemType> TryAddItem(string? valueOrText)
    {
        if (valueOrText.IsNullOrEmpty())
        {
            Logger.LogWarning("Value or text is null or empty, skipping add");
            return AddItemType.Error;
        }

        LogDebug($"Trying to add item: {valueOrText} ...");

        TItem? item = default;

        foreach (TomSelectOption option in _workingOptions)
        {
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

        foreach (TItem i in Items)
        {
            string? value = ToValueFromItem(i);

            if (value == valueOrText)
            {
                LogWarning($"Item ({valueOrText}) already exists, skipping add");
                return AddItemType.Error;
            }
        }

        _workingItems.Add(item);
        await SyncItems();

        return AddItemType.Normal;
    }

    [JSInvokable("OnInitializedJs")]
    public async Task OnInitializedJs()
    {
        if (OnInitialize.HasDelegate)
            await OnInitialize.InvokeAsync();
    }

    private async ValueTask CleanItems()
    {
        LogDebug("Cleaning Items...");

        var cleaned = new Dictionary<string, TItem>();

        var requiresSync = false;

        foreach (TItem item in _workingItems)
        {
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
            await SyncItems();
        }
    }

    private Task SyncItems()
    {
        _itemsHash = _workingItems.GetAggregateHashCode(false);
        return ItemsChanged.InvokeAsync(_workingItems);
    }

    private void SyncOptions()
    {
        _workingOptionsHash = _workingOptions.GetAggregateHashCode();
    }
}
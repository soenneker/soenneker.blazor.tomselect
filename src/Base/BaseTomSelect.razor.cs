using System.Threading;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Soenneker.Blazor.TomSelect.Base.Abstract;
using Soenneker.Blazor.Utils.InteropEventListener.Abstract;
using Soenneker.Blazor.TomSelect.Abstract;
using Microsoft.Extensions.Logging;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;

namespace Soenneker.Blazor.TomSelect.Base;

///<inheritdoc cref="IBaseTomSelect"/>
public partial class BaseTomSelect : ComponentBase, IBaseTomSelect
{
    protected DotNetObjectReference<BaseTomSelect>? DotNetReference;

    protected ITomSelectInterop TomSelectInterop = default!;

    protected IInteropEventListener InteropEventListener = default!;

    /// <summary>
    /// The actual HTML element's id
    /// </summary>
    protected readonly string ElementId = Guid.NewGuid().ToString();

    protected readonly CancellationTokenSource CTs = new();

    protected ElementReference ElementReference;

    protected ILogger<BaseTomSelect> Logger = default!;

    /// <summary>
    /// Destroys the element.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        DotNetReference?.Dispose();
        InteropEventListener.DisposeForElement(ElementId);

        await CTs.CancelAsync().NoSync();
        await TomSelectInterop.Destroy(ElementId).NoSync();
    }

    protected void LogWarning(string message)
    {
        if (Debug)
            Console.WriteLine(message);
    }

    protected void LogDebug(string message)
    {
        if (Debug)
            Console.WriteLine(message);
    }
}
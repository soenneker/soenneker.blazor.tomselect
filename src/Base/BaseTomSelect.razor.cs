using System.Threading;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Soenneker.Blazor.TomSelect.Base.Abstract;
using Soenneker.Blazor.Utils.InteropEventListener.Abstract;
using Soenneker.Blazor.TomSelect.Abstract;
using Microsoft.Extensions.Logging;

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
    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        DotNetReference?.Dispose();
        InteropEventListener.DisposeForElement(ElementId);
        CTs.Cancel();
        return TomSelectInterop.Destroy(ElementId);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        DotNetReference?.Dispose();
        InteropEventListener.DisposeForElement(ElementId);
        CTs.Cancel();
        TomSelectInterop.Destroy(ElementId);
    }
}
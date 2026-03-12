using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Blazor.TomSelect.Abstract;
using Soenneker.Blazor.Utils.InteropEventListener.Registrars;
using Soenneker.Blazor.Utils.ResourceLoader.Registrars;

namespace Soenneker.Blazor.TomSelect.Registrars;

/// <summary>
/// A Blazor interop library for the select user control library, Tom Select
/// </summary>
public static class TomSelectRegistrar
{
    /// <summary>
    /// Adds <see cref="ITomSelectInterop"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddTomSelectInteropAsScoped(this IServiceCollection services)
    {
        services.AddResourceLoaderAsScoped().AddInteropEventListenerAsScoped().TryAddScoped<ITomSelectInterop, TomSelectInterop>();

        return services;
    }
}

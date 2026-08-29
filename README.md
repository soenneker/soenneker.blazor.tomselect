[![](https://img.shields.io/nuget/v/soenneker.blazor.tomselect.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.blazor.tomselect/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.blazor.tomselect/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.blazor.tomselect/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.blazor.tomselect.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.blazor.tomselect/)
[![](https://img.shields.io/badge/Demo-Live-blueviolet?style=for-the-badge&logo=github)](https://soenneker.github.io/soenneker.blazor.tomselect)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.blazor.tomselect/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.blazor.tomselect/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Blazor.TomSelect

A strongly typed Blazor component and JS interop wrapper for searchable single-select, multi-select, user-created, and remotely loaded [Tom Select](https://tom-select.js.org/) controls.

[Live demo](https://soenneker.github.io/soenneker.blazor.tomselect)

## Installation

```bash
dotnet add package Soenneker.Blazor.TomSelect
```

Register the interop service in `Program.cs`:

```csharp
using Soenneker.Blazor.TomSelect.Registrars;

builder.Services.AddTomSelectInteropAsScoped();
```

Add the component namespace to `_Imports.razor`:

```razor
@using Soenneker.Blazor.TomSelect
```

## Basic usage

`Data` supplies the available options. `Items` contains the selected `TItem` objects and is the normal source of truth for selection state:

```razor
@using Soenneker.Blazor.TomSelect.Configuration

<TomSelect TItem="Country"
           TType="string"
           Data="_countries"
           TextField="country => country.Name"
           ValueField="country => country.Id.ToString()"
           @bind-Items="_selectedCountries"
           Multiple="true"
           Placeholder="Select countries…"
           Configuration="_configuration" />

@code {
    private IReadOnlyList<Country> _countries =
    [
        new(1, "Canada"),
        new(2, "Mexico"),
        new(3, "United States")
    ];

    private List<Country> _selectedCountries = [];

    private readonly TomSelectConfiguration _configuration = new()
    {
        MaxItems = 3,
        CloseAfterSelect = false
    };

    private sealed record Country(int Id, string Name);
}
```

`TextField` is the label shown to the user. `ValueField` must return a stable, non-empty, unique string for each option. Duplicate or null values cannot be represented reliably and are removed or skipped.

The component's browser value path is string-based, so `TType="string"` is the appropriate declaration. The bound result is still `List<TItem>`. With `Multiple="false"`, that list contains zero or one item; use `Items.FirstOrDefault()` when the rest of your model expects a scalar.

When the parent replaces `Data` or `Items`, the component synchronizes its browser options and selection. Replace the list or ensure your item type has meaningful equality/hash behavior when changing mutable data. Use `UpdateOption(value, updatedItem)` for an explicit in-place option update.

## Creating new options

Enable creation and provide exactly one function that converts typed text into a valid `TItem`:

```razor
<TomSelect TItem="Tag"
           TType="string"
           Data="_availableTags"
           TextField="tag => tag.Name"
           ValueField="tag => tag.Id"
           @bind-Items="_selectedTags"
           Create="true"
           CreateFuncSync="CreateTag" />

@code {
    private Tag CreateTag(string text) => new(Guid.NewGuid().ToString("N"), text.Trim());
}
```

Use `CreateFunc` for asynchronous creation or `CreateFuncSync` for synchronous creation, never both. Validate length, format, duplicates, and authorization in that function before persisting user-created values. `CreateOnBlur` and `CreateFilter` provide additional Tom Select behavior but do not replace server-side validation.

## Remote loading

Set `LoadFunc` or `LoadFuncSync` to query options from .NET. The wrapper enables Tom Select's load callback automatically:

```razor
<TomSelect TItem="Customer"
           TType="string"
           Data="[]"
           TextField="customer => customer.DisplayName"
           ValueField="customer => customer.Id"
           @bind-Items="_customers"
           LoadFunc="SearchCustomersAsync"
           Configuration="_remoteConfiguration" />

@code {
    private readonly TomSelectConfiguration _remoteConfiguration = new()
    {
        ShouldLoadMinQueryLength = 2,
        LoadThrottle = 300,
        MaxOptions = 20
    };

    private async ValueTask<IEnumerable<Customer>> SearchCustomersAsync(string query)
    {
        string encoded = Uri.EscapeDataString(query);
        return await Http.GetFromJsonAsync<List<Customer>>($"api/customers/search?q={encoded}") ?? [];
    }
}
```

The load delegate receives only the query string, not a per-request cancellation token. Apply authorization and result limits on the server, encode the query, handle timeouts/failures, and avoid returning sensitive records merely because their text matches. `LoadThrottle` reduces request frequency but is not a server-side rate limit.

## Rendering and HTML safety

Default labels are HTML-escaped. `OptionTemplate` and `ItemTemplate` let Blazor provide trusted template markup; values substituted through `{{property.path}}` placeholders are also escaped:

```razor
<OptionTemplate>
    <div class="customer-option">
        <strong>{{displayName}}</strong>
        <span>{{email}}</span>
    </div>
</OptionTemplate>
```

Template markup itself is inserted into Tom Select's DOM. Keep it application-owned, and validate values used in URL-bearing attributes such as `href` or `src`; HTML escaping alone does not validate URL schemes.

`RenderOptionHtml`, `RenderOptionHtmlAsync`, `RenderItemHtml`, and `RenderItemHtmlAsync` are raw HTML escape hatches. Their return values are not sanitized. Never concatenate untrusted labels, descriptions, URLs, or remote API fields without context-appropriate encoding and URL validation.

## Events and imperative methods

Use `@bind-Items` for domain state. Event callbacks such as `OnInitialize`, `OnChange`, `OnItemAdd`, `OnItemRemove`, `OnItemCreated`, `OnFocus`, and `OnBlur` are useful for UI side effects and telemetry that does not contain sensitive option data.

The component reference exposes option/item mutation, focus, dropdown, enable/disable, lock, cache, and refresh methods. Wait for `OnInitialize` before calling imperative methods. `Reinitialize()` clears and repopulates options and selections from the current `Data` and `Items`; it is not a general configuration hot-reload mechanism.

## Assets and cleanup

`UseCdn` defaults to `true` and loads pinned Tom Select script/style assets from jsDelivr with integrity validation. Set it to `false` to use the package's bundled `_content` assets. `UseBootstrap5Styling` selects the Bootstrap 5 stylesheet; set it to `false` for Tom Select's regular stylesheet.

Every component instance owns separate browser callbacks and a removal observer. Normal Blazor disposal explicitly destroys the Tom Select instance; detached targets are also cleaned up when an ancestor is removed. Applications using direct `ITomSelectInterop` calls should still call `Destroy(elementId)` when they own the target lifecycle.

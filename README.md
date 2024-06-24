[![](https://img.shields.io/nuget/v/soenneker.blazor.tomselect.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.blazor.tomselect/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.blazor.tomselect/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.blazor.tomselect/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.blazor.tomselect.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.blazor.tomselect/)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Blazor.TomSelect
### A Blazor interop library for the select user control library, Tom Select

This library simplifies the integration of Tom Select into Blazor applications, providing access to options, methods, plugins, and events. A demo project showcasing common usages is included.

Diligence was taken to align the Blazor API with JS. Refer to the [Tom Select documentation](https://tom-select.js.org/) for details.

## Installation

```
dotnet add package Soenneker.Blazor.TomSelect
```

### Add the following to your `Startup.cs` file

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddTomSelect();
}
```

## Usage

```razor
@using Soenneker.Blazor.TomSelect

<TomSelect
    TItem="Country" TType="string" OnItemAdd="OnItemAdd"
    Data="@_countries"
    TextField="@(item => item.Name)"
    ValueField="@(item => item.Id.ToString())" @ref="_tomSelect" Items="_selectedCountries">
</TomSelect>

@code{
    private TomSelect<Country, string> _tomSelect = default!;

    private List<Country>? _selectedCountries;
    private List<Country>? _countries;

    protected override async Task OnInitializedAsync()
    {
        _countries = await Http.GetFromJsonAsync<List<Country>>("sample-data/countries.json");
    }

    private void OnItemAdd((string str, TomSelectOption obj) result)
    {
        Logger.LogInformation("OnItemAdd fired: Value: {value}", str);
    }

    private void LogSelectedItems()
    {
        foreach (Country item in _tomSelect.Items)
        {
            Logger.LogInformation("Selected item: {0}", item.Name);
        }
    }
}
```

⚠️ While a lot of the Tom Select library has been implemented, there are features not yet supported. If you need assistance or want to request a new feature, please open an issue or submit a pull request.
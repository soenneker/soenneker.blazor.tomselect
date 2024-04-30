using System.Collections.Generic;

namespace Soenneker.Blazor.TomSelect.Demo.Dtos;

public class Country
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Culture { get; set; }

    public string? FlagIcon { get; set; }

    public string? Region { get; set; }

    public IEnumerable<string>? AlternativeNames { get; set; }
}
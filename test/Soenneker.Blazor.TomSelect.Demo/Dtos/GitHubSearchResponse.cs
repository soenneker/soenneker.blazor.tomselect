using System.Text.Json.Serialization;

namespace Soenneker.Blazor.TomSelect.Demo.Dtos;

public sealed class GitHubSearchResponse
{
    [JsonPropertyName("items")]
    public GitHubRepo[] Items { get; set; } = [];
}
using System.Text.Json.Serialization;

namespace Soenneker.Blazor.TomSelect.Server.Dtos;

public sealed class GitHubSearchResponse
{
    [JsonPropertyName("items")]
    public GitHubRepo[] Items { get; set; } = [];
}

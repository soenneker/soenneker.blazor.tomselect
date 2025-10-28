using System.Text.Json.Serialization;

namespace Soenneker.Blazor.TomSelect.Server.Dtos;

public sealed class GitHubRepo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("html_url")]
    public string Url { get; set; } = "";

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("language")]
    public string? Language { get; set; }

    [JsonPropertyName("stargazers_count")]
    public int Stars { get; set; }

    [JsonPropertyName("forks_count")]
    public int Forks { get; set; }

    [JsonPropertyName("watchers_count")]
    public int Watchers { get; set; }

    [JsonPropertyName("owner")]
    public GitHubUser Owner { get; set; } = new();
}

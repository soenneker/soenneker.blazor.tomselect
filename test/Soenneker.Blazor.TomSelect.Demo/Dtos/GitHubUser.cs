using System.Text.Json.Serialization;

namespace Soenneker.Blazor.TomSelect.Demo.Dtos;

public sealed class GitHubUser
{
    [JsonPropertyName("login")]
    public string Login { get; set; } = "";

    [JsonPropertyName("avatar_url")]
    public string AvatarUrl { get; set; } = "";
}
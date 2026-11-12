using System.Text.Json.Serialization;

namespace Cronos.Features.Game.Steam.Model;

public class SteamUserInfo
{
    [JsonPropertyName("steamid")]
    public string Id { get; set; }

    [JsonPropertyName("personaname")]
    public string Name { get; set; }
}
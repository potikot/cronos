using System.Text.Json.Serialization;

namespace Cronos.Features.Game.Steam.Model.ApiResponses;

public class SAR_GetPlayerSummary
{
    [JsonPropertyName("response")]
    public ResponseData Response { get; set; }
    
    public class ResponseData
    {
        [JsonPropertyName("players")]
        public List<SteamUserInfo> Players { get; set; }
    }
}
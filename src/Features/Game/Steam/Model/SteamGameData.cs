using Cronos.Features.Game.Shared;

namespace Cronos.Features.Game.Steam.Model;

public class SteamGameData(
    int appId,
    string title,
    string url,
    string buyGameUrl = null) : BaseGameData(title, url, buyGameUrl)
{
    public int AppId { get; } = appId;
}

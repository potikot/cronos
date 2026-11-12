using Cronos.Features.Game.Shared;

namespace Cronos.Features.Game.EpicGames.Model;

public class EpicGameData(
    string title,
    string url,
    string buyGameUrl = null) : BaseGameData(title, url, buyGameUrl) { }
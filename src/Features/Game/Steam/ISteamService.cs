using Cronos.Bootstrap.Lifecylcle;
using Cronos.Features.Game.Shared;
using Cronos.Features.Game.Steam.Model;

namespace Cronos.Features.Game.Steam;

public interface ISteamService : IGameService, IOnStartAsync
{
    public Task<bool> Login(string apiKey, string clientId);
    
    public Task<SteamUserInfo> GetPlayerInfoAsync();
}
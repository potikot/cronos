namespace Cronos.Features.Game.Shared;

public interface IGameService
{
    List<BaseGameData> FreeGames { get; }
    bool IsLoggedIn { get; }

    Task<bool> Login();
    void Logout();

    Task<List<BaseGameData>> GetFreeGamesAsync();
    Task<bool> IsGameOwnedAsync(BaseGameData game);
}
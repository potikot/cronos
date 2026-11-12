namespace Cronos.Features.Game.Shared;

public abstract class BaseGameData
{
    public string Title { get; }
    public string Url { get; }
    public string BuyUrl { get; }
    
    public bool IsOwned { get; set; }
    
    protected BaseGameData(string title, string url, string buyUrl)
    {
        Title = title;
        Url = url;
        BuyUrl = buyUrl;
    }
}

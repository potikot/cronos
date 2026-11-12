namespace Cronos.Infrastructure.Save;

public interface ISaveOptions
{
    string SaveFileName { get; }
}

public class SaveOptions : ISaveOptions
{
    public string SaveFileName { get; set; }
}
namespace Cronos.Features.Yougile.Model;

public class YouGileBoard(string id)
{
    public string Id { get; } = id;
    public string Name { get; set; }
    
    public List<YouGileColumn> Columns { get; } = [];
}
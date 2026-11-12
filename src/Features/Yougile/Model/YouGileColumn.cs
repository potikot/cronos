namespace Cronos.Features.Yougile.Model;

public class YouGileColumn(string id)
{
    public string Id { get; } = id;
    public string Name { get; set; }
    
    public List<YougileTask> Tasks { get; } = [];
}
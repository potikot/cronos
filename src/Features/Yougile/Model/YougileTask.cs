using Newtonsoft.Json;

namespace Cronos.Features.Yougile.Model;

public class YougileTask
{
    [JsonIgnore] public string Id { get; set; }

    [JsonProperty("title")] public string Title { get; set; }
    [JsonProperty("description")] public string Description { get; set; }
    [JsonProperty("columnId")] public string ColumnId { get; set; }
}
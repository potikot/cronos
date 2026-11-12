namespace Cronos.Infrastructure.StringFormatter.Model;

public class FormatterParams : Dictionary<string, object>
{
    public FormatterParams() { }
    public FormatterParams(int capacity) : base(capacity) { }
}
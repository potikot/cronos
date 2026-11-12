namespace Cronos.Infrastructure.CommandProcessor.Commands;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
public class SettingAttribute(string name, string description) : Attribute
{
    public string Name { get; } = name;
    public string Description { get; } = description;
}
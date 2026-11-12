namespace Cronos.Bootstrap.DependencyInjection;

[AttributeUsage(AttributeTargets.Class)]
public class BindOrderAttribute(int order) : Attribute
{
    public int Order { get; } = order;
}
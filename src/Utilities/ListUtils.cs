namespace Cronos.Utilities;

public static class ListUtils
{
    public static bool InRange<T>(this IReadOnlyList<T> list, int index)
    {
        return index >= 0 && index < list.Count;
    }
}
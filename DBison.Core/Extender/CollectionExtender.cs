namespace DBison.Core.Extender;
public static class CollectionExtender
{
    public static bool IsEmpty<T>(this IEnumerable<T> source)
    {
        return source == null || !source.Any();
    }

    public static bool IsNotEmpty<T>(this IEnumerable<T> source) => !IsEmpty(source);
}

namespace DBison.Core.Extender;
public static class CollectionExtender
{
    #region [IsEmpty]
    public static bool IsEmpty<T>(this IEnumerable<T> source)
    {
        return source == null || !source.Any();
    }
    #endregion

    #region [IsNotEmpty]
    public static bool IsNotEmpty<T>(this IEnumerable<T> source) => !IsEmpty(source);
    #endregion
}

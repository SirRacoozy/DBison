namespace DBison.Core.Extender;
public static class NumericExtender
{
    public static long ToLongValue(this object value)
    {
        if (value == null || DBNull.Value == value)
            return 0;
        long.TryParse(value.ToString(), out var casted);

        return casted;
    }
}

using System.Text;

namespace DBison.Core.Extender;
public static class StringExtender
{
    public static string ToStringValue(this object rawValue)
    {
        if (rawValue == null)
            return string.Empty;
        else if (rawValue is string rawString)
            return rawString;
        else
            return rawValue.ToStringValue();
    }

    public static bool IsNotNullOrEmpty(this string value)
        => !string.IsNullOrEmpty(value);
    public static bool IsNullOrEmpty(this string value)
        => string.IsNullOrEmpty(value);

    public static bool IsNullOrEmpty(this object value) => value == null || value.ToString()?.Trim().Length == 0;

    public static byte[] ToByteArray(this string value)
        => Encoding.UTF8.GetBytes(value);

    public static string ToUTF8String(this string value)
        => Encoding.Unicode.GetString(Encoding.UTF8.GetBytes(value));

    public static bool IsEquals(this string value, string valueToCompare)
    {
        if (value == null && valueToCompare == null)
            return true;
        if ((value == null && valueToCompare != null) || (value != null && valueToCompare == null))
            return false;

        return value.Equals(valueToCompare, StringComparison.InvariantCultureIgnoreCase);
    }

    public static bool IsNullEmptyOrWhitespace(this string value)
    {
        return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
    }
    public static bool IsNotNullEmptyOrWhitespace(this string value)
    {
        return !IsNullEmptyOrWhitespace(value);
    }
}

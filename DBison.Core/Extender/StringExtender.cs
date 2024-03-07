using DBison.Core.Entities.Enums;
using System.Text;

namespace DBison.Core.Extender;
public static class StringExtender
{
    private const StringSplitOptions m_STRINGSPLITTING_NO_EMPTY_TTRIM = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
    public static string ToStringValue(this object rawValue)
    {
        if (rawValue == null)
            return string.Empty;
        else if (rawValue is string rawString)
            return rawString;
        else
            return rawValue.ToString();
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

    public static (string, eDMLOperator) ConvertToSelectStatement(this string value)
    {
        if (value == null)
            return (string.Empty, eDMLOperator.None);

        if (value.StartsWith("select", StringComparison.InvariantCultureIgnoreCase))
            return (value, eDMLOperator.None);
        else if (value.StartsWith("delete", StringComparison.InvariantCultureIgnoreCase))
            return (__ConvertDeleteToSelectStatement(value), eDMLOperator.Delete);
        else if (value.StartsWith("update", StringComparison.InvariantCultureIgnoreCase))
            return (__ConvertUpdateToSelectStatement(value), eDMLOperator.Update);

        return (string.Empty, eDMLOperator.None);

    }

    private static string __ConvertDeleteToSelectStatement(string value)
    {
        return value.Replace("delete", "select *", StringComparison.InvariantCultureIgnoreCase);
    }
    private static string __ConvertUpdateToSelectStatement(string value)
    {
        value = value.ToLower();
        var splittedBySet = value.Split("set", m_STRINGSPLITTING_NO_EMPTY_TTRIM).ToList();

        var table = splittedBySet.FirstOrDefault()?.Split(' ', m_STRINGSPLITTING_NO_EMPTY_TTRIM).LastOrDefault();

        var splittedByWhere = splittedBySet.LastOrDefault()?.Split("where", m_STRINGSPLITTING_NO_EMPTY_TTRIM).ToList();

        var splittedColumnsAssignments = splittedByWhere.FirstOrDefault()?.Split(",", m_STRINGSPLITTING_NO_EMPTY_TTRIM).ToList();

        var columnsNames = new List<string>();

        splittedColumnsAssignments.ForEach(x => columnsNames.Add(string.Join("", x.Take(x.IndexOf('='))).Trim()));

        StringBuilder sb = new();

        sb.Append("select ");
        var columns = string.Join(',', columnsNames);
        if(columnsNames.Count > 1)
            columns = columns.Take(columns.Length - 1).ToStringValue();

        sb.Append(columns);
        sb.Append(" from ");
        sb.Append(table);
        sb.Append(" where ");
        sb.Append(splittedByWhere.LastOrDefault() ?? string.Empty);

        return sb.ToString();
    }
}

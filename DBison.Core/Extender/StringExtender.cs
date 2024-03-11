using DBison.Core.Entities.Enums;
using System.Text;
using System.Text.RegularExpressions;

namespace DBison.Core.Extender;
public static class StringExtender
{
    private const StringSplitOptions m_STRINGSPLITTING_NO_EMPTY_TRIM = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
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
        if (value.IsNullEmptyOrWhitespace())
            return (string.Empty, eDMLOperator.None);

        if (value.StartsWith("select", StringComparison.InvariantCultureIgnoreCase))
            return (value, eDMLOperator.None);
        else if (value.StartsWith("delete", StringComparison.InvariantCultureIgnoreCase))
            return (__ConvertDeleteToSelectStatement(value), eDMLOperator.Delete);
        else if (value.StartsWith("update", StringComparison.InvariantCultureIgnoreCase))
            return (__ConvertUpdateToSelectStatement(value), eDMLOperator.Update);
        else if (value.StartsWith("insert", StringComparison.InvariantCultureIgnoreCase))
            return (__ConvertInsertToSelectStatement(value), eDMLOperator.Insert);

        return (string.Empty, eDMLOperator.None);

    }

    private static string __ConvertInsertToSelectStatement(string value)
    {
        var match = Regex.Match(value, @"INSERT INTO (\w+) ?\((.+)\) VALUES (\(.+\)),+", RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            throw new ArgumentException("Invalid SQL insert statement.");
        }

        var table = match.Groups[1].Value;
        var columns = match.Groups[2].Value.Split(',').Select(x => x.Trim()).ToArray();
        var valuesGroups = match.Groups[3].Value.Split(new[] { "),(" }, StringSplitOptions.None)
            .Select(x => x.Trim(' ', '(', ')').Split(',').Select(v => v.Trim()).ToArray()).ToArray();

        var selectStatements = valuesGroups.Select(values =>
        {
            var whereClause = string.Join(" AND ", columns.Zip(values, (c, v) => $"{c} = {v}"));
            return $"SELECT * FROM {table} WHERE {whereClause}";
        });

        return string.Join("; ", selectStatements);
    }
    private static string __ConvertDeleteToSelectStatement(string value)
    {
        var match = Regex.Match(value, @"DELETE FROM (\w+) WHERE (.+)", RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            throw new ArgumentException("Invalid SQL delete statement.");
        }

        var table = match.Groups[1].Value;
        var whereClause = match.Groups[2].Value;

        return $"SELECT * FROM {table} WHERE {whereClause}";
    }
    private static string __ConvertUpdateToSelectStatement(string value)
    {
        var match = Regex.Match(value, @"UPDATE (\w+) SET (.+) WHERE (.+)", RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            throw new ArgumentException("Invalid SQL update statement.");
        }

        var table = match.Groups[1].Value;
        var whereClause = match.Groups[3].Value;

        return $"SELECT * FROM {table} WHERE {whereClause}";
    }
}

using DBison.Core.Entities.Enums;
using System.Text;
using System.Text.RegularExpressions;

namespace DBison.Core.Extender;
public static class StringExtender
{
    private const StringSplitOptions m_STRINGSPLITTING_NO_EMPTY_TRIM = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

    #region [ToStringValue]
    public static string ToStringValue(this object rawValue)
    {
        if (rawValue == null)
            return string.Empty;
        else if (rawValue is string rawString)
            return rawString;
        else
            return rawValue.ToString();
    }
    #endregion

    #region [IsNotNullOrEmpty]
    public static bool IsNotNullOrEmpty(this string value)
        => !string.IsNullOrEmpty(value);
    #endregion

    #region [IsNullOrEmpty]
    public static bool IsNullOrEmpty(this string value)
        => string.IsNullOrEmpty(value);

    public static bool IsNullOrEmpty(this object value) => value == null || value.ToString()?.Trim().Length == 0;
    #endregion

    #region [ToByteArray]
    public static byte[] ToByteArray(this string value)
        => Encoding.UTF8.GetBytes(value);
    #endregion

    #region [ToUTF8String]
    public static string ToUTF8String(this string value)
        => Encoding.Unicode.GetString(Encoding.UTF8.GetBytes(value));
    #endregion

    #region [IsEquals]
    public static bool IsEquals(this string value, string valueToCompare)
    {
        if (value == null && valueToCompare == null)
            return true;
        if ((value == null && valueToCompare != null) || (value != null && valueToCompare == null))
            return false;

        return value.Equals(valueToCompare, StringComparison.InvariantCultureIgnoreCase);
    }
    #endregion

    #region [IsNotEquals]
    public static bool IsNotEquals(this string value, string valueToCompare) => !IsEquals(value, valueToCompare);
    #endregion

    #region [IsNullEmptyOrWhitespace]
    public static bool IsNullEmptyOrWhitespace(this string value)
    {
        return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
    }
    #endregion

    #region [IsNotNullEmptyOrWhitespace]
    public static bool IsNotNullEmptyOrWhitespace(this string value)
    {
        return !IsNullEmptyOrWhitespace(value);
    }
    #endregion

    #region [AllIndexesOf]
    public static List<int> AllIndexesOf(this string str, string value, StringComparison comparisonType)
    {
        if (value.IsNullOrEmpty())
            throw new ArgumentException("The string to find may not be empty", "value");
        List<int> indexes = new List<int>();
        for (int index = 0; ; index += value.Length)
        {
            index = str.IndexOf(value, index, comparisonType);
            if (index == -1)
                return indexes;
            indexes.Add(index);
        }
    }
    #endregion

    #region [ExtractStatements]
    public static List<string> ExtractStatements(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return new List<string>();

        input = input.RemoveComments();

        // List of common SQL command keywords that can start a new statement
        string[] sqlKeywords = new[] { "USE", "SELECT", "INSERT", "UPDATE", "DELETE", "CREATE", "ALTER", "DROP", "EXEC", "WITH" };

        // Pattern to detect command start (a keyword at the start of a line or after a semicolon)
        string pattern = $@"(^|\s|;)\b({string.Join("|", sqlKeywords)})\b";

        // Matches SQL commands considering keywords only at valid positions
        var matches = Regex.Matches(input, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);

        List<string> commands = new List<string>();
        int lastIndex = 0;
        string currentDatabase = null;

        foreach (Match match in matches)
        {
            int startIndex = match.Index;

            //Extract the command before the current keyword
            if (lastIndex < startIndex)
            {
                string command = input.Substring(lastIndex, startIndex - lastIndex).Trim();
                if (command.StartsWith("use", StringComparison.InvariantCultureIgnoreCase))
                {
                    lastIndex = startIndex;
                    continue;
                }
                if (!string.IsNullOrWhiteSpace(command))
                {
                    // If a USE statement is active, prepend it
                    if (!string.IsNullOrEmpty(currentDatabase) && !command.StartsWith("use", StringComparison.OrdinalIgnoreCase))
                    {
                        command = $"USE {currentDatabase};\n{command}";
                    }

                    commands.Add(command);
                }
            }

            // Check if this is a USE command
            string keyword = match.Groups[2].Value.ToUpper();
            if (keyword == "USE")
            {
                // Extract the database name
                var useMatch = Regex.Match(input.Substring(startIndex), @"USE\s+\[?([^\]\s;]+)\]?", RegexOptions.IgnoreCase);
                if (useMatch.Success)
                {
                    currentDatabase = useMatch.Groups[1].Value;
                }
            }

            lastIndex = startIndex;
        }

        // Add the final command
        if (lastIndex < input.Length)
        {
            string lastCommand = input.Substring(lastIndex).Trim();
            if (!string.IsNullOrWhiteSpace(lastCommand))
            {
                if (!string.IsNullOrEmpty(currentDatabase) && !lastCommand.StartsWith("use", StringComparison.OrdinalIgnoreCase))
                {
                    lastCommand = $"USE {currentDatabase};\n{lastCommand}";
                }

                commands.Add(lastCommand);
            }
        }

        return commands;
    }
    #endregion

    public static string RemoveComments(this string input)
    {
        // Entferne mehrzeilige Kommentare (/* ... */)
        string noMultiLineComments = Regex.Replace(input, @"/\*.*?\*/", string.Empty, RegexOptions.Singleline);

        // Entferne einzeilige Kommentare (-- ...)
        string noSingleLineComments = Regex.Replace(noMultiLineComments, @"--.*?$", string.Empty, RegexOptions.Multiline);

        return noSingleLineComments;
    }

    #region [ConvertToSelectStatement]
    public static (string, eDMLOperator) ConvertToSelectStatement(this string value)
    {
        if (value.IsNullEmptyOrWhitespace())
            return (string.Empty, eDMLOperator.None);

        try
        {
            if (value.StartsWith("select", StringComparison.InvariantCultureIgnoreCase))
                return (value, eDMLOperator.None);
            else if (value.StartsWith("delete", StringComparison.InvariantCultureIgnoreCase))
                return (__ConvertDeleteToSelectStatement(value), eDMLOperator.Delete);
            else if (value.StartsWith("update", StringComparison.InvariantCultureIgnoreCase))
                return (__ConvertUpdateToSelectStatement(value), eDMLOperator.Update);
            else if (value.StartsWith("insert", StringComparison.InvariantCultureIgnoreCase))
                return (__ConvertInsertToSelectStatement(value), eDMLOperator.Insert);
        }
        catch (Exception)
        {

        }

        return (string.Empty, eDMLOperator.None);

    }
    #endregion

    #region [__ConvertInsertToSelectStatement]
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
    #endregion

    #region [__ConvertDeleteToSelectStatement]
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
    #endregion

    #region [__ConvertUpdateToSelectStatement]
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
    #endregion
}

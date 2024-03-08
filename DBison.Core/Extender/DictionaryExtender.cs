using Microsoft.Data.SqlClient;
using System.Collections.Concurrent;

namespace DBison.Core.Extender;
public static class DictionaryExtender
{
    public static void ConvertToSqlParameter(this Dictionary<string, object> dict, SqlCommand command)
    {
        ArgumentNullException.ThrowIfNull(dict, nameof(dict));
        ArgumentNullException.ThrowIfNull(command, nameof(command));

        ConcurrentBag<SqlParameter> parameters = new();
        Parallel.ForEach(dict, item => parameters.Add(new(item.Key, item.Value)));

        // Adding all items to the parameters
        command.Parameters.AddRange([.. parameters]);
    }
}

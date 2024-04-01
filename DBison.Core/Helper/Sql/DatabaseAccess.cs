using DBison.Core.Entities;
using DBison.Core.Extender;
using Microsoft.Data.SqlClient;

namespace DBison.Core.Helper.Sql;
public class DatabaseAccess : IDisposable
{
    private SqlConnection? m_Connection = null;

    private bool m_Disposed = false;

    #region [DatabaseAccess]
    public DatabaseAccess(ServerInfo serverInfo, DatabaseInfo databaseInfo)
    {
        ArgumentNullException.ThrowIfNull(databaseInfo, nameof(databaseInfo));
        m_Connection = DatabaseConnectionManager.Instance.AddOrGetConnection(serverInfo, databaseInfo);
        m_Connection.Open();
    }
    #endregion

    #region [ExecuteCommand]
    public void ExecuteCommand(string sql, Dictionary<string, object>? parameter = null)
    {
        if (sql.IsNullEmptyOrWhitespace())
            throw new ArgumentNullException(nameof(sql));

        using var Command = new SqlCommand(sql, m_Connection);

        if (parameter is not null && parameter.Count != 0)
            parameter.ConvertToSqlParameter(Command);

        Command.ExecuteNonQuery();
    }
    #endregion

    #region [Dispose]
    public void Dispose()
    {
        __Dispose(true);
    }
    #endregion

    #region [__Dispose]
    private void __Dispose(bool disposing)
    {
        if (disposing)
        {
            if (m_Connection is not null)
            {
                try
                {
                    DatabaseConnectionManager.Instance.CloseConnection(m_Connection);
                }
                catch { /* Case can be ignored */ }

                m_Connection = null;
            }
        }
        m_Disposed = true;
    }
    #endregion
}

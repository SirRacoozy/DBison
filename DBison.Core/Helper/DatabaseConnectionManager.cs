using DBison.Core.Entities;
using Microsoft.Data.SqlClient;
using System.Collections.Concurrent;
using System.Data;

namespace DBison.Core.Helper;
public class DatabaseConnectionManager
{

    #region - ctor -
    /// <summary>
    /// Creates an instance of the database connection manager.
    /// </summary>
    internal DatabaseConnectionManager()
    {
        m_Connections = new();
    }
    #endregion

    #region - properties -

    #region [Instance]
    /// <summary>
    /// Gets the instance of the manager.
    /// </summary>
    public static DatabaseConnectionManager Instance = m_Manager ?? new();
    #endregion

    #region [m_Manager]
    /// <summary>
    /// Gets or sets the current manager.
    /// </summary>
    private static DatabaseConnectionManager? m_Manager { get; set; }
    #endregion

    #region [m_Connections]
    /// <summary>
    /// Gets or sets the connection dictionary.
    /// </summary>
    private ConcurrentDictionary<DatabaseInfo, SqlConnection> m_Connections { get; set; }
    #endregion

    #endregion

    #region - methods -

    #region [AddOrGetConnection]
    /// <summary>
    /// Add or get the connection for a provided DatabaseInfo object.
    /// </summary>
    /// <param name="databaseInfo">The database info object.</param>
    /// <returns>The sql connection.</returns>
    public SqlConnection AddOrGetConnection(DatabaseInfo databaseInfo)
    {
        if (m_Connections.ContainsKey(databaseInfo))
        {
            if (m_Connections.TryGetValue(databaseInfo, out var connection))
                return connection;
            connection = __CreateConnection(databaseInfo);
            m_Connections[databaseInfo] = connection;
            return connection;
        }

        var conn = __CreateConnection(databaseInfo);
        _ = m_Connections.TryAdd(databaseInfo, conn);
        return conn;
    }
    #endregion

    #region [CloseConnection]
    /// <summary>
    /// Closes the provided sql connection.
    /// </summary>
    /// <param name="connection">The sql connection to close.</param>
    /// <exception cref="ArgumentNullException">Throws if the connection is null.</exception>
    public void CloseConnection(SqlConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);
        if (connection.State != ConnectionState.Closed)
            connection.Close();
        var connectionsToRemove = m_Connections.Where(x => x.Value.Equals(connection));
        foreach (var conn in connectionsToRemove)
            _ = m_Connections.TryRemove(conn);
    }
    #endregion

    #region [CloseConnection]
    /// <summary>
    /// Closes the sql connection for the given database info object.
    /// </summary>
    /// <param name="databaseInfo">The database info object.</param>
    /// <exception cref="ArgumentNullException">Throws if the database info object is null.</exception>
    public void CloseConnection(DatabaseInfo databaseInfo)
    {
        ArgumentNullException.ThrowIfNull(databaseInfo);
        if (m_Connections.ContainsKey(databaseInfo))
            CloseConnection(m_Connections[databaseInfo]);
    }
    #endregion

    #region [__CreateConnection]
    /// <summary>
    /// Creates a connection for a given database info object.
    /// </summary>
    /// <param name="databaseInfo">The database info object.</param>
    /// <returns>The sql connection.</returns>
    private SqlConnection __CreateConnection(DatabaseInfo databaseInfo)
    {
        var builder = new SqlConnectionStringBuilder()
        {
            DataSource = databaseInfo.Server,
            InitialCatalog = databaseInfo.Name,
        };
        if (!databaseInfo.UseIntegratedSecurity)
        {
            builder.UserID = databaseInfo.Username;
            builder.Password = databaseInfo.Password;
        }
        return new SqlConnection(builder.ToString());
    }
    #endregion

    #endregion

}

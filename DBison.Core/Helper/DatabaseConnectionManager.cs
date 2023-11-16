﻿using DBison.Core.Entities;
using Microsoft.Data.SqlClient;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;

namespace DBison.Core.Helper;
public class DatabaseConnectionManager
{

	#region - ctor -
	internal DatabaseConnectionManager()
	{
		Connections = new();
	}
	#endregion

	#region - properties -

	#region [Instance]
	public static DatabaseConnectionManager Instance = Manager ?? new();
	#endregion

	#region [m_Manager]
	private static DatabaseConnectionManager? Manager { get; set; }
	#endregion

	#region [m_Connections]
	private ConcurrentDictionary<DatabaseInfo, SqlConnection> Connections { get; set; }
	#endregion

	#endregion

	#region - methods -

	#region [AddOrGetConnection]
	public SqlConnection AddOrGetConnection(DatabaseInfo databaseInfo)
	{
		if(Connections.ContainsKey(databaseInfo))
		{
			if (Connections.TryGetValue(databaseInfo, out var connection))
				return connection;
			connection = __CreateConnection(databaseInfo);
			Connections[databaseInfo] = connection;
			return connection;
		}

		var conn = __CreateConnection(databaseInfo);
        _ = Connections.TryAdd(databaseInfo, conn);
		return conn;
	}
	#endregion

	#region [CloseConnection]
	public void CloseConnection(SqlConnection connection)
	{
		if (connection is null) throw new ArgumentNullException(nameof(connection));
		if (connection.State != ConnectionState.Closed)
			connection.Close();
		var connectionsToRemove = Connections.Where(x => x.Value.Equals(connection));
		foreach (var conn in connectionsToRemove)
            _ = Connections.TryRemove(conn);
	}
	#endregion

	#region [CloseConnection]
	public void CloseConnection(DatabaseInfo databaseInfo)
	{
        ArgumentNullException.ThrowIfNull(databaseInfo);
        if (Connections.ContainsKey(databaseInfo))
			CloseConnection(Connections[databaseInfo]);
	}
	#endregion

	#region [__CreateConnection]
	private SqlConnection __CreateConnection(DatabaseInfo databaseInfo)
	{
		var builder = new SqlConnectionStringBuilder()
		{
			DataSource = databaseInfo.Server,
			InitialCatalog = databaseInfo.Name,
		};
		if(!databaseInfo.UseIntegratedSecurity)
		{
			builder.UserID = databaseInfo.Username;
			builder.Password = databaseInfo.Password;
		}
		return new SqlConnection(builder.ToString());
	}
	#endregion

	#endregion

}

using DBison.Core.Entities;
using DBison.Core.Extender;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DBison.Core.Helper;
public class ServerQueryHelper
{
    ServerInfo m_Server;
    private SqlCommand m_Command;
    public ServerQueryHelper(ServerInfo serverInfo)
    {
        m_Server = serverInfo;
    }

    public bool IgnoreNextException { get; set; }

    public void LoadServerObjects()
    {
        __LoadDataBases();
    }

    public void LoadTables(DatabaseInfo databaseInfo)
    {
        try
        {
            if (databaseInfo == null)
                return;
            if (databaseInfo is ExtendedDatabaseInfo extendedDatabase)
            {
                var sql = "SELECT name, type  FROM sys.all_objects " +
              "WHERE type IN ('U') AND is_ms_shipped != 1 " +
              "ORDER BY name ASC";
                using var access = new DataConnection(databaseInfo);
                var reader = access.GetReader(sql);
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var name = reader[0].ToStringValue();
                        extendedDatabase.Tables.Add(new DBisonTable(name, m_Server, extendedDatabase));
                    }
                }
            }
        }
        catch (Exception)
        {
        }
    }

    public void LoadViews(DatabaseInfo databaseInfo)
    {
        try
        {
            if (databaseInfo == null)
                return;
            if (databaseInfo is ExtendedDatabaseInfo extendedDatabase)
            {
                var sql = "SELECT name, type  FROM sys.all_objects " +
              "WHERE type IN ('V') AND is_ms_shipped != 1 " +
              "ORDER BY name ASC";
                using var access = new DataConnection(databaseInfo);
                var reader = access.GetReader(sql);
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var name = reader[0].ToStringValue();
                        extendedDatabase.Views.Add(new DBisonView(name, m_Server, extendedDatabase));
                    }
                }
            }
        }
        catch (Exception)
        {
        }
    }

    public void LoadTrigger(DatabaseInfo databaseInfo)
    {
        try
        {
            if (databaseInfo == null)
                return;
            if (databaseInfo is ExtendedDatabaseInfo extendedDatabase)
            {
                var sql = "SELECT name, type  FROM sys.all_objects " +
              "WHERE type IN ('TR') AND is_ms_shipped != 1 " +
              "ORDER BY name ASC";
                using var access = new DataConnection(databaseInfo);
                var reader = access.GetReader(sql);
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var name = reader[0].ToStringValue();
                        extendedDatabase.Triggers.Add(new DBisonTrigger(name, m_Server, extendedDatabase));
                    }
                }
            }
        }
        catch (Exception)
        {
        }
    }

    public void LoadProcedures(DatabaseInfo databaseInfo)
    {
        try
        {
            if (databaseInfo == null)
                return;
            if (databaseInfo is ExtendedDatabaseInfo extendedDatabase)
            {
                var sql = "SELECT name, type  FROM sys.all_objects " +
              "WHERE type IN ('P') AND is_ms_shipped != 1 " +
              "ORDER BY name ASC";
                using var access = new DataConnection(databaseInfo);
                var reader = access.GetReader(sql);
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var name = reader[0].ToStringValue();
                        extendedDatabase.Procedures.Add(new DBisonStoredProcedure(name, m_Server, extendedDatabase));
                    }
                }
            }
        }
        catch (Exception)
        {
        }
    }

    public DataTable FillDataTable(DatabaseInfo databaseInfo, string sql, Action<Exception> errorCallback)
    {
        if (sql.IsNullOrEmpty())
            return null;

        try
        {
            using var access = new DataConnection(databaseInfo);
            using SqlDataAdapter dataAdapter = new SqlDataAdapter(sql, access.GetConnectionRef());
            m_Command = dataAdapter.SelectCommand ?? dataAdapter.UpdateCommand ?? dataAdapter.InsertCommand ?? dataAdapter.DeleteCommand;
            using SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
            var table = new DataTable();
            dataAdapter.Fill(table);
            return table;
        }
        catch (Exception ex)
        {
            if (!IgnoreNextException)
                errorCallback(ex);
            IgnoreNextException = false;
            return null;
        }
        finally
        {
            m_Command = null;
        }
    }

    public void Cancel()
    {
        IgnoreNextException = true;
        m_Command?.Cancel();
    }

    private void __LoadDataBases()
    {
        if (m_Server == null)
            return;
        var sql = "SELECT name as dataBaseName, state_desc isOnline FROM sys.databases " +
            "WHERE name NOT IN ('master','tempdb','model','msdb')" +
            "ORDER BY name ASC";
        using var access = new DataConnection(new DatabaseInfo("master", m_Server, null));
        var reader = access.GetReader(sql);
        if (reader != null && reader.HasRows)
        {
            while (reader.Read())
            {
                m_Server.DatabaseInfos.Add(new ExtendedDatabaseInfo(reader[0].ToStringValue(), m_Server, null)
                {

                });
            }
        }
    }
}
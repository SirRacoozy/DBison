using DBison.Core.Entities;
using DBison.Core.Extender;

namespace DBison.Core.Helper;
public class ServerQueryHelper
{
    ServerInfo m_Server;
    public ServerQueryHelper(ServerInfo serverInfo)
    {
        m_Server = serverInfo;
    }

    public void LoadServerObjects()
    {
        __LoadDataBases();
    }

    private void __LoadDataBases()
    {
        if (m_Server == null)
            return;
        var sql = "SELECT name as dataBaseName, state_desc isOnline FROM sys.databases " +
            "WHERE name NOT IN ('master','tempdb','model','msdb')" +
            "ORDER BY name ASC";
        using (var access = new DataConnection(new DatabaseInfo("master", m_Server)))
        {
            var reader = access.GetReader(sql);
            if (reader != null && reader.HasRows)
            {
                while (reader.Read())
                {
                    m_Server.DatabaseInfos.Add(new ExtendedDatabaseInfo(reader[0].ToStringValue(), m_Server)
                    {

                    });
                }
            }
        }
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
              "WHERE type IN ('U')";
                using (var access = new DataConnection(databaseInfo))
                {
                    var reader = access.GetReader(sql);
                    if (reader != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var name = reader[0].ToStringValue();
                            extendedDatabase.Tables.Add(new Table(name));
                        }
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
              "WHERE type IN ('V')";
                using (var access = new DataConnection(databaseInfo))
                {
                    var reader = access.GetReader(sql);
                    if (reader != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var name = reader[0].ToStringValue();
                            extendedDatabase.Views.Add(new View(name));
                        }
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
              "WHERE type IN ('TR')";
                using (var access = new DataConnection(databaseInfo))
                {
                    var reader = access.GetReader(sql);
                    if (reader != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var name = reader[0].ToStringValue();
                            extendedDatabase.Triggers.Add(new Trigger(name));
                        }
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
              "WHERE type IN ('P')";
                using (var access = new DataConnection(databaseInfo))
                {
                    var reader = access.GetReader(sql);
                    if (reader != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var name = reader[0].ToStringValue();
                            extendedDatabase.Procedures.Add(new StoredProcedure(name));
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
        }
    }
}
using DBison.Core.Entities;
using DBison.Core.Extender;
using DBison.Core.Utils.SettingsSystem;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DBison.Core.Helper;
public class ServerQueryHelper
{
    ServerInfo m_Server;
    private SqlCommand m_Command;
    private string m_DefaultDataPath;

    #region [ServerQueryHelper]
    public ServerQueryHelper(ServerInfo serverInfo)
    {
        m_Server = serverInfo;
        __GetServerDefaultProperties();
    }
    #endregion

    #region - public methods -

    #region [IgnoreNextException]
    public bool IgnoreNextException { get; set; }
    #endregion

    #region [LoadServerObjects]
    public void LoadServerObjects()
    {
        __LoadDataBases();
    }
    #endregion

    #region [LoadTables]
    public void LoadTables(DatabaseInfo databaseInfo, string filter)
    {
        try
        {
            if (databaseInfo == null || databaseInfo.DataBaseState != eDataBaseState.ONLINE)
                return;
            if (databaseInfo is ExtendedDatabaseInfo extendedDatabase)
            {
                extendedDatabase.Tables.Clear();
                var sql = "SELECT name, type  FROM sys.all_objects " +
              "WHERE type IN ('U') AND is_ms_shipped != 1 ";
                if (filter.IsNotNullOrEmpty())
                    sql += $" AND name LIKE '%{filter}%'";
                sql += " ORDER BY name ASC";
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
    #endregion

    #region [LoadViews]
    public void LoadViews(DatabaseInfo databaseInfo, string filter)
    {
        try
        {
            if (databaseInfo == null || databaseInfo.DataBaseState != eDataBaseState.ONLINE)
                return;
            if (databaseInfo is ExtendedDatabaseInfo extendedDatabase)
            {
                extendedDatabase.Views.Clear();
                var sql = "SELECT name, type  FROM sys.all_objects " +
              "WHERE type IN ('V') AND is_ms_shipped != 1 ";
                if (filter.IsNotNullOrEmpty())
                    sql += $" AND name LIKE '%{filter}%'";
                sql += " ORDER BY name ASC";
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
    #endregion

    #region [LoadTrigger]
    public void LoadTrigger(DatabaseInfo databaseInfo, string filter)
    {
        try
        {
            if (databaseInfo == null || databaseInfo.DataBaseState != eDataBaseState.ONLINE)
                return;
            if (databaseInfo is ExtendedDatabaseInfo extendedDatabase)
            {
                extendedDatabase.Triggers.Clear();
                var sql = "SELECT name, type  FROM sys.all_objects " +
              "WHERE type IN ('TR') AND is_ms_shipped != 1 ";
                if (filter.IsNotNullOrEmpty())
                    sql += $" AND name LIKE '%{filter}%'";
                sql += " ORDER BY name ASC";
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
    #endregion

    #region [LoadProcedures]
    public void LoadProcedures(DatabaseInfo databaseInfo, string filter)
    {
        try
        {
            if (databaseInfo == null || databaseInfo.DataBaseState != eDataBaseState.ONLINE)
                return;
            if (databaseInfo is ExtendedDatabaseInfo extendedDatabase)
            {
                extendedDatabase.Procedures.Clear();
                var sql = "SELECT name, type  FROM sys.all_objects " +
              "WHERE type IN ('P') AND is_ms_shipped != 1 ";
                if (filter.IsNotNullOrEmpty())
                    sql += $" AND name LIKE '%{filter}%'";
                sql += " ORDER BY name ASC";
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
    #endregion

    #region [FillDataTable]
    public DataTable FillDataTable(DatabaseInfo databaseInfo, string sql, Action<Exception> errorCallback)
    {
        if (sql.IsNullOrEmpty() || databaseInfo.DataBaseState != eDataBaseState.ONLINE)
            return null;

        return __GetDataTableForSingleSQLStatement(databaseInfo, sql, errorCallback);
    }

    private DataTable __GetDataTableForSingleSQLStatement(DatabaseInfo databaseInfo, string sql, Action<Exception> errorCallback)
    {
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
    #endregion

    #region [GetDataBaseState]
    public eDataBaseState GetDataBaseState(DatabaseInfo databaseInfo)
    {
        try
        {
            if (databaseInfo == null || databaseInfo.Name.IsNullOrEmpty())
                return eDataBaseState.OFFLINE;

            var sql = $"SELECT state isOnline FROM sys.databases WHERE name = '{databaseInfo.Name}' ";
            using var access = new DataConnection(new DatabaseInfo("master", m_Server, null));
            var reader = access.GetReader(sql);
            if (reader != null && reader.HasRows)
            {
                while (reader.Read())
                {
                    return (eDataBaseState)Convert.ToInt32(reader[0]);
                }
            }
            return eDataBaseState.OFFLINE;
        }
        catch (Exception)
        {
            return eDataBaseState.OFFLINE;
        }
    }
    #endregion

    #region [SwitchDataBaseStatus]
    public void SwitchDataBaseStatus(DatabaseInfo databaseInfo)
    {
        if (databaseInfo == null || databaseInfo.Name.IsNullOrEmpty())
            return;
        __SetDatabaseState(databaseInfo, databaseInfo.DataBaseState == eDataBaseState.ONLINE ? eDataBaseState.OFFLINE : eDataBaseState.ONLINE);
    }
    #endregion

    #region [Cancel]
    public void Cancel()
    {
        IgnoreNextException = true;
        m_Command?.Cancel();
        IgnoreNextException = false;
    }
    #endregion

    #region [CloneDataBase]
    public void CloneDataBase(DatabaseInfo dataBase, string newName, string dataFileName, string logFileName)
    {
        if (dataBase == null || newName.IsNullOrEmpty())
            return;

        var sql = $@"USE [master];
CREATE DATABASE [{newName}]
    ON (FILENAME = '{dataFileName}'),
       (FILENAME = '{logFileName}')
    FOR ATTACH;";


        using var access = new DataConnection(__GetMasterDataBaseInfo());
        access.ExecuteNonQuery(sql);
    }
    #endregion

    #region [TakeDataBaseOffline]
    public void TakeDataBaseOffline(DatabaseInfo databaseInfo)
    {
        __SetDatabaseState(databaseInfo, eDataBaseState.OFFLINE);
    }
    #endregion

    #region [TakeDataBaseOnline]
    public void TakeDataBaseOnline(DatabaseInfo databaseInfo)
    {
        __SetDatabaseState(databaseInfo, eDataBaseState.ONLINE);
    }
    #endregion

    #region [DeleteDataBase]
    public void DeleteDataBase(DatabaseInfo databaseInfo)
    {
        __SetDatabaseState(databaseInfo, eDataBaseState.OFFLINE);
        var sql = "USE [master] \n";
        sql += $" DROP DATABASE [{databaseInfo.Name}]";
        using var access = new DataConnection(__GetMasterDataBaseInfo());
        access.ExecuteNonQuery(sql);
    }
    #endregion

    #region [RenameDataBase]
    public void RenameDataBase(DatabaseInfo databaseInfo, string newName)
    {
        var sql = $@"USE [MASTER] ALTER DATABASE [{databaseInfo.Name}] MODIFY NAME = [{newName}] ;";
        using var access = new DataConnection(__GetMasterDataBaseInfo());
        access.ExecuteNonQuery(sql);
    }
    #endregion

    #region [BackupDataBase]
    public void BackupDataBase(DatabaseInfo dataBase, string backupPath)
    {
        var sql = @$"USE [master];
BACKUP DATABASE [{dataBase.Name}]
TO DISK = '{backupPath}';";
        using var access = new DataConnection(__GetMasterDataBaseInfo());
        access.ExecuteNonQuery(sql);
    }
    #endregion

    #region [RestoreBackup]
    public void RestoreBackup(DatabaseInfo dataBase, string backupPath)
    {
        string restoreAsName = dataBase.Name;
        var backupPreviewData = __GetLogicalFileNamesByBackupFile(backupPath);
        __KillAllConnections(dataBase);
        var builder = new System.Text.StringBuilder();
        _ = builder.Append("USE [master] \n");
        builder.Append($@"RESTORE DATABASE [{restoreAsName}]
FROM DISK = N'{backupPath}'
WITH REPLACE,
    MOVE '{backupPreviewData["DataFile"]}' TO '{m_DefaultDataPath}{restoreAsName}.mdf',
    MOVE '{backupPreviewData["LogFile"]}' TO '{m_DefaultDataPath}{restoreAsName}.ldf'");
        using var access = new DataConnection(__GetMasterDataBaseInfo());
        access.ExecuteNonQuery(builder.ToString());
    }
    #endregion

    #region [DeleteDatabaseFile]
    public void DeleteDatabaseFile(DatabaseInfo databaseInfo, string fileName)
    {
        //Does not work, stupid topic
        var sql = @$"USE master;
ALTER DATABASE [{databaseInfo.Name}]
REMOVE FILE [{fileName}];";
        using var access = new DataConnection(__GetMasterDataBaseInfo());
        access.ExecuteNonQuery(sql);
    }
    #endregion

    #endregion

    #region - private methods -

    #region [__LoadDataBases]
    private void __LoadDataBases()
    {
        if (m_Server == null)
            return;

        var sql = $@"SELECT 
    sysDb.name AS dataBaseName, 
    sysDb.state AS isOnline,
    STRING_AGG(dataF.physical_name, ', ') AS dataFileLocations,
    SUM(dataF.size) AS totalDataFileSize,
    STRING_AGG(logF.physical_name, ', ') AS logFileLocations,
    SUM(logF.size) AS totalLogFileSize
FROM sys.databases sysDb
LEFT JOIN sys.master_files dataF
    ON sysDb.database_id = dataF.database_id AND dataF.type_desc = 'ROWS'
LEFT JOIN sys.master_files logF
    ON sysDb.database_id = logF.database_id AND logF.type_desc = 'LOG'
{(Settings.ShowSystemDatabases ? "" : "WHERE sysDb.name NOT IN ('master','tempdb','model','msdb')")}
GROUP BY sysDb.name, sysDb.state
ORDER BY sysDb.name ASC;
";

        m_Server.DatabaseInfos.Clear();
        using var access = new DataConnection(new DatabaseInfo("master", m_Server, null));
        var reader = access.GetReader(sql);
        if (reader != null && reader.HasRows)
        {
            while (reader.Read())
            {
                var dataBaseInfo = new ExtendedDatabaseInfo(reader[0].ToStringValue(), m_Server, null)
                {
                    DataBaseState = (eDataBaseState)Convert.ToInt32(reader[1]),
                    DataFileLocation = reader[2].ToStringValue(),
                    DataFileSize = reader[3].ToLongValue(),
                    LogFileLocation = reader[4].ToStringValue(),
                    LogFileSize = reader[5].ToLongValue(),
                    IsRealDataBaseNode = true,
                };
                dataBaseInfo.ExpectedBackupDirectory = $@"{Path.GetDirectoryName(dataBaseInfo.DataFileLocation)}\backup\[{dataBaseInfo.Name}]_BACKUP";
                m_Server.DatabaseInfos.Add(dataBaseInfo);
            }
        }
    }
    #endregion

    #region [__GetMasterDataBaseInfo]
    private DatabaseInfo __GetMasterDataBaseInfo()
    {
        return new DatabaseInfo("master", m_Server, null);
    }
    #endregion

    #region [__SetDatabaseState]
    private void __SetDatabaseState(DatabaseInfo databaseInfo, eDataBaseState newState)
    {
        string newStateString;
        if (newState == eDataBaseState.ONLINE)
        {
            newStateString = "ONLINE";
        }
        else if (newState == eDataBaseState.OFFLINE)
        {
            __KillAllConnections(databaseInfo);
            newStateString = "OFFLINE";
        }
        else
        {
            return;
        }
        var masterRef = __GetMasterDataBaseInfo();
        string sql = @$"USE master; ALTER DATABASE [{databaseInfo.Name}] SET {newStateString} WITH ROLLBACK IMMEDIATE;";
        var access = new DataConnection(masterRef);
        access.ExecuteNonQuery(sql);
        access.Dispose();
    }
    #endregion

    #region [__KillAllConnections]
    private void __KillAllConnections(DatabaseInfo databaseInfo)
    {
        if (databaseInfo == null || databaseInfo.DataBaseState != eDataBaseState.ONLINE || databaseInfo.Name.IsNullOrEmpty())
            return;

        var sql = @$"
USE master;
DECLARE @dbname NVARCHAR(128)
SET @dbname = N'{databaseInfo.Name}';

DECLARE @killstmt NVARCHAR(4000) = '';

SELECT @killstmt = @killstmt + 'KILL ' + CONVERT(VARCHAR(10), spid) + ';'
FROM master.dbo.sysprocesses
WHERE dbid = DB_ID(@dbname)
  AND spid > 50;
EXEC(@killstmt);";
        using var access = new DataConnection(__GetMasterDataBaseInfo());
        access.ExecuteNonQuery(sql);
    }
    #endregion

    #region [__GetLogicalFileNamesByBackupFile]
    private Dictionary<string, string> __GetLogicalFileNamesByBackupFile(string backupFile)
    {
        var DataFile = string.Empty;
        var LogFile = string.Empty;
        var sql = $"RESTORE FILELISTONLY FROM DISK = '{backupFile}'";
        using var access = new DataConnection(new DatabaseInfo("master", m_Server, null));
        var reader = access.GetReader(sql);
        {
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var Type = reader["Type"].ToStringValue();
                    if (Type.Equals("D"))
                        DataFile = reader["LogicalName"].ToStringValue();
                    else if (Type.Equals("L"))
                        LogFile = reader["LogicalName"].ToStringValue();
                }
                reader.Close();
            }
        }
        var BackupFileList = new Dictionary<string, string>
        {
            { nameof(DataFile), DataFile },
            { nameof(LogFile), LogFile }
        };
        return BackupFileList;
    }
    #endregion

    #region [__GetServerDefaultProperties]
    private void __GetServerDefaultProperties()
    {
        try
        {
            using var access = new DataConnection(new DatabaseInfo("master", m_Server, null));
            var reader = access.GetReader(@"SELECT SERVERPROPERTY('InstanceDefaultDataPath') AS InstanceDefaultDataPath");
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                        m_DefaultDataPath = reader["InstanceDefaultDataPath"].ToStringValue();
                    reader.Close();
                }
            }
        }
        catch (Exception)
        {
        }
    }
    #endregion

    #endregion

}
using DBison.Core.Entities;
using DBison.Core.Extender;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DBison.Core.Database;
public class MSSQLDatabaseAccess : IDatabaseAccess
{
    private bool m_Disposed = false;
    private SqlConnection m_Connection;
    private SqlCommand? m_Command = null;

    #region - ctor -
    public MSSQLDatabaseAccess(ServerInfo serverInfo)
    {
        ArgumentNullException.ThrowIfNull(nameof(serverInfo));

        var result = __CreateConnection(serverInfo);

        if (result != null && !result.Succeeded)
            throw new ArgumentException(result.Message + "\n" + (result.Exception?.Message ?? string.Empty), nameof(serverInfo));
    }
    #endregion

    #region - methods -

    #region - public methods -

    #region [BackupDatabase]
    public OperationResult BackupDatabase(string name, string path) => throw new NotImplementedException();
    #endregion

    #region [Cancel]
    public OperationResult Cancel() => throw new NotImplementedException();
    #endregion

    #region [CloneDatabase]
    public OperationResult CloneDatabase(string name, string newName, string dataFileName, string logFileName) => throw new NotImplementedException();
    #endregion

    #region [DeleteDatabase]
    public OperationResult DeleteDatabase(string name) => throw new NotImplementedException();
    #endregion

    #region [ExecuteCommand]
    public OperationResult ExecuteCommand(string sql, Dictionary<string, object> parameters) => throw new NotImplementedException();
    #endregion

    #region [ExecuteDataReader]
    public OperationResult ExecuteDataReader(string sql, Dictionary<string, object> parameters) => throw new NotImplementedException();
    #endregion

    #region [ExecuteScalar]
    public OperationResult ExecuteScalar(string sql, Dictionary<string, object> parameters) => throw new NotImplementedException();
    #endregion

    #region [GetDatabaseState]
    public OperationResult<eDataBaseState> GetDatabaseState(string name) => throw new NotImplementedException();
    #endregion

    #region [GetDataTable]
    public OperationResult<DataTable> GetDataTable(string sql, Dictionary<string, object> parameters) => throw new NotImplementedException();
    #endregion

    #region [PopulateExtendedDatabaseInfo]
    public OperationResult PopulateExtendedDatabaseInfo(ExtendedDatabaseInfo extendedDatabaseInfo, string filter) => throw new NotImplementedException();
    #endregion

    #region [RenameDatabase]
    public OperationResult RenameDatabase(string name, string newName) => throw new NotImplementedException();
    #endregion

    #region [RestoreBackup]
    public OperationResult RestoreBackup(string name, string path) => throw new NotImplementedException();
    #endregion

    #region [SwitchDatabaseState]
    public OperationResult SwitchDatabaseState(string name) => throw new NotImplementedException();
    #endregion

    #endregion

    #region - private methods -

    #region [__OpenDatabaseConnection]
    private OperationResult __OpenDatabaseConnection()
    {
        try
        {
            m_Connection.Open();
            return OperationResult.Ok();
        }
        catch (Exception _)
        {
            return OperationResult.Fail(TextConsts.OPERATION_FAILED_1.Format(nameof(__OpenDatabaseConnection)), _);
        }
    }
    #endregion

    #region [__CloseConnection]
    private OperationResult __CloseConnection()
    {
        try
        {
            if (m_Connection != null && m_Connection.State != ConnectionState.Closed)
                m_Connection.Close();

            return OperationResult.Ok();
        } 
        catch(Exception _)
        {
            return OperationResult.Fail(TextConsts.OPERATION_FAILED_1.Format(nameof(__OpenDatabaseConnection)), _);
        }
    }
    #endregion

    #region [__CreateConnection]
    private OperationResult __CreateConnection(ServerInfo serverInfo)
    {
        try
        {

            SqlConnectionStringBuilder builder = new();
            builder.DataSource = serverInfo.Name;
            builder.InitialCatalog = serverInfo.DatabaseInfos?.FirstOrDefault()?.Name ?? string.Empty;
            builder.TrustServerCertificate = true;

            if (serverInfo.UseIntegratedSecurity)
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.UserID = serverInfo.Username;
                builder.Password = serverInfo.Password;
            }

            m_Connection = new(builder.ToString());

            return OperationResult.Ok();
        }
        catch (Exception _)
        {
            return OperationResult.Fail(TextConsts.OPERATION_FAILED_1.Format(nameof(__CreateConnection)), _);
        }

    }
    #endregion

    #endregion

    #endregion

    #region [Dispose]
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (m_Disposed)
            return;

        if (disposing)
        {
            m_Command?.Dispose();

            __CloseConnection();
            m_Connection?.Dispose();
        }

        m_Disposed = true;
    }
    #endregion 
}

using DBison.Core.Entities;
using DBison.Core.Extender;
using Microsoft.Data.SqlClient;

namespace DBison.Core.Helper;
public class DataConnection : IDisposable
{
    private bool m_DisposedValue;

    private readonly SqlConnection m_Connection;

    #region [DataConnection]
    public DataConnection(DatabaseInfo dataBaseInfo)
    {
        if (dataBaseInfo == null || dataBaseInfo.Server == null || dataBaseInfo.Name.IsNullOrEmpty() || dataBaseInfo.Server.IsNullOrEmpty())
            return;
        m_Connection = DatabaseConnectionManager.Instance.AddOrGetConnection(dataBaseInfo.Server, dataBaseInfo);
    }
    #endregion

    #region [GetReader]
    public SqlDataReader GetReader(string sql)
    {
        __Prepare();
        return __GetCommand(sql).ExecuteReader();
    }
    #endregion

    #region [ExecuteNonQuery]
    public int ExecuteNonQuery(string sql)
    {
        __Prepare();
        return __GetCommand(sql).ExecuteNonQuery();
    }
    #endregion

    #region [ExecuteScalar]
    public object ExecuteScalar(string sql)
    {
        __Prepare();
        return __GetCommand(sql).ExecuteScalar();
    }
    #endregion

    #region [GetConnectionRef]
    public SqlConnection GetConnectionRef() => m_Connection;
    #endregion

    #region [CloseConnection]
    public void CloseConnection()
    {
        __CleanUp();
    }
    #endregion

    #region [__Prepare]
    private void __Prepare()
    {
        if (m_Connection.State != System.Data.ConnectionState.Open)
            m_Connection.Open();
    }
    #endregion

    #region [__CleanUp]
    private void __CleanUp()
    {
        m_Connection.Close();
    }
    #endregion

    #region [__GetCommand]
    private SqlCommand __GetCommand(string sqlText)
    {
        return new SqlCommand(sqlText, m_Connection);
    }
    #endregion

    #region [Dispose]
    protected virtual void Dispose(bool disposing)
    {
        if (!m_DisposedValue)
        {
            if (disposing)
            {
                __CleanUp();
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            m_DisposedValue = true;
        }
    }
    #endregion

    #region [Dispose]
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion   
}

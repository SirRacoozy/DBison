using DBison.Core.Entities;
using DBison.Core.Extender;
using Microsoft.Data.SqlClient;

namespace DBison.Core.Helper;
public class DataConnection : IDisposable
{
    private bool m_DisposedValue;
    private readonly SqlConnection m_Connection;

    public DataConnection(DatabaseInfo dataBaseInfo)
    {
        if (dataBaseInfo == null || dataBaseInfo.Server == null || dataBaseInfo.Name.IsNullOrEmpty() || dataBaseInfo.Server.IsNullOrEmpty())
            return;
        m_Connection = DatabaseConnectionManager.Instance.AddOrGetConnection(dataBaseInfo.Server, dataBaseInfo);
    }

    public SqlDataReader GetReader(string sql)
    {
        __Prepare();
        return __GetCommand(sql).ExecuteReader();
    }


    public int ExecuteNonQuery(string sql)
    {
        __Prepare();
        return __GetCommand(sql).ExecuteNonQuery();
    }

    public object ExecuteScalar(string sql)
    {
        __Prepare();
        return __GetCommand(sql).ExecuteScalar();
    }

    public SqlConnection GetConnectionRef() => m_Connection;

    public void CloseConnection()
    {
        __CleanUp();
    }


    private void __Prepare()
    {
        if (m_Connection.State != System.Data.ConnectionState.Open)
            m_Connection.Open();
    }

    private void __CleanUp()
    {
        m_Connection.Close();
    }

    private SqlCommand __GetCommand(string sqlText)
    {
        return new SqlCommand(sqlText, m_Connection);
    }


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

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~DataConnection()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

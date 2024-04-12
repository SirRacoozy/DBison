using DBison.Core.Entities;
using System.Data;

namespace DBison.Core.Database;
public interface IDatabaseAccess : IDisposable
{
    OperationResult ExecuteCommand(string sql, Dictionary<string, object> parameters);
    OperationResult ExecuteDataReader(string sql, Dictionary<string, object> parameters);
    OperationResult ExecuteScalar(string sql, Dictionary<string, object> parameters);
    OperationResult PopulateExtendedDatabaseInfo(ExtendedDatabaseInfo extendedDatabaseInfo, string filter);
    OperationResult<DataTable> GetDataTable(string sql, Dictionary<string, object> parameters);
    OperationResult<eDataBaseState> GetDatabaseState(string name);
    OperationResult SwitchDatabaseState(string name);
    OperationResult Cancel();
    OperationResult CloneDatabase(string name, string newName, string dataFileName, string logFileName);
    OperationResult DeleteDatabase(string name);
    OperationResult RenameDatabase(string name, string newName);
    OperationResult BackupDatabase(string name, string path);
    OperationResult RestoreBackup(string name, string path);
}

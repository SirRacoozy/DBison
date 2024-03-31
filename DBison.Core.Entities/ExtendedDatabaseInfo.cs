namespace DBison.Core.Entities;
public class ExtendedDatabaseInfo : DatabaseInfo
{
    public ExtendedDatabaseInfo(string name, ServerInfo server, DatabaseInfo dataBase) : base(name, server, dataBase)
    {
    }

    public List<DBisonStoredProcedure> Procedures { get; private set; } = [];
    public List<DBisonTable> Tables { get; private set; } = [];
    public List<DBisonTrigger> Triggers { get; private set; } = [];
    public List<DBisonView> Views { get; private set; } = [];
    public string DataFileLocation { get; set; }
    public long DataFileSize { get; set; }
    public string LogFileLocation { get; set; }
    public long LogFileSize { get; set; }

}

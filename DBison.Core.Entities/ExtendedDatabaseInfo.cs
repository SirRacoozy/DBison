namespace DBison.Core.Entities;
public class ExtendedDatabaseInfo : DatabaseInfo
{
    public ExtendedDatabaseInfo(string name, ServerInfo server, DatabaseInfo dataBase) : base(name, server, dataBase)
    {
    }

    public List<StoredProcedure> Procedures { get; private set; } = [];
    public List<Table> Tables { get; private set; } = [];
    public List<Trigger> Triggers { get; private set; } = [];
    public List<View> Views { get; private set; } = [];

}

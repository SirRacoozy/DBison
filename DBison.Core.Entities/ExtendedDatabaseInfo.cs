namespace DBison.Core.Entities;
public class ExtendedDatabaseInfo : DatabaseInfo
{
    public ExtendedDatabaseInfo(string name, ServerInfo serverInfo) : base(name, serverInfo)
    {
    }

    public List<StoredProcedure> Procedures { get; private set; } = [];
    public List<Table> Tables { get; private set; } = [];
    public List<Trigger> Triggers { get; private set; } = [];
    public List<View> Views { get; private set; } = [];

}

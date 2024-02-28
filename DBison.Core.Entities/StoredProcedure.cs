namespace DBison.Core.Entities;
public class StoredProcedure : DatabaseObjectBase
{
    public StoredProcedure(string name, ServerInfo server, DatabaseInfo dataBase) : base(name, server, dataBase)
    {
    }
}

namespace DBison.Core.Entities;
public class DBisonStoredProcedure : DatabaseObjectBase
{
    public DBisonStoredProcedure(string name, ServerInfo server, DatabaseInfo dataBase) : base(name, server, dataBase)
    {
    }
}

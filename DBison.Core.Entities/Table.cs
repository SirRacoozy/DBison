namespace DBison.Core.Entities;
public class Table : DatabaseObjectBase
{
    public Table(string name, ServerInfo server, DatabaseInfo dataBase) : base(name, server, dataBase)
    {
    }
}

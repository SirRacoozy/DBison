namespace DBison.Core.Entities;
public class View : DatabaseObjectBase
{
    public View(string name, ServerInfo server, DatabaseInfo dataBase) : base(name, server, dataBase)
    {
    }
}

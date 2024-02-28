namespace DBison.Core.Entities;
public class Trigger : DatabaseObjectBase
{
    public Trigger(string name, ServerInfo server, DatabaseInfo dataBase) : base(name, server, dataBase)
    {
    }
}

using DBison.Core.Entities.Interfaces;

namespace DBison.Core.Entities;
public abstract class DatabaseObjectBase : IServerDataBaseContext
{
    public DatabaseObjectBase(string name, ServerInfo server, DatabaseInfo dataBase)
    {
        Name = name;
        Server = server;
        DataBase = dataBase;
    }

    #region [Name]
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    #endregion

    public bool IsMainNode { get; set; }
    public ServerInfo Server { get; private set; }
    public DatabaseInfo DataBase { get; private set; }
}

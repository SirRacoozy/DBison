using DBison.Core.Entities.Interfaces;

namespace DBison.Core.Entities;
public abstract class DatabaseObjectBase : IServerDataBaseContext
{
    public DatabaseObjectBase(string name, ServerInfo server, DatabaseInfo dataBase)
    {
        Name = name;
        Server = server;

        if (this is DatabaseInfo databaseInfo) //If we create the databaseinfo object we cant pass it to the constructor, its not instanciated at this moment, so cast it and set it here
            DataBase = databaseInfo;
        else
            DataBase = dataBase;
    }

    #region [Name]
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    #endregion

    public bool IsMainNode { get; set; }
    public bool IsFolder { get; set; }
    public bool IsPlaceHolder { get; set; }
    public bool IsRealDataBaseNode { get; set; }
    public ServerInfo Server { get; private set; }
    public DatabaseInfo DataBase { get; private set; }

    public DatabaseObjectBase Clone()
    {
        return this.MemberwiseClone() as DatabaseObjectBase;
    }
}

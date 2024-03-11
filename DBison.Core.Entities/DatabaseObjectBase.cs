using DBison.Core.Entities.Interfaces;

namespace DBison.Core.Entities;
public abstract class DatabaseObjectBase : IServerDataBaseContext
{
    #region [DatabaseObjectBase]
    public DatabaseObjectBase(string name, ServerInfo server, DatabaseInfo dataBase)
    {
        Name = name;
        Server = server;

        if (this is DatabaseInfo databaseInfo) //If we create the databaseinfo object we cant pass it to the constructor, its not instanciated at this moment, so cast it and set it here
            DataBase = databaseInfo;
        else
            DataBase = dataBase;
    }
    #endregion

    #region [Name]
    public string Name { get; set; } = string.Empty;
    #endregion

    #region [IsMainNode]
    public bool IsMainNode { get; set; }
    #endregion

    #region [IsFolder]
    public bool IsFolder { get; set; }
    #endregion

    #region [IsPlaceHolder]
    public bool IsPlaceHolder { get; set; }
    #endregion

    #region [IsRealDataBaseNode]
    public bool IsRealDataBaseNode { get; set; }
    #endregion

    #region [Server]
    public ServerInfo Server { get; private set; }
    #endregion

    #region [DataBase]
    public DatabaseInfo DataBase { get; private set; }
    #endregion

    #region - properties -
    public eDataBaseState DataBaseState { get; set; }

    #endregion

    #region [Clone]
    public DatabaseObjectBase Clone()
    {
        return this.MemberwiseClone() as DatabaseObjectBase;
    }
    #endregion
}

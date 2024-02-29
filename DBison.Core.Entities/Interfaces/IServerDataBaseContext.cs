namespace DBison.Core.Entities.Interfaces;
internal interface IServerDataBaseContext
{
    ServerInfo Server { get; }
    DatabaseInfo DataBase { get; }
}

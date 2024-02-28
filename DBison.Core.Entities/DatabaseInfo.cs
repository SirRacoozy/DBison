
namespace DBison.Core.Entities;
public class DatabaseInfo : DatabaseObjectBase
{
    public DatabaseInfo(string name, ServerInfo serverInfo) : base(name)
    {
        Server = serverInfo;
    }

    #region - properties -

    #region [Server]
    /// <summary>
    /// Gets or sets the server.
    /// </summary>
    public ServerInfo Server { get; set; } 
    #endregion

    #endregion

    #region - methods -

    #region - override -

    #region [ToString]
    /// <summary>
    /// Creates a string representation of the DatabaseInfo.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() => $"{nameof(Name)}: {Name}, {nameof(Server)}: {Server}, {nameof(Server.Username)}: {Server.Username}, {nameof(Server.UseIntegratedSecurity)}: {Server.UseIntegratedSecurity}";
    #endregion

    #region [GetHashCode]
    /// <summary>
    /// Gets the hash code of the object.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode() => HashCode.Combine(Name, Server.Name, Server.Username, Server.Password, Server.UseIntegratedSecurity);
    #endregion

    #region [Equals]
    /// <summary>
    /// Checks if the provided object is equal to this instance.
    /// </summary>
    /// <param name="obj">The object to check.</param>
    /// <returns>True if the objects are equal; otherwise false.</returns>
    public override bool Equals(object? obj) => obj is not null || obj is not DatabaseInfo ? false : GetHashCode() == (obj as DatabaseInfo)!.GetHashCode();
    #endregion

    #endregion

    #endregion
}

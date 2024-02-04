
namespace DBison.Core.Entities;
public class DatabaseInfo : DatabaseObjectBase
{
    public DatabaseInfo(string name) : base(name)
    {
    }

    #region - properties -

    #region [Server]
    /// <summary>
    /// Gets or sets the server.
    /// </summary>
    public string Server { get; set; } = string.Empty;
    #endregion

    #region [Username]
    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    public string Username { get; set; } = string.Empty;
    #endregion

    #region [Password]
    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string Password { get; set; } = string.Empty;
    #endregion

    #region [UseIntegratedSecurity]
    /// <summary>
    /// Gets or sets a flag representing the usage of integrated security.
    /// </summary>
    public bool UseIntegratedSecurity { get; set; } = false;
    #endregion

    #endregion

    #region - methods -

    #region - override -

    #region [ToString]
    /// <summary>
    /// Creates a string representation of the DatabaseInfo.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() => $"{nameof(Name)}: {Name}, {nameof(Server)}: {Server}, {nameof(Username)}: {Username}, {nameof(UseIntegratedSecurity)}: {UseIntegratedSecurity}";
    #endregion

    #region [GetHashCode]
    /// <summary>
    /// Gets the hash code of the object.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode() => HashCode.Combine(Name, Server, Username, Password, UseIntegratedSecurity);
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

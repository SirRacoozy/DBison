namespace DBison.Core.Entities;
public class DatabaseInfo
{
    #region - properties -

    #region [Name]
    public string Name { get; set; } = string.Empty;
    #endregion

    #region [Server]
    public string Server {  get; set; } = string.Empty;
    #endregion

    #region [Username]
    public string Username { get; set; } = string.Empty;
    #endregion

    #region [Password]
    public string Password { internal get; set; } = string.Empty;
    #endregion

    #region [UseIntegratedSecurity]
    public bool UseIntegratedSecurity { get; set; } = false;
    #endregion

    #endregion

    #region - methods -

    #region - override -

    #region [ToString]
    public override string ToString() => $"{nameof(Name)}: {Name}, {nameof(Server)}: {Server}, {nameof(Username)}: {Username}, {nameof(UseIntegratedSecurity)}: {UseIntegratedSecurity}";
    #endregion

    #region [GetHashCode]
    public override int GetHashCode() => HashCode.Combine(Name, Server, Username, UseIntegratedSecurity);
    #endregion

    #region [Equals]
    public override bool Equals(object? obj) => obj is not null || obj is not DatabaseInfo ? false : GetHashCode() == (obj as DatabaseInfo)!.GetHashCode();
    #endregion

    #endregion

    #endregion
}

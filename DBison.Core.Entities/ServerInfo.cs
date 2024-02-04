using System.Net;

namespace DBison.Core.Entities;
public class ServerInfo
{
    public ServerInfo(string serverName)
    {
        ServerName = serverName;
    }

    #region [ServerName]
    /// <summary>
    /// Gets or sets the ServerName.
    /// We can connect to an SQL Server with IP or Hostname
    /// </summary>
    public string ServerName { get; private set; }
    #endregion

    #region [ServerAddress]
    /// <summary>
    /// Gets or sets the ServerAddress
    /// We can connect to an SQL Server with IP or Hostname
    /// </summary>
    public IPAddress ServerAddress { get; set; } = IPAddress.None;
    #endregion

    #region [DatabaseInfos]
    /// <summary>
    /// Gets or sets the DatabaseInfos
    /// </summary>
    public List<DatabaseInfo> DatabaseInfos { get; private set; } = [];
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

}

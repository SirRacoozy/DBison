using System.Net;

namespace DBison.Core.Entities;
public class ServerInfo
{
    #region [ServerName]
    /// <summary>
    /// Gets or sets the ServerName.
    /// We can connect to an SQL Server with IP or Hostname
    /// </summary>
    public string ServerName { get; set; } = string.Empty;
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

}

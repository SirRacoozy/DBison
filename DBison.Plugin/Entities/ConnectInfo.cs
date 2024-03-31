namespace DBison.Plugin.Entities;

public record ConnectInfo
{
    public string ServerName { get; init; } = string.Empty;
    public string DatabaseName { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public bool UseIntegratedSecurity { get; init; }

}

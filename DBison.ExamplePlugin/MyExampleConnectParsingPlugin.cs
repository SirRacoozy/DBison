using DBison.Plugin;
using DBison.Plugin.Entities;

namespace DBison.ExamplePlugin;
public class MyExampleConnectParsingPlugin : IConnectParsingPlugin
{
    public string Name => "Example";

    public string Description => "Example";

    public ConnectInfo? ParseConnectInput(string connectInput) => new() { ServerName = "localhost", DatabaseName = "MyTestDatabase", UseIntegratedSecurity = true };
}

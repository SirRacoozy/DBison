using DBison.Core.Entities;
using DBison.Plugin;
using DBison.Plugin.Entities;

namespace DBison.ExamplePlugin;

public class MyExampleSearchParsingPlugin : ISearchParsingPlugin
{
    public string Name => "Example";

    public string Description => "Example";

    public ConnectInfo? ParseSearchInput(string searchInput) => new() { ServerName = "localhost", UseIntegratedSecurity = true };
}

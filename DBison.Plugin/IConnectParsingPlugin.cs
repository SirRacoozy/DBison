using DBison.Plugin.Entities;

namespace DBison.Plugin;
public interface IConnectParsingPlugin : IPlugin
{
    ConnectInfo? ParseConnectInput(string connectInput);
}

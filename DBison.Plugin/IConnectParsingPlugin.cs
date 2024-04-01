using DBison.Plugin.Entities;

namespace DBison.Plugin;
public interface IConnectParsingPlugin : IPlugin
{
    /// <summary>
    /// Parses the input from the connect dialog and gives the ConnectInfo back. If the plugin can't parse the input, you need to return null.
    /// </summary>
    /// <param name="connectInput">The input from the connect dialog.</param>
    /// <returns>The ConnectInfo or null if the plugin couldn't parse the input.</returns>
    ConnectInfo? ParseConnectInput(string connectInput);
}

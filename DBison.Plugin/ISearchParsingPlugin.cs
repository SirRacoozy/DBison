using DBison.Plugin.Entities;

namespace DBison.Plugin;

public interface ISearchParsingPlugin : IPlugin
{
    /// <summary>
    /// Parsing the search input.
    /// </summary>
    /// <param name="searchInput">The search input.</param>
    /// <returns>The ConnectInfo or null if the plugin couldn't parse the input..</returns>
    ConnectInfo? ParseSearchInput(string searchInput);
}

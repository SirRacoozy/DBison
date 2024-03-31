using DBison.Plugin.Entities;

namespace DBison.Plugin;

public interface ISearchParsingPlugin : IPlugin
{
    /// <summary>
    /// Parsing the search input.
    /// </summary>
    /// <param name="searchInput">The search input.</param>
    /// <returns>Return the database info to which a connection should be opened.</returns>
    ConnectInfo? ParseSearchInput(string searchInput);
}

using DBison.Core.Entities;
using DBison.Plugin;

namespace DBison.ExamplePlugin;

public class MyExamplePlugin : ISearchParsingPlugin
{
    public string Name => throw new NotImplementedException();

    public string Description => throw new NotImplementedException();

    public DatabaseInfo? ParseSearchInput(string searchInput) => new() { Name = searchInput };
}

using DBison.Plugin;

namespace DBison.Core.PluginSystem;
public class PluginLoader
{
    #region - ctor -
    public PluginLoader(string path)
    {
        ArgumentNullException.ThrowIfNull(nameof(path));
        if (!Directory.Exists(path))
            throw new ArgumentException($"'{nameof(path)}' is not a directory.");


    }
    #endregion

    #region - properties -

    #region [ContextMenuPlugins]
    public IEnumerable<IContextMenuPlugin> ContextMenuPlugins { get; set; } = new List<IContextMenuPlugin>();
    #endregion

    #region [SearchParsingPlugins]
    public IEnumerable<ISearchParsingPlugin> SearchParsingPlugins { get; set; } = new List<ISearchParsingPlugin>();
    #endregion

    #endregion

    #region - methods -

    #region [__Load]
    private void __Load(/*string path*/)
    {

    }
    #endregion

    #endregion
}

using DBison.Plugin;
using System.Reflection;

namespace DBison.Core.PluginSystem;
public class PluginLoader
{
    #region - ctor -
    public PluginLoader(string path)
    {
        ArgumentNullException.ThrowIfNull(nameof(path));
        if (!File.Exists(path))
            throw new ArgumentException($"'{nameof(path)}' is not a file.");

        __Load(path);
    }
    #endregion

    #region - properties -

    #region [ContextMenuPlugins]
    public List<IContextMenuPlugin> ContextMenuPlugins { get; set; } = new List<IContextMenuPlugin>();
    #endregion

    #region [SearchParsingPlugins]
    public List<ISearchParsingPlugin> SearchParsingPlugins { get; set; } = new List<ISearchParsingPlugin>();
    #endregion

    #endregion

    #region - methods -

    #region [__Load]
    private void __Load(string path)
    {
        var assembly = Assembly.LoadFrom(path);
        var type = assembly.GetType("DBison.ExamplePlugin.MyExamplePlugin");
        var plugin = Activator.CreateInstance(type) as ISearchParsingPlugin;
        SearchParsingPlugins.Add(plugin);
    }
    #endregion

    #endregion
}

using DBison.Plugin;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;

namespace DBison.Core.PluginSystem;
public class PluginLoader
{
    #region - ctor -
    public PluginLoader(string path)
    {
        ArgumentNullException.ThrowIfNull(nameof(path));
        if (!Directory.Exists(path))
            throw new ArgumentException($"'{nameof(path)}' is not a directory.");

        var dlls = __GetDllFileNames(path);

        __LoadPlugins(dlls);
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


    #region [__LoadPlugins]

    #endregion

    #region [__LoadPlugins]
    private void __LoadPlugins(List<string> files)
    {
        var searchBag = new ConcurrentBag<ISearchParsingPlugin>();
        var commandBag = new ConcurrentBag<IContextMenuPlugin>();

        _ = Parallel.ForEach(files, (file) =>
        {
            try
            {
                __LoadPluginsFromAssembly(ref searchBag, ref commandBag, file);
            }
            catch (Exception ex) 
            {
            }
        });

        ContextMenuPlugins.AddRange(commandBag.ToList());
        SearchParsingPlugins.AddRange(searchBag.ToList());
    }

    #endregion
    #region [__LoadPluginsFromAssembly]
    private void __LoadPluginsFromAssembly(ref ConcurrentBag<ISearchParsingPlugin> searchBag, ref ConcurrentBag<IContextMenuPlugin> commandBag, string file)
    {
        var assembly = Assembly.LoadFrom(file);
        var types = assembly.GetTypes();

        foreach(var type in types)
        {
            try
            {
                var instance = Activator.CreateInstance(type);

                if (instance is ISearchParsingPlugin parsingPlugin)
                    searchBag.Add(parsingPlugin);
                else if (instance is IContextMenuPlugin menuPlugin)
                    commandBag.Add(menuPlugin);
            }
            catch (Exception) { }
        }
    }
    #endregion

    #region [__GetDllFileNames]
    private List<string> __GetDllFileNames(string path)
    {
        var list = new List<string>();

        var files = Directory.GetFiles(path);

        list.AddRange(files.Where(x => x.EndsWith(".dll")));

        _ = list.RemoveAll(x => x.Equals("DBison.Plugin.dll"));

        return list;
    }
    #endregion

    #endregion
}

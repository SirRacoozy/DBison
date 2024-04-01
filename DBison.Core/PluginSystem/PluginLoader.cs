using DBison.Core.Extender;
using DBison.Core.Utils.SettingsSystem;
using DBison.Plugin;
using System.Collections.Concurrent;
using System.Data;
using System.Reflection;

namespace DBison.Core.PluginSystem;
public class PluginLoader
{
    private readonly string m_Path = Settings.PluginPath;
    private static PluginLoader m_Instance;

    #region - ctor -
    private PluginLoader()
    {
        if (m_Path.IsNullOrEmpty())
            return;
        if (!Directory.Exists(m_Path))
            throw new ArgumentException($"'{nameof(m_Path)}' is not a directory.");

        var dlls = __GetDllFileNames();

        __LoadPlugins(dlls);
    }
    #endregion

    #region - properties -

    #region [Instance]
    public static PluginLoader Instance => m_Instance ??= new PluginLoader();
    #endregion

    #region [ContextMenuPlugins]
    public List<IContextMenuPlugin> ContextMenuPlugins { get; set; } = new();
    #endregion

    #region [SearchParsingPlugins]
    public List<ISearchParsingPlugin> SearchParsingPlugins { get; set; } = new();
    #endregion

    #region [ConnectParsingPlugin]
    public List<IConnectParsingPlugin> ConnectParsingPlugins { get; set; } = new();
    #endregion

    #endregion

    #region - methods -

    #region [__LoadPlugins]
    private void __LoadPlugins(List<string> files)
    {
        var searchBag = new ConcurrentBag<ISearchParsingPlugin>();
        var commandBag = new ConcurrentBag<IContextMenuPlugin>();
        var connectBag = new ConcurrentBag<IConnectParsingPlugin>();

        _ = Parallel.ForEach(files, (file) =>
        {
            try
            {
                __LoadPluginsFromAssembly(ref searchBag, ref commandBag, ref connectBag, file);
            }
            catch (Exception ex)
            {
            }
        });

        ContextMenuPlugins.AddRange(commandBag.ToList());
        SearchParsingPlugins.AddRange(searchBag.ToList());
        ConnectParsingPlugins.AddRange(connectBag.ToList());
    }

    #endregion
    #region [__LoadPluginsFromAssembly]
    private void __LoadPluginsFromAssembly(ref ConcurrentBag<ISearchParsingPlugin> searchBag, ref ConcurrentBag<IContextMenuPlugin> commandBag, ref ConcurrentBag<IConnectParsingPlugin> connectBag, string file)
    {
        var assembly = Assembly.LoadFrom(file);
        var types = assembly.GetTypes();

        foreach (var type in types)
        {
            try
            {
                if (type.IsInterface || !typeof(IPlugin).IsAssignableFrom(type))
                {
                    continue;
                }

                var instance = Activator.CreateInstance(type);

                switch (instance)
                {
                    case ISearchParsingPlugin searchPlugin:
                        searchBag.Add(searchPlugin);
                        break;
                    case IConnectParsingPlugin connectPlugin:
                        connectBag.Add(connectPlugin);
                        break;
                    case IContextMenuPlugin contextPlugin:
                        commandBag.Add(contextPlugin);
                        break;
                }
            }
            catch (Exception) { }
        }
    }
    #endregion

    #region [__GetDllFileNames]
    private List<string> __GetDllFileNames()
    {
        var list = new List<string>();

        var files = Directory.GetFiles(m_Path);

        list.AddRange(files.Where(x => x.EndsWith(".dll")));

        _ = list.RemoveAll(x => x.Equals("DBison.Plugin.dll"));

        return list;
    }
    #endregion

    #endregion
}

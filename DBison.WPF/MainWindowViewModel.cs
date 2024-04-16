using ControlzEx.Theming;
using DBison.Core.Attributes;
using DBison.Core.Entities;
using DBison.Core.Extender;
using DBison.Core.PluginSystem;
using DBison.Core.Utils.SettingsSystem;
using DBison.Plugin.Entities;
using DBison.WPF.ClientBaseClasses;
using DBison.WPF.Dialogs;
using DBison.WPF.ViewModels;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DBison.WPF;
public class MainWindowViewModel : ClientViewModelBase
{
    #region - needs -
    bool m_HasAddServerError = false;
    bool m_WasAutoConnectError = true;
    DispatcherTimer m_ExecutionTimer;
    #endregion

    #region [Ctor]
    public MainWindowViewModel()
    {
        ServerTreeItems = new ObservableCollection<ServerObjectTreeItemViewModel>();
        __PrepareTimer();
        m_WasAutoConnectError = Settings.AutoConnectEnabled;
        TabItems = new ObservableCollection<TabItemViewModelBase>();
        __InitServers();
        __ConnectToDefaultServer();
        __HandleSettingsChanged();
    }
    #endregion

    #region - public properties -

    #region [ServerItems]
    public ObservableCollection<ServerViewModel> ServerItems
    {
        get => Get<ObservableCollection<ServerViewModel>>();
        set => Set(value);
    }
    #endregion

    #region [ServerTreeItems]
    public ObservableCollection<ServerObjectTreeItemViewModel> ServerTreeItems
    {
        get => Get<ObservableCollection<ServerObjectTreeItemViewModel>>();
        set => Set(value);
    }
    #endregion

    #region [TabItems]
    public ObservableCollection<TabItemViewModelBase> TabItems
    {
        get => Get<ObservableCollection<TabItemViewModelBase>>();
        set => Set(value);
    }
    #endregion

    #region [SelectedTabItem]
    public TabItemViewModelBase SelectedTabItem
    {
        get => Get<TabItemViewModelBase>();
        set => Set(value);
    }
    #endregion

    #region [SelectedServer]
    public ServerViewModel SelectedServer
    {
        get => Get<ServerViewModel>();
        set => Set(value);
    }
    #endregion

    #region [LastSelectedDatabase]
    public DatabaseInfo LastSelectedDatabase
    {
        get => Get<DatabaseInfo>();
        set
        {
            Set(value);
            __CheckStateIfNeeded();
        }
    }
    #endregion

    #region [LastSelectedTreeItem]
    public ServerObjectTreeItemViewModel LastSelectedTreeItem
    {
        get => Get<ServerObjectTreeItemViewModel>();
        set => Set(value);
    }
    #endregion

    #region [FilterText]
    public string FilterText
    {
        get => Get<string>();
        set => Set(value);
    }
    #endregion

    #region [ServerItemsTreeView]
    public TreeView ServerItemsTreeView
    {
        get => Get<TreeView>();
        set => Set(value);
    }
    #endregion

    #endregion

    #region - commands -

    #region [Execute_QuitApplication]
    public void Execute_QuitApplication()
    {
        Environment.Exit(0);
    }
    #endregion

    #region [Execute_RestartApplication]
    public void Execute_RestartApplication()
    {
        string assemblyPath = Path.ChangeExtension(Assembly.GetEntryAssembly().Location, "exe");
        Process.Start(assemblyPath);
        Application.Current.Shutdown();
    }
    #endregion

    #region [Execute_ConnectParseConnect]
    public void Execute_ConnectParseConnect()
    {
        var addServerVm = new AddServerDialogViewModel(null);
        addServerVm.ServerName = Clipboard.GetText();
        addServerVm.OkClicked += (x, connectInfo) => __AddServer(connectInfo);
        addServerVm.Execute_Ok();
    }
    #endregion

    #region [Execute_ResetUIScaling]
    public void Execute_ResetUIScaling()
    {
        Settings.UIScaling = 1;
        Settings.FontSize = 20;
    }
    #endregion

    #region [Execute_IncreaseScaling]
    public void Execute_IncreaseScaling()
    {
        Settings.UIScaling += 0.1;
    }
    #endregion

    #region [Execute_DecreaseScaling]
    public void Execute_DecreaseScaling()
    {
        Settings.UIScaling -= 0.1;
    }
    #endregion

    #region [Execute_AddServer]
    public void Execute_AddServer()
    {
        var dialog = new AddServerDialog();
        dialog.ServerConnectRequested += (sender, e) => __AddServer(e);
        dialog.ShowDialog();
    }
    #endregion

    #region [Execute_ToggleSettings]
    public void Execute_ToggleSettings()
    {
        __ToggleSettingsPageIfNeeded();
    }
    #endregion

    #region [Execute_NewQuery]
    public void Execute_NewQuery()
    {
        __AddQueryPageIfPossible(string.Empty);
    }
    #endregion

    #region [Execute_OpenQueryFromFile]
    public void Execute_OpenQueryFromFile()
    {
        var dlg = new OpenFileDialog()
        {
            Filter = "sql files (*.sql)|*.sql|All files (*.*)|*.*",
        };
        dlg.ShowDialog();
        if (dlg.FileName.IsNotNullOrEmpty())
        {
            try
            {
                __AddQueryPageIfPossible(File.ReadAllText(dlg.FileName));
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex);
            }
        }
    }
    #endregion

    #endregion

    #region - public methods

    #region [RemoveServer]
    public void RemoveServer(ServerViewModel server)
    {
        if (server != null && ServerItems.Contains(server))
        {
            ServerTreeItems.Remove(server.ServerNode);
            ServerItems.Remove(server);
            server.Dispose();
            OnPropertyChanged(nameof(ServerItems));
            SelectedServer = ServerItems.FirstOrDefault();
        }
        if (SelectedServer == null)
        {
            TabItems = new(TabItems.Where(q => q is SettingsTabViewModel));
            OnPropertyChanged(nameof(TabItems));
        }
        OnPropertyChanged(nameof(ServerTreeItems));
    }
    #endregion

    #region [QueryPagesChanged]
    [DependsUpon(nameof(SelectedServer))]
    public void QueryPagesChanged()
    {
        if (SelectedServer == null)
            return;
        var settingPage = TabItems.FirstOrDefault(q => q is SettingsTabViewModel);
        TabItems = new(SelectedServer.ServerQueryPages);
        if (settingPage != null)
            TabItems.Add(settingPage);
        OnPropertyChanged(nameof(TabItems));
        SelectedTabItem = TabItems.LastOrDefault(x => x is ServerQueryPageViewModel) ?? settingPage;
    }
    #endregion

    #region [SetSelectedServerIfNeeded]
    internal void SetSelectedServerIfNeeded(object selectedItem)
    {
        if (selectedItem is ServerViewModel serverViewModel)
        {
            SelectedServer = serverViewModel;
            LastSelectedDatabase = serverViewModel.DatabaseObject.DatabaseInfos.FirstOrDefault();
        }
        else if (selectedItem is ServerObjectTreeItemViewModel treeItemObject)
        {
            if (ServerItems.IsEmpty())
                return;
            LastSelectedTreeItem = treeItemObject;
            var serverVm = ServerItems.FirstOrDefault(x => x.DatabaseObject == treeItemObject.DatabaseObject.Server);
            if (serverVm != null && SelectedServer != serverVm)
                SelectedServer = serverVm;
            if (LastSelectedDatabase != treeItemObject.DatabaseObject.DataBase)
                LastSelectedDatabase = treeItemObject.DatabaseObject.DataBase;
        }
    }
    #endregion

    #region [CloseSettings]
    internal void CloseSettings()
    {
        TabItems = new(TabItems.Where(x => x is not SettingsTabViewModel));
        SelectedTabItem = TabItems.LastOrDefault();
    }
    #endregion

    #region [RefreshLastSelectedDataBaseState]
    public void RefreshLastSelectedDataBaseState()
    {
        __CheckStateIfNeeded();
    }
    #endregion

    #region [RemoveAllServer]
    public void RemoveAllServer()
    {
        foreach (var serverItem in ServerItems)
        {
            serverItem.Dispose();
        }
        foreach (var serverTreeItem in ServerTreeItems)
        {
            serverTreeItem.Dispose();
        }
        ServerItems.Clear();
        ServerTreeItems.Clear();
        OnPropertyChanged(nameof(ServerTreeItems));
        OnPropertyChanged(nameof(ServerItems));
        SelectedServer = null;
        TabItems.Clear();
        OnPropertyChanged(nameof(TabItems));
    }
    #endregion

    #endregion

    #region - private methods -

    #region [__ConnectToDefaultServer]
    private void __ConnectToDefaultServer()
    {
        new TaskFactory().StartNew(() =>
        {
            if (Settings.AutoConnectEnabled)
            {
                ExecuteOnDispatcher(() =>
                {
                    __AddServer(new ConnectInfo()
                    {
                        ServerName = Settings.AutoConnectServerName,
                        Username = Settings.AutoConnectIGS ? Environment.UserName : Settings.AutoConnectUsername,
                        Password = Settings.AutoConnectPassword,
                        UseIntegratedSecurity = Settings.AutoConnectIGS,
                    });
                });
            }
            else
            {
                ExecuteOnDispatcherWithDelay(Execute_AddServer, TimeSpan.FromSeconds(1));
            }
        });
    }
    #endregion

    #region [__AddQueryPageIfPossible]
    private void __AddQueryPageIfPossible(string queryText)
    {
        if (SelectedServer != null && SelectedServer.DatabaseObject is ServerInfo serverInfo)
        {
            var dataBase = LastSelectedDatabase ?? serverInfo.DatabaseInfos.FirstOrDefault();
            if (dataBase != null)
                SelectedServer.AddNewQueryPage(dataBase, queryText);
        }
    }
    #endregion

    #region [__ToggleSettingsPageIfNeeded]
    private void __ToggleSettingsPageIfNeeded()
    {
        if (!TabItems.Any(q => q is SettingsTabViewModel))
            TabItems.Add(new SettingsTabViewModel(this) { Header = "Settings" });
        else
            CloseSettings();
        SelectedTabItem = TabItems.LastOrDefault(s => s is SettingsTabViewModel);
    }
    #endregion

    #region [__InitServers]
    private void __InitServers()
    {
        ServerItems = new ObservableCollection<ServerViewModel>();
        SelectedServer = ServerItems.FirstOrDefault();
    }
    #endregion

    #region [__AddServer]
    private void __AddServer(ConnectInfo server)
    {
        if (ServerItems == null)
            ServerItems = new ObservableCollection<ServerViewModel>();

        var alreadyExistingServer = ServerItems.FirstOrDefault(s => s.DatabaseObject is ServerInfo serverInfo && serverInfo.Name.IsEquals(server.ServerName));
        if (alreadyExistingServer != null)
        {
            if (ServerItemsTreeView.ItemContainerGenerator.ContainerFromItem(alreadyExistingServer.ServerNode) is TreeViewItem tvi)
                tvi.IsSelected = true;
            SelectedServer = alreadyExistingServer;
            return;
        }

        if (!__PingServer(server.ServerName))
        {
            ShowMessageAsync($"{server.ServerName} not available", $"Server {server.ServerName} is not available or has no MSSQL Instance");
            return;
        }
        var serverInfo = new ServerInfo(server.ServerName) { UseIntegratedSecurity = server.UseIntegratedSecurity, Username = server.Username, Password = server.Password };

        var filterText = server.DatabaseName.IsNotNullOrEmpty() ? $"d:{server.DatabaseName}" : string.Empty;

        var newServerViewModel = new ServerViewModel(serverInfo, __NewServerViewModel_ErrorOccured, this, filterText);
        if (m_HasAddServerError)
        {
            newServerViewModel.Dispose();
            m_HasAddServerError = false;
            return;
        }
        if (Settings.AutoConnectEnabled)
            m_WasAutoConnectError = false;
        ServerItems.Add(newServerViewModel);
        ServerTreeItems.Add(newServerViewModel.ServerNode);
        OnPropertyChanged(nameof(ServerTreeItems));
        SelectedServer = newServerViewModel;
        if (Settings.OpenQueryOnServerAdded)
            __AddQueryPageIfPossible(string.Empty);
    }
    #endregion

    #region [__NewServerViewModel_ErrorOccured]
    private void __NewServerViewModel_ErrorOccured(object? sender, Exception e)
    {
        m_HasAddServerError = true;
        if (m_WasAutoConnectError)
        {
            m_WasAutoConnectError = false;
            ExecuteOnDispatcherWithDelay(Execute_AddServer, TimeSpan.FromSeconds(1));
        }
        else
        {
            ShowExceptionMessage(e);
        }
    }
    #endregion

    #region [__PrepareTimer]
    private void __PrepareTimer()
    {
        ExecuteOnDispatcher(() =>
        {
            if (m_ExecutionTimer == null)
            {
                m_ExecutionTimer = new DispatcherTimer();
                m_ExecutionTimer.Interval = TimeSpan.FromSeconds(Settings.FilterUpdateRate);
                m_ExecutionTimer.Tick += __ExecutionTimer_Tick;
            }
            m_ExecutionTimer?.Stop();
            m_ExecutionTimer?.Start();
        });
    }
    #endregion

    #region [__ExecutionTimer_Tick]
    private void __ExecutionTimer_Tick(object? sender, EventArgs e)
    {
        __Filter();
    }
    #endregion

    #region [__Filter]
    private void __Filter()
    {
        if (ServerItems.IsEmpty())
            return;
        var textToFilter = FilterText != null ? FilterText.Trim() : string.Empty;
        var factory = new TaskFactory();

        var minFilterChars = Settings.MinFilterChar;

        if (!textToFilter.StartsWith("d:") && textToFilter.Length < minFilterChars)
            textToFilter = string.Empty;

        factory.StartNew(() =>
        {
            foreach (var serverItem in ServerItems)
            {
                serverItem.Filter(textToFilter);
            }
        });
    }
    #endregion

    #region [__PingServer]
    private bool __PingServer(string serverName)
    {
        if (serverName.IsNullOrEmpty())
            return false;
        var ping = new Ping();
        try
        {
            var pingResult = ping.Send(serverName);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    #endregion

    #region [__CheckStateIfNeeded]
    private void __CheckStateIfNeeded()
    {
        if (LastSelectedDatabase == null || !LastSelectedDatabase.IsRealDataBaseNode)
            return;

        var getState = SelectedServer.GetDataBaseState(LastSelectedDatabase);
        if (LastSelectedDatabase.DataBaseState != getState)
        {
            LastSelectedDatabase.DataBaseState = getState;
            LastSelectedTreeItem.RefreshState();
        }
    }
    #endregion

    #region [__HandleSettingsChanged]
    private void __HandleSettingsChanged()
    {
        Application.Current.Resources["GlobalFontSize"] = Convert.ToDouble(Settings.FontSize);
        Application.Current.Resources["GlobalScaleFactor"] = Settings.UIScaling;
        SettingsHandler.SettingChanged += __SettingsHandler_SettingChanged;
    }
    #endregion

    #region [__SettingsHandler_SettingChanged]
    private void __SettingsHandler_SettingChanged(object? sender, Core.EventArguments.SettingChangedEventArgs e)
    {
        switch (e.ChangedSettingName)
        {
            case nameof(Settings.UseDarkMode):
                var baseTheme = Settings.UseDarkMode ? "Dark" : "Light";
                _ = ThemeManager.Current.ChangeTheme(Application.Current, $"{baseTheme}.Purple");
                break;
            case nameof(Settings.FilterUpdateRate):
                m_ExecutionTimer.Interval = TimeSpan.FromSeconds(Settings.FilterUpdateRate);
                break;
            case nameof(Settings.PluginPath):
                //If the PluginPath changed, we need to refresh at runtime to execute the plugins in the new directory or no plugins
                PluginLoader.ClearPluginLoader();
                break;
            case nameof(Settings.FontSize):
                Application.Current.Resources["GlobalFontSize"] = Convert.ToDouble(Settings.FontSize);
                break;
            case nameof(Settings.UIScaling):
                Application.Current.Resources["GlobalScaleFactor"] = Settings.UIScaling;
                break;
        }
        var settingsTab = TabItems.LastOrDefault(s => s is SettingsTabViewModel);
        if (settingsTab is SettingsTabViewModel settingsTabVm)
        {
            settingsTabVm.RefreshSettings();
        }
    }
    #endregion

    #endregion

}

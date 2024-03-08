using DBison.Core.Attributes;
using DBison.Core.Entities;
using DBison.Core.Extender;
using DBison.Core.Utils.SettingsSystem;
using DBison.WPF.ClientBaseClasses;
using DBison.WPF.Dialogs;
using DBison.WPF.ViewModels;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Threading;

namespace DBison.WPF;
public class MainWindowViewModel : ClientViewModelBase
{
    bool m_HasAddServerError = false;
    DispatcherTimer m_ExecutionTimer;
    public MainWindowViewModel()
    {
        __PrepareTimer();
        TabItems = new ObservableCollection<TabItemViewModelBase>();
        __InitServers();
        ExecuteOnDispatcherWithDelay(Execute_AddServer, TimeSpan.FromSeconds(1));
    }

    #region - public properties -

    #region [ServerItems]
    public ObservableCollection<ServerViewModel> ServerItems
    {
        get => Get<ObservableCollection<ServerViewModel>>();
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

    #endregion

    #region - commands -

    #region [Execute_QuitApplication]
    public void Execute_QuitApplication()
    {
        Environment.Exit(0);
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
    }
    #endregion

    #region [QueryPagesChanged]
    [DependsUpon(nameof(SelectedServer))]
    public void QueryPagesChanged()
    {
        if (SelectedServer == null)
            return;
        var settingPage = TabItems.FirstOrDefault(q => q is SettingsTabViewModel);
        TabItems = new(SelectedServer?.ServerQueryPages);
        if (settingPage != null)
            TabItems.Add(settingPage);
        OnPropertyChanged(nameof(TabItems));
        var lastQueryPage = TabItems.LastOrDefault(x => x is ServerQueryPageViewModel);
        if (lastQueryPage != null)
            SelectedTabItem = (ServerQueryPageViewModel)lastQueryPage;
        else
            SelectedTabItem = null;
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
    }
    #endregion

    #endregion

    #region - private methods -

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
    private void __AddServer(ServerInfo server)
    {
        if (ServerItems == null)
            ServerItems = new ObservableCollection<ServerViewModel>();
        var newServerViewModel = new ServerViewModel(server, __NewServerViewModel_ErrorOccured, this);
        if (m_HasAddServerError)
        {
            newServerViewModel.Dispose();
            m_HasAddServerError = false;
            return;
        }
        ServerItems.Add(newServerViewModel);
        SelectedServer = newServerViewModel;
    }
    #endregion

    #region [__NewServerViewModel_ErrorOccured]
    private void __NewServerViewModel_ErrorOccured(object? sender, Exception e)
    {
        m_HasAddServerError = true;
        ShowExceptionMessage(e);
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
        var textToFilter = FilterText != null ? FilterText.Trim() : string.Empty;
        var factory = new TaskFactory();

        var minFilterChars = Settings.MinFilterChar;

        if (textToFilter.Length < minFilterChars)
            textToFilter = string.Empty;

        factory.StartNew(() =>
        {
            Parallel.ForEachAsync(ServerItems, async (serverItem, ct) =>
            {
                serverItem.Filter(textToFilter);
            });
        });
    }
    #endregion

    #endregion

}

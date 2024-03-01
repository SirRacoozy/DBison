using DBison.Core.Attributes;
using DBison.Core.Entities;
using DBison.Core.Extender;
using DBison.WPF.ClientBaseClasses;
using DBison.WPF.Dialogs;
using DBison.WPF.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace DBison.WPF;
public class MainWindowViewModel : ClientViewModelBase
{
    bool m_Error = false;
    public MainWindowViewModel()
    {
        QueryPages = new ObservableCollection<TabItemViewModelBase>();
        __GetSettings();
        __InitServers();
        __ExecuteOnDispatcherWithDelay(Execute_AddServer, TimeSpan.FromSeconds(1));
    }

    #region [ServerItems]
    public ObservableCollection<ServerViewModel> ServerItems
    {
        get => Get<ObservableCollection<ServerViewModel>>();
        set => Set(value);
    }
    #endregion

    public ObservableCollection<TabItemViewModelBase> QueryPages
    {
        get => Get<ObservableCollection<TabItemViewModelBase>>();
        set => Set(value);
    }

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
        set
        {
            Set(value);
        }
    }
    #endregion

    #region [SettingsVm]
    public SettingsViewModel SettingsVm
    {
        get => Get<SettingsViewModel>();
        set => Set(value);
    }
    #endregion

    public void Execute_QuitApplication()
    {
        Environment.Exit(0);
    }

    public void Execute_AddServer()
    {
        var dialog = new AddServerDialog();
        dialog.ServerConnectRequested += (sender, e) => __AddServer(e);
        dialog.ShowDialog();
    }

    public void Execute_OpenSettings()
    {
        __AddSettingsPageIfNeeded();
    }

    public void Execute_NewQuery()
    {
        __AddQueryPageIfPossible(string.Empty);
    }

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
            catch (Exception)
            {
                //Ignore for first time
            }
        }
    }

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
            QueryPages = new(QueryPages.Where(q => q is SettingsTabViewModel));
            OnPropertyChanged(nameof(QueryPages));
        }
    }

    [DependsUpon(nameof(SelectedServer))]
    public void QueryPagesChanged()
    {
        if (SelectedServer == null)
            return;
        var settingPage = QueryPages.FirstOrDefault(q => q is SettingsTabViewModel);
        QueryPages = new(SelectedServer?.ServerQueryPages);
        if (settingPage != null)
            QueryPages.Add(settingPage);
        OnPropertyChanged(nameof(QueryPages));
        var lastQueryPage = QueryPages.LastOrDefault(x => x is ServerQueryPageViewModel);
        if (lastQueryPage != null)
            SelectedTabItem = (ServerQueryPageViewModel)lastQueryPage;
        else
            SelectedTabItem = null; //TODO: Settingstab?
    }

    private void __AddQueryPageIfPossible(string queryText)
    {
        if (SelectedServer != null && SelectedServer.DatabaseObject is ServerInfo serverInfo)
        {
            var dataBase = LastSelectedDatabase ?? serverInfo.DatabaseInfos.FirstOrDefault();
            if (dataBase != null)
                SelectedServer.AddNewQueryPage(dataBase, queryText);
        }
    }

    private void __AddSettingsPageIfNeeded()
    {
        if (!QueryPages.Any(q => q is SettingsTabViewModel))
            QueryPages.Add(new SettingsTabViewModel(this) { SettingsViewModel = SettingsVm, Header = "Settings" });
        SelectedTabItem = QueryPages.LastOrDefault(s => s is SettingsTabViewModel);
    }

    private void __InitServers()
    {
        ServerItems = new ObservableCollection<ServerViewModel>();
        SelectedServer = ServerItems.FirstOrDefault();
    }

    private void __AddServer(ServerInfo server)
    {
        if (ServerItems == null)
            ServerItems = new ObservableCollection<ServerViewModel>();
        var newServerViewModel = new ServerViewModel(server, __NewServerViewModel_ErrorOccured, this);
        if (m_Error)
        {
            newServerViewModel.Dispose();
            m_Error = false;
            return;
        }
        ServerItems.Add(newServerViewModel);
        SelectedServer = newServerViewModel;
    }

    private void __NewServerViewModel_ErrorOccured(object? sender, Exception e)
    {
        m_Error = true;
        if (Application.Current.MainWindow is MetroWindow metroWnd)
        {
            metroWnd.ShowMessageAsync("Exception occured", e.Message);
        }
    }

    private void __ExecuteOnDispatcherWithDelay(Action action, TimeSpan delay)
    {
        new Task(() =>
        {
            Thread.Sleep(delay);
            Application.Current.Dispatcher.Invoke(action);
        }).Start();
    }

    private void __GetSettings()
    {
        SettingsVm = new SettingsViewModel();
    }

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

    internal void CloseSettings()
    {
        QueryPages = new(QueryPages.Where(x => x is not SettingsTabViewModel));
    }
}

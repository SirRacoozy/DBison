using DBison.Core.Attributes;
using DBison.Core.Entities;
using DBison.Core.Extender;
using DBison.WPF.ClientBaseClasses;
using DBison.WPF.Dialogs;
using DBison.WPF.ViewModels;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace DBison.WPF;
public class MainWindowViewModel : ClientViewModelBase
{
    bool m_HasAddServerError = false;
    public MainWindowViewModel()
    {
        TabItems = new ObservableCollection<TabItemViewModelBase>();
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
        set
        {
            Set(value);
        }
    }
    #endregion

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

    private void __AddQueryPageIfPossible(string queryText)
    {
        if (SelectedServer != null && SelectedServer.DatabaseObject is ServerInfo serverInfo)
        {
            var dataBase = LastSelectedDatabase ?? serverInfo.DatabaseInfos.FirstOrDefault();
            if (dataBase != null)
                SelectedServer.AddNewQueryPage(dataBase, queryText);
        }
    }

    private void __ToggleSettingsPageIfNeeded()
    {
        if (!TabItems.Any(q => q is SettingsTabViewModel))
            TabItems.Add(new SettingsTabViewModel(this) { Header = "Settings" });
        else
            CloseSettings();
        SelectedTabItem = TabItems.LastOrDefault(s => s is SettingsTabViewModel);
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
        if (m_HasAddServerError)
        {
            newServerViewModel.Dispose();
            m_HasAddServerError = false;
            return;
        }
        ServerItems.Add(newServerViewModel);
        SelectedServer = newServerViewModel;
    }

    private void __NewServerViewModel_ErrorOccured(object? sender, Exception e)
    {
        m_HasAddServerError = true;
        ShowExceptionMessage(e);
    }

    private void __ExecuteOnDispatcherWithDelay(Action action, TimeSpan delay)
    {
        new Task(() =>
        {
            Thread.Sleep(delay);
            Application.Current.Dispatcher.Invoke(action);
        }).Start();
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
        TabItems = new(TabItems.Where(x => x is not SettingsTabViewModel));
    }
}

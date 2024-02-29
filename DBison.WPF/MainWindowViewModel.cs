using DBison.Core.Attributes;
using DBison.Core.Entities;
using DBison.WPF.ClientBaseClasses;
using DBison.WPF.Dialogs;
using DBison.WPF.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.ObjectModel;
using System.Windows;

namespace DBison.WPF;
public class MainWindowViewModel : ClientViewModelBase
{
    bool m_Error = false;
    public MainWindowViewModel()
    {
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

    public ObservableCollection<ServerQueryPageViewModel> QueryPages
    {
        get => Get<ObservableCollection<ServerQueryPageViewModel>>();
        set => Set(value);
    }

    #region [SelectedQueryPage]
    public ServerQueryPageViewModel SelectedQueryPage
    {
        get => Get<ServerQueryPageViewModel>();
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

    #region [AreSettingsOpen]
    public bool AreSettingsOpen
    {
        get => Get<bool>();
        set => Set(value);
    }
    #endregion

    public void Execute_AddServer()
    {
        var dialog = new AddServerDialog();
        dialog.ServerConnectRequested += (sender, e) => __AddServer(e);
        dialog.ShowDialog();
    }

    public void Execute_OpenSettings()
    {
        AreSettingsOpen = !AreSettingsOpen;
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
            QueryPages = new ObservableCollection<ServerQueryPageViewModel>();
            OnPropertyChanged(nameof(QueryPages));
        }
    }

    [DependsUpon(nameof(SelectedServer))]
    public void QueryPagesChanged()
    {
        if (SelectedServer == null)
            return;
        QueryPages = new(SelectedServer?.ServerQueryPages);
        OnPropertyChanged(nameof(QueryPages));
        SelectedQueryPage = QueryPages.LastOrDefault();
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
        }
        else if (selectedItem is ServerObjectTreeItemViewModel treeItemObject)
        {
            var serverVm = ServerItems.FirstOrDefault(x => x.DatabaseObject == treeItemObject.DatabaseObject.Server);
            if (serverVm != null && SelectedServer != serverVm)
                SelectedServer = serverVm;
        }
    }
}

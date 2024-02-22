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
        __InitServers();
        __ExecuteOnDispatcherWithDelay(Execute_AddServer, TimeSpan.FromSeconds(1));
    }

    #region [Server]
    public ObservableCollection<ServerViewModel> Server
    {
        get => Get<ObservableCollection<ServerViewModel>>();
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

    public void Execute_AddServer()
    {
        var dialog = new AddServerDialog();
        dialog.ServerConnectRequested += (sender, e) => __AddServer(e);
        dialog.ShowDialog();
    }

    public void RemoveServer(ServerViewModel server)
    {
        if (server != null && Server.Contains(server))
        {
            Server.Remove(server);
            server.Dispose();
            OnPropertyChanged(nameof(Server));
            SelectedServer = Server.FirstOrDefault();
        }
    }

    private void __InitServers()
    {
        Server = new ObservableCollection<ServerViewModel>();
        SelectedServer = Server.FirstOrDefault();
    }

    private void __AddServer(ServerInfo server)
    {
        if (Server == null)
            Server = new ObservableCollection<ServerViewModel>();
        var newServerViewModel = new ServerViewModel(server, __NewServerViewModel_ErrorOccured, this);
        if (m_Error)
        {
            newServerViewModel.Dispose();
            m_Error = false;
            return;
        }
        Server.Add(newServerViewModel);
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

}

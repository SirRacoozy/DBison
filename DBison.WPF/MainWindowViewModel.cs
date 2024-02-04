using DBison.Core.Baseclasses;
using DBison.Core.Entities;
using DBison.WPF.Dialogs;
using DBison.WPF.ViewModels;
using System.Collections.ObjectModel;

namespace DBison.WPF;
public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        __InitServers();
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

    private void __InitServers()
    {
        Server = new ObservableCollection<ServerViewModel>();
        SelectedServer = Server.FirstOrDefault();
    }

    private void __AddServer(ServerInfo server)
    {
        if (Server == null)
            Server = new ObservableCollection<ServerViewModel>();
        var newServerViewModel = new ServerViewModel(server);
        Server.Add(newServerViewModel);
        SelectedServer = newServerViewModel;
    }
}

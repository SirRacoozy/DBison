using DBison.Core.Baseclasses;
using DBison.Core.Entities;
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

    private void __InitServers()
    {
        Server = new ObservableCollection<ServerViewModel>();
       //TODO: Server via dialog
        SelectedServer = Server.FirstOrDefault();
    }

    private void __AddServer(string serverName)
    {
        if (Server == null)
        {
            Server = new ObservableCollection<ServerViewModel>();
        }
        var server = new ServerInfo(serverName)
        {
            Username = "", //TODO: Userid here, dialog
            Password = "", //TODO: PW here, dialog 
        };
        Server.Add(new ServerViewModel(server));
    }
}

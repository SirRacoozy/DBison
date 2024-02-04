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

    public string FilterText
    {
        get => Get<string>();
        set
        {
            Set(value);
        }
    }

    private void __InitServers()
    {
        Server = new ObservableCollection<ServerViewModel>();
        __AddServer("Server 1");
        __AddServer("Server 2");
        __AddServer("Server mit langen Namen");
        SelectedServer = Server.FirstOrDefault();
    }

    private void __AddServer(string serverName)
    {
        if (Server == null)
        {
            Server = new ObservableCollection<ServerViewModel>();
        }
        var server = new ServerInfo
        {
            ServerName = serverName,
        };

        __AddSampleDatabaseInfos(server);

        Server.Add(new ServerViewModel(server));
    }

    private void __AddSampleDatabaseInfos(ServerInfo server)
    {
        var databaseInfo1 = new ExtendedDatabaseInfo("Database 1");
        databaseInfo1.Tables.Add(new Table("Table 1"));
        databaseInfo1.Tables.Add(new Table("Table 2"));
        databaseInfo1.Views.Add(new View("View 1"));
        databaseInfo1.Views.Add(new View("View 2"));
        databaseInfo1.Triggers.Add(new Trigger("Trigger 1"));
        databaseInfo1.Triggers.Add(new Trigger("Trigger 2"));
        databaseInfo1.Procedures.Add(new StoredProcedure("Procedure 1"));
        databaseInfo1.Procedures.Add(new StoredProcedure("Procedure 2"));
        server.DatabaseInfos.Add(databaseInfo1);

        var databaseInfo2 = new ExtendedDatabaseInfo("Database 2");
        databaseInfo2.Tables.Add(new Table("Table 1"));
        databaseInfo2.Tables.Add(new Table("Table 2"));
        databaseInfo2.Views.Add(new View("View 1"));
        databaseInfo2.Views.Add(new View("View 2"));
        databaseInfo2.Triggers.Add(new Trigger("Trigger 1"));
        databaseInfo2.Triggers.Add(new Trigger("Trigger 2"));
        databaseInfo2.Procedures.Add(new StoredProcedure("Procedure 1"));
        databaseInfo2.Procedures.Add(new StoredProcedure("Procedure 2"));
        server.DatabaseInfos.Add(databaseInfo2);
    }
}

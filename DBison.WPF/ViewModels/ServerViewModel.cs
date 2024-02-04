using DBison.Core.Baseclasses;
using DBison.Core.Entities;
using DBison.Core.Extender;
using System.Collections.ObjectModel;

namespace DBison.WPF.ViewModels;
public class ServerViewModel : ViewModelBase
{
    public ServerViewModel(ServerInfo server)
    {
        Server = server;
        //ServerObjects = new ObservableCollection<ServerObjectTreeItemViewModel>();
        __AddNewQueryPage("Query 1");
        __AddNewQueryPage("Query 2");
        __AddNewQueryPage("Query 3");
    }

    public ServerInfo Server
    {
        get => Get<ServerInfo>() ?? new ServerInfo();
        set
        {
            Set(value);
            if (value != null)
                __InitTreeView(value);
        }
    }

    public ObservableCollection<ServerObjectTreeItemViewModel> ServerObjects
    {
        get => Get<ObservableCollection<ServerObjectTreeItemViewModel>>();
        set => Set(value);
    }

    public ObservableCollection<ServerQueryPageViewModel> ServerQueryPages
    {
        get => Get<ObservableCollection<ServerQueryPageViewModel>>();
        set => Set(value);
    }

    public string Header
    {
        get
        {
            if (Server.ServerName.IsNotNullOrEmpty())
                return Server.ServerName;
            return Server.ServerAddress.ToString();
            //else if(Server.ServerAddress ... IsNotDefault)
            //return "Unknown Server";
        }
    }

    private void __InitTreeView(ServerInfo server)
    {
        var treeItems = new ObservableCollection<ServerObjectTreeItemViewModel>(); //Should be the main nodes

        var databaseNode = __GetTreeItemViewModel(new DatabaseInfo("Databases")); //First Main Node
        databaseNode.IsExpanded = true;
        foreach (var dataBase in server.DatabaseInfos)
        {
            var databaseTreeItemVM = __GetTreeItemViewModel(dataBase);

            if (dataBase is ExtendedDatabaseInfo extendedInfo)
            {
                var tablesNode = __GetTreeItemViewModel(new Table("Tables"));
                extendedInfo.Tables.ForEach(t => tablesNode.ServerObjects.Add(__GetTreeItemViewModel(t)));
                databaseTreeItemVM.ServerObjects.Add(tablesNode);

                var viewNode = __GetTreeItemViewModel(new Table("Views"));
                extendedInfo.Views.ForEach(v => viewNode.ServerObjects.Add(__GetTreeItemViewModel(v)));
                databaseTreeItemVM.ServerObjects.Add(viewNode);

                var triggerNode = __GetTreeItemViewModel(new Table("Trigger"));
                extendedInfo.Triggers.ForEach(t => triggerNode.ServerObjects.Add(__GetTreeItemViewModel(t)));
                databaseTreeItemVM.ServerObjects.Add(triggerNode);

                var prodceduresNode = __GetTreeItemViewModel(new Table("Procedures"));
                extendedInfo.Procedures.ForEach(p => prodceduresNode.ServerObjects.Add(__GetTreeItemViewModel(p)));
                databaseTreeItemVM.ServerObjects.Add(prodceduresNode);
            }

            databaseNode.ServerObjects.Add(databaseTreeItemVM);
        }

        treeItems.Add(databaseNode); //Add main nodes

        ServerObjects = treeItems;
    }

    private ServerObjectTreeItemViewModel __GetTreeItemViewModel(DatabaseObjectBase databaseObject)
    {
        var treeItemViewModel = new ServerObjectTreeItemViewModel(databaseObject);
        treeItemViewModel.ServerObjects = new ObservableCollection<ServerObjectTreeItemViewModel>();
        return treeItemViewModel;
    }

    private void __AddNewQueryPage(string name)
    {
        if (ServerQueryPages == null)
            ServerQueryPages = new ObservableCollection<ServerQueryPageViewModel>();
        var viewModel = new ServerQueryPageViewModel(name);

        ServerQueryPages.Add(viewModel);
        OnPropertyChanged(nameof(ServerQueryPages));
    }

}

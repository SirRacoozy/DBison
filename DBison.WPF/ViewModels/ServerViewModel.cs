using DBison.Core.Baseclasses;
using DBison.Core.Entities;
using DBison.Core.Extender;
using DBison.Core.Helper;
using System.Collections.ObjectModel;

namespace DBison.WPF.ViewModels;
public class ServerViewModel : ViewModelBase
{
    ServerQueryHelper m_ServerQueryHelper;
    public ServerViewModel(ServerInfo server)
    {
        m_ServerQueryHelper = new ServerQueryHelper(server);
        m_ServerQueryHelper.LoadServerObjects();
        Server = server;
        __InitTreeView(server);
    }

    public ServerInfo Server
    {
        get => Get<ServerInfo>() ?? new ServerInfo("LOCALHOST");
        set => Set(value);
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

    public ServerQueryPageViewModel SelectedQueryPage
    {
        get => Get<ServerQueryPageViewModel>();
        set => Set(value);
    }

    public string Header
    {
        get
        {
            if (Server.ServerName.IsNotNullOrEmpty())
                return Server.ServerName;
            return Server.ServerAddress.ToString();
        }
    }

    public bool IsBusy
    {
        get => Get<bool>();
        set => Set(value);
    }

    public void SetBusyState(bool busy)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            IsBusy = busy;
        });
    }

    public void AddNewQueryPage(ServerObjectTreeItemViewModel serverObjectTreeItemViewModel)
    {
        if (ServerQueryPages == null)
            ServerQueryPages = new ObservableCollection<ServerQueryPageViewModel>();

        var viewModel = new ServerQueryPageViewModel($"Query ({serverObjectTreeItemViewModel.DatabaseObject.Name}) {ServerQueryPages.Count + 1}");

        ServerQueryPages.Add(viewModel);
        OnPropertyChanged(nameof(ServerQueryPages));
        SelectedQueryPage = viewModel;
    }

    private void __InitTreeView(ServerInfo server)
    {
        var treeItems = new ObservableCollection<ServerObjectTreeItemViewModel>(); //Should be the main nodes

        var databaseNode = __GetTreeItemViewModel(new DatabaseInfo("Databases", new ServerInfo("")), null); //First Main Node
        databaseNode.IsExpanded = true;
        foreach (var dataBase in server.DatabaseInfos)
        {
            var databaseTreeItemVM = __GetTreeItemViewModel(dataBase, null);

            if (dataBase is ExtendedDatabaseInfo extendedInfo)
            {
                var tablesNode = __GetTreeItemViewModel(new Table("Tables") { IsMainNode = true }, extendedInfo);
                tablesNode.ServerObjects.Add(__GetTreeItemViewModel(new Table("Loading...."), extendedInfo)); //Needs to be set, to expand and load real objects then
                databaseTreeItemVM.ServerObjects.Add(tablesNode);

                var viewNode = __GetTreeItemViewModel(new Table("Views") { IsMainNode = true }, extendedInfo);
                viewNode.ServerObjects.Add(__GetTreeItemViewModel(new View("Loading...."), extendedInfo)); //Needs to be set, to expand and load real objects then
                databaseTreeItemVM.ServerObjects.Add(viewNode);

                var triggerNode = __GetTreeItemViewModel(new Table("Trigger") { IsMainNode = true }, extendedInfo);
                triggerNode.ServerObjects.Add(__GetTreeItemViewModel(new Trigger("Loading...."), extendedInfo)); //Needs to be set, to expand and load real objects then
                databaseTreeItemVM.ServerObjects.Add(triggerNode);

                var prodceduresNode = __GetTreeItemViewModel(new Table("Procedures") { IsMainNode = true }, extendedInfo);
                prodceduresNode.ServerObjects.Add(__GetTreeItemViewModel(new StoredProcedure("Loading...."), extendedInfo)); //Needs to be set, to expand and load real objects then
                databaseTreeItemVM.ServerObjects.Add(prodceduresNode);
            }

            databaseNode.ServerObjects.Add(databaseTreeItemVM);
        }

        treeItems.Add(databaseNode); //Add main nodes

        ServerObjects = treeItems;
    }

    private ServerObjectTreeItemViewModel __GetTreeItemViewModel(DatabaseObjectBase databaseObject, ExtendedDatabaseInfo extendedDatabaseRef)
    {
        var treeItemViewModel = new ServerObjectTreeItemViewModel(databaseObject, m_ServerQueryHelper, extendedDatabaseRef, this);
        treeItemViewModel.ServerObjects = new ObservableCollection<ServerObjectTreeItemViewModel>();
        return treeItemViewModel;
    }
}

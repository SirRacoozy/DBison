using DBison.Core.Baseclasses;
using DBison.Core.Entities;
using DBison.Core.Extender;
using DBison.Core.Helper;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace DBison.WPF.ViewModels;
public class ServerViewModel : ViewModelBase
{
    ServerQueryHelper m_ServerQueryHelper;
    Action<object?, Exception> m_OnError;
    MainWindowViewModel m_MainWindowViewModel;

    #region [Ctor]
    public ServerViewModel(ServerInfo server, Action<object?, Exception> onError, MainWindowViewModel mainWindowViewModel)
    {
        m_MainWindowViewModel = mainWindowViewModel;
        SelectedBackGround = Brushes.Gray;
        m_OnError = onError;
        __InitServer(server);
    }
    #endregion

    #region - properties -
    #region - public properties -

    public Brush SelectedBackGround
    {
        get => Get<Brush>();
        set => Set(value);
    }

    #region [Server]
    public ServerInfo Server
    {
        get => Get<ServerInfo>() ?? new ServerInfo("LOCALHOST");
        set => Set(value);
    }
    #endregion

    #region [ServerObjects]
    public ObservableCollection<ServerObjectTreeItemViewModel> ServerObjects
    {
        get => Get<ObservableCollection<ServerObjectTreeItemViewModel>>();
        set => Set(value);
    }
    #endregion

    #region [ServerQueryPages]
    public ObservableCollection<ServerQueryPageViewModel> ServerQueryPages
    {
        get
        {
            var get = Get<ObservableCollection<ServerQueryPageViewModel>>();
            if (get == null)
                ServerQueryPages = new ObservableCollection<ServerQueryPageViewModel>();
            return Get<ObservableCollection<ServerQueryPageViewModel>>();
        }
        set
        {
            if (Get<ObservableCollection<ServerQueryPageViewModel>>() == null && value != null)
            {
                value.CollectionChanged += (sender, e) =>
                {
                    OnPropertyChanged(nameof(ServerQueryPages));
                };
            }
            Set(value);
        }
    }
    #endregion

    #region [SelectedQueryPage]
    public ServerQueryPageViewModel SelectedQueryPage
    {
        get => Get<ServerQueryPageViewModel>();
        set => Set(value);
    }
    #endregion

    #region [Header]
    public string Header
    {
        get
        {
            if (Server.ServerName.IsNotNullOrEmpty())
                return Server.ServerName;
            return Server.ServerAddress.ToString();
        }
    }
    #endregion

    #region [IsBusy]
    public bool IsBusy
    {
        get => Get<bool>();
        set => Set(value);
    }
    #endregion

    #endregion
    #endregion

    #region - public methods -

    public void Execute_Close()
    {
        m_MainWindowViewModel.RemoveServer(this);
    }

    #region [SetBusyState]
    public void SetBusyState(bool busy)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            IsBusy = busy;
        });
    }
    #endregion

    #region [AddNewQueryPage]
    public void AddNewQueryPage(ServerObjectTreeItemViewModel serverObjectTreeItemViewModel)
    {
        var viewModel = new ServerQueryPageViewModel($"Query {ServerQueryPages.Count + 1} ({serverObjectTreeItemViewModel.DatabaseObject.Name})", this);
        ServerQueryPages.Add(viewModel);
        SelectedQueryPage = viewModel;
    }
    #endregion

    #region [AddTableDataPage]
    public void AddTableDataPage(ServerObjectTreeItemViewModel serverObjectTreeItemViewModel)
    {
        int top = 100;

        if (serverObjectTreeItemViewModel.DatabaseObject is not Table)
            return;

        var viewModel = new ServerQueryPageViewModel($"Table Data TOP {top} ({serverObjectTreeItemViewModel.DatabaseObject.Name})", this);
        viewModel.ResultOnly = true;

        var dataTable = m_ServerQueryHelper.FillDataTable(serverObjectTreeItemViewModel.ExtendedDatabaseRef, serverObjectTreeItemViewModel.DatabaseObject.Name, top);

        if (dataTable == null)
            return;

        viewModel.ResultSets.Add(new ResultSetViewModel()
        {
            ResultLines = dataTable.DefaultView
        });

        ServerQueryPages.Add(viewModel);
        SelectedQueryPage = viewModel;
    }
    #endregion

    #region [ExecuteError]
    public void ExecuteError(Exception ex)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() => m_OnError?.Invoke(this, ex));
    }
    #endregion

    public void RemoveQuery(ServerQueryPageViewModel queryVm)
    {
        if (queryVm != null && ServerQueryPages.Contains(queryVm))
        {
            ServerQueryPages.Remove(queryVm);
            queryVm.Dispose();
            OnPropertyChanged(nameof(ServerQueryPages));
            SelectedQueryPage = ServerQueryPages.FirstOrDefault();
        }
    }

    #endregion

    #region [Dispose]
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
    #endregion

    #region - private methods -

    #region [__InitServer]
    private void __InitServer(ServerInfo server)
    {
        try
        {
            m_ServerQueryHelper = new ServerQueryHelper(server);
            m_ServerQueryHelper.LoadServerObjects();
            Server = server;
            __InitTreeView(server);
        }
        catch (Exception ex)
        {
            m_OnError?.Invoke(this, ex);
        }
    }
    #endregion

    #region [__InitTreeView]
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
    #endregion

    #region [__GetTreeItemViewModel]
    private ServerObjectTreeItemViewModel __GetTreeItemViewModel(DatabaseObjectBase databaseObject, ExtendedDatabaseInfo extendedDatabaseRef)
    {
        var treeItemViewModel = new ServerObjectTreeItemViewModel(databaseObject, m_ServerQueryHelper, extendedDatabaseRef, this);
        treeItemViewModel.ServerObjects = new ObservableCollection<ServerObjectTreeItemViewModel>();
        return treeItemViewModel;
    }
    #endregion

    #endregion
}
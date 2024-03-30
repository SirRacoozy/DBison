using DBison.Core.Entities;
using DBison.Core.Extender;
using DBison.Core.Helper;
using DBison.Core.Utils.SettingsSystem;
using DBison.WPF.ClientBaseClasses;
using DBison.WPF.HelperObjects;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Media;

namespace DBison.WPF.ViewModels;
public class ServerViewModel : ClientViewModelBase
{
    ServerQueryHelper m_ServerQueryHelper;
    Action<object?, Exception> m_OnError;
    MainWindowViewModel m_MainWindowViewModel;
    private string m_Filter;
    private ServerInfo m_Server;
    private ServerObjectTreeItemViewModel m_DataBaseNode;

    #region [Ctor]
    public ServerViewModel(ServerInfo server, Action<object?, Exception> onError, MainWindowViewModel mainWindowViewModel)
    {
        m_Server = server;
        m_MainWindowViewModel = mainWindowViewModel;
        SelectedBackGround = Brushes.Gray;
        m_OnError = onError;
        DatabaseObject = m_Server;
        m_ServerQueryHelper = new ServerQueryHelper(m_Server);
        __InitServer();
    }
    #endregion

    #region - properties -
    #region - public properties -

    #region [SelectedBackGround]
    public Brush SelectedBackGround
    {
        get => Get<Brush>();
        set => Set(value);
    }
    #endregion

    #region [IsExpanded]
    public bool IsExpanded
    {
        get
        {
            if (m_Filter.IsNotNullOrEmpty())
                return true;
            return Get<bool>();
        }
        set => Set(value);
    }
    #endregion

    #region [DatabaseObject]
    public ServerInfo DatabaseObject
    {
        get => Get<ServerInfo>();
        set => Set(value);
    }
    #endregion

    #region [ServerNode]
    public ServerObjectTreeItemViewModel ServerNode
    {
        get => Get<ServerObjectTreeItemViewModel>();
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
                value.CollectionChanged += __CollectionChanged;
            Set(value);
        }
    }
    #endregion

    #region [Header]
    public string Header
    {
        get
        {
            if (DatabaseObject.Name.IsNotNullOrEmpty())
                return DatabaseObject.Name;
            return DatabaseObject.ServerAddress.ToString();
        }
    }
    #endregion

    #region [AllDataBaseFolder]
    public ObservableCollection<ServerObjectTreeItemViewModel> AllDataBaseFolder
    {
        get => Get<ObservableCollection<ServerObjectTreeItemViewModel>>();
        set => Set(value);
    }
    #endregion

    #endregion
    #endregion

    #region - public methods -

    #region [Execute_Close]
    public void Execute_Close()
    {
        m_MainWindowViewModel.RemoveServer(this);
    }
    #endregion

    #region [AddNewQueryPage]
    internal void AddNewQueryPage(DatabaseInfo dataBase, string queryText)
    {
        __AddQueryPage(new QueryPageCreationReq()
        {
            DataBaseObject = dataBase,
            QueryText = queryText,
        });
    }
    #endregion


    #region [AddTableDataPage]
    public void AddTableDataPage(ServerObjectTreeItemViewModel serverObjectTreeItemViewModel)
    {
        int top = Settings.Limit;

        if (serverObjectTreeItemViewModel.DatabaseObject is not DBisonTable && serverObjectTreeItemViewModel.DatabaseObject is not DBisonView)
            return;

        var req = new QueryPageCreationReq()
        {
            Name = $"Table Data TOP {top} ({DatabaseObject.Name}.{serverObjectTreeItemViewModel.DatabaseObject.Name})",
            QueryText = $"SELECT TOP {top} * FROM {serverObjectTreeItemViewModel.DatabaseObject.Name}",
            DataBaseObject = serverObjectTreeItemViewModel.DatabaseObject,
            ExtendedDatabaseRef = serverObjectTreeItemViewModel.ExtendedDatabaseRef,
        };
        __AddQueryPage(req);
    }
    #endregion

    #region [ExecuteError]
    public void ExecuteError(Exception ex)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() => m_OnError?.Invoke(this, ex));
    }
    #endregion

    #region [RemoveQuery]
    public void RemoveQuery(TabItemViewModelBase tabItemVm)
    {
        if (tabItemVm != null && ServerQueryPages.Contains(tabItemVm) && tabItemVm is ServerQueryPageViewModel queryPageVm)
        {
            ServerQueryPages.Remove(queryPageVm);
            tabItemVm.Dispose();
            OnPropertyChanged(nameof(ServerQueryPages));
            m_MainWindowViewModel.QueryPagesChanged();
        }
    }
    #endregion

    #region [Filter]
    public void Filter(string filter)
    {
        if (m_Filter == filter || AllDataBaseFolder == null)
            return;
        m_Filter = filter;

        if (m_Filter.IsNullOrEmpty())
        {
            foreach (var folder in AllDataBaseFolder)
            {
                folder.Clear();
                folder.IsExpanded = false;
            }
        }
        else
        {
            foreach (var folder in AllDataBaseFolder)
            {
                folder.Filter();
            }
        }
    }
    #endregion

    #region [GetDataBaseState]
    public eDataBaseState GetDataBaseState(DatabaseInfo databaseInfo)
    {
        return m_ServerQueryHelper.GetDataBaseState(databaseInfo);
    }
    #endregion

    #region [RefreshDataBase]
    internal void RefreshDataBase(ServerObjectTreeItemViewModel serverObjectTreeItemViewModel)
    {
        serverObjectTreeItemViewModel.ServerObjects.Clear();
        __SetDataBaseObjects(serverObjectTreeItemViewModel.DatabaseObject.DataBase, serverObjectTreeItemViewModel);
    }
    #endregion

    #region [RefreshServer]
    internal void RefreshServer()
    {
        m_ServerQueryHelper.LoadServerObjects();
        __SetDatabaseNodes();
    }
    #endregion

    #endregion

    #region - private methods -

    #region [__InitServer]
    private void __InitServer()
    {
        try
        {
            m_ServerQueryHelper.LoadServerObjects();
            m_Filter = string.Empty; //Ensure no filtering
            __InitTreeView();
        }
        catch (Exception ex)
        {
            m_OnError?.Invoke(this, ex);
        }
    }
    #endregion

    #region [__InitTreeView]
    private void __InitTreeView()
    {
        ServerNode = new ServerObjectTreeItemViewModel(null, m_Server, m_ServerQueryHelper, null, this, m_MainWindowViewModel);
        var treeItems = new ObservableCollection<ServerObjectTreeItemViewModel>(); //Should be the main nodes
        AllDataBaseFolder = new ObservableCollection<ServerObjectTreeItemViewModel>();

        m_DataBaseNode = __GetTreeItemViewModel(ServerNode, new DatabaseInfo("Databases", m_Server, null) { IsMainNode = true }, null); //First Main Node
        ServerNode.ServerObjects.Add(m_DataBaseNode);
        __SetDatabaseNodes();

        treeItems.Add(m_DataBaseNode); //Add main nodes

        ServerObjects = treeItems;
    }

    private void __SetDatabaseNodes()
    {
        m_DataBaseNode.ServerObjects.Clear();
        foreach (var dataBase in m_Server.DatabaseInfos)
        {
            var databaseTreeItemVM = __GetTreeItemViewModel(m_DataBaseNode, dataBase, null);
            databaseTreeItemVM.DatabaseObject.IsMainNode = true;

            m_DataBaseNode.ServerObjects.Add(databaseTreeItemVM);

            if (dataBase.DataBaseState != eDataBaseState.ONLINE)
                continue;
            __SetDataBaseObjects(dataBase, databaseTreeItemVM);

        }
        OnPropertyChanged(nameof(ServerObjects));
    }
    #endregion

    #region [__GetTreeItemViewModel]
    private ServerObjectTreeItemViewModel __GetTreeItemViewModel(ServerObjectTreeItemViewModel parent, DatabaseObjectBase databaseObject, ExtendedDatabaseInfo extendedDatabaseRef)
    {
        var treeItemViewModel = new ServerObjectTreeItemViewModel(parent, databaseObject, m_ServerQueryHelper, extendedDatabaseRef, this, m_MainWindowViewModel);
        if (databaseObject.IsFolder)
            AllDataBaseFolder.Add(treeItemViewModel);
        treeItemViewModel.ServerObjects = new ObservableCollection<ServerObjectTreeItemViewModel>();
        return treeItemViewModel;
    }
    #endregion

    #region [__AddQueryPage]
    private void __AddQueryPage(QueryPageCreationReq req)
    {
        if (req == null || req.DataBaseObject == null || !req.DataBaseObject.DataBase.IsRealDataBaseNode || req.DataBaseObject.DataBase.DataBaseState != eDataBaseState.ONLINE)
            return;

        var dataBaseName = req.DataBaseObject.Name;

        req.ServerQueryHelper = m_ServerQueryHelper;
        req.ServerViewModel = this;

        if (req.Name.IsNullOrEmpty())
            req.Name = $"Query {ServerQueryPages.Count + 1} - [{ServerNode.TreeItemHeader}].[{dataBaseName}]";

        ServerQueryPages.Add(new ServerQueryPageViewModel(req));
        m_MainWindowViewModel.QueryPagesChanged();
    }
    #endregion

    #region [__CollectionChanged]
    private void __CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(ServerQueryPages));
    }
    #endregion

    #region [__SetDataBaseNodes]
    private void __SetDataBaseObjects(DatabaseInfo dataBase, ServerObjectTreeItemViewModel databaseTreeItemVM)
    {
        if (dataBase is ExtendedDatabaseInfo extendedInfo)
        {
            var tablesNode = __GetTreeItemViewModel(databaseTreeItemVM, new DBisonTable("Tables", m_Server, extendedInfo) { IsMainNode = true, IsFolder = true }, extendedInfo);
            tablesNode.ServerObjects.Add(__GetTreeItemViewModel(tablesNode, new DBisonTable("Loading....", m_Server, extendedInfo) { IsPlaceHolder = true }, extendedInfo)); //Needs to be set, to expand and load real objects then
            databaseTreeItemVM.ServerObjects.Add(tablesNode);

            var viewNode = __GetTreeItemViewModel(databaseTreeItemVM, new DBisonView("Views", m_Server, extendedInfo) { IsMainNode = true, IsFolder = true }, extendedInfo);
            viewNode.ServerObjects.Add(__GetTreeItemViewModel(viewNode, new DBisonView("Loading....", m_Server, extendedInfo) { IsPlaceHolder = true }, extendedInfo)); //Needs to be set, to expand and load real objects then
            databaseTreeItemVM.ServerObjects.Add(viewNode);

            var triggerNode = __GetTreeItemViewModel(databaseTreeItemVM, new DBisonTrigger("Trigger", m_Server, extendedInfo) { IsMainNode = true, IsFolder = true }, extendedInfo);
            triggerNode.ServerObjects.Add(__GetTreeItemViewModel(triggerNode, new DBisonTrigger("Loading....", m_Server, extendedInfo) { IsPlaceHolder = true }, extendedInfo)); //Needs to be set, to expand and load real objects then
            databaseTreeItemVM.ServerObjects.Add(triggerNode);

            var prodceduresNode = __GetTreeItemViewModel(databaseTreeItemVM, new DBisonStoredProcedure("Procedures", m_Server, extendedInfo) { IsMainNode = true, IsFolder = true }, extendedInfo);
            prodceduresNode.ServerObjects.Add(__GetTreeItemViewModel(prodceduresNode, new DBisonStoredProcedure("Loading....", m_Server, extendedInfo) { IsPlaceHolder = true }, extendedInfo)); //Needs to be set, to expand and load real objects then
            databaseTreeItemVM.ServerObjects.Add(prodceduresNode);
        }
    }
    #endregion

    #endregion

    #region [Dispose]
    protected override void Dispose(bool disposing)
    {
        if (!disposing || IsDisposed)
            return;

        ServerQueryPages.CollectionChanged -= __CollectionChanged;
        foreach (var queryPage in ServerQueryPages)
        {
            queryPage.Dispose();
        }
        ServerQueryPages = null;

        base.Dispose(disposing);
    }
    #endregion
}
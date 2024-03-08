﻿using DBison.Core.Entities;
using DBison.Core.Extender;
using DBison.Core.Helper;
using DBison.Core.Utils.SettingsSystem;
using DBison.WPF.ClientBaseClasses;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media;

namespace DBison.WPF.ViewModels;
public class ServerViewModel : ClientViewModelBase
{
    ServerQueryHelper m_ServerQueryHelper;
    Action<object?, Exception> m_OnError;
    MainWindowViewModel m_MainWindowViewModel;
    private string m_Filter;
    private ServerInfo m_Server;

    #region [Ctor]
    public ServerViewModel(ServerInfo server, Action<object?, Exception> onError, MainWindowViewModel mainWindowViewModel)
    {
        m_Server = server;
        m_MainWindowViewModel = mainWindowViewModel;
        SelectedBackGround = Brushes.Gray;
        m_OnError = onError;
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

    #region [CloseVisibility]
    public Visibility CloseVisibility => Visibility.Visible;
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
    public void AddNewQueryPage(DatabaseObjectBase databaseObject, string queryText)
        => __AddQueryPage(databaseObject, databaseObject.Name, queryText);
    public void AddNewQueryPage(ServerObjectTreeItemViewModel serverObjectTreeItemViewModel, string queryText)
    {
        var databaseObjectName = DatabaseObject.Name;
        __AddQueryPage(serverObjectTreeItemViewModel?.DatabaseObject, databaseObjectName, queryText);
    }
    #endregion

    #region [AddTableDataPage]
    public void AddTableDataPage(ServerObjectTreeItemViewModel serverObjectTreeItemViewModel)
    {
        int top = Settings.Limit;

        if (serverObjectTreeItemViewModel.DatabaseObject is not DBisonTable && serverObjectTreeItemViewModel.DatabaseObject is not DBisonView)
            return;

        var sql = $"SELECT TOP {top} * FROM {serverObjectTreeItemViewModel.DatabaseObject.Name}";
        var viewModel = new ServerQueryPageViewModel($"Table Data TOP {top} ({DatabaseObject.Name}.{serverObjectTreeItemViewModel.DatabaseObject.Name})", this, serverObjectTreeItemViewModel.DatabaseObject, m_ServerQueryHelper);
        viewModel.QueryText = sql;

        viewModel.FillDataTable(sql, serverObjectTreeItemViewModel.ExtendedDatabaseRef);
        __AddQueryPage(viewModel, string.Empty);
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
            __InitTreeView();
        }
        else
        {
            foreach (var folder in AllDataBaseFolder)
            {
                folder.Filter(filter);
            }
        }
    }
    #endregion

    #endregion

    #region - private methods -

    #region [__InitServer]
    private void __InitServer()
    {
        try
        {
            m_ServerQueryHelper = new ServerQueryHelper(m_Server);
            m_ServerQueryHelper.LoadServerObjects();
            DatabaseObject = m_Server;
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
        var treeItems = new ObservableCollection<ServerObjectTreeItemViewModel>(); //Should be the main nodes
        AllDataBaseFolder = new ObservableCollection<ServerObjectTreeItemViewModel>();

        var databaseNode = __GetTreeItemViewModel(null, new DatabaseInfo("Databases", m_Server, null) { IsMainNode = true }, null); //First Main Node
        foreach (var dataBase in m_Server.DatabaseInfos)
        {
            var databaseTreeItemVM = __GetTreeItemViewModel(databaseNode, dataBase, null);
            databaseTreeItemVM.DatabaseObject.IsMainNode = true;

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

            databaseNode.ServerObjects.Add(databaseTreeItemVM);
        }

        treeItems.Add(databaseNode); //Add main nodes

        ServerObjects = treeItems;
    }
    #endregion

    #region [__GetTreeItemViewModel]
    private ServerObjectTreeItemViewModel __GetTreeItemViewModel(ServerObjectTreeItemViewModel parent, DatabaseObjectBase databaseObject, ExtendedDatabaseInfo extendedDatabaseRef)
    {
        var treeItemViewModel = new ServerObjectTreeItemViewModel(parent, databaseObject, m_ServerQueryHelper, extendedDatabaseRef, this);
        if (databaseObject.IsFolder)
            AllDataBaseFolder.Add(treeItemViewModel);
        treeItemViewModel.ServerObjects = new ObservableCollection<ServerObjectTreeItemViewModel>();
        return treeItemViewModel;
    }
    #endregion

    #region [__AddQueryPage]
    private void __AddQueryPage(DatabaseObjectBase databaseObject, string databaseObjectName, string queryText)
    {
        if (databaseObjectName.IsNullOrEmpty())
            databaseObjectName = databaseObject.Name;
        var viewModel = new ServerQueryPageViewModel($"Query {ServerQueryPages.Count + 1} - {databaseObjectName}.{databaseObjectName}", this, databaseObject, m_ServerQueryHelper);
        __AddQueryPage(viewModel, queryText);
    }

    private void __AddQueryPage(ServerQueryPageViewModel viewModel, string queryText)
    {
        if (queryText.IsNotNullOrEmpty())
            viewModel.QueryText = queryText;
        ServerQueryPages.Add(viewModel);
        m_MainWindowViewModel.QueryPagesChanged();
    }
    #endregion

    #region [__CollectionChanged]
    private void __CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(ServerQueryPages));
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
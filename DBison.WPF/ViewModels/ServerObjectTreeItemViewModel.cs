using DBison.Core.Attributes;
using DBison.Core.Entities;
using DBison.Core.Extender;
using DBison.Core.Helper;
using DBison.Core.Utils.Commands;
using DBison.Core.Utils.SettingsSystem;
using DBison.WPF.ClientBaseClasses;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DBison.WPF.ViewModels;

public class ServerObjectTreeItemViewModel : ClientViewModelBase
{
    readonly ServerQueryHelper m_ServerQueryHelper;
    readonly ServerViewModel m_ServerVm;
    readonly MainWindowViewModel m_MainWindowViewModel;
    public ServerObjectTreeItemViewModel(ServerObjectTreeItemViewModel parent, DatabaseObjectBase databaseObject, ServerQueryHelper serverQueryHelper, ExtendedDatabaseInfo extendedDatabaseRef, ServerViewModel serverVm, MainWindowViewModel mainWindowViewModel)
    {
        m_MainWindowViewModel = mainWindowViewModel;
        ServerObjects = new ObservableCollection<ServerObjectTreeItemViewModel>();
        Parent = parent;
        SelectedBackGround = Brushes.Gray;
        DatabaseObject = databaseObject;
        m_ServerQueryHelper = serverQueryHelper;
        ExtendedDatabaseRef = extendedDatabaseRef;
        m_ServerVm = serverVm;
        __SetContextMenu();
    }

    #region - public properties -

    public Visibility StateVisibility => DatabaseObject is DatabaseInfo && DatabaseObject.IsRealDataBaseNode ? Visibility.Visible : Visibility.Collapsed;

    public ServerObjectTreeItemViewModel Parent
    {
        get => Get<ServerObjectTreeItemViewModel>();
        private set => Set(value);
    }

    public Brush SelectedBackGround
    {
        get => Get<Brush>();
        set => Set(value);
    }

    public DatabaseObjectBase DatabaseObject
    {
        get => Get<DatabaseObjectBase>();
        set => Set(value);
    }

    public ExtendedDatabaseInfo ExtendedDatabaseRef
    {
        get => Get<ExtendedDatabaseInfo>();
        private set => Set(value);
    }

    public bool IsExpanded
    {
        get => Get<bool>();
        set
        {
            Set(value);
            if (value && ServerObjects.IsNotEmpty() && ServerObjects.All(o => o.DatabaseObject != null && o.DatabaseObject.IsPlaceHolder))
                __LoadSubObjects();
            if (Parent != null)
                Parent.IsExpanded = value;
        }
    }

    public Visibility CloseVisibility => DatabaseObject != null && DatabaseObject is ServerInfo ? Visibility.Visible : Visibility.Collapsed;

    public ObservableCollection<ServerObjectTreeItemViewModel> ServerObjects
    {
        get => Get<ObservableCollection<ServerObjectTreeItemViewModel>>();
        set
        {
            if (value != null)
            {
                Set(value);
                value.CollectionChanged += (object? sender, NotifyCollectionChangedEventArgs e) =>
                {
                    OnPropertyChanged(nameof(ServerObjects));
                };
            }
        }
    }

    public ObservableCollection<MenuItem> MenuItems
    {
        get => Get<ObservableCollection<MenuItem>>();
        set
        {
            Set(value);
            if (value != null)
                value.CollectionChanged += (sender, e) => OnPropertyChanged(nameof(MenuItems));
        }
    }

    public bool IsLoading
    {
        get => Get<bool>();
        set => Set(value);
    }

    [DependsUpon(nameof(DatabaseObject))]
    [DependsUpon(nameof(IsLoading))]
    public string TreeItemHeader
    {
        get
        {
            var baseName = DatabaseObject.Name;
            if (IsLoading)
                baseName += " - Loading";
            return baseName;
        }
    }

    #endregion

    #region - public methods

    #region [Filter]
    public void Filter()
    {
        __LoadSubObjects();
    }
    #endregion

    #region [Clear]
    public void Clear()
    {
        ExecuteOnDispatcher(() =>
        {
            ServerObjects.Clear();
            OnPropertyChanged(nameof(ServerObjects));
            IsExpanded = false;
            var m_Server = m_ServerVm.DatabaseObject;
            if (Parent.DatabaseObject is ExtendedDatabaseInfo extendedInfo)
            {
                var databaseObject = DatabaseObject.Clone();
                databaseObject.Name = "Loading....";
                databaseObject.IsPlaceHolder = true;
                ServerObjects.Add(__GetTreeItemViewModel(Parent, databaseObject, extendedInfo)); //Needs to be set, to expand and load real objects then
            }
        });
    }
    #endregion

    #region [RefreshState]
    internal void RefreshState()
    {
        if (DatabaseObject.DataBase.DataBaseState == eDataBaseState.ONLINE)
            m_ServerVm.RefreshDataBase(this);
        else
            ExecuteOnDispatcher(() => ServerObjects.Clear());
        OnPropertyChanged(nameof(DatabaseObject));
    }
    #endregion

    #region [OnCanExecuteChanged]
    public override void OnCanExecuteChanged(string commandName)
    {
        GetCommandNames().ForEach(c =>
        {
            try
            {
                var command = Get<RelayCommand>(commandName);
                if (command != null)
                    System.Windows.Application.Current.Dispatcher.Invoke(() => command.OnCanExecuteChanged());
            }
            catch (Exception ex)
            {
                m_ServerVm?.ExecuteError(ex);
            }
        });
    }
    #endregion

    #endregion

    #region - commands -

    #region [Execute_NewQuery]
    public void Execute_NewQuery()
    {
        m_ServerVm.AddNewQueryPage(DatabaseObject.DataBase, string.Empty);
    }
    #endregion

    #region [Execute_ShowTableData]
    public void Execute_ShowTableData()
    {
        m_ServerVm.AddTableDataPage(this);
    }
    #endregion

    #region [Execute_SwitchState]
    public void Execute_SwitchState()
    {
        try
        {
            m_ServerQueryHelper.SwitchDataBaseStatus(DatabaseObject.DataBase);
            m_MainWindowViewModel.RefreshLastSelectedDataBaseState();
            __SetContextMenu();
        }
        catch (Exception ex)
        {
            ShowExceptionMessage(ex);
        }
    }
    #endregion

    #region [Execute_Clone]
    public void Execute_Clone()
    {
        try
        {
            var newName = GetInput("Choose new name for clone", "Please type a name for the clone", $"{DatabaseObject.DataBase.Name}_Clone");
            var dataBase = DatabaseObject.DataBase;
            if (dataBase is ExtendedDatabaseInfo extendedDbInfo)
            {
                var newDataFileLocation = @$"{Path.GetDirectoryName(extendedDbInfo.DataFileLocation)}\{newName}.mdf";
                var newLogFileLocation = @$"{Path.GetDirectoryName(extendedDbInfo.LogFileLocation)}\{newName}.ldf";
                ExecuteWithOfflineDb(dataBase, m_ServerQueryHelper, () =>
                {
                    File.Copy(extendedDbInfo.DataFileLocation, newDataFileLocation, true);
                    File.Copy(extendedDbInfo.LogFileLocation, newLogFileLocation, true);
                    m_ServerQueryHelper.CloneDataBase(DatabaseObject.DataBase, newName, newDataFileLocation, newLogFileLocation);
                });
                m_ServerVm.RefreshServer();
            }
        }
        catch (Exception ex)
        {
            ShowExceptionMessage(ex);
        }
    }
    #endregion

    #region [Execute_DeleteDataBase]
    public void Execute_DeleteDataBase()
    {
        try
        {
            var dataBase = DatabaseObject.DataBase;
            if (dataBase is ExtendedDatabaseInfo extendedDbInfo)
            {
                m_ServerQueryHelper.DeleteDataBase(DatabaseObject.DataBase);
                if (File.Exists(extendedDbInfo.DataFileLocation))
                    File.Delete(extendedDbInfo.DataFileLocation);
                if (File.Exists(extendedDbInfo.LogFileLocation))
                    File.Delete(extendedDbInfo.LogFileLocation);
                m_ServerVm.RefreshServer();
            }
        }
        catch (Exception ex)
        {
            ShowExceptionMessage(ex);
        }
    }
    #endregion

    #region [Execute_RefreshServer]
    public void Execute_RefreshServer()
    {
        try
        {
            m_ServerVm.RefreshServer();
        }
        catch (Exception ex)
        {
            ShowExceptionMessage(ex);
        }
    }
    #endregion

    #region [Execute_Rename]
    public void Execute_Rename()
    {
        try
        {
            var newName = GetInput("Choose new name for renaming", "Please type a name for the rename", DatabaseObject.DataBase.Name);
            var dataBase = DatabaseObject.DataBase;
            m_ServerQueryHelper.RenameDataBase(dataBase, newName);
            m_ServerVm.RefreshServer();
        }
        catch (Exception ex)
        {
            ShowExceptionMessage(ex);
        }
    }
    #endregion

    #region [CanExecute_DeleteLogFile]
    public bool CanExecute_DeleteLogFile() => false;
    #endregion

    #region [Execute_DeleteLogFile]
    public void Execute_DeleteLogFile()
    {
        try
        {
            var dataBase = DatabaseObject.DataBase;
            if (dataBase is ExtendedDatabaseInfo extendedDbInfo)
            {
                m_ServerQueryHelper.DeleteDatabaseFile(dataBase, extendedDbInfo.LogFileLocation);
                ExecuteWithOfflineDb(dataBase, m_ServerQueryHelper, () =>
                {
                    if (File.Exists(extendedDbInfo.LogFileLocation))
                        File.Delete(extendedDbInfo.LogFileLocation);
                });
            }
        }
        catch (Exception ex)
        {
            ShowExceptionMessage(ex);
        }
    }
    #endregion

    #region [Execute_Close]
    public void Execute_Close()
    {
        m_MainWindowViewModel.RemoveServer(m_ServerVm);
    }
    #endregion

    #region [CanExecute_CreateBackup]
    public bool CanExecute_CreateBackup() => false;
    #endregion

    #region [Execute_CreateBackup]
    public void Execute_CreateBackup()
    {
        try
        {

        }
        catch (Exception ex)
        {
            ShowExceptionMessage(ex);
        }
    }
    #endregion

    #region [CanExecute_CreateODBC]
    public bool CanExecute_CreateODBC() => false;
    #endregion

    #region [Execute_CreateODBC]
    public void Execute_CreateODBC()
    {
        try
        {

        }
        catch (Exception ex)
        {
            ShowExceptionMessage(ex);
        }
    }
    #endregion

    #endregion

    #region - private methods -

    #region [__LoadSubObjects]
    private void __LoadSubObjects()
    {
        var task = new Task(() =>
        {
            if (ExtendedDatabaseRef == null || !DatabaseObject.IsMainNode || m_ServerVm == null)
                return;

            try
            {
                if (DatabaseObject is DBisonTable)
                {
                    __SetLoading(true);
                    m_ServerQueryHelper.LoadTables(ExtendedDatabaseRef, m_MainWindowViewModel.FilterText);
                    __AddSubVms(new(ExtendedDatabaseRef.Tables));
                    __SetLoading(false);
                }
                else if (DatabaseObject is DBisonView)
                {
                    __SetLoading(true);
                    m_ServerQueryHelper.LoadViews(ExtendedDatabaseRef, m_MainWindowViewModel.FilterText);
                    __AddSubVms(new(ExtendedDatabaseRef.Views));
                    __SetLoading(false);
                }
                else if (DatabaseObject is DBisonTrigger)
                {
                    __SetLoading(true);
                    m_ServerQueryHelper.LoadTrigger(ExtendedDatabaseRef, m_MainWindowViewModel.FilterText);
                    __AddSubVms(new(ExtendedDatabaseRef.Triggers));
                    __SetLoading(false);
                }
                else if (DatabaseObject is DBisonStoredProcedure)
                {
                    __SetLoading(true);
                    m_ServerQueryHelper.LoadProcedures(ExtendedDatabaseRef, m_MainWindowViewModel.FilterText);
                    __AddSubVms(new(ExtendedDatabaseRef.Procedures));
                    __SetLoading(false);
                }
            }
            catch (Exception ex)
            {
                m_ServerVm.ExecuteError(ex);
            }
        });
        task.Start();
    }
    #endregion

    #region [__SetLoading]
    private void __SetLoading(bool loading)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
        {
            IsLoading = loading;
        }));
    }
    #endregion

    #region [__AddSubVms]
    private void __AddSubVms(List<DatabaseObjectBase> databaseObjects)
    {
        ExecuteOnDispatcher(() =>
        {
            ServerObjects.Clear();
            foreach (var dbObject in databaseObjects)
            {
                ServerObjects.Add(new ServerObjectTreeItemViewModel(this, dbObject, m_ServerQueryHelper, ExtendedDatabaseRef, m_ServerVm, m_MainWindowViewModel));
            }
            if (Settings.AutoExpandNodes && m_MainWindowViewModel.FilterText.IsNotNullOrEmpty() && ServerObjects.Any())
                IsExpanded = true;
        });
    }
    #endregion

    #region [__SetContextMenu]
    private void __SetContextMenu()
    {
        if (MenuItems == null)
            MenuItems = new ObservableCollection<MenuItem>();
        else
            MenuItems.Clear();

        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
        {
            //Menus on server nodes
            if (DatabaseObject is ServerInfo)
            {
                MenuItems.Add(new MenuItem { Header = $"Refresh Server ", Command = this["RefreshServer"] as ICommand });
            }
            //Menus on database nodes
            else if (DatabaseObject is DatabaseInfo dbInfo)
            {
                if (dbInfo.IsRealDataBaseNode)
                    __AddDataBaseMenuItems(dbInfo);
                else
                    MenuItems.Add(new MenuItem { Header = $"Refresh", Command = this["RefreshServer"] as ICommand });
            }
            //Menus on table or view nodes
            else if (!DatabaseObject.IsFolder && (DatabaseObject is DBisonTable || DatabaseObject is DBisonView))
            {
                MenuItems.Add(new MenuItem { Header = $"Show {DatabaseObject.Name} data", Command = this["ShowTableData"] as ICommand });
            }
        }));
    }
    #endregion

    #region [__AddDataBaseMenuItems]
    private void __AddDataBaseMenuItems(DatabaseInfo dbInfo)
    {
        var tmpMenuItems = new List<MenuItem>();
        tmpMenuItems.Add(new MenuItem { Header = $"Delete database [{DatabaseObject.Name}]", Command = this["DeleteDataBase"] as ICommand });
        if (dbInfo.DataBaseState == eDataBaseState.OFFLINE)
        {
            tmpMenuItems.Add(new MenuItem { Header = $"Take [{DatabaseObject.Name}] Online", Command = this["SwitchState"] as ICommand });
        }
        else
        {
            tmpMenuItems.Add(new MenuItem
            {
                Header = $"New Query - [{DatabaseObject.Name}]",
                Command = this["NewQuery"] as ICommand
            });
            tmpMenuItems.Add(new MenuItem
            {
                Header = $"Switch database State [{DatabaseObject.Name}]",
                Command = this["SwitchState"] as ICommand
            });
            tmpMenuItems.Add(new MenuItem
            {
                Header = $"Delete Log File",
                Command = this["DeleteLogFile"] as ICommand
            });
            tmpMenuItems.Add(new MenuItem
            {
                Header = $"Rename",
                Command = this["Rename"] as ICommand
            });
            tmpMenuItems.Add(new MenuItem
            {
                Header = $"Clone",
                Command = this["Clone"] as ICommand
            });
            tmpMenuItems.Add(new MenuItem
            {
                Header = $"CreateBackup",
                Command = this["CreateBackup"] as ICommand
            });
            tmpMenuItems.Add(new MenuItem
            {
                Header = $"CreateODBC",
                Command = this["CreateODBC"] as ICommand
            });
        }
        MenuItems = new(tmpMenuItems.OrderBy(m => m.Header));
    }
    #endregion

    #region [__GetTreeItemViewModel]
    private ServerObjectTreeItemViewModel __GetTreeItemViewModel(ServerObjectTreeItemViewModel parent, DatabaseObjectBase databaseObject, ExtendedDatabaseInfo extendedDatabaseRef)
    {
        var treeItemViewModel = new ServerObjectTreeItemViewModel(parent, databaseObject, m_ServerQueryHelper, extendedDatabaseRef, m_ServerVm, m_MainWindowViewModel);
        treeItemViewModel.ServerObjects = new ObservableCollection<ServerObjectTreeItemViewModel>();
        return treeItemViewModel;
    }
    #endregion

    #endregion

}
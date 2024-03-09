using DBison.Core.Attributes;
using DBison.Core.Entities;
using DBison.Core.Extender;
using DBison.Core.Helper;
using DBison.Core.Utils.Commands;
using DBison.Core.Utils.SettingsSystem;
using DBison.WPF.ClientBaseClasses;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DBison.WPF.ViewModels;

public class ServerObjectTreeItemViewModel : ClientViewModelBase
{
    readonly ServerQueryHelper m_ServerQueryHelper;
    readonly ServerViewModel m_ServerVm;
    private string m_Filter;
    public ServerObjectTreeItemViewModel(ServerObjectTreeItemViewModel parent, DatabaseObjectBase databaseObject, ServerQueryHelper serverQueryHelper, ExtendedDatabaseInfo extendedDatabaseRef, ServerViewModel serverVm)
    {
        Parent = parent;
        SelectedBackGround = Brushes.Gray;
        DatabaseObject = databaseObject;
        m_ServerQueryHelper = serverQueryHelper;
        ExtendedDatabaseRef = extendedDatabaseRef;
        m_ServerVm = serverVm;
        __SetContextMenu();
    }

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
            if (value && ServerObjects.All(o => o.DatabaseObject.IsPlaceHolder))
                __LoadSubObjects();
            if (Parent != null)
                Parent.IsExpanded = true;
            //else
            //{

            //}
        }
    }

    public Visibility CloseVisibility => Visibility.Collapsed;

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
        set => Set(value);
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

    public void Filter(string filter)
    {
        m_Filter = filter;
        __LoadSubObjects();
    }

    public void Execute_NewQuery()
    {
        m_ServerVm.AddNewQueryPage(this, string.Empty);
    }

    public void Execute_ShowTableData()
    {
        m_ServerVm.AddTableDataPage(this);
    }

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
                    m_ServerQueryHelper.LoadTables(ExtendedDatabaseRef, m_Filter);
                    __AddSubVms(new(ExtendedDatabaseRef.Tables));
                    __SetLoading(false);
                }
                else if (DatabaseObject is DBisonView)
                {
                    __SetLoading(true);
                    m_ServerQueryHelper.LoadViews(ExtendedDatabaseRef, m_Filter);
                    __AddSubVms(new(ExtendedDatabaseRef.Views));
                    __SetLoading(false);
                }
                else if (DatabaseObject is DBisonTrigger)
                {
                    __SetLoading(true);
                    m_ServerQueryHelper.LoadTrigger(ExtendedDatabaseRef, m_Filter);
                    __AddSubVms(new(ExtendedDatabaseRef.Triggers));
                    __SetLoading(false);
                }
                else if (DatabaseObject is DBisonStoredProcedure)
                {
                    __SetLoading(true);
                    m_ServerQueryHelper.LoadProcedures(ExtendedDatabaseRef, m_Filter);
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

    private void __SetLoading(bool loading)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
        {
            IsLoading = loading;
        }));
    }

    private void __AddSubVms(List<DatabaseObjectBase> databaseObjects)
    {
        ExecuteOnDispatcher(() =>
        {
            ServerObjects.Clear();
            foreach (var dbObject in databaseObjects)
            {
                ServerObjects.Add(new ServerObjectTreeItemViewModel(this, dbObject, m_ServerQueryHelper, ExtendedDatabaseRef, m_ServerVm));
            }
            if (Settings.AutoExpandNodes && m_Filter.IsNotNullOrEmpty() && ServerObjects.Any())
                IsExpanded = true;
        });
    }

    private void __SetContextMenu()
    {
        if (DatabaseObject == null || DatabaseObject.IsMainNode)
            return;
        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
        {
            MenuItems = new ObservableCollection<MenuItem>();
            if (DatabaseObject is DatabaseInfo)
            {
                MenuItems.Add(new MenuItem { Header = $"New Query - [{DatabaseObject.Name}]", Command = this["NewQuery"] as ICommand });
            }
            else if (DatabaseObject is DBisonTable || DatabaseObject is DBisonView)
            {
                MenuItems.Add(new MenuItem { Header = $"Show {DatabaseObject.Name} data", Command = this["ShowTableData"] as ICommand });
            }
        }));
    }

}

﻿using DBison.Core.Baseclasses;
using DBison.Core.Entities;
using DBison.Core.Helper;
using DBison.Core.Utils.Commands;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DBison.WPF.ViewModels;

public class ServerObjectTreeItemViewModel : ViewModelBase
{
    ServerQueryHelper m_ServerQueryHelper;
    ServerViewModel m_ServerVm;
    public ServerObjectTreeItemViewModel(DatabaseObjectBase databaseObject, ServerQueryHelper serverQueryHelper, ExtendedDatabaseInfo extendedDatabaseRef, ServerViewModel serverVm)
    {
        SelectedBackGround = Brushes.Gray;
        DatabaseObject = databaseObject;
        m_ServerQueryHelper = serverQueryHelper;
        ExtendedDatabaseRef = extendedDatabaseRef;
        m_ServerVm = serverVm;
        __SetContextMenu();
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
            if (value && ServerObjects.Count == 1)
                __LoadSubObjects();
        }
    }

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

    public void Execute_NewQuery()
    {
        m_ServerVm.AddNewQueryPage(this);
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
                m_ServerVm.ExecuteError(ex);
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
                if (DatabaseObject is Table)
                {
                    m_ServerVm.SetBusyState(true);
                    m_ServerQueryHelper.LoadTables(ExtendedDatabaseRef);
                    ServerObjects = __GetSubVms(new(ExtendedDatabaseRef.Tables));
                    m_ServerVm.SetBusyState(false);
                }
                else if (DatabaseObject is View)
                {
                    m_ServerVm.SetBusyState(true);
                    m_ServerQueryHelper.LoadViews(ExtendedDatabaseRef);
                    ServerObjects = __GetSubVms(new(ExtendedDatabaseRef.Views));
                    m_ServerVm.SetBusyState(false);
                }
                else if (DatabaseObject is Trigger)
                {
                    m_ServerVm.SetBusyState(true);
                    m_ServerQueryHelper.LoadTrigger(ExtendedDatabaseRef);
                    ServerObjects = __GetSubVms(new(ExtendedDatabaseRef.Triggers));
                    m_ServerVm.SetBusyState(false);
                }
                else if (DatabaseObject is StoredProcedure)
                {
                    m_ServerVm.SetBusyState(true);
                    m_ServerQueryHelper.LoadProcedures(ExtendedDatabaseRef);
                    ServerObjects = __GetSubVms(new(ExtendedDatabaseRef.Procedures));
                    m_ServerVm.SetBusyState(false);
                }
            }
            catch (Exception ex)
            {
                m_ServerVm.ExecuteError(ex);
            }
        });
        task.Start();
    }

    private ObservableCollection<ServerObjectTreeItemViewModel> __GetSubVms(List<DatabaseObjectBase> databaseObjects)
    {
        return new(databaseObjects.Select(o => new ServerObjectTreeItemViewModel(o, m_ServerQueryHelper, ExtendedDatabaseRef, m_ServerVm)));
    }

    private void __SetContextMenu()
    {
        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
        {
            MenuItems = new ObservableCollection<MenuItem>();
            if (DatabaseObject is DatabaseInfo)
            {
                MenuItems.Add(new MenuItem { Header = "New Query", Command = this["NewQuery"] as ICommand });
            }
            else if (DatabaseObject is Table tbl)
            {
                MenuItems.Add(new MenuItem { Header = $"Show {tbl.Name} data", Command = this["ShowTableData"] as ICommand });
            }
        }));
    }

}
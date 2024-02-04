using DBison.Core.Baseclasses;
using DBison.Core.Entities;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Controls;

namespace DBison.WPF.ViewModels;

public class ServerObjectTreeItemViewModel : ViewModelBase
{
    public ServerObjectTreeItemViewModel(DatabaseObjectBase databaseObject)
    {
        DatabaseObject = databaseObject;
        MenuItems = new ObservableCollection<MenuItem>()
        {
            new MenuItem {Header = "Foo1"},
        };
    }

    public DatabaseObjectBase DatabaseObject
    {
        get => Get<DatabaseObjectBase>();
        set => Set(value);
    }

    public bool IsExpanded
    {
        get => Get<bool>();
        set => Set(value);
    }

    public ObservableCollection<ServerObjectTreeItemViewModel> ServerObjects
    {
        get => Get<ObservableCollection<ServerObjectTreeItemViewModel>>();
        set
        {
            if (ServerObjects == null && value != null)
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

}

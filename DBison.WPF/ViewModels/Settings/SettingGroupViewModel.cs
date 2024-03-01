using DBison.WPF.ClientBaseClasses;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace DBison.WPF.ViewModels
{
    public class SettingGroupViewModel : ClientViewModelBase
    {
        public SettingGroupViewModel(string groupName, string groupHeader)
        {
            Name = groupName;
            GroupHeader = groupHeader;
            SettingItems = new ObservableCollection<SettingItemViewModel>();
            SettingItems.CollectionChanged += (sender, e) => OnPropertyChanged(nameof(SettingItems));
        }

        public string Name
        {
            get => Get<string>();
            private set => Set(value);
        }

        public string GroupHeader
        {
            get => Get<string>();
            private set => Set(value);
        }

        public ObservableCollection<SettingItemViewModel> SettingItems
        {
            get => Get<ObservableCollection<SettingItemViewModel>>();
            private set => Set(value);
        }

    }
}

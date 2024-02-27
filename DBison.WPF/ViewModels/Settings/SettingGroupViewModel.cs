using DBison.WPF.ClientBaseClasses;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace DBison.WPF.ViewModels
{
    public class SettingGroupViewModel : ClientViewModelBase
    {
        public SettingGroupViewModel(string groupName, string groupHeader)
        {
            GroupName = groupName;
            GroupHeader = groupHeader;
            SettingItems = new ObservableCollection<SettingItemViewModel>();
            SettingItems.CollectionChanged += (sender, e) => OnPropertyChanged(nameof(SettingItems));
            TabItem = __InitTabItem(groupName);
        }

        #region [TabItem]
        public TabItem TabItem
        {
            get => Get<TabItem>();
            set => Set(value);
        }
        #endregion

        public string GroupName
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

        private TabItem __InitTabItem(string groupName)
            => new TabItem()
            {
                Header = groupName,
                DataContext = this,
                Content = new ItemsControl
                {
                    ItemsSource = SettingItems,
                    Focusable = false,
                }
            };
    }
}

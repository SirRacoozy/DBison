using DBison.WPF.ClientBaseClasses;
using System.Collections.ObjectModel;

namespace DBison.WPF.ViewModels
{
    public class SettingGroupViewModel : ClientViewModelBase
    {
        #region - ctor -
        public SettingGroupViewModel(string groupName, string groupHeader)
        {
            Name = groupName;
            GroupHeader = groupHeader;
            SettingItems = new ObservableCollection<SettingItemViewModel>();
            SettingItems.CollectionChanged += (sender, e) => OnPropertyChanged(nameof(SettingItems));
        }
        #endregion

        #region [Name]
        public string Name
        {
            get => Get<string>();
            private set => Set(value);
        }
        #endregion

        #region [GroupHeader]
        public string GroupHeader
        {
            get => Get<string>();
            private set => Set(value);
        }
        #endregion

        #region [SettingItems]
        public ObservableCollection<SettingItemViewModel> SettingItems
        {
            get => Get<ObservableCollection<SettingItemViewModel>>();
            private set => Set(value);
        }
        #endregion
    }
}

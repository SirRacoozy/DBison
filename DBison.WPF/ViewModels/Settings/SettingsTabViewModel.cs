using DBison.Core.Attributes;
using DBison.Core.EventArguments;
using DBison.Core.Extender;
using DBison.Core.Utils.SettingsSystem;
using DBison.WPF.ClientBaseClasses;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace DBison.WPF.ViewModels
{
    public class SettingsTabViewModel : TabItemViewModelBase
    {
        private MainWindowViewModel m_MainWindowViewModel;

        #region - ctor -
        public SettingsTabViewModel(MainWindowViewModel mainWindowViewModel) : base(true)
        {
            __ReadAndGenerateSettings();
            SettingsHandler.SettingChanged += __SettingsHandler_SettingChanged;
            m_MainWindowViewModel = mainWindowViewModel;
        }
        #endregion

        #region [SelectedSettingsGroup]
        public SettingGroupViewModel SelectedSettingsGroup
        {
            get => Get<SettingGroupViewModel>();
            set => Set(value);
        }
        #endregion

        #region [SettingGroups]
        public ObservableCollection<SettingGroupViewModel> SettingGroups
        {
            get => Get<ObservableCollection<SettingGroupViewModel>>();
            set => Set(value);
        }
        #endregion

        #region [Execute_Close]
        public override void Execute_Close()
        {
            m_MainWindowViewModel.CloseSettings();
        }
        #endregion

        #region [__ReadAndGenerateSettings]
        private void __ReadAndGenerateSettings()
        {
            SettingGroups = new ObservableCollection<SettingGroupViewModel>();
            var properties = typeof(Settings).GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(typeof(SettingAttribute), false);
                if (attributes != null && attributes.FirstOrDefault() is SettingAttribute settingAttribute)
                {
                    RangeAttribute range = null;
                    var rangeAttributes = property.GetCustomAttributes(typeof(RangeAttribute), false);
                    if (rangeAttributes != null && rangeAttributes.FirstOrDefault() is RangeAttribute rangeAttribute)
                        range = rangeAttribute;
                    var settingsGroup = __AddOrGetSettingsGroupIfNeeded(settingAttribute);
                    settingsGroup.SettingItems.Add(new SettingItemViewModel(settingAttribute, range, property));
                }
            }

            __EvaluateAllSettings();
            SelectedSettingsGroup = SettingGroups.FirstOrDefault();
        }
        #endregion

        #region [__SettingsHandler_SettingChanged]
        private void __SettingsHandler_SettingChanged(object? sender, SettingChangedEventArgs e)
        {
            SettingsHandler.SettingChanged -= __SettingsHandler_SettingChanged;
            __EvaluateAllSettings();
            SettingsHandler.SettingChanged += __SettingsHandler_SettingChanged;
        }
        #endregion

        #region [__EvaluateAllSettings]
        private void __EvaluateAllSettings()
        {
            var allSettings = SettingGroups.SelectMany(g => g.SettingItems);
            foreach (var settingItem in allSettings)
            {
                settingItem.EvaluateDependencies(allSettings);
            }
        }
        #endregion

        #region [__AddOrGetSettingsGroupIfNeeded]
        private SettingGroupViewModel __AddOrGetSettingsGroupIfNeeded(SettingAttribute attribute)
        {
            SettingGroupViewModel settingsGroup;
            var groupName = attribute.GroupName;
            settingsGroup = SettingGroups.FirstOrDefault(g => g.Name.IsEquals(groupName));
            if (settingsGroup != null)
                return settingsGroup;
            settingsGroup = new SettingGroupViewModel(groupName, attribute.Header);
            SettingGroups.Add(settingsGroup);
            return settingsGroup;
        }
        #endregion
    }
}

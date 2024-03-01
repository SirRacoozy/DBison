﻿using DBison.Core.Attributes;
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
        public SettingsTabViewModel(MainWindowViewModel mainWindowViewModel) : base(true)
        {
            __ReadAndGenerateSettings();
            m_MainWindowViewModel = mainWindowViewModel;
        }

        #region [Execute_Close]
        public override void Execute_Close()
        {
            m_MainWindowViewModel.CloseSettings();
        }
        #endregion


        public SettingGroupViewModel SelectedSettingsGroup
        {
            get => Get<SettingGroupViewModel>();
            set => Set(value);
        }

        public ObservableCollection<SettingGroupViewModel> SettingGroups
        {
            get => Get<ObservableCollection<SettingGroupViewModel>>();
            set => Set(value);
        }

        private void __ReadAndGenerateSettings()
        {
            SettingGroups = new ObservableCollection<SettingGroupViewModel>();
            var properties = typeof(Settings).GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(typeof(SettingAttribute), false);
                if (attributes != null && attributes.FirstOrDefault() is SettingAttribute settingAttribute)
                {
                    if (!settingAttribute.IsVisible)
                        continue;
                    RangeAttribute range = null;
                    var rangeAttributes = property.GetCustomAttributes(typeof(RangeAttribute), false);
                    if (rangeAttributes != null && rangeAttributes.FirstOrDefault() is RangeAttribute rangeAttribute)
                        range = rangeAttribute;
                    var settingsGroup = __AddOrGetSettingsGroupIfNeeded(settingAttribute);
                    settingsGroup.SettingItems.Add(new SettingItemViewModel(settingAttribute, range, property));
                }
            }
            SelectedSettingsGroup = SettingGroups.FirstOrDefault();
        }
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

    }
}
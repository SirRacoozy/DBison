using DBison.WPF.ClientBaseClasses;

namespace DBison.WPF.ViewModels
{
    public class SettingsTabViewModel : TabItemViewModelBase
    {
        public SettingsTabViewModel() : base(true)
        {
        }

        public SettingsViewModel SettingsViewModel
        {
            get => Get<SettingsViewModel>();
            set => Set(value);
        }

    }
}

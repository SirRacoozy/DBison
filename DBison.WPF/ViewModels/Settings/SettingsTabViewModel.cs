using DBison.WPF.ClientBaseClasses;

namespace DBison.WPF.ViewModels
{
    public class SettingsTabViewModel : TabItemViewModelBase
    {
        private MainWindowViewModel m_MainWindowViewModel;
        public SettingsTabViewModel(MainWindowViewModel mainWindowViewModel) : base(true)
        {
            m_MainWindowViewModel = mainWindowViewModel;
        }

        public SettingsViewModel SettingsViewModel
        {
            get => Get<SettingsViewModel>();
            set => Set(value);
        }

        #region [Execute_Close]
        public override void Execute_Close()
        {
            m_MainWindowViewModel.CloseSettings();
        }
        #endregion

    }
}

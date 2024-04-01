using DBison.Core.BaseClasses;
using DBison.Core.Entities;
using DBison.Core.Helper;
using DBison.Core.Utils.Commands;
using DBison.Core.Utils.SettingsSystem;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Media;

namespace DBison.WPF.ClientBaseClasses
{
    public class ClientViewModelBase : ViewModelBase
    {
        #region - ctor -
        public ClientViewModelBase()
        {
            SystemEvents.UserPreferenceChanged += (_, _) => OnPropertyChanged(nameof(IconSource));
            ForeGround = Settings.UseDarkMode ? Brushes.White : Brushes.Black;
            SettingsHandler.SettingChanged += (sender, e) =>
            {
                if (e.ChangedSettingName == nameof(Settings.UseDarkMode))
                {
                    ForeGround = Settings.UseDarkMode ? Brushes.White : Brushes.Black;
                    BackGround = Settings.UseDarkMode ? Brushes.Gray : Brushes.White;
                    OnPropertyChanged(nameof(ImageSource));
                }
            };
        }
        #endregion

        public string ImageSource
        {
            get => Settings.UseDarkMode ? "./../Resources/AppIcon_White.ico" : "./../Resources/AppIcon_Black.ico";
        }
        public string IconSource
        {
            get => __GetWindowsTheme() ? "./../Resources/AppIcon_White.ico" : "./../Resources/AppIcon_Black.ico";
        }

        #region [ForeGround]
        public Brush ForeGround
        {
            get => Get<Brush>();
            set => Set(value);
        }
        #endregion

        #region [BackGround]
        public Brush BackGround
        {
            get => Get<Brush>();
            set => Set(value);
        }
        #endregion

        #region - public methods -
        #region [OnCanExecuteChanged]
        public override void OnCanExecuteChanged(string commandName)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var command = Get<RelayCommand>(commandName);
                if (command != null)
                    command.OnCanExecuteChanged();
            }));
        }
        #endregion

        #region [ShowExceptionMessage]
        public void ShowExceptionMessage(Exception e)
        {
            if (Application.Current.MainWindow is MetroWindow metroWnd)
            {
                metroWnd.ShowMessageAsync("Exception occured", e.Message);
            }
        }
        #endregion

        #region [ShowMessageAsync]
        public void ShowMessageAsync(string header, string message)
        {
            if (Application.Current.MainWindow is MetroWindow metroWnd)
            {
                metroWnd.ShowMessageAsync(header, message);
            }
        }
        #endregion

        #region [ExecuteOnDispatcherWithDelay]
        public void ExecuteOnDispatcherWithDelay(Action action, TimeSpan delay)
        {
            new Task(() =>
            {
                Thread.Sleep(delay);
                Application.Current.Dispatcher.Invoke(action);
            }).Start();
        }
        #endregion

        #region [ExecuteOnDispatcher]
        public void ExecuteOnDispatcher(Action actionToExecute)
        {
            _ = Application.Current.Dispatcher.BeginInvoke(actionToExecute);
        }
        #endregion

        #region [GetInput]
        public string GetInput(string title, string message, string defaultInput)
        {
            if (Application.Current.MainWindow is MetroWindow metroWnd)
            {
                var Settings = new MetroDialogSettings
                {
                    DefaultText = defaultInput,
                };
                return DialogManager.ShowModalInputExternal(metroWnd, title, message, Settings)?.Trim();
            }
            return defaultInput;
        }
        #endregion

        #region [ExecuteWithOfflineDb]
        public void ExecuteWithOfflineDb(DatabaseInfo databaseInfo, ServerQueryHelper queryHelper, Action action)
        {
            bool wasDbOnline = databaseInfo.DataBaseState == eDataBaseState.ONLINE;
            try
            {
                queryHelper.TakeDataBaseOffline(databaseInfo);
                action?.Invoke();
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex);
            }
            finally
            {
                if (wasDbOnline)
                    queryHelper.TakeDataBaseOnline(databaseInfo);
            }
        }
        #endregion

        #endregion

        #region - private methods -
        private const string m_RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

        private const string m_RegistryValueName = "AppsUseLightTheme";

        private bool __GetWindowsTheme()
        {
            using var key = Registry.CurrentUser.OpenSubKey(m_RegistryKeyPath);
            var registryValueObject = key?.GetValue(m_RegistryValueName);
            if (registryValueObject == null)
            {
                return false;
            }
            var registryValue = (int)registryValueObject;

            return registryValue > 0 ? false : true;
        }
        #endregion

    }
}

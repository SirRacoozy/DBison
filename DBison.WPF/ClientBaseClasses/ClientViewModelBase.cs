using DBison.Core.Baseclasses;
using DBison.Core.Utils.Commands;
using DBison.Core.Utils.SettingsSystem;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using System.Windows.Media;

namespace DBison.WPF.ClientBaseClasses
{
    public class ClientViewModelBase : ViewModelBase
    {
        public ClientViewModelBase()
        {
            ForeGround = Settings.UseDarkMode ? Brushes.White : Brushes.Black;
            SettingsHandler.SettingChanged += (sender, e) =>
            {
                if (e.ChangedSettingName == nameof(Settings.UseDarkMode))
                {
                    ForeGround = Settings.UseDarkMode ? Brushes.White : Brushes.Black;
                    BackGround = Settings.UseDarkMode ? Brushes.Gray : Brushes.White;
                }
            };
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

        public void ShowMessageAsync(string header, string message)
        {
            if (Application.Current.MainWindow is MetroWindow metroWnd)
            {
                metroWnd.ShowMessageAsync(header, message);
            }
        }

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

        #endregion

    }
}

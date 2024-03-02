using DBison.Core.Baseclasses;
using DBison.Core.Utils.Commands;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;

namespace DBison.WPF.ClientBaseClasses
{
    public class ClientViewModelBase : ViewModelBase
    {
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

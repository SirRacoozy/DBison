using DBison.Core.Baseclasses;
using DBison.Core.Utils.Commands;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;

namespace DBison.WPF.ClientBaseClasses
{
    public class ClientViewModelBase : ViewModelBase
    {
        public override void OnCanExecuteChanged(string commandName)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var command = Get<RelayCommand>(commandName);
                if (command != null)
                    command.OnCanExecuteChanged();
            }));
        }

        public void ShowExceptionMessage(Exception e)
        {
            if (Application.Current.MainWindow is MetroWindow metroWnd)
            {
                metroWnd.ShowMessageAsync("Exception occured", e.Message);
            }
        }

    }
}

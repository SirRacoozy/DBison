using DBison.Core.Baseclasses;
using DBison.Core.Utils.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}

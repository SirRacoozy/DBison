using DBison.Core.Entities;
using DBison.WPF.ViewModels;
using DBison.WPF.Views;
using System.Windows;

namespace DBison.WPF.Dialogs;
public class AddServerDialog
{
    public event EventHandler<ServerInfo> ServerConnectRequested;
    public void ShowDialog()
    {
        var window = new Window();
        var viewModel = new AddServerDialogViewModel(window);
        viewModel.OkClicked += (sender, e) => { ServerConnectRequested?.Invoke(null, e); };
        window.Content = new AddServerDialogContent()
        {
            DataContext = viewModel
        };
        window.SizeToContent = SizeToContent.WidthAndHeight;
        _ = window.ShowDialog();
    }
}

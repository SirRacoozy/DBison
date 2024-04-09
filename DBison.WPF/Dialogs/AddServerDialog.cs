using DBison.Plugin.Entities;
using DBison.WPF.ViewModels;
using DBison.WPF.Views;
using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DBison.WPF.Dialogs;
public class AddServerDialog
{
    public event EventHandler<ConnectInfo> ServerConnectRequested;

    #region [ShowDialog]
    public void ShowDialog()
    {
        var window = new MetroWindow();
        window.ResizeMode = ResizeMode.NoResize;
        window.ShowCloseButton = false;
        var viewModel = new AddServerDialogViewModel(window);
        viewModel.OkClicked += (sender, e) => { ServerConnectRequested?.Invoke(null, e); };

        window.Content = new Border()
        {
            BorderBrush = Brushes.White,
            BorderThickness = new Thickness(.5),
            Child = new AddServerDialogContent()
            {
                DataContext = viewModel,
            },
        };
        window.SizeToContent = SizeToContent.WidthAndHeight;
        _ = window.ShowDialog();
    }
    #endregion

}

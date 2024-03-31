using DBison.Core.Entities;
using DBison.Plugin.Entities;
using DBison.WPF.Controls;
using DBison.WPF.ViewModels;
using DBison.WPF.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shell;

namespace DBison.WPF.Dialogs;
public class AddServerDialog
{
    public event EventHandler<ConnectInfo> ServerConnectRequested;
    public void ShowDialog()
    {
        var window = new Window();
        window.WindowStyle = WindowStyle.None;
        window.ResizeMode = ResizeMode.NoResize;
        var viewModel = new AddServerDialogViewModel(window);
        viewModel.OkClicked += (sender, e) => { ServerConnectRequested?.Invoke(null, e); };

        WindowChrome.SetWindowChrome(window, new WindowChrome { UseAeroCaptionButtons = false });

        window.Content = new Border()
        {
            BorderBrush = Brushes.White,
            BorderThickness = new Thickness(.5),
            Child = new WindowChromedContent()
            {
                Content = new AddServerDialogContent()
                {
                    DataContext = viewModel,
                },
                Window = window
            }
        };
        window.SizeToContent = SizeToContent.WidthAndHeight;
        _ = window.ShowDialog();
    }
}

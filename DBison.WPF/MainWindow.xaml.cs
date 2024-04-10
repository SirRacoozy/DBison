using DBison.Core.Utils.SettingsSystem;
using DBison.WPF.HelperObjects;
using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Input;

namespace DBison.WPF;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : MetroWindow
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += __MainWindow_Loaded;
        DataContext = new MainWindowViewModel();
    }

    private void __MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is MetroWindow wnd)
            wnd.SetButtons();
    }

    private void __MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
    {
        if (Keyboard.Modifiers == ModifierKeys.Control && DataContext is MainWindowViewModel vm)
        {
            if (e.Delta > 0)
                Settings.UIScaling += 0.1;
            else if (e.Delta < 0)
                Settings.UIScaling -= 0.1;
        }
    }
}
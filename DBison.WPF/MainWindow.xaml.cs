using DBison.WPF.HelperObjects;
using MahApps.Metro.Controls;
using System.Windows;

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
}
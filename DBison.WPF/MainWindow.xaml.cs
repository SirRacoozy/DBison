using DBison.Core.PluginSystem;
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
        DataContext = new MainWindowViewModel();
    }

    private void __Click_Click(object sender, System.Windows.RoutedEventArgs e)
    {
#if DEBUG
        __TestPluginSystem();
#endif

    }

#if DEBUG
    private static void __TestPluginSystem()
    {
        var loader = PluginLoader.Instance;

        //_ = MessageBox.Show($"{r1.Name}\n{r2.Message}");
    }
#endif
}
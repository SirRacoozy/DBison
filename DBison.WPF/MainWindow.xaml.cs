using ControlzEx.Standard;
using DBison.Core.PluginSystem;
using MahApps.Metro.Controls;
using System.Diagnostics;
using System.Windows;

namespace DBison.WPF;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : MetroWindow
{
    public MainWindow() => InitializeComponent();

    private void __Click_Click(object sender, System.Windows.RoutedEventArgs e)
    {
#if DEBUG
        __TestPluginSystem();
#endif

    }

#if DEBUG
    private static void __TestPluginSystem()
    {
        var path = @"D:\Dbison\DBison.ExamplePlugin\bin\Debug\net8.0\";
        var loader = new PluginLoader(path);
        var r1 = loader.SearchParsingPlugins.First().ParseSearchInput(string.Empty);
        var r2 = loader.ContextMenuPlugins.First().Execute(null);

        _ = MessageBox.Show($"{r1.Name}\n{r2.Message}");
    }
#endif
}
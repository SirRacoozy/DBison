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
        var path = @"D:\Dbison\DBison.ExamplePlugin\bin\Debug\net8.0\DBison.ExamplePlugin.dll";
        var loader = new PluginLoader(path);
        var result = loader.SearchParsingPlugins.First().ParseSearchInput("Hallo Welt");
        _ = MessageBox.Show($"{result.Name}");
        
    }
}
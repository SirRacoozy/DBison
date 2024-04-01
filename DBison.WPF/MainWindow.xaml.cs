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
}
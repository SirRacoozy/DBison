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
        StateChanged += __MainWindowStateChangeRaised;
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
        var path = @"D:\Dbison\DBison.ExamplePlugin\bin\Debug\net8.0\";
        var loader = new PluginLoader(path);
        var r1 = loader.SearchParsingPlugins.First().ParseSearchInput(string.Empty);
        var r2 = loader.ContextMenuPlugins.First().Execute(null);

        //_ = MessageBox.Show($"{r1.Name}\n{r2.Message}");
    }
#endif

    private void __CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = true;
    }

    private void __Execute_Minimize(object sender, ExecutedRoutedEventArgs e)
    {
        SystemCommands.MinimizeWindow(this);
    }

    private void __Execute_Maximize(object sender, ExecutedRoutedEventArgs e)
    {
        SystemCommands.MaximizeWindow(this);
    }

    private void __Execute_Restore(object sender, ExecutedRoutedEventArgs e)
    {
        SystemCommands.RestoreWindow(this);
    }

    private void __Execute_Close(object sender, ExecutedRoutedEventArgs e)
    {
        SystemCommands.CloseWindow(this);
    }

    private void __MainWindowStateChangeRaised(object sender, EventArgs e)
    {
        if (WindowState == WindowState.Maximized)
        {
            MainWindowBorder.BorderThickness = new Thickness(2);
            RestoreButton.Visibility = Visibility.Visible;
            MaximizeButton.Visibility = Visibility.Collapsed;
        }
        else
        {
            MainWindowBorder.BorderThickness = new Thickness(2);
            RestoreButton.Visibility = Visibility.Collapsed;
            MaximizeButton.Visibility = Visibility.Visible;
        }
    }

    private void __MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            if (WindowState == WindowState.Maximized)
                SystemCommands.RestoreWindow(this);
            else
                SystemCommands.MaximizeWindow(this);
        }
    }
}
using ControlzEx.Theming;
using System.Configuration;
using System.Data;
using System.Windows;

namespace DBison.WPF;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ThemeManager.Current.ChangeTheme(this, "Dark.Purple");
    }
}


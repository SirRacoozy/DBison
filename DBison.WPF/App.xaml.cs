using ControlzEx.Theming;
using DBison.Core.Utils.SettingsSystem;
using NLog;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace DBison.WPF;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private List<ScrollViewer> m_Scrollers = new List<ScrollViewer>();
    private static Logger m_Logger = LogManager.GetCurrentClassLogger();
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        __SetupExceptionHandling();
        var baseTheme = Settings.UseDarkMode ? "Dark" : "Light";
        _ = ThemeManager.Current.ChangeTheme(this, $"{baseTheme}.Purple");
    }

    private void __SetupExceptionHandling()
    {
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            __LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

        DispatcherUnhandledException += (s, e) =>
        {
            __LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
            e.Handled = true;
        };

        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            __LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
            e.SetObserved();
        };
    }

    private void __LogUnhandledException(Exception exception, string source)
    {
        string message = $"Unhandled exception ({source})";
        try
        {
            System.Reflection.AssemblyName assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
            message = string.Format("Unhandled exception in {0} v{1}", assemblyName.Name, assemblyName.Version);
        }
        catch (Exception ex)
        {
            m_Logger.Error(ex, "Exception in LogUnhandledException");
        }
        finally
        {
            m_Logger.Error(exception, message);
        }
    }

    private void __ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
    {
        if (sender is ScrollViewer sv)
        {
            foreach (var tsv in m_Scrollers.Where(s => s != sv))
            {
                tsv.ScrollToVerticalOffset(sv.VerticalOffset);
                tsv.ScrollToHorizontalOffset(sv.HorizontalOffset);
            }
        }
    }

    private void __ScrollViewerLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is ScrollViewer sv && !m_Scrollers.Contains(sv))
        {
            m_Scrollers.Add(sv);
            sv.ScrollChanged += __ScrollChanged;
        }
    }
}


using ControlzEx.Theming;
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
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        _ = ThemeManager.Current.ChangeTheme(this, "Dark.Purple");
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


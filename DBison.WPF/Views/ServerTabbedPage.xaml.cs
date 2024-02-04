using DBison.WPF.Controls;
using System.Windows.Controls;

namespace DBison.WPF.Views;
/// <summary>
/// Interaction logic for ServerTabbedPage.xaml
/// </summary>
public partial class ServerTabbedPage : UserControlBase
{
    public ServerTabbedPage()
    {
        InitializeComponent();
    }

    private void __TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        if (DataContext is MainWindowViewModel mainVm && sender is TextBox t)
        {
            mainVm.FilterText = t.Text;
        }
    }
}

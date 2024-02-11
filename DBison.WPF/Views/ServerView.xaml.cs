using DBison.WPF.Controls;
using DBison.WPF.ViewModels;
using System.Windows.Controls;

namespace DBison.WPF.Views;
/// <summary>
/// Interaction logic for ServerView.xaml
/// </summary>
public partial class ServerView : UserControlBase
{
    public ServerView()
    {
        InitializeComponent();
    }

    #region - private methods -
    #region [__SelectionChanged]
    private void __SelectionChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (sender is TextBox tb && DataContext is ServerQueryPageViewModel vm)
        {
            vm.SelectedQueryText = tb.SelectedText;
            vm.QueryText = tb.Text;
        }
    }
    #endregion

    #region [__LostTextBoxFocus]
    private void __LostTextBoxFocus(object sender, System.Windows.RoutedEventArgs e)
    {
        e.Handled = true;
    }
    #endregion
    #endregion
}

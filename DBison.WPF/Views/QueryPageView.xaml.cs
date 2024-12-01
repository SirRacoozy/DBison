using DBison.WPF.Controls;
using DBison.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DBison.WPF.Views;
/// <summary>
/// Interaction logic for QueryPageView.xaml
/// </summary>
public partial class QueryPageView : UserControl
{
    #region - ctor -
    public QueryPageView()
    {
        InitializeComponent();
        Loaded += __Loaded;
    }
    #endregion

    #region [__Loaded]
    private void __Loaded(object sender, RoutedEventArgs e)
    {
        QueryTextBox.Focusable = true;
        Keyboard.Focus(QueryTextBox);
    }
    #endregion

    #region [__SelectionChanged]
    private void __SelectionChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (sender is LineNumberedTextBox tb && DataContext is ServerQueryPageViewModel vm)
        {
            vm.SelectedQueryText = !string.IsNullOrWhiteSpace(tb.Selection.Text) ? tb.Selection.Text : tb.Text;
            vm.QueryText = tb.Text;
        }
    }
    #endregion
}

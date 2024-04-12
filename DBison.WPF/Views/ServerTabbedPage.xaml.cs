using DBison.WPF.ClientBaseClasses;
using DBison.WPF.Controls;
using System.Windows.Controls;
using System.Windows.Input;

namespace DBison.WPF.Views;
/// <summary>
/// Interaction logic for ServerTabbedPage.xaml
/// </summary>
public partial class ServerTabbedPage : UserControl
{
    #region - ctor -
    public ServerTabbedPage()
    {
        InitializeComponent();
    }
    #endregion

    #region [__TextChanged]
    private void __TextChanged(object sender, TextChangedEventArgs e)
    {
        if (DataContext is MainWindowViewModel mainVm && sender is TextBox t)
            mainVm.FilterText = t.Text;
    }
    #endregion

    #region [__SelectedItemChanged]
    private void __SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
    {
        if (sender is ServerInfoTreeView tv && tv.DataContext is MainWindowViewModel mainVm)
            mainVm.SetSelectedServerIfNeeded(tv.TreeView.SelectedItem);
    }
    #endregion
}

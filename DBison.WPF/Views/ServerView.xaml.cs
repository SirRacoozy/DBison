using DBison.WPF.Controls;
using DBison.WPF.Converter;
using DBison.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace DBison.WPF.Views;
/// <summary>
/// Interaction logic for ServerView.xaml
/// </summary>
public partial class ServerView : UserControl
{
    #region - ctor -
    public ServerView()
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
            vm.SelectedQueryText = tb.Selection.Text;
            vm.QueryText = tb.Text;
        }
    }
    #endregion

    #region [__AutoGeneratingColumn]
    private void __AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        if (e.PropertyType == typeof(string))
        {
            var templateColumn = new DataGridTemplateColumn();
            templateColumn.Header = e.Column.Header;

            var cellTemplate = new DataTemplate();
            var textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            var textBinding = new System.Windows.Data.Binding(e.PropertyName);
            textBinding.Converter = new SingleLineTextConverter();
            textBlockFactory.SetBinding(TextBlock.TextProperty, textBinding);
            cellTemplate.VisualTree = textBlockFactory;

            templateColumn.CellTemplate = cellTemplate;

            e.Column = templateColumn;
        }
    }
    #endregion
}

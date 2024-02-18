using DBison.WPF.Controls;
using DBison.WPF.Converter;
using DBison.WPF.ViewModels;
using System.Windows;
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

    private void __AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        // Überprüfen, ob es sich um eine TextColumn handelt
        if (e.PropertyType == typeof(string))
        {
            // Spalte als DataGridTemplateColumn festlegen
            var templateColumn = new DataGridTemplateColumn();
            templateColumn.Header = e.Column.Header;

            // CellTemplate erstellen
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
}

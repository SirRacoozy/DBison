using System.Windows;
using System.Windows.Controls;

namespace DBison.WPF.Controls;
/// <summary>
/// Interaction logic for SingleLineTextBlock.xaml
/// </summary>
public partial class SingleLineTextBlock : UserControl
{
    public static readonly DependencyProperty TextProperty =
           DependencyProperty.Register("Text", typeof(string), typeof(SingleLineTextBlock), new PropertyMetadata(string.Empty, __StaticTextChanged));

    #region [SingleLineTextBlock]
    public SingleLineTextBlock()
    {
        InitializeComponent();
    }
    #endregion

    #region [Text]
    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }
    #endregion

    #region [__StaticTextChanged]
    private static void __StaticTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((SingleLineTextBlock)d).__TextChanged(e);
    }
    #endregion

    #region [__TextChanged]
    private void __TextChanged(DependencyPropertyChangedEventArgs e)
    {
        txtTextBlock.Margin = new Thickness(MainWindowViewModel.ResultCellMargin); //TODO: As setting
        if (e.NewValue == "null")
        {
            txtTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            txtTextBlock.Opacity = 0.5;
        }
        else
        {
            txtTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            txtTextBlock.Opacity = 1;
        }
    }
    #endregion
}
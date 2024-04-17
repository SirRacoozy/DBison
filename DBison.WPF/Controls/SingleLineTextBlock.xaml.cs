using System.Windows;
using System.Windows.Controls;

namespace DBison.WPF.Controls;
/// <summary>
/// Interaction logic for SingleLineTextBlock.xaml
/// </summary>
public partial class SingleLineTextBlock : UserControl
{
    public static readonly DependencyProperty TextProperty =
           DependencyProperty.Register("Text", typeof(string), typeof(SingleLineTextBlock), new PropertyMetadata(string.Empty));


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
}
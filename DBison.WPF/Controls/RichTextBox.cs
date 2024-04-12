using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace DBison.WPF.Controls;
public class RichTextBox : System.Windows.Controls.RichTextBox
{
    private bool m_PreventDocumentUpdate;

    private bool m_PreventTextUpdate;

    #region [RichTextBox]
    public RichTextBox()
    {
        Loaded += __RichTextBox_Loaded;
    }

    public RichTextBox(System.Windows.Documents.FlowDocument document)
      : base(document)
    {
    }
    #endregion

    #region [TextProperty]
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(RichTextBox), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, __OnTextPropertyChanged, __CoerceTextProperty, true, UpdateSourceTrigger.LostFocus));
    #endregion

    #region [TextFormatterProperty]
    public static readonly DependencyProperty TextFormatterProperty = DependencyProperty.Register("TextFormatter", typeof(PlainTextFormatter), typeof(RichTextBox), new FrameworkPropertyMetadata(new PlainTextFormatter(), __OnTextFormatterPropertyChanged));
    #endregion

    #region [Text]
    public string Text
    {
        get
        {
            return (string)GetValue(TextProperty);
        }
        set
        {
            SetValue(TextProperty, value);
        }
    }
    #endregion

    #region [TextFormatter]
    public PlainTextFormatter TextFormatter
    {
        get
        {
            return (PlainTextFormatter)GetValue(TextFormatterProperty);
        }
        set
        {
            SetValue(TextFormatterProperty, value);
        }
    }
    #endregion

    #region [OnTextFormatterPropertyChanged]
    protected virtual void OnTextFormatterPropertyChanged(PlainTextFormatter oldValue, PlainTextFormatter newValue)
    {
        __UpdateTextFromDocument();
    }
    #endregion

    #region [OnTextChanged]
    protected override void OnTextChanged(System.Windows.Controls.TextChangedEventArgs e)
    {
        __UpdateTextFromDocument();
        base.OnTextChanged(e);
    }
    #endregion

    #region [Clear]
    public void Clear()
    {
        Document.Blocks.Clear();
    }
    #endregion

    #region [BeginInit]
    public override void BeginInit()
    {
        base.BeginInit();
        m_PreventTextUpdate = true;
        m_PreventDocumentUpdate = true;
    }
    #endregion

    #region [EndInit]
    public override void EndInit()
    {
        base.EndInit();
        m_PreventTextUpdate = false;
        m_PreventDocumentUpdate = false;
        if (!string.IsNullOrEmpty(Text))
        {
            __UpdateDocumentFromText();
        }
        else
        {
            __UpdateTextFromDocument();
        }
    }
    #endregion

    #region [__OnTextPropertyChanged]
    private static void __OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((RichTextBox)d).__UpdateDocumentFromText();
    }
    #endregion

    #region [__CoerceTextProperty]
    private static object __CoerceTextProperty(DependencyObject d, object value)
    {
        return value ?? "";
    }
    #endregion

    #region [__OnTextFormatterPropertyChanged]
    private static void __OnTextFormatterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RichTextBox richTextBox)
            richTextBox.OnTextFormatterPropertyChanged((PlainTextFormatter)e.OldValue, (PlainTextFormatter)e.NewValue);
    }
    #endregion

    #region [__UpdateTextFromDocument]
    private void __UpdateTextFromDocument()
    {
        if (m_PreventTextUpdate)
            return;

        m_PreventDocumentUpdate = true;
        SetCurrentValue(TextProperty, TextFormatter.GetText(Document));
        m_PreventDocumentUpdate = false;
    }
    #endregion

    #region [__UpdateDocumentFromText]
    private void __UpdateDocumentFromText()
    {
        if (m_PreventDocumentUpdate)
            return;

        m_PreventTextUpdate = true;
        TextFormatter.SetText(Document, Text);
        m_PreventTextUpdate = false;
    }
    #endregion

    #region [__RichTextBox_Loaded]
    private void __RichTextBox_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= __RichTextBox_Loaded;
        InputBindings.Add(new KeyBinding(ApplicationCommands.NotACommand, Key.T,
                                                         ModifierKeys.Control));
        InputBindings.Add(new KeyBinding(ApplicationCommands.NotACommand, Key.R,
                                                         ModifierKeys.Control));
    }
    #endregion

}
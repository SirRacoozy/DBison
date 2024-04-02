using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DBison.WPF.Controls
{
    public class LineNumberedTextBox : RichTextBox
    {
        #region - ctor -
        static LineNumberedTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LineNumberedTextBox), new FrameworkPropertyMetadata(typeof(LineNumberedTextBox)));
        }

        public LineNumberedTextBox()
        {
            Document.Blocks.Clear();
        }
        #endregion

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(LineNumberedTextBox)/*, new PropertyMetadata(string.Empty, __TextChanged)*/);

        #region [OnApplyTemplate]
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            __UpdateLineNumber();
        }
        #endregion

        #region [__UpdateLineNumber]
        private void __UpdateLineNumber()
        {
            if (GetTemplateChild("PART_LineNumberTextBlock") is TextBlock lineNumberTextBlock)
                lineNumberTextBlock.Text = string.Join("\n", Enumerable.Range(1, __GetText().Split('\n').Length));
        }
        #endregion

        #region [OnTextChanged]
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            __UpdateLineNumber();
        }
        #endregion

        #region [__GetText]
        private string __GetText()
        {
            return new TextRange(Document.ContentStart, Document.ContentEnd).Text;
        }
        #endregion

    }
}

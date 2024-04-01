using System.Windows;
using System.Windows.Controls;

namespace DBison.WPF.Controls
{
    public class LineNumberedTextBox : TextBox
    {
        #region - ctor -
        static LineNumberedTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LineNumberedTextBox), new FrameworkPropertyMetadata(typeof(LineNumberedTextBox)));
        }
        #endregion

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
                lineNumberTextBlock.Text = string.Join("\n", Enumerable.Range(1, Text.Split('\n').Length));
        }
        #endregion

        #region [OnTextChanged]
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            __UpdateLineNumber();
        }
        #endregion
    }
}

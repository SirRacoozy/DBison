using System.Windows;
using System.Windows.Controls;

namespace DBison.WPF.Controls
{
    public class __LineNumberedTextBox : TextBox
    {
        #region [__LineNumberedTextBox]
        static __LineNumberedTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(__LineNumberedTextBox), new FrameworkPropertyMetadata(typeof(__LineNumberedTextBox)));
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

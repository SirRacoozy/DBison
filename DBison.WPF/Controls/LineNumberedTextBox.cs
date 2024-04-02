using DBison.Core.Extender;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DBison.WPF.Controls
{
    public class LineNumberedTextBox : RichTextBox
    {
        private bool m_SkipHighliting;

        #region - ctor -
        static LineNumberedTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LineNumberedTextBox), new FrameworkPropertyMetadata(typeof(LineNumberedTextBox)));
        }
        #endregion

        #region [Text]
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(LineNumberedTextBox));
        #endregion

        #region [OnApplyTemplate]
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            __UpdateLineNumber();
        }
        #endregion

        #region [OnTextChanged]
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            if (!m_SkipHighliting)
            {
                __UpdateLineNumber();
                __HighlightSQLKeyWords();
            }
        }
        #endregion

        #region [__UpdateLineNumber]
        private void __UpdateLineNumber()
        {
            if (GetTemplateChild("PART_LineNumberTextBlock") is TextBlock lineNumberTextBlock)
                lineNumberTextBlock.Text = string.Join("\n", Enumerable.Range(1, __GetText().Split(Environment.NewLine, StringSplitOptions.TrimEntries).Length - 1));
        }
        #endregion

        #region [__GetText]
        private string __GetText()
        {
            return new TextRange(Document.ContentStart, Document.ContentEnd).Text;
        }
        #endregion

        #region [__HighlightSQLKeyWords]
        private void __HighlightSQLKeyWords()
        {
            TextRange textRange = new TextRange(Document.ContentStart, Document.ContentEnd);
            m_SkipHighliting = true;
            textRange.ClearAllProperties();
            m_SkipHighliting = false;
            string[] sqlKeywords = new string[]
            {
            "SELECT", "INSERT", "UPDATE", "DELETE", "FROM", "WHERE", "JOIN", "INNER", "OUTER",
            "LEFT", "RIGHT", "ON", "AND", "ORDER", "IN", "NOT", "NULL", "OR", "BY", "GROUP",
            "HAVING", "ASC", "AS", "DISTINCT", "TOP", "INTO", "VALUES", "CREATE", "ALTER", "DROP",
            "TABLE", "INDEX", "PROCEDURE", "FUNCTION", "VIEW", "DATABASE", "USE", "BEGIN",
            "END", "IF", "ELSE", "WHILE", "GO"
            };

            foreach (var keyWord in sqlKeywords)
            {
                __ExecuteHighlight(keyWord, textRange);
            }
        }
        #endregion

        #region [__ExecuteHighlight]
        private void __ExecuteHighlight(string searchText, TextRange textRange)
        {
            var rich = this;
            string textBoxText = textRange.Text;

            if (textBoxText.IsNullOrEmpty())
            {
                return;
            }

            else
            {
                for (TextPointer startPointer = rich.Document.ContentStart; startPointer.CompareTo(rich.Document.ContentEnd) <= 0; startPointer = startPointer?.GetNextContextPosition(LogicalDirection.Forward))
                {
                    if (startPointer == null)
                        break;
                    if (startPointer.CompareTo(rich.Document.ContentEnd) == 0)
                    {
                        break;
                    }

                    string parsedString = startPointer.GetTextInRun(LogicalDirection.Forward);
                    var allIndexesOf = parsedString.AllIndexesOf(searchText, StringComparison.InvariantCultureIgnoreCase);

                    foreach (var indexOf in allIndexesOf)
                    {
                        if (parsedString.IsNullOrEmpty())
                            continue;

                        if (indexOf >= 0)
                        {
                            string charBefore = string.Empty;
                            string charAfter = string.Empty;
                            var charBeforeIndex = indexOf - 1;
                            var charAfterIndex = indexOf + searchText.Length;
                            if (charBeforeIndex != -1)
                                charBefore = parsedString.Substring(charBeforeIndex, 1);
                            if (charAfterIndex < parsedString.Length)
                                charAfter = parsedString.Substring(charAfterIndex, 1);

                            startPointer = startPointer.GetPositionAtOffset(indexOf);

                            if (charBefore.IsNotNullOrEmpty() && charBefore.IsNotEquals(" "))
                                continue;
                            if (charAfter.IsNotNullOrEmpty() && charAfter.IsNotEquals(" "))
                                continue;

                            if (startPointer != null)
                            {
                                TextPointer nextPointer = startPointer.GetPositionAtOffset(searchText.Length);
                                if (nextPointer == null)
                                    continue;
                                TextRange searchedTextRange = new TextRange(startPointer, nextPointer);
                                m_SkipHighliting = true;
                                searchedTextRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Blue));
                                m_SkipHighliting = false;
                            }
                        }
                    }
                }
            }
        }
        #endregion

    }
}
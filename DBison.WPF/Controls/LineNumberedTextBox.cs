using DBison.Core.Extender;
using DBison.Core.Utils.SettingsSystem;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DBison.WPF.Controls
{
    public class LineNumberedTextBox : DBison.WPF.Controls.RichTextBox
    {
        private bool m_SkipHighliting;
        private Popup m_Popup;
        private ListBox m_SuggestionListBox;

        public event EventHandler<object> SuggestionChoosed;

        #region - ctor -
        static LineNumberedTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LineNumberedTextBox), new FrameworkPropertyMetadata(typeof(LineNumberedTextBox)));
        }

        public LineNumberedTextBox()
        {
            TextFormatter = new PlainTextFormatter();
            __AddPopUp();
        }
        #endregion

        #region [OnApplyTemplate]
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            __UpdateLineNumber();
        }
        #endregion

        #region [SuggestionSource]
        public IEnumerable<object> SuggestionSource
        {
            get
            {
                return (IEnumerable<object>)GetValue(SuggestionSourceProperty);
            }
            set
            {
                SetValue(SuggestionSourceProperty, value);
            }
        }

        public static readonly DependencyProperty SuggestionSourceProperty =
                DependencyProperty.Register("SuggestionSource", typeof(IEnumerable<object>), typeof(LineNumberedTextBox), new PropertyMetadata(null, __SuggestionSourceChanged));

        private static void __SuggestionSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LineNumberedTextBox)d).__UpdatePopup();
        }
        #endregion

        #region [SuggestionDisplayMemberPath]
        public string SuggestionDisplayMemberPath
        {
            get => (string)GetValue(SuggestionDisplayMemberPathProperty);
            set => SetValue(SuggestionDisplayMemberPathProperty, value);
        }

        public static readonly DependencyProperty SuggestionDisplayMemberPathProperty =
                DependencyProperty.Register("SuggestionDisplayMemberPath", typeof(string), typeof(LineNumberedTextBox), new PropertyMetadata(null, __SuggestionDisplayMemberPathChanged));

        private static void __SuggestionDisplayMemberPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LineNumberedTextBox)d).__AdjustSuggestionListBoxProperties();
        }
        #endregion

        #region [OnTextChanged]
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            __UpdateLineNumber();
            if (!m_SkipHighliting)
            {
                __HighlightSQLKeyWords();
            }
            __UpdatePopup();
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
            Brush highlightBrush = Settings.UseDarkMode ? (Brush)new BrushConverter().ConvertFrom("#00BBC9") : Brushes.Blue;
            var rich = this;
            string textBoxText = textRange.Text;

            if (textBoxText.IsNullOrEmpty())
            {
                return;
            }

            for (TextPointer startPointer = rich.Document.ContentStart; startPointer?.CompareTo(rich.Document.ContentEnd) <= 0; startPointer = startPointer?.GetNextContextPosition(LogicalDirection.Forward))
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
                            searchedTextRange.ApplyPropertyValue(TextElement.ForegroundProperty, highlightBrush);
                            m_SkipHighliting = false;
                        }
                    }
                }
            }
        }
        #endregion

        #region [__AddPopUp]
        private void __AddPopUp()
        {
            m_Popup = new Popup();
            m_Popup.Placement = PlacementMode.Relative;
            m_Popup.StaysOpen = false;
            m_Popup.AllowsTransparency = true;
            m_Popup.Child = new Border
            {
                Background = System.Windows.Media.Brushes.LightYellow,
                BorderBrush = System.Windows.Media.Brushes.Black,
                BorderThickness = new Thickness(1),
                Child = new ContentPresenter()
            };
        }
        #endregion

        #region [__UpdatePopup]
        private void __UpdatePopup()
        {
            if (!IsLoaded) return;
            if (m_Popup != null)
            {
                m_Popup.DataContext = this.DataContext;
                var border = (Border)m_Popup.Child;
                var contentPresenter = (ContentPresenter)border.Child;
                var PopupContent = SuggestionSource;
                m_SuggestionListBox = new ListBox()
                {
                    ItemsSource = SuggestionSource,
                    SelectedIndex = 0,
                };
                __HandleListBoxEvents();
                contentPresenter.Content = m_SuggestionListBox;
                Rect positionRect = CaretPosition.GetCharacterRect(LogicalDirection.Backward);
                Point point = PointToScreen(positionRect.BottomRight);
                m_Popup.HorizontalOffset = point.X;
                m_Popup.VerticalOffset = point.Y;
                __AdjustSuggestionListBoxProperties();

                if (PopupContent != null)
                {
                    if (!m_Popup.IsOpen)
                    {
                        m_Popup.IsOpen = true;
                    }
                }
                else
                {
                    if (m_Popup.IsOpen)
                    {
                        m_Popup.IsOpen = false;
                    }
                }
                Keyboard.Focus(m_SuggestionListBox);
            }
        }
        #endregion

        #region [__AdjustSuggestionListBoxProperties]
        private void __AdjustSuggestionListBoxProperties()
        {
            if (m_SuggestionListBox == null)
                return;

            m_SuggestionListBox.DisplayMemberPath = SuggestionDisplayMemberPath;
        }
        #endregion

        #region [__HandleListBoxEvents]
        private void __HandleListBoxEvents()
        {
            if (m_SuggestionListBox == null)
                return;
            m_SuggestionListBox.MouseDoubleClick += __SuggestionListBox_MouseDoubleClick;
            m_SuggestionListBox.KeyDown += __SuggestionListBox_KeyUp;
        }
        #endregion

        #region [__SuggestionListBox_KeyUp]
        private void __SuggestionListBox_KeyUp(object sender, KeyEventArgs e)
        {
            //if (Keyboard.Modifiers != ModifierKeys.None)
            //    return;
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                m_SuggestionListBox.KeyDown -= __SuggestionListBox_KeyUp;
                __TakeSuggestionSelection();
            }
            else
            {
                Keyboard.Focus(this);
                return;
            }
        }
        #endregion

        #region [__SuggestionListBox_MouseDoubleClick]
        private void __SuggestionListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            m_SuggestionListBox.MouseDoubleClick -= __SuggestionListBox_MouseDoubleClick;
            __TakeSuggestionSelection();
        }
        #endregion

        #region [__TakeSuggestionSelection]
        private void __TakeSuggestionSelection()
        {
            //SuggestionChoosed?.Invoke(this, m_SuggestionListBox?.SelectedItem);

            //__ReplaceIncompleteWordWithSelection();

            if (m_Popup != null)
                m_Popup.IsOpen = false;
        }
        #endregion

    }
}
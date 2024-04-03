using System.Windows.Documents;

namespace DBison.WPF.Controls;
public class PlainTextFormatter
{
    #region [GetText]
    public string GetText(FlowDocument document)
    {
        return new TextRange(document.ContentStart, document.ContentEnd).Text;
    }
    #endregion

    #region [SetText]
    public void SetText(FlowDocument document, string text)
    {
        new TextRange(document.ContentStart, document.ContentEnd).Text = text;
    }
    #endregion
}
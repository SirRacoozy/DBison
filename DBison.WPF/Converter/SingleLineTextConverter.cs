using DBison.Core.Extender;
using System.Windows.Data;

namespace DBison.WPF.Converter;
public class SingleLineTextConverter : IValueConverter
{
    #region [Convert]
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        string s = value.ToStringValue();
        s = s.Replace(Environment.NewLine, " ");
        s = s.Replace("\n", " ");
        return s;
    }
    #endregion

    #region [ConvertBack]
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
    #endregion
}

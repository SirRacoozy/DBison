using System.Globalization;
using System.Windows.Data;

namespace DBison.WPF.Converter;
public class EmptyValueConverter : IValueConverter
{
    #region [Convert]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == DBNull.Value)
        {
            return "null";
        }
        else
        {
            return value;
        }
    }
    #endregion

    #region [ConvertBack]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
    #endregion
}

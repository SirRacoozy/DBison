using System.Globalization;
using System.Windows.Data;

namespace DBison.WPF.Converter
{
    public class WidthDividerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double width && parameter is string divider)
            {
                return width / System.Convert.ToInt32(divider);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

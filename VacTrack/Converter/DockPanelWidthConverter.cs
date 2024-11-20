using System.Globalization;
using System.Windows.Data;

namespace VacTrack.Converter
{
    public class DockPanelWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double totalWidth)
            {
                // Вычитаем ширину кнопок и других фиксированных элементов
                double reservedWidth = 600; // Задайте ширину кнопок/отступов
                return Math.Max(0, totalWidth - reservedWidth);
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using DatabaseManager;
using System.Globalization;
using System.Windows.Data;

namespace VacTrack.Converter
{
    class CounterpartieTypeConverter : IValueConverter
    {
        const string UrName = "Юр. Лицо";
        const string FizName = "Физ. Лицо";
        private static IEnumerable<string> UrFiz => [UrName, FizName];

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CounterpartieType type)
                return type switch
                {
                    CounterpartieType.Ur => UrName,
                    CounterpartieType.Fiz => FizName,
                    _ => value
                };

            if (value is List<CounterpartieType>)
                return UrFiz;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string type)
                return type switch
                {
                    UrName => CounterpartieType.Ur,
                    FizName => CounterpartieType.Fiz,
                    _ => throw new ArgumentException($"Неизвестный тип контрагента: \"{type}\". Проверьте корректность данных.")
                };
            throw new ArgumentException($"Ожидалась строка, но получен объект типа {value?.GetType().Name ?? "null"}. Проверьте входные данные.");
        }
    }
}

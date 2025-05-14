using System.Globalization;
using System.Windows.Data;
using DatabaseManager;

namespace VacTrack.Converter
{
    internal class ContractStatusConverter : IValueConverter
    {
        const string created = "Создан";
        const string running = "Выполняется";
        const string completed = "Завершен";

        private static IEnumerable<string> Statuses => [created, running, completed];

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ContractStatus type)
                return type switch
                {
                    ContractStatus.created => created,
                    ContractStatus.running => running,
                    ContractStatus.completed => completed,
                    _ => value
                };

            if (value is List<ContractStatus>)
                return Statuses;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string type)
                return type switch
                {
                    created => ContractStatus.created,
                    running => ContractStatus.running,
                    completed => ContractStatus.completed,
                    _ => throw new ArgumentException($"Неизвестный статус: \"{type}\". Проверьте корректность данных.")
                };
            throw new ArgumentException($"Ожидалась строка, но получен объект типа {value?.GetType().Name ?? "null"}. Проверьте входные данные.");
        }
    }
}

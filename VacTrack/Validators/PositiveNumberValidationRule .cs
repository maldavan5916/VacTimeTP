using System.Globalization;
using System.Windows.Controls;

namespace VacTrack.Validators
{
    public class PositiveNumberValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            // Преобразуем входное значение в строку и проверяем, не пустое ли оно
            string? input = (value ?? "").ToString();

            if (string.IsNullOrWhiteSpace(input))
                return new ValidationResult(false, "Значение не может быть пустым.");

            // Пытаемся преобразовать в число
            if (double.TryParse(input, NumberStyles.Float, cultureInfo, out double result))
            {
                // Проверяем, что число строго положительное
                if (result > 0)
                    return ValidationResult.ValidResult;
                else
                    return new ValidationResult(false, "Число должно быть строго положительным.");
            }

            // Если преобразование не удалось
            return new ValidationResult(false, "Некорректное числовое значение.");
        }
    }
}

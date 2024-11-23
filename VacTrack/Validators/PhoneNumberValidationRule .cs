using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace VacTrack.Validators
{
    public class PhoneNumberValidationRule : ValidationRule
    {
        // Список регулярных выражений для проверки номера телефона
        private readonly string[] _patterns =
        {
            @"^\+?[1-9]\d{1,14}$",                 // Международный формат (E.164)
            @"^(?:\+7|8)\d{10}$",                 // Российский номер (+7 или 8)
            @"^\+?\d{1,4}[-\s]?\(?\d{1,3}\)?[-\s]?\d{1,4}[-\s]?\d{1,4}$" // С пробелами и дефисами
        };

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = (value ?? "").ToString();

            if (string.IsNullOrWhiteSpace(input))
            {
                return new ValidationResult(false, "Номер телефона не может быть пустым.");
            }

            foreach (var pattern in _patterns)
            {
                if (Regex.IsMatch(input, pattern))
                {
                    return ValidationResult.ValidResult;
                }
            }

            return new ValidationResult(false, "Введите корректный номер телефона.");
        }
    }
}

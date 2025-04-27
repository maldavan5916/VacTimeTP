using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace VacTrack.Validators
{
    public class PasswordCheck : ValidationRule
    {
        public string? FirstPassword { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var password = value as string ?? string.Empty;

            if (FirstPassword != null && password != FirstPassword)
                return new ValidationResult(false, "Пароли не совпадают.");

            // Проверка на пустоту
            if (string.IsNullOrWhiteSpace(password))
                return new ValidationResult(false, "Пароль не может быть пустым.");

            // Длина пароля
            if (password.Length < 6)
                return new ValidationResult(false, "Пароль должен содержать минимум 6 символов.");

            // Наличие хотя бы одной заглавной буквы
            if (!Regex.IsMatch(password, @"[A-Z]"))
                return new ValidationResult(false, "Пароль должен содержать хотя бы одну заглавную букву.");

            // Наличие хотя бы одной цифры
            if (!Regex.IsMatch(password, @"[0-9]"))
                return new ValidationResult(false, "Пароль должен содержать хотя бы одну цифру.");


            return ValidationResult.ValidResult;
        }
    }
}

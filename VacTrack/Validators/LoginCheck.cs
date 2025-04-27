using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using DatabaseManager;

namespace VacTrack.Validators
{
    class LoginCheck : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var login = value as string ?? string.Empty;

            if (string.IsNullOrWhiteSpace(login))
                return new ValidationResult(false, "Логин не может быть пустым.");

            if (login.Length < 4)
                return new ValidationResult(false, "Логин должен содержать минимум 4 символа.");

            if (!Regex.IsMatch(login, @"^[а-яА-Яa-zA-Z0-9_]+$"))
                return new ValidationResult(false, "Логин должен содержать только буквы, цифры и подчёркивания.");

            using (var db = new DatabaseContext())
                if (db.Users.Any(u => u.Login == login))
                    return new ValidationResult(false, "Такой пользователь уже есть.");


            return ValidationResult.ValidResult;
        }
    }
}

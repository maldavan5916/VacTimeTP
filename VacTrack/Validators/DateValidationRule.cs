using System.Globalization;
using System.Windows.Controls;

namespace VacTrack.Validators
{
    public class DateValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return DateTime.TryParse((value ?? "").ToString(), out _)
                ? ValidationResult.ValidResult
                : new ValidationResult(false, "Неверный формат даты.");
        }
    }
}

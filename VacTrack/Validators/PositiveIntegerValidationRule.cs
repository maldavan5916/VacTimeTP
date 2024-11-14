using System.Globalization;
using System.Windows.Controls;

namespace VacTrack.Validators
{
    public class PositiveIntegerValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (int.TryParse((value ?? "").ToString(), out int result) && result > 0)
                return ValidationResult.ValidResult;
            return new ValidationResult(false, "Должно быть положительным числом.");
        }
    }
}

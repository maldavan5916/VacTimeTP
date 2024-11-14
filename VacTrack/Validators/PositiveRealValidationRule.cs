using System.Globalization;
using System.Windows.Controls;

namespace VacTrack.Validators
{
    public class PositiveRealValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (double.TryParse((value ?? "").ToString(), out double result) && result > 0)
                return ValidationResult.ValidResult;
            return new ValidationResult(false, "Должно быть положительным числом.");
        }
    }
}

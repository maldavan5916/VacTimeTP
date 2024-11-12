using System.Globalization;
using System.Windows.Controls;

namespace VacTrack.Validators
{
    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrWhiteSpace((value ?? "").ToString())
                ? new ValidationResult(false, "Не может быть пустым.")
                : ValidationResult.ValidResult;
        }
    }
}

using System.Globalization;
using System.Windows.Controls;

namespace VacTrack.Validators
{
    public class LengthRangeValidationRule : ValidationRule
    {
        public int MinLength { get; set; } = 0; // Минимальная длина (по умолчанию — без ограничения)
        public int MaxLength { get; set; } = int.MaxValue; // Максимальная длина (по умолчанию — без ограничения)
        public int Length { get; set; } = 0;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = (value ?? "").ToString();

            if (Length == 0)
            {
                if (input.Length < MinLength)
                    return new ValidationResult(false, $"Не может быть менее {MinLength} символов.");

                if (input.Length > MaxLength)
                    return new ValidationResult(false, $"Не может превышать {MaxLength} символов.");
            }
            else
            {
                if (input.Length != Length)
                    return new ValidationResult(false, $"Должно быть ровно {Length} символов.");
            }

            return ValidationResult.ValidResult;
        }
    }
}

using System.Globalization;
using System.Windows.Controls;

namespace prime
{
    public class CommandValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value==null)
                return new ValidationResult(false, "required!");
            return ValidationResult.ValidResult;
        }
    }
}
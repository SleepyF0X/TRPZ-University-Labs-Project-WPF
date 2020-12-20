using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TRPZLabRab.ValidationRules
{
    public sealed class AgeValidationRule : ValidationRule
    {
        public string Parameter { get; set; }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var result = new ValidationResult(true, null);
            if (value.ToString().Equals(string.Empty)) value = 0;
            var inputAge = Convert.ToInt32(value.ToString());
            if (inputAge <18)
                result = new ValidationResult(false, $"Your age must be 18+");
            else if (inputAge>150)
                result = new ValidationResult(false, $"Invalid Age");

            return result;
        }
    }
}

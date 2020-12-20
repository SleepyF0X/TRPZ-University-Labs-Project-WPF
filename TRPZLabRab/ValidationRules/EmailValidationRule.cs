﻿using System;
using System.Globalization;
using System.Net.Mail;
using System.Windows.Controls;

namespace TRPZLabRab.ValidationRules
{
    public sealed class EmailValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var result = new ValidationResult(true, null);
            var inputEmail = (value ?? string.Empty).ToString();
            if (inputEmail == string.Empty)
                result = new ValidationResult(false, "Email is required.");
            else
                try
                {
                    var validEmail = new MailAddress(inputEmail);
                }
                catch (FormatException)
                {
                    result = new ValidationResult(false, "Email is invalid.");
                }
                catch (ArgumentNullException)
                {
                    result = new ValidationResult(false, "Email is invalid.");
                }

            return result;
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Application.Validation.Attributes;
public class BirthdayRangeAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is DateTime date)
        {
            if (date > DateTime.Today || date < DateTime.Today.AddYears(-100))
                return new ValidationResult("Birthday must be within the last 100 years and not in the future.");
        }
        return ValidationResult.Success;
    }
}

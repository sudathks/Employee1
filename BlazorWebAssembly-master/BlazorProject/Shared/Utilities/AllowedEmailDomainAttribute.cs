using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorProject.Shared.Utilities
{
    public class AllowedEmailDomainAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || value.ToString().Length == 0)
            {
                return new ValidationResult("Email Address is required");
            }
            else
            {
                if (value.ToString().IndexOf('@') == -1) 
                {
                    return new ValidationResult("Invalid Email Address");
                }

                string[] strings = value.ToString().Split('@');

                if (strings[1].ToLower() == "pragimatech.com")
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Email domain must be pragimtech.com");
                }
            }
        }
    }
}

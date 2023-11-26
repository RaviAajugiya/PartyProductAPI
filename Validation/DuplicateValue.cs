using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace PartyProductAPI.Validation
{
    public class DuplicateValue : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {

            return ValidationResult.Success;

        }
    }
}

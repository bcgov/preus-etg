using CJG.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace CJG.Web.External.Helpers
{
    public class ValidateNullableInt : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            // Ensure this is an integer.
            if (!value.IsType(typeof(int), typeof(int?)))
                return false;

            if (value.IsNullable())
                return value != null ? base.IsValid(value) : true;
            else
                return base.IsValid(value);
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value != null)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Please enter total training hours to the nearest hour.");
            }
        }
    }
}
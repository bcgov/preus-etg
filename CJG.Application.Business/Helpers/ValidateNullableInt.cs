using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CJG.Application.Business.Helpers
{
    public class ValidateNullableIntAttribute : ValidationAttribute
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
                return base.IsValid(value, context);
            }
        }
    }
}

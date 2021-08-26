using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Web.External.Helpers
{
    /// <summary>
    /// <typeparamref name="ModelStateExtensions"/> static class, provides extension methods for <typeparamref name="ModelStateDictionary"/> objects.
    /// </summary>
    public static class ModelStateExtensions
    {
        public static string GetErrorMessages(this ModelStateDictionary modelState)
        {
            return modelState.GetErrorMessages(Environment.NewLine);
        }

        public static string GetErrorMessages(this ModelStateDictionary modelState, string deliminator)
        {
            return (from item in modelState.Values
                    from error in item.Errors
                    select error?.ErrorMessage).DefaultIfEmpty().Distinct().Aggregate((a, b) => $"{a}{deliminator}{b}");
        }

        public static string GetErrorMessages(this IEnumerable<ValidationResult> validationResults)
        {
            return validationResults.GetErrorMessages(Environment.NewLine);
        }

        public static string GetErrorMessages(this IEnumerable<ValidationResult> validationResults, string deliminator)
        {
            return (from error in validationResults
                    select error?.ErrorMessage).DefaultIfEmpty().Distinct().Aggregate((a, b) => $"{a}{deliminator}{b}");
        }

        /// <summary>
        /// Automatically apply the <typeparamref name="DbEntityValidationException"/> messages to the <typeparamref name="ModelStateDictionary"/>.
        /// </summary>
        /// <param name="modelState"></param>
        /// <param name="ex"></param>
        /// <param name="path"></param>
        public static void UpdateModelState(this ModelStateDictionary modelState, DbEntityValidationException ex, string path = null)
        {
            foreach (var validation in ex.EntityValidationErrors.Where(eve => !eve.IsValid))
            {
                path = String.IsNullOrEmpty(path) ? ObjectContext.GetObjectType(validation.Entry.Entity.GetType()).Name : path;
                foreach (var error in validation.ValidationErrors)
                {
                    modelState.AddModelError(String.IsNullOrEmpty(path) ? error.PropertyName : $"{path}.{error.PropertyName}", error.ErrorMessage);
                }
            }
        }
    }
}
using RazorEngine.Templating;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;
using System.Linq;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="ExceptionExtensions"/> static class, provides extension methods for <typeparamref name="Exception"/> objects.
	/// </summary>
	public static class ExceptionExtensions
	{
		/// <summary>
		/// Extracts all the validation error messages and returns them as a single string.
		/// </summary>
		/// <param name="ex"></param>
		/// <param name="delimiter"></param>
		/// <returns></returns>
		public static string GetValidationMessages(this DbEntityValidationException ex, string delimiter = " ")
		{
			var errors = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
			return errors.Count() == 0 ? ex.Message : errors.Distinct().ToArray().Aggregate((a, b) => $"{a}{delimiter}{b}");
		}

		/// <summary>
		/// Aggregate all the validation error messages into a single error message.
		/// </summary>
		/// <param name="validationResults"></param>
		/// <param name="deliminator"></param>
		/// <returns></returns>
		public static string GetErrorMessages(this IEnumerable<ValidationResult> validationResults, string deliminator = " ")
		{
			return (from error in validationResults
					select error?.ErrorMessage).DefaultIfEmpty().Distinct().Aggregate((a, b) => $"{a}{deliminator}{b}");
		}

		/// <summary>
		/// Aggregate all the compiler errors for the template into a single string.
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="deliminator"></param>
		/// <returns></returns>
		public static string GetErrorMessages(this TemplateCompilationException exception, string deliminator = "<br/>")
		{
			return exception.CompilerErrors.Select(e => e.ErrorText).Aggregate((a, b) => $"{a}{deliminator}{b}");
		}
	}
}

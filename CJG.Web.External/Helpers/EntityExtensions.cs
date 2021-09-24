using System;
using System.Collections.Generic;
using System.Linq;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Core.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CJG.Infrastructure.Identity;
using System.Data.Entity.Validation;

namespace CJG.Web.External.Helpers
{
	public static class EntityExtensions
	{
		public static string GetUserFullName(this User model)
		{
			var fullName = string.Format("{0} {1}", (string.IsNullOrEmpty(model.FirstName) ? string.Empty : model.FirstName), (string.IsNullOrEmpty(model.LastName) ? string.Empty : model.LastName));
			return fullName;
		}

		public static string GetInternalUserFullName(this ApplicationUser user)
		{
			var fullName = string.Format("{0} {1}", (string.IsNullOrEmpty(user.InternalUser.FirstName) ? string.Empty : user.InternalUser.FirstName), (string.IsNullOrEmpty(user.InternalUser.LastName) ? string.Empty : user.InternalUser.LastName));
			return fullName;
		}

		public static IList<KeyValuePair<string, string>> ConvertToKeyValuePairs(this IEnumerable<Region> regions)
		{
			if (regions == null)
				throw new ArgumentNullException(nameof(regions));

			return regions.OrderBy(x => $"{x.RowSequence + 10000}-{x.Name}")
				.Select(x => new KeyValuePair<string, string>(x.Id, x.Name)).ToList();
		}

		public static IList<KeyValuePair<string, string>> ConvertToKeyValuePairs(this IEnumerable<Country> countries)
		{
			if (countries == null)
				throw new ArgumentNullException(nameof(countries));

			return countries.OrderBy(x => $"{x.RowSequence + 10000}-{x.Name}")
				.Select(x => new KeyValuePair<string, string>(x.Id, x.Name)).ToList();
		}

		public static IList<KeyValuePair<TKeyType, string>> ConvertToKeyValuePairs<TKeyType>(this IEnumerable<LookupTable<TKeyType>> lookupTable)
		{
			if (lookupTable == null)
				throw new ArgumentNullException(nameof(lookupTable));

			return lookupTable.OrderBy(x => $"{x.RowSequence + 10000}-{x.Caption}")
				.Select(x => new KeyValuePair<TKeyType, string>(x.Id, x.Caption)).ToList();
		}

		public static string FormatCodeAndDescription(this NaIndustryClassificationSystem naics)
		{
			return naics?.ToString() ?? string.Empty;
		}

		/// <summary>
		/// Get the name of the file, which will be the course name or the program description depending on the program type.
		/// </summary>
		/// <param name="application"></param>
		/// <returns></returns>
		public static string GetFileName(this GrantApplication application)
		{
			switch (application.GetProgramType())
			{
				case (ProgramTypes.WDAService):
					return application.ProgramDescription?.Description ?? "Program Name";
				case (ProgramTypes.EmployerGrant):
					return application.TrainingPrograms.FirstOrDefault()?.CourseTitle ?? "Training Program Name";
				default:
					return "";
			}
		}

		/// <summary>
		/// Check if the specified entity property values are valid.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="service"></param>
		/// <param name="entity"></param>
		/// <returns></returns>
		public static bool IsValid<TEntity>(this IService service, TEntity entity) where TEntity : class, IValidatableObject
		{
			var results = service.Validate(entity);
			return results.Count() == 0;
		}

		/// <summary>
		/// Get a collection of validation results for the specified entity.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="service"></param>
		/// <param name="entity"></param>
		/// <returns></returns>
		public static IEnumerable<ValidationResult> Validate<TEntity>(this IService service, TEntity entity) where TEntity : class, IValidatableObject
		{
			var context = new ValidationContext(entity);
			return service.Validate(entity);
		}

		/// <summary>
		/// Get a collection of validation results for the specifieid entity and also update the ModelStateDictionary.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="service"></param>
		/// <param name="entity"></param>
		/// <param name="modelState"></param>
		/// <returns></returns>
		public static IEnumerable<ValidationResult> Validate<TEntity>(this IService service, TEntity entity, ModelStateDictionary modelState) where TEntity : class
		{
			var result = service.Validate<TEntity>(entity);
			result.ForEach(vr => vr.MemberNames.ForEach(mn => modelState.AddModelError(mn, vr.ErrorMessage)));
			return result;
		}

		/// <summary>
		/// Convert the specified source into the specified type (TEntity), and update the ModelStateDictionary.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="service"></param>
		/// <param name="source"></param>
		/// <param name="modelState"></param>
		/// <param name="ignoreCase"></param>
		/// <returns></returns>
		public static TEntity ConvertAndValidate<TEntity>(this IService service, object source, ModelStateDictionary modelState, bool ignoreCase = false) where TEntity : class
		{
			var validationResults = new List<ValidationResult>();
			var result = service.ConvertAndValidate<TEntity>(source, validationResults, ignoreCase);

			validationResults.ForEach(vr => vr.MemberNames.ForEach(mn => modelState.AddModelError(mn, vr.ErrorMessage)));
			return result;
		}

		/// <summary>
		/// Copy the source property values into the specified result, and update the ModelStateDictionary.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="service"></param>
		/// <param name="source"></param>
		/// <param name="result"></param>
		/// <param name="modelState"></param>
		/// <param name="ignoreCase"></param>
		public static void ConvertAndValidate<TEntity>(this IService service, object source, TEntity result, ModelStateDictionary modelState, bool ignoreCase = false) where TEntity : class
		{
			var validationResults = new List<ValidationResult>();
			service.ConvertAndValidate<TEntity>(source, result, validationResults, ignoreCase);

			validationResults.ForEach(vr => vr.MemberNames.ForEach(mn => modelState.AddModelError(mn, vr.ErrorMessage)));
		}

		/// <summary>
		/// Convert the specified source into the specified type (TEntity), and update the ModelStateDictionary.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="service"></param>
		/// <param name="source"></param>
		/// <param name="modelState"></param>
		/// <param name="ignoreCase"></param>
		/// <returns></returns>
		public static TEntity ConvertAndValidate<TEntity>(this IService service, object source, bool ignoreCase = false) where TEntity : class
		{
			var validationResults = new List<ValidationResult>();
			var result = service.ConvertAndValidate<TEntity>(source, validationResults, ignoreCase);

			if (validationResults.Count() > 0)
				throw new DbEntityValidationException(validationResults.GetErrorMessages());

			return result;
		}

		public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> source)
		{
			return source ?? Enumerable.Empty<T>();
		}
	}
}
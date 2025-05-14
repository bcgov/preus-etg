using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using CJG.Core.Entities;

namespace CJG.Application.Services
{
	public static class Utilities
	{
		public static Tuple<string, bool, Attachment> UploadPostedFile(HttpPostedFileBase applicationDocument, string documentType)
		{
			int maxUploadSize = int.Parse(ConfigurationManager.AppSettings["MaxUploadSizeInBytes"]);
			string[] permittedAttachmentTypes = ConfigurationManager.AppSettings["PermittedAttachmentTypes"].Split('|');
			var invalidRequest = false;
			var message = string.Empty;
			var applicationDocumentFile = new Attachment();

			if (applicationDocument.ContentLength < maxUploadSize)
			{
				var fileName = Path.GetFileNameWithoutExtension(applicationDocument.FileName);
				var fileExtension = Path.GetExtension(applicationDocument.FileName);

				if (permittedAttachmentTypes.Contains(fileExtension))
				{
					applicationDocumentFile.FileName = fileName;
					applicationDocumentFile.FileExtension = fileExtension;

					using (var memoryStream = new MemoryStream())
					{
						applicationDocument.InputStream.CopyTo(memoryStream);
						applicationDocumentFile.AttachmentData = memoryStream.ToArray();
					}
				}
				else
				{
					message = "Please attach your Proof of Instructor Qualifications, Course Outline or Training Business Case in one of the following formats: " + string.Join(", ", permittedAttachmentTypes);
					invalidRequest = true;
				}
			}
			else
			{
				message = "Maximum attachment size is 5MB";
				invalidRequest = true;
			}

			return Tuple.Create(message, invalidRequest, applicationDocumentFile);
		}

		public static bool IsValidTrainingDates(DateTime newStartDate, DateTime newEndDate, DateTime trainingPeriodStartDate, DateTime trainingPeriodEndDate)
		{
			return newStartDate.WithinRange(trainingPeriodStartDate, trainingPeriodEndDate)
			       && newEndDate.WithinRange(trainingPeriodStartDate, trainingPeriodEndDate)
			       && newStartDate > AppDateTime.UtcNow.Date && (trainingPeriodEndDate >= trainingPeriodStartDate)
			       && newEndDate <= newStartDate.AddYears(1);
		}

		public static bool WithinRange(this DateTime checkDate, DateTime startDate, DateTime endDate)
		{
			return checkDate >= startDate && checkDate <= endDate;
		}

		public static IOrderedQueryable<TSource> Order<TSource>(this IQueryable<TSource> source, string propertyName, bool ascending)
		{
			var queryElementTypeParam = Expression.Parameter(typeof(TSource));
			var memberAccess = Expression.PropertyOrField(queryElementTypeParam, propertyName);
			var keySelector = Expression.Lambda(memberAccess, queryElementTypeParam);
			var orderBy = Expression.Call(typeof(Queryable), ascending ? "OrderBy" : "OrderByDescending", new[] { typeof(TSource), memberAccess.Type }, source.Expression, Expression.Quote(keySelector));

			return (IOrderedQueryable<TSource>)source.Provider.CreateQuery<TSource>(orderBy);
		}

		public static IOrderedQueryable<TSource> OrderByDynamic<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> member, bool ascending = true) where TSource : new()
		{
			return ascending ? source.OrderBy(member) : source.OrderByDescending(member);

		}

		/// <summary>
		/// Get all possible combinations 
		/// </summary>
		/// <typeparam name="T">Type of element</typeparam>
		/// <param name="items">Original list of items that needs to be combined</param>
		/// <param name="count">Number of elements in one combination</param>
		/// <returns>Collection of combined elements</returns>
		public static IEnumerable<IEnumerable<T>> GetCombinations<T>(this IList<T> items, int count)
		{
			var i = 0;

			foreach (var item in items.ToList())
			{
				if (count == 1)
				{
					yield return new[] { item };
				}
				else
				{
					foreach (var result in GetCombinations(items.Skip(i + 1).ToList(), count - 1))
						yield return new[] { item }.Concat(result);
				}

				++i;
			}
		}

		/// <summary>
		/// Format Canadian Postal code - add space between 2 code segments  & convert to upper case.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string FormatCanadianPostalCode(string value)
		{
			var postalcode = value;
			if (!string.IsNullOrEmpty(postalcode))
			{
				postalcode = postalcode.ToUpper().Replace(" ", "");
				if (postalcode.Length == 6) postalcode = postalcode.Insert(postalcode.Length - 3, " ");
				else postalcode = value; //no change
			}
			return postalcode;
		}

		public static StringBuilder ParseTemplate(IDictionary<string, string> placeholders, string templateText, bool verifyEmptyValues = false)
		{
			var str = new StringBuilder(templateText);
			var regex = new Regex(@"(\:\:.+?\:\:)", RegexOptions.IgnoreCase | RegexOptions.Multiline);

			foreach (Match match in regex.Matches(templateText))
			{
				var placeholderKey = match.Value.Replace("::", string.Empty);

				if (!placeholders.ContainsKey(placeholderKey))
				{
					throw new ApplicationException($"Template placeholder {match} is not supported");
				}

				var placeholderValue = placeholders[placeholderKey];

				if (verifyEmptyValues && string.IsNullOrEmpty(placeholderValue))
				{
					throw new ApplicationException($"Value for template placeholder {match.Value} is empty");
				}

				str.Replace(match.Value, placeholderValue);
			}
			return str;
		}

		/// <summary>
		/// Shallow copy value properties from one object to another based on matching property names.
		/// By default will attempt to convert byte[] into string and vise-versa.
		/// If the property is an object it will only copy if it is the same type.
		/// </summary>
		/// <typeparam name="From"></typeparam>
		/// <typeparam name="To"></typeparam>
		/// <param name="source"></param>
		/// <param name="target"></param>
		/// <param name="excludeItems"></param>
		public static void MapProperties<From, To>(From source, To target, Func<From, object> excludeItems = null)
		{
			if (source == null)
				return;

			if (target == null)
				throw new ArgumentNullException(nameof(target));

			var excludedProperties = excludeItems?.Invoke(source).GetType().GetProperties().Select(o => o.Name).ToList();
			var sourceProperties = typeof(From).GetProperties().Where(x => x.CanRead && (excludedProperties == null || !excludedProperties.Contains(x.Name))).ToList();
			MapProperties(source, target, sourceProperties);
		}

		/// <summary>
		/// Shallow copy value properties from one object to another based on matching property names.
		/// By default will attempt to convert byte[] into string and vise-versa.
		/// If the property is an object it will only copy if it is the same type.
		/// </summary>
		/// <typeparam name="From"></typeparam>
		/// <typeparam name="To"></typeparam>
		/// <param name="source"></param>
		/// <param name="target"></param>
		public static void MapProperties<From, To>(From source, To target) where To : new()
		{
			if (source == null)
				return;

			if (target == null)
				throw new ArgumentNullException(nameof(target));

			var sourceProperties = source.GetType().GetProperties().Where(p => p.CanRead && !p.GetGetMethod().IsVirtual);
			MapProperties(source, target, sourceProperties);
		}

		/// <summary>
		/// Shallow copy value properties from one object to another based on matching property names.
		/// By default will attempt to convert byte[] into string and vise-versa.
		/// If the property is an object it will only copy if it is the same type.
		/// </summary>
		/// <typeparam name="From"></typeparam>
		/// <typeparam name="To"></typeparam>
		/// <param name="source"></param>
		/// <param name="target"></param>
		/// <param name="properties"></param>
		private static void MapProperties<From, To>(From source, To target, IEnumerable<PropertyInfo> properties)
		{
			var targetProperties = typeof(To)
				.GetProperties()
				.Where(p => p.CanWrite && !p.GetGetMethod().IsVirtual)
				.ToList();
			
			foreach (var from in properties)
			{
				var to = targetProperties.FirstOrDefault(p => p.Name.Equals(from.Name));
				if (to == null)
					continue;

				object value = from.GetValue(source);
					
				if (value != null && from.PropertyType.IsArray && from.PropertyType.GetElementType() == typeof(byte) && to.PropertyType == typeof(string))
				{
					to.SetValue(target, Convert.ToBase64String((byte[])value));
				}
				else if (value != null && from.PropertyType == typeof(string) && to.PropertyType.IsArray && to.PropertyType.GetElementType() == typeof(byte))
				{
					to.SetValue(target, Convert.FromBase64String((string)value));
				}
				else if (from.PropertyType == typeof(string) && to.PropertyType.IsEnum)
				{
					to.SetValue(target, Enum.Parse(to.PropertyType, (string)value));
				}
				else if (from.PropertyType.IsEnum && to.PropertyType == typeof(string))
				{
					to.SetValue(target, Enum.GetName(from.PropertyType, value));
				}
				else if (from.PropertyType.IsValueType || from.PropertyType.IsEnum || from.PropertyType == typeof(string) || from.PropertyType == to.PropertyType)
				{
					to.SetValue(target, value);
				}
			}
		}

		public static IEnumerable<KeyValuePair<string, string>> GetErrors(DbEntityValidationException e)
		{
			return e.EntityValidationErrors.SelectMany(t => t.ValidationErrors).Select(t => new KeyValuePair<string, string>(t.PropertyName, t.ErrorMessage));
		}

		public static List<KeyValuePair<string, string>> GetErrors<T>(DbEntityValidationException e, T target)
		{
			var errors = GetErrors(e);
			var result = new List<KeyValuePair<string, string>>();

			foreach (var error in errors)
			{
				if (!string.IsNullOrWhiteSpace(error.Key))
				{
					var property = typeof(T).GetProperty(error.Key);

					if (property == null)
					{
						var properties = target.GetType().GetProperties().Where(t => !t.GetType().IsValueType
							&& t.PropertyType != typeof(string)
							&& t.PropertyType != typeof(bool));

						foreach (var item in properties)
						{
							var attr = item.GetValue(target, null);
							if (attr != null)
								property = attr.GetType().GetProperty(error.Key);
							if (property != null)
								break;
						}
					}

					var errorAdded = false;
					if (property != null)
					{
						var displayName = property.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;

						if (displayName != null)
						{
							result.Add(new KeyValuePair<string, string>(error.Key, error.Value.Replace(error.Key, displayName.Name)));
							errorAdded = true;
						}
					}

					if (!errorAdded)
					{
						result.Add(error);
					}
				}
				else
				{
					result.Add(error);
				}
			}

			return result;
		}

		public static Dictionary<string, object> GetPropertyValues<T>(T source, Func<T, object> properties = null)
		{
			if (source == null)
				return null;

			var values = new Dictionary<string, object>();

			var includedProperties = properties?.Invoke(source)
				.GetType()
				.GetProperties()
				.Select(o => o.Name)
				.ToList();

			var sourceProperties = typeof(T).GetProperties()
				.Where(x => x.CanRead && (includedProperties == null || includedProperties.Contains(x.Name)))
				.ToList();

			foreach (var property in sourceProperties)
				if (!property.GetGetMethod().IsVirtual)
					values.Add(property.Name, property.GetValue(source, null));

			return values;
		}

		public static bool HasPropertyChanges<T>(T source, Dictionary<string, object> values)
		{
			if (source == null || values == null)
				return false;

			var sourceProperties = typeof(T).GetProperties()
				.Where(t => !t.GetGetMethod().IsVirtual)
				.ToList();

			foreach (var property in sourceProperties)
				if (values.ContainsKey(property.Name))
					if (!(property.GetValue(source, null) == null && values[property.Name] == null) &&
						property.GetValue(source, null)?.Equals(values[property.Name]) != true)
						return true;

			return false;
		}
	}
}

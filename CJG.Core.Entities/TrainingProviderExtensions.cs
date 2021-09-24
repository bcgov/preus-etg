using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Identity;

namespace CJG.Core.Entities
{
	/// <summary>
	/// TrainingProviderExtensions static class, provides extension methods for training providers.
	/// </summary>
	public static class TrainingProviderExtensions
	{
		/// <summary>
		/// Get the grant application for this training provider.
		/// </summary>
		/// <param name="trainingProvider"></param>
		/// <returns></returns>
		public static GrantApplication GetGrantApplication(this TrainingProvider trainingProvider)
		{
			return trainingProvider.GrantApplication ?? trainingProvider.TrainingProgram?.GrantApplication;
		}

		/// <summary>
		/// Get the prior approved training provider for the specified training provider.
		/// Returns null if there is no prior approved training provider.
		/// </summary>
		/// <param name="trainingProvider"></param>
		/// <returns></returns>
		public static TrainingProvider GetPriorApproved(this TrainingProvider trainingProvider)
		{
			return trainingProvider?.OriginalTrainingProvider.RequestedTrainingProviders
				.Where(tp => tp.TrainingProviderState == TrainingProviderStates.Complete && tp.TrainingProviderInventoryId != null && tp.DateAdded < trainingProvider.DateAdded)
				.OrderByDescending(tp => tp.DateAdded)
				.FirstOrDefault() ?? trainingProvider.OriginalTrainingProvider;
		}

		/// <summary>
		/// Get the prior requested training provider for the specified training provider.
		/// Returns null if there is no prior requested training provider.
		/// </summary>
		/// <param name="trainingProvider"></param>
		/// <returns></returns>
		public static TrainingProvider GetPrior(this TrainingProvider trainingProvider)
		{
			return trainingProvider.OriginalTrainingProvider?.RequestedTrainingProviders // Start with the original provider.
				.Where(tp => tp.Id != trainingProvider.Id && tp.TrainingProviderState.In(TrainingProviderStates.Complete, TrainingProviderStates.Denied) && tp.TrainingProviderInventoryId != null)
				.OrderByDescending(tp => tp.DateAdded)
				.FirstOrDefault()
				?? trainingProvider.RequestedTrainingProviders // If the specified training provider is the original.
				.Where(tp => tp.Id != trainingProvider.Id && tp.TrainingProviderState.In(TrainingProviderStates.Complete, TrainingProviderStates.Denied) && tp.TrainingProviderInventoryId != null)
				.OrderByDescending(tp => tp.DateAdded)
				.FirstOrDefault()
				?? trainingProvider.OriginalTrainingProvider; // Default to the original provider.
		}

		/// <summary>
		/// Check if the training provider type is a private sector and required documents.
		/// Some training provider types are for the private sector and they require additional support documentation.
		/// If the training provider type validation is date specific it means that before the specified date those training provider types don't require additional support documentation.
		/// </summary>
		/// <param name="trainingProvider"></param>
		/// <param name="checkPrivateSectorsOn"></param>
		/// <returns></returns>
		public static bool IsPrivateSectorType(this TrainingProvider trainingProvider, DateTime? checkPrivateSectorsOn = null)
		{
			var trainingProviderType = trainingProvider.TrainingProviderType ?? throw new ArgumentNullException($"{trainingProvider}.{nameof(TrainingProviderType)}");
			var grantApplication = trainingProvider.GetGrantApplication();
			return trainingProviderType.PrivateSectorValidationType.IsPrivateSectorType(grantApplication?.DateSubmitted, checkPrivateSectorsOn);
		}

		/// <summary>
		/// Check if the training provider validation type is a private sector and required documents.
		/// Some training provider types are for the private sector and they require additional support documentation.
		/// If the training provider type validation is date specific it means that before the specified date those training provider types don't require additional support documentation.
		/// </summary>
		/// <param name="validationType"></param>
		/// <param name="applicationSubmitted"></param>
		/// <param name="checkPrivateSectorsOn"></param>
		/// <returns></returns>
		public static bool IsPrivateSectorType(this TrainingProviderPrivateSectorValidationTypes validationType, DateTime? applicationSubmitted, DateTime? checkPrivateSectorsOn = null)
		{
			return validationType == TrainingProviderPrivateSectorValidationTypes.Always
				|| (validationType == TrainingProviderPrivateSectorValidationTypes.ByDateSetting
					&& (applicationSubmitted == null || applicationSubmitted.Value.ToLocalTime().Date <= (checkPrivateSectorsOn?.ToLocalTime().Date ?? AppDateTime.UtcNow)));
		}

		/// <summary>
		/// Determine if the training provider has been validated.
		/// </summary>
		/// <param name="trainingProvider"></param>
		/// <returns></returns>
		public static bool IsValidated(this TrainingProvider trainingProvider)
		{
			return trainingProvider.TrainingProviderInventoryId.HasValue;
		}

		/// <summary>
		/// Get the value differences between a Training Provider and it's Original. Meant to be used to get the difference between a TP and a change request TP.
		/// </summary>
		/// <param name="trainingProvider"></param>
		/// <returns></returns>
		public static List<string> GetChangeRequestDifferences(this TrainingProvider trainingProvider)
		{
			var differences = new List<string>();

			var originalProvider = trainingProvider.OriginalTrainingProvider;
			if (originalProvider == null)
				return differences;

			var fieldsToCheck = new List<string>
			{
				nameof(TrainingProvider.Name),
				nameof(TrainingProvider.ContactFirstName),
				nameof(TrainingProvider.ContactLastName),
				nameof(TrainingProvider.ContactEmail),
				nameof(TrainingProvider.ContactPhoneNumber),
				nameof(TrainingProvider.ContactPhoneExtension),
			};

			var addressFieldsToCheck = new List<string>
			{
				nameof(ApplicationAddress.AddressLine1),
				nameof(ApplicationAddress.AddressLine2),
				nameof(ApplicationAddress.City),
				nameof(ApplicationAddress.PostalCode)
			};

			differences.Add(CreateChangeLine(nameof(TrainingProvider.TrainingProviderType.Caption), originalProvider.TrainingProviderType, trainingProvider.TrainingProviderType, "Training Provider Type"));

			differences.AddRange(fieldsToCheck.Select(field => CreateChangeLine(field, originalProvider, trainingProvider)));

			if (originalProvider.TrainingAddress != null && trainingProvider.TrainingAddress != null)
			{
				differences.AddRange(addressFieldsToCheck.Select(field => CreateChangeLine(field, originalProvider.TrainingAddress, trainingProvider.TrainingAddress)));
				differences.Add(CreateChangeLine(nameof(Region.Name), originalProvider.TrainingAddress.Region, trainingProvider.TrainingAddress.Region, "Region"));
				differences.Add(CreateChangeLine(nameof(Country.Name), originalProvider.TrainingAddress.Country, trainingProvider.TrainingAddress.Country, "Country"));
			}

			if (originalProvider.TrainingProviderAddress != null && trainingProvider.TrainingProviderAddress != null)
			{
				differences.AddRange(addressFieldsToCheck.Select(field => CreateChangeLine(field, originalProvider.TrainingProviderAddress, trainingProvider.TrainingProviderAddress)));
				differences.Add(CreateChangeLine(nameof(Region.Name), originalProvider.TrainingProviderAddress.Region, trainingProvider.TrainingProviderAddress.Region, "Region"));
				differences.Add(CreateChangeLine(nameof(Country.Name), originalProvider.TrainingProviderAddress.Country, trainingProvider.TrainingProviderAddress.Country, "Country"));
			}

			if (originalProvider.BusinessCaseDocument != null || trainingProvider.BusinessCaseDocument != null)
			{
				differences.Add(CreateChangeLine(nameof(Attachment.FileName), originalProvider.BusinessCaseDocument, trainingProvider.BusinessCaseDocument, "Business Case File Name"));
				differences.Add(CreateChangeLine(nameof(Attachment.FileExtension), originalProvider.BusinessCaseDocument, trainingProvider.BusinessCaseDocument, "Business Case File Extension"));
				differences.Add(CreateChangeLine(nameof(Attachment.Description), originalProvider.BusinessCaseDocument, trainingProvider.BusinessCaseDocument, "Business Case File Description"));
			}

			if (originalProvider.CourseOutlineDocument != null || trainingProvider.CourseOutlineDocument != null)
			{
				differences.Add(CreateChangeLine(nameof(Attachment.FileName), originalProvider.CourseOutlineDocument, trainingProvider.CourseOutlineDocument, "Course Outline File Name"));
				differences.Add(CreateChangeLine(nameof(Attachment.FileExtension), originalProvider.CourseOutlineDocument, trainingProvider.CourseOutlineDocument, "Course Outline File Extension"));
				differences.Add(CreateChangeLine(nameof(Attachment.Description), originalProvider.CourseOutlineDocument, trainingProvider.CourseOutlineDocument, "Course Outline File Description"));
			}

			if (originalProvider.ProofOfQualificationsDocument != null || trainingProvider.ProofOfQualificationsDocument != null)
			{
				differences.Add(CreateChangeLine(nameof(Attachment.FileName), originalProvider.ProofOfQualificationsDocument, trainingProvider.ProofOfQualificationsDocument, "Proof of Qualifications File Name"));
				differences.Add(CreateChangeLine(nameof(Attachment.FileExtension), originalProvider.ProofOfQualificationsDocument, trainingProvider.ProofOfQualificationsDocument, "Proof of Qualifications File Extension"));
				differences.Add(CreateChangeLine(nameof(Attachment.Description), originalProvider.ProofOfQualificationsDocument, trainingProvider.ProofOfQualificationsDocument, "Proof of Qualifications File Description"));
			}

			return differences
				.Where(d => d != null)
				.ToList();
		}

		private static string CreateChangeLine(string field, object originalVersion, object newVersion, string fieldNameOverride = null)
		{
			if (originalVersion == null || newVersion == null)
				return null;

			var from = originalVersion.GetPropertyValue<string>(field) ?? string.Empty;
			var to = newVersion.GetPropertyValue<string>(field) ?? string.Empty;

			return from != to ? $"{fieldNameOverride ?? field}: changed from '{from}' to '{to}'" : null;
		}

		private static string CreateChangeLine(string field, Attachment originalVersion, Attachment newVersion, string fieldNameOverride = null)
		{
			if (originalVersion == null && newVersion == null)
				return null;

			if (originalVersion == null)
				originalVersion = new Attachment();

			if (newVersion == null)
				newVersion = new Attachment();

			var from = originalVersion.GetPropertyValue<string>(field) ?? string.Empty;
			var to = newVersion.GetPropertyValue<string>(field) ?? string.Empty;

			return from != to ? $"{fieldNameOverride ?? field}: changed from '{from}' to '{to}'" : null;
		}

		private static TResult GetPropertyValue<TResult>(this object t, string propertyName)
		{
			object val = t.GetType().GetProperties().Single(pi => pi.Name == propertyName).GetValue(t, null);
			return (TResult)val;
		}
	}
}

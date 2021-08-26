using System;
using System.Linq;

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
	}
}

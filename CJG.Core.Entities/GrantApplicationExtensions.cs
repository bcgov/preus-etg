using System;
using System.Collections.Generic;
using System.Linq;
using CJG.Core.Entities.Helpers;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="GrantApplicationExtensions"/> static class, provides extension methods for GrantApplication object.
	/// </summary>
	public static class GrantApplicationExtensions
	{
		/// <summary>
		/// Determine if the specified service type requires eligible components.
		/// This is used to ensure that at least the minimum skills training components are eligible.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static bool RequiresEligibleServiceComponents(this GrantApplication grantApplication)
		{
			var services = grantApplication
				.TrainingCost
				.EligibleCosts
				.Where(ec => ec.EligibleExpenseType.ServiceCategory?.ServiceTypeId.In(ServiceTypes.SkillsTraining, ServiceTypes.EmploymentServicesAndSupports) ?? false)
				.ToList();

			if (!services.Any())
				return false;

			return services.Any(ec => ec.Breakdowns.Count(b => b.IsEligible) < (ec.EligibleExpenseType.ServiceCategory?.ServiceTypeId == ServiceTypes.SkillsTraining ? ec.EligibleExpenseType.ServiceCategory?.MinPrograms : ec.EligibleExpenseType.MinProviders));
		}

		/// <summary>
		/// Change the state of the application to incomplete.
		/// Reset the file number.
		/// Reset the assessor.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns>Whether the application state was changed.</returns>
		public static bool MarkWithdrawnAndReturnedApplicationAsIncomplete(this GrantApplication grantApplication)
		{
			if ((grantApplication.ApplicationStateExternal == ApplicationStateExternal.ApplicationWithdrawn
			  && grantApplication.ApplicationStateInternal == ApplicationStateInternal.ApplicationWithdrawn)
			  || grantApplication.ApplicationStateExternal == ApplicationStateExternal.NotAccepted)
			{
				// This GrantApplication was withdrawn or returned as not accepted, and can now be edited for resubmission
				grantApplication.ApplicationStateInternal = ApplicationStateInternal.Draft;
				grantApplication.ApplicationStateExternal = ApplicationStateExternal.Incomplete;

				// remove the file number
				grantApplication.FileNumber = string.Empty;

				// also remove the assigned assessor
				grantApplication.Assessor = null;
				return true;
			}
			return false;
		}

		public static bool RequiresCIPSValidation(this GrantApplication grantApplication)
		{
			return grantApplication.TrainingPrograms.Any(p => p.CipsCode == null);
		}

		public static bool AllowReprioritization(this GrantApplication grantApplication)
		{
			var validStates = new List<ApplicationStateInternal>
			{
				ApplicationStateInternal.New,
				ApplicationStateInternal.PendingAssessment
			};

			return validStates.Contains(grantApplication.ApplicationStateInternal);
		}

		/// <summary>
		/// Determine whether any training providers require validation.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static bool RequiresTrainingProviderValidation(this GrantApplication grantApplication)
		{
			var isInValidWorkflowState = grantApplication.ApplicationStateInternal.IsValidWorkflowTrigger(ApplicationWorkflowTrigger.ValidateTrainingProvider);

			var allProvidersValidated = grantApplication.TrainingPrograms
				.SelectMany(tp => tp.TrainingProviders)
				.Union(grantApplication.TrainingProviders)
				.All(tp => tp.TrainingProviderInventoryId != null);

			return isInValidWorkflowState && !allProvidersValidated;
		}

		/// <summary>
		/// Determines whether the number of agreed participants in Training Costs matches the number of Approved Participants in the participants list
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static bool RequiresNumParticipantsMatchNumApprovedParticipants(this GrantApplication grantApplication)
		{
			return grantApplication.RequireAllParticipantsBeforeSubmission && 
				   grantApplication.TrainingCost.AgreedParticipants != grantApplication.ParticipantForms.Count(p => p.Approved.HasValue && p.Approved.Value);
		}

		public static bool HasRequiredParticipants(this GrantApplication grantApplication)
		{
			var totalParticipants = grantApplication.ParticipantForms.Count;
			return totalParticipants >= grantApplication.TrainingCost.EstimatedParticipants;
		}

		/// <summary>
		/// Checks to see if there are any change requests.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static bool HasChangeRequest(this GrantApplication grantApplication)
		{
			return grantApplication.TrainingPrograms.Any(p => p.RequestedTrainingProvider != null)
				|| grantApplication.TrainingProviders.Any(tp => tp.RequestedTrainingProvider != null);
		}

		/// <summary>
		/// Determine if the grant application is from a WDA services program.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static bool IsWDAService(this GrantApplication grantApplication)
		{
			return grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.WDAService;
		}

		/// <summary>
		/// Get the original training providers for the specified grant application.
		/// This returns all training providers that were in the original application.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static IEnumerable<TrainingProvider> GetOriginalTrainingProviders(this GrantApplication grantApplication)
		{
			var programs = grantApplication.TrainingPrograms
				.SelectMany(p => p.TrainingProviders.Where(tp => tp.TrainingProviderState == TrainingProviderStates.Complete
				                                                 && tp.OriginalTrainingProviderId == null))
				.ToList();
			return programs.Distinct();
		}

		/// <summary>
		/// Get all of the currently approved training providers.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static IEnumerable<TrainingProvider> GetApprovedTrainingProviders(this GrantApplication grantApplication)
		{
			var original = grantApplication.GetOriginalTrainingProviders();

			return original.Select(tp => tp.GetPriorApproved() ?? tp);
		}

		/// <summary>
		/// Get the currently requested training providers.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static IEnumerable<TrainingProvider> GetChangeRequests(this GrantApplication grantApplication)
		{
			var original = grantApplication.GetOriginalTrainingProviders();

			return original.Select(tp => tp.RequestedTrainingProvider).Where(tp => tp != null);
		}

		/// <summary>
		/// Get the previous requested training providers.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static IEnumerable<TrainingProvider> GetPreviousChangeRequest(this GrantApplication grantApplication)
		{
			var original = grantApplication.GetOriginalTrainingProviders();
			var prior = original.Select(tp => tp.GetPrior()).Where(tp => tp != null).ToList();

			if (!prior.Any())
				return new List<TrainingProvider>();

			// Only want the requested training providers in the last request date.
			var lastRequestDate = prior.Max(tp => tp.DateAdded);
			return prior.Where(tp => lastRequestDate.Subtract(tp.DateAdded).TotalMinutes < 1);
		}

		/// <summary>
		/// Determine if the change request can be approved.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static bool CanApproveChangeRequest(this GrantApplication grantApplication)
		{
			return grantApplication.GetChangeRequests().Any(tp => tp.TrainingProviderState == TrainingProviderStates.Requested);
		}

		/// <summary>
		/// Determine if the change request can be denied.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static bool CanDenyChangeRequest(this GrantApplication grantApplication)
		{
			return grantApplication.GetChangeRequests().Any();
		}

		/// <summary>
		/// Check if the grant application is submittable.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static bool IsSubmittable(this GrantApplication grantApplication)
		{
			// If attachments are required they must be provided.
			if (grantApplication.GrantOpening.GrantStream.AttachmentsIsEnabled
			    && grantApplication.GrantOpening.GrantStream.AttachmentsRequired
			    && grantApplication.Attachments.Count == 0)
			{
				return false;
			}

			return !grantApplication.GrantOpening.State.In(GrantOpeningStates.Unscheduled, GrantOpeningStates.Closed)
				&& grantApplication.TrainingCost.TrainingCostState == TrainingCostStates.Complete
				&& grantApplication.TrainingPrograms.Count == 1
				&& grantApplication.TrainingPrograms.FirstOrDefault(tp => tp.TrainingProgramState == TrainingProgramStates.Complete
				                                                          && tp.TrainingProviders.Count == 1
				                                                          && tp.TrainingProviders.First().TrainingProviderState == TrainingProviderStates.Complete) != null
				&& grantApplication.EligibilityConfirmed()
				&& grantApplication.HasValidDates(grantApplication.GrantOpening.TrainingPeriod.StartDate, grantApplication.GrantOpening.TrainingPeriod.EndDate)
				&& grantApplication.HasValidBusinessCase()
				&& grantApplication.HasRequiredParticipants();
		}

		/// <summary>
		/// Check if the grant application is ready for PIF collection.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static bool IsPIFSubmittable(this GrantApplication grantApplication)
		{
			return !grantApplication.GrantOpening.State.In(GrantOpeningStates.Unscheduled, GrantOpeningStates.Closed)
				&& grantApplication.TrainingCost.GetMaxParticipants() >= 1
				&& grantApplication.TrainingPrograms.FirstOrDefault(tp => tp.TrainingProgramState == TrainingProgramStates.Complete
				                                                          && tp.TrainingProviders.Count == 1
				                                                          && tp.TrainingProviders.First().TrainingProviderState == TrainingProviderStates.Complete) != null
				&& grantApplication.TrainingPrograms.Count == 1;
		}

		/// <summary>
		/// Return participant reporting due date based on training program start date.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static DateTime GetParticipantReportingDueDate(this GrantApplication grantApplication)
		{
			if (grantApplication.TrainingPrograms == null || grantApplication.TrainingPrograms.Count <= 0)
				return AppDateTime.Now;

			var earliestTrainingProgram = grantApplication.TrainingPrograms.OrderBy(tp => tp.StartDate).FirstOrDefault();
			if (earliestTrainingProgram == null)
				return AppDateTime.Now;

			return earliestTrainingProgram.StartDate.AddDays(-5);
		}

		/// <summary>
		/// Determines whether a training provider change request can be made.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="addModelError"></param>
		/// <returns></returns>
		public static bool CanMakeTrainingProviderChangeRequest(this GrantApplication grantApplication, Action<string, string> addModelError = null)
		{
			// If a Claim is currently in a submitted state they can't make changes to the Training Provider or Training Program.
			if (grantApplication.CanMakeChangeRequest())
			{
				addModelError?.Invoke("", "A claim is currently being processed, you cannot submit a change request.");
				return false;
			}

			if (grantApplication.ApplicationStateExternal == ApplicationStateExternal.ChangeRequestSubmitted)
			{
				addModelError?.Invoke("", "You cannot make an new change request when one is still being processed.");
				return false;
			}

			return true;
		}

		/// <summary>
		/// Get the total estimated reimbursement of all training programs in this grant application.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static decimal GetEstimatedReimbursement(this GrantApplication grantApplication)
		{
			return Math.Round(grantApplication.TrainingCost.TotalEstimatedReimbursement, 2);
		}

		/// <summary>
		/// Get the total agreed commitment of all training programs in this grant application.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static decimal GetAgreedCommitment(this GrantApplication grantApplication)
		{
			return Math.Round(grantApplication.TrainingCost.AgreedCommitment, 2);
		}

		/// <summary>
		/// Get the reductions of all training programs in this grant application, calculated by summing the estimated reimbursement - agreed max reimbursement, excluding any eligible costs added by the assesor
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static decimal GetReductions(this GrantApplication grantApplication)
		{
			return Math.Round(grantApplication.TrainingPrograms.Sum(tp => tp.GrantApplication.TrainingCost.EligibleCosts.Where(ec => !ec.AddedByAssessor).Sum(ec => ec.EstimatedReimbursement - ec.AgreedMaxReimbursement)));
		}

		/// <summary>
		/// Get the <typeparamref name="GrantApplicationStateChange"/> object represented by the specified <typeparamref name="ApplicationStateInternal"/> value.
		/// This will return the most recent state change for the specified state.
		/// </summary>
		/// <param name="grantApplication">GrantApplication object.</param>
		/// <param name="state"></param>
		/// <returns></returns>
		public static GrantApplicationStateChange GetStateChange(this GrantApplication grantApplication, ApplicationStateInternal state)
		{
			return grantApplication.StateChanges.Where(s => s.ToState == state).OrderByDescending(s => s.DateAdded).FirstOrDefault();
		}

		/// <summary>
		/// Get the <typeparamref name="GrantApplicationStateChange"/> object represented by the specified <typeparamref name="ApplicationStateInternal"/> value.
		/// This will return the most recent state change for the specified state.
		/// </summary>
		/// <param name="grantApplication">GrantApplication object.</param>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public static GrantApplicationStateChange GetStateChange(this GrantApplication grantApplication, ApplicationStateInternal from, ApplicationStateInternal to)
		{
			return grantApplication.StateChanges.Where(s => s.FromState == from && s.ToState == to).OrderByDescending(s => s.DateAdded).FirstOrDefault();
		}

		/// <summary>
		/// Get the reason for the claim being returned to the applicant.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static string GetClaimReturnedToApplicantReason(this GrantApplication grantApplication)
		{
			if (!grantApplication.ApplicationStateInternal.In(ApplicationStateInternal.ClaimReturnedToApplicant))
				return null;

			var claim = grantApplication.GetCurrentClaim();

			return claim?.ClaimAssessmentNotes ?? grantApplication.GetReason(ApplicationStateInternal.ClaimReturnedToApplicant);
		}

		/// <summary>
		/// Get the most recent reason for the specified states.
		/// The first state change that matches will be returned.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="states"></param>
		/// <returns></returns>
		public static string GetReason(this GrantApplication grantApplication, params ApplicationStateInternal[] states)
		{
			if (states == null || states.Length == 0)
				throw new ArgumentException(nameof(states));

			var values = states.Select(s => s).ToArray();
			return grantApplication.StateChanges.Where(s => values.Contains(s.ToState)).OrderByDescending(s => s.DateAdded).Select(s => s.Reason).FirstOrDefault();
		}

		/// <summary>
		/// Get the reason for being denied.
		/// This will return the most recent denial state.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static string GetDeniedReason(this GrantApplication grantApplication)
		{
			return grantApplication.GetReason(ApplicationStateInternal.RecommendedForDenial, ApplicationStateInternal.ApplicationDenied, ApplicationStateInternal.ClaimDenied, ApplicationStateInternal.ChangeForDenial);
		}

		public static string GetApprovedReason(this GrantApplication grantApplication)
		{
			return grantApplication.GetReason(ApplicationStateInternal.RecommendedForApproval);
		}

		/// <summary>
		/// Get the selected reasons for being denied.
		/// This will return the selected denial reasons on the grant file.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static string GetSelectedDeniedReason(this GrantApplication grantApplication)
		{
			if (!grantApplication.ApplicationStateInternal.In(ApplicationStateInternal.RecommendedForDenial, ApplicationStateInternal.ApplicationDenied))
				return string.Empty;

			if (!(grantApplication.GrantApplicationDenialReasons?.Count > 0))
				return string.Empty;

			var selectedDeniedReasons = grantApplication
				.GrantApplicationDenialReasons
				.Select(r => r.Caption)
				.OrderBy(c => c);

			return string.Join("; ", selectedDeniedReasons);
		}

		/// <summary>
		/// Get the reason for being cancelled.
		/// This will return the most recent cancellation state.
		/// </summary>
		/// <param name="grantApplication">GrantApplication object.</param>
		/// <returns></returns>
		public static string GetCancelledReason(this GrantApplication grantApplication)
		{
			return grantApplication.GetReason(ApplicationStateInternal.CancelledByAgreementHolder, ApplicationStateInternal.CancelledByMinistry);
		}

		/// <summary>
		/// Get the terminal reason, either offer withdrawn, cancelled or denied.
		/// This will return the most recent terminal state.
		/// </summary>
		/// <param name="grantApplication">GrantApplication object.</param>
		/// <returns></returns>
		public static string GetTerminalReason(this GrantApplication grantApplication)
		{
			if (grantApplication.ApplicationStateInternal.In(ApplicationStateInternal.AgreementRejected, ApplicationStateInternal.OfferWithdrawn, ApplicationStateInternal.CancelledByAgreementHolder, ApplicationStateInternal.CancelledByMinistry, ApplicationStateInternal.RecommendedForDenial, ApplicationStateInternal.ApplicationDenied, ApplicationStateInternal.ClaimDenied))
			{
				// only show the most pertinent reason (typically one of agreement rejected, offer withdrawn, cancellation or the denial of either the application or a claim)
				return $"{grantApplication.GetReason(ApplicationStateInternal.AgreementRejected, ApplicationStateInternal.OfferWithdrawn)}{grantApplication.GetCancelledReason()}{grantApplication.GetDeniedReason()}";
			}
			else
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// Determine whether this grant application has been approved by the assessor and applicant.
		/// </summary>
		/// <param name="grantApplication">GrantApplication object.</param>
		/// <returns></returns>
		public static bool HasBeenApproved(this GrantApplication grantApplication)
		{
			return grantApplication.ApplicationStateInternal > ApplicationStateInternal.CancelledByMinistry
				|| grantApplication.ApplicationStateInternal.In(ApplicationStateInternal.AgreementAccepted);
		}

		/// <summary>
		/// Determine whether this grant application is currently under assessment.
		/// </summary>
		/// <param name="grantApplication">GrantApplication object.</param>
		/// <returns></returns>
		public static bool IsUnderAssessment(this GrantApplication grantApplication)
		{
			return grantApplication.ApplicationStateInternal.In(
				ApplicationStateInternal.UnderAssessment,
				ApplicationStateInternal.RecommendedForApproval,
				ApplicationStateInternal.RecommendedForDenial,
				ApplicationStateInternal.ReturnedToAssessment
			);
		}

		/// <summary>
		/// Determine whether this v has had an offer issued.
		/// </summary>
		/// <param name="grantApplication">GrantApplication object.</param>
		/// <returns></returns>
		public static bool HasOfferBeenIssued(this GrantApplication grantApplication)
		{
			return !grantApplication.ApplicationStateInternal.In(
				ApplicationStateInternal.Draft,
				ApplicationStateInternal.New,
				ApplicationStateInternal.PendingAssessment,
				ApplicationStateInternal.ApplicationDenied,
				ApplicationStateInternal.ApplicationWithdrawn,
				ApplicationStateInternal.Unfunded,
				ApplicationStateInternal.UnderAssessment,
				ApplicationStateInternal.RecommendedForApproval,
				ApplicationStateInternal.RecommendedForDenial,
				ApplicationStateInternal.ReturnedToAssessment
			);
		}

		/// <summary>
		/// Copies the user and their organization information into the Application.
		/// </summary>
		/// <param name="grantApplication">GrantApplication object.</param>
		/// <param name="user"><typeparamref name="User"/> object to copy from.</param>
		public static void CopyApplicant(this GrantApplication grantApplication, User user)
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (user.PhysicalAddress == null)
			{
				throw new InvalidOperationException($"Applicant must have a physical address.");
			}

			grantApplication.ApplicantBCeID = user.BCeIDGuid;
			grantApplication.ApplicantSalutation = user.Salutation;
			grantApplication.ApplicantFirstName = user.FirstName;
			grantApplication.ApplicantLastName = user.LastName;
			grantApplication.ApplicantPhoneNumber = user.PhoneNumber;
			grantApplication.ApplicantPhoneExtension = user.PhoneExtension;
			grantApplication.ApplicantJobTitle = user.JobTitle;

			if (grantApplication.IsAlternateContact == true && grantApplication.AlternateFirstName != null && grantApplication.AlternateLastName != null && grantApplication.AlternatePhoneNumber != null)
			{
				if (grantApplication.ApplicantPhysicalAddress == null)
				{
					grantApplication.ApplicantPhysicalAddress = new ApplicationAddress
					{
						AddressLine1 = user.PhysicalAddress.AddressLine1,
						AddressLine2 = user.PhysicalAddress.AddressLine2,
						City = user.PhysicalAddress.City,
						RegionId = user.PhysicalAddress.RegionId,
						Region = user.PhysicalAddress.Region,
						PostalCode = user.PhysicalAddress.PostalCode,
						CountryId = user.PhysicalAddress.CountryId,
						Country = user.PhysicalAddress.Country
					};
				}
				else
				{
					grantApplication.ApplicantPhysicalAddress.AddressLine1 = user.PhysicalAddress.AddressLine1;
					grantApplication.ApplicantPhysicalAddress.AddressLine2 = user.PhysicalAddress.AddressLine2;
					grantApplication.ApplicantPhysicalAddress.City = user.PhysicalAddress.City;
					grantApplication.ApplicantPhysicalAddress.RegionId = user.PhysicalAddress.RegionId;
					grantApplication.ApplicantPhysicalAddress.Region = user.PhysicalAddress.Region;
					grantApplication.ApplicantPhysicalAddress.PostalCode = user.PhysicalAddress.PostalCode;
					grantApplication.ApplicantPhysicalAddress.CountryId = user.PhysicalAddress.CountryId;
					grantApplication.ApplicantPhysicalAddress.Country = user.PhysicalAddress.Country;
				}
			}
			else
			{
				if (grantApplication.ApplicantPhysicalAddress == null)
				{
					grantApplication.ApplicantPhysicalAddress = new ApplicationAddress
					{
						AddressLine1 = user.PhysicalAddress.AddressLine1,
						AddressLine2 = user.PhysicalAddress.AddressLine2,
						City = user.PhysicalAddress.City,
						RegionId = user.PhysicalAddress.RegionId,
						Region = user.PhysicalAddress.Region,
						PostalCode = user.PhysicalAddress.PostalCode,
						CountryId = user.PhysicalAddress.CountryId,
						Country = user.PhysicalAddress.Country
					};
				}
				else
				{
					grantApplication.ApplicantPhysicalAddress.AddressLine1 = user.PhysicalAddress.AddressLine1;
					grantApplication.ApplicantPhysicalAddress.AddressLine2 = user.PhysicalAddress.AddressLine2;
					grantApplication.ApplicantPhysicalAddress.City = user.PhysicalAddress.City;
					grantApplication.ApplicantPhysicalAddress.RegionId = user.PhysicalAddress.RegionId;
					grantApplication.ApplicantPhysicalAddress.Region = user.PhysicalAddress.Region;
					grantApplication.ApplicantPhysicalAddress.PostalCode = user.PhysicalAddress.PostalCode;
					grantApplication.ApplicantPhysicalAddress.CountryId = user.PhysicalAddress.CountryId;
					grantApplication.ApplicantPhysicalAddress.Country = user.PhysicalAddress.Country;
				}
			}

			if (grantApplication.IsAlternateContact == true && grantApplication.AlternateFirstName != null && grantApplication.AlternateLastName != null && grantApplication.AlternatePhoneNumber != null)
			{
				if (user.MailingAddress != null)
				{
					if (user.MailingAddressId == user.PhysicalAddressId)
					{
						grantApplication.ApplicantMailingAddress = grantApplication.ApplicantPhysicalAddress;
					}
					else if (grantApplication.ApplicantMailingAddressId == grantApplication.ApplicantPhysicalAddressId
						|| grantApplication.ApplicantMailingAddress == null)
					{
						grantApplication.ApplicantMailingAddress = new ApplicationAddress
						{
							AddressLine1 = user.MailingAddress.AddressLine1,
							AddressLine2 = user.MailingAddress.AddressLine2,
							City = user.MailingAddress.City,
							RegionId = user.MailingAddress.RegionId,
							Region = user.MailingAddress.Region,
							PostalCode = user.MailingAddress.PostalCode,
							CountryId = user.MailingAddress.CountryId,
							Country = user.MailingAddress.Country
						};
					}
					else
					{
						grantApplication.ApplicantMailingAddress.AddressLine1 = user.MailingAddress.AddressLine1;
						grantApplication.ApplicantMailingAddress.AddressLine2 = user.MailingAddress.AddressLine2;
						grantApplication.ApplicantMailingAddress.City = user.MailingAddress.City;
						grantApplication.ApplicantMailingAddress.RegionId = user.MailingAddress.RegionId;
						grantApplication.ApplicantMailingAddress.Region = user.MailingAddress.Region;
						grantApplication.ApplicantMailingAddress.PostalCode = user.MailingAddress.PostalCode;
						grantApplication.ApplicantMailingAddress.CountryId = user.MailingAddress.CountryId;
						grantApplication.ApplicantMailingAddress.Country = user.MailingAddress.Country;
					}
				}
			}
			else
			{
				if (user.MailingAddress != null)
				{
					if (user.MailingAddressId == user.PhysicalAddressId)
					{
						grantApplication.ApplicantMailingAddress = grantApplication.ApplicantPhysicalAddress;
					}
					else if (grantApplication.ApplicantMailingAddressId == grantApplication.ApplicantPhysicalAddressId
						|| grantApplication.ApplicantMailingAddress == null)
					{
						grantApplication.ApplicantMailingAddress = new ApplicationAddress
						{
							AddressLine1 = user.MailingAddress.AddressLine1,
							AddressLine2 = user.MailingAddress.AddressLine2,
							City = user.MailingAddress.City,
							RegionId = user.MailingAddress.RegionId,
							Region = user.MailingAddress.Region,
							PostalCode = user.MailingAddress.PostalCode,
							CountryId = user.MailingAddress.CountryId,
							Country = user.MailingAddress.Country
						};
					}
					else
					{
						grantApplication.ApplicantMailingAddress.AddressLine1 = user.MailingAddress.AddressLine1;
						grantApplication.ApplicantMailingAddress.AddressLine2 = user.MailingAddress.AddressLine2;
						grantApplication.ApplicantMailingAddress.City = user.MailingAddress.City;
						grantApplication.ApplicantMailingAddress.RegionId = user.MailingAddress.RegionId;
						grantApplication.ApplicantMailingAddress.Region = user.MailingAddress.Region;
						grantApplication.ApplicantMailingAddress.PostalCode = user.MailingAddress.PostalCode;
						grantApplication.ApplicantMailingAddress.CountryId = user.MailingAddress.CountryId;
						grantApplication.ApplicantMailingAddress.Country = user.MailingAddress.Country;
					}
				}
			}
		}

		/// <summary>
		/// Copies the user and their organization information into the Application.
		/// </summary>
		/// <param name="grantApplication">GrantApplication object.</param>
		/// <param name="user"><typeparamref name="User"/> object to copy from.</param>
		public static void CopyAlternateAddress(this GrantApplication grantApplication, User user)
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (user.PhysicalAddress == null)
				throw new InvalidOperationException($"Applicant must have a physical address.");
		}

		/// <summary>
		/// Copies the organization information into the Application.
		/// </summary>
		/// <param name="grantApplication">GrantApplication object.</param>
		/// <param name="organization"><typeparamref name="Organization"/> object to copy from.</param>
		/// <param name="naIndustryClassificationSystemService"></param>
		public static void CopyOrganization(this GrantApplication grantApplication, Organization organization, Func<int, int, NaIndustryClassificationSystem> filterNaicsMaxLevel = null)
		{
			grantApplication.Organization = organization ?? throw new ArgumentNullException(nameof(organization));
			grantApplication.OrganizationId = organization.Id;
			grantApplication.OrganizationBCeID = organization.BCeIDGuid;

			if (organization.HeadOfficeAddress != null)
			{
				if (grantApplication.OrganizationAddress == null)
				{
					grantApplication.OrganizationAddress = new ApplicationAddress
					{
						AddressLine1 = organization.HeadOfficeAddress.AddressLine1,
						AddressLine2 = organization.HeadOfficeAddress.AddressLine2,
						City = organization.HeadOfficeAddress.City,
						RegionId = organization.HeadOfficeAddress.RegionId,
						Region = organization.HeadOfficeAddress.Region,
						PostalCode = organization.HeadOfficeAddress.PostalCode,
						CountryId = organization.HeadOfficeAddress.CountryId,
						Country = organization.HeadOfficeAddress.Country
					};
				}
				else
				{
					grantApplication.OrganizationAddress.AddressLine1 = organization.HeadOfficeAddress.AddressLine1;
					grantApplication.OrganizationAddress.AddressLine2 = organization.HeadOfficeAddress.AddressLine2;
					grantApplication.OrganizationAddress.City = organization.HeadOfficeAddress.City;
					grantApplication.OrganizationAddress.RegionId = organization.HeadOfficeAddress.RegionId;
					grantApplication.OrganizationAddress.Region = organization.HeadOfficeAddress.Region;
					grantApplication.OrganizationAddress.PostalCode = organization.HeadOfficeAddress.PostalCode;
					grantApplication.OrganizationAddress.CountryId = organization.HeadOfficeAddress.CountryId;
					grantApplication.OrganizationAddress.Country = organization.HeadOfficeAddress.Country;
				}
			}
			grantApplication.OrganizationType = organization.OrganizationType;
			grantApplication.OrganizationLegalStructure = organization.LegalStructure;
			grantApplication.OrganizationYearEstablished = organization.YearEstablished;
			grantApplication.OrganizationNumberOfEmployeesInBC = organization.NumberOfEmployeesInBC;
			grantApplication.OrganizationNumberOfEmployeesWorldwide = organization.NumberOfEmployeesWorldwide;
			grantApplication.OrganizationLegalName = organization.LegalName;
			grantApplication.OrganizationDoingBusinessAs = organization.DoingBusinessAs;
			grantApplication.OrganizationAnnualTrainingBudget = organization.AnnualTrainingBudget;
			grantApplication.OrganizationAnnualEmployeesTrained = organization.AnnualEmployeesTrained;
			grantApplication.ProgramDescription = grantApplication.ProgramDescription ?? new ProgramDescription(grantApplication)
			{
				Description = "N/A"
			};
			grantApplication.NAICS = filterNaicsMaxLevel == null || organization.Naics == null
				? organization.Naics
				: filterNaicsMaxLevel(organization.Naics.Id, 5); // reset Naics code no deeper than level 5
			grantApplication.OrganizationBusinessLicenseNumber = organization.BusinessLicenseNumber;
		}

		/// <summary>
		/// Add a record to the BusinessContactRoles that associates the specified user as the Application Administrator for this Grant Application.
		/// </summary>
		/// <param name="grantApplication">GrantApplication object.</param>
		/// <param name="administrator"><typeparamref name="User"/> object to associate to this Grant Application.</param>
		public static void AddApplicationAdministrator(this GrantApplication grantApplication, User administrator)
		{
			if (administrator == null)
				throw new ArgumentNullException(nameof(administrator));

			grantApplication.BusinessContactRoles.Add(new BusinessContactRole
			{
				GrantApplication = grantApplication,
				GrantApplicationId = grantApplication.Id,
				User = administrator,
				UserId = administrator.Id
			});
		}

		/// <summary>
		/// Get all the Application Administrators for this Grant Application.
		/// </summary>
		/// <param name="grantApplication">GrantApplication object.</param>
		/// <returns>All <typeparamref name="User"/> objects which are the Application Administrators.</returns>
		public static IQueryable<User> GetApplicationAdministrators(this GrantApplication grantApplication)
		{
			return grantApplication.BusinessContactRoles.Select(bcr => bcr.User).AsQueryable();
		}

		/// <summary>
		/// Get all the Application Administrator IDs for this Grant Application.
		/// </summary>
		/// <param name="grantApplication">GrantApplication object.</param>
		/// <returns>All <typeparamref name="User"/> objects which are the Application Administrators.</returns>
		public static IQueryable<int> GetApplicationAdministratorIds(this GrantApplication grantApplication)
		{
			return grantApplication.BusinessContactRoles.Select(bcr => bcr.UserId).AsQueryable();
		}

		/// <summary>
		/// Get all the Employer Administrators for this Grant Application.
		/// </summary>
		/// <param name="grantApplication">GrantApplication object.</param>
		/// <returns>All <typeparamref name="User"/> objects which are the Employer Administrators.</returns>
		public static IQueryable<User> GetEmployerAdministrators(this GrantApplication grantApplication)
		{
			return grantApplication.BusinessContactRoles.Select(bcr => bcr.User).AsQueryable();
		}

		/// <summary>
		/// Determine whether the internal state change transition is valid.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public static bool IsValidStateTransition(this GrantApplication grantApplication, ApplicationStateInternal to)
		{
			return grantApplication.ApplicationStateInternal.IsValidStateTransition(to);
		}

		/// <summary>
		/// Determine whether the external state change transition is valid.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public static bool IsValidStateTransition(this GrantApplication grantApplication, ApplicationStateExternal to)
		{
			return grantApplication.ApplicationStateExternal.IsValidStateTransition(to);
		}

		/// <summary>
		/// Determine if the specified user is the grant application administrator.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public static bool IsApplicationAdministrator(this GrantApplication grantApplication, int userId)
		{
			if (userId <= 0)
				return false;

			return grantApplication?.BusinessContactRoles.Any(bcr => bcr.UserId == userId) == true;
		}

		/// <summary>
		/// Determine if the specified user is the grant application administrator.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		public static bool IsApplicationAdministrator(this GrantApplication grantApplication, User user)
		{
			if (user == null)
				return false;

			return grantApplication?.IsApplicationAdministrator(user?.Id ?? 0) ?? false;
		}

		/// <summary>
		/// Determine if the specified user is allowed to view the grant application.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		public static bool IsAuthorizedToView(this GrantApplication grantApplication, User user)
		{
			if (user == null)
				return false;

			return grantApplication.IsApplicationAdministrator(user);
		}

		/// <summary>
		/// Determine if the specified user is the current grant application assessor.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		public static bool IsApplicationAssessor(this GrantApplication grantApplication, InternalUser user)
		{
			if (user == null)
				return false;

			return grantApplication?.AssessorId == user?.Id;
		}

		/// <summary>
		/// Removes the currently assigned assessor from the Grant Application.
		/// </summary>
		/// <param name="grantApplication"></param>
		public static void RemoveAssignedAssessor(this GrantApplication grantApplication)
		{
			grantApplication.Assessor = null;
			grantApplication.AssessorId = null;
		}

		public static void ResetCipsCode(this GrantApplication grantApplication)
		{
			foreach (var trainingProgram in grantApplication.TrainingPrograms.Where(tp => tp.CipsCode != null))
			{
				trainingProgram.CipsCode = null;
				trainingProgram.TargetCipsCodeId = null;
			}
		}

		public static void ResetValidatedTrainingProvider(this GrantApplication grantApplication)
		{
			foreach (var trainingProgram in grantApplication.TrainingPrograms)
			{
				foreach (var trainingProvider in trainingProgram.TrainingProviders)
				{
					if (!trainingProvider.TrainingProviderInventoryId.HasValue)
						continue;

					var findTrainingProviderName = trainingProvider.Name;

					// Always reset the validation state, but attempt to restore the original name
					// Attempt to find the note that was created when the provider was first validated
					var changeNote = grantApplication.Notes
						.Where(n => n.NoteTypeId == NoteTypes.ED)
						.Where(n => n.Content.Contains($"\"name\": \"Training Provider -") && n.Content.Contains(findTrainingProviderName))
						.OrderBy(n => n.DateAdded)
						.FirstOrDefault();

					if (changeNote != null)
					{ 
						var oldName = changeNote.GetOldValueFromNewValue("Name", findTrainingProviderName);
						if (!string.IsNullOrWhiteSpace(oldName))
							trainingProvider.Name = oldName;
					}

					trainingProvider.TrainingProviderInventoryId = null;
				}
			}
		}

		public static void ResetParticipantEligibilityStatus(this GrantApplication grantApplication)
		{
			foreach (var participantForm in grantApplication.ParticipantForms)
				participantForm.Approved = null;
		}

		/// <summary>
		/// reverts application status to incomplete
		/// </summary>
		/// <param name="grantApplication"></param>
		public static void RevertStatus(this GrantApplication grantApplication)
		{
			grantApplication.ApplicationStateExternal = ApplicationStateExternal.Incomplete;
		}

		/// <summary>
		/// reverts program description section status to incomplete and clear naics code
		/// </summary>
		/// <param name="programDescription"></param>
		public static void ClearNaics(this ProgramDescription programDescription)
		{
			programDescription.DescriptionState = ProgramDescriptionStates.Incomplete;
			programDescription.TargetNAICSId = 0;
		}

		/// <summary>
		/// Checks whether the eligibility is required and confirmed for the grant application.
		/// If it is not required then it defaults to confirmed.
		/// -------------
		/// NOTE: Assuming the system has a changed Elig. question, this will return FALSE for an aapplication that
		/// actually passed eligibility.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static bool EligibilityConfirmed(this GrantApplication grantApplication)
		{
			// Assemble the (possibly new) questions, and the current answers.
			// Match them up.
			// If any answers are wrong EligibilityEnabled is false.
			var answers = grantApplication.GrantStreamEligibilityAnswers;
			var questions = grantApplication.GrantOpening?.GrantStream?.GrantStreamEligibilityQuestions;

			bool newConfirmed = true;
			if (answers != null && questions != null)
			{
				foreach (var question in questions)
				{
					if (question.IsActive && question.EligibilityPositiveAnswerRequired)
					{
						var answer = answers.Where(x => x.GrantStreamEligibilityQuestionId == question.Id).FirstOrDefault();
						if (answer == null)
							newConfirmed = false;	// No answer to a question: fail
						else if (!answer.EligibilityAnswer)
							newConfirmed = false;	// Wrong answer
					}
				}
			}
			else
			{
				newConfirmed = false;	// questions/ answers null (should be count = 0): fail
			}

			return newConfirmed;
		}
		/// <summary>
		/// Validate whether the employment services and supports information is complete.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static bool EmploymentServicesAndSupportsConfirmed(this GrantApplication grantApplication)
		{
			var serviceCategories = grantApplication.GrantOpening.GrantStream.ProgramConfiguration.EligibleExpenseTypes.Where(eet => eet.ServiceCategory.ServiceTypeId == ServiceTypes.EmploymentServicesAndSupports);
			if (!serviceCategories.Any())
				return true;

			// Must select at least one service line and have a cost associated with on ESS service category.
			foreach (var eligibleCost in grantApplication.TrainingCost.EligibleCosts.Where(ec => ec.EligibleExpenseType.ServiceCategory?.ServiceTypeId == ServiceTypes.EmploymentServicesAndSupports))
			{
				var serviceLines = eligibleCost.Breakdowns.Count();
				if ((eligibleCost.EstimatedCost == 0 && serviceLines > 0)
				    || (eligibleCost.TrainingProviders?.Count() > 0 && (eligibleCost.EstimatedCost == 0 || serviceLines == 0))
				    || eligibleCost.EligibleExpenseType.ServiceCategory.MinProviders > serviceLines
				    || (eligibleCost.EligibleExpenseType.ServiceCategory.MaxProviders > 0 && eligibleCost.EligibleExpenseType.ServiceCategory.MaxProviders < serviceLines)
				    || (eligibleCost.EstimatedCost > 0 && serviceLines == 0)
				    || (eligibleCost.EstimatedCost > 0 && eligibleCost.TrainingProviders.Count() == 0 && eligibleCost.EligibleExpenseType.MaxProviders > 0))
					return false;
			}

			return true;
		}

		/// <summary>
		/// Validate whether the skills training component information is complete.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static bool SkillsTrainingConfirmed(this GrantApplication grantApplication)
		{
			var serviceCategories = grantApplication.GrantOpening.GrantStream.ProgramConfiguration.EligibleExpenseTypes.Where(eet => eet.ServiceCategory?.ServiceTypeId == ServiceTypes.SkillsTraining);
			if (!serviceCategories.Any())
				return true;

			foreach (var eligibleCost in grantApplication.TrainingCost.EligibleCosts.Where(ec => ec.EligibleExpenseType.ServiceCategory?.ServiceTypeId == ServiceTypes.SkillsTraining))
			{
				var components = eligibleCost.Breakdowns.Count();
				if (eligibleCost.EstimatedCost <= 0
				    || eligibleCost.EligibleExpenseType.ServiceCategory.MinPrograms > components
				    || eligibleCost.EligibleExpenseType.ServiceCategory.MaxPrograms < components
				    || eligibleCost.Breakdowns.Any(stc => stc.EstimatedCost <= 0))
					return false;
			}

			return true;
		}

		/// <summary>
		/// return true if PIFS are not required or
		/// the number of PIFs equals the number of participants
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static bool PIFCompletionConfirmed(this GrantApplication grantApplication)
		{
			return grantApplication.RequireAllParticipantsBeforeSubmission
				? grantApplication.ParticipantForms.Count() == grantApplication.TrainingCost.EstimatedParticipants
				: true;
		}

		public static bool CanViewParticipantEligibilty(this GrantApplication grantApplication)
		{
			return grantApplication.RequireAllParticipantsBeforeSubmission &&
					(grantApplication.ApplicationStateInternal == ApplicationStateInternal.OfferIssued ||
					 grantApplication.ApplicationStateInternal == ApplicationStateInternal.AgreementAccepted ||
					 grantApplication.ApplicationStateInternal == ApplicationStateInternal.ApplicationDenied ||
					 grantApplication.ApplicationStateInternal == ApplicationStateInternal.AgreementRejected ||
					 grantApplication.ApplicationStateInternal == ApplicationStateInternal.ApplicationWithdrawn ||
					 grantApplication.ApplicationStateInternal == ApplicationStateInternal.CancelledByMinistry ||
					 grantApplication.ApplicationStateInternal == ApplicationStateInternal.CancelledByAgreementHolder ||
					 grantApplication.ApplicationStateInternal == ApplicationStateInternal.ChangeRequest ||
					 grantApplication.ApplicationStateInternal == ApplicationStateInternal.ChangeForApproval ||
					 grantApplication.ApplicationStateInternal == ApplicationStateInternal.ChangeForDenial ||
					 grantApplication.ApplicationStateInternal == ApplicationStateInternal.ChangeReturned ||
					 grantApplication.ApplicationStateInternal == ApplicationStateInternal.ChangeRequestDenied ||
					 grantApplication.ApplicationStateInternal == ApplicationStateInternal.NewClaim ||
					 grantApplication.ApplicationStateInternal == ApplicationStateInternal.ClaimAssessEligibility ||
					 grantApplication.ApplicationStateInternal == ApplicationStateInternal.ClaimReturnedToApplicant ||
					 grantApplication.ApplicationStateInternal == ApplicationStateInternal.ClaimDenied ||
					 grantApplication.ApplicationStateInternal == ApplicationStateInternal.ClaimApproved ||
					 grantApplication.ApplicationStateInternal == ApplicationStateInternal.CompletionReporting ||
					 grantApplication.ApplicationStateInternal == ApplicationStateInternal.Closed ||
					 grantApplication.ApplicationStateInternal == ApplicationStateInternal.ClaimAssessReimbursement ||
					 grantApplication.ApplicationStateInternal == ApplicationStateInternal.AgreementRejected);
		}

		public static List<ParticipantForm> ApprovedParticipants(this GrantApplication grantApplication)
		{
			return grantApplication.ParticipantForms.Where(w => w.Approved.HasValue && w.Approved.Value).ToList();
		}

		/// <summary>
		/// Determine whether the GrantApplication invitation link has expired.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static bool IsInvitationExpired(this GrantApplication grantApplication)
		{
			if (grantApplication?.InvitationExpiresOn < AppDateTime.UtcNow)
				return true;

			if (grantApplication.CanReportParticipants)
				return false;

			// Under any of the following states the employer enrollment has expired.
			// We don't include claim states here as they are checked in the next step.
			// Some of these states should never occur during participant reporting anyways, but we check them all.
			if (new[] {
				ApplicationStateInternal.AgreementRejected,
				ApplicationStateInternal.ApplicationDenied,
				ApplicationStateInternal.ApplicationWithdrawn,
				ApplicationStateInternal.CancelledByAgreementHolder,
				ApplicationStateInternal.CancelledByMinistry,
				ApplicationStateInternal.Closed,
				ApplicationStateInternal.OfferIssued,
				ApplicationStateInternal.OfferWithdrawn,
				ApplicationStateInternal.PendingAssessment,
				ApplicationStateInternal.RecommendedForApproval,
				ApplicationStateInternal.RecommendedForDenial,
				ApplicationStateInternal.ReturnedToAssessment,
				ApplicationStateInternal.UnderAssessment,
				ApplicationStateInternal.Unfunded,
				ApplicationStateInternal.ReturnedUnassessed
			}.Contains(grantApplication.ApplicationStateInternal)) return true;

			// If a claim is submitted but not assessed, the invitation is considered expired.
			return grantApplication.IsCurrentClaimUnassessed();
		}

		/// <summary>
		/// The Training Program start date must be between the training period start and end dates (inclusive), and not be before the application creation or submitted dates.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static bool HasValidStartDate(this GrantApplication grantApplication)
		{
			return grantApplication.HasValidStartDate(
				   grantApplication.GrantOpening.TrainingPeriod.StartDate,
				   grantApplication.GrantOpening.TrainingPeriod.EndDate);
		}

		/// <summary>
		/// The Training Program start date must be between the training period start and end dates (inclusive), and not be before the application creation or submitted dates.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="trainingPeriodStartDate"></param>
		/// <param name="trainingPeriodEndDate"></param>
		/// <returns></returns>
		private static bool HasValidStartDate(this GrantApplication grantApplication, DateTime trainingPeriodStartDate, DateTime trainingPeriodEndDate)
		{
			var earliest = (
				grantApplication.ApplicationStateInternal.In(ApplicationStateInternal.Draft)
				? (grantApplication.DateAdded >= AppDateTime.UtcNow ? grantApplication.DateAdded : AppDateTime.UtcNow)
				: (grantApplication.DateSubmitted ?? AppDateTime.UtcNow)
				).ToLocalMorning();

			return grantApplication.StartDate.ToLocalMorning().Date >= trainingPeriodStartDate.ToLocalMorning().Date
				&& grantApplication.StartDate.ToLocalMorning().Date <= trainingPeriodEndDate.ToLocalMidnight().Date
				&& grantApplication.StartDate.ToLocalMorning().Date >= earliest.Date;
		}

		/// <summary>
		/// Get the earliest valid delivery start date
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static DateTime EarliestValidStartDate(this GrantApplication grantApplication)
		{
			var useDateSubmitted = grantApplication.DateSubmitted ??
			                       (grantApplication.ApplicationStateInternal > ApplicationStateInternal.Draft
				                       ? grantApplication.DateAdded
				                       : AppDateTime.UtcNow);

			var dateSubmitted = useDateSubmitted.ToLocalMorning().Date;
			var trainingPeriodStartDate = grantApplication.GrantOpening.TrainingPeriod.StartDate.ToLocalMorning().Date;

			return dateSubmitted > trainingPeriodStartDate ? dateSubmitted : trainingPeriodStartDate;
		}

		/// <summary>
		/// The Training Program end date must on or after the start date, and must not be more than a year after the start date.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static bool HasValidEndDate(this GrantApplication grantApplication)
		{
			if (grantApplication.StartDate.ToLocalMorning().Date > grantApplication.EndDate.ToLocalMidnight().Date
				|| grantApplication.StartDate.ToLocalMorning().AddYears(1).Date < grantApplication.EndDate.ToLocalMidnight().Date)
				return false;

			return true;
		}

		/// <summary>
		/// Validates both the start date and end dates.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static bool HasValidDates(this GrantApplication grantApplication)
		{
			return HasValidStartDate(grantApplication) && HasValidEndDate(grantApplication);
		}

		/// <summary>
		/// Validates both the start date and end dates.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="trainingPeriodStartDate"></param>
		/// <param name="trainingPeriodEndDate"></param>
		/// <returns></returns>
		public static bool HasValidDates(this GrantApplication grantApplication, DateTime trainingPeriodStartDate, DateTime trainingPeriodEndDate)
		{
			return HasValidStartDate(grantApplication, trainingPeriodStartDate, trainingPeriodEndDate) && HasValidEndDate(grantApplication);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static bool HasValidBusinessCase(this GrantApplication grantApplication)
		{
			if (!grantApplication.GrantOpening.GrantStream.BusinessCaseIsEnabled || !grantApplication.GrantOpening.GrantStream.BusinessCaseRequired)
				return true;

			return grantApplication.GrantOpening.GrantStream.BusinessCaseIsEnabled
					&& grantApplication.GrantOpening.GrantStream.BusinessCaseRequired
					&& grantApplication.BusinessCaseDocument != null;
		}

		/// <summary>
		/// Determine if the applicant can make a change request for a training/service provider.
		/// If the claim type is single amendable claim, then do not allow a change request if a prior claim has been approved.
		/// If the claim type is multiple without amendments, then do not allow a change request during a claim assessment.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static bool CanChangeDeliveryDates(this GrantApplication grantApplication)
		{
			return grantApplication.ApplicationStateInternal.In(ApplicationStateInternal.AgreementAccepted, ApplicationStateInternal.ChangeRequestDenied, ApplicationStateInternal.ClaimReturnedToApplicant, ApplicationStateInternal.ClaimDenied, ApplicationStateInternal.ClaimApproved);
		}

		/// <summary>
		/// Determine if the applicant can make a change request for a training/service provider.
		/// If the claim type is single amendable claim, then do not allow a change request if a prior claim has been approved.
		/// If the claim type is multiple without amendments, then do not allow a change request during a claim assessment.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static bool CanMakeChangeRequest(this GrantApplication grantApplication)
		{
			if (grantApplication.ApplicationStateInternal.In(
				ApplicationStateInternal.AgreementAccepted,
				ApplicationStateInternal.ChangeRequestDenied,
				ApplicationStateInternal.NewClaim,
				ApplicationStateInternal.ClaimReturnedToApplicant,
				ApplicationStateInternal.ClaimDenied,
				ApplicationStateInternal.ClaimApproved))
			{
				var claimType = grantApplication.GetClaimType();
				// Do not allow change request if a prior claim has been approved.
				if (claimType == ClaimTypes.SingleAmendableClaim)
					return !grantApplication.HasPriorApprovedClaim();

				// Do not allow change requests during a claim assessment.
				return !(grantApplication.GetCurrentClaim()?.ClaimState.In(ClaimState.Unassessed) ?? false);
			}

			return false;
		}

		public static bool ViableTrainingDateUpdate(this GrantApplication grantApplication, DateTime proposedStartDate)
		{
			// If the date hasn't changed, bypass
			if (grantApplication.StartDate == proposedStartDate)
				return true;

			if (grantApplication.ApplicationStateInternal.In(
				ApplicationStateInternal.NewClaim,
				ApplicationStateInternal.ClaimDenied,
				ApplicationStateInternal.ClaimApproved,
				ApplicationStateInternal.ClaimAssessEligibility,
				ApplicationStateInternal.ClaimAssessReimbursement,
				ApplicationStateInternal.ClaimReturnedToApplicant))
				return false;

			return true;
		}

		/// <summary>
		/// Determine if the applicant can make a change request for a training dates.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static bool CanChangeTrainingDates(this GrantApplication grantApplication)
		{
			if (grantApplication.ApplicationStateInternal.In(ApplicationStateInternal.AgreementAccepted, ApplicationStateInternal.ChangeRequestDenied, ApplicationStateInternal.NewClaim, ApplicationStateInternal.ClaimReturnedToApplicant, ApplicationStateInternal.ClaimDenied, ApplicationStateInternal.ClaimApproved))
			{
				var claimType = grantApplication.GetClaimType();

				if (claimType == ClaimTypes.SingleAmendableClaim)
				{
					// Do not allow change request if a prior claim has been approved.
					return !grantApplication.HasPriorApprovedClaim();
				}

				// Do not allow change requests during a claim assessment.
				return !(grantApplication.GetCurrentClaim()?.ClaimState.In(ClaimState.Unassessed) ?? false);
			}

			return false;
		}

		public static bool TrainingHasStarted(this GrantApplication grantApplication)
		{
			if (grantApplication.ApplicationStateInternal == ApplicationStateInternal.Draft)
				return false;

			var earliestTrainingProgram = grantApplication.TrainingPrograms
				.OrderBy(tp => tp.StartDate)
				.FirstOrDefault();

			if (earliestTrainingProgram == null)
				return false;

			return AppDateTime.UtcNow >= earliestTrainingProgram.StartDate;
		}

		/// <summary>
		/// Determine if the grant application has had a claim that has been submitted.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static bool HasSubmittedAClaim(this GrantApplication grantApplication)
		{
			return grantApplication.Claims.Any(c => !c.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete) ||
			 (c.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete) && c.GrantApplication.ApplicationStateInternal == ApplicationStateInternal.ClaimReturnedToApplicant));
		}

		/// <summary>
		/// Determine if the grant application has had a prior approved claim.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static bool HasPriorApprovedClaim(this GrantApplication grantApplication)
		{
			return grantApplication.Claims.Any(c => c.ClaimState.In(ClaimState.ClaimApproved, ClaimState.PaymentRequested, ClaimState.ClaimPaid, ClaimState.AmountOwing, ClaimState.AmountReceived));
		}

		/// <summary>
		/// Determine if the grant application has a submitted but not assessed claim.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static bool IsCurrentClaimUnassessed(this GrantApplication grantApplication)
		{
			return grantApplication.Claims.Any() && grantApplication.GetCurrentClaim().ClaimState == ClaimState.Unassessed;
		}

		/// <summary>
		/// Determine if the grant application has a claim that is currently submitted.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static bool IsCurrentClaimSubmitted(this GrantApplication grantApplication)
		{
			return !grantApplication.GetCurrentClaim()?.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete) ?? false;
		}

		/// <summary>
		/// Get the current claim, which will be the one with the highest version number.
		/// While a grant application can currently have multiple claims and multiple versions of those claims, we only allow one claim and many versions presently.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static Claim GetCurrentClaim(this GrantApplication grantApplication)
		{
			return grantApplication.Claims
				.OrderByDescending(c => c.Id)
				.ThenByDescending(c => c.ClaimVersion)
				.FirstOrDefault();
		}
		/// <summary>
		/// Get the most recent prior claim that was approved.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static Claim GetPriorApprovedClaim(this GrantApplication grantApplication)
		{
			return grantApplication.Claims.Where(c => c.ClaimState == ClaimState.ClaimApproved).OrderByDescending(c => c.ClaimVersion).FirstOrDefault();
		}

		/// <summary>
		/// Get the prior claim.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static Claim GetPriorClaim(this GrantApplication grantApplication)
		{
			return grantApplication.Claims.Where(c => !c.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete, ClaimState.Unassessed)).OrderByDescending(c => c.ClaimVersion).FirstOrDefault();
		}

		/// <summary>
		/// Get Program Type for the specified grant application.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static ProgramTypes GetProgramType(this GrantApplication grantApplication)
		{
			return grantApplication?.GrantOpening?.GrantStream?.GrantProgram?.ProgramTypeId ?? throw new ArgumentNullException(nameof(GrantProgram));
		}

		/// <summary>
		/// Get the Program Claim Type for the specified grant application.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static ClaimTypes GetClaimType(this GrantApplication grantApplication)
		{
			return grantApplication?.GrantOpening?.GrantStream?.GrantProgram?.ProgramConfiguration?.ClaimTypeId
			       ?? throw new ArgumentNullException(nameof(ProgramConfiguration));
		}

		/// <summary>
		/// Enable Participant Reporting
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static void EnableParticipantReporting(this GrantApplication grantApplication)
		{
			grantApplication.CanReportParticipants = true;
			grantApplication.InvitationExpiresOn = null;
			grantApplication.InvitationKey = Guid.NewGuid();
		}

		/// <summary>
		/// Disable Participant Reporting
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static void DisableParticipantReporting(this GrantApplication grantApplication)
		{
			grantApplication.CanReportParticipants = false;
			grantApplication.InvitationExpiresOn = AppDateTime.UtcNow;
		}

		/// <summary>
		/// Enable Applicant Participant Reporting
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static void EnableApplicantParticipantReporting(this GrantApplication grantApplication)
		{
			grantApplication.CanApplicantReportParticipants = true;
		}

		/// <summary>
		/// Disable Applicant Participant Reporting
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static void DisableApplicantParticipantReporting(this GrantApplication grantApplication)
		{
			grantApplication.CanApplicantReportParticipants = false;
		}

		/// <summary>
		/// Determine if the agreement needs to be updated based on the changes submitted to the datasource.
		/// This must be called before the Commit() or CommitTransaction() to detect whether the agreement needs to be updated.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="changes"></param>
		/// <returns></returns>
		public static bool AgreementUpdateRequired(this GrantApplication grantApplication, EntityChanges changes)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			if (changes == null) throw new ArgumentNullException(nameof(changes));

			if (grantApplication.GrantAgreement == null) return false;

			// If the grant application dates have been changed.
			if (changes.HasChanged(grantApplication, nameof(GrantApplication.StartDate), nameof(GrantApplication.EndDate))) return true;

			// If the training programs have been changed.
			if (changes.HasAdded(typeof(TrainingProgram))) return true;
			if (changes.HasDeleted(typeof(TrainingProgram))) return true;
			if (changes.HasChanged(typeof(TrainingProgram), nameof(TrainingProgram.CourseTitle), nameof(TrainingProgram.StartDate), nameof(TrainingProgram.EndDate), nameof(TrainingProgram.ServiceLineId), nameof(TrainingProgram.ServiceLineBreakdownId))) return true;

			// If a training provider has been deleted.
			if (changes.HasDeleted(typeof(TrainingProvider))) return true;

			// If the approved training providers have been changed.
			var approvedTrainingProviders = grantApplication.GetOriginalTrainingProviders().Where(tp => tp.IsValidated()).Select(tp => tp.ApprovedTrainingProvider);
			foreach (var approved in approvedTrainingProviders)
			{
				var tpChanges = changes[approved];
				if (tpChanges != null && tpChanges.IsChanged) return true;
			}

			// If costs have been changed.
			if (changes.HasChanged(typeof(TrainingCost))) return true;
			if (changes.HasChanged(typeof(EligibleCost))) return true;
			if (changes.HasChanged(typeof(EligibleCostBreakdown))) return true;

			return false;
		}

		public static void Clone(this GrantApplication grantApplication, GrantApplication seedGrantApplication)
		{
			grantApplication.OrganizationTypeId = seedGrantApplication.OrganizationTypeId;
			grantApplication.OrganizationType = seedGrantApplication.OrganizationType;

			grantApplication.OrganizationLegalName = seedGrantApplication.OrganizationLegalName;
			grantApplication.OrganizationLegalStructureId = seedGrantApplication.OrganizationLegalStructureId;
			grantApplication.OrganizationLegalStructure = seedGrantApplication.OrganizationLegalStructure;
			grantApplication.OrganizationBusinessLicenseNumber = seedGrantApplication.OrganizationBusinessLicenseNumber;

			grantApplication.OrganizationBCeID = seedGrantApplication.OrganizationBCeID;

			grantApplication.OrganizationYearEstablished = seedGrantApplication.OrganizationYearEstablished;
			grantApplication.OrganizationNumberOfEmployeesWorldwide = seedGrantApplication.OrganizationNumberOfEmployeesWorldwide;
			grantApplication.OrganizationAnnualTrainingBudget = seedGrantApplication.OrganizationAnnualTrainingBudget;
			grantApplication.OrganizationAnnualEmployeesTrained = seedGrantApplication.OrganizationAnnualEmployeesTrained;
			grantApplication.PrioritySectorId = seedGrantApplication.PrioritySectorId;
			grantApplication.OrganizationDoingBusinessAs = seedGrantApplication.OrganizationDoingBusinessAs;
			grantApplication.NAICSId = seedGrantApplication.NAICSId;


			grantApplication.OrganizationNumberOfEmployeesInBC = seedGrantApplication.OrganizationNumberOfEmployeesInBC;
			grantApplication.RiskClassificationId = seedGrantApplication.RiskClassificationId;
			grantApplication.RiskClassification = seedGrantApplication.RiskClassification;

			grantApplication.EligibilityConfirmed = seedGrantApplication.EligibilityConfirmed;
			grantApplication.CanApplicantReportParticipants = seedGrantApplication.CanApplicantReportParticipants;

			grantApplication.HoldPaymentRequests = seedGrantApplication.HoldPaymentRequests;

			grantApplication.UsedDeliveryPartner = seedGrantApplication.UsedDeliveryPartner;
			grantApplication.DeliveryPartnerId = seedGrantApplication.DeliveryPartnerId;

			grantApplication.OrganizationBusinessLicenseNumber = seedGrantApplication.OrganizationBusinessLicenseNumber;
			grantApplication.CanReportParticipants = seedGrantApplication.CanReportParticipants;

			grantApplication.ScheduledNotificationsEnabled = seedGrantApplication.ScheduledNotificationsEnabled;

			grantApplication.AlternateSalutation = seedGrantApplication.AlternateSalutation;
			grantApplication.AlternateFirstName = seedGrantApplication.AlternateFirstName;
			grantApplication.AlternateLastName = seedGrantApplication.AlternateLastName;
			grantApplication.AlternatePhoneNumber = seedGrantApplication.AlternatePhoneNumber;
			grantApplication.AlternatePhoneExtension = seedGrantApplication.AlternatePhoneExtension;
			grantApplication.AlternateJobTitle = seedGrantApplication.AlternateJobTitle;
			grantApplication.AlternateEmail = seedGrantApplication.AlternateEmail;
			grantApplication.IsAlternateContact = seedGrantApplication.IsAlternateContact;
			grantApplication.HasRequestedAdditionalFunding = seedGrantApplication.HasRequestedAdditionalFunding;
			grantApplication.DescriptionOfFundingRequested = seedGrantApplication.DescriptionOfFundingRequested;

			//migrate the business case info if the BusinessCase flag is true for both new and seed grants
			if (grantApplication.GrantOpening.GrantStream.BusinessCaseIsEnabled &&
				seedGrantApplication.GrantOpening.GrantStream.BusinessCaseIsEnabled)
			{
				grantApplication.BusinessCase = seedGrantApplication.BusinessCase;
			}			
		}

		public static GrantApplication Clone(this GrantApplication grantApplication)
		{
			var application = new GrantApplication();

			application.ApplicationStateExternal = grantApplication.ApplicationStateExternal;
			application.ApplicationStateInternal = grantApplication.ApplicationStateInternal;
			application.FileNumber = grantApplication.FileNumber;

			application.GrantOpeningId = grantApplication.GrantOpeningId;
			application.GrantOpening = grantApplication.GrantOpening;
	
			application.ApplicationTypeId = grantApplication.ApplicationTypeId;
			application.ApplicationType = grantApplication.ApplicationType;

			application.HostingTrainingProgram = grantApplication.HostingTrainingProgram;
			application.ApplicantBCeID = grantApplication.ApplicantBCeID;

			application.ApplicantSalutation = grantApplication.ApplicantSalutation;
			application.ApplicantFirstName = grantApplication.ApplicantFirstName;
			application.ApplicantLastName = grantApplication.ApplicantLastName;

			application.ApplicantPhoneNumber = grantApplication.ApplicantPhoneNumber;
			application.ApplicantPhoneExtension = grantApplication.ApplicantPhoneExtension;
			application.ApplicantJobTitle = grantApplication.ApplicantJobTitle;

			application.OrganizationId = grantApplication.OrganizationId;
			application.Organization = grantApplication.Organization;

			application.OrganizationTypeId = grantApplication.OrganizationTypeId;
			application.OrganizationType = grantApplication.OrganizationType;

			application.OrganizationLegalName = grantApplication.OrganizationLegalName;
			application.OrganizationLegalStructureId = grantApplication.OrganizationLegalStructureId;
			application.OrganizationLegalStructure = grantApplication.OrganizationLegalStructure;
			application.OrganizationBusinessLicenseNumber = grantApplication.OrganizationBusinessLicenseNumber;
		
			application.OrganizationBCeID = grantApplication.OrganizationBCeID;

			application.OrganizationYearEstablished = grantApplication.OrganizationYearEstablished;
			application.OrganizationNumberOfEmployeesWorldwide = grantApplication.OrganizationNumberOfEmployeesWorldwide;
			application.OrganizationAnnualTrainingBudget = grantApplication.OrganizationAnnualTrainingBudget;
			application.OrganizationAnnualEmployeesTrained = grantApplication.OrganizationAnnualEmployeesTrained;
			application.PrioritySectorId = grantApplication.PrioritySectorId;
			application.OrganizationDoingBusinessAs = grantApplication.OrganizationDoingBusinessAs;
			application.NAICSId = grantApplication.NAICSId;			

			application.AssessorNote = grantApplication.AssessorNote;
			application.MaxReimbursementAmt = grantApplication.MaxReimbursementAmt;
			application.ReimbursementRate = grantApplication.ReimbursementRate;

			application.OrganizationNumberOfEmployeesInBC = grantApplication.OrganizationNumberOfEmployeesInBC;
			application.RiskClassificationId = grantApplication.RiskClassificationId;
			application.RiskClassification = grantApplication.RiskClassification;

			application.EligibilityConfirmed = grantApplication.EligibilityConfirmed;
			application.CanApplicantReportParticipants = grantApplication.CanApplicantReportParticipants;
			application.StartDate = grantApplication.StartDate;
			application.EndDate = grantApplication.EndDate;
			application.DateUpdated = grantApplication.DateUpdated;

			application.CompletionReportId = grantApplication.CompletionReportId;
			application.CompletionReport = grantApplication.CompletionReport;

			application.HoldPaymentRequests = grantApplication.HoldPaymentRequests;

			application.UsedDeliveryPartner = grantApplication.UsedDeliveryPartner;
			application.DeliveryPartnerId = grantApplication.DeliveryPartnerId;

			application.OrganizationBusinessLicenseNumber = grantApplication.OrganizationBusinessLicenseNumber;
			application.CanReportParticipants = grantApplication.CanReportParticipants;
			application.UsePIFInvitations = grantApplication.UsePIFInvitations;

			application.ScheduledNotificationsEnabled = grantApplication.ScheduledNotificationsEnabled;

			application.AlternateSalutation = grantApplication.AlternateSalutation;
			application.AlternateFirstName = grantApplication.AlternateFirstName;
			application.AlternateLastName = grantApplication.AlternateLastName;
			application.AlternatePhoneNumber = grantApplication.AlternatePhoneNumber;
			application.AlternatePhoneExtension = grantApplication.AlternatePhoneExtension;
			application.AlternateJobTitle = grantApplication.AlternateJobTitle;
			application.AlternateEmail = grantApplication.AlternateEmail;
			application.IsAlternateContact = grantApplication.IsAlternateContact;
			application.HasRequestedAdditionalFunding = grantApplication.HasRequestedAdditionalFunding;
			application.DescriptionOfFundingRequested = grantApplication.DescriptionOfFundingRequested;

			application.BusinessCase = grantApplication.BusinessCase;

			return application;
		}

		#region Training Costs
		/// <summary>
		/// Calculates the estimated cost limit based on the eligible expense type and rate.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="eligibleCost"></param>
		/// <returns></returns>
		public static decimal CalculateEstimatedCostLimit(this GrantApplication grantApplication, EligibleCost eligibleCost)
		{
			return grantApplication.TrainingCost.CalculateEstimatedCostLimit(eligibleCost);
		}

		/// <summary>
		/// Calculate the total agreed cost of all eligible costs.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static decimal CalculateAgreedMaxReimbursement(this GrantApplication grantApplication)
		{
			return grantApplication.TrainingCost.CalculateAgreedMaxReimbursement();
		}

		/// <summary>
		/// Calculate the total agreed employer contribution of all eligible costs.
		/// </summary>s
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static decimal CalculateAgreedEmployerContribution(this GrantApplication grantApplication)
		{
			return grantApplication.TrainingCost.CalculateAgreedEmployerContribution();
		}

		/// <summary>
		/// Reset the eligible estimated cost calculations.
		/// This will also set all agreed values to 0.
		/// </summary>
		/// <param name="grantApplication"></param>
		public static void ResetEstimatedCosts(this GrantApplication grantApplication)
		{
			grantApplication.TrainingCost.ResetEstimatedCosts();
		}

		/// <summary>
		/// Calculate the estimated costs for the specified training program.
		/// </summary>
		/// <param name="grantApplication"></param>
		public static void RecalculateEstimatedCosts(this GrantApplication grantApplication)
		{
			grantApplication.TrainingCost.RecalculateEstimatedCosts();
		}

		/// <summary>
		/// Calculate the agreed cots for the specified training program.
		/// </summary>
		/// <param name="grantApplication"></param>
		public static void RecalculateAgreedCosts(this GrantApplication grantApplication)
		{
			grantApplication.TrainingCost.RecalculateAgreedCosts();
		}

		/// <summary>
		/// Copy the estimated values into the agreed values.
		/// </summary>
		/// <param name="grantApplication"></param>
		public static void CopyEstimatedIntoAgreed(this GrantApplication grantApplication)
		{
			grantApplication.TrainingCost.CopyEstimatedIntoAgreed();
		}

		/// <summary>
		/// Get the maximum amount of participants based on the grant application state.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static int GetMaxParticipants(this GrantApplication grantApplication)
		{
			return grantApplication.TrainingCost.GetMaxParticipants();
		}

		/// <summary>
		/// Calculate the total agreed cost of all eligible costs less any travel expenses.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static decimal AgreedNonTravelGovernmentReimbursement(this GrantApplication grantApplication)
		{
			return grantApplication.TrainingCost.AgreedNonTravelGovernmentReimbursement();
		}

		/// <summary>
		/// Calculate the total agreed cost of all eligible costs less any travel expenses.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static decimal EstimatedNonTravelGovernmentReimbursement(this GrantApplication grantApplication)
		{
			return grantApplication.TrainingCost.EstimatedNonTravelGovernmentReimbursement();
		}

		/// <summary>
		/// Calculate the amount that has been paid or owing for this grant application.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static decimal AmountPaidOrOwing(this GrantApplication grantApplication)
		{
			return grantApplication.Claims.Where(c => c.ClaimState.In(ClaimState.ClaimApproved, ClaimState.AmountOwing, ClaimState.AmountReceived, ClaimState.ClaimPaid, ClaimState.PaymentRequested)).Sum(c => c.TotalAssessedReimbursement);
		}

		/// <summary>
		/// GetProgramDescription
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static string GetProgramDescription(this GrantApplication grantApplication)
		{
			var descriptions = new Dictionary<ProgramTypes, string>
			{
				[ProgramTypes.EmployerGrant] = grantApplication?.TrainingPrograms?.FirstOrDefault()?.CourseTitle,
				[ProgramTypes.WDAService] = grantApplication?.ProgramDescription?.Description
			};
			if (!descriptions.ContainsKey(grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId))
				return string.Empty;

			return descriptions[grantApplication.GetProgramType()];
		}

		/// <summary>
		/// GetProviderLocation
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public static string GetProviderLocation(this GrantApplication grantApplication)
		{
			var defaultTrainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();

			var locations = new Dictionary<ProgramTypes, string>
			{
				[ProgramTypes.EmployerGrant] = "",
				[ProgramTypes.WDAService] = ""
			};

			if (!locations.ContainsKey(grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId))
				return string.Empty;

			if (grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.EmployerGrant
				&& defaultTrainingProgram?.TrainingProvider?.TrainingAddress != null)
			{
				locations[grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId] = $"{defaultTrainingProgram?.TrainingProvider?.TrainingAddress?.City}, {defaultTrainingProgram?.TrainingProvider?.TrainingAddress?.Region?.Name}, {defaultTrainingProgram?.TrainingProvider?.TrainingAddress?.Country?.Name}";
			}

			return locations[grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId];
		}
		#endregion
	}
}
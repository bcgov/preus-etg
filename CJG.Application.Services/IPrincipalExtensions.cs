using CJG.Core.Entities;
using CJG.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="IPrincipalExtensions"/> static class, provides extension methods for <typeparamref name="IPrincipal"/>.
	/// </summary>
	public static class IPrincipalExtensions
	{
		/// <summary>
		/// Get the application ID for the specified user.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public static int? GetUserId(this IPrincipal user)
		{
			var cp = user as ClaimsPrincipal;

			if (cp == null)
				return null;

			var identifier = cp.FindFirst(AppClaimTypes.UserId)?.Value;

			if (String.IsNullOrEmpty(identifier))
				return null;

			int id;

			if (int.TryParse(identifier, out id))
				return id;

			return null;
		}

		/// <summary>
		/// Get the BCeId GUID value for the specified user.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public static Guid? GetBCeIdGuid(this IPrincipal user)
		{
			var cp = user as ClaimsPrincipal;

			if (cp == null)
				return null;

			var identifier = cp.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

			if (String.IsNullOrEmpty(identifier))
				return null;

			Guid guid;

			if (Guid.TryParse(identifier, out guid))
				return guid;

			return null;
		}

		/// <summary>
		/// Get the user name for the specified user.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public static string GetUserName(this IPrincipal user)
		{
			var cp = user as ClaimsPrincipal;

			if (cp == null)
				return null;

			return cp.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
		}

		/// <summary>
		/// Get the Claim of the specified type.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="claimType"></param>
		/// <returns></returns>
		private static string GetClaimValue(IPrincipal user, string claimType)
		{
			return (user as ClaimsPrincipal)?.FindFirst(claimType)?.Value;
		}

		/// <summary>
		/// Get the First Name Claim.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public static string GetFirstName(this IPrincipal user)
		{
			return GetClaimValue(user, System.Security.Claims.ClaimTypes.GivenName);
		}

		/// <summary>
		/// Get the Organization Legal Name Claim.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public static string GetOrganizationLegalName(this IPrincipal user)
		{
			return GetClaimValue(user, AppClaimTypes.OrganizationName);
		}

		/// <summary>
		/// Get the Last Name Claim.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public static string GetLastName(this IPrincipal user)
		{
			return GetClaimValue(user, System.Security.Claims.ClaimTypes.Surname);
		}

		/// <summary>
		/// Get all the Privilege Claims for the specified user.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public static IEnumerable<Privilege> GetPrivileges(this IPrincipal user)
		{
			return ((ClaimsIdentity)user.Identity).Claims.Where(c => c.Type == AppClaimTypes.Privilege).Select(x => (Privilege)Enum.Parse(typeof(Privilege), x.Value)).ToList();
		}

		/// <summary>
		/// Determine if the specified user has the specified privilege claim.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="privilege"></param>
		/// <returns></returns>
		public static bool HasPrivilege(this IPrincipal user, Privilege privilege)
		{
			return GetPrivileges(user).Contains(privilege);
		}

		/// <summary>
		/// Determine if the specified user has any of the specified privilege claims.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="privileges"></param>
		/// <returns></returns>
		public static bool HasPrivilege(this IPrincipal user, params Privilege[] privileges)
		{
			return privileges.Intersect(GetPrivileges(user)).Any();
		}

		/// <summary>
		/// Determines if the specified user is allowed to perform the application workflow trigger on the grant application, in its current state.
		/// To determine whether a User can perform an action the user's privileges, the grant application state and the desired application workflow trigger must be evaluated together.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="grantApplication"></param>
		/// <param name="conditionalTrigger">A conditional function to determine which trigger to test.</param>
		/// <returns></returns>
		public static bool CanPerformAction(this IPrincipal user, GrantApplication grantApplication, Func<IPrincipal, GrantApplication, ApplicationWorkflowTrigger> conditionalTrigger)
		{
			return user.CanPerformAction(grantApplication, conditionalTrigger(user, grantApplication));
		}

		/// <summary>
		/// Determine if the specified user is allowed to perform the application workflow trigger on the grant application, in its current state.
		/// To determine whether a User can perform an action the user's privileges, the grant application state and the desired application workflow trigger must be evaluated together.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="grantApplication"></param>
		/// <param name="trigger"></param>
		/// <returns></returns>
		public static bool CanPerformAction(this IPrincipal user, GrantApplication grantApplication, ApplicationWorkflowTrigger trigger)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			// The user hasn't been identified.
			if (!(user is ClaimsPrincipal) || !user.Identity.IsAuthenticated)
				return false;

			// May not attempt an invalid application workflow trigger.
			var triggers = grantApplication.ApplicationStateInternal.GetValidWorkflowTriggers();
			if (!triggers.Contains(trigger))
				return false;

			var userId = user.GetUserId();
			var isApplicationAdministrator = user.GetAccountType() == AccountTypes.External && grantApplication.IsApplicationAdministrator(userId.Value);
			var currentClaim = grantApplication.GetCurrentClaim();
			var hasSubmittedAClaim = grantApplication.HasSubmittedAClaim();
			var hasApprovedClaim = (currentClaim?.HasPriorApprovedClaim() ?? false) || (currentClaim?.IsApproved() ?? false);
			var programType = grantApplication.GetProgramType();
			var claimType = grantApplication.GetClaimType();

			// Internal users are not allowed to perform external application workflow triggers.
			if (trigger.IsExternalWorkflowTrigger())
			{
				switch (trigger)
				{
					case (ApplicationWorkflowTrigger.SubmitApplication):
					case (ApplicationWorkflowTrigger.WithdrawClaim):
					case (ApplicationWorkflowTrigger.AcceptGrantAgreement):
					case (ApplicationWorkflowTrigger.RejectGrantAgreement):
					case (ApplicationWorkflowTrigger.SubmitChangeRequest):
					case (ApplicationWorkflowTrigger.SubmitCompletionReport):
						return isApplicationAdministrator;
					case (ApplicationWorkflowTrigger.SubmitClaim):
						return isApplicationAdministrator
							&& (claimType == Core.Entities.ClaimTypes.SingleAmendableClaim && grantApplication.ParticipantForms.Any() || claimType == Core.Entities.ClaimTypes.MultipleClaimsWithoutAmendments)
							&& currentClaim != null
							&& currentClaim.ClaimState == ClaimState.Complete;
					case (ApplicationWorkflowTrigger.CancelAgreementHolder):
						return isApplicationAdministrator && !hasSubmittedAClaim && !hasApprovedClaim;
					case (ApplicationWorkflowTrigger.EditParticipants):
						if (grantApplication.CanReportParticipants) return true;
						else if (claimType == Core.Entities.ClaimTypes.MultipleClaimsWithoutAmendments)
							return isApplicationAdministrator && !(currentClaim?.ClaimState.In(ClaimState.Unassessed) ?? false);
						else
							return isApplicationAdministrator && (currentClaim?.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete) ?? true);
					case (ApplicationWorkflowTrigger.WithdrawApplication):
						return isApplicationAdministrator && grantApplication.ApplicationStateExternal.In(ApplicationStateExternal.Submitted);
					default:
						return false;
				}
			}

			// Verify that the specified user is the Grant Application Assessor.
			var isAssessor = userId == grantApplication.AssessorId;

			switch (trigger)
			{
				case (ApplicationWorkflowTrigger.ViewApplication):
					return isApplicationAdministrator || user.HasPrivilege(Privilege.IA1);
				case (ApplicationWorkflowTrigger.ReassignAssessor):
					return user.HasPrivilege(Privilege.AM2, Privilege.AM3, Privilege.AM5);
				case (ApplicationWorkflowTrigger.ViewClaim):
					return isApplicationAdministrator || user.HasPrivilege(Privilege.IA1);
				case (ApplicationWorkflowTrigger.AddNote):
				case (ApplicationWorkflowTrigger.DeleteNote):
					return user.HasPrivilege(Privilege.AM1);
				case (ApplicationWorkflowTrigger.EditSummary):
					switch (grantApplication.ApplicationStateInternal)
					{
						case (ApplicationStateInternal.Draft):
						case (ApplicationStateInternal.ApplicationWithdrawn):
						case (ApplicationStateInternal.Unfunded):
							return isApplicationAdministrator;
						case (ApplicationStateInternal.New):
						case (ApplicationStateInternal.OfferIssued):
						case (ApplicationStateInternal.OfferWithdrawn):
						case (ApplicationStateInternal.CancelledByAgreementHolder):
						case (ApplicationStateInternal.CancelledByMinistry):
						case (ApplicationStateInternal.Closed):
						case (ApplicationStateInternal.AgreementRejected):
						case (ApplicationStateInternal.ReturnedUnassessed):
							return false;
						case (ApplicationStateInternal.ApplicationDenied):
							return user.HasPrivilege(Privilege.AM4);
						case (ApplicationStateInternal.PendingAssessment):
							return user.HasPrivilege(Privilege.AM2, Privilege.AM3);
						default:
							return user.HasPrivilege(Privilege.AM4) || (user.HasPrivilege(Privilege.AM2) && isAssessor);
					}
				case (ApplicationWorkflowTrigger.EditApplicantContact):
					switch (grantApplication.ApplicationStateInternal)
					{
						case (ApplicationStateInternal.Draft):
						case (ApplicationStateInternal.ApplicationWithdrawn):
							return isApplicationAdministrator;
						case (ApplicationStateInternal.Unfunded):
							return isApplicationAdministrator || user.HasPrivilege(Privilege.AM4);
						case (ApplicationStateInternal.New):
						case (ApplicationStateInternal.PendingAssessment):
							return user.HasPrivilege(Privilege.AM2, Privilege.AM3);
						case (ApplicationStateInternal.UnderAssessment):
						case (ApplicationStateInternal.RecommendedForDenial):
						case (ApplicationStateInternal.RecommendedForApproval):
						case (ApplicationStateInternal.ReturnedToAssessment):
						case (ApplicationStateInternal.ChangeRequest):
						case (ApplicationStateInternal.ChangeForApproval):
						case (ApplicationStateInternal.ChangeForDenial):
						case (ApplicationStateInternal.ChangeReturned):
							return user.HasPrivilege(Privilege.AM3) || (user.HasPrivilege(Privilege.AM2) && isAssessor);
						case (ApplicationStateInternal.Closed):
						case (ApplicationStateInternal.ReturnedUnassessed):
							return false;
						case (ApplicationStateInternal.ApplicationDenied):
						case (ApplicationStateInternal.OfferWithdrawn):
						case (ApplicationStateInternal.CancelledByAgreementHolder):
						case (ApplicationStateInternal.CancelledByMinistry):
							return user.HasPrivilege(Privilege.AM4);
						default:
							return user.HasPrivilege(Privilege.AM4) || (user.HasPrivilege(Privilege.AM2) && isAssessor);
					}
				// The difference between ChangeApplicantContact (below) and (above) is in the ChangeReturned and the default case.
				// The "Change Applicant Contact" button is enabled for all assessors.
				case (ApplicationWorkflowTrigger.ChangeApplicantContactButton):
					switch (grantApplication.ApplicationStateInternal)
					{
						case (ApplicationStateInternal.Draft):
						case (ApplicationStateInternal.ApplicationWithdrawn):
							return isApplicationAdministrator;
						case (ApplicationStateInternal.Unfunded):
							return isApplicationAdministrator || user.HasPrivilege(Privilege.AM4);
						case (ApplicationStateInternal.New):
						case (ApplicationStateInternal.PendingAssessment):
							return user.HasPrivilege(Privilege.AM2, Privilege.AM3);
						case (ApplicationStateInternal.UnderAssessment):
						case (ApplicationStateInternal.RecommendedForDenial):
						case (ApplicationStateInternal.RecommendedForApproval):
						case (ApplicationStateInternal.ReturnedToAssessment):
						case (ApplicationStateInternal.ChangeRequest):
						case (ApplicationStateInternal.ChangeForApproval):
						case (ApplicationStateInternal.ChangeForDenial):
						case (ApplicationStateInternal.ChangeReturned):
							return user.HasPrivilege(Privilege.AM3) || user.HasPrivilege(Privilege.AM2);
						case (ApplicationStateInternal.Closed):
						case (ApplicationStateInternal.ReturnedUnassessed):
							return false;
						case (ApplicationStateInternal.ApplicationDenied):
						case (ApplicationStateInternal.OfferWithdrawn):
						case (ApplicationStateInternal.CancelledByAgreementHolder):
						case (ApplicationStateInternal.CancelledByMinistry):
							return user.HasPrivilege(Privilege.AM4);
						default:
							return user.HasPrivilege(Privilege.AM4) || user.HasPrivilege(Privilege.AM2);
					}
				case ApplicationWorkflowTrigger.EditApplicant:
					switch (grantApplication.ApplicationStateInternal)
					{
						case ApplicationStateInternal.Draft:
						case ApplicationStateInternal.ApplicationWithdrawn:
						case ApplicationStateInternal.Unfunded:
							return isApplicationAdministrator;
						case ApplicationStateInternal.New:
						case ApplicationStateInternal.OfferIssued:
						case ApplicationStateInternal.OfferWithdrawn:
						case ApplicationStateInternal.Closed:
						case ApplicationStateInternal.AgreementRejected:
						case ApplicationStateInternal.CancelledByAgreementHolder:
						case ApplicationStateInternal.CancelledByMinistry:
						case ApplicationStateInternal.CompletionReporting:
						case ApplicationStateInternal.ApplicationDenied:
						case ApplicationStateInternal.ReturnedUnassessed:
							return false;
						default:
							return user.HasPrivilege(Privilege.AM4);
					}
				case (ApplicationWorkflowTrigger.EditApplication): // TODO: This needs to be removed and replaced by the specific workflow triggers.
					switch (grantApplication.ApplicationStateInternal)
					{
						case (ApplicationStateInternal.Draft):
						case (ApplicationStateInternal.ApplicationWithdrawn):
						case (ApplicationStateInternal.Unfunded):
							return isApplicationAdministrator;
						case (ApplicationStateInternal.New):
						case (ApplicationStateInternal.PendingAssessment):
						case (ApplicationStateInternal.RecommendedForApproval):
						case (ApplicationStateInternal.RecommendedForDenial):
						case (ApplicationStateInternal.ChangeForApproval):
						case (ApplicationStateInternal.ChangeForDenial):
							return user.HasPrivilege(Privilege.AM3);
						case (ApplicationStateInternal.ChangeRequest):
							return !hasSubmittedAClaim && isApplicationAdministrator || user.HasPrivilege(Privilege.AM2);
						case (ApplicationStateInternal.AgreementAccepted): // Needed for v1.10 to handle the applicant changing the training dates in the agreement.
						case (ApplicationStateInternal.ChangeRequestDenied):
						case (ApplicationStateInternal.ChangeReturned):
						case (ApplicationStateInternal.ClaimReturnedToApplicant):
						case (ApplicationStateInternal.ClaimDenied):
							return (isApplicationAdministrator && !hasApprovedClaim) || user.HasPrivilege(Privilege.AM4);
						case (ApplicationStateInternal.ReturnedUnassessed):
							return false;
						default:
							return user.HasPrivilege(Privilege.AM4) || (user.HasPrivilege(Privilege.AM2) && isAssessor);
					}
				case (ApplicationWorkflowTrigger.EditApplicationAttachments):
					switch (grantApplication.ApplicationStateInternal)
					{
						case (ApplicationStateInternal.Draft):
						case (ApplicationStateInternal.ApplicationWithdrawn):
						case (ApplicationStateInternal.Unfunded):
							return isApplicationAdministrator;
						case (ApplicationStateInternal.New):
						case (ApplicationStateInternal.Closed):
						case (ApplicationStateInternal.OfferIssued):
						case (ApplicationStateInternal.OfferWithdrawn):
						case (ApplicationStateInternal.AgreementRejected):
						case (ApplicationStateInternal.CancelledByAgreementHolder):
						case (ApplicationStateInternal.CancelledByMinistry):
						case (ApplicationStateInternal.ApplicationDenied):
						case (ApplicationStateInternal.ReturnedUnassessed):
							return false; // No one is allowed to edit attachments now.
						case (ApplicationStateInternal.PendingAssessment):
							return user.HasPrivilege(Privilege.AM2, Privilege.AM3);
						case (ApplicationStateInternal.RecommendedForApproval):
						case (ApplicationStateInternal.RecommendedForDenial):
						case (ApplicationStateInternal.ChangeForApproval):
						case (ApplicationStateInternal.ChangeForDenial):
							return user.HasPrivilege(Privilege.AM3);
						case (ApplicationStateInternal.AgreementAccepted):
							return user.HasPrivilege(Privilege.AM4);
						default:
							return user.HasPrivilege(Privilege.AM4) || (user.HasPrivilege(Privilege.AM2) && isAssessor);
					}
				case (ApplicationWorkflowTrigger.EditProgramDescription):
					switch (grantApplication.ApplicationStateInternal)
					{
						case (ApplicationStateInternal.Draft):
						case (ApplicationStateInternal.ApplicationWithdrawn):
						case (ApplicationStateInternal.Unfunded):
							return isApplicationAdministrator;
						case (ApplicationStateInternal.New):
						case (ApplicationStateInternal.Closed):
						case (ApplicationStateInternal.AgreementRejected):
						case (ApplicationStateInternal.OfferIssued):
						case (ApplicationStateInternal.OfferWithdrawn):
						case (ApplicationStateInternal.CancelledByAgreementHolder):
						case (ApplicationStateInternal.CancelledByMinistry):
						case (ApplicationStateInternal.ApplicationDenied):
						case (ApplicationStateInternal.CompletionReporting):
						case (ApplicationStateInternal.ReturnedUnassessed):
							return false;
						case (ApplicationStateInternal.PendingAssessment):
						case (ApplicationStateInternal.RecommendedForApproval):
						case (ApplicationStateInternal.RecommendedForDenial):
						case (ApplicationStateInternal.ChangeForApproval):
						case (ApplicationStateInternal.ChangeForDenial):
							return user.HasPrivilege(Privilege.AM3);
						case (ApplicationStateInternal.UnderAssessment):
						case (ApplicationStateInternal.ReturnedToAssessment):
						case (ApplicationStateInternal.ChangeRequest):
						case (ApplicationStateInternal.ChangeReturned):
						case (ApplicationStateInternal.ChangeRequestDenied):
							return (user.HasPrivilege(Privilege.AM3) || (user.HasPrivilege(Privilege.AM2) && isAssessor));
						case (ApplicationStateInternal.NewClaim):
						case (ApplicationStateInternal.ClaimAssessEligibility):
						case (ApplicationStateInternal.ClaimAssessReimbursement):
							return user.HasPrivilege(Privilege.AM4) || (user.HasPrivilege(Privilege.AM2) && isAssessor);
						default:
							return user.HasPrivilege(Privilege.AM4);
					}
				case (ApplicationWorkflowTrigger.ValidateTrainingProvider):
					switch (grantApplication.ApplicationStateInternal)
					{
						case (ApplicationStateInternal.New):
						case (ApplicationStateInternal.Draft):
						case (ApplicationStateInternal.ApplicationWithdrawn):
						case (ApplicationStateInternal.Closed):
						case (ApplicationStateInternal.Unfunded):
						case (ApplicationStateInternal.AgreementRejected):
						case (ApplicationStateInternal.CancelledByAgreementHolder):
						case (ApplicationStateInternal.CancelledByMinistry):
						case (ApplicationStateInternal.OfferIssued):
						case (ApplicationStateInternal.OfferWithdrawn):
						case (ApplicationStateInternal.NewClaim):
						case (ApplicationStateInternal.ClaimAssessEligibility):
						case (ApplicationStateInternal.ClaimAssessReimbursement):
						case (ApplicationStateInternal.ClaimDenied):
						case (ApplicationStateInternal.ClaimApproved):
						case (ApplicationStateInternal.CompletionReporting):
						case (ApplicationStateInternal.ApplicationDenied):
						case (ApplicationStateInternal.ReturnedUnassessed):
							return false; // No one is allowed to validate training providers now.
						case (ApplicationStateInternal.PendingAssessment):
						case (ApplicationStateInternal.RecommendedForApproval):
						case (ApplicationStateInternal.RecommendedForDenial):
						case (ApplicationStateInternal.ChangeForApproval):
						case (ApplicationStateInternal.ChangeForDenial):
							return user.HasPrivilege(Privilege.AM3);
						case (ApplicationStateInternal.UnderAssessment):
						case (ApplicationStateInternal.ReturnedToAssessment):
						case (ApplicationStateInternal.ChangeRequest):
						case (ApplicationStateInternal.ChangeReturned):
							return (user.HasPrivilege(Privilege.AM3) || (user.HasPrivilege(Privilege.AM2) && isAssessor));
						default:
							return user.HasPrivilege(Privilege.AM4);
					}
				case (ApplicationWorkflowTrigger.EditTrainingProvider):
					switch (grantApplication.ApplicationStateInternal)
					{
						case (ApplicationStateInternal.Draft):
						case (ApplicationStateInternal.ApplicationWithdrawn):
						case (ApplicationStateInternal.Unfunded):
							return isApplicationAdministrator;
						case (ApplicationStateInternal.New):
						case (ApplicationStateInternal.Closed):
						case (ApplicationStateInternal.AgreementRejected):
						case (ApplicationStateInternal.CancelledByAgreementHolder):
						case (ApplicationStateInternal.CancelledByMinistry):
						case (ApplicationStateInternal.OfferIssued):
						case (ApplicationStateInternal.OfferWithdrawn):
						case (ApplicationStateInternal.CompletionReporting):
						case (ApplicationStateInternal.ApplicationDenied):
						case (ApplicationStateInternal.ReturnedUnassessed):
							return false; // No one is allowed to edit training providers now.
						case (ApplicationStateInternal.PendingAssessment):
						case (ApplicationStateInternal.RecommendedForApproval):
						case (ApplicationStateInternal.RecommendedForDenial):
						case (ApplicationStateInternal.ChangeForApproval):
						case (ApplicationStateInternal.ChangeForDenial):
							return !grantApplication.RequiresTrainingProviderValidation() && user.HasPrivilege(Privilege.AM3);
						case (ApplicationStateInternal.UnderAssessment):
						case (ApplicationStateInternal.ReturnedToAssessment):
						case (ApplicationStateInternal.ChangeRequest):
						case (ApplicationStateInternal.ChangeReturned):
							// If there are requested training providers then they must be validated before any edits can be made to any training providers.
							return !grantApplication.RequiresTrainingProviderValidation() && (user.HasPrivilege(Privilege.AM3) || (user.HasPrivilege(Privilege.AM2) && isAssessor));
						default:
							return !grantApplication.RequiresTrainingProviderValidation() && user.HasPrivilege(Privilege.AM4);
					}
				case (ApplicationWorkflowTrigger.AddRemoveTrainingProvider):
					switch (grantApplication.ApplicationStateInternal)
					{
						case (ApplicationStateInternal.Draft):
						case (ApplicationStateInternal.ApplicationWithdrawn):
						case (ApplicationStateInternal.Unfunded):
							return isApplicationAdministrator;
						case (ApplicationStateInternal.AgreementAccepted):
						case (ApplicationStateInternal.ChangeRequestDenied):
							return (isApplicationAdministrator || user.HasPrivilege(Privilege.AM4)) && (programType == ProgramTypes.WDAService || !hasSubmittedAClaim);
						case (ApplicationStateInternal.New):
						case (ApplicationStateInternal.Closed):
						case (ApplicationStateInternal.AgreementRejected):
						case (ApplicationStateInternal.CancelledByAgreementHolder):
						case (ApplicationStateInternal.CancelledByMinistry):
						case (ApplicationStateInternal.OfferIssued):
						case (ApplicationStateInternal.OfferWithdrawn):
						case (ApplicationStateInternal.CompletionReporting):
						case (ApplicationStateInternal.ApplicationDenied):
						case (ApplicationStateInternal.NewClaim): // During a current claim don't allow adding or removing training providers.
						case (ApplicationStateInternal.ClaimAssessEligibility):
						case (ApplicationStateInternal.ClaimAssessReimbursement):
						case (ApplicationStateInternal.ReturnedUnassessed):
							return false; // No one is allowed to add/remove training providers now.
						case (ApplicationStateInternal.PendingAssessment):
						case (ApplicationStateInternal.RecommendedForApproval):
						case (ApplicationStateInternal.RecommendedForDenial):
						case (ApplicationStateInternal.ChangeForApproval):
						case (ApplicationStateInternal.ChangeForDenial):
							return user.HasPrivilege(Privilege.AM3);
						case (ApplicationStateInternal.UnderAssessment):
						case (ApplicationStateInternal.ReturnedToAssessment):
						case (ApplicationStateInternal.ChangeRequest):
						case (ApplicationStateInternal.ChangeReturned):
							return user.HasPrivilege(Privilege.AM3) || (user.HasPrivilege(Privilege.AM2) && isAssessor);
						default:
							return user.HasPrivilege(Privilege.AM4);
					}
				case (ApplicationWorkflowTrigger.AddOrRemoveTrainingProgram):
					switch (grantApplication.ApplicationStateInternal)
					{
						case (ApplicationStateInternal.Draft):
						case (ApplicationStateInternal.ApplicationWithdrawn):
						case (ApplicationStateInternal.Unfunded):
							return isApplicationAdministrator;
						case (ApplicationStateInternal.New):
						case (ApplicationStateInternal.Closed):
						case (ApplicationStateInternal.AgreementRejected):
						case (ApplicationStateInternal.CancelledByAgreementHolder):
						case (ApplicationStateInternal.CancelledByMinistry):
						case (ApplicationStateInternal.OfferIssued):
						case (ApplicationStateInternal.OfferWithdrawn):
						case (ApplicationStateInternal.CompletionReporting):
						case (ApplicationStateInternal.ApplicationDenied):
						case (ApplicationStateInternal.NewClaim): // During a current claim don't allow adding or removing training programs.
						case (ApplicationStateInternal.ClaimAssessEligibility):
						case (ApplicationStateInternal.ClaimAssessReimbursement):
						case (ApplicationStateInternal.ReturnedUnassessed):
							return false; // No one is allowed to add/remove training programs now.
						case (ApplicationStateInternal.PendingAssessment):
						case (ApplicationStateInternal.RecommendedForApproval):
						case (ApplicationStateInternal.RecommendedForDenial):
						case (ApplicationStateInternal.ChangeForApproval):
						case (ApplicationStateInternal.ChangeForDenial):
							return user.HasPrivilege(Privilege.AM3);
						case (ApplicationStateInternal.UnderAssessment):
						case (ApplicationStateInternal.ReturnedToAssessment):
							return user.HasPrivilege(Privilege.AM3) || (user.HasPrivilege(Privilege.AM2) && isAssessor);
						default:
							return user.HasPrivilege(Privilege.AM4);
					}
				case (ApplicationWorkflowTrigger.EditTrainingProgram):
					switch (grantApplication.ApplicationStateInternal)
					{
						case (ApplicationStateInternal.Draft):
						case (ApplicationStateInternal.ApplicationWithdrawn):
						case (ApplicationStateInternal.Unfunded):
							return isApplicationAdministrator;
						case (ApplicationStateInternal.New):
						case (ApplicationStateInternal.Closed):
						case (ApplicationStateInternal.AgreementRejected):
						case (ApplicationStateInternal.CancelledByAgreementHolder):
						case (ApplicationStateInternal.CancelledByMinistry):
						case (ApplicationStateInternal.OfferIssued):
						case (ApplicationStateInternal.OfferWithdrawn):
						case (ApplicationStateInternal.CompletionReporting):
						case (ApplicationStateInternal.ApplicationDenied):
						case (ApplicationStateInternal.ReturnedUnassessed):
							return false; // No one is allowed to edit training programs now.
						case (ApplicationStateInternal.PendingAssessment):
						case (ApplicationStateInternal.RecommendedForApproval):
						case (ApplicationStateInternal.RecommendedForDenial):
						case (ApplicationStateInternal.ChangeForApproval):
						case (ApplicationStateInternal.ChangeForDenial):
							return user.HasPrivilege(Privilege.AM3);
						case (ApplicationStateInternal.UnderAssessment):
						case (ApplicationStateInternal.ReturnedToAssessment):
						case (ApplicationStateInternal.ChangeRequest):
						case (ApplicationStateInternal.ChangeReturned):
							return user.HasPrivilege(Privilege.AM3) || (user.HasPrivilege(Privilege.AM2) && isAssessor);
						case (ApplicationStateInternal.AgreementAccepted):
							return user.HasPrivilege(Privilege.AM4) || isAssessor;
						default:
							return user.HasPrivilege(Privilege.AM4);
					}
				case (ApplicationWorkflowTrigger.EditTrainingCosts):
					switch (grantApplication.ApplicationStateInternal)
					{
						case (ApplicationStateInternal.Draft):
						case (ApplicationStateInternal.ApplicationWithdrawn):
						case (ApplicationStateInternal.Unfunded):
							return isApplicationAdministrator;
						case (ApplicationStateInternal.New):
						case (ApplicationStateInternal.Closed):
						case (ApplicationStateInternal.AgreementRejected):
						case (ApplicationStateInternal.CancelledByAgreementHolder):
						case (ApplicationStateInternal.CancelledByMinistry):
						case (ApplicationStateInternal.OfferIssued):
						case (ApplicationStateInternal.OfferWithdrawn):
						case (ApplicationStateInternal.CompletionReporting):
						case (ApplicationStateInternal.ApplicationDenied):
						case (ApplicationStateInternal.NewClaim): // During a current claim don't allow editing costs.
						case (ApplicationStateInternal.ClaimAssessEligibility):
						case (ApplicationStateInternal.ClaimAssessReimbursement):
						case (ApplicationStateInternal.ReturnedUnassessed):
							return false; // No one is allowed to add/remove training costs now.
						case (ApplicationStateInternal.PendingAssessment):
						case (ApplicationStateInternal.RecommendedForApproval):
						case (ApplicationStateInternal.RecommendedForDenial):
							return user.HasPrivilege(Privilege.AM3);
						case (ApplicationStateInternal.UnderAssessment):
						case (ApplicationStateInternal.ReturnedToAssessment):
							return user.HasPrivilege(Privilege.AM3) || (user.HasPrivilege(Privilege.AM2) && isAssessor);
						default:
							return user.HasPrivilege(Privilege.AM4) && !hasSubmittedAClaim;
					}
				case (ApplicationWorkflowTrigger.EditTrainingCostOverride):
					switch (grantApplication.ApplicationStateInternal)
					{
						case (ApplicationStateInternal.New):
							return false;
						default:
							return user.HasPrivilege(Privilege.AM4);
					}
				case (ApplicationWorkflowTrigger.CreateClaim):
					switch (grantApplication.ApplicationStateInternal)
					{
						case (ApplicationStateInternal.AgreementAccepted):
							return (isApplicationAdministrator || user.HasPrivilege(Privilege.AM3))
								&& ((currentClaim == null && (grantApplication.ParticipantForms.Any() && claimType == Core.Entities.ClaimTypes.SingleAmendableClaim || claimType == Core.Entities.ClaimTypes.MultipleClaimsWithoutAmendments))
									|| (claimType == Core.Entities.ClaimTypes.MultipleClaimsWithoutAmendments && (currentClaim?.ClaimState.In(ClaimState.ClaimApproved, ClaimState.ClaimDenied, ClaimState.AmountOwing, ClaimState.AmountReceived, ClaimState.PaymentRequested, ClaimState.ClaimPaid) ?? true)));
						case (ApplicationStateInternal.ClaimDenied):
						case (ApplicationStateInternal.ClaimApproved):
							return isApplicationAdministrator
								&& claimType == Core.Entities.ClaimTypes.MultipleClaimsWithoutAmendments && (currentClaim?.ClaimState.In(ClaimState.ClaimDenied, ClaimState.ClaimPaid, ClaimState.ClaimApproved, ClaimState.AmountOwing, ClaimState.AmountReceived, ClaimState.PaymentRequested) ?? true);
						default:
							return false;
					}
				case (ApplicationWorkflowTrigger.EditClaim):
					switch (grantApplication.ApplicationStateInternal)
					{
						case (ApplicationStateInternal.AgreementAccepted):
						case (ApplicationStateInternal.ChangeRequest):
						case (ApplicationStateInternal.ChangeRequestDenied):
						case (ApplicationStateInternal.ChangeForApproval):
						case (ApplicationStateInternal.ChangeForDenial):
						case (ApplicationStateInternal.ChangeReturned):
						case (ApplicationStateInternal.ClaimReturnedToApplicant):
							return isApplicationAdministrator
								&& (grantApplication.ParticipantForms.Any() && claimType == Core.Entities.ClaimTypes.SingleAmendableClaim || claimType == Core.Entities.ClaimTypes.MultipleClaimsWithoutAmendments)
								&& (currentClaim?.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete) ?? true);
						case (ApplicationStateInternal.ClaimAssessEligibility):
							return user.HasPrivilege(Privilege.AM4) || (user.HasPrivilege(Privilege.AM2) && isAssessor);
						case (ApplicationStateInternal.ClaimAssessReimbursement):
							return user.HasPrivilege(Privilege.AM4) || (user.HasPrivilege(Privilege.AM5) && isAssessor);
						default:
							return false; // No one is allowed to edit the claim now.
					}
				// State change actions.
				case (ApplicationWorkflowTrigger.SelectForAssessment):
				case (ApplicationWorkflowTrigger.RemoveFromAssessment):
				case (ApplicationWorkflowTrigger.BeginAssessment):
					return user.HasPrivilege(Privilege.AM1, Privilege.AM2);
				case ApplicationWorkflowTrigger.ReturnUnderAssessmentToDraft:
					return user.HasPrivilege(Privilege.AM4);
				case (ApplicationWorkflowTrigger.RecommendForApproval):
				case (ApplicationWorkflowTrigger.RecommendForDenial):
				case (ApplicationWorkflowTrigger.RecommendChangeForApproval):
				case (ApplicationWorkflowTrigger.RecommendChangeForDenial):
					return user.HasPrivilege(Privilege.AM4) || (user.HasPrivilege(Privilege.AM2) && isAssessor);
				case (ApplicationWorkflowTrigger.ReturnToAssessment):
					return !grantApplication.ApplicationStateInternal.In(ApplicationStateInternal.ApplicationDenied)
						? user.HasPrivilege(Privilege.AM4) || user.HasPrivilege(Privilege.AM3) && isAssessor && !hasApprovedClaim
						: user.HasPrivilege(Privilege.AM3);
				case (ApplicationWorkflowTrigger.IssueOffer):
				case (ApplicationWorkflowTrigger.WithdrawOffer):
				case (ApplicationWorkflowTrigger.DenyApplication):
				case (ApplicationWorkflowTrigger.CancelAgreementMinistry):
					return user.HasPrivilege(Privilege.AM4) || ((user.HasPrivilege(Privilege.AM3) && isAssessor) && !hasApprovedClaim);
				case ApplicationWorkflowTrigger.ReverseAgreementCancelledByMinistry:
					return user.HasPrivilege(Privilege.AM4);
				case ApplicationWorkflowTrigger.ReturnWithdrawnOfferToAssessment:
					return user.HasPrivilege(Privilege.AM4);
				case ApplicationWorkflowTrigger.ReturnOfferToAssessment:
					return user.HasPrivilege(Privilege.AM4);
				case (ApplicationWorkflowTrigger.ReturnChangeToAssessment):
					return user.HasPrivilege(Privilege.AM4) || (user.HasPrivilege(Privilege.AM2) && isAssessor);
				case (ApplicationWorkflowTrigger.ApproveChangeRequest):
				case (ApplicationWorkflowTrigger.DenyChangeRequest):
					return user.HasPrivilege(Privilege.AM4);
				case (ApplicationWorkflowTrigger.SelectClaimForAssessment):
					return user.HasPrivilege(Privilege.AM1, Privilege.AM2, Privilege.AM3, Privilege.AM5);
				case (ApplicationWorkflowTrigger.RemoveClaimFromAssessment):
				case (ApplicationWorkflowTrigger.ReturnClaimToApplicant):
					switch (grantApplication.ApplicationStateInternal)
					{
						case (ApplicationStateInternal.ClaimAssessReimbursement):
							return user.HasPrivilege(Privilege.AM5) && isAssessor;
						default:
							return user.HasPrivilege(Privilege.AM2, Privilege.AM5) && isAssessor;
					}
				case (ApplicationWorkflowTrigger.AssessReimbursement):
					return user.HasPrivilege(Privilege.AM2) && isAssessor;
				case (ApplicationWorkflowTrigger.DenyClaim):
					switch (grantApplication.ApplicationStateInternal)
					{
						case (ApplicationStateInternal.ClaimAssessReimbursement):
							return user.HasPrivilege(Privilege.AM5) && isAssessor;
						default:
							return user.HasPrivilege(Privilege.AM2, Privilege.AM5) && isAssessor;
					}
				case (ApplicationWorkflowTrigger.AmendClaim):
					return claimType == Core.Entities.ClaimTypes.SingleAmendableClaim && ((isApplicationAdministrator && (currentClaim?.ClaimState.In(ClaimState.ClaimDenied) ?? false))
						|| (user.HasPrivilege(Privilege.AM2, Privilege.AM5) && isAssessor && !(currentClaim?.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete, ClaimState.Unassessed) ?? true))
						|| (user.HasPrivilege(Privilege.AM3) && !(currentClaim?.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete, ClaimState.Unassessed) ?? true)));
				case (ApplicationWorkflowTrigger.AssessEligibility):
				case (ApplicationWorkflowTrigger.ApproveClaim):
					return user.HasPrivilege(Privilege.AM5) && isAssessor;
				case (ApplicationWorkflowTrigger.ReturnUnfundedApplications):
					return user.HasPrivilege(Privilege.GM1);
				case (ApplicationWorkflowTrigger.ReturnUnassessed):
					return user.HasPrivilege(Privilege.GM1);
				case ApplicationWorkflowTrigger.ReturnUnassessedToNew:
					return user.HasPrivilege(Privilege.AM4);
				case (ApplicationWorkflowTrigger.CloseClaimReporting):
					return (user.HasPrivilege(Privilege.AM3) || (user.HasPrivilege(Privilege.AM2, Privilege.AM5) && isAssessor)) && hasSubmittedAClaim;
				case (ApplicationWorkflowTrigger.Close):
				case (ApplicationWorkflowTrigger.EnableClaimReporting):
				case (ApplicationWorkflowTrigger.EnableCompletionReporting):
					return (user.HasPrivilege(Privilege.AM3) || (user.HasPrivilege(Privilege.AM2, Privilege.AM5) && isAssessor));
				case (ApplicationWorkflowTrigger.GeneratePaymentRequest):
					return user.HasPrivilege(Privilege.PM1);
				case (ApplicationWorkflowTrigger.ViewParticipants):
					return isApplicationAdministrator || user.HasPrivilege(Privilege.IA2);
				case ApplicationWorkflowTrigger.UpdateParticipants:
					return user.HasPrivilege(Privilege.AM4) || (user.HasPrivilege(Privilege.AM2) && isAssessor);
				case (ApplicationWorkflowTrigger.EnableParticipantReporting):
					return (user.HasPrivilege(Privilege.AM4) || (user.HasPrivilege(Privilege.AM2) && isAssessor))
						&& claimType == Core.Entities.ClaimTypes.MultipleClaimsWithoutAmendments;
				case (ApplicationWorkflowTrigger.EnableApplicantReportingOfParticipants):
					return grantApplication.GrantOpening.GrantStream.CanApplicantReportParticipants && (user.HasPrivilege(Privilege.AM4) || (user.HasPrivilege(Privilege.AM2) && isAssessor));
				case (ApplicationWorkflowTrigger.HoldPaymentRequests):
					return user.HasPrivilege(Privilege.AM2, Privilege.AM3);
				default:
					return false;
			}
		}

		/// <summary>
		/// Determine if the specified user is allowed to perform the application workflow trigger on the grant application, in its current state.
		/// To determine whether a User can perform an action the user's privileges, the grant application state and the desired application workflow trigger must be evaluated together.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="trainingProvider"></param>
		/// <param name="trigger"></param>
		/// <returns></returns>
		public static bool CanPerformAction(this IPrincipal user, TrainingProvider trainingProvider, ApplicationWorkflowTrigger trigger)
		{
			if (trainingProvider == null) throw new ArgumentNullException(nameof(trainingProvider));
			var grantApplication = trainingProvider.GetGrantApplication();

			// The user hasn't been identified.
			if (!(user is ClaimsPrincipal) || !user.Identity.IsAuthenticated)
				return false;

			// May not attempt an invalid application workflow trigger.
			var triggers = grantApplication.ApplicationStateInternal.GetValidWorkflowTriggers();
			if (!triggers.Contains(trigger))
				return false;

			var userId = user.GetUserId();
			var isApplicationAdministrator = user.GetAccountType() == AccountTypes.External && grantApplication.IsApplicationAdministrator(userId.Value);
			var isAssessor = userId == grantApplication.AssessorId;

			switch (trigger)
			{
				// If there are requested training providers then they must be validated before any edits can be made to any training providers.
				case (ApplicationWorkflowTrigger.EditTrainingProvider):
					switch (grantApplication.ApplicationStateInternal)
					{
						case (ApplicationStateInternal.Draft):
						case (ApplicationStateInternal.ApplicationWithdrawn):
						case (ApplicationStateInternal.Unfunded):
							return isApplicationAdministrator;
						case (ApplicationStateInternal.New):
						case (ApplicationStateInternal.Closed):
						case (ApplicationStateInternal.AgreementRejected):
						case (ApplicationStateInternal.CancelledByAgreementHolder):
						case (ApplicationStateInternal.CancelledByMinistry):
						case (ApplicationStateInternal.ApplicationDenied):
						case (ApplicationStateInternal.CompletionReporting):
						case (ApplicationStateInternal.ReturnedUnassessed):
							return false; // No one is allowed to edit training providers now.
						case (ApplicationStateInternal.PendingAssessment):
						case (ApplicationStateInternal.RecommendedForApproval):
						case (ApplicationStateInternal.RecommendedForDenial):
						case (ApplicationStateInternal.ChangeForApproval):
						case (ApplicationStateInternal.ChangeForDenial):
							return trainingProvider.TrainingProviderInventoryId != null && user.HasPrivilege(Privilege.AM3);
						case (ApplicationStateInternal.UnderAssessment):
						case (ApplicationStateInternal.ReturnedToAssessment):
						case (ApplicationStateInternal.ChangeReturned):
							return trainingProvider.TrainingProviderInventoryId != null && (user.HasPrivilege(Privilege.AM3) || (user.HasPrivilege(Privilege.AM2) && isAssessor));
						case (ApplicationStateInternal.ChangeRequest):
							// If newly requested and valid provider, allow director and assessor to edit
							return trainingProvider.TrainingProviderState.In(TrainingProviderStates.Complete, TrainingProviderStates.Requested, TrainingProviderStates.RequestApproved, TrainingProviderStates.RequestDenied)
								&& trainingProvider.TrainingProviderInventoryId != null
								&& (user.HasPrivilege(Privilege.AM3) || (user.HasPrivilege(Privilege.AM2) && isAssessor));
						case (ApplicationStateInternal.AgreementAccepted):
						case (ApplicationStateInternal.ChangeRequestDenied):
							return (isApplicationAdministrator && trainingProvider.TrainingProviderState == TrainingProviderStates.Requested)
								|| user.HasPrivilege(Privilege.AM4);
						case (ApplicationStateInternal.ClaimReturnedToApplicant):
						case (ApplicationStateInternal.ClaimDenied):
						case (ApplicationStateInternal.ClaimApproved):
							return user.HasPrivilege(Privilege.AM4)
								|| (isApplicationAdministrator && trainingProvider.TrainingProviderState.In(TrainingProviderStates.Complete, TrainingProviderStates.Requested));
						default:
							return user.HasPrivilege(Privilege.AM4);
					}
				case (ApplicationWorkflowTrigger.AddRemoveTrainingProvider):
					switch (grantApplication.ApplicationStateInternal)
					{
						case (ApplicationStateInternal.Draft):
						case (ApplicationStateInternal.ApplicationWithdrawn):
							return isApplicationAdministrator;
						case (ApplicationStateInternal.New):
						case (ApplicationStateInternal.Closed):
						case (ApplicationStateInternal.ApplicationDenied):
						case (ApplicationStateInternal.AgreementRejected):
						case (ApplicationStateInternal.CancelledByAgreementHolder):
						case (ApplicationStateInternal.CancelledByMinistry):
						case (ApplicationStateInternal.CompletionReporting):
						case (ApplicationStateInternal.ReturnedUnassessed):
							return false; // No one is allowed to add/remove training providers now.
						case (ApplicationStateInternal.PendingAssessment):
							return user.HasPrivilege(Privilege.AM3);
						case (ApplicationStateInternal.UnderAssessment):
						case (ApplicationStateInternal.ReturnedToAssessment):
						case (ApplicationStateInternal.ChangeRequest):
						case (ApplicationStateInternal.ChangeReturned):
							return (user.HasPrivilege(Privilege.AM3) || (user.HasPrivilege(Privilege.AM2) && isAssessor))
								&& trainingProvider.TrainingProviderState.In(TrainingProviderStates.Incomplete, TrainingProviderStates.Complete) // Not allowed to delete change requests.
								&& trainingProvider.RequestedTrainingProvider == null; // Not allowed to delete a provider with an active change request.
						case (ApplicationStateInternal.RecommendedForApproval):
						case (ApplicationStateInternal.RecommendedForDenial):
						case (ApplicationStateInternal.ChangeForApproval):
						case (ApplicationStateInternal.ChangeForDenial):
							return user.HasPrivilege(Privilege.AM3)
								&& trainingProvider.TrainingProviderState.In(TrainingProviderStates.Incomplete, TrainingProviderStates.Complete) // Not allowed to delete change requests.
								&& trainingProvider.RequestedTrainingProvider == null; // Not allowed to delete a provider with an active change request.
						case (ApplicationStateInternal.NewClaim):
						case (ApplicationStateInternal.ClaimAssessEligibility):
						case (ApplicationStateInternal.ClaimAssessReimbursement):
							return false; // During claims do not allow adding/removing training providers.
						default:
							return (isApplicationAdministrator && trainingProvider.TrainingProviderState.In(TrainingProviderStates.Requested))
								|| (user.HasPrivilege(Privilege.AM4)
									&& trainingProvider.TrainingProviderState.In(TrainingProviderStates.Incomplete, TrainingProviderStates.Complete) // Not allowed to delete change requests.
									&& trainingProvider.RequestedTrainingProvider == null); // Not allowed to delete a provider with an active change request.)
					}
			}
			return user.CanPerformAction(grantApplication, trigger);
		}
	}
}
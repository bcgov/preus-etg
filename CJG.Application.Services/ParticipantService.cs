﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using CJG.Core.Entities;
using CJG.Core.Interfaces;
using CJG.Core.Interfaces.Service;
using CJG.Core.Interfaces.Service.Settings;
using CJG.Infrastructure.Entities;
using NLog;

namespace CJG.Application.Services
{
	public class ParticipantService : Service, IParticipantService
	{
		private readonly INotificationService _notificationService;
		private readonly INotificationSettings _notificationSettings;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		/// <param name="notificationService"></param>
		/// <param name="notificationSettings"></param>
		public ParticipantService(IDataContext context, HttpContextBase httpContext, ILogger logger, INotificationService notificationService, INotificationSettings notificationSettings)
			: base(context, httpContext, logger)
		{
			_notificationService = notificationService;
			_notificationSettings = notificationSettings;
		}

		/// <summary>
		/// Add the specified <typeparamref name="ParticipantForm"/> if the invitation has not expired.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">The invitation has expired.</exception>
		/// <param name="participantForm"></param>
		public ParticipantForm Add(ParticipantForm participantForm)
		{
			if (participantForm == null)
				throw new ArgumentNullException(nameof(participantForm));

			var grantApplication = Get<GrantApplication>(participantForm.GrantApplicationId);

			if (grantApplication?.InvitationKey == null || grantApplication.IsInvitationExpired())
				throw new InvalidOperationException("The invitation key is invalid or has expired.");

			_dbContext.ParticipantForms.Add(participantForm);

			var claim = grantApplication.GetCurrentClaim();
			if (claim != null && claim.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete))
			{
				var numberOfParticipants = grantApplication.ParticipantForms.Count(p => !p.IsExcludedFromClaim);

				var programType = grantApplication.GetProgramType();
				foreach (var claimEligibleCost in claim.EligibleCosts)
				{
					// Add the participant to each eligible expense that must be participant assigned.
					if (claimEligibleCost.EligibleExpenseType.ExpenseTypeId == ExpenseTypes.ParticipantAssigned)
					{
						var newParticipantCost = new ParticipantCost(claimEligibleCost, participantForm);
						claimEligibleCost.ParticipantCosts.Add(newParticipantCost);
					}
					else if (claimEligibleCost.EligibleExpenseType.ExpenseTypeId == ExpenseTypes.NotParticipantLimited && programType == ProgramTypes.WDAService)
					{
						// Only allow the maximum agreed participants.
						if (numberOfParticipants > grantApplication.TrainingCost.AgreedParticipants)
						{
							if (grantApplication.TrainingCost.AgreedParticipants != claimEligibleCost.ClaimParticipants)
							{
								claimEligibleCost.ClaimParticipants = grantApplication.TrainingCost.AgreedParticipants;
							}
						}
						else if (numberOfParticipants != claimEligibleCost.ClaimParticipants)
						{
							claimEligibleCost.ClaimParticipants = numberOfParticipants;
						}
					}
				}

				claim.ClaimState = ClaimState.Incomplete;
				claim.RecalculateClaimedCosts();
				claim.RecalculateAssessedCosts(true);
			}

			_dbContext.CommitTransaction();
			return participantForm;
		}

		public ParticipantCost Add(ParticipantCost participantCost)
		{
			if (participantCost == null)
				throw new ArgumentNullException(nameof(participantCost));

			_dbContext.ParticipantCosts.Add(participantCost);
			_dbContext.Commit();

			return participantCost;
		}

		public ParticipantCost Update(ParticipantCost participantCost)
		{
			if (participantCost == null)
				throw new ArgumentNullException(nameof(participantCost));

			_dbContext.Update<ParticipantCost>(participantCost);
			_dbContext.Commit();

			return participantCost;
		}

		/// <summary>
		/// used when the assessor sets the Approved property for the participants
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="participantsApproved"></param>
		public void ApproveDenyParticipants(int grantApplicationId, Dictionary<int?, bool?> participantsApproved)
		{
			var grantApplication = _dbContext.GrantApplications.FirstOrDefault(w => w.Id == grantApplicationId);

			if (grantApplication == null)
			{
				return;
			}

			foreach (var participant in participantsApproved)
			{
				var pf = _dbContext.ParticipantForms.FirstOrDefault(w => w.Id == participant.Key.Value);
				if (pf != null)
				{
					pf.Approved = participant.Value;
					_dbContext.Update(pf);
				}
			}

			//do we need to adjust the training costs
			var totalApproved = participantsApproved.Count(w => w.Value.HasValue && w.Value.Value == true);

			//if the total approved does not match the agreed participant count
			if (totalApproved != grantApplication.TrainingCost.AgreedParticipants)
			{
				//adjust the costs if the maxAgreedParticipants is the same for all eligible costs and match the training costs agreed participant number
				var agreedParticipantsDoNotMatch = grantApplication.TrainingCost.EligibleCosts.Count(w => w.AgreedMaxParticipants != grantApplication.TrainingCost.AgreedParticipants);

				if (agreedParticipantsDoNotMatch == 0)
				{
					var reimbursementRate = (decimal)grantApplication.GrantOpening.GrantStream.ReimbursementRate;

					//auto adjust the agreed numbers based on the estimated numbers
					foreach (var ec in grantApplication.TrainingCost.EligibleCosts)
					{
						ec.AgreedMaxParticipants = totalApproved;
						ec.AgreedMaxParticipantCost = ec.AgreedMaxParticipantCost;

						ec.AgreedMaxCost = ec.AgreedMaxParticipants * ec.AgreedMaxParticipantCost;

						ec.AgreedMaxReimbursement = ec.AgreedMaxCost * reimbursementRate;
						ec.AgreedEmployerContribution = ec.AgreedMaxCost - ec.AgreedMaxReimbursement;

						ec.RecalculateEstimatedCost();
						ec.RecalculateAgreedCosts(ec.AgreedMaxParticipantCost);

						_dbContext.Update(ec);
					}

					grantApplication.TrainingCost.AgreedParticipants = totalApproved;
					grantApplication.TrainingCost.TotalAgreedMaxCost = grantApplication.TrainingCost.EligibleCosts.Sum(s => s.AgreedMaxCost);
					grantApplication.TrainingCost.AgreedCommitment = grantApplication.TrainingCost.EligibleCosts.Sum(s => s.AgreedMaxReimbursement);

					grantApplication.TrainingCost.RecalculateEstimatedCosts();
					grantApplication.TrainingCost.RecalculateAgreedCosts();

					_dbContext.Update(grantApplication.TrainingCost);

				}
			}

			_dbContext.Commit();
		}

		public void ReportAttendance(Dictionary<int, bool?> participantAttended)
		{
			foreach (var participant in participantAttended)
			{
				var pf = _dbContext.ParticipantForms.FirstOrDefault(w => w.Id == participant.Key);
				if (pf != null)
				{
					pf.Attended = participant.Value;
					_dbContext.Update(pf);
				}
			}
			_dbContext.Commit();
		}

		/// <summary>
		/// Get the participant form for the specified id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ParticipantForm Get(int id)
		{
			var participant = Get<ParticipantForm>(id);

			if (!_httpContext.User.CanPerformAction(participant.GrantApplication, ApplicationWorkflowTrigger.ViewParticipants))
				throw new NotAuthorizedException($"User does not have permission to view participants from grant application '{participant.GrantApplication}'.");

			return participant;
		}

		/// <summary>
		/// Get all the <typeparamref name="ParticipantForm"/> objects for the specified <typeparamref name="GrantApplciation"/>.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		public IEnumerable<ParticipantForm> GetParticipantFormsForGrantApplication(int grantApplicationId)
		{
			return Get<GrantApplication>(grantApplicationId).ParticipantForms.ToArray();
		}

		public IEnumerable<ParticipantCost> GetParticipantCostsForClaimEligibleCost(int eligibleCostId)
		{
			return _dbContext.ParticipantCosts.Where(pc => pc.ClaimEligibleCostId == eligibleCostId);
		}

		public IEnumerable<ParticipantCost> GetParticipantCosts(ClaimEligibleCost eligibleCost)
		{
			return _dbContext.ParticipantCosts.Where(pc => pc.ClaimEligibleCostId == eligibleCost.Id);
		}

		/// <summary>
		/// The number of participants that have costs associated with the specified Claim.
		/// </summary>
		/// <param name="claimId">The id of the Claim.</param>
		/// <param name="claimVersion">The version of the Claim.</param>
		/// <returns>Number of participants that have costs associated with the specified Claim.</returns>
		public int GetParticipantsWithClaimEligibleCostCount(int claimId, int claimVersion)
		{
			var claim = _dbContext.Claims.Include(c => c.EligibleCosts).Include("EligibleCosts.ParticipantCosts").FirstOrDefault(c => c.Id == claimId && c.ClaimVersion == claimVersion);

			if (claim == null)
				throw new InvalidOperationException("Claim version does not exist.");

			return claim.ParticipantsWithEligibleCosts();
		}

		/// <summary>
		/// Get enrollments for Unemployed participant who are receiving EI
		/// </summary>
		/// <param name="currentDate"></param>
		/// <param name="take"></param>
		/// <param name="cutoffDate"></param>
		/// <returns>Collection of ParticipantEnrollment</returns>
		public IEnumerable<ParticipantForm> GetUnemployedParticipantEnrollments(DateTime currentDate, int take, DateTime cutoffDate)
		{
			var currentDateUtc = currentDate.ToUniversalTime();
			var cutoffDateUtc = cutoffDate.ToUniversalTime();
			var defaultGrantProgramId = GetDefaultGrantProgramId();

			// Is Currently receiving EI benefits
			return (from pf in _dbContext.ParticipantForms
					join ga in _dbContext.GrantApplications on pf.GrantApplicationId equals ga.Id
					where
						pf.DateAdded >= cutoffDateUtc
						&& ga.GrantOpening.GrantStream.GrantProgramId == defaultGrantProgramId
						&& (pf.ReceivingEIBenefit || pf.EIBenefitId == 1)  // "Currently Receiving"
						&& (!pf.ReportedOn.HasValue || pf.ReportedOn > currentDateUtc)
						&& ga.ApplicationStateInternal != ApplicationStateInternal.Draft
					select pf
				).Include(x => x.GrantApplication)
				.Take(take);
		}

		/// <summary>
		/// Update ParticipantEnrolments with date when they were reported
		/// </summary>
		/// <param name="participantEnrollments"></param>
		/// <param name="reportedDate"></param>
		public void UpdateReportedDate(IEnumerable<ParticipantForm> participantEnrollments, DateTime reportedDate)
		{
			foreach (var participantEnrollment in participantEnrollments)
			{
				participantEnrollment.ReportedOn = reportedDate.ToUniversalTime();
			}

			_dbContext.Commit();
		}


		public void UpdateExpectedOutcome(ParticipantForm participant, ExpectedParticipantOutcome? modelExpectedOutcome)
		{
			var pf = _dbContext.ParticipantForms.FirstOrDefault(pf => pf.Id == participant.Id);
			if (pf != null)
			{
				pf.ExpectedParticipantOutcome = modelExpectedOutcome;
				_dbContext.Update(pf);
			}

			_dbContext.Commit();
		}

		/// <summary>
		/// Remove the participant from the grant application.
		/// Remove any costs associated with them.
		/// Remove an completion report answers associated with them.
		/// </summary>
		/// <param name="participant"></param>
		public void RemoveParticipant(ParticipantForm participant)
		{
			var grantApplication = participant.GrantApplication;

			if (!_httpContext.User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditParticipants))
				throw new NotAuthorizedException("Can only remove participants if there are no claims in a Denied or Approved state and there is only one existing claim in the Incomplete, Complete or ClaimReturnedToApplication state.");

			if (participant.ClaimReported)
				throw new NotAuthorizedException("The participant may not be removed from this application, it may be part of an prior claim.");

			participant.IsExcludedFromClaim = true;

			var claim = grantApplication.GetCurrentClaim();
			if (claim != null && claim.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete))
			{
				// Remove all participant costs associated with this participant.
				var participantCosts = _dbContext.ParticipantCosts
					.Where(pc => pc.ParticipantFormId == participant.Id
					             && pc.ClaimEligibleCost.ClaimId == claim.Id
					             && pc.ClaimEligibleCost.ClaimVersion == claim.ClaimVersion);

				foreach (var participantCost in participantCosts)
					_dbContext.ParticipantCosts.Remove(participantCost);

				claim.ClaimState = ClaimState.Incomplete;
				claim.RecalculateClaimedCosts();
				claim.RecalculateAssessedCosts();

				_dbContext.Update(claim);

				// CJG-476 - remove any related claim participants prior to removing the participant form
				var claimWithParticipants = _dbContext.Claims
					.Include(c => c.ParticipantForms)
					.FirstOrDefault(c => c.Id == claim.Id && c.ClaimVersion == claim.ClaimVersion);

				if (claimWithParticipants?.ParticipantForms?.Any() == true)
				{
					foreach (var participantForm in claimWithParticipants.ParticipantForms.ToArray())
					{
						claimWithParticipants.ParticipantForms.Remove(participantForm);
						_dbContext.ParticipantForms.Remove(participantForm);
					}
				}
			}

			// also need to check to see if this participant has been included on a completion report, in which case their answers need to be removed
			// get all of the answers submitted by the participant
			var participantAnswers = _dbContext.ParticipantCompletionReportAnswers.Where(pcra => pcra.ParticipantFormId == participant.Id);
			foreach (var participantAnswer in participantAnswers)
			{
				_dbContext.ParticipantCompletionReportAnswers.Remove(participantAnswer);
			}

			participant.EligibleCostBreakdowns.Clear();

			_dbContext.ParticipantForms.Remove(participant);
			_dbContext.CommitTransaction();
		}

		/// <summary>
		/// Include the participant and update the current claim.
		/// </summary>
		/// <param name="participant"></param>
		public void IncludeParticipant(ParticipantForm participant)
		{
			if (participant == null)
				throw new ArgumentNullException(nameof(participant));

			var grantApplication = participant.GrantApplication;

			if (!_httpContext.User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditParticipants))
				throw new NotAuthorizedException("User is not allowed to update participants.");

			participant.IsExcludedFromClaim = false;

			var claim = grantApplication.GetCurrentClaim();
			if (claim != null && claim.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete))
			{
				var numberOfParticipants = grantApplication.ParticipantForms.Count(p => !p.IsExcludedFromClaim);
				foreach (var claimEligibleCost in claim.EligibleCosts)
				{
					if (claimEligibleCost.EligibleExpenseType.ExpenseTypeId == ExpenseTypes.ParticipantAssigned)
					{
						// Only add them if they aren't already added.
						var costs = claimEligibleCost.ParticipantCosts.Any(pc => pc.ParticipantFormId == participant.Id);
						if (!costs)
						{
							var participantCost = new ParticipantCost(claimEligibleCost, participant.Id);
							claimEligibleCost.ParticipantCosts.Add(participantCost);
						}
					}
				}

				claim.ClaimState = ClaimState.Incomplete;
				claim.RecalculateClaimedCosts();
				claim.RecalculateAssessedCosts();
			}

			_dbContext.CommitTransaction();
		}

		/// <summary>
		/// Exclude the participant and update the current claim.
		/// </summary>
		/// <param name="participant"></param>
		public void ExcludeParticipant(ParticipantForm participant)
		{
			if (participant == null)
				throw new ArgumentNullException(nameof(participant));

			var grantApplication = participant.GrantApplication;

			if (!_httpContext.User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditParticipants))
				throw new NotAuthorizedException("User is not allowed to update participants.");

			participant.IsExcludedFromClaim = true;

			var claim = grantApplication.GetCurrentClaim();
			if (claim != null && claim.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete))
			{
				var numberOfParticipants = grantApplication.ParticipantForms.Count(p => !p.IsExcludedFromClaim);
				foreach (var claimEligibleCost in claim.EligibleCosts)
				{
					if (claimEligibleCost.EligibleExpenseType.ExpenseTypeId == ExpenseTypes.ParticipantAssigned)
					{
						// Remove participant costs from the claim.
						var participantCostIds = claimEligibleCost.ParticipantCosts.Where(pc => pc.ParticipantFormId == participant.Id).Select(pc => pc.Id).ToArray(); // It should only return one, but the database supports multiple.
						var participantCosts = _dbContext.ParticipantCosts.Where(pc => participantCostIds.Contains(pc.Id));
						participantCosts.ForEach(participantCost =>
						{
							Remove(participantCost);
						});
					}
				}

				claim.ClaimState = ClaimState.Incomplete;
				claim.RecalculateClaimedCosts();
				claim.RecalculateAssessedCosts();
			}

			_dbContext.CommitTransaction();
		}

		public IDictionary<string, decimal> GetParticipantYTD(GrantApplication grantApplication)
		{
			var enteredSiNs = grantApplication.ParticipantForms.Select(x => x.SIN).ToList();
			var defaultGrantProgramId = GetDefaultGrantProgramId();
			var applicationStates = new[]
			{
				ApplicationStateInternal.AgreementAccepted, ApplicationStateInternal.ChangeRequest,
				ApplicationStateInternal.ChangeForApproval, ApplicationStateInternal.ChangeForDenial,
				ApplicationStateInternal.ChangeReturned, ApplicationStateInternal.ChangeRequestDenied,
				ApplicationStateInternal.NewClaim,
				ApplicationStateInternal.ClaimAssessEligibility, ApplicationStateInternal.ClaimReturnedToApplicant,
				ApplicationStateInternal.ClaimDenied, ApplicationStateInternal.ClaimApproved,
				ApplicationStateInternal.Closed,
				ApplicationStateInternal.ClaimAssessReimbursement, ApplicationStateInternal.CompletionReporting
			};

			var queryParticipantCost = (from pc in _dbContext.ParticipantCosts
					join pf in _dbContext.ParticipantForms on pc.ParticipantFormId equals pf.Id
					join ga in _dbContext.GrantApplications on pf.GrantApplicationId equals ga.Id
					where applicationStates.Contains(ga.ApplicationStateInternal)
					      && ga.GrantOpening.GrantStream.GrantProgramId == defaultGrantProgramId
					      && ga.GrantOpening.TrainingPeriod.FiscalYearId == grantApplication.GrantOpening.TrainingPeriod.FiscalYearId
					      && enteredSiNs.Contains(pf.SIN)
					select new {pc, pf}
					into pg
					select pg)
				.ToList();

			return queryParticipantCost
				.GroupBy(o => o.pf.SIN)
				.Select(o => new { Amount = o.Sum(b => b.pc.AssessedReimbursement), SIN = o.Key })
				.ToDictionary(x => x.SIN, x => x.Amount);
		}

		public IEnumerable<ParticipantForm> GetParticipantFormsBySIN(string sin)
		{
			return _dbContext.ParticipantForms.Where(w => w.SIN == sin);
		}

		public ParticipantInvitation UpdateParticipantInvitation(ParticipantInvitation participantInvitation)
		{
				if (participantInvitation == null)
					throw new ArgumentNullException(nameof(participantInvitation));

				_dbContext.Update(participantInvitation);
				_dbContext.Commit();

				return participantInvitation;
		}

		public ParticipantInvitation RemoveParticipantInvitation(ParticipantInvitation participantInvitation)
		{
			if (participantInvitation == null)
				throw new ArgumentNullException(nameof(participantInvitation));

			participantInvitation.IndividualKey = Guid.NewGuid();
			participantInvitation.FirstName = null;
			participantInvitation.LastName = null;
			participantInvitation.EmailAddress = null;
			participantInvitation.ExpectedParticipantOutcome = 0;
			participantInvitation.ParticipantInvitationStatus = ParticipantInvitationStatus.Empty;

			if (participantInvitation.ParticipantForm != null)
			{
				participantInvitation.GrantApplication.ParticipantForms.Remove(participantInvitation.ParticipantForm);
				_dbContext.ParticipantForms.Remove(participantInvitation.ParticipantForm);
			}

			_dbContext.Update(participantInvitation);
			_dbContext.Commit();

			return participantInvitation;
		}

		public ParticipantInvitation SendParticipantInvitation(ParticipantInvitation participantInvitation)
		{
			if (participantInvitation == null)
				throw new ArgumentNullException(nameof(participantInvitation));

			var grantApplication = participantInvitation.GrantApplication;

//			var GrantProgramName = grantApplication?.GrantOpening?.GrantStream?.GrantProgram?.Name ?? throw new ArgumentNullException(nameof(grantApplication), "The argument 'grantApplication' must provide the grant program name.");
			var startDate = grantApplication.StartDate.ToLocalTime();
			var invitationBrowserLink = $"{_httpContext.Request.Url.GetLeftPart(UriPartial.Authority)}/Part/Information/{HttpUtility.UrlEncode(grantApplication.InvitationKey.ToString())}/{HttpUtility.UrlEncode(participantInvitation.IndividualKey.ToString())}";

			var inviteSubject = $"Invitation to Participate in {grantApplication.GetProgramDescription()} - Training Funded by the B.C. Employer Training Grant";
			var inviteBody = $@"<p>Dear {participantInvitation.FirstName} {participantInvitation.LastName},</p>
<p>You have been chosen by your employer to participate in {grantApplication.GetProgramDescription()} training starting on {startDate.ToLocalMorning():yyyy-MM-dd}. This training will be funded by your employer as well as through the B.C. Employer Training Grant program. If approved, your training expenses will be fully covered, and you will not be required to pay anything.</p>
<p>To make sure you are eligible, please complete your Participant Information Form (PIF) as soon as possible. Click the following link using the Chrome or Firefox browser: <a href='{invitationBrowserLink}'>{invitationBrowserLink}</a></p>
<p>For questions, please contact your employer. We are also available to answer any questions at <a href='mailto: ETG@gov.bc.ca'>ETG@gov.bc.ca</a>.</p>
<p>Thank you,</p>
<p>The B.C. Employer Training Grant</p>";

			var sender = $"{_notificationSettings.DefaultSenderName} <{_notificationSettings.DefaultSenderAddress}>";

			var email = new NotificationQueue(participantInvitation.GrantApplication, participantInvitation, sender, inviteBody, inviteSubject);
			_notificationService.SendNotification(email);

			participantInvitation.ParticipantInvitationStatus = ParticipantInvitationStatus.Sent;

			_dbContext.Update(participantInvitation);
			_dbContext.Commit();

			return participantInvitation;
		}

		public ParticipantInvitation GetInvitation(int grantApplicationId, int invitationId)
		{
			var invitation = Get<ParticipantInvitation>(invitationId);

			if (!_httpContext.User.CanPerformAction(invitation.GrantApplication, ApplicationWorkflowTrigger.ViewParticipants))
				throw new NotAuthorizedException($"User does not have permission to view participants from grant application '{invitation.GrantApplication}'.");

			return invitation;
		}

		public ParticipantInvitation CompleteIndividualInvitation(ParticipantForm participantForm, ParticipantInvitation participantInvitation)
		{
			if (participantForm == null)
				throw new ArgumentNullException(nameof(participantForm));

			if (participantInvitation == null)
				throw new ArgumentNullException(nameof(participantInvitation));

			participantInvitation.ParticipantForm = participantForm;
			participantInvitation.ParticipantInvitationStatus = ParticipantInvitationStatus.Completed;

			_dbContext.Update(participantInvitation);
			_dbContext.Commit();

			return participantInvitation;
		}
	}
}

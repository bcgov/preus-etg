using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Entities.Extensions;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Ext.Models.ParticipantReporting
{
	public class ReportingViewModel : BaseViewModel
	{
		public int GrantApplicationId { get; set; }
		public string ClaimRowVersion { get; set; }
		public string GrantProgramName { get; set; }
		public string GrantProgramCode { get; set; }
		public bool CanApplicantReportParticipants { get; set; }

		public Guid InvitationKey { get; set; }
		public string InvitationEmailText { get; set; }
		public string InvitationBrowserLink { get; set; }

		public bool ParticipantsEditable { get; set; }

		public ApplicationStateInternal ApplicationStateInternal { get; set; }
		public ApplicationStateExternal ApplicationStateExternal { get; set; }

		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public DateTime ParticipantReportingDueDate { get; set; }

		public int MaxParticipantsAllowed { get; set; }
		public bool AllowIncludeAll { get; set; }

		public bool UseInvitations { get; set; }

		public List<KeyValuePair<int, string>> ExpectedOutcomes { get; set; } = new List<KeyValuePair<int, string>>();

		public IEnumerable<ParticipantViewModel> Participants { get; set; } = new List<ParticipantViewModel>();
		public IEnumerable<ParticipantInvitationModel> ParticipantInvitations { get; set; } = new List<ParticipantInvitationModel>();

		public List<ParticipantWarningModel> ParticipantWarnings { get; set; }

		public ProgramTitleLabelViewModel ProgramTitleLabel { get; set; }

		public ProgramTypes ProgramType { get; set; }
		public bool ShowEligibility { get; set; }

		public ReportingViewModel()
		{
		}

		public ReportingViewModel(GrantApplication grantApplication, IParticipantService participantService, HttpContextBase context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			GrantApplicationId = grantApplication?.Id ?? throw new ArgumentNullException(nameof(grantApplication));

			var currentClaim = grantApplication.GetCurrentClaim();

			ClaimRowVersion = currentClaim != null ? Convert.ToBase64String(currentClaim.RowVersion) : null;
			GrantProgramName = grantApplication?.GrantOpening?.GrantStream?.GrantProgram?.Name ?? throw new ArgumentNullException(nameof(grantApplication), "The argument 'grantApplication' must provide the grant program name.");
			GrantProgramCode = grantApplication?.GrantOpening?.GrantStream?.GrantProgram?.ProgramCode ?? throw new ArgumentNullException(nameof(grantApplication), "The argument 'grantApplication' must provide the grant program code.");
			CanApplicantReportParticipants = grantApplication.CanApplicantReportParticipants && grantApplication.CanReportParticipants;
			InvitationKey = grantApplication.InvitationKey;
			ApplicationStateInternal = grantApplication.ApplicationStateInternal;
			ApplicationStateExternal = grantApplication.ApplicationStateExternal;
			StartDate = grantApplication.StartDate.ToLocalTime();
			EndDate = grantApplication.EndDate.ToLocalTime();

			if (grantApplication.TrainingPrograms?.Count > 0)
			{
				var firstTrainingProgram = grantApplication.TrainingPrograms.OrderBy(tp => tp.StartDate).FirstOrDefault();
				ParticipantReportingDueDate = firstTrainingProgram.StartDate.AddDays(-5);
			}

			MaxParticipantsAllowed = grantApplication.GetMaxParticipants();
			ShowEligibility = grantApplication.CanViewParticipantEligibilty();
			UseInvitations = grantApplication.UsePIFInvitations;

			ExpectedOutcomes = new List<KeyValuePair<int, string>> {
				new KeyValuePair<int, string>(0, "Please select expected training outcome"),
				GetExpectedItem(ExpectedParticipantOutcome.IncreasedJobSecurity),
				GetExpectedItem(ExpectedParticipantOutcome.IncreasedPay),
				GetExpectedItem(ExpectedParticipantOutcome.Promotion),
				GetExpectedItem(ExpectedParticipantOutcome.MoveFromPartTimeToFullTime),
				GetExpectedItem(ExpectedParticipantOutcome.MoveFromTransitionalToPermanent),
				GetExpectedItem(ExpectedParticipantOutcome.NoOutcome)
			};

			Participants = grantApplication.ParticipantForms
				.OrderBy(pe => pe.LastName)
				.ThenBy(pe => pe.FirstName)
				.Select(pf => new ParticipantViewModel(pf, ShowEligibility, currentClaim))
				.ToArray();

			var applicationHasBeenReturnedToDraft = grantApplication.HasBeenReturnedToDraft();
            ParticipantInvitations = grantApplication.ParticipantInvitations
                .OrderBy(pi => pi.DateAdded)
                .Select(pi => new ParticipantInvitationModel(pi, applicationHasBeenReturnedToDraft))
                .ToList();

            ParticipantWarnings = GetParticipantWarnings(grantApplication, participantService);
			ParticipantsEditable = context.User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditParticipants);

			AllowIncludeAll = Participants.Any(pf => pf.ClaimReported) && ApplicationStateExternal.In(ApplicationStateExternal.ClaimReturned, ApplicationStateExternal.Approved, ApplicationStateExternal.AmendClaim, ApplicationStateExternal.ClaimApproved, ApplicationStateExternal.ClaimDenied);

			ProgramTitleLabel = new ProgramTitleLabelViewModel(grantApplication, false);

			ProgramType = grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId;

			InvitationBrowserLink = $"{context.Request.Url.GetLeftPart(UriPartial.Authority)}/Part/Information/{HttpUtility.UrlEncode(grantApplication.InvitationKey.ToString())}";
			InvitationEmailText =
				"Dear {{participant}},\r\n\r\n" +
				$"You have been chosen by your employer to participate in {grantApplication.GetProgramDescription()} training starting on {StartDate.ToLocalMorning():yyyy-MM-dd}. This training will be funded by your employer as well as through the B.C. Employer Training Grant program. If approved, your training expenses will be fully covered, and you will not be required to pay anything.\r\n\r\n" +
				$"To make sure you are eligible, please complete your Participant Information Form (PIF) as soon as possible. Click the following link using the Chrome or Firefox browser: { InvitationBrowserLink }\r\n\r\n" +
				"For questions, please contact your employer. We are also available to answer any questions at ETG@gov.bc.ca.\r\n\r\n" +
				"Thank you,\r\n" +
				"The B.C. Employer Training Grant";
		}

		private List<ParticipantWarningModel> GetParticipantWarnings(GrantApplication grantApplication, IParticipantService participantService)
		{
			var warnings = new List<ParticipantWarningModel>();

			var maxReimbursementAmount = grantApplication.MaxReimbursementAmt;
			var grantApplicationFiscal = grantApplication.GrantOpening.TrainingPeriod.FiscalYearId;

			var applicationClaimStatuses = new List<ApplicationStateInternal>
			{
				ApplicationStateInternal.Closed,
				ApplicationStateInternal.CompletionReporting
			};

			foreach (var participant in grantApplication.ParticipantForms)
			{
				var otherParticipantForms = participantService.GetParticipantFormsBySIN(participant.SIN);

				var participantPayments = 0M;

				foreach (var form in otherParticipantForms.Where(opf => opf.GrantApplicationId != GrantApplicationId)
					.Where(opf => opf.GrantApplication.GrantOpening.TrainingPeriod.FiscalYearId == grantApplicationFiscal)
					.Where(opf => applicationClaimStatuses.Contains(opf.GrantApplication.ApplicationStateInternal)))
				{
					var totalPastCosts = form.ParticipantCosts.Sum(c => c.AssessedReimbursement);
					participantPayments += totalPastCosts;
				}

				warnings.Add(new ParticipantWarningModel
				{
					ParticipantName = $"{participant.FirstName} {participant.LastName}",
					CurrentClaims = participantPayments,
					FiscalYearLimit = maxReimbursementAmount
				});
			}

			return warnings;
		}

		private static KeyValuePair<int, string> GetExpectedItem(ExpectedParticipantOutcome item)
		{
			return new KeyValuePair<int, string>( (int)item, item.GetDescription());
		}
	}
}
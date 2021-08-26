using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Web.External.Areas.Ext.Models.ParticipantReporting
{
	public class ReportingViewModel : BaseViewModel
	{
		#region Properties
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

		public IEnumerable<ParticipantViewModel> Participants { get; set; } = new List<ParticipantViewModel>();

		public ProgramTitleLabelViewModel ProgramTitleLabel { get; set; }
		#endregion

		#region Constructors
		public ReportingViewModel()
		{

		}

		public ReportingViewModel(GrantApplication grantApplication, HttpContextBase context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			this.GrantApplicationId = grantApplication?.Id ?? throw new ArgumentNullException(nameof(grantApplication));

			var currentClaim = grantApplication.GetCurrentClaim();

			this.ClaimRowVersion = currentClaim != null ? Convert.ToBase64String(currentClaim.RowVersion) : null;
			this.GrantProgramName = grantApplication?.GrantOpening?.GrantStream?.GrantProgram?.Name ?? throw new ArgumentNullException(nameof(grantApplication), "The argument 'grantApplication' must provide the grant program name.");
			this.GrantProgramCode = grantApplication?.GrantOpening?.GrantStream?.GrantProgram?.ProgramCode ?? throw new ArgumentNullException(nameof(grantApplication), "The argument 'grantApplication' must provide the grant program code.");
			this.CanApplicantReportParticipants = grantApplication.CanApplicantReportParticipants && grantApplication.CanReportParticipants;
			this.InvitationKey = grantApplication.InvitationKey;
			this.ApplicationStateInternal = grantApplication.ApplicationStateInternal;
			this.ApplicationStateExternal = grantApplication.ApplicationStateExternal;
			this.StartDate = grantApplication.StartDate.ToLocalTime();
			this.EndDate = grantApplication.EndDate.ToLocalTime();
			this.ParticipantReportingDueDate = grantApplication.GrantAgreement.ParticipantReportingDueDate.ToLocalTime();
			this.MaxParticipantsAllowed = grantApplication.GetMaxParticipants();
			this.Participants = grantApplication.ParticipantForms
												.OrderBy(pe => pe.LastName)
												.ThenBy(pe => pe.FirstName)
												.Select(pf => new ParticipantViewModel(pf, currentClaim))
												.ToArray();
			this.ParticipantsEditable = context.User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditParticipants);

			this.AllowIncludeAll = this.Participants.Any(pf => pf.ClaimReported) && this.ApplicationStateExternal.In(ApplicationStateExternal.ClaimReturned, ApplicationStateExternal.Approved, ApplicationStateExternal.AmendClaim, ApplicationStateExternal.ClaimApproved, ApplicationStateExternal.ClaimDenied);

			this.ProgramTitleLabel = new ProgramTitleLabelViewModel(grantApplication, false);

			var programType = grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId;
			var defaultTrainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();
			var programTitle = programType == ProgramTypes.EmployerGrant ? defaultTrainingProgram?.CourseTitle : grantApplication.ProgramDescription.Description;
			var location = programType == ProgramTypes.EmployerGrant ? $"{defaultTrainingProgram?.TrainingProvider.TrainingAddress.City}, {defaultTrainingProgram?.TrainingProvider.TrainingAddress.Region.Name}, {defaultTrainingProgram?.TrainingProvider.TrainingAddress.Country.Name}" : "";
			this.InvitationBrowserLink = $"{context.Request.Url.GetLeftPart(UriPartial.Authority)}/Part/Information/{HttpUtility.UrlEncode(grantApplication.InvitationKey.ToString())}";

			var invitation = $"As this training is being funded through the {this.GrantProgramName}, you must complete a participant information form using the following link:\r\n\r\n{this.InvitationBrowserLink}\r\n\r\n";
			this.InvitationEmailText =
				"Dear {{participant}},\r\n\r\n" +
				"You have been identified as a participant for the following training program:\r\n\r\n" +
				$"{programTitle}\r\n" +
				$"Start Date: {this.StartDate.ToLocalMorning():yyyy-MM-dd}\r\n" +
				$"Location: {location}\r\n\r\n" +
				$"{invitation}" +
				"Please use a current version of Chrome or Firefox to enter participant information.\r\n\r\n" +
				$"Please complete your participant information form prior to midnight on {this.ParticipantReportingDueDate:yyyy-MM-dd}. " +
				"If you do not complete this form, you may not be able to participate in the training.";
		}
		#endregion
	}
}
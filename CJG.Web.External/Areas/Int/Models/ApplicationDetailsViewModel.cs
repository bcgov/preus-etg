using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace CJG.Web.External.Areas.Int.Models
{
	public class ApplicationDetailsViewModel : BaseViewModel
	{
		#region Properties

		public string RowVersion { get; set; }

		public int GrantStreamId { get; set; }
		public int GrantProgramId { get; set; }
		public ProgramTypes ProgramType { get; set; }

		public DateTime TrainingPeriodStart { get; set; }
		public DateTime TrainingPeriodEnd { get; set; }
		public DateTime DeliveryStartDate { get; set; }

		public WorkflowViewModel WorkflowViewModel { get; set; }

		public bool ShowAttachments { get; }
		public bool ShowAssessors { get; }
		public bool ShowCompletionReport { get; }
		public bool ShowTrainingProviderChangeRequested { get; }
		public bool ShowTrainingProviders { get; }
		public bool ShowProgramDescription { get; }
		public bool ShowTrainingPrograms { get; }
		public bool ShowSkillsTraining { get; }
		public bool RequiresTrainingProviderValidation { get; set; }
		public bool ShowESS { get; }
		public bool ShowParticipants { get; }
		public bool ShowClaims { get; }

		public bool IsAssessor { get; set; }
		public bool UnderAssessment { get; set; }
		public string AttachmentHeader { get; set; }
		public bool ShowApplicantReportingOfParticipantsButton { get; set; }
		public bool ShowParticipantReportingButton { get; set; }
		public bool ShowHoldPaymentRequestButton { get; set; }
		public bool HoldPaymentRequests { get; set; }
		public bool CanViewParticipants { get; set; }
		public bool CanReportParticipants { get; set; }
		public bool CanApplicantReportParticipants { get; set; }
		public int MaxParticipants { get; set; }
		public int TotalParticipants { get; set; }

		public bool EditSummary { get; set; }
		public bool EditApplicantContact { get; set; }
		public bool EditApplicant { get; set; }
		public bool EditAttachments { get; set; }
		public bool EditProgramDescription { get; set; }
		public bool EditTrainingProgram { get; set; }
		public bool AddRemoveTrainingPrograms { get; set; }
		public bool EditTrainingProvider { get; set; }
		public bool EditProviderServices { get; set; }
		public bool AddRemoveTrainingProviders { get; set; }
		public bool EditProgramCost { get; set; }
		public IEnumerable<Applications.ApplicationComponentViewModel> Components { get; set; }
		public ClaimTypes ClaimType { get; set; }
		public bool HasAgreement { get; set; }
		public bool ScheduledNotificationsEnabled { get; set; }
		#endregion

		#region Constructors
		public ApplicationDetailsViewModel() { }
		public ApplicationDetailsViewModel(GrantApplication grantApplication, IPrincipal user, Func<string, string> GetWorkflowUrl)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			if (grantApplication.GrantOpening?.GrantStream == null) throw new ArgumentNullException(nameof(GrantStream));
			if (grantApplication.GrantOpening?.GrantStream?.GrantProgram == null) throw new ArgumentNullException(nameof(GrantProgram));

			this.Id = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.GrantStreamId = grantApplication.GrantOpening.GrantStreamId;
			this.GrantProgramId = grantApplication.GrantOpening.GrantStream.GrantProgramId;
			this.ProgramType = grantApplication.GetProgramType();

			this.TrainingPeriodStart = grantApplication.GrantOpening.TrainingPeriod.StartDate.ToLocalTime();
			this.TrainingPeriodEnd = grantApplication.GrantOpening.TrainingPeriod.EndDate.ToLocalTime();
			this.DeliveryStartDate = grantApplication.StartDate.ToLocalTime();

			this.WorkflowViewModel = new WorkflowViewModel(grantApplication, user, GetWorkflowUrl);
			this.RequiresTrainingProviderValidation = grantApplication.RequiresTrainingProviderValidation();
			this.ShowAttachments = grantApplication.GrantOpening?.GrantStream?.AttachmentsIsEnabled == true || grantApplication.Attachments.Count > 0;
			this.ShowAssessors = grantApplication.ApplicationStateInternal == ApplicationStateInternal.PendingAssessment;
			this.ShowCompletionReport = grantApplication.HasBeenApproved() && (grantApplication.EndDate < AppDateTime.UtcNow || grantApplication.ApplicationStateInternal.In(ApplicationStateInternal.Closed, ApplicationStateInternal.CompletionReporting));
			this.ShowTrainingProviderChangeRequested = grantApplication.ApplicationStateExternal == ApplicationStateExternal.ChangeRequestSubmitted;
			this.ShowProgramDescription = this.ProgramType == ProgramTypes.WDAService;
			this.ShowTrainingProviders = this.ProgramType == ProgramTypes.EmployerGrant;
			this.ShowTrainingPrograms = this.ProgramType == ProgramTypes.EmployerGrant;
			this.AttachmentHeader = grantApplication.GrantOpening.GrantStream.AttachmentsHeader;

			// This isn't technically correct
			this.ShowSkillsTraining = this.ProgramType == ProgramTypes.WDAService;

			// This isn't technically correct
			this.ShowESS = grantApplication.GetProgramType() == ProgramTypes.WDAService;

			this.ShowParticipants = grantApplication.ParticipantForms.Count > 0
				|| user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EnableApplicantReportingOfParticipants)
				|| user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EnableParticipantReporting);
			this.ShowClaims = grantApplication.Claims.Count() > 0 && grantApplication.HasSubmittedAClaim();

			this.IsAssessor = user.GetUserId() == grantApplication.AssessorId;
			this.UnderAssessment = grantApplication?.ApplicationStateInternal.In(ApplicationStateInternal.New,
																			ApplicationStateInternal.PendingAssessment,
																			ApplicationStateInternal.UnderAssessment,
																			ApplicationStateInternal.ReturnedToAssessment,
																			ApplicationStateInternal.RecommendedForApproval,
																			ApplicationStateInternal.RecommendedForDenial) == true;

			this.ShowApplicantReportingOfParticipantsButton = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EnableApplicantReportingOfParticipants);
			this.ShowParticipantReportingButton = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EnableParticipantReporting);
			this.ShowHoldPaymentRequestButton = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.HoldPaymentRequests);
			this.HoldPaymentRequests = grantApplication.HoldPaymentRequests;
			this.CanViewParticipants = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.ViewParticipants);
			this.CanReportParticipants = grantApplication.CanReportParticipants;
			this.CanApplicantReportParticipants = grantApplication.CanApplicantReportParticipants;
			this.TotalParticipants = grantApplication.ParticipantForms.Count;
			this.MaxParticipants = grantApplication.TrainingCost.AgreedParticipants;

			if (this.ProgramType == ProgramTypes.EmployerGrant)
			{
				var program = grantApplication.TrainingPrograms.FirstOrDefault();
				this.Components = new[]
				{
					new Applications.ApplicationComponentViewModel(program.TrainingProvider.ApprovedTrainingProvider ?? program.TrainingProvider, user, 1),
					new Applications.ApplicationComponentViewModel(program, user, 2)
				};
			}
			else
			{
				this.Components = grantApplication.TrainingCost.EligibleCosts.Where(ec => ec.EligibleExpenseType.ServiceCategory?.ServiceTypeId != ServiceTypes.Administration).Select(ec => new Applications.ApplicationComponentViewModel(ec, user)).OrderBy(c => c.RowSequence).ToArray();
			}

			this.EditSummary = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditSummary);
			this.EditApplicantContact = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditApplicantContact);
			this.EditApplicant = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditApplicant);
			this.EditAttachments = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditApplicationAttachments);
			this.EditProgramDescription = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditProgramDescription);
			this.EditTrainingProvider = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditTrainingProvider);
			this.EditTrainingProgram = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditTrainingProgram);
			this.AddRemoveTrainingPrograms = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.AddOrRemoveTrainingProgram);
			this.EditProgramCost = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditTrainingCosts);
			this.EditProviderServices = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditTrainingCosts);
			this.AddRemoveTrainingProviders = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.AddRemoveTrainingProvider);
			this.ClaimType = grantApplication.GrantOpening.GrantStream.ProgramConfiguration.ClaimTypeId;
			this.HasAgreement = grantApplication.GrantAgreement != null;
			this.ScheduledNotificationsEnabled = grantApplication.ScheduledNotificationsEnabled;
		}
		#endregion
	}
}

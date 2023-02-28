using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Int.Models.Attachments;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.Applications
{
    public class ApplicationSummaryViewModel : BaseViewModel
	{
		public string RowVersion { get; set; }
		public int TotalGrantApplications { get; set; }
		public decimal TotalGrantApplicationCost { get; set; }
		public string TerminalReason { get; set; }
		public string HighLevelDenialReasons { get; set; }
		public string FileNumber { get; set; }
		public DateTime? DateUpdated { get; set; }
		public DateTime? DateSubmitted { get; set; }
		public ApplicationStateViewModel ApplicationStateExternalViewModel { get; set; }
		public ApplicationStateViewModel ApplicationStateInternalViewModel { get; set; }
		public string GrantStreamFullName { get; set; }
		public int GrantProgramId { get; set; }
		public DateTime TrainingPeriodStartDate { get; set; }
		public DateTime TrainingPeriodEndDate { get; set; }
		public int OrgId { get; set; }
		public string OrganizationLegalName { get; set; }
		public string DoingBusinessAs { get; set; }
		public string DoingBusinessAsMinistry { get; set; }
		public string StatementOfRegistrationNumber { get; set; }
		public decimal EligibleTotalCost { get; set; }
		public int? AssessorId { get; set; }
		public InternalUser Assessor { get; set; }
		public int? DeliveryPartnerId { get; set; }
		public int? DeliveryPartnerServicesId { get; set; }
		public int? RiskClassificationId { get; set; }
		public bool ShowAssessorName { get; set; }
		[Required(ErrorMessage = "Delivery Start Date is required.")]
		public DateTime DeliveryStartDate { get; set; }
		[Required(ErrorMessage = "Delivery End Date is required.")]
		public DateTime DeliveryEndDate { get; set; }
		public IEnumerable<int> SelectedDeliveryPartnerServiceIds { get; set; } = new List<int>();
		public string AssignedBy { get; set; }
		public bool AllowEditDeliveryPartner { get; set; }
		public bool CanModifyDeliveryDates { get; set; }
		public bool AllowDirectorUpdate { get; set; }
		public bool AllowReAssign { get; set; }
		public bool EditSummary { get; set; }
		public bool? HasRequestedAdditionalFunding { get; set; }

		public string DescriptionOfFundingRequested { get; set; }
		public string BusinessCaseHeader { get; set; } = "Business Case";
		public AttachmentViewModel BusinessCaseDocument { get; set; }
		public ProgramTypes ProgramType { get; private set; }

		public int? PrioritizationScore { get; set; }

		public ApplicationSummaryViewModel() { }

		public ApplicationSummaryViewModel(GrantApplication grantApplication,
											IDeliveryPartnerService deliveryPartnerService,
											IAuthorizationService authorizationService,
											IGrantApplicationService grantApplicationService,
											IRiskClassificationService riskClassificationService,
											IPrincipal user)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));
			if (deliveryPartnerService == null)
				throw new ArgumentNullException(nameof(deliveryPartnerService));
			if (authorizationService == null)
				throw new ArgumentNullException(nameof(authorizationService));
			if (grantApplicationService == null)
				throw new ArgumentNullException(nameof(grantApplicationService));
			if (riskClassificationService == null)
				throw new ArgumentNullException(nameof(riskClassificationService));
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			Id = grantApplication.Id;
			RowVersion = Convert.ToBase64String(grantApplication.RowVersion);

			Assessor = grantApplication.Assessor == null ? null : new InternalUser
			{
				Id = grantApplication.Assessor.Id,
				LastName = grantApplication.Assessor.LastName,
				FirstName = grantApplication.Assessor.FirstName,
				IDIR = grantApplication.Assessor.IDIR,
				Email = grantApplication.Assessor.Email
			};

			ShowAssessorName = grantApplication.Assessor != null && grantApplication.ApplicationStateInternal >= ApplicationStateInternal.UnderAssessment;
			TerminalReason = grantApplication.GetTerminalReason();
			HighLevelDenialReasons = grantApplication.GetSelectedDeniedReason();
			TotalGrantApplications = grantApplicationService.GetApplicationsCountByFiscal(grantApplication);
			TotalGrantApplicationCost = grantApplicationService.GetApplicationsCostByFiscal(grantApplication);
			EligibleTotalCost = grantApplication.TrainingCost != null ? (grantApplication.ApplicationStateInternal.ShowAgreedCosts() ? grantApplication.TrainingCost.TotalAgreedMaxCost : grantApplication.TrainingCost.TotalEstimatedCost) : 0;
			ApplicationStateExternalViewModel = new ApplicationStateViewModel
			{
				Id = (int)grantApplication.ApplicationStateExternal,
				Name = grantApplication.ApplicationStateExternal.ToString(),
				Description = grantApplication.ApplicationStateExternal.GetDescription()
			};
			ApplicationStateInternalViewModel = new ApplicationStateViewModel
			{
				Id = (int)grantApplication.ApplicationStateInternal,
				Name = grantApplication.ApplicationStateInternal.ToString(),
				Description = grantApplication.ApplicationStateInternal.GetDescription()
			};
			DateSubmitted = grantApplication.DateSubmitted?.ToLocalTime();
			DateUpdated = grantApplication.DateUpdated?.ToLocalTime();
			FileNumber = grantApplication.FileNumber;
			GrantStreamFullName = grantApplication.GrantOpening.GrantStream.FullName;
			GrantProgramId = grantApplication.GrantOpening.GrantStream.GrantProgramId;
			OrgId = grantApplication.Organization.Id;
			OrganizationLegalName = grantApplication.OrganizationLegalName;
			DoingBusinessAs = grantApplication.OrganizationDoingBusinessAs;
			DoingBusinessAsMinistry = grantApplication.Organization.DoingBusinessAsMinistry;
			StatementOfRegistrationNumber = grantApplication.Organization.StatementOfRegistrationNumber;
			TrainingPeriodStartDate = grantApplication.GrantOpening.TrainingPeriod.StartDate.ToLocalTime();
			TrainingPeriodEndDate = grantApplication.GrantOpening.TrainingPeriod.EndDate.ToLocalTime();
			DeliveryStartDate = grantApplication.StartDate.ToLocalTime();
			DeliveryEndDate = grantApplication.EndDate.ToLocalTime();
			DeliveryPartnerId = grantApplication.DeliveryPartner?.Id;
			RiskClassificationId = grantApplication.RiskClassification?.Id;
			AssignedBy = grantApplication.GetStateChange(ApplicationStateInternal.UnderAssessment)?.Assessor.FirstName + " " + grantApplication.GetStateChange(ApplicationStateInternal.UnderAssessment)?.Assessor.LastName;

			SelectedDeliveryPartnerServiceIds = grantApplication.DeliveryPartnerServices.Select(d => d.Id).ToArray();
			AllowEditDeliveryPartner = grantApplication.GrantOpening.GrantStream.IncludeDeliveryPartner;

			EditSummary = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditApplication);
			CanModifyDeliveryDates = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditApplication);
			AllowReAssign = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.ReassignAssessor);
			AllowDirectorUpdate = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.DenyApplication);
			HasRequestedAdditionalFunding = grantApplication.HasRequestedAdditionalFunding;
			DescriptionOfFundingRequested = grantApplication.DescriptionOfFundingRequested;
			if (grantApplication.BusinessCaseDocument != null)
				BusinessCaseDocument = new AttachmentViewModel(grantApplication.BusinessCaseDocument);
			ProgramType = grantApplication.GetProgramType();
			BusinessCaseHeader = grantApplication.GrantOpening.GrantStream.BusinessCaseInternalHeader;

			PrioritizationScore = grantApplication.PrioritizationScoreBreakdown != null ? grantApplication.PrioritizationScore : (int?)null;
		}

		public GrantApplication MapToGrantApplication(ApplicationSummaryViewModel model, GrantApplication grantApplication)
		{
			grantApplication.RowVersion = Convert.FromBase64String(model.RowVersion);
			grantApplication.StartDate = model.DeliveryStartDate.ToUniversalTime();
			grantApplication.EndDate = model.DeliveryEndDate.ToUniversalTime();
			grantApplication.RiskClassificationId = model.RiskClassificationId;
			grantApplication.DeliveryPartnerId = model.DeliveryPartnerId;

			return grantApplication;
		}
	}
}

using CJG.Application.Business.Models;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using CJG.Core.Interfaces.Service;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using CJG.Application.Services;

namespace CJG.Web.External.Areas.Int.Models.Applications
{
	public class ApplicationSummaryViewModel : BaseViewModel
	{
		#region Properties
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
		public Attachments.AttachmentViewModel BusinessCaseDocument { get; set; }
		public ProgramTypes ProgramType { get; private set; }

		#endregion

		#region Contructors
		public ApplicationSummaryViewModel() { }

		public ApplicationSummaryViewModel(GrantApplication grantApplication,
											IDeliveryPartnerService deliveryPartnerService,
											IAuthorizationService authorizationService,
											IGrantApplicationService grantApplicationService,
											IRiskClassificationService riskClassificationService,
											IPrincipal user)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			if (deliveryPartnerService == null) throw new ArgumentNullException(nameof(deliveryPartnerService));
			if (authorizationService == null) throw new ArgumentNullException(nameof(authorizationService));
			if (grantApplicationService == null) throw new ArgumentNullException(nameof(grantApplicationService));
			if (riskClassificationService == null) throw new ArgumentNullException(nameof(riskClassificationService));
			if (user == null) throw new ArgumentNullException(nameof(user));

			this.Id = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String((byte[])grantApplication.RowVersion);

			this.Assessor = grantApplication.Assessor == null ? null : new InternalUser
			{
				Id = grantApplication.Assessor.Id,
				LastName = grantApplication.Assessor.LastName,
				FirstName = grantApplication.Assessor.FirstName,
				IDIR = grantApplication.Assessor.IDIR,
				Email = grantApplication.Assessor.Email
			};

			this.ShowAssessorName = grantApplication.Assessor != null && grantApplication.ApplicationStateInternal >= ApplicationStateInternal.UnderAssessment;
			this.TerminalReason = grantApplication.GetTerminalReason();
			this.HighLevelDenialReasons = grantApplication.GetSelectedDeniedReason();
			this.TotalGrantApplications = grantApplicationService.GetApplicationsCountByFiscal(grantApplication);
			this.TotalGrantApplicationCost = grantApplicationService.GetApplicationsCostByFiscal(grantApplication);
			this.EligibleTotalCost = grantApplication.TrainingCost != null ? (grantApplication.ApplicationStateInternal.ShowAgreedCosts() ? grantApplication.TrainingCost.TotalAgreedMaxCost : grantApplication.TrainingCost.TotalEstimatedCost) : 0;
			this.ApplicationStateExternalViewModel = new ApplicationStateViewModel
			{
				Id = (int)grantApplication.ApplicationStateExternal,
				Name = grantApplication.ApplicationStateExternal.ToString(),
				Description = grantApplication.ApplicationStateExternal.GetDescription()
			};
			this.ApplicationStateInternalViewModel = new ApplicationStateViewModel
			{
				Id = (int)grantApplication.ApplicationStateInternal,
				Name = grantApplication.ApplicationStateInternal.ToString(),
				Description = grantApplication.ApplicationStateInternal.GetDescription()
			};
			this.DateSubmitted = grantApplication.DateSubmitted?.ToLocalTime();
			this.DateUpdated = grantApplication.DateUpdated?.ToLocalTime();
			this.FileNumber = grantApplication.FileNumber;
			this.GrantStreamFullName = grantApplication.GrantOpening.GrantStream.FullName;
			this.GrantProgramId = grantApplication.GrantOpening.GrantStream.GrantProgramId;
			this.OrgId = grantApplication.Organization.Id;
			this.OrganizationLegalName = grantApplication.OrganizationLegalName;
			this.DoingBusinessAs = grantApplication.OrganizationDoingBusinessAs;
			this.DoingBusinessAsMinistry = grantApplication.Organization.DoingBusinessAsMinistry;
			this.StatementOfRegistrationNumber = grantApplication.Organization.StatementOfRegistrationNumber;
			this.TrainingPeriodStartDate = grantApplication.GrantOpening.TrainingPeriod.StartDate.ToLocalTime();
			this.TrainingPeriodEndDate = grantApplication.GrantOpening.TrainingPeriod.EndDate.ToLocalTime();
			this.DeliveryStartDate = grantApplication.StartDate.ToLocalTime();
			this.DeliveryEndDate = grantApplication.EndDate.ToLocalTime();
			this.DeliveryPartnerId = grantApplication.DeliveryPartner?.Id;
			this.RiskClassificationId = grantApplication.RiskClassification?.Id;
			this.AssignedBy = grantApplication.GetStateChange(ApplicationStateInternal.UnderAssessment)?.Assessor.FirstName + " " + grantApplication.GetStateChange(ApplicationStateInternal.UnderAssessment)?.Assessor.LastName;

			this.SelectedDeliveryPartnerServiceIds = grantApplication.DeliveryPartnerServices.Select(d => d.Id).ToArray();
			this.AllowEditDeliveryPartner = grantApplication.GrantOpening.GrantStream.IncludeDeliveryPartner;

			this.EditSummary = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditApplication);
			this.CanModifyDeliveryDates = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditApplication);
			this.AllowReAssign = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.ReassignAssessor);
			this.AllowDirectorUpdate = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.DenyApplication);
			this.HasRequestedAdditionalFunding = grantApplication.HasRequestedAdditionalFunding;
			this.DescriptionOfFundingRequested = grantApplication.DescriptionOfFundingRequested;
			if (grantApplication.BusinessCaseDocument != null)
				this.BusinessCaseDocument = new Attachments.AttachmentViewModel(grantApplication.BusinessCaseDocument);
			this.ProgramType = grantApplication.GetProgramType();
			this.BusinessCaseHeader = grantApplication.GrantOpening.GrantStream.BusinessCaseInternalHeader;
		}
		#endregion

		#region Methods
		public GrantApplication MapToGrantApplication(ApplicationSummaryViewModel model, GrantApplication grantApplication)
		{
			grantApplication.RowVersion = Convert.FromBase64String(model.RowVersion);
			grantApplication.StartDate = model.DeliveryStartDate.ToUniversalTime();
			grantApplication.EndDate = model.DeliveryEndDate.ToUniversalTime();
			grantApplication.RiskClassificationId = model.RiskClassificationId;
			grantApplication.DeliveryPartnerId = model.DeliveryPartnerId;

			return grantApplication;
		}
		#endregion
	}
}

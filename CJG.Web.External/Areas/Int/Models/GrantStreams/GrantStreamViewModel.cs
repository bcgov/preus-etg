using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;
using DataAnnotationsExtensions;

namespace CJG.Web.External.Areas.Int.Models.GrantStreams
{
	public class GrantStreamViewModel : BaseViewModel
	{
		#region Properties
		public int GrantProgramId { get; set; }

		public GrantProgramStates GrantProgramState { get; set; }

		public ProgramTypes ProgramType { get; set; }

		public string RowVersion { get; set; }

		[Required(ErrorMessage = "Grant stream name is required."), Display(Name = "Grant stream Name")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Grant stream objective is required."), Display(Name = "Objective")]
		public string Objective { get; set; }

		public bool IsActive { get; set; }

		public bool IncludeDeliveryPartner { get; set; }

		public DateTime? DateFirstUsed { get; set; }

		public int ProgramConfigurationId { get; set; }
		public StreamProgramConfigurationViewModel ProgramConfiguration { get; set; }

		#region Rates
		[Required(ErrorMessage = "Fiscal year participant limit is required.")]
		[Min(1, ErrorMessage = "Fiscal year participant limit must be greater than $0.")]
		[Display(Name = "FY Participant Limit")]
		public decimal MaxReimbursementAmt { get; set; }

		[Required(ErrorMessage = "Reimbursement rate is required.")]
		[Display(Name = "Reimbursement Rate")]
		public double ReimbursementRate { get; set; }

		[Display(Name = "Denied")]
		public double DefaultDeniedRate { get; set; }

		[Display(Name = "Withdrawn")]
		public double DefaultWithdrawnRate { get; set; }

		[Display(Name = "Reductions")]
		public double DefaultReductionRate { get; set; }

		[Display(Name = "Slippage")]
		public double DefaultSlippageRate { get; set; }

		[Display(Name = "Cancellations")]
		public double DefaultCancellationRate { get; set; }
		#endregion

		public bool HasApplications { get; set; }
		public bool SelfProgramConfiguration { get; set; }
		public bool SelfAccountCode { get; set; }
		public int? AccountCodeId { get; set; }
		public AccountCodeViewModel AccountCode { get; set; }

		#region Application Attachments
		public bool AttachmentsIsEnabled { get; set; }
		public bool AttachmentsRequired { get; set; }
		public string AttachmentsHeader { get; set; }
		public string AttachmentsUserGuidance { get; set; }
		public int AttachmentsMaximum { get; set; }
		#endregion

		#region Application Business Case
		public bool BusinessCaseIsEnabled { get; set; }
		public bool BusinessCaseRequired { get; set; }
		public string BusinessCaseInternalHeader { get; set; }
		public string BusinessCaseExternalHeader { get; set; }
		public string BusinessCaseUserGuidance { get; set; }
		public string BusinessCaseTemplateURL { get; set; }
		#endregion

		#region Eligibility
		public bool EligibilityEnabled { get; set; }
		public bool EligibilityRequired { get; set; }
		[Display(Name = "Requirements")]
		public string EligibilityRequirements { get; set; }
		public string EligibilityQuestion { get; set; }
		#endregion

		#region Reporting
		public bool CanApplicantReportParticipants { get; set; }
		public bool HasParticipantOutcomeReporting { get; set; }
		public bool RequireAllParticipantsBeforeSubmission { get; set; }
		#endregion

		public bool CanDelete { get; set; }

		public List<GrantStreamQuestionViewModel> StreamQuestions { get; set; }

		#endregion

		#region Constructors
		public GrantStreamViewModel() { }

		public GrantStreamViewModel(GrantStream grantStream, IServiceCategoryService serviceCategoryService, IStaticDataService staticDataService, IGrantStreamService grantStreamService)
		{
			if (grantStream == null) throw new ArgumentNullException(nameof(grantStream));
			if (serviceCategoryService == null) throw new ArgumentNullException(nameof(serviceCategoryService));
			if (staticDataService == null) throw new ArgumentNullException(nameof(staticDataService));

			Utilities.MapProperties(grantStream, this);

			RowVersion = Convert.ToBase64String(grantStream.RowVersion);
			ProgramType = grantStream.GrantProgram.ProgramTypeId;
			GrantProgramState = grantStream.GrantProgram.State;
			ProgramConfigurationId = grantStream.ProgramConfigurationId;
			AccountCodeId = grantStream.AccountCodeId;

			EligibilityRequirements = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(this.EligibilityRequirements));
			SelfAccountCode = grantStream.AccountCodeId != grantStream.GrantProgram.AccountCodeId;
			SelfProgramConfiguration = grantStream.ProgramConfigurationId != grantStream.GrantProgram.ProgramConfigurationId;
			HasApplications = grantStreamService.HasApplications(grantStream.Id);

			CanDelete = grantStream.DateFirstUsed == null && !grantStream.IsActive && grantStream.GrantOpenings.Count == 0;

			StreamQuestions = grantStreamService.GetGrantStreamQuestions(grantStream.Id)
					.Select(n => new GrantStreamQuestionViewModel(n)).ToList();
		}
		#endregion
	}
}

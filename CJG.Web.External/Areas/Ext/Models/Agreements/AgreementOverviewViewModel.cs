using System;
using System.Security.Principal;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Ext.Models.Agreements
{
	public class AgreementOverviewViewModel : BaseViewModel
	{
		public string RowVersion { get; set; }
		public CovertLetterViewModel CoverLetter { get; set; }
		public ScheduleAViewModel ScheduleA { get; set; }
		public ScheduleBViewModel ScheduleB { get; set; }
		public bool AllowProviderChangeRequest { get; set; } = true;
		public bool AllowCancelAgreement { get; set; } = true;
		public bool AllowDeliveryDateChange { get; set; } = true;
		public bool AllowSubmitChangeRequest { get; set; } = true;
		public ApplicationStateInternal ApplicationStateInternal { get; set; }
		public ApplicationStateExternal ApplicationStateExternal { get; set; }
		public CancelAgreementViewModal CancelModal { get; set; }
		public ProgramTitleLabelViewModel GrantAgreementApplicationViewModel { get; set; }

		public AgreementOverviewViewModel()
		{

		}
		public AgreementOverviewViewModel(GrantApplication grantApplication, IPrincipal user)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			if (user == null)
				throw new ArgumentNullException(nameof(user));

			Id = grantApplication.Id;
			RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			CoverLetter = new CovertLetterViewModel(grantApplication);
			ScheduleA = new ScheduleAViewModel(grantApplication);
			ScheduleB = new ScheduleBViewModel(grantApplication);
			ApplicationStateInternal = grantApplication.ApplicationStateInternal;
			ApplicationStateExternal = grantApplication.ApplicationStateExternal;
			CancelModal = new CancelAgreementViewModal(grantApplication);

			AllowDeliveryDateChange =  grantApplication.CanChangeDeliveryDates();
			AllowProviderChangeRequest = grantApplication.CanMakeChangeRequest();
			AllowSubmitChangeRequest = grantApplication.HasChangeRequest() && user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.SubmitChangeRequest);
			AllowCancelAgreement = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.CancelAgreementHolder);

			GrantAgreementApplicationViewModel = new ProgramTitleLabelViewModel(grantApplication);
		}
	}
}
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.Security.Principal;

namespace CJG.Web.External.Areas.Ext.Models.Agreements
{
	public class AgreementOverviewViewModel : BaseViewModel
	{
		#region Properties
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
		#endregion

		#region Constructors
		public AgreementOverviewViewModel()
		{

		}
		public AgreementOverviewViewModel(GrantApplication grantApplication, IPrincipal user)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			if (user == null) throw new ArgumentNullException(nameof(user));

			this.Id = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.CoverLetter = new CovertLetterViewModel(grantApplication);
			this.ScheduleA = new ScheduleAViewModel(grantApplication);
			this.ScheduleB = new ScheduleBViewModel(grantApplication);
			this.ApplicationStateInternal = grantApplication.ApplicationStateInternal;
			this.ApplicationStateExternal = grantApplication.ApplicationStateExternal;
			this.CancelModal = new CancelAgreementViewModal(grantApplication);

			this.AllowDeliveryDateChange =  grantApplication.CanChangeDeliveryDates();
			this.AllowProviderChangeRequest = grantApplication.CanMakeChangeRequest();
			this.AllowSubmitChangeRequest = grantApplication.HasChangeRequest() && user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.SubmitChangeRequest);
			this.AllowCancelAgreement = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.CancelAgreementHolder);

			this.GrantAgreementApplicationViewModel = new ProgramTitleLabelViewModel(grantApplication);
		}
		#endregion
	}
}
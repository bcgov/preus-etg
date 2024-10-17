using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;

namespace CJG.Web.External.Areas.Int.Models.Agreements
{
	public class GrantAgreementViewModel : BaseViewModel
	{
		#region Properties
		public int GrantApplicationId { get; set; }
		public string RowVersion { get; set; }
		public DateTime DeliveryStartDate { get; set; }
		public bool CoverLetterConfirmed { get; set; }
		public bool ScheduleAConfirmed { get; set; }
		public bool ScheduleBConfirmed { get; set; }
		public int Versions { get; set; }

		public ApplicationWorkflowViewModel ApplicationWorkflowViewModel { get; set; }
		#endregion

		#region Constructors
		public GrantAgreementViewModel() { }
		public GrantAgreementViewModel(GrantApplication grantApplication)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));

			this.Id = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.DeliveryStartDate = grantApplication.StartDate.ToLocalTime();
			this.CoverLetterConfirmed = grantApplication.GrantAgreement.CoverLetterConfirmed;
			this.ScheduleAConfirmed = grantApplication.GrantAgreement.ScheduleAConfirmed;
			this.ScheduleBConfirmed = grantApplication.GrantAgreement.ScheduleBConfirmed;
			this.ApplicationWorkflowViewModel = new ApplicationWorkflowViewModel(grantApplication);
			this.Versions = grantApplication.GrantAgreement.ScheduleA.VersionNumber;
		}
		#endregion
	}
}

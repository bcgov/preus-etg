using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Ext.Models.Agreements
{

	public class ScheduleBViewModel : GrantAgreementDocumentViewModel
	{
		#region Constructors
		public ScheduleBViewModel()
		{

		}
		public ScheduleBViewModel(GrantApplication grantApplication) : base(grantApplication, ga => ga.ScheduleB)
		{
			this.Confirmation = grantApplication.GrantAgreement.ScheduleBConfirmed;
		}
		#endregion
	}
}
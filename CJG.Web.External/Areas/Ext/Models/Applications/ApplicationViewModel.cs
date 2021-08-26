using CJG.Core.Entities;
using CJG.Web.External.Areas.Ext.Models.Overview;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class ApplicationViewModel : BaseApplicationViewModel
	{
		#region Properties
		public OverviewApplicantContact ApplicantContact { get; set; }
		public OverviewApplicantViewModel Applicant { get; set; }

		public ProgramTitleLabelViewModel ProgramTitleLabel { get; set; }
		#endregion

		#region Contructors
		public ApplicationViewModel()
		{

		}
		public ApplicationViewModel(GrantApplication grantApplication, User user) :base(grantApplication)
		{
			this.ApplicantContact = new OverviewApplicantContact(grantApplication, user);
			this.Applicant = new OverviewApplicantViewModel(grantApplication);
			this.ProgramTitleLabel = new ProgramTitleLabelViewModel(grantApplication);
		}
		#endregion
	}
}
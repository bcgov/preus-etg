using CJG.Core.Entities;
using CJG.Web.External.Areas.Ext.Models.Overview;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class ApplicationViewModel : BaseApplicationViewModel
	{
		public OverviewApplicantContact ApplicantContact { get; set; }
		public OverviewApplicantViewModel Applicant { get; set; }
		public int SelectedNewUser { get; set; }

		public ProgramTitleLabelViewModel ProgramTitleLabel { get; set; }

		public ApplicationViewModel()
		{

		}
		public ApplicationViewModel(GrantApplication grantApplication, User user) :base(grantApplication)
		{
			ApplicantContact = new OverviewApplicantContact(grantApplication, user);
			Applicant = new OverviewApplicantViewModel(grantApplication);
			ProgramTitleLabel = new ProgramTitleLabelViewModel(grantApplication);
		}
	}
}
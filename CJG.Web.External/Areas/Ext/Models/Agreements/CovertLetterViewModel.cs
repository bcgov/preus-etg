using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Ext.Models.Agreements
{
	public class CovertLetterViewModel : GrantAgreementDocumentViewModel
	{
		public CovertLetterViewModel()
		{

		}
		public CovertLetterViewModel(GrantApplication grantApplication) : base(grantApplication, ga => ga.CoverLetter)
		{
			this.Confirmation = grantApplication.GrantAgreement.CoverLetterConfirmed;
		}
	}
}
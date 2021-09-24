using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models.Agreements
{
	public class GrantAgreementCoverLetterViewModel : BaseViewModel
	{
		public int GrantApplicationId { get; set; }
		public string Body { get; set; }

		public GrantAgreementCoverLetterViewModel() { }
		public GrantAgreementCoverLetterViewModel(GrantApplication grantApplication, int? version)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));

			this.GrantApplicationId = grantApplication.Id;
			this.Body = version != null && grantApplication.GrantAgreement.ScheduleA.Versions.Any()
				? grantApplication.GrantAgreement.CoverLetter.Versions.FirstOrDefault(o => o.VersionNumber == version)?.Body ?? grantApplication.GrantAgreement.CoverLetter.Body
				: grantApplication.GrantAgreement.CoverLetter.Body;
		}
	}
}

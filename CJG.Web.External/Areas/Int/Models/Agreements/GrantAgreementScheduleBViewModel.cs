using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models.Agreements
{
	public class GrantAgreementScheduleBViewModel : BaseViewModel
	{
		public int GrantApplicationId { get; set; }
		public string Body { get; set; }

		public GrantAgreementScheduleBViewModel() { }
		public GrantAgreementScheduleBViewModel(GrantApplication grantApplication, int? version)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));

			this.GrantApplicationId = grantApplication.Id;
			this.Body = version != null && grantApplication.GrantAgreement.ScheduleA.Versions.Any()
				? grantApplication.GrantAgreement.ScheduleB.Versions.FirstOrDefault(o => o.VersionNumber == version)?.Body ?? grantApplication.GrantAgreement.ScheduleB.Body
				: grantApplication.GrantAgreement.ScheduleB.Body;
		}
	}
}

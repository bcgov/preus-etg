using CJG.Core.Entities;
using System;

namespace CJG.Web.External.Areas.Int.Models.BatchApprovals
{
	public class IssueOfferViewModel
	{
		#region Properties
		public int Id { get; set; }
		public string RowVersion { get; set; }
		public string FileNumber { get; set; }
		public string ErrorMessage { get; set; }
		#endregion

		#region Constructors
		public IssueOfferViewModel() { }

		public IssueOfferViewModel(GrantApplication grantApplication)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));

			this.Id = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.FileNumber = grantApplication.FileNumber;
		}
		#endregion
	}
}
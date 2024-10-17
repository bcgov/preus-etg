using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Models.Shared;
using System;
using System.Security.Principal;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class ClaimListSharedViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }
		public bool HasAM1 { get; set; }
		public bool HasAM2 { get; set; }
		public bool HasAM5 { get; set; }
		public bool IsAssessor { get; set; }
		public bool AllowViewClaim { get; set; }
		public bool HoldPaymentRequests { get; set; }
		#endregion

		#region Constructors
		public ClaimListSharedViewModel() { }


		public ClaimListSharedViewModel(GrantApplication grantApplication, IPrincipal user)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			if (user == null) throw new ArgumentNullException(nameof(user));

			this.Id = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.HasAM1 = user.HasPrivilege(Privilege.AM1);
			this.HasAM2 = user.HasPrivilege(Privilege.AM2);
			this.HasAM5 = user.HasPrivilege(Privilege.AM5);
			this.IsAssessor = grantApplication.AssessorId == user.GetUserId();
			this.AllowViewClaim = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.ViewClaim);
			this.HoldPaymentRequests = grantApplication.HoldPaymentRequests;
		}
		#endregion
	}
}

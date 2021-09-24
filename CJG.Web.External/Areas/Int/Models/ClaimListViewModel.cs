using CJG.Core.Entities;
using CJG.Web.External.Areas.Ext.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace CJG.Web.External.Areas.Int.Models
{
	public class ClaimListViewModel : ClaimListSharedViewModel
	{
		#region Properties
		public IEnumerable<ClaimViewModel> ClaimViewModels { get; set; }
		#endregion

		#region Constructors
		public ClaimListViewModel() { }

		public ClaimListViewModel(GrantApplication grantApplication, IPrincipal user) : base(grantApplication, user)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			if (user == null) throw new ArgumentNullException(nameof(user));

			var claims = grantApplication.Claims.Where(c => c.ClaimState.In(
					ClaimState.Unassessed,
					ClaimState.ClaimApproved,
					ClaimState.ClaimDenied,
					ClaimState.ClaimAmended,
					ClaimState.PaymentRequested,
					ClaimState.AmountOwing,
					ClaimState.ClaimPaid,
					ClaimState.AmountReceived)
					|| (c.ClaimState.In(ClaimState.Incomplete)
					&& c.DateSubmitted.HasValue
					&& grantApplication.ApplicationStateInternal != ApplicationStateInternal.ClaimReturnedToApplicant)||
					(c.ClaimState.In(ClaimState.Incomplete,ClaimState.Complete) && c.GrantApplication.ApplicationStateInternal == ApplicationStateInternal.ClaimReturnedToApplicant));

			this.ClaimViewModels = claims.Select(c => new ClaimViewModel(c)).ToArray();
		}
		#endregion


	}
}

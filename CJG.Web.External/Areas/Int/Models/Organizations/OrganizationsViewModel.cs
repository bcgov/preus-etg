using CJG.Application.Services;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Models.Shared;
using System;
using System.Security.Principal;

namespace CJG.Web.External.Areas.Int.Models.Organizations
{
	public class OrganizationsViewModel : BaseViewModel
	{
		#region Properties
		public bool AdminUser { get; set; }
		public bool AllowDeleteOrganization { get; set; }
		#endregion

		#region Constructors
		public OrganizationsViewModel() { }

		public OrganizationsViewModel(IPrincipal user)
		{
			if (user == null) throw new ArgumentNullException(nameof(user));
			this.AdminUser = user.HasPrivilege(Privilege.TP2);
			this.AllowDeleteOrganization = user.IsInRole("Director") || user.IsInRole("System Administrator");
		}
		#endregion
	}
}

using CJG.Application.Services;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Models.Shared;
using System;
using System.Security.Principal;

namespace CJG.Web.External.Areas.Int.Models.TrainingProviders
{
	public class TrainingProvidersInventoryViewModel : BaseViewModel
	{
		#region Properties
		public bool AdminUser { get; set; }
		#endregion

		#region Constructors
		public TrainingProvidersInventoryViewModel() { }

		public TrainingProvidersInventoryViewModel(IPrincipal user)
		{
			if (user == null) throw new ArgumentNullException(nameof(user));
			this.AdminUser = user.HasPrivilege(Privilege.TP2);
		}
		#endregion
	}
}

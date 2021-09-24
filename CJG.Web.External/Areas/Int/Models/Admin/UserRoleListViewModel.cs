using CJG.Infrastructure.Identity;
using CJG.Web.External.Models.Shared;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models
{
	public class UserRoleListViewModel : BaseViewModel
	{
		#region Properties
		public IEnumerable<UserRoleViewModel> Roles { get; set; }
		#endregion

		#region Constructors
		public UserRoleListViewModel() { }

		public UserRoleListViewModel(ApplicationRole[] roles) {
			Roles = roles.Select(r => new UserRoleViewModel() { RoleId = r.Id, RoleName = r.Name }).ToList();
		}
		#endregion
	}
}

using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models
{
	public class UserRoleViewModel : BaseViewModel
	{
		#region Properties
		public string RoleId { get; set; }
		public string RoleName { get; set; }
		#endregion

		#region Constructors
		public UserRoleViewModel() { }
		#endregion
	}
}

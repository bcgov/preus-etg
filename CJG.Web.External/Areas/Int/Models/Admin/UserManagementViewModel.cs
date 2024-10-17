using CJG.Core.Entities.Helpers;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models
{
	public class UserManagementViewModel : BaseViewModel
	{
		#region Properties
        public PageList<UserViewModel> Users { get; set; }
		#endregion

		#region Constructors
		public UserManagementViewModel() { }

		public UserManagementViewModel(PageList<UserViewModel> pageListOfUsers) {
			Users = pageListOfUsers;
		}
		#endregion
	}
}
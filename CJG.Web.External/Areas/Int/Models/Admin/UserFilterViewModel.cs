using CJG.Core.Entities.Helpers;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models
{
	public class UserFilterViewModel : BaseViewModel
	{
		#region Properties
		public string SearchCriteria { get; set; }
		public string[] OrderBy { get; set; }
		#endregion

		#region Constructors
		public UserFilterViewModel() { }
		#endregion

		#region Methods
		public UserFilter GetFilter()
		{
			return new UserFilter(this.SearchCriteria, this.OrderBy);
		}
		#endregion
	}
}

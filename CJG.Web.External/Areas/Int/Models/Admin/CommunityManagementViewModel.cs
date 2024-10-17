using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models
{
	public class CommunityManagementViewModel : BaseViewModel
    {
        public List<CommunityViewModel> Communities { get; set; } = new List<CommunityViewModel>();

		public CommunityManagementViewModel() { }

		public CommunityManagementViewModel(IEnumerable<Community> communities) {
			this.Communities = communities.Select(c => new CommunityViewModel(c)).ToList();
		}
	}
}
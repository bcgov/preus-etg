using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models
{
	public class WDAServiceViewModel : BaseViewModel
	{
		#region Properties
		public IEnumerable<ServiceCategoryViewModel> ServiceCategories { get; set; } = new List<ServiceCategoryViewModel>();
		public bool Deleted { get; set; }
		#endregion

		#region Constructors
		public WDAServiceViewModel() { }

		public WDAServiceViewModel(IEnumerable<ServiceCategory> serviceCategories) 
		{
			this.ServiceCategories = serviceCategories.Select(sc => new ServiceCategoryViewModel(sc)).ToArray();
		}
		#endregion
	}
}

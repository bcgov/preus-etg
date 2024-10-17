using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models
{
	public class ServiceCategoryViewModel : LookupTableViewModel
	{
		#region Properties
		[Required(ErrorMessage = "Service Type is required")]
		public ServiceTypes? ServiceTypeId { get; set; }

		public IEnumerable<ServiceLineViewModel> ServiceLines { get; set; } = new List<ServiceLineViewModel>();
		public bool AutoInclude { get; set; }
		public bool AllowMultiple { get; set; }
		public int MinPrograms { get; set; }
		public int MaxPrograms { get; set; }
		public int MinProviders { get; set; }
		public int MaxProviders { get; set; }
		public bool CompletionReport { get; set; }
		#endregion

		#region Constructors
		public ServiceCategoryViewModel() { }

		public ServiceCategoryViewModel(ServiceCategory serviceCategory)
		{
			Utilities.MapProperties(serviceCategory, this);
			this.AllowDelete = !serviceCategory.EligibleExpenseTypes.Any(eet => eet.EligibleCosts.Any());
			this.ServiceLines = serviceCategory.ServiceLines.OrderBy(sl => sl.RowSequence).ThenBy(sl => sl.Caption).Select(sl => new ServiceLineViewModel(sl)).ToArray();
		}
		#endregion
	}
}
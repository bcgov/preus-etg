using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models
{
	public class ServiceLineViewModel : LookupTableViewModel
	{
		#region Properties
		public IEnumerable<LookupTableViewModel> ServiceLineBreakdowns { get; set; } = new List<LookupTableViewModel>();
		public string BreakdownCaption { get; set; }
		public bool EnableCost { get; set; }
		#endregion

		#region Constructors
		public ServiceLineViewModel() { }

		public ServiceLineViewModel(ServiceLine serviceLine)
		{
			Utilities.MapProperties(serviceLine, this);

			this.AllowDelete = !serviceLine.EligibleExpenseBreakdowns.Any(eeb => eeb.EligibleCostBreakdowns.Any() || eeb.EligibleCostBreakdowns.Any(ecb => ecb.TrainingPrograms.Any()));
			this.ServiceLineBreakdowns = serviceLine.ServiceLineBreakdowns.OrderBy(slb => slb.RowSequence).ThenBy(slb => slb.Caption).Select(slb => new LookupTableViewModel(slb)
			{
				AllowDelete = !slb.TrainingPrograms.Any()
			}).ToArray();
		}
		#endregion
	}
}
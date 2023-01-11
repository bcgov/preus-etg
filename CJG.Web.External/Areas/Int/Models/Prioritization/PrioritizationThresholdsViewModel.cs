using System.ComponentModel.DataAnnotations;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.Prioritization
{
	public class PrioritizationThresholdsViewModel : BaseViewModel
	{
		[Required(ErrorMessage = "You must enter a numeric Industry Threshold.")]
		public int IndustryThreshold { get; set; }

		[Required(ErrorMessage = "You must enter a numeric Regional Threshold.")]
		public decimal RegionalThreshold { get; set; }

		[Required(ErrorMessage = "You must enter an Employee Count Threshold.")]
		[Range(1, 999999, ErrorMessage = "The Employee Count Threshold must be between 1 and 999,999")]
		public int EmployeeCountThreshold { get; set; }
	}
}
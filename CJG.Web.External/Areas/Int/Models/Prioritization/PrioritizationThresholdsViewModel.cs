using System.ComponentModel.DataAnnotations;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.Prioritization
{
    public class PrioritizationThresholdsViewModel : BaseViewModel
	{
		[Required(ErrorMessage = "You must enter a numeric Industry Threshold.")]
		public int IndustryThreshold { get; set; }

		[Required(ErrorMessage = "You must enter a numeric High Opportunity Occupation Threshold.")]
		public decimal HighOpportunityOccupationThreshold { get; set; }

		[Required(ErrorMessage = "You must enter a numeric Regional Threshold.")]
		public decimal RegionalThreshold { get; set; }

		[Required(ErrorMessage = "You must enter an Employee Count Threshold.")]
		[Range(1, 999999, ErrorMessage = "The Employee Count Threshold must be between 1 and 999,999")]
		public int EmployeeCountThreshold { get; set; }

		[Required(ErrorMessage = "You must enter a numeric Industry Score.")]
		public int IndustryAssignedScore { get; set; }

		[Required(ErrorMessage = "You must enter a numeric High Opportunity Occupation Score.")]
		public int HighOpportunityOccupationAssignedScore { get; set; }

		[Required(ErrorMessage = "You must enter a numeric Regional Score.")]
		public int RegionalThresholdAssignedScore { get; set; }

		[Required(ErrorMessage = "You must enter a numeric Employee Count Score.")]
		public int EmployeeCountAssignedScore { get; set; }

		[Required(ErrorMessage = "You must enter a numeric First Time Applicant Score.")]
		public int FirstTimeApplicantAssignedScore { get; set; }

	}
}
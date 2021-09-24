using CJG.Web.External.Helpers.Validation;
using CJG.Web.External.Models.Shared;
using DataAnnotationsExtensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace CJG.Web.External.Areas.Int.Models.SkillsTraining
{
	public class UpdateSkillsTrainingProgramViewModel : BaseViewModel
	{
		#region Properties
		public int GrantApplicationId { get; set; }

		[Min(1, ErrorMessage = "Skills training focus is required")]
		public int ServiceLineId { get; set; }

		public int? ServiceLineBreakdownId { get; set; }

		public int EligibleCostId { get; set; }

		[Required(ErrorMessage = "Start date is required and must be within the program delivery dates")]
		public DateTime? StartDate { get; set; }

		[Required(ErrorMessage = "End date is required and must be within the program delivery dates")]
		public DateTime? EndDate { get; set; }

		[Required(ErrorMessage = "Course title is required")]
		public string CourseTitle { get; set; }

		[Required(ErrorMessage = "Total training hours must be greater than or equal to 1"), Min(1, ErrorMessage = "Total training hours must be greater than or equal to 1")]
		public int? TotalTrainingHours { get; set; }

		public string TitleOfQualification { get; set; }

		[RequiredEnumerable(ErrorMessage = "A Delivery method is required")]
		public int[] SelectedDeliveryMethodIds { get; set; }

		[Required(ErrorMessage = "Expected qualification is required")]
		public int? ExpectedQualificationId { get; set; }

		[Required(ErrorMessage = "Skill level is required")]
		public int? SkillLevelId { get; set; }

		public decimal AgreedCost { get; set; }

		public UpdateSkillsTrainingProviderViewModel TrainingProvider { get; set; }
		#endregion

		#region Constructors
		public UpdateSkillsTrainingProgramViewModel() { }
		#endregion
	}
}
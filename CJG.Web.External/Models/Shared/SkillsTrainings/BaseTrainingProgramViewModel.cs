using CJG.Web.External.Helpers.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace CJG.Web.External.Models.Shared.SkillsTrainings
{
	public abstract class BaseTrainingProgramViewModel
	{
		#region Properties
		public int Id { get; set; }

		public int GrantApplicationId { get; set; }

		public string RowVersion { get; set; }

		public DateTime DeliveryStartDate { get; set; }

		public DateTime DeliveryEndDate { get; set; }

		[Required(ErrorMessage = "The Start Date field is required.")]
		public DateTime? StartDate { get; set; }
		public int StartYear { get; set; }
		public int StartMonth { get; set; }
		public int StartDay { get; set; }

		[Required(ErrorMessage = "The End Date field is required.")]
		public DateTime? EndDate { get; set; }
		public int EndYear { get; set; }
		public int EndMonth { get; set; }
		public int EndDay { get; set; }

		[Required(ErrorMessage = "You must enter a training course title."), MaxLength(500)]
		public string CourseTitle { get; set; }

		[Required(ErrorMessage = "Total training hours is required.")]
		[Helpers.ValidateNullableInt(ErrorMessage = "Please enter total training hours to the nearest hour.")]
		public int? TotalTrainingHours { get; set; }

		[Required(ErrorMessage = "If you have expected qualifications you must include the title of the qualification."), MaxLength(500)]
		public string TitleOfQualification { get; set; }

		[RequiredEnumerable(ErrorMessage = "The Primary Delivery Method is required.")]
		public int[] SelectedDeliveryMethodIds { get; set; }

		[Required(ErrorMessage = "You must select an expected qualification.")]
		public int? ExpectedQualificationId { get; set; }

		[Required(ErrorMessage = "You must select a skill level.")]
		public int? SkillLevelId { get; set; }
		#endregion
	}
}

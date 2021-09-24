using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Helpers;
using CJG.Web.External.Models.Shared;
using CJG.Web.External.Models.Shared.SkillsTrainings;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;

namespace CJG.Web.External.Areas.Int.Models.SkillsTraining
{
	public class SkillsTrainingProgramViewModel : BaseViewModel
	{
		#region Properties
		public bool CanEdit { get; set; }

		public bool CanRemove { get; set; }

		public string RowVersion { get; set; }

		public int GrantApplicationId { get; set; }

		public int ServiceLineId { get; set; }

		public int? ServiceLineBreakdownId { get; set; }

		public int EligibleCostId { get; set; }

		public int EligibleCostBreakdownId { get; set; }

		public string EligibleCostBreakdownRowVersion { get; set; }

		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		public string CourseTitle { get; set; }

		public int? TotalTrainingHours { get; set; }

		public string TitleOfQualification { get; set; }

		public int[] SelectedDeliveryMethodIds { get; set; }

		public int? ExpectedQualificationId { get; set; }

		public int? SkillLevelId { get; set; }

		public decimal EstimatedCost { get; set; }

		public decimal AgreedCost { get; set; }
		[CustomValidation(typeof(ProgramDescriptionViewModelValidation), "ValidateCIPS")]
		public int? CipsCode1Id { get; set; }

		[CustomValidation(typeof(ProgramDescriptionViewModelValidation), "ValidateCIPS")]
		public int? CipsCode2Id { get; set; }

		[CustomValidation(typeof(ProgramDescriptionViewModelValidation), "ValidateCIPS")]
		public int? CipsCode3Id { get; set; }
		#endregion

		#region Constructors
		public SkillsTrainingProgramViewModel() { }

		public SkillsTrainingProgramViewModel(TrainingProgram trainingProgram, IPrincipal user, ICipsCodesService _cipsCodesService)
		{
			if (trainingProgram == null) throw new ArgumentNullException(nameof(trainingProgram));
			if (user == null) throw new ArgumentNullException(nameof(user));

			this.Id = trainingProgram.Id;
			this.RowVersion = trainingProgram.RowVersion != null ? Convert.ToBase64String(trainingProgram.RowVersion) : null;
			this.GrantApplicationId = trainingProgram.GrantApplicationId;

			this.CanEdit = user.CanPerformAction(trainingProgram.GrantApplication, ApplicationWorkflowTrigger.EditTrainingProgram);
			this.CanRemove = (trainingProgram.EligibleCostBreakdown?.AddedByAssessor ?? true) && user.CanPerformAction(trainingProgram.GrantApplication, ApplicationWorkflowTrigger.AddOrRemoveTrainingProgram);

			this.ServiceLineId = trainingProgram.ServiceLineId ?? throw new ArgumentException("The training program is not a valid skills training component.", nameof(trainingProgram));
			this.ServiceLineBreakdownId = trainingProgram.ServiceLineBreakdownId;
			this.EligibleCostId = trainingProgram.EligibleCostBreakdown?.EligibleCostId ?? throw new ArgumentException("The training program is not a valid skills training component.", nameof(trainingProgram));
			this.EligibleCostBreakdownId = trainingProgram.EligibleCostBreakdownId ?? throw new ArgumentException("The training program is not a valid skills training component.", nameof(trainingProgram));
			this.EligibleCostBreakdownRowVersion = Convert.ToBase64String(trainingProgram.EligibleCostBreakdown.RowVersion);
			this.StartDate = trainingProgram.StartDate.ToLocalTime();
			this.EndDate = trainingProgram.EndDate.ToLocalTime();
			this.CourseTitle = trainingProgram.CourseTitle;
			this.TotalTrainingHours = trainingProgram.TotalTrainingHours;
			this.TitleOfQualification = trainingProgram.TitleOfQualification;
			this.SelectedDeliveryMethodIds = trainingProgram.DeliveryMethods.Select(dm => dm.Id).ToArray();
			this.ExpectedQualificationId = trainingProgram.ExpectedQualificationId;
			this.SkillLevelId = trainingProgram.SkillLevelId;
			this.EstimatedCost = trainingProgram.EligibleCostBreakdown.EstimatedCost;
			this.AgreedCost = trainingProgram.EligibleCostBreakdown.AssessedCost;

			#region CIPS Codes
			var cipsCodes = _cipsCodesService.GetListOfCipsCodes(trainingProgram.CipsCode == null ? 0 : trainingProgram.CipsCode.Id);

			cipsCodes.ForEach(item =>
			{
				var property = GetType().GetProperty($"CipsCode{item.Level}Id");
				property?.SetValue(this, item.Id);
			});
			#endregion
		}
		#endregion

		#region Methods
		public static explicit operator SkillTrainingViewModel(SkillsTrainingProgramViewModel model)
		{
			var result = new SkillTrainingViewModel();
			Utilities.MapProperties(model, result);
			return result;
		}
		#endregion
	}
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Int.Models.Attachments;
using CJG.Web.External.Helpers;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.TrainingPrograms
{
    public class TrainingProgramViewModel : BaseViewModel
	{
		public string RowVersion { get; set; }
		public string CourseTitle { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public DateTime MinStartDate { get; set; }
		public DateTime MaxStartDate { get; set; }
		public DateTime MinEndDate { get; set; }
		public DateTime MaxEndDate { get; set; }
		public int? InDemandOccupationId { get; set; }
		public int SkillLevelId { get; set; }
		public int? SkillFocusId { get; set; }
		public int ExpectedQualificationId { get; set; }
		public int? TrainingLevelId { get; set; }
		public string TrainingBusinessCase { get; set; }
		public int TotalTrainingHours { get; set; }
		public string TitleOfQualification { get; set; }

		public bool HasOfferedThisTypeOfTrainingBefore { get; set; }
		public bool HasRequestedAdditionalFunding { get; set; }
		public string DescriptionOfFundingRequested { get; set; }
		public bool? MemberOfUnderRepresentedGroup { get; set; }
		public TrainingProgramStates TrainingProgramState { get; set; }
		public int? ServiceLineId { get; set; }
		public int? ServiceLineBreakdownId { get; set; }
		public int? EligibleCostBreakdownId { get; set; }

		public AttachmentViewModel CourseOutlineDocument { get; set; }
		public string CourseLink { get; set; }

		public IEnumerable<int> SelectedDeliveryMethodIds { get; set; }
		public IEnumerable<int> SelectedUnderRepresentedGroupIds { get; set; }

		[CustomValidation(typeof(ProgramDescriptionViewModelValidation), "ValidateCIPS")]
		public int? CipsCode1Id { get; set; }
		[CustomValidation(typeof(ProgramDescriptionViewModelValidation), "ValidateCIPS")]
		public int? CipsCode2Id { get; set; }
		[CustomValidation(typeof(ProgramDescriptionViewModelValidation), "ValidateCIPS")]
		public int? CipsCode3Id { get; set; }

		public bool RequiresCIPSValidation { get; set; }
		public bool CanEdit { get; set; }

		public TrainingProgramViewModel() { }

		public TrainingProgramViewModel(TrainingProgram trainingProgram, ICipsCodesService cipsCodesService)
		{
			Id = trainingProgram?.Id ?? throw new ArgumentNullException(nameof(trainingProgram));
			Utilities.MapProperties(trainingProgram, this);
			StartDate = trainingProgram.StartDate.ToLocalMorning();
			EndDate = trainingProgram.EndDate.ToLocalMidnight();

			MinStartDate = trainingProgram.GrantApplication.GrantOpening.TrainingPeriod.StartDate;
			MaxStartDate = trainingProgram.GrantApplication.GrantOpening.TrainingPeriod.EndDate;
			MaxEndDate = trainingProgram.GrantApplication.GrantOpening.TrainingPeriod.EndDate.AddYears(1);

			SelectedDeliveryMethodIds = trainingProgram.DeliveryMethods.Select(dm => dm.Id).ToArray();
			SelectedUnderRepresentedGroupIds = trainingProgram.UnderRepresentedGroups.Select(dm => dm.Id).ToArray();
			CanEdit = true;
			CourseLink = trainingProgram.CourseLink;
			RequiresCIPSValidation = trainingProgram.CipsCode == null;

			if (trainingProgram.CourseOutlineDocument != null)
				CourseOutlineDocument = new AttachmentViewModel(trainingProgram.CourseOutlineDocument);

			MapCipsCodes(trainingProgram, cipsCodesService);
		}

		private void MapCipsCodes(TrainingProgram trainingProgram, ICipsCodesService cipsCodesService)
		{
			var cipsCodes = cipsCodesService.GetListOfCipsCodes(trainingProgram.CipsCode == null ? 0 : trainingProgram.CipsCode.Id);
			cipsCodes.ForEach(item =>
			{
				var property = GetType().GetProperty($"CipsCode{item.Level}Id");
				property?.SetValue(this, item.Id);
			});
		}

		public void MapTo(TrainingProgram trainingProgram, IStaticDataService staticDataService)
		{
			if (trainingProgram == null) throw new ArgumentNullException(nameof(trainingProgram));
			if (staticDataService == null) throw new ArgumentNullException(nameof(staticDataService));

			Utilities.MapProperties(this, trainingProgram);
			trainingProgram.StartDate = StartDate.ToUtcMorning();
			trainingProgram.EndDate = EndDate.ToUtcMidnight();

			// Only add/remove the specified delivery methods.
			if (SelectedDeliveryMethodIds?.Any() ?? false)
			{
				var modelIds = SelectedDeliveryMethodIds.ToArray();
				var currentIds = trainingProgram.DeliveryMethods.Select(dm => dm.Id).ToArray();
				var removeIds = currentIds.Except(modelIds);
				var addIds = modelIds.Except(currentIds).Except(removeIds);

				foreach (var id in removeIds)
				{
					var remove = staticDataService.GetDeliveryMethod(id);
					trainingProgram.DeliveryMethods.Remove(remove);
				}

				foreach (var id in addIds)
				{
					var add = staticDataService.GetDeliveryMethod(id);
					trainingProgram.DeliveryMethods.Add(add);
				}
			}
			else
			{
				// Remove all the delivery methods.
				trainingProgram.DeliveryMethods.Clear();
			}

			// Only add/remove the specified underrepresented groups.
			if ((MemberOfUnderRepresentedGroup.HasValue && MemberOfUnderRepresentedGroup.Value) && (SelectedUnderRepresentedGroupIds?.Any() ?? false))
			{
				var modelIds = SelectedUnderRepresentedGroupIds.ToArray();
				var currentIds = trainingProgram.UnderRepresentedGroups.Select(dm => dm.Id).ToArray();
				var removeIds = currentIds.Except(modelIds);
				var addIds = modelIds.Except(currentIds).Except(removeIds);

				foreach (var id in removeIds)
				{
					var remove = staticDataService.GetUnderRepresentedGroup(id);
					trainingProgram.UnderRepresentedGroups.Remove(remove);
				}

				foreach (var id in addIds)
				{
					var add = staticDataService.GetUnderRepresentedGroup(id);
					trainingProgram.UnderRepresentedGroups.Add(add);
				}
			}
			else
			{
				// Remove all the underrepresented groups.
				trainingProgram.UnderRepresentedGroups.Clear();
			}

			trainingProgram.TargetCipsCodeId = CipsCode3Id ?? CipsCode2Id ?? CipsCode1Id;
		}
	}
}
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models.TrainingPrograms
{
	public class TrainingProgramViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }
		public string CourseTitle { get; set; }
		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }
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

		public IEnumerable<int> SelectedDeliveryMethodIds { get; set; }
		public IEnumerable<int> SelectedUnderRepresentedGroupIds { get; set; }
		#endregion

		#region Constructors
		public TrainingProgramViewModel() { }

		public TrainingProgramViewModel(TrainingProgram trainingProgram)
		{
			this.Id = trainingProgram?.Id ?? throw new ArgumentNullException(nameof(trainingProgram));
			Utilities.MapProperties(trainingProgram, this);
			this.StartDate = trainingProgram.StartDate.ToLocalMorning();
			this.EndDate = trainingProgram.EndDate.ToLocalMidnight();

			this.SelectedDeliveryMethodIds = trainingProgram.DeliveryMethods.Select(dm => dm.Id).ToArray();
			this.SelectedUnderRepresentedGroupIds = trainingProgram.UnderRepresentedGroups.Select(dm => dm.Id).ToArray();
		}
		#endregion

		#region Methods
		public void MapTo(TrainingProgram trainingProgram, IStaticDataService staticDataService)
		{
			if (trainingProgram == null) throw new ArgumentNullException(nameof(trainingProgram));
			if (staticDataService == null) throw new ArgumentNullException(nameof(staticDataService));

			Utilities.MapProperties(this, trainingProgram);
			trainingProgram.StartDate = this.StartDate.ToUtcMorning();
			trainingProgram.EndDate = this.EndDate.ToUtcMidnight();

			// Only add/remove the specified delivery methods.
			if (this.SelectedDeliveryMethodIds?.Any() ?? false)
			{
				var modelIds = this.SelectedDeliveryMethodIds.ToArray();
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
			if ((this.MemberOfUnderRepresentedGroup.HasValue && this.MemberOfUnderRepresentedGroup.Value) && (this.SelectedUnderRepresentedGroupIds?.Any() ?? false))
			{
				var modelIds = this.SelectedUnderRepresentedGroupIds.ToArray();
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
		}
		#endregion
	}
}
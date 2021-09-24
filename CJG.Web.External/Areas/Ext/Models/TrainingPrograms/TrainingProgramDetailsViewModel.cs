using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared.SkillsTrainings;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;

namespace CJG.Web.External.Areas.Ext.Models.TrainingPrograms
{
	public class TrainingProgramDetailsViewModel : BaseTrainingProgramViewModel
	{
		#region Properties

		[Required(ErrorMessage = "You must select an in-demand occupation.")]
		public int? InDemandOccupationId { get; set; }
		[Required(ErrorMessage = "You must select a skills focus.")]
		public int? SkillFocusId { get; set; }
		[Required(ErrorMessage = "You must select a training level.")]
		public int? TrainingLevelId { get; set; }
		public string TrainingBusinessCase { get; set; }
		[Required(ErrorMessage = "You must select whether you have offered this type of training before.")]
		public bool? HasOfferedThisTypeOfTrainingBefore { get; set; }
		[Required(ErrorMessage = "You must select whether you have previously received or are requesting additional funding.")]
		public bool? HasRequestedAdditionalFunding { get; set; }
		[Required(ErrorMessage = "You must describe the funding received or requested for this training.")]
		public string DescriptionOfFundingRequested { get; set; }
		[Required(ErrorMessage = "You must select whether you are a member of an underrepresented group.")]
		public bool? MemberOfUnderRepresentedGroup { get; set; }
		public int[] SelectedUnderRepresentedGroupIds { get; set; }
		#endregion

		#region Constructors
		public TrainingProgramDetailsViewModel()
		{
		}

		public TrainingProgramDetailsViewModel(TrainingProgram trainingProgram)
		{
			if (trainingProgram == null)
				throw new ArgumentNullException(nameof(trainingProgram));
			this.Id = trainingProgram.Id;
			this.RowVersion = trainingProgram.RowVersion == null ? null : Convert.ToBase64String(trainingProgram.RowVersion);
			this.InDemandOccupationId = trainingProgram.InDemandOccupationId == 0 ? null : (int?)trainingProgram.InDemandOccupationId;
			this.SkillLevelId = trainingProgram.SkillLevelId == 0 ? null : (int?)trainingProgram.SkillLevelId;
			this.SkillFocusId = trainingProgram.SkillFocusId == 0 ? null : (int?)trainingProgram.SkillFocusId;
			this.ExpectedQualificationId = trainingProgram.ExpectedQualificationId == 0 ? null : (int?)trainingProgram.ExpectedQualificationId;
			this.TrainingLevelId = trainingProgram.TrainingLevelId == 0 ? null : trainingProgram.TrainingLevelId;
			this.CourseTitle = trainingProgram.CourseTitle;
			this.TrainingBusinessCase = trainingProgram.TrainingBusinessCase;
			this.TotalTrainingHours = trainingProgram.TotalTrainingHours == 0 ? null : (int?)trainingProgram.TotalTrainingHours;
			this.TitleOfQualification = trainingProgram.TitleOfQualification;
			this.HasOfferedThisTypeOfTrainingBefore = trainingProgram.Id == 0 ? null : (bool?)trainingProgram.HasOfferedThisTypeOfTrainingBefore;
			this.HasRequestedAdditionalFunding = trainingProgram.Id == 0 ? null : (bool?)trainingProgram.HasRequestedAdditionalFunding;
			this.DescriptionOfFundingRequested = trainingProgram.DescriptionOfFundingRequested;
			this.MemberOfUnderRepresentedGroup = trainingProgram.MemberOfUnderRepresentedGroup;

			if (trainingProgram.GrantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.EmployerGrant)
			{
				this.StartDate = trainingProgram.GrantApplication.StartDate.ToLocalTime();
				this.StartYear = trainingProgram.GrantApplication.StartDate.ToLocalTime().Year;
				this.StartMonth = trainingProgram.GrantApplication.StartDate.ToLocalTime().Month;
				this.StartDay = trainingProgram.GrantApplication.StartDate.ToLocalTime().Day;
				this.EndDate = trainingProgram.GrantApplication.EndDate.ToLocalTime();
				this.EndYear = trainingProgram.GrantApplication.EndDate.ToLocalTime().Year;
				this.EndMonth = trainingProgram.GrantApplication.EndDate.ToLocalTime().Month;
				this.EndDay = trainingProgram.GrantApplication.EndDate.ToLocalTime().Day;
			}
			else
			{
				this.StartDate = trainingProgram.StartDate.ToLocalTime();
				this.StartYear = trainingProgram.StartDate.ToLocalTime().Year;
				this.StartMonth = trainingProgram.StartDate.ToLocalTime().Month;
				this.StartDay = trainingProgram.StartDate.ToLocalTime().Day;
				this.EndDate = trainingProgram.EndDate.ToLocalTime();
				this.EndYear = trainingProgram.EndDate.ToLocalTime().Year;
				this.EndMonth = trainingProgram.EndDate.ToLocalTime().Month;
				this.EndDay = trainingProgram.EndDate.ToLocalTime().Day;
			}

			this.SelectedDeliveryMethodIds = trainingProgram.DeliveryMethods.Select(dm => dm.Id).ToArray();
			this.SelectedUnderRepresentedGroupIds = trainingProgram.UnderRepresentedGroups.Select(dm => dm.Id).ToArray();
			this.GrantApplicationId = trainingProgram.GrantApplicationId;
			this.DeliveryStartDate = trainingProgram.GrantApplication.StartDate.ToLocalTime();
			this.DeliveryEndDate = trainingProgram.GrantApplication.EndDate.ToLocalTime();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Add/Update the specified training program in the datasource.
		/// </summary>
		/// <param name="_grantApplicationService"></param>
		/// <param name="_trainingProgramService"></param>
		/// <param name="_trainingProviderService"></param>
		/// <param name="_staticDataService"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		public TrainingProgram UpdateTrainingProgram(IGrantApplicationService _grantApplicationService,
													 ITrainingProgramService _trainingProgramService,
													 ITrainingProviderService _trainingProviderService,
													 IStaticDataService _staticDataService,
													 IPrincipal user)
		{
			if (_grantApplicationService == null) throw new ArgumentNullException(nameof(_grantApplicationService));
			if (_trainingProgramService == null) throw new ArgumentNullException(nameof(_trainingProgramService));
			if (_trainingProviderService == null) throw new ArgumentNullException(nameof(_trainingProviderService));
			if (_staticDataService == null) throw new ArgumentNullException(nameof(_staticDataService));
			if (user == null) throw new ArgumentNullException(nameof(user));

			var grantApplication = _grantApplicationService.Get(this.GrantApplicationId);
			var create = this.Id == 0;

			if (create && !user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.AddOrRemoveTrainingProgram))
				throw new NotAuthorizedException($"User does not have permission add the training program in Grant Application '{grantApplication?.Id}'.");
			else if (!user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditTrainingProgram))
				throw new NotAuthorizedException($"User does not have permission edit the training program in Grant Application '{grantApplication?.Id}'.");

			var trainingProgram = !create ? _trainingProgramService.Get(this.Id) : new TrainingProgram(grantApplication);
			if (!create)
			{
				trainingProgram.RowVersion = Convert.FromBase64String(this.RowVersion);
			}

			trainingProgram.TrainingProgramState = TrainingProgramStates.Complete;
			trainingProgram.StartDate = ((DateTime)this.StartDate).ToLocalMorning().ToUtcMorning();
			trainingProgram.EndDate = ((DateTime)this.EndDate).ToLocalMidnight().ToUtcMidnight();
			trainingProgram.CourseTitle = this.CourseTitle;

			// Only add/remove the specified delivery methods.
			if (this.SelectedDeliveryMethodIds != null && this.SelectedDeliveryMethodIds.Any())
			{
				var thisIds = this.SelectedDeliveryMethodIds.ToArray();
				var currentIds = trainingProgram.DeliveryMethods.Select(dm => dm.Id).ToArray();
				var removeIds = currentIds.Except(thisIds);
				var addIds = thisIds.Except(currentIds).Except(removeIds);

				foreach (var removeId in removeIds)
				{
					var deliveryMethod = _staticDataService.GetDeliveryMethod(removeId);
					trainingProgram.DeliveryMethods.Remove(deliveryMethod);
				}

				foreach (var addId in addIds)
				{
					var deliveryMethod = _staticDataService.GetDeliveryMethod(addId);
					trainingProgram.DeliveryMethods.Add(deliveryMethod);
				}
			}
			else
			{
				// Remove all the delivery methods.
				trainingProgram.DeliveryMethods.Clear();
			}

			trainingProgram.SkillFocusId = this.SkillFocusId.Value;
			trainingProgram.SkillLevelId = this.SkillLevelId.Value;
			trainingProgram.TotalTrainingHours = this.TotalTrainingHours.Value;
			trainingProgram.HasOfferedThisTypeOfTrainingBefore = this.HasOfferedThisTypeOfTrainingBefore.Value;
			trainingProgram.HasRequestedAdditionalFunding = this.HasRequestedAdditionalFunding.Value;

			if (this.HasRequestedAdditionalFunding.Value)
			{
				trainingProgram.DescriptionOfFundingRequested = this.DescriptionOfFundingRequested;
			}
			else
			{
				trainingProgram.DescriptionOfFundingRequested = null;
			}

			trainingProgram.ExpectedQualificationId = this.ExpectedQualificationId.Value;
			if (new int[] { 5 }.Contains(this.ExpectedQualificationId.GetValueOrDefault()))
			{
				trainingProgram.TitleOfQualification = null;
			}
			else
			{
				trainingProgram.TitleOfQualification = this.TitleOfQualification;
			}

			if (new int[] { 5 }.Contains(this.SkillFocusId.GetValueOrDefault()))
			{
				trainingProgram.InDemandOccupationId = this.InDemandOccupationId;
				trainingProgram.TrainingLevelId = this.TrainingLevelId;

				trainingProgram.MemberOfUnderRepresentedGroup = this.MemberOfUnderRepresentedGroup.Value;

				// Only add/remove the specified under represented groups.
				if (this.SelectedUnderRepresentedGroupIds != null && this.SelectedUnderRepresentedGroupIds.Any() && this.MemberOfUnderRepresentedGroup.Value)
				{
					var thisIds = this.SelectedUnderRepresentedGroupIds.ToArray();
					var currentIds = trainingProgram.UnderRepresentedGroups.Select(dm => dm.Id).ToArray();
					var removeIds = currentIds.Except(thisIds);
					var addIds = thisIds.Except(currentIds).Except(removeIds);
					foreach (var removeId in removeIds)
					{
						var underRepresentedGroup = _staticDataService.GetUnderRepresentedGroup(removeId);
						trainingProgram.UnderRepresentedGroups.Remove(underRepresentedGroup);
					}

					foreach (var addId in addIds)
					{
						var underRepresentedGroup = _staticDataService.GetUnderRepresentedGroup(addId);
						trainingProgram.UnderRepresentedGroups.Add(underRepresentedGroup);
					}
				}
				else
				{
					// Remove all under represented groups.
					trainingProgram.UnderRepresentedGroups.Clear();
				}
			}
			else
			{
				trainingProgram.InDemandOccupationId = null;
				trainingProgram.TrainingLevelId = null;
				trainingProgram.MemberOfUnderRepresentedGroup = null;
				trainingProgram.UnderRepresentedGroups.Clear();
			}

			grantApplication.MarkWithdrawnAndReturnedApplicationAsIncomplete();

			// If the delivery dates fall outside of the valid dates, make the delivery dates equal to the earliest valid dates.
			if (!grantApplication.HasValidStartDate())
				grantApplication.StartDate = grantApplication.EarliestValidStartDate().ToUtcMorning();

			if (!grantApplication.HasValidEndDate())
				grantApplication.EndDate = grantApplication.StartDate < trainingProgram.EndDate ? trainingProgram.EndDate : grantApplication.StartDate;

			if (create)
				_trainingProgramService.Add(trainingProgram);
			else
				_trainingProgramService.Update(trainingProgram);

			this.RowVersion = Convert.ToBase64String(trainingProgram.RowVersion);

			return trainingProgram;
		}
		#endregion
	}
}

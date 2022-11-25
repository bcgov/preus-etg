using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Validation;
using CJG.Web.External.Models.Shared;
using CJG.Web.External.Models.Shared.TrainingProviders;
using Constants = CJG.Core.Entities.Constants;

namespace CJG.Web.External.Areas.Ext.Models.TrainingPrograms
{
    public class TrainingProgramViewModel : BaseViewModel
	{
		public string RowVersion { get; set; }
		public int GrantApplicationId { get; set; }
		public string GrantApplicationRowVersion { get; set; }

		public DateTime IntakePeriodStartDate { get; set; }
		public DateTime IntakePeriodEndDate { get; set; }
		public DateTime IntakePeriodMaxEndDate { get; set; }

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

		[Required(ErrorMessage = "You must enter a course, program or certificate name."), MaxLength(500)]
		public string CourseTitle { get; set; }

		[Required(ErrorMessage = "Total training hours is required.")]
		[ValidateNullableInt(ErrorMessage = "Please enter total training hours to the nearest hour.")]
		public int? TotalTrainingHours { get; set; }

		[Required(ErrorMessage = "If you have expected qualifications you must include the title of the qualification."), MaxLength(500)]
		public string TitleOfQualification { get; set; }

		[RequiredEnumerable(ErrorMessage = "You must select at least one primary delivery method.")]
		public int[] SelectedDeliveryMethodIds { get; set; }

		[Required(ErrorMessage = "You must select an expected qualification.")]
		public int? ExpectedQualificationId { get; set; }

		[Required(ErrorMessage = "You must select a skill level.")]
		public int? SkillLevelId { get; set; }

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

		//[Required(ErrorMessage = "You must select whether you are a member of an underrepresented group.")]  //Removed for CJG-520. This prevent completion of ETG applications when it is required.
		public bool? MemberOfUnderRepresentedGroup { get; set; }

		[RequiredEnumerable(ErrorMessage = "You must select at least one underrepresented group.")]
		public int[] SelectedUnderRepresentedGroupIds { get; set; }

		[AllowHtml]
		[Required(ErrorMessage = "Business training relevance is required")]
		[MaxLength(150000, ErrorMessage = "Business training relevance cannot exceed 150000 characters.")]
		public string BusinessTrainingRelevance { get; set; }

		[AllowHtml]
		[Required(ErrorMessage = "Participant training relevance is required")]
		[MaxLength(150000, ErrorMessage = "Participant training relevance cannot exceed 150000 characters.")]
		public string ParticipantTrainingRelevance { get; set; }

		[CustomValidation(typeof(CourseLinkValidation), "ValidateCourseLink")]
		public string CourseLink { get; set; }

		public TrainingProgramAttachmentViewModel CourseOutlineDocument { get; set; } = new TrainingProgramAttachmentViewModel();

		public TrainingProgramViewModel()
		{
		}

		public TrainingProgramViewModel(TrainingProgram trainingProgram)
		{
			if (trainingProgram == null) throw new ArgumentNullException(nameof(trainingProgram));
			if (trainingProgram.GrantApplication == null) throw new ArgumentNullException($"{nameof(trainingProgram)}.{nameof(trainingProgram.GrantApplication)}");

			Id = trainingProgram.Id;
			RowVersion = trainingProgram.RowVersion == null ? null : Convert.ToBase64String(trainingProgram.RowVersion);

			GrantApplicationId = trainingProgram.GrantApplicationId;
			GrantApplicationRowVersion = Convert.ToBase64String(trainingProgram.GrantApplication.RowVersion);

			InDemandOccupationId = trainingProgram.InDemandOccupationId == 0 ? null : trainingProgram.InDemandOccupationId;
			SkillLevelId = trainingProgram.SkillLevelId == 0 ? null : (int?)trainingProgram.SkillLevelId;
			SkillFocusId = trainingProgram.SkillFocusId == 0 ? null : trainingProgram.SkillFocusId;
			ExpectedQualificationId = trainingProgram.ExpectedQualificationId == 0 ? null : (int?)trainingProgram.ExpectedQualificationId;
			TrainingLevelId = trainingProgram.TrainingLevelId == 0 ? null : trainingProgram.TrainingLevelId;
			CourseTitle = trainingProgram.CourseTitle;
			TrainingBusinessCase = trainingProgram.TrainingBusinessCase;
			TotalTrainingHours = trainingProgram.TotalTrainingHours == 0 ? null : (int?)trainingProgram.TotalTrainingHours;
			TitleOfQualification = trainingProgram.TitleOfQualification;
			HasOfferedThisTypeOfTrainingBefore = trainingProgram.Id == 0 ? null : (bool?)trainingProgram.HasOfferedThisTypeOfTrainingBefore;
			HasRequestedAdditionalFunding = trainingProgram.Id == 0 ? null : (bool?)trainingProgram.HasRequestedAdditionalFunding;
			DescriptionOfFundingRequested = trainingProgram.DescriptionOfFundingRequested;
			MemberOfUnderRepresentedGroup = trainingProgram.MemberOfUnderRepresentedGroup;
			CourseLink = string.IsNullOrEmpty(trainingProgram.CourseLink) ? null :
				trainingProgram.CourseLink.Contains("http://") || trainingProgram.CourseLink.Contains("https://") ? trainingProgram.CourseLink : "http://" + trainingProgram.CourseLink;

			// Always default to the Grant Application Start and End dates. They need to be in Sync for ETG.
            StartDate = trainingProgram.GrantApplication.StartDate.ToLocalTime();
            StartYear = trainingProgram.GrantApplication.StartDate.ToLocalTime().Year;
            StartMonth = trainingProgram.GrantApplication.StartDate.ToLocalTime().Month;
            StartDay = trainingProgram.GrantApplication.StartDate.ToLocalTime().Day;
            EndDate = trainingProgram.GrantApplication.EndDate.ToLocalTime();
            EndYear = trainingProgram.GrantApplication.EndDate.ToLocalTime().Year;
            EndMonth = trainingProgram.GrantApplication.EndDate.ToLocalTime().Month;
            EndDay = trainingProgram.GrantApplication.EndDate.ToLocalTime().Day;

			SelectedDeliveryMethodIds = trainingProgram.DeliveryMethods.Select(dm => dm.Id).ToArray();
			SelectedUnderRepresentedGroupIds = trainingProgram.UnderRepresentedGroups.Select(dm => dm.Id).ToArray();
			GrantApplicationId = trainingProgram.GrantApplicationId;

			IntakePeriodStartDate = trainingProgram.GrantApplication.GrantOpening.TrainingPeriod.StartDate.ToLocalTime();
			IntakePeriodEndDate = trainingProgram.GrantApplication.GrantOpening.TrainingPeriod.EndDate.ToLocalTime();
			IntakePeriodMaxEndDate = trainingProgram.GrantApplication.GrantOpening.TrainingPeriod.EndDate.AddYears(1).ToLocalTime();

			DeliveryStartDate = trainingProgram.GrantApplication.StartDate.ToLocalTime();
			DeliveryEndDate = trainingProgram.GrantApplication.EndDate.ToLocalTime();

			BusinessTrainingRelevance = trainingProgram.BusinessTrainingRelevance;
			ParticipantTrainingRelevance = trainingProgram.ParticipantTrainingRelevance;

			CourseOutlineDocument = new TrainingProgramAttachmentViewModel(trainingProgram.CourseOutlineDocument, Id, RowVersion);
		}

		/// <summary>
		/// Add/Update the specified training program in the datasource.
		/// </summary>
		/// <param name="grantApplicationService"></param>
		/// <param name="trainingProgramService"></param>
		/// <param name="trainingProviderService"></param>
		/// <param name="staticDataService"></param>
		/// <param name="attachmentService"></param>
		/// <param name="user"></param>
		/// <param name="files"></param>
		/// <returns></returns>
		public TrainingProgram UpdateTrainingProgram(IGrantApplicationService grantApplicationService,
													 ITrainingProgramService trainingProgramService,
													 ITrainingProviderService trainingProviderService,
													 IStaticDataService staticDataService,
													 IAttachmentService attachmentService,
													 IPrincipal user,
													 HttpPostedFileBase[] files = null)
		{
			if (grantApplicationService == null) throw new ArgumentNullException(nameof(grantApplicationService));
			if (trainingProgramService == null) throw new ArgumentNullException(nameof(trainingProgramService));
			if (trainingProviderService == null) throw new ArgumentNullException(nameof(trainingProviderService));
			if (staticDataService == null) throw new ArgumentNullException(nameof(staticDataService));
			if (user == null) throw new ArgumentNullException(nameof(user));

			var grantApplication = grantApplicationService.Get(GrantApplicationId);
			var create = Id == 0;

			var trainingProgram = !create ? trainingProgramService.Get(Id) : new TrainingProgram(grantApplication);
			if (!create)
			{
				trainingProgram.RowVersion = Convert.FromBase64String(RowVersion);
			}

			trainingProgram.TrainingProgramState = TrainingProgramStates.Complete;

			trainingProgram.StartDate = ((DateTime)StartDate).ToLocalMorning().ToUtcMorning();
			trainingProgram.EndDate = ((DateTime)EndDate).ToLocalMidnight().ToUtcMidnight();
			trainingProgram.CourseTitle = CourseTitle;
			trainingProgram.CourseLink = !string.IsNullOrWhiteSpace(CourseLink) ? CourseLink.Contains("http://") || CourseLink.Contains("https://") ? CourseLink : "http://" + CourseLink : null;

			// Only add/remove the specified delivery methods.
			if (SelectedDeliveryMethodIds != null && SelectedDeliveryMethodIds.Any())
			{
				var thisIds = SelectedDeliveryMethodIds.ToArray();
				var currentIds = trainingProgram.DeliveryMethods.Select(dm => dm.Id).ToArray();
				var removeIds = currentIds.Except(thisIds);
				var addIds = thisIds.Except(currentIds).Except(removeIds);

				foreach (var removeId in removeIds)
				{
					var deliveryMethod = staticDataService.GetDeliveryMethod(removeId);
					trainingProgram.DeliveryMethods.Remove(deliveryMethod);
				}

				foreach (var addId in addIds)
				{
					var deliveryMethod = staticDataService.GetDeliveryMethod(addId);
					trainingProgram.DeliveryMethods.Add(deliveryMethod);
				}
				if (SelectedDeliveryMethodIds.Contains(Constants.Delivery_Classroom) || SelectedDeliveryMethodIds.Contains(Constants.Delivery_Workplace))
                {
					if (trainingProgram.TrainingProvider != null)
						if (trainingProgram.TrainingProvider.TrainingAddress == null)
							trainingProgram.TrainingProvider.TrainingProviderState = TrainingProviderStates.Incomplete;
                }

				else if (!(SelectedDeliveryMethodIds.Contains(Constants.Delivery_Classroom) || SelectedDeliveryMethodIds.Contains(Constants.Delivery_Workplace)))
				{
					if (trainingProgram.TrainingProvider?.TrainingAddress != null)
						trainingProgram.TrainingProvider.TrainingAddress = null;
				}
			}
			else
			{
				// Remove all the delivery methods.
				trainingProgram.DeliveryMethods.Clear();
			}

			trainingProgram.SkillFocusId = SkillFocusId.Value;
			trainingProgram.SkillLevelId = SkillLevelId.Value;
			trainingProgram.TotalTrainingHours = TotalTrainingHours.Value;
			trainingProgram.HasOfferedThisTypeOfTrainingBefore = HasOfferedThisTypeOfTrainingBefore.Value;
			trainingProgram.HasRequestedAdditionalFunding = HasRequestedAdditionalFunding.Value;
			trainingProgram.BusinessTrainingRelevance = BusinessTrainingRelevance;
			trainingProgram.ParticipantTrainingRelevance = ParticipantTrainingRelevance;

			trainingProgram.DescriptionOfFundingRequested = HasRequestedAdditionalFunding.Value ? DescriptionOfFundingRequested : null;

			trainingProgram.ExpectedQualificationId = ExpectedQualificationId.Value;
			trainingProgram.TitleOfQualification = new[] { 5 }.Contains(ExpectedQualificationId.GetValueOrDefault()) ? null : TitleOfQualification;

			if (new[] { 5 }.Contains(SkillFocusId.GetValueOrDefault()))
			{
				trainingProgram.InDemandOccupationId = InDemandOccupationId;
				trainingProgram.TrainingLevelId = TrainingLevelId;

				//trainingProgram.MemberOfUnderRepresentedGroup = this.MemberOfUnderRepresentedGroup.Value;  // CJG-520. Should not call .Value on a nullable type
				trainingProgram.MemberOfUnderRepresentedGroup = MemberOfUnderRepresentedGroup;

				// Only add/remove the specified under represented groups.
				//if (this.SelectedUnderRepresentedGroupIds != null && this.SelectedUnderRepresentedGroupIds.Any() && this.MemberOfUnderRepresentedGroup.Value) // Should not call .Value on a nullable type
				if (SelectedUnderRepresentedGroupIds != null && SelectedUnderRepresentedGroupIds.Any() && (MemberOfUnderRepresentedGroup ?? false))
				{
					var thisIds = SelectedUnderRepresentedGroupIds.ToArray();
					var currentIds = trainingProgram.UnderRepresentedGroups.Select(dm => dm.Id).ToArray();
					var removeIds = currentIds.Except(thisIds);
					var addIds = thisIds.Except(currentIds).Except(removeIds);
					foreach (var removeId in removeIds)
					{
						var underRepresentedGroup = staticDataService.GetUnderRepresentedGroup(removeId);
						trainingProgram.UnderRepresentedGroups.Remove(underRepresentedGroup);
					}

					foreach (var addId in addIds)
					{
						var underRepresentedGroup = staticDataService.GetUnderRepresentedGroup(addId);
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

			if (grantApplication.MarkWithdrawnAndReturnedApplicationAsIncomplete())
			{
				grantApplication.RowVersion = Convert.FromBase64String(GrantApplicationRowVersion);
			}

			// If the delivery dates fall outside of the valid dates, make the delivery dates equal to the earliest valid dates.
			if (!grantApplication.HasValidStartDate())
			{
				grantApplication.RowVersion = Convert.FromBase64String(GrantApplicationRowVersion);
				grantApplication.StartDate = grantApplication.EarliestValidStartDate().ToUtcMorning();
			}

			if (!grantApplication.HasValidEndDate())
			{
				grantApplication.RowVersion = Convert.FromBase64String(GrantApplicationRowVersion);
				grantApplication.EndDate = grantApplication.StartDate < trainingProgram.EndDate ? trainingProgram.EndDate : grantApplication.StartDate;
			}

			UpdateAttachments(trainingProgram, attachmentService, files);

			if (create)
				trainingProgramService.Add(trainingProgram);
			else
				trainingProgramService.Update(trainingProgram);

			return trainingProgram;
		}


		/// <summary>
		/// Add/update/remove attachments associated with the specific properties of the training provider.
		/// </summary>
		/// <param name="trainingProgram"></param>
		/// <param name="attachmentService"></param>
		/// <param name="files"></param>
		private void UpdateAttachments(TrainingProgram trainingProgram, IAttachmentService attachmentService, HttpPostedFileBase[] files = null)
		{
			if (trainingProgram == null)
				throw new ArgumentNullException(nameof(trainingProgram));

			if (attachmentService == null)
				throw new ArgumentNullException(nameof(attachmentService));

			// If files were provided add/update the attachments for the specified properties.
			if (files == null || !files.Any())
				return;

			if (CourseOutlineDocument?.Index != null && files.Count() > CourseOutlineDocument.Index)
			{
				var attachment = files[CourseOutlineDocument.Index.Value].UploadFile(CourseOutlineDocument.Description, CourseOutlineDocument.FileName);
				attachment.Id = CourseOutlineDocument.Id;

				if (CourseOutlineDocument.Id == 0)
				{
					trainingProgram.CourseOutlineDocument = attachment;
					attachmentService.Add(attachment);
					CourseOutlineDocument.Id = attachment.Id;
				}
				else
				{
					attachment.RowVersion = Convert.FromBase64String(CourseOutlineDocument.RowVersion);
					attachmentService.Update(attachment);
				}
			}

			if (trainingProgram.CourseOutlineDocumentId.HasValue && trainingProgram.CourseOutlineDocumentId != CourseOutlineDocument?.Id)
			{
				// Remove the prior attachment because it has been replaced.
				var attachment = attachmentService.Get(trainingProgram.CourseOutlineDocumentId.Value);
				trainingProgram.CourseOutlineDocumentId = null;
				attachmentService.Remove(attachment);
			}

			trainingProgram.CourseOutlineDocumentId = trainingProgram.CourseOutlineDocument?.Id;
		}
	}
}

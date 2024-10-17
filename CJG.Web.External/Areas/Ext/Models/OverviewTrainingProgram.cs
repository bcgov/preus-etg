using System;
using System.Collections.Generic;
using System.Linq;
using CJG.Application.Services;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class OverviewTrainingProgram
	{
		public int Id { get; set; }
		public TrainingProgramStates TrainingProgramState { get; set; }
		public string CourseTitle { get; set; }
		public IEnumerable<DeliveryMethod> DeliveryMethods { get; set; }
		public int TotalTrainingHours { get; set; }
		public SkillsFocus SkillFocus { get; set; }
		public SkillLevel SkillLevel { get; set; }
		public string EligibleExpenseBreakdown { get; set; }
		public string ServiceLineBreakdown { get; set; }
		public bool ShowSkillFocusFields => SkillFocus != null && (SkillFocus.Id == 5 || SkillFocus.Id == 6);
		public InDemandOccupation InDemandOccupation { get; set; }
		public TrainingLevel TrainingLevel { get; set; }
		public bool? MemberOfUnderRepresentedGroup { get; set; }
		public IEnumerable<UnderRepresentedGroup> UnderRepresentedGroups { get; set; }
		public ExpectedQualification ExpectedQualification { get; set; }
		public string TitleOfQualification { get; set; }
		public bool HasOfferedThisTypeOfTrainingBefore { get; set; }
		public bool HasRequestedAdditionalFunding { get; set; }
		public string DescriptionOfFundingRequested { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public OverviewTrainingProvider AssociatedProvider { get; set; }
		public decimal? TotalCost { get; set; }
		public string RowVersion { get; set; }
		public string CourseLink { get; set; }
		public Attachment CourseOutlineDocument { get; set; }
		public string BusinessTrainingRelevance { get; set; }
		public string ParticipantTrainingRelevance { get; set; }

		public OverviewTrainingProgram()
		{

		}
		public OverviewTrainingProgram(TrainingProgram trainingProgram)
		{
			Utilities.MapProperties(trainingProgram, this);
			TrainingProgramState = trainingProgram.TrainingProgramState;
			DeliveryMethods = trainingProgram.DeliveryMethods;
			SkillLevel = trainingProgram.SkillLevel;
			SkillFocus = trainingProgram.SkillFocus;
			EligibleExpenseBreakdown = trainingProgram.EligibleCostBreakdown?.EligibleExpenseBreakdown.Caption;
			ServiceLineBreakdown = trainingProgram.ServiceLineBreakdown?.Caption;
			InDemandOccupation = trainingProgram.InDemandOccupation;
			TrainingLevel = trainingProgram.TrainingLevel;
			UnderRepresentedGroups = trainingProgram.UnderRepresentedGroups;
			ExpectedQualification = trainingProgram.ExpectedQualification;
			StartDate = trainingProgram.StartDate.ToLocalTime();
			EndDate = trainingProgram.EndDate.ToLocalTime();
			CourseLink = trainingProgram.CourseLink;
			CourseOutlineDocument = trainingProgram.CourseOutlineDocument;

			TotalCost = trainingProgram.EligibleCostBreakdown?.EstimatedCost;
			if (trainingProgram.TrainingProviders.Any())
			{
				AssociatedProvider = new OverviewTrainingProvider(trainingProgram.TrainingProviders.First());
			}
		}
	}
}
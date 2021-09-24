using CJG.Application.Services;
using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class OverviewTrainingProgram
	{
		#region Properties
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
		#endregion

		#region Constructors
		public OverviewTrainingProgram()
		{

		}
		public OverviewTrainingProgram(TrainingProgram trainingProgram)
		{
			Utilities.MapProperties(trainingProgram, this);
			this.TrainingProgramState = trainingProgram.TrainingProgramState;
			this.DeliveryMethods = trainingProgram.DeliveryMethods;
			this.SkillLevel = trainingProgram.SkillLevel;
			this.SkillFocus = trainingProgram.SkillFocus;
			this.EligibleExpenseBreakdown = trainingProgram.EligibleCostBreakdown?.EligibleExpenseBreakdown.Caption;
			this.ServiceLineBreakdown = trainingProgram.ServiceLineBreakdown?.Caption;
			this.InDemandOccupation = trainingProgram.InDemandOccupation;
			this.TrainingLevel = trainingProgram.TrainingLevel;
			this.UnderRepresentedGroups = trainingProgram.UnderRepresentedGroups;
			this.ExpectedQualification = trainingProgram.ExpectedQualification;
			this.StartDate = trainingProgram.StartDate.ToLocalTime();
			this.EndDate = trainingProgram.EndDate.ToLocalTime();
			this.CourseLink = trainingProgram.CourseLink;

			this.TotalCost = trainingProgram.EligibleCostBreakdown?.EstimatedCost;
			if (trainingProgram.TrainingProviders.Any())
			{
				this.AssociatedProvider = new OverviewTrainingProvider(trainingProgram.TrainingProviders.First());
			}
		}
		#endregion
	}
}
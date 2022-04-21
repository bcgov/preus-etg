using CJG.Core.Entities;
using System;

namespace CJG.Web.External.Areas.Int.Models
{
	public class ParticipantTrainingHistory
	{
		#region Properties
		public int GrantId { get; set; }
		public string FileNumber { get; set; }
		public string TrainingStartDate { get; set; }
		public string TrainingEndDate { get; set; }
		public string TrainingStream { get; set; }
		public string ApplicationStatus { get; set; }
		public string TrainingProvider { get; set; }
		public string TrainingCourse { get; set; }
		public string ApprovedGovtContribution { get; set; }
		public string AmountPaid { get; set; }
		#endregion

		//ParticipantForm participant
		public ParticipantTrainingHistory(TrainingProgram trainingProgram, decimal govContri, decimal paid)
		{
			this.GrantId = trainingProgram.GrantApplication.Id;
			this.FileNumber = trainingProgram.GrantApplication.FileNumber ?? "";

			this.TrainingStartDate = trainingProgram.StartDate.ToString("yyyy-MM-dd");
			this.TrainingEndDate = trainingProgram.EndDate.ToString("yyyy-MM-dd");
			this.TrainingStream = trainingProgram.GrantApplication.GrantOpening.GrantStream.Name;
			this.ApplicationStatus = trainingProgram.GrantApplication.ApplicationStateInternal.GetDescription();

			this.TrainingProvider = trainingProgram.TrainingProvider.Name;
			this.TrainingCourse = trainingProgram.CourseTitle;

			this.ApprovedGovtContribution = govContri.ToString("c");
			this.AmountPaid = paid.ToString("c");
		}
	}
}
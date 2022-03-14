using CJG.Core.Entities;
using System;

namespace CJG.Web.External.Areas.Int.Models
{
	public class ParticipantTrainingHistory
	{
		#region Properties
		public int GrantId { get; set; }
		public string FileNo { get; set; }
		public DateTime TrainingStartDate { get; set; }
		public DateTime TrainingEndDate { get; set; }
		public string TrainingStream { get; set; }
		public string ApplicationStatus { get; set; }
		public string TrainingProvider { get; set; }
		public string TrainingCourse { get; set; }
		public decimal ApprovedGovtContribution { get; set; }
		public decimal AmountPaid { get; set; }
		#endregion

		//ParticipantForm participant
		public ParticipantTrainingHistory(TrainingProgram trainingProgram, decimal govContri, decimal paid)
		{
			this.GrantId = trainingProgram.GrantApplication.Id;
			this.FileNo = trainingProgram.GrantApplication.FileNumber ?? "";

			this.TrainingStartDate = trainingProgram.StartDate;
			this.TrainingEndDate = trainingProgram.EndDate;
			this.TrainingStream = trainingProgram.GrantApplication.GrantOpening.GrantStream.Name;
			this.ApplicationStatus = trainingProgram.GrantApplication.ApplicationStateInternal.GetDescription();

			this.TrainingProvider = trainingProgram.TrainingProvider.Name;
			this.TrainingCourse = trainingProgram.CourseTitle;

			this.ApprovedGovtContribution = govContri;
			this.AmountPaid = paid;
		}
	}
}
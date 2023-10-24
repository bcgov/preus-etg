using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Int.Models
{
	public class ParticipantTrainingHistory
	{
		public int ParticipantFormId { get; set; }
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

		public ParticipantTrainingHistory(TrainingProgram trainingProgram, decimal governmentContribution, decimal paid, ParticipantForm participantForm)
		{
			GrantId = trainingProgram.GrantApplication.Id;
			ParticipantFormId = participantForm.Id;
			FileNumber = trainingProgram.GrantApplication.FileNumber ?? "";

			TrainingStartDate = trainingProgram.StartDate.ToString("yyyy-MM-dd");
			TrainingEndDate = trainingProgram.EndDate.ToString("yyyy-MM-dd");
			TrainingStream = trainingProgram.GrantApplication.GrantOpening.GrantStream.Name;
			ApplicationStatus = trainingProgram.GrantApplication.ApplicationStateInternal.GetDescription();

			TrainingProvider = trainingProgram.TrainingProvider.Name;
			TrainingCourse = trainingProgram.CourseTitle;

			ApprovedGovtContribution = governmentContribution.ToString("c");
			AmountPaid = paid.ToString("c");
		}
	}
}
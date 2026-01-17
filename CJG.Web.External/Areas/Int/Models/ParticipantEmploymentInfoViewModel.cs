using System.Linq;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Helpers;

namespace CJG.Web.External.Areas.Int.Models
{
	public class ParticipantEmploymentInfoViewModel
	{
		public string EmploymentStatus { get; set; }
		public string MultipleEmploymentPositions { get; set; }
		public string CityofWork { get; set; }
		public string ReceivingEIValue { get; set; }

		public string JobTitleBeforeTraining { get; set; }
		public string JobTitleAfterTraining { get; set; }

		public string CurrentNocLevel5 { get; set; }
		public string FutureNocLevel5 { get; set; }
		public string PreviousEmploymentNocLevel5 { get; set; }

		public string ReceivingIA { get; set; }
		public string Apprentice { get; set; }
		public string EmployedByEmployer { get; set; }
		public string ITARegistered { get; set; }
		public string DurationOfEmployment { get; set; }
		public string ParticipatingInOtherFundingProg { get; set; }
		public string OwnerOfBusiness { get; set; }
		public string ProgramDescription { get; set; }
		public string OtherProgramDesc { get; set; }
		public int? HoursPerWeek { get; set; }
		public int? PreviousHoursPerWeek { get; set; }
		public string PreviousEmployerFullName { get; set; }

		public string MostImportantResult { get; set; }
		public string TypeOfEmployment { get; set; }
		public string AverageHourlyWage { get; set; }
		public string PreviousHourlyWage { get; set; }
		public string MaternalParentalBenefits { get; set; }
		public bool ShowEmploymentFields { get; set; }

		public ParticipantEmploymentInfoViewModel()
		{
		}

		public ParticipantEmploymentInfoViewModel(ParticipantForm participantForm, INationalOccupationalClassificationService nationalOccupationalClassificationService)
		{
			EmploymentStatus = participantForm.EmploymentStatus?.Caption;
			MultipleEmploymentPositions = participantForm.MultipleEmploymentPositions.AsYesOrNo();
				
			CityofWork = participantForm.PrimaryCity;
			ReceivingEIValue = participantForm.EIBenefit?.Caption;
			if (participantForm?.GrantApplication?.GrantOpening?.GrantStream?.GrantProgram?.ProgramTypeId == ProgramTypes.WDAService)
				ReceivingEIValue = participantForm.ReceivingEIBenefit.ToStringValue();

			if (participantForm.CurrentNoc != null && participantForm.CurrentNoc.Id != 0 && !string.IsNullOrWhiteSpace(participantForm.CurrentNoc.Code))
				CurrentNocLevel5 = participantForm.CurrentNoc.ToString();

			if (participantForm.FutureNoc != null && participantForm.FutureNoc.Id != 0 && !string.IsNullOrWhiteSpace(participantForm.FutureNoc.Code))
				FutureNocLevel5 = participantForm.FutureNoc.ToString();

			if (participantForm.PreviousEmploymentNoc != null && participantForm.PreviousEmploymentNoc.Id != 0 && !string.IsNullOrWhiteSpace(participantForm.PreviousEmploymentNoc.Code))
				PreviousEmploymentNocLevel5 = participantForm.PreviousEmploymentNoc.ToString();

			JobTitleBeforeTraining = participantForm.JobTitleBefore;
			JobTitleAfterTraining = participantForm.JobTitleFuture;

			ReceivingIA = participantForm.BceaClient ? "Yes" : "No";
			Apprentice = participantForm.Apprentice ? "Yes" : "No";
			EmployedByEmployer = participantForm.EmployedBySupportEmployer ? "Yes" : "No";
			ITARegistered = participantForm.ItaRegistered ? "Yes" : "No";
			DurationOfEmployment = participantForm.HowLongYears.HasValue || participantForm.HowLongMonths.HasValue
				? $"{participantForm.HowLongYears ?? 0} Years {participantForm.HowLongMonths ?? 0} Months"
				:null;
			ParticipatingInOtherFundingProg = participantForm.OtherPrograms ? "Yes" :  "No";
			OwnerOfBusiness = participantForm.BusinessOwner ? "Yes" : "No";
			ProgramDescription = participantForm.ProgramDescription;
			OtherProgramDesc = participantForm.OtherProgramDesc;
			HoursPerWeek = participantForm.AvgHoursPerWeek;
			PreviousHoursPerWeek = participantForm.PreviousAvgHoursPerWeek;
			PreviousEmployerFullName = participantForm.PreviousEmployerFullName;
			MostImportantResult = participantForm.TrainingResult?.Caption;
			TypeOfEmployment = participantForm.EmploymentType?.Caption;
			AverageHourlyWage = string.Format("{0:c}", participantForm.HourlyWage);
			PreviousHourlyWage = participantForm.PreviousHourlyWage.HasValue
				? string.Format("{0:c}", participantForm.PreviousHourlyWage)
				: string.Empty;
			MaternalParentalBenefits = participantForm.MaternalPaternal ? "Yes" : "No";
			ShowEmploymentFields = new[] { "Employed", "Self-employed" }.Contains(EmploymentStatus);
		}
	}
}
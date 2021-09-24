using System.Linq;
using CJG.Core.Entities;
using CJG.Web.External.Helpers;

namespace CJG.Web.External.Areas.Int.Models
{
	public class ParticipantEmploymentInfoViewModel
	{
		#region Properties
		public string EmploymentStatus { get; set; }
		public string CityofWork { get; set; }
		public string ReceivingEIValue { get; set; }

		#region NOC
		public string CurrentNocLevel4 { get; set; }
		public string FutureNocLevel4 { get; set; }
		#endregion

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
		public string MostImportantResult { get; set; }
		public string TypeOfEmployment { get; set; }
		public string AverageHourlyWage { get; set; }
		public string MaternalParentalBenefits { get; set; }
		public bool ShowEmploymentFields { get; set; }
		#endregion

		#region Constructors
		public ParticipantEmploymentInfoViewModel()
		{

		}

		public ParticipantEmploymentInfoViewModel(ParticipantForm participantForm, CJG.Core.Interfaces.Service.INationalOccupationalClassificationService nationalOccupationalClassificationService)
		{
			this.EmploymentStatus = participantForm.EmploymentStatus?.Caption;
			this.CityofWork = participantForm.PrimaryCity;
			this.ReceivingEIValue = participantForm.EIBenefit?.Caption;
			if (participantForm?.GrantApplication?.GrantOpening?.GrantStream?.GrantProgram?.ProgramTypeId == ProgramTypes.WDAService)
				this.ReceivingEIValue = participantForm.ReceivingEIBenefit.ToStringValue();

			if (participantForm.CurrentNoc.Id != 0)
			{
				var nocIDs = nationalOccupationalClassificationService.GetNationalOccupationalClassificationIds(participantForm.CurrentNoc.Id);
				var level4CurrentNoc = nationalOccupationalClassificationService.GetNationalOccupationalClassification(nocIDs.Item4);
				CurrentNocLevel4 = !string.IsNullOrWhiteSpace(level4CurrentNoc?.Code) ? $"{level4CurrentNoc.Code + " | " + level4CurrentNoc.Description}" : null;
			}

			if (participantForm.FutureNoc.Id != 0)
			{
				var nocIDs = nationalOccupationalClassificationService.GetNationalOccupationalClassificationIds(participantForm.FutureNoc.Id);
				var level4FutureNoc = nationalOccupationalClassificationService.GetNationalOccupationalClassification(nocIDs.Item4);
				FutureNocLevel4 = !string.IsNullOrWhiteSpace(level4FutureNoc?.Code) ? $"{level4FutureNoc.Code + " | " + level4FutureNoc.Description}" : null;
			}

			this.ReceivingIA = participantForm.BceaClient ? "Yes" : "No";
			this.Apprentice = participantForm.Apprentice ? "Yes" : "No";
			this.EmployedByEmployer = participantForm.EmployedBySupportEmployer ? "Yes" : "No";
			this.ITARegistered = participantForm.ItaRegistered ? "Yes" : "No";
			this.DurationOfEmployment = participantForm.HowLongYears.HasValue || participantForm.HowLongMonths.HasValue
				? $"{participantForm.HowLongYears ?? 0} Years {participantForm.HowLongMonths ?? 0} Months"
				:null;
			this.ParticipatingInOtherFundingProg = participantForm.OtherPrograms ? "Yes" :  "No";
			this.OwnerOfBusiness = participantForm.BusinessOwner ? "Yes" : "No";
			this.ProgramDescription = participantForm.ProgramDescription;
			this.OtherProgramDesc = participantForm.OtherProgramDesc;
			this.HoursPerWeek = participantForm.AvgHoursPerWeek;
			this.MostImportantResult = participantForm.TrainingResult?.Caption;
			this.TypeOfEmployment = participantForm.EmploymentType?.Caption;
			this.AverageHourlyWage = string.Format("{0:c}", participantForm.HourlyWage);
			this.MaternalParentalBenefits = participantForm.MaternalPaternal ? "Yes" : "No";
			this.ShowEmploymentFields = new[] { "Employed", "Self-employed" }.Contains(this.EmploymentStatus);
		}
		#endregion
	}
}
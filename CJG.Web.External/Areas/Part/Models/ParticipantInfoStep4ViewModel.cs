using CJG.Application.Business.Models;
using CJG.Core.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CJG.Web.External.Areas.Part.Models
{
	public class ParticipantInfoStep4ViewModel : StepCompletedViewModelBase
	{
		[Required(ErrorMessage = "The Employment Status field is required.")]
		[Range(1, 5, ErrorMessage = "The Employment Status field is required.")]
		public int EmploymentStatus { get; set; }
		public List<KeyValuePair<int, string>> EmploymentStatuses { get; set; } = new List<KeyValuePair<int, string>>();
		
		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidateEmploymentType")]
		public int? EmploymentType { get; set; }
		public List<KeyValuePair<int, string>> EmploymentTypes { get; set; } = new List<KeyValuePair<int, string>>();

		[Required(ErrorMessage = "The Training Result field is required.")]
		[Range(1, int.MaxValue, ErrorMessage = "The Training Result field is required.")]
		public int TrainingResult { get; set; }
		public List<KeyValuePair<int, string>> TrainingResults { get; set; } = new List<KeyValuePair<int, string>>();

		#region Current NOC
		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidateCurrentNoc")]
		public int? CurrentNoc1Id { get; set; }
		public List<KeyValuePair<int, string>> CurrentNoc1Codes { get; set; } = new List<KeyValuePair<int, string>>();

		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidateCurrentNoc")]
		public int? CurrentNoc2Id { get; set; }
		public List<KeyValueParent<int, string, int>> CurrentNoc2Codes { get; set; } = new List<KeyValueParent<int, string, int>>();

		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidateCurrentNoc")]
		public int? CurrentNoc3Id { get; set; }
		public List<KeyValueParent<int, string, int>> CurrentNoc3Codes { get; set; } = new List<KeyValueParent<int, string, int>>();

		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidateCurrentNoc")]
		public int? CurrentNoc4Id { get; set; }
		public List<KeyValueParent<int, string, int>> CurrentNoc4Codes { get; set; } = new List<KeyValueParent<int, string, int>>();

		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidateCurrentNoc")]
		public int? CurrentNoc5Id { get; set; }
		public List<KeyValueParent<int, string, int>> CurrentNoc5Codes { get; set; } = new List<KeyValueParent<int, string, int>>();
		#endregion

		#region Future NOC
		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidateFutureNoc")]
		public int? FutureNoc1Id { get; set; }
		public List<KeyValuePair<int, string>> FutureNoc1Codes { get; set; } = new List<KeyValuePair<int, string>>();

		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidateFutureNoc")]
		public int? FutureNoc2Id { get; set; }
		public List<KeyValueParent<int, string, int>> FutureNoc2Codes { get; set; } = new List<KeyValueParent<int, string, int>>();

		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidateFutureNoc")]
		public int? FutureNoc3Id { get; set; }
		public List<KeyValueParent<int, string, int>> FutureNoc3Codes { get; set; } = new List<KeyValueParent<int, string, int>>();

		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidateFutureNoc")]
		public int? FutureNoc4Id { get; set; }
		public List<KeyValueParent<int, string, int>> FutureNoc4Codes { get; set; } = new List<KeyValueParent<int, string, int>>();

		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidateFutureNoc")]
		public int? FutureNoc5Id { get; set; }
		public List<KeyValueParent<int, string, int>> FutureNoc5Codes { get; set; } = new List<KeyValueParent<int, string, int>>();
		#endregion

		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidateJobTitleBefore")]
		public string JobTitleBefore { get; set; }

		[Required(ErrorMessage = "Your Job Title after training is required.")]
		public string JobTitleFuture { get; set; }

		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidateCurrentReceiveEI")]
		public bool? CurrentReceiveEI { get; set; }

		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidatePastMaternalPaternal")]
		public bool? PastMaternalPaternal { get; set; }
		
		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidateEIBenefit")]
		public int EIBenefit { get; set; }

		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidateMaternalPaternal")]
		public bool? MaternalPaternal { get; set; }

		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidateBceaClient")]
		public bool? BceaClient { get; set; }

		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidateEmployedBy")]
		public bool? EmployedBySupportEmployer { get; set; }

		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidateHowLongYears"), Range(0, 50, ErrorMessage = "The number of years must be within 0 to 50.")]
		public int? HowLongYears { get; set; }

		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidateHowLongMonths"), Range(0, 12, ErrorMessage = "The number of months must be within 0 to 12.")]
		public int? HowLongMonths { get; set; }

		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidateBusinessOwner")]
		public bool? BusinessOwner { get; set; }

		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidateAvgHoursPerWeek"), Range(0, 168, ErrorMessage = "The average hours per week must be within 0 to 168.")]
		public int? AvgHoursPerWeek { get; set; }

		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidateHourlyWage"), Range(0, 99999, ErrorMessage = "The hourly rate must be within $0 to $99,999.")]
		public decimal? HourlyWage { get; set; }

		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidatePrimaryCity")]
		public string PrimaryCity { get; set; }

		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidateApprentice")]
		public bool? Apprentice { get; set; }

		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidateItaRegistered")]
		public bool? ItaRegistered { get; set; }

		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidateOtherPrograms")]
		public bool? OtherPrograms { get; set; }

		[CustomValidation(typeof(ParticipantInfoStep4VmValidation), "ValidateOtherProgramDesc")]
		public string OtherProgramDesc { get; set; }

		public ProgramTypes ProgramType { get; set; }
	}
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Part.Models
{
	public class ParticipantInfoStep3ViewModel : StepCompletedViewModelBase
	{
		[Range(1, 4, ErrorMessage = "Status field is required")]
		public int CanadianStatus { get; set; }
		public IEnumerable<KeyValuePair<int, string>> CanadianStatuses { get; set; }

		[CustomValidation(typeof(ParticipantInfoStep3VmValidation), "ValidateAboriginalBand")]
		public int? AboriginalBand { get; set; }
		public IEnumerable<KeyValuePair<int, string>> AboriginalBands { get; set; }

		public int? MartialStatusId { get; set; }
		public IEnumerable<KeyValuePair<int, string>> MartialStatuses { get; set; }

		public int? FederalOfficialLanguageId { get; set; }
		public IEnumerable<KeyValuePair<int, string>> FederalOfficialLanguages { get; set; }

		[Display(Name = "Education Level")]
		[Range(1, 6, ErrorMessage = "Education Level field is required")]
		public int EducationLevel { get; set; }
		public IEnumerable<KeyValuePair<int, string>> EducationLevels { get; set; }

		public int NumberOfDependents { get; set; }
		public int? Age { get; set; }
		public int? BirthYear { get; set; }

		[Required(ErrorMessage = "An answer is required")]
		public bool? CanadaImmigrant { get; set; }

		[CustomValidation(typeof(ParticipantInfoStep3VmValidation), "ValidateYearToCanada")]
		public int YearToCanada { get; set; }

		[Required(ErrorMessage = "An answer is required")]
		public bool? CanadaRefugee { get; set; }

		[StringLength(200, ErrorMessage = "The field must be a string with a maximum length of 200")]
		[RegularExpression("^[\\sA-Za-z]*$", ErrorMessage = "Country Invalid Format")]
		[CustomValidation(typeof(ParticipantInfoStep3VmValidation), "ValidateFromCountry")]
		public string FromCountry { get; set; }

		[Range(1, 3, ErrorMessage = "Gender field is required")]
		public int Gender { get; set; }

		[CustomValidation(typeof(ParticipantInfoStep3VmValidation), "ValidateYouthInCare")]
		public bool? YouthInCare { get; set; }

		[Range(1, 3, ErrorMessage = "Disability field is required.")]
		public int PersonDisability { get; set; }

		[Range(1, 3, ErrorMessage = "Aboriginal field is required.")]
		public int PersonAboriginal { get; set; }

		[CustomValidation(typeof(ParticipantInfoStep3VmValidation), "ValidateLiveOnReserve")]
		public bool? LiveOnReserve { get; set; }

		[Range(1, 3, ErrorMessage = "Minority field is required.")]
		public int VisibleMinority { get; set; }

		[CustomValidation(typeof(ParticipantInfoStep3VmValidation), "ValidateLastHighSchoolName")]
		[StringLength(250, ErrorMessage = "The field must be a string with a maximum length of 250")]
		[RegularExpression("^[\\sA-Za-z0-9\\-\\`\\'\\\"\\#\\(\\)]*$", ErrorMessage = "Last High School Name Invalid Format")]
		public string LastHighSchoolName { get; set; }

		[CustomValidation(typeof(ParticipantInfoStep3VmValidation), "ValidateLastHighSchoolCity")]
		[StringLength(250, ErrorMessage = "The field must be a string with a maximum length of 250")]
		[RegularExpression("^[\\sA-Za-z0-9\\-\\`\\'\\\"\\#\\(\\)]*$", ErrorMessage = "Last High School City Invalid Format")]
		public string LastHighSchoolCity { get; set; }

		public IEnumerable<KeyValuePair<string, string>> ImmigrationCountries { get; set; }
	}
}

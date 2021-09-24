using CJG.Application.Services;
using CJG.Core.Entities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Web.External.Areas.Part.Models
{
    public class ParticipantInfoStep2ViewModel : StepCompletedViewModelBase
    {
        [Required(ErrorMessage = "Province is required")]
        public string RegionId { get; set; }
        public List<KeyValuePair<string, string>> Provinces { get; set; }

        public string ProgramEmployerName { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(100, ErrorMessage = "The field must be a string with a maximum length of 100")]
        [NameValidation]
        public string FirstName { get; set; }

        [StringLength(100, ErrorMessage = "The field must be a string with a maximum length of 100")]
        [NameValidation]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(100, ErrorMessage = "The field must be a string with a maximum length of 100")]
        [NameValidation]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [CustomValidation(typeof(ParticipantInfoStep2VmValidation), "ValidateBirthDate")]
        public DateTime DateOfBirth { get; set; }

        public int? BirthDay { get; set; }
        public int? BirthMonth { get; set; }
        public int? BirthYear { get; set; }

        public int ParticipantOldestAge { get; set; }
        public int ParticipantYoungestAge { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Social Insurance Number is required")]
        [ConvertMap(nameof(SIN), new[] { nameof(SIN1), nameof(SIN2), nameof(SIN3) }, "{0}{1}{2}")]
        [RegularExpression("^[0-9]{9}$", ErrorMessage = "Social Insurance Number must be 9-digit number")]
        [CustomValidation(typeof(ParticipantInfoStep2VmValidation), "ValidateSIN")]
		[CustomValidation(typeof(ParticipantInfoStep2VmValidation), "IsDuplicateSIN")]
		
		public string SIN { get; set; }

        public string SIN1 { get; set; }
        public string SIN2 { get; set; }
        public string SIN3 { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Personal phone number is required")]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Personal phone number must be 10-digit number")]
        [ConvertMap(nameof(Phone1), new[] { nameof(Phone1AreaCode), nameof(Phone1Exchange), nameof(Phone1Number) }, "{0}{1}{2}")]
        public string Phone1 { get; set; }

        public string Phone1AreaCode { get; set; }
        public string Phone1Exchange { get; set; }
        public string Phone1Number { get; set; }
        public string Phone1Extension { get; set; }

        [NotMapped]
        [ConvertMap(nameof(Phone2), new[] { nameof(Phone2AreaCode), nameof(Phone2Exchange), nameof(Phone2Number) }, "{0}{1}{2}")]
        [CustomValidation(typeof(ParticipantInfoStep2VmValidation), "ValidatePhone2")]
        public string Phone2 { get; set; }

        public string Phone2AreaCode { get; set; }
        public string Phone2Exchange { get; set; }
        public string Phone2Number { get; set; }
        public string Phone2Extension { get; set; }

        [Required(ErrorMessage = "Email Address is required")]
        [StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
        [RegularExpression(@"^[\s*\d+\s*]*[a-zA-Z0-9_\.-]+@([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,6}[\s*\d+\s*]*$", ErrorMessage = "Provide a valid Email Address")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Address Line 1 is required")]
        [StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
        public string AddressLine1 { get; set; }

        [StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
        public string AddressLine2 { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
        public string City { get; set; }

        private string _postalCode;
        [Required(ErrorMessage = "Postal Code is required")]
        [RegularExpression(CJG.Core.Entities.Constants.PostalCodeValidationRegEx, ErrorMessage = "Postal code must contain 6  Alphanumeric characters")]
        public string PostalCode
        {
            get
            {
                return Utilities.FormatCanadianPostalCode(_postalCode);
            }
            set
            {
                _postalCode = value;
            }
        }

        public List<string> EnteredSINs { get; set; }
    }
}

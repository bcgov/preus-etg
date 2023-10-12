using System;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Int.Models
{
	public class ParticipantContactInfoViewModel
	{
		private const string DATEFORMAT = "yyyy-MM-dd";

		public string ReportedEmployerName { get; set; }
		public string ResidencyStatus { get; set; }
		public string FullName { get; set; }
		public string Immigrated { get; set; }
		public string DateOfBirth { get; set; }
		public int Age { get; set; }
		public string Refugee { get; set; }
		public string Gender { get; set; }
		public string YouthInCare { get; set; }
		public string SIN { get; set; }
		public string PersonWithDisability { get; set; }
		public string ParticipantPrimaryPhone { get; set; }
		public string AboriginalPerson { get; set; }
		public string AlternatePhone { get; set; }
		public string AboriginalGroup { get; set; }
		public string ParticipantEmail { get; set; }
		public string Living { get; set; }
		public string ResidentialAddressLine1 { get; set; }
		public string ResidentialAddressLine2 { get; set; }
		public string VisibleMinority { get; set; }
		public string City { get; set; }
		public string HighestLevelEducation { get; set; }
		public string PostalCode { get; set; }
		public string MaritalStatus { get; set; }
		public string FederalOfficialLanguage { get; set; }
		public int NumberOfDependents { get; set; }

		public ParticipantContactInfoViewModel(ParticipantForm participantForm)
		{
			ReportedEmployerName = participantForm.GrantApplication.OrganizationLegalName;
			ResidencyStatus = participantForm.CanadianStatus.Caption;
			FullName = participantForm.LastName + ", " + participantForm.FirstName + " " + participantForm.MiddleName;
			Immigrated = ImmigrantState(participantForm.CanadaImmigrant, participantForm.YearToCanada) ;
			DateOfBirth = participantForm.BirthDate.ToString(DATEFORMAT);
			Age = AppDateTime.UtcNow.Year - participantForm.BirthDate.Year;
			Refugee = RefugeeState(participantForm.CanadaRefugee, participantForm.FromCountry);
			Gender = GenderState(participantForm.Gender);
			YouthInCare = participantForm.YouthInCare ? "Yes" : "No";
			SIN = participantForm.SIN;
			PersonWithDisability = TriState(participantForm.PersonDisability, null);
			ParticipantPrimaryPhone = participantForm.PhoneNumber1 + (string.IsNullOrWhiteSpace(participantForm.PhoneExtension1) ? "" : " Ext " + participantForm.PhoneExtension1);
			AboriginalPerson = TriState(participantForm.PersonAboriginal, null);
			AlternatePhone = !string.IsNullOrWhiteSpace(participantForm.PhoneNumber2) ? (participantForm.PhoneNumber2 + (string.IsNullOrWhiteSpace(participantForm.PhoneExtension2) ? "" : " Ext " + participantForm.PhoneExtension2)) : "";
			AboriginalGroup = participantForm.PersonAboriginal == 1 ? participantForm?.AboriginalBand?.Caption : null;
			ParticipantEmail = participantForm.EmailAddress;
			Living = participantForm.PersonAboriginal == 1 && participantForm.AboriginalBand?.Id == 1 
				? (participantForm.LiveOnReserve ? "On" : "Off") + " Reserve"
				:null;
			ResidentialAddressLine1 = participantForm.AddressLine1;
			ResidentialAddressLine2 = String.IsNullOrWhiteSpace(participantForm.AddressLine2) ? "" : participantForm.AddressLine2;
			VisibleMinority = TriState(participantForm.VisibleMinority, null);
			City = participantForm.City;
			HighestLevelEducation = participantForm?.EducationLevel?.Caption;
			PostalCode = participantForm.PostalCode;
			MaritalStatus = participantForm.MaritalStatus?.Caption;
			FederalOfficialLanguage = participantForm.FederalOfficialLanguage?.Caption;
			NumberOfDependents = participantForm.NumberOfDependents;
		 }

		public ParticipantContactInfoViewModel()
		{

		}

		public string TriState(int? value, string[] options)
		{
			if (value != null)
			{
				// use default values
				if (options == null)
				{
					options = new[] { "Yes", "No", "Prefer not to answer" };
				}

				switch(value)
                {
					case 1:
						return options[0];
					case 2:
						return options[1];
					case 3:
						return options[2];
					default:
						return null;
				}
				
			}

			return null;
		}

		public string ImmigrantState(bool? options,int yeartoCanada)
		{
				switch (options)
				{
					case true:
						return $"Yes in {yeartoCanada}";
					case false:
						return "No";
					default:
						return null;
				}
		}

		public string RefugeeState(bool? options, string fromCountry)
		{
			switch (options)
			{
				case true:
					return $"Yes in {fromCountry}";
				case false:
					return "No";
				default:
					return null;
			}
		}

		public string GenderState(int gender)
		{
			switch (gender)
			{
				case 1:
					return "Male";
				case 2:
					return "Female";
				case 3:
					return "Prefer not to answer";
				case 4:
					return "Unspecified";
				default:
					return null;
			}
		}
	}
}
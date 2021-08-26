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
		public string MartialStatus { get; set; }
		public string FederalOfficialLanguage { get; set; }
		public int NumberOfDependents { get; set; }
		public string LastHighSchoolAttended { get; set; }
		public string LastHighSchoolCity { get; set; }


		public ParticipantContactInfoViewModel(ParticipantForm participantForm)
		{
			this.ReportedEmployerName = participantForm.GrantApplication.OrganizationLegalName;
			this.ResidencyStatus = participantForm.CanadianStatus.Caption;
			this.FullName = participantForm.LastName + ", " + participantForm.FirstName + " " + participantForm.MiddleName;
			this.Immigrated = participantForm.CanadaImmigrant ? string.Format("Yes in {0}", participantForm.YearToCanada) : "No";
			this.DateOfBirth = participantForm.BirthDate.ToString(DATEFORMAT);
			this.Age = AppDateTime.UtcNow.Year - participantForm.BirthDate.Year;
			this.Refugee = participantForm.CanadaRefugee ? string.Format("Yes from {0}", participantForm.FromCountry) : "No";
			this.Gender = TriState(participantForm.Gender, new[] { "Male", "Female", "Prefer not to answer" });
			this.YouthInCare = participantForm.YouthInCare ? "Yes" : "No";
			this.SIN = participantForm.SIN;
			this.PersonWithDisability = TriState(participantForm.PersonDisability, null);
			this.ParticipantPrimaryPhone = participantForm.PhoneNumber1 + (string.IsNullOrWhiteSpace(participantForm.PhoneExtension1) ? "" : " Ext " + participantForm.PhoneExtension1);
			this.AboriginalPerson = TriState(participantForm.PersonAboriginal, null);
			this.AlternatePhone = !string.IsNullOrWhiteSpace(participantForm.PhoneNumber2) ? (participantForm.PhoneNumber2 + (string.IsNullOrWhiteSpace(participantForm.PhoneExtension2) ? "" : " Ext " + participantForm.PhoneExtension2)) : "";
			this.AboriginalGroup = participantForm.PersonAboriginal == 1 ? participantForm?.AboriginalBand?.Caption : null;
			this.ParticipantEmail = participantForm.EmailAddress;
			this.Living = participantForm.PersonAboriginal == 1 && participantForm.AboriginalBand?.Id == 1 
				? (participantForm.LiveOnReserve ? "On" : "Off") + " Reserve"
				:null;
			this.ResidentialAddressLine1 = participantForm.AddressLine1;
			this.ResidentialAddressLine2 = String.IsNullOrWhiteSpace(participantForm.AddressLine2) ? "" : participantForm.AddressLine2;
			this.VisibleMinority = TriState(participantForm.VisibleMinority, null);
			this.City = participantForm.City;
			this.HighestLevelEducation = participantForm?.EducationLevel?.Caption;
			this.PostalCode = participantForm.PostalCode;
			this.MartialStatus = participantForm.MartialStatus?.Caption;
			this.FederalOfficialLanguage = participantForm.FederalOfficialLanguage?.Caption;
			this.NumberOfDependents = participantForm.NumberOfDependents;
			this.LastHighSchoolAttended = participantForm.LastHighSchoolName;
			this.LastHighSchoolCity = participantForm.LastHighSchoolCity;
		 }

		public ParticipantContactInfoViewModel()
		{

		}

		public string TriState(int value, string[] options)
		{
			// use default values
			if (options == null)
			{
				 options = new string[] { "Yes", "No", "Prefer not to answer" };
			}

			value = value.In(1, 2) ? value : 3;

			return options[value - 1];
		}
	}
}
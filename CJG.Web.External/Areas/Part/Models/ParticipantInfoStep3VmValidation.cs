using System;
using System.ComponentModel.DataAnnotations;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Part.Models
{
    public class ParticipantInfoStep3VmValidation
    {
        public static ValidationResult ValidateYearToCanada(int YearToCanada, ValidationContext context)
        {
            ParticipantInfoStep3ViewModel model = context.ObjectInstance as ParticipantInfoStep3ViewModel;
            if (model == null) throw new ArgumentNullException();
            ValidationResult result = ValidationResult.Success;

            if (model.CanadaImmigrant.GetValueOrDefault())
            {
                int upperYear = AppDateTime.UtcNow.Year;
                int lowerYear = upperYear - 75;

                if (YearToCanada < lowerYear || YearToCanada > upperYear)
                {
                    result = new ValidationResult(string.Format("Year field must be between {0} and {1}.", lowerYear, upperYear));
                }
                else
                {
                    if (YearToCanada < model.BirthYear)
                    {
                        result = new ValidationResult(string.Format("Immigration Date cannot be earlier than your birth year of {0}.", model.BirthYear));
                    }
                }
            }
            return result;
        }

        public static ValidationResult ValidateYouthInCare(bool? YouthInCare, ValidationContext context)
        {
            ParticipantInfoStep3ViewModel model = context.ObjectInstance as ParticipantInfoStep3ViewModel;
            if (model == null) throw new ArgumentNullException();
            ValidationResult result = ValidationResult.Success;

            if (model.Age > 14 && model.Age < 25)
            {
                if (!YouthInCare.HasValue)
                {
                    result = new ValidationResult("The Youth In Care field is required");
                }
            }
            return result;
        }

        public static ValidationResult ValidateLiveOnReserve(bool? LiveOnReserve, ValidationContext context)
        {
            ParticipantInfoStep3ViewModel model = context.ObjectInstance as ParticipantInfoStep3ViewModel;
            if (model == null) throw new ArgumentNullException();
            ValidationResult result = ValidationResult.Success;

            if (model.PersonAboriginal == 1)
            {
                if (model.AboriginalBand != null)
                {
                    if (model.AboriginalBand.Value == 1)
                    {
                        if (!LiveOnReserve.HasValue)
                        {
                            result = new ValidationResult("On/Off Reserve field is required");
                        }
                    }
                }
            }
            return result;
        }

		public static ValidationResult ValidatePersonAboriginal(int? PersonAboriginal, ValidationContext context)
		{
			ParticipantInfoStep3ViewModel model = context.ObjectInstance as ParticipantInfoStep3ViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;

			if (model.CanadianStatus == 1)
			{
				if (!PersonAboriginal.HasValue)
				{
							result = new ValidationResult("Aboriginal field is required.");
				}
			}
			return result;
		}

		public static ValidationResult ValidateImmigrant(bool? Immigrant, ValidationContext context)
		{
			ParticipantInfoStep3ViewModel model = context.ObjectInstance as ParticipantInfoStep3ViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;

			if (model.PersonAboriginal == 2 || model.PersonAboriginal==3)
			{
				if (!Immigrant.HasValue)
				{
					result = new ValidationResult("Canada Immigrant answer is required.");
				}
			}
			return result;
		}
		public static ValidationResult ValidateRefugee(bool? Refugee, ValidationContext context)
		{
			ParticipantInfoStep3ViewModel model = context.ObjectInstance as ParticipantInfoStep3ViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;

			if (model.PersonAboriginal == 2 || model.PersonAboriginal == 3)
			{
				if (!Refugee.HasValue)
				{
					result = new ValidationResult("Canada Refugee answer is required.");
				}
			}
			return result;
		}
		public static ValidationResult ValidateVisibileMinority(int? VisibileMinority, ValidationContext context)
		{
			ParticipantInfoStep3ViewModel model = context.ObjectInstance as ParticipantInfoStep3ViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;

			if (model.PersonAboriginal == 2 || model.PersonAboriginal == 3)
			{
				if (!VisibileMinority.HasValue)
				{
					result = new ValidationResult("Visibile Minority answer is required.");
				}
			}
			return result;
		}

		public static ValidationResult ValidateAboriginalBand(int? AboriginalBand, ValidationContext context)
        {
            ParticipantInfoStep3ViewModel model = context.ObjectInstance as ParticipantInfoStep3ViewModel;
            if (model == null) throw new ArgumentNullException();
            ValidationResult result = ValidationResult.Success;

            if (model.PersonAboriginal == 1)
            {
                if (!AboriginalBand.HasValue)
                {
                    result = new ValidationResult("A selection is required");
                }
            }
            return result;
        }

        public static ValidationResult ValidateFromCountry(string FromCountry, ValidationContext context)
        {
            ParticipantInfoStep3ViewModel model = context.ObjectInstance as ParticipantInfoStep3ViewModel;
            if (model == null) throw new ArgumentNullException();
            ValidationResult result = ValidationResult.Success;

            if (model.CanadaRefugee.GetValueOrDefault() && String.IsNullOrWhiteSpace(FromCountry))
            {
                result = new ValidationResult("The Country field is required.");
            }
            return result;
        }

        public static ValidationResult ValidateLastHighSchoolName(string LastHighSchoolName, ValidationContext context)
        {
            ParticipantInfoStep3ViewModel model = context.ObjectInstance as ParticipantInfoStep3ViewModel;
            if (model == null) throw new ArgumentNullException();
            ValidationResult result = ValidationResult.Success;

            if (model.EducationLevel != 1 && string.IsNullOrEmpty(LastHighSchoolName))
            {
                result = new ValidationResult("Name of last high school is required.");
            }
            return result;
        }

        public static ValidationResult ValidateLastHighSchoolCity(string LastHighSchoolCity, ValidationContext context)
        {
            ParticipantInfoStep3ViewModel model = context.ObjectInstance as ParticipantInfoStep3ViewModel;
            if (model == null) throw new ArgumentNullException();
            ValidationResult result = ValidationResult.Success;

            if (model.EducationLevel != 1 && string.IsNullOrEmpty(LastHighSchoolCity))
            {
                result = new ValidationResult("City of last high school is required.");
            }
            return result;
        }

		//public static ValidationResult ValidateEitherVisibileMinorityorIndigenous(int? VisibleMinority, ValidationContext context)
		//{
		//	ParticipantInfoStep3ViewModel model = context.ObjectInstance as ParticipantInfoStep3ViewModel;
		//	if (model == null) throw new ArgumentNullException();
		//	ValidationResult result = ValidationResult.Success;

		//	if (model.VisibleMinority == 1 && model.PersonAboriginal == 1)
		//	{
		//		result = new ValidationResult("Both Indigenous and Visible Minority cannot be marked Yes");
		//	}

		//	return result;
		//}
	}
}
using CJG.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace CJG.Web.External.Areas.Part.Models
{
	public class ParticipantInfoStep4VmValidation
	{
		private static bool IsEmployed(int employmentType)
		{
			return employmentType == 2 || employmentType == 3;
		}

		public static ValidationResult ValidateHowLongYears(int? HowLongYears, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;

			if (IsEmployed(model.EmploymentStatus))
			{
				if (!HowLongYears.HasValue)
				{
					result = new ValidationResult("The Year field is required.");
				}
				else if (HowLongYears.Value < 0)
				{
					result = new ValidationResult("The Year field must be greater than or equal to 0.");
				}
			}
			return result;
		}

		public static ValidationResult ValidateHowLongMonths(int? HowLongMonths, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;

			if (IsEmployed(model.EmploymentStatus))
			{
				if (!HowLongMonths.HasValue)
				{
					result = new ValidationResult("The Month field is required.");
				}
				else if (HowLongMonths.Value < 0)
				{
					result = new ValidationResult("The Month field must be greater than or equal to 0.");
				}
			}
			return result;
		}

		public static ValidationResult ValidateAvgHoursPerWeek(int? AvgHoursPerWeek, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;

			if (IsEmployed(model.EmploymentStatus))
			{
				if (!AvgHoursPerWeek.HasValue)
				{
					result = new ValidationResult("The Average Hour field is required.");
				}
				else if (AvgHoursPerWeek.Value < 0)
				{
					result = new ValidationResult("The Average Hour field must be greater than or equal to 0.");
				}
			}
			return result;
		}

		public static ValidationResult ValidateApprentice(bool? Apprentice, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;

			if (!Apprentice.HasValue)
			{
				result = new ValidationResult("The Apprentice field is required.");
			}
			return result;
		}

		public static ValidationResult ValidateOtherPrograms(bool? OtherPrograms, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;

			if (!OtherPrograms.HasValue)
			{
				result = new ValidationResult("The Other Funded field is required.");
			}
			return result;
		}

		public static ValidationResult ValidateItaRegistered(bool? itaRegistered, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;

			if (model.Apprentice.HasValue && model.Apprentice.Value == true)
			{
				if (!itaRegistered.HasValue)
				{
					result = new ValidationResult("The ITA Registered field is required.");
				}
			}
			return result;
		}

		public static ValidationResult ValidateEIBenefit(int eIBenefit, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;
			bool currentReceiveEI = model.CurrentReceiveEI ?? false;

			if (currentReceiveEI)
			{
				if (!(eIBenefit >= 1 && eIBenefit <= 6))
				{
					result = new ValidationResult("EI Benefit field is required.");
				}
			}

			return result;
		}

		public static ValidationResult ValidateMaternalPaternal(bool? maternalPaternal, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;

			bool currentReceiveEI = model.CurrentReceiveEI ?? false;

			if (model.EIBenefit != 6)
			{
				if (!maternalPaternal.HasValue && currentReceiveEI)
				{
					result = new ValidationResult("The Maternal / Paternal field is required.");
				}
			}
			return result;
		}

		public static ValidationResult ValidatePastMaternalPaternal(bool? pastMaternalPaternal, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;

			bool currentReceiveEI = model.CurrentReceiveEI ?? false;


			if (!pastMaternalPaternal.HasValue && currentReceiveEI && (model.EIBenefit >= 2 && model.EIBenefit <= 5))
			{
				result = new ValidationResult("The past Maternal / Paternal field is required.");
			}

			return result;
		}

		public static ValidationResult ValidateCurrentReceiveEI(bool? currentReceiveEI, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;

			if (!currentReceiveEI.HasValue && model.ProgramType == ProgramTypes.WDAService)
			{
				result = new ValidationResult("Currently receiving EI field is required.");
			}
			return result;
		}

		public static ValidationResult ValidateBceaClient(bool? bceaClient, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;

			if (model.EmploymentStatus == 1)
			{
				if (!bceaClient.HasValue)
				{
					result = new ValidationResult("The Income Assistance field is required.");
				}
			}
			return result;
		}

		public static ValidationResult ValidateEmployedBy(bool? employedBy, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;

			if (IsEmployed(model.EmploymentStatus))
			{
				if (!employedBy.HasValue)
				{
					result = new ValidationResult("The Employed By field is required.");
				}
			}
			return result;
		}

		public static ValidationResult ValidateBusinessOwner(bool? businessOwner, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;

			if (IsEmployed(model.EmploymentStatus))
			{
				if (!businessOwner.HasValue)
				{
					result = new ValidationResult("The Owner field is required.");
				}
			}
			return result;
		}

		public static ValidationResult ValidateHourlyWage(decimal? HourlyWage, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;

			if (IsEmployed(model.EmploymentStatus))
			{
				if (!HourlyWage.HasValue)
				{
					result = new ValidationResult("The Wage field is required.");
				}
				else if (HourlyWage.Value < 0)
				{
					result = new ValidationResult("The Wage field must be greater than or equal to 0.");
				}
			}
			return result;
		}

		public static ValidationResult ValidatePrimaryCity(string PrimaryCity, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;

			if (IsEmployed(model.EmploymentStatus))
			{
				if (String.IsNullOrWhiteSpace(PrimaryCity))
				{
					result = new ValidationResult("The City field is required.");
				}
			}
			return result;
		}

		public static ValidationResult ValidateEmploymentType(int? EmploymentType, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;

			if (IsEmployed(model.EmploymentStatus))
			{
				if (EmploymentType == null || EmploymentType <= 0)
				{
					result = new ValidationResult("The Employment Type field is required.");
				}
			}
			return result;
		}

		public static ValidationResult ValidateJobTitleBefore(string currentJobTitle, ValidationContext context)
		{
			var model = context.ObjectInstance as ParticipantInfoStep4ViewModel;

			if (model == null)
				throw new ArgumentNullException();

			if (IsEmployed(model.EmploymentStatus) && string.IsNullOrWhiteSpace(currentJobTitle))
				return new ValidationResult("Your Job Title before training is required.");

			return ValidationResult.Success;
		}

		public static ValidationResult ValidateCurrentNoc(int? nocCode, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;

			if (IsEmployed(model.EmploymentStatus))
			{
				if (!nocCode.HasValue || nocCode <= 0)
				{
					result = new ValidationResult("Your current National Occupation Classification (NOC) before training is required.");
				}
			}
			return result;
		}

		public static ValidationResult ValidateFutureNoc(int? nocCode, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;

			if (!nocCode.HasValue || nocCode <= 0)
			{
				result = new ValidationResult("Your National Occupation Classification (NOC) after training is required.");
			}
			return result;
		}

		public static ValidationResult ValidateOtherProgramDesc(string otherProgramDesc, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;

			if (model.OtherPrograms.HasValue && model.OtherPrograms.Value == true)
			{
				if (String.IsNullOrWhiteSpace(otherProgramDesc))
				{
					result = new ValidationResult("The provincially or federally funded program field is required.");
				}
			}
			return result;
		}
	}
}
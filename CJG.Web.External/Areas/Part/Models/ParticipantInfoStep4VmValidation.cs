using System;
using System.ComponentModel.DataAnnotations;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Part.Models
{
    public class ParticipantInfoStep4VmValidation
	{
		private static bool IsEmployed(int employmentType)
		{
			return employmentType == 2 || employmentType == 3 || employmentType == 6;
		}

		private static bool WasEmployed(int employmentType)
		{
			return employmentType == 1 || employmentType == 4;
		}

		private static bool WasEmployedOrInTraining(int employmentType)
		{
			return employmentType == 1 || employmentType == 4 || employmentType == 5;
		}

		public static ValidationResult ValidateMultipleEmploymentPositions(bool? multipleEmploymentPositions, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null)
				throw new ArgumentNullException();

			var result = ValidationResult.Success;

			if (!IsEmployed(model.EmploymentStatus))
				return result;

			if (!multipleEmploymentPositions.HasValue)
				result = new ValidationResult("The Multiple Employment Positions field is required.");

			return result;
		}

		public static ValidationResult ValidatePreviousEmploymentLastDayOfWork(DateTime? lastDateOfWork, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null)
				throw new ArgumentNullException();

			if (!WasEmployed(model.EmploymentStatus))
				return ValidationResult.Success;

			if (!lastDateOfWork.HasValue)
				return new ValidationResult("Previous employment last day of work is required.");

			if (lastDateOfWork.Value.ToUniversalTime() > AppDateTime.UtcNow)
				return new ValidationResult("Previous employment last day of work cannot be greater than today.");

			return ValidationResult.Success;
		}

		public static ValidationResult ValidateHowLongYears(int? howLongYears, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null)
				throw new ArgumentNullException();

			ValidationResult result = ValidationResult.Success;

			if (IsEmployed(model.EmploymentStatus))
			{
				if (!howLongYears.HasValue)
				{
					result = new ValidationResult("The Year field is required.");
				}
				else if (howLongYears.Value < 0)
				{
					result = new ValidationResult("The Year field must be greater than or equal to 0.");
				}
			}
			return result;
		}

		public static ValidationResult ValidateHowLongMonths(int? howLongMonths, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null)
				throw new ArgumentNullException();

			ValidationResult result = ValidationResult.Success;

			if (IsEmployed(model.EmploymentStatus))
			{
				if (!howLongMonths.HasValue)
				{
					result = new ValidationResult("The Month field is required.");
				}
				else if (howLongMonths.Value < 0)
				{
					result = new ValidationResult("The Month field must be greater than or equal to 0.");
				}
			}
			return result;
		}

		public static ValidationResult ValidateAvgHoursPerWeek(int? avgHoursPerWeek, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null)
				throw new ArgumentNullException();

			ValidationResult result = ValidationResult.Success;

			if (IsEmployed(model.EmploymentStatus))
			{
				if (!avgHoursPerWeek.HasValue)
				{
					result = new ValidationResult("The Average Hour field is required.");
				}
				else if (avgHoursPerWeek.Value < 0)
				{
					result = new ValidationResult("The Average Hour field must be greater than or equal to 0.");
				}
			}
			return result;
		}

		public static ValidationResult ValidatePreviousAvgHoursPerWeek(int? previousAverageHoursPerWeek, ValidationContext context)
		{
			var model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null)
				throw new ArgumentNullException();

			if (!WasEmployed(model.EmploymentStatus))
				return ValidationResult.Success;

			if (!previousAverageHoursPerWeek.HasValue)
				return new ValidationResult("The Previous Average Hour field is required.");

			var averageHoursPerWeek = previousAverageHoursPerWeek.Value;
			if (averageHoursPerWeek < 0 || averageHoursPerWeek > 168.0m)
				return new ValidationResult("The Previous Average Hours per Week must be within 0 to 168.");

			return ValidationResult.Success;
		}

		public static ValidationResult ValidateApprentice(bool? apprentice, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null)
				throw new ArgumentNullException();

			ValidationResult result = ValidationResult.Success;

			if (!apprentice.HasValue)
				result = new ValidationResult("The Apprentice field is required.");

			return result;
		}

		public static ValidationResult ValidateOtherPrograms(bool? otherPrograms, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null)
				throw new ArgumentNullException();

			ValidationResult result = ValidationResult.Success;

			if (!otherPrograms.HasValue)
				result = new ValidationResult("The Other Funded field is required.");

			return result;
		}

		public static ValidationResult ValidateItaRegistered(bool? itaRegistered, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null)
				throw new ArgumentNullException();

			ValidationResult result = ValidationResult.Success;

			if (model.Apprentice.HasValue && model.Apprentice.Value)
			{
				if (!itaRegistered.HasValue)
					result = new ValidationResult("The ITA Registered field is required.");
			}
			return result;
		}

		public static ValidationResult ValidateEIBenefit(int eIBenefit, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null)
				throw new ArgumentNullException();

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
			if (model == null)
				throw new ArgumentNullException();

			ValidationResult result = ValidationResult.Success;

			bool currentReceiveEI = model.CurrentReceiveEI ?? false;
			if (model.EIBenefit != 6)
			{
				if (!maternalPaternal.HasValue && currentReceiveEI)
					result = new ValidationResult("The Maternal / Paternal field is required.");
			}

			return result;
		}

		public static ValidationResult ValidatePastMaternalPaternal(bool? pastMaternalPaternal, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null)
				throw new ArgumentNullException();

			ValidationResult result = ValidationResult.Success;

			bool currentReceiveEI = model.CurrentReceiveEI ?? false;
			if (!pastMaternalPaternal.HasValue && currentReceiveEI && (model.EIBenefit >= 2 && model.EIBenefit <= 5))
				result = new ValidationResult("The past Maternal / Paternal field is required.");

			return result;
		}

		public static ValidationResult ValidateCurrentReceiveEI(bool? currentReceiveEI, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null)
				throw new ArgumentNullException();

			ValidationResult result = ValidationResult.Success;

			if (!currentReceiveEI.HasValue && model.ProgramType == ProgramTypes.WDAService)
				result = new ValidationResult("Currently receiving EI field is required.");

			return result;
		}

		public static ValidationResult ValidateBceaClient(bool? bceaClient, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null)
				throw new ArgumentNullException();

			ValidationResult result = ValidationResult.Success;

			if (model.EmploymentStatus == 1)
			{
				if (!bceaClient.HasValue)
					result = new ValidationResult("The Income Assistance field is required.");
			}

			return result;
		}

		public static ValidationResult ValidateEmployedBy(bool? employedBy, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null)
				throw new ArgumentNullException();

			ValidationResult result = ValidationResult.Success;

			if (IsEmployed(model.EmploymentStatus))
			{
				if (!employedBy.HasValue)
					result = new ValidationResult("The Employed By field is required.");
			}

			return result;
		}

		public static ValidationResult ValidateBusinessOwner(bool? businessOwner, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null)
				throw new ArgumentNullException();

			ValidationResult result = ValidationResult.Success;

			if (IsEmployed(model.EmploymentStatus))
			{
				if (!businessOwner.HasValue)
					result = new ValidationResult("The Owner field is required.");
			}

			return result;
		}

		public static ValidationResult ValidateHourlyWage(decimal? hourlyWage, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null)
				throw new ArgumentNullException();

			ValidationResult result = ValidationResult.Success;

			if (IsEmployed(model.EmploymentStatus))
			{
				if (!hourlyWage.HasValue)
				{
					result = new ValidationResult("The Wage field is required.");
				}
				else if (hourlyWage.Value < 0)
				{
					result = new ValidationResult("The Wage field must be greater than or equal to 0.");
				}
			}

			return result;
		}

		public static ValidationResult ValidatePreviousHourlyWage(decimal? previousHourlyWage, ValidationContext context)
		{
			var model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null)
				throw new ArgumentNullException();

			if (!WasEmployed(model.EmploymentStatus))
				return ValidationResult.Success;

			if (!previousHourlyWage.HasValue)
				return new ValidationResult("The Previous Hourly Wage field is required.");

			if (previousHourlyWage.Value < 0)
				return new ValidationResult("The Previous Hourly Wage field must be greater than or equal to 0.");

			return ValidationResult.Success;
		}

		public static ValidationResult ValidatePreviousEmployerFullName(string previousEmployerFullName, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null)
				throw new ArgumentNullException();

			ValidationResult result = ValidationResult.Success;

			if (WasEmployedOrInTraining(model.EmploymentStatus))
			{
				if (string.IsNullOrWhiteSpace(previousEmployerFullName))
					result = new ValidationResult("The Last Previous Employer field is required.");
			}

			return result;
		}

		public static ValidationResult ValidatePrimaryCity(string primaryCity, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null)
				throw new ArgumentNullException();

			ValidationResult result = ValidationResult.Success;

			if (IsEmployed(model.EmploymentStatus))
			{
				if (string.IsNullOrWhiteSpace(primaryCity))
					result = new ValidationResult("The City field is required.");
			}

			return result;
		}

		public static ValidationResult ValidateEmploymentType(int? employmentType, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null)
				throw new ArgumentNullException();

			ValidationResult result = ValidationResult.Success;

			if (IsEmployed(model.EmploymentStatus))
			{
				if (employmentType == null || employmentType <= 0)
					result = new ValidationResult("The Employment Type field is required.");
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
			if (model == null)
				throw new ArgumentNullException();

			ValidationResult result = ValidationResult.Success;

			if (IsEmployed(model.EmploymentStatus))
			{
				if (!nocCode.HasValue || nocCode <= 0)
					result = new ValidationResult("Your current National Occupation Classification (NOC) before training is required.");
			}

			return result;
		}

		public static ValidationResult ValidateFutureNoc(int? nocCode, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null)
				throw new ArgumentNullException();

			ValidationResult result = ValidationResult.Success;

			if (!nocCode.HasValue || nocCode <= 0)
				result = new ValidationResult("Your National Occupation Classification (NOC) after training is required.");

			return result;
		}

		public static ValidationResult ValidatePreviousEmploymentNoc(int? nocCode, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null)
				throw new ArgumentNullException();

			if (!WasEmployed(model.EmploymentStatus))
				return ValidationResult.Success;

			if (!nocCode.HasValue || nocCode <= 0)
				return new ValidationResult("Your National Occupation Classification (NOC) for previous employment is required.");

			return ValidationResult.Success;
		}

		public static ValidationResult ValidatePreviousEmploymentNaics(int? naicsCode, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null)
				throw new ArgumentNullException();

			if (!WasEmployed(model.EmploymentStatus))
				return ValidationResult.Success;

			if (!naicsCode.HasValue || naicsCode <= 0)
				return new ValidationResult("Your North American Industry Classification System (NAICS) for previous employment is required.");

			return ValidationResult.Success;
		}

		public static ValidationResult ValidateOtherProgramDesc(string otherProgramDesc, ValidationContext context)
		{
			ParticipantInfoStep4ViewModel model = context.ObjectInstance as ParticipantInfoStep4ViewModel;
			if (model == null)
				throw new ArgumentNullException();

			ValidationResult result = ValidationResult.Success;

			if (model.OtherPrograms.HasValue && model.OtherPrograms.Value == true)
			{
				if (string.IsNullOrWhiteSpace(otherProgramDesc))
					result = new ValidationResult("The provincially or federally funded program field is required.");
			}

			return result;
		}
	}
}
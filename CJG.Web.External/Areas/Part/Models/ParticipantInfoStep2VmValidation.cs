using CJG.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace CJG.Web.External.Areas.Part.Models
{
    public class ParticipantInfoStep2VmValidation
    {
      public static ValidationResult ValidateSIN(string sin, ValidationContext context)
        {
			
			ParticipantInfoStep2ViewModel model = context.ObjectInstance as ParticipantInfoStep2ViewModel;
            if (model == null) throw new ArgumentNullException();
            ValidationResult result = ValidationResult.Success;

            if (!IsValidSIN(sin))
            {
                 result = new ValidationResult("Incorrect Social Insurance Number");            
            }
            return result;
        }

        private static bool IsValidSIN(string socialInsuranceNumber) 
        {
            bool retValue = false;

            if (!String.IsNullOrEmpty(socialInsuranceNumber)
              && socialInsuranceNumber.Length == 9
              && socialInsuranceNumber != "000000000"
              && socialInsuranceNumber[0] != '0'
              && socialInsuranceNumber[0] != '8')
            {
                //add socialSecurityNumber != "000000000" condition before of adding leading 0
                int totalSSN = socialInsuranceNumber.Where((e) => e >= '0' && e <= '9').Select((e, i) => (e - 48) * (i % 2 == 0 ? 1 : 2)).Sum((e) => e / 10 + e % 10);
                retValue = totalSSN % 10 == 0;
            }
            return retValue;
        }

        public static ValidationResult ValidatePhone2(string phone2, ValidationContext context)
        {
            ParticipantInfoStep2ViewModel model = context.ObjectInstance as ParticipantInfoStep2ViewModel;
            if (model == null) throw new ArgumentNullException();
            ValidationResult result = ValidationResult.Success;
            if (!string.IsNullOrWhiteSpace(phone2))
            {
                var reg = new Regex(@"^[0-9]{10}$");
                if (!reg.IsMatch(phone2))
                { 
                    result = new ValidationResult("Alternative phone number must be 10-digits or blank");           
                }
            }
            return result;
        }

        public static ValidationResult ValidateBirthDate(string birthDate, ValidationContext context)
        {
            ParticipantInfoStep2ViewModel model = context.ObjectInstance as ParticipantInfoStep2ViewModel;
            if (model == null) throw new ArgumentNullException();
            ValidationResult result = ValidationResult.Success;
            if (!string.IsNullOrWhiteSpace(birthDate))
            {
				DateTime dteBirthDate = DateTime.Parse(birthDate);
				var oldest = AppDateTime.UtcNow.AddYears(-1 * Participant.OldestAge).Date;
				var youngest = AppDateTime.UtcNow.AddYears(-1 * Participant.YoungestAge).Date;
				if (dteBirthDate < oldest || dteBirthDate > youngest)
				{
					var errorMsg = string.Format("Participant must be between the ages of {0} and {1}.", model.ParticipantYoungestAge, model.ParticipantOldestAge);
					result = new ValidationResult(errorMsg);
				}

			}
            return result;
        }
		public static ValidationResult IsDuplicateSIN(string SIN, ValidationContext validationContext)
		{
			ParticipantInfoStep2ViewModel model = validationContext.ObjectInstance as ParticipantInfoStep2ViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;
			if (model.EnteredSINs != null)
			{
				foreach (var Existingsin in model.EnteredSINs)
				{
					if (Existingsin == SIN)
					{
						var errorMsg = string.Format("This SIN has already been used. If you would like to resubmit your PIF, you must first have your employer delete your previous PIF from the application.");
						result = new ValidationResult(errorMsg);

				}

				}
			}
			return result;
		}
	}
}
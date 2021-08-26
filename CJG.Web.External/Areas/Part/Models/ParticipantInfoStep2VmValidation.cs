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
                var oldest = AppDateTime.UtcNow.AddYears(-1 * model.ParticipantOldestAge).Date;
                var youngest = AppDateTime.UtcNow.AddYears(-1 * model.ParticipantYoungestAge).Date;

                if (dteBirthDate < oldest || dteBirthDate > youngest)
                {
                    var errorMsg = string.Format ("The birthdate must be between {0} and {1}.", oldest.ToString("yyyy-MM-dd"), youngest.ToString("yyyy-MM-dd"));
                    result = new ValidationResult(errorMsg);
                }
            }
            return result;
        }
    }
}
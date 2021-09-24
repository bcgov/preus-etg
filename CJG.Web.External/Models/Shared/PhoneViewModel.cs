using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CJG.Web.External.Models.Shared
{
    public class PhoneViewModel
    {
        [Required(ErrorMessage = "Contact phone number is required")]
        [RegularExpression("^\\D*(\\d\\D*){10}", ErrorMessage = "Contact phone number must be a 10-digit number")]
        public string Phone
        {
            get
            {
                return $"({PhoneAreaCode}) {PhoneExchange}-{PhoneNumber}";
            }
        }

        public string PhoneAreaCode { get; set; }
        public string PhoneExchange { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneExtension { get; set; }

        public PhoneViewModel()
        {
        }

        public PhoneViewModel(string phoneNumber, string phoneExtension)
        {
            PhoneAreaCode = GetPhoneAreaCodeString(phoneNumber);
            PhoneExchange = GetPhoneExchangeString(phoneNumber);
            PhoneNumber = GetPhoneNumberString(phoneNumber);
            PhoneExtension = phoneExtension;
        }

        public string GetPhoneAreaCodeString(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;

            string phoneAreaCode = StripPhoneNumber(value);
            if (string.IsNullOrEmpty(phoneAreaCode) || phoneAreaCode.Length <= 3) return null;

            return phoneAreaCode.Substring(0, (phoneAreaCode.Length >= 3) ? 3 : phoneAreaCode.Length);
        }

        public string GetPhoneExchangeString(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;

            string phoneExch = StripPhoneNumber(value);
            if (string.IsNullOrEmpty(phoneExch) || phoneExch.Length <= 3) return null;

            return phoneExch.Substring(3, (phoneExch.Length >= 6) ? 3 : phoneExch.Length - 3);
        }

        public string GetPhoneNumberString(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;

            string phoneNbr = StripPhoneNumber(value);
            if (string.IsNullOrEmpty(phoneNbr) || phoneNbr.Length <= 4) return null;

            return phoneNbr.Substring(6, (phoneNbr.Length >= 10) ? 4 : phoneNbr.Length - 6);
        }
        public string StripPhoneNumber(string value)
        {
            var nonNumeric = new Regex(@"[^\d]");
            if (value == null)
                return value;

            return nonNumeric.Replace(value, "");
        }
    }
}
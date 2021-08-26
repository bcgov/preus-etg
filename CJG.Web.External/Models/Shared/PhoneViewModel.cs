using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace CJG.Web.External.Models.Shared
{
    public class PhoneViewModel
    {
        //private string _phone;
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

        #region Methods
        public PhoneViewModel()
        {
        }
        public PhoneViewModel(string phoneNumber, string phoneExtension)
        {
            this.PhoneAreaCode = GetPhoneAreaCodeString(phoneNumber);
            this.PhoneExchange = GetPhoneExchangeString(phoneNumber);
            this.PhoneNumber = GetPhoneNumberString(phoneNumber);
            this.PhoneExtension = phoneExtension;
        }
        public string GetPhoneAreaCodeString(string value)
        {
            if (String.IsNullOrEmpty(value)) return null;

            string phoneAreaCode = StripPhoneNumber(value);
            if (String.IsNullOrEmpty(phoneAreaCode) || phoneAreaCode.Length <= 3) return null;

            return phoneAreaCode.Substring(0, (phoneAreaCode.Length >= 3) ? 3 : phoneAreaCode.Length);
        }

        public string GetPhoneExchangeString(string value)
        {
            if (String.IsNullOrEmpty(value)) return null;

            string phoneExch = StripPhoneNumber(value);
            if (String.IsNullOrEmpty(phoneExch) || phoneExch.Length <= 3) return null;

            return phoneExch.Substring(3, (phoneExch.Length >= 6) ? 3 : phoneExch.Length - 3);
        }

        public string GetPhoneNumberString(string value)
        {
            if (String.IsNullOrEmpty(value)) return null;

            string phoneNbr = StripPhoneNumber(value);
            if (String.IsNullOrEmpty(phoneNbr) || phoneNbr.Length <= 4) return null;

            return phoneNbr.Substring(6, (phoneNbr.Length >= 10) ? 4 : phoneNbr.Length - 6);
        }
        public string StripPhoneNumber(string value)
        {
            Regex NonNumeric = new Regex(@"[^\d]");
            if (value == null)
                return value;

            return NonNumeric.Replace(value, "");
        }
        #endregion
    }
}
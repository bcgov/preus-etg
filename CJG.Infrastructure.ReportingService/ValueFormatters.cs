using System.Text;
using CJG.Core.Entities;

namespace CJG.Infrastructure.ReportingService
{
    internal static class ValueFormatters
    {
        public static string FormatContactInfo(ParticipantForm f)
        {
            var str = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(f.EmailAddress))
            {
                str.Append("email: ");
                str.Append(f.EmailAddress);
            }

            var phone1 = FormatPhone(f.PhoneNumber1, f.PhoneExtension1);
            var phone2 = FormatPhone(f.PhoneNumber2, f.PhoneExtension2);

            if (!string.IsNullOrWhiteSpace(phone1) || !string.IsNullOrWhiteSpace(phone2))
            {
                if (str.Length > 0)
                {
                    str.Append(", ");
                }

                str.Append("phone: ");

                if (!string.IsNullOrWhiteSpace(phone1))
                {
                    str.Append(phone1);
                }

                if (!string.IsNullOrWhiteSpace(phone2))
                {
                    if (!string.IsNullOrWhiteSpace(phone1))
                    {
                        str.Append(", ");
                    }

                    str.Append(phone2);
                }
            }

            return str.ToString();
        }

        public static string FormatPhone(string phoneNumber, string phoneNumberExt = null)
        {
            return phoneNumber + (string.IsNullOrWhiteSpace(phoneNumberExt)
                ? null
                : " ext." + phoneNumberExt);
        }

        public static string FormatName(string firstName, string lastName, string middleName = null)
        {
            if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName) && string.IsNullOrWhiteSpace(middleName))
            {
                return null;
            }

            return $"{lastName}, {firstName}" +
                (string.IsNullOrWhiteSpace(middleName) ? null : $" {middleName}");
        }
    }
}

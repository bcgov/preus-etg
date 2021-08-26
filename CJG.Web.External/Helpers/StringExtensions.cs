using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace CJG.Web.External.Helpers
{
	/// <summary>
	/// <typeparamref name="StringExtensions"/> static class, provides extension methods for strings.
	/// </summary>
	public static class StringExtensions
	{
		private const string EclipsisText = "...";
		private static Regex NonNumeric = new Regex(@"[^\d]");

		/// <summary>
		/// Changes all spaces into hyphens.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string ChangeSpacesToHyphens(this string value)
		{
			return value.Replace(" ", "-");
		}

		/// <summary>
		/// Truncate the string to the specified length.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="maxLength"></param>
		/// <returns></returns>
		public static string Truncate(this string value, int maxLength)
		{
			return value.Length <= maxLength ? value : value.Substring(0, maxLength);
		}

		/// <summary>
		/// Truncate the string to the specified length and append elipses...
		/// </summary>
		/// <param name="value"></param>
		/// <param name="maxLength"></param>
		/// <returns></returns>
		public static string TruncateWithEllipsis(this string value, int maxLength)
		{
			return value.Length <= maxLength ? value : value.Substring(0, maxLength) + EclipsisText;
		}

		/// <summary>
		/// Strips all non-numeric values from string value.
		/// </summary>
		/// <param name="value">The phone number to strip of non-numeric values.</param>
		/// <returns>Returns only the numeric values of the phone number</returns>
		public static string StripPhoneNumber(this string value)
		{
			if (value == null)
				return value;

			return NonNumeric.Replace(value, "");
		}

		public static string FormatPhoneNumberWithExtension(this string phoneNumber, string phoneNumberExt, string extensionDelimeter = "ext.")
		{
			return phoneNumber + (string.IsNullOrWhiteSpace(phoneNumberExt)
				? null
				: $" {extensionDelimeter} {phoneNumberExt}");
		}

		/// <summary>
		/// Formats the phone number
		/// </summary>
		/// <example>(123) 123-1234</example>
		/// <param name="value">The phone number you want to format.</param>
		/// <returns>A formatted phone number.</returns>
		public static string FormatPhoneNumber(this string value)
		{
			if (value == null)
				return null;

			var clean = value.StripPhoneNumber();

			if (clean.Length != 10)
			{
				// let the model validation fail and display the appropriate message
				return value;
			}

			return $"({clean.Substring(0, 3)}) {clean.Substring(3, 3)}-{clean.Substring(6, 4)}";
		}

		/// <summary>
		/// Extracts the 3 digit area code part of a phone number (i..e (123) 123-1234).
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string GetPhoneAreaCode(this string value)
		{
			if (String.IsNullOrEmpty(value)) return null;

			var result = value.StripPhoneNumber();
			if (String.IsNullOrEmpty(result) || result.Length <= 3) return null;

			return result.Substring(0, (result.Length >= 3) ? 3 : result.Length);
		}

		/// <summary>
		/// Extracts the 3 digit exchange part of a phone number (i.e. (123) 123-1234).
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string GetPhoneExchange(this string value)
		{
			if (String.IsNullOrEmpty(value)) return null;

			var result = value.StripPhoneNumber();
			if (String.IsNullOrEmpty(result) || result.Length <= 3) return null;

			return result.Substring(3, (result.Length >= 6) ? 3 : result.Length - 3);
		}

		/// <summary>
		/// Extracts the 4 digit number part of a phone number (i.e. (123) 123-1234).
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string GetPhoneNumber(this string value)
		{
			if (String.IsNullOrEmpty(value)) return null;

			var result = value.StripPhoneNumber();
			if (String.IsNullOrEmpty(result) || result.Length <= 4) return null;

			return result.Substring(6, (result.Length >= 10) ? 4 : result.Length - 6);
		}

		/// <summary>
		/// Strip out non-numeric and return the value.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string GetPhoneExtension(this string value)
		{
			if (String.IsNullOrEmpty(value)) return null;

			var result = value.StripPhoneNumber();
			return result;
		}

		public static string FormatTextLinesToHtml(string text, int maxLengthLastLine, int maxLines)
		{
			if (string.IsNullOrWhiteSpace(text)) return text;

			var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

			var str = new StringBuilder();

			for (var i = 0; i < lines.Length; i++)
			{
				if (i > 0)
				{
					str.Append("<br/>");
				}

				if (i >= lines.Length - 1)
				{
					str.Append(TruncateWithEllipsis(lines[i], maxLengthLastLine));
				}
				else if (i >= maxLines - 1)
				{
					str.Append(lines[i].Length <= maxLengthLastLine
						? lines[i]
						: lines[i].Substring(0, maxLengthLastLine));

					str.Append(EclipsisText);
				}
				else
				{
					str.Append(lines[i]);
				}

				if(i >= maxLines - 1) break;
			}

			return str.ToString();
		}

		public static string ToDollarCurrencyString(this decimal number, int precision = 2, int currencyNegativePattern = 0)
		{
			NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
			nfi.CurrencyDecimalDigits = precision;
			nfi.CurrencyNegativePattern = currencyNegativePattern;
			return number.ToString("C", nfi);
		}

		

		/// <summary>
		/// Custom percentage formating that doesn't round values but truncate for given number of decimal points. 
		/// Supports only culture invariant predefined format "0.0%;(0.0)%"
		/// </summary>
		/// <param name="number">Source value</param>
		/// <param name="decimalPlaces">Number of decimal places</param>
		/// <param name="defaultFormatWithRounding">Use default C# formatting (0.9995 -> 100.0%) or custom without rounding (0.9995 -> 99.9%)</param>
		/// <returns></returns>
		public static string ToPercentString(this decimal number, int decimalPlaces = 1, bool defaultFormatWithRounding = false)
		{
			if (defaultFormatWithRounding)
			{
				var zeroesTemplatePart = new string('0', decimalPlaces);
				var template = $"0.{zeroesTemplatePart}%;(0.{zeroesTemplatePart})%";
				return number.TruncateDecimal(3 + decimalPlaces).ToString(template);
			}
			else
			{
				var isNegative = number < 0;
				var roundedNumber = number.TruncateDecimal(3 + decimalPlaces) * 100;
				var roundedNumberString = (isNegative ? Math.Abs(roundedNumber) : roundedNumber).ToString(CultureInfo.InvariantCulture);
				var dec = new CultureInfo("en-US", false).NumberFormat.NumberDecimalSeparator[0];
				var foundDecimalPlace = roundedNumberString.LastIndexOf(dec);
				string foramttedString;
				if (foundDecimalPlace > 0)
				{
					var length = foundDecimalPlace + decimalPlaces + 1;
					foramttedString = length > roundedNumberString.Length 
						? roundedNumberString + new string('0', decimalPlaces - (roundedNumberString.Length - foundDecimalPlace - 1))
						: roundedNumberString.Substring(0, length);
				}
				else
				{
					foramttedString = roundedNumberString + "." + new string('0', decimalPlaces);
				}
				return $"{(isNegative ? $"({foramttedString})" : foramttedString)}%";
			}
			
		}

		public static decimal TruncateDecimal(this decimal value, int precision)
		{
			var step = (decimal)Math.Pow(10, precision);
			var tmp = Math.Truncate(step * value);
			return tmp / step;
		}

		/// <summary>
		/// Removes the file extension from the file name
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string RemoveFileExtension(this string value)
		{
			var fileName = value;

			if (fileName.Contains("."))
			{
				fileName = value.Split('.')[0];
			}

			return fileName;
		}
	}
}
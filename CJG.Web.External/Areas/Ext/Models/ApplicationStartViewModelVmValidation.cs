using CJG.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class ApplicationStartViewModelVmValidation
	{
		public static ValidationResult ValidateAlternateFirstName(string alternateFirstName, ValidationContext context)
		{
			ApplicationStartViewModel model = context.ObjectInstance as ApplicationStartViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;
			bool isAlternateContact = model.IsAlternateContact == null ? false : model.IsAlternateContact.Value;

			if (!(string.IsNullOrWhiteSpace(alternateFirstName) && !isAlternateContact))
			{
				var reg = new Regex(@"^[a-zA-Z '-]*$");

				if (string.IsNullOrWhiteSpace(alternateFirstName))
				{
					result = new ValidationResult("First Name is required.");
				}
				else if (!reg.IsMatch(alternateFirstName))
				{
					result = new ValidationResult("Invalid format.");
				}
			}
			return result;
		}

		public static ValidationResult ValidateAlternateLastName(string alternateLastName, ValidationContext context)
		{
			ApplicationStartViewModel model = context.ObjectInstance as ApplicationStartViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;
			bool isAlternateContact = model.IsAlternateContact == null ? false : model.IsAlternateContact.Value;

			if (!(string.IsNullOrWhiteSpace(alternateLastName) && !isAlternateContact))
			{
				var reg = new Regex(@"^[a-zA-Z '-]*$");

				if (string.IsNullOrWhiteSpace(alternateLastName))
				{
					result = new ValidationResult("Last Name is required.");
				}
				else if (!reg.IsMatch(alternateLastName))
				{
					result = new ValidationResult("Invalid format.");
				}
			}
			return result;
		}

		public static ValidationResult ValidateAlternateEmail(string alternateEmail, ValidationContext context)
		{
			ApplicationStartViewModel model = context.ObjectInstance as ApplicationStartViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;
			bool isAlternateContact = model.IsAlternateContact == null ? false : model.IsAlternateContact.Value;

			if (!(string.IsNullOrWhiteSpace(alternateEmail) && !isAlternateContact))
			{
				var reg = new Regex(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z])?)*$");

				if (string.IsNullOrWhiteSpace(alternateEmail))
				{
					result = new ValidationResult("Email Address is required.");
				}
				else if (!reg.IsMatch(alternateEmail))
				{
					result = new ValidationResult("Invalid Email Address.");
				}
			}
			return result;
		}

		public static ValidationResult ValidateAlternateJobTitle(string alternateJobTitle, ValidationContext context)
		{
			ApplicationStartViewModel model = context.ObjectInstance as ApplicationStartViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;
			bool isAlternateContact = model.IsAlternateContact == null ? false : model.IsAlternateContact.Value;

			if (!(string.IsNullOrWhiteSpace(alternateJobTitle) && !isAlternateContact))
			{
				var reg = new Regex(@"^[a-zA-Z '-]*$");

				if (string.IsNullOrWhiteSpace(alternateJobTitle))
				{
					result = new ValidationResult("Job Title is required.");
				}
				else if (!reg.IsMatch(alternateJobTitle))
				{
					result = new ValidationResult("Invalid format.");
				}
			}
			return result;
		}


		public static ValidationResult ValidateAlternatePhone(string alternatePhone, ValidationContext context)
		{
			ApplicationStartViewModel model = context.ObjectInstance as ApplicationStartViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;

			bool isAlternateContact = model.IsAlternateContact == null ? false : model.IsAlternateContact.Value;

			if (!string.IsNullOrWhiteSpace(alternatePhone))
			{
				var reg = new Regex(@"^\(\d{3}\)\ \d{3}\-\d{4}");

				if ((!reg.IsMatch(alternatePhone) || alternatePhone == "() -") && isAlternateContact)
				{
					result = new ValidationResult("Phone number is required and must be 10-digits");
				}
			}
			return result;
		}


	}
}
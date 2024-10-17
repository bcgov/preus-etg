using CJG.Core.Entities;
using CJG.Core.Entities.Attributes;
using CJG.Web.External.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class TrainingTrainerDetailsListViewModel : ViewModelBase
	{
		#region Properties
		[Required(ErrorMessage = "The Contact First Name is required"),NameValidation("Contact First Name ")]
		public string ContactFirstName { get; set; }

		[Required(ErrorMessage = "The Contact Last Name is required"), NameValidation("Contact Last Name ")]
		public string ContactLastName { get; set; }

		[Required(ErrorMessage = "The Contact email is required")]
		[StringLength(500, ErrorMessage = "The field must be a string with a maximum length of 500")]
		[RegularExpression(@"^[\s*\d+\s*]*[a-zA-Z0-9_\.-]+@([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,6}[\s*\d+\s*]*$", ErrorMessage = "Email Address is not valid")]
		public string ContactEmail { get; set; }
		
		public string ContactNumberAreaCode { get; set; }
		public string ContactNumberExchange { get; set; }
		public string ContactNumberNumber { get; set; }
		public string ContactNumberExtension { get; set; }
		public string ContactPhoneNumber =>
			$"({ContactNumberAreaCode}) {ContactNumberExchange}-{ContactNumberNumber}" 
				+ (string.IsNullOrWhiteSpace(ContactNumberExtension) ? null :$" Ext: {ContactNumberExtension}");

		[Required(ErrorMessage = "The Contact Phone Number is required")]
		[StringLength(10, MinimumLength = 10, ErrorMessage = "The Contact Phone Number is invalid")]
		public string PhoneNumberNoExtension => ContactNumberAreaCode + ContactNumberExchange + ContactNumberNumber;
		#endregion

		#region Constructors
		public TrainingTrainerDetailsListViewModel()
		{

		}

		public TrainingTrainerDetailsListViewModel(TrainingProvider trainingProvider)
		{
			if (trainingProvider == null) throw new ArgumentNullException(nameof(trainingProvider));

			this.ContactFirstName = trainingProvider.ContactFirstName;
			this.ContactLastName = trainingProvider.ContactLastName;
			this.ContactEmail = trainingProvider.ContactEmail;
			this.ContactNumberAreaCode = trainingProvider.ContactPhoneNumber?.GetPhoneAreaCode()?.ToString();
			this.ContactNumberExchange = trainingProvider.ContactPhoneNumber?.GetPhoneExchange()?.ToString();
			this.ContactNumberNumber = trainingProvider.ContactPhoneNumber?.GetPhoneNumber()?.ToString();
			this.ContactNumberExtension = trainingProvider.ContactPhoneExtension?.GetPhoneExtension()?.ToString();
		}
		#endregion
	}
}
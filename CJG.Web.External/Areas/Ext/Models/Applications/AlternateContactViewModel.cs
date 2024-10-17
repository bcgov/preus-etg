using System;
using System.Security.Principal;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Ext.Models.Applications
{
	public class AlternateContactViewModel : BaseViewModel
	{
		public bool IsAlternateContact { get; set; }
		public string AlternateSalutation { get; set; }
		public string AlternateFirstName { get; set; }
		public string AlternateLastName { get; set; }
		public string AlternateJobTitle { get; set; }
		public string AlternateEmail { get; set; }
		public PhoneViewModel PhoneNumberViewModel { get; set; }

		public bool MailingAddressSameAsPhysical { get; set; }
		public bool EnableChangeApplicationContactButton { get; set; }

		public string RowVersion { get; set; }

		public bool EditApplicantContact { get; set; }

		public AlternateContactViewModel() { }

		public AlternateContactViewModel(GrantApplication grantApplication, IUserService userService, IPrincipal user)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			if (userService == null)
				throw new ArgumentNullException(nameof(userService));

			if (user == null)
				throw new ArgumentNullException(nameof(user));

			Id = grantApplication.Id;
			RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			IsAlternateContact = grantApplication.IsAlternateContact.Value;
			AlternateSalutation = grantApplication.ApplicantSalutation;
			AlternateFirstName = grantApplication.ApplicantFirstName;
			AlternateLastName = grantApplication.ApplicantLastName;
			AlternateJobTitle = grantApplication.ApplicantJobTitle;
			PhoneNumberViewModel = new PhoneViewModel(grantApplication.AlternatePhoneNumber, grantApplication.AlternatePhoneExtension);

			EditApplicantContact = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditApplicantContact);
			EnableChangeApplicationContactButton = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.ChangeApplicantContactButton);
		}

		public AlternateContactViewModel(GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			Id = grantApplication.Id;
			RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			IsAlternateContact = grantApplication.IsAlternateContact.Value;
			AlternateFirstName = grantApplication.AlternateFirstName;
			AlternateLastName = grantApplication.AlternateLastName;
			AlternateJobTitle = grantApplication.AlternateJobTitle;
			AlternateEmail = grantApplication.AlternateEmail;
			PhoneNumberViewModel = new PhoneViewModel(grantApplication.AlternatePhoneNumber, grantApplication.AlternatePhoneExtension);
		}

		public void MapToGrantApplication(GrantApplication grantApplication, IStaticDataService staticDataService, IApplicationAddressService applicationAddressService)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			if (staticDataService == null) throw new ArgumentNullException(nameof(staticDataService));
			if (applicationAddressService == null) throw new ArgumentNullException(nameof(applicationAddressService));

			grantApplication.RowVersion = Convert.FromBase64String(RowVersion);
			grantApplication.IsAlternateContact = IsAlternateContact;
			grantApplication.AlternateSalutation = AlternateSalutation;
			grantApplication.AlternateFirstName = AlternateFirstName;
			grantApplication.AlternateLastName = AlternateLastName;
			grantApplication.AlternatePhoneNumber = PhoneNumberViewModel.Phone;
			grantApplication.AlternatePhoneExtension = PhoneNumberViewModel.PhoneExtension;
			grantApplication.AlternateJobTitle = AlternateJobTitle;
			grantApplication.AlternateEmail = AlternateEmail;
		}
	}
}
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace CJG.Web.External.Areas.Int.Models
{
	public class ApplicantContactViewModel : BaseViewModel
	{
		#region Properties
		public string ApplicantSalutation { get; set; }

		public string ApplicantFirstName { get; set; }

		public string ApplicantLastName { get; set; }

		public string ApplicantJobTitle { get; set; }

		public string ApplicantEmail { get; set; }
		public bool MailingAddressSameAsPhysical { get; set; }
		public bool EnableChangeApplicationContactButton { get; set; }
		public string RowVersion { get; set; }
		public AddressViewModel PhysicalAddress { get; set; }
		public AddressViewModel MailingAddress { get; set; } 
		public PhoneViewModel PhoneNumberViewModel { get; set; }

		public bool EditApplicantContact { get; set; }
		#endregion

		#region Constructors
		public ApplicantContactViewModel()
		{

		}

		public ApplicantContactViewModel(GrantApplication grantApplication, IUserService userService, IPrincipal user)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			if (userService == null) throw new ArgumentNullException(nameof(userService));
			if (user == null) throw new ArgumentNullException(nameof(user));

			this.Id = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.ApplicantSalutation = grantApplication.ApplicantSalutation;
			this.ApplicantFirstName = grantApplication.ApplicantFirstName;
			this.ApplicantLastName = grantApplication.ApplicantLastName;
			this.ApplicantJobTitle = grantApplication.ApplicantJobTitle;
			this.PhoneNumberViewModel = new PhoneViewModel(grantApplication.ApplicantPhoneNumber,
														grantApplication.ApplicantPhoneExtension);
			if (grantApplication.ApplicantPhysicalAddress != null)
			{
				this.PhysicalAddress = new AddressViewModel(grantApplication.ApplicantPhysicalAddress);
			}
			this.MailingAddressSameAsPhysical = 
				grantApplication.ApplicantMailingAddress?.IsSameAddress(grantApplication.ApplicantPhysicalAddress) ?? false;
			if (this.MailingAddressSameAsPhysical || grantApplication.ApplicantMailingAddress == null)
			{
				this.MailingAddress = new AddressViewModel();
			}
			else
			{
				this.MailingAddress = new AddressViewModel(grantApplication.ApplicantMailingAddress);
			}

			if (grantApplication.GetApplicationAdministratorIds().ToList() is List<int> applicationAdministrators && applicationAdministrators.Count > 0)
			{
				this.ApplicantEmail = userService.GetUser(applicationAdministrators[0])?.EmailAddress;
			}

			this.EditApplicantContact = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditApplicantContact);
			this.EnableChangeApplicationContactButton = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditApplicantContact);
		}
		#endregion

		#region Methods
		public void MapToGrantApplication(GrantApplication grantApplication, IStaticDataService staticDataService, IApplicationAddressService applicationAddressService)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			if (staticDataService == null) throw new ArgumentNullException(nameof(staticDataService));
			if (applicationAddressService == null) throw new ArgumentNullException(nameof(applicationAddressService));

			grantApplication.RowVersion = Convert.FromBase64String(this.RowVersion);
			grantApplication.ApplicantSalutation = this.ApplicantSalutation;
			grantApplication.ApplicantFirstName = this.ApplicantFirstName;
			grantApplication.ApplicantLastName = this.ApplicantLastName;
			grantApplication.ApplicantPhoneNumber = this.PhoneNumberViewModel.Phone;
			grantApplication.ApplicantPhoneExtension = this.PhoneNumberViewModel.PhoneExtension;
			grantApplication.ApplicantJobTitle = this.ApplicantJobTitle;
			this.PhysicalAddress.MapToApplicationAddress(grantApplication.ApplicantPhysicalAddress, staticDataService, applicationAddressService);
			if (this.MailingAddressSameAsPhysical)
			{
				grantApplication.ApplicantMailingAddress = grantApplication.ApplicantPhysicalAddress;
				grantApplication.ApplicantMailingAddressId = grantApplication.ApplicantPhysicalAddressId;
			}
			else
			{
				if (this.MailingAddress.Id == 0)
				{
					var address = new ApplicationAddress();
					this.MailingAddress.MapToApplicationAddress(address, staticDataService, applicationAddressService);
					grantApplication.ApplicantMailingAddress = address;
					grantApplication.ApplicantMailingAddressId = address.Id;
				}
				else
				{
					this.MailingAddress.MapToApplicationAddress(grantApplication.ApplicantMailingAddress, staticDataService, applicationAddressService);
				}
			}
		}
		#endregion
	}
}
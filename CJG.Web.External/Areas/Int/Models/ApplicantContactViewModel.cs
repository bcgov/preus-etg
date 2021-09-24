using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Models.Shared;

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
		public bool EnableChangeAlternateContactButton { get; set; }
		public string RowVersion { get; set; }
		public AddressViewModel PhysicalAddress { get; set; }
		public AddressViewModel MailingAddress { get; set; }
		public PhoneViewModel PhoneNumberViewModel { get; set; }
		public PhoneViewModel AlternatePhoneNumberViewModel { get; set; }

		public ProgramTypes ProgramType { get; set; }
		public string AlternateFirstName { get; set; }
		public string AlternateLastName { get; set; }
		public string AlternateEmail { get; set; }
		public string AlternateJobTitle { get; set; }
		public string AlternatePhoneNumber { get; set; }
		public string AlternatePhoneExtension { get; set; }

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

			Id = grantApplication.Id;
			RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			ApplicantSalutation = grantApplication.ApplicantSalutation;
			ApplicantFirstName = grantApplication.ApplicantFirstName;
			ApplicantLastName = grantApplication.ApplicantLastName;
			ApplicantJobTitle = grantApplication.ApplicantJobTitle;
			PhoneNumberViewModel = new PhoneViewModel(grantApplication.ApplicantPhoneNumber, grantApplication.ApplicantPhoneExtension);
			ProgramType = grantApplication.GetProgramType();
			AlternatePhoneNumberViewModel = grantApplication.AlternatePhoneNumber == null ? null : new PhoneViewModel(grantApplication.AlternatePhoneNumber,
											grantApplication.AlternatePhoneExtension);
			AlternateFirstName = grantApplication.AlternateFirstName;
			AlternateLastName = grantApplication.AlternateLastName;
			AlternateEmail = grantApplication.AlternateEmail;
			AlternateJobTitle = grantApplication.AlternateJobTitle;
			AlternatePhoneNumber = grantApplication.AlternatePhoneNumber;
			AlternatePhoneExtension = grantApplication.AlternatePhoneExtension;
			
			if (grantApplication.ApplicantPhysicalAddress != null)
			{
				PhysicalAddress = new AddressViewModel(grantApplication.ApplicantPhysicalAddress);
			}

			MailingAddressSameAsPhysical = grantApplication.ApplicantMailingAddress?.IsSameAddress(grantApplication.ApplicantPhysicalAddress) ?? false;
			if (MailingAddressSameAsPhysical || grantApplication.ApplicantMailingAddress == null)
			{
				MailingAddress = new AddressViewModel();
			}
			else
			{
				MailingAddress = new AddressViewModel(grantApplication.ApplicantMailingAddress);
			}

			if (grantApplication.GetApplicationAdministratorIds().ToList() is List<int> applicationAdministrators && applicationAdministrators.Count > 0)
			{
				ApplicantEmail = userService.GetUser(applicationAdministrators[0])?.EmailAddress;
			}

			EditApplicantContact = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditApplicantContact);
			EnableChangeApplicationContactButton = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.ChangeApplicantContactButton);

			var director = user.HasPrivilege(Privilege.AM4);
			var assessor = user.HasPrivilege(Privilege.AM2, Privilege.AM3, Privilege.AM5);
			EnableChangeAlternateContactButton = (director || assessor) && grantApplication.GetProgramType() == ProgramTypes.WDAService;
		}
		
		#endregion

		#region Methods
		public void MapToGrantApplication(GrantApplication grantApplication, IStaticDataService staticDataService, IApplicationAddressService applicationAddressService)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			if (staticDataService == null) throw new ArgumentNullException(nameof(staticDataService));
			if (applicationAddressService == null) throw new ArgumentNullException(nameof(applicationAddressService));

			grantApplication.RowVersion = Convert.FromBase64String(RowVersion);
			grantApplication.ApplicantSalutation = ApplicantSalutation;
			grantApplication.ApplicantFirstName = ApplicantFirstName;
			grantApplication.ApplicantLastName = ApplicantLastName;
			grantApplication.ApplicantPhoneNumber = PhoneNumberViewModel.Phone;
			grantApplication.ApplicantPhoneExtension = PhoneNumberViewModel.PhoneExtension;
			grantApplication.ApplicantJobTitle = ApplicantJobTitle;

			PhysicalAddress.MapToApplicationAddress(grantApplication.ApplicantPhysicalAddress, staticDataService, applicationAddressService);
			if (MailingAddressSameAsPhysical)
			{
				grantApplication.ApplicantMailingAddress = grantApplication.ApplicantPhysicalAddress;
				grantApplication.ApplicantMailingAddressId = grantApplication.ApplicantPhysicalAddressId;
			}
			else
			{
				if (MailingAddress.Id == 0)
				{
					var address = new ApplicationAddress();
					MailingAddress.MapToApplicationAddress(address, staticDataService, applicationAddressService);
					grantApplication.ApplicantMailingAddress = address;
					grantApplication.ApplicantMailingAddressId = address.Id;
				}
				else
				{
					MailingAddress.MapToApplicationAddress(grantApplication.ApplicantMailingAddress, staticDataService, applicationAddressService);
				}
			}
		}
		#endregion
	}
}
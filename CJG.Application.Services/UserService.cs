using CJG.Application.Business.Models;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="UserService"/> class, provides a way to manage users within the application.
	/// </summary>
	public class UserService : Service, IUserService
	{
		#region Variables
		private readonly IBCeIDService _bceIdService;
		private readonly ISiteMinderService _siteMinderService;
		private readonly IStaticDataService _staticDataService;

		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="UserService"/> class.
		/// </summary>
		/// <param name="bceIdRepository"></param>
		/// <param name="siteMinderService"></param>
		/// <param name="staticDataService"></param>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public UserService(IBCeIDService bceIdRepository, ISiteMinderService siteMinderService, IStaticDataService staticDataService, IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
			_bceIdService = bceIdRepository;
			_siteMinderService = siteMinderService;
			_staticDataService = staticDataService;
		}

		static UserService()
		{
		}

		#endregion

		#region Methods
		/// <summary>
		/// Create a new user profile in the datasource from BCeID.
		/// </summary>
		/// <param name="guid"></param>
		/// <param name="userProfileDetails"></param>
		public void CreateUserProfile(Guid guid, UserProfileDetailModel userProfileDetails)
		{
			var newUser = GetBCeIDUser(guid);
			if (newUser == null)
				throw new NoContentException(nameof(newUser));

			userProfileDetails.BindBusinessUserToEntity(newUser);

			if (newUser.Organization?.BCeIDGuid != null)
			{
				var organization = _dbContext.Organizations.SingleOrDefault(o => o.BCeIDGuid == newUser.Organization.BCeIDGuid);

				// If the Organization already exists, use it instead of the one provided by SiteMinder.
				if (organization != null)
					newUser.Organization = organization;

				// CJG-549: MUST test for users list not being null
				bool haveUsers = _dbContext?.Users != null;
				newUser.IsOrganizationProfileAdministrator = true;

				if (haveUsers && organization != null)
				{
					newUser.IsOrganizationProfileAdministrator = !_dbContext.Users.Any(u => u.OrganizationId == organization.Id && u.IsOrganizationProfileAdministrator);
				}
			}

			newUser.Organization.RequiredProfileUpdate = true; // CJG-678: a stop-gap solution to by pass new organization validation for new users.

			_dbContext.Users.Add(newUser);
			_dbContext.CommitTransaction();
		}

		/// <summary>
		/// Update an existing user profile in the datasource.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="userProfileDetails"></param>
		public void UpdateUserProfile(int userId, UserProfileDetailModel userProfileDetails)
		{
			UpdateUserDetails(userId, userProfileDetails, false);
			_dbContext.CommitTransaction();
		}

		public void UpdateUserDetails(int userId, UserProfileDetailModel userProfileDetails, bool standAloneTransaction = true)
		{
			var user = _dbContext.Users.Find(userId);
			if (user == null) throw new NoContentException(nameof(user));

			userProfileDetails.BindBusinessUserToEntity(user);

			_dbContext.Update(user);
			if (standAloneTransaction)
				_dbContext.CommitTransaction();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		public UserProfileConfirmationModel GetConfirmationDetails(Guid guid)
		{
			var user = GetBCeIDUser(guid);
			var data = new UserProfileConfirmationModel
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				EmailAddress = user.EmailAddress,
				PhoneNumber = user.PhoneNumber
			};
			return data;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public UserProfileDetailModel GetUserProfileDetails(int userId)
		{
			var model = new UserProfileDetailModel();
			if (userId == 0)
			{
				model.MailingAddressSameAsBusiness = true;
				model.PhysicalRegionId = "BC";
				model.Subscribe = true;
			}
			else
			{
				var currentUser = _dbContext.Users.Find(userId);
				if (currentUser == null) throw new NoContentException(nameof(currentUser));
				model.PhoneExtension = currentUser.PhoneExtension;
				if (currentUser.PhoneNumber?.Length == 14 &&
				   (currentUser.PhoneNumber.Substring(0, 1) == "(" && currentUser.PhoneNumber.Substring(4, 2) == ") " && currentUser.PhoneNumber.Substring(9, 1) == "-"))
				{
					model.PhoneAreaCode = currentUser.PhoneNumber.Substring(1, 3);
					model.PhoneExchange = currentUser.PhoneNumber.Substring(6, 3);
					model.PhoneNumber = currentUser.PhoneNumber.Substring(10, 4);
				}

				if (currentUser.PhysicalAddress != null)
				{
					model.PhysicalAddressLine1 = currentUser.PhysicalAddress.AddressLine1;
					model.PhysicalAddressLine2 = currentUser.PhysicalAddress.AddressLine2;
					model.PhysicalCity = currentUser.PhysicalAddress.City;
					model.PhysicalRegionId = currentUser.PhysicalAddress.Region.Id;
					model.PhysicalPostalCode = currentUser.PhysicalAddress.PostalCode;
					model.PhysicalCountryId = currentUser.PhysicalAddress.Country.Id;
				}

				if (currentUser.MailingAddressId == currentUser.PhysicalAddressId)
				{
					model.MailingAddressSameAsBusiness = true;
				}
				else
				{
					model.MailingAddressSameAsBusiness = false;

					if (currentUser.MailingAddress != null)
					{
						model.MailingAddressLine1 = currentUser.MailingAddress.AddressLine1;
						model.MailingAddressLine2 = currentUser.MailingAddress.AddressLine2;
						model.MailingCity = currentUser.MailingAddress.City;
						model.MailingRegionId = currentUser.MailingAddress.Region.Id;
						model.MailingPostalCode = currentUser.MailingAddress.PostalCode;
						model.MailingCountryId = currentUser.MailingAddress.Country.Id;
					}
				}

				model.Subscribe = currentUser.IsSubscriberToEmail;
				model.Title = currentUser.JobTitle;
			}

			model.Provinces = _staticDataService.GetProvinces().Select(x => new KeyValuePair<string, string>(x.Id.ToString(CultureInfo.InvariantCulture), x.Name)).ToArray();
			return model;
		}

		/// <summary>
		/// Get the external user with the specified BCeID.
		/// </summary>
		/// <param name="userGuid"></param>
		/// <returns></returns>
		public User GetUser(Guid userGuid)
		{
			return _dbContext.Users.SingleOrDefault(x => x.BCeIDGuid == userGuid);
		}

		/// <summary>
		/// Get the external user with the specified Id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public User GetUser(int id)
		{
			return _dbContext.Users.Find(id);
		}

		/// <summary>
		/// Get the internal user with the specified Id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public InternalUser GetInternalUser(int id)
		{
			return _dbContext.InternalUsers.Find(id);
		}

		/// <summary>
		/// Get the internal user with the specified user name.
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		public InternalUser GetInternalUser(string userName)
		{
			return _dbContext.InternalUsers.SingleOrDefault(u => String.Compare(u.IDIR, userName, true) == 0);
		}

		/// <summary>
		/// Register the external user with the application.
		/// </summary>
		/// <param name="newUser"></param>
		/// <returns></returns>
		public User RegisterUser(User newUser)
		{
			//
			// Launched to save the user, on Step2 create profile page
			//
			if (newUser.Organization?.BCeIDGuid != null)
			{
				var organization = _dbContext.Organizations.SingleOrDefault(o => o.BCeIDGuid == newUser.Organization.BCeIDGuid);

				// If the Organization already exists, use it instead of the one provided by SiteMinder.
				if (organization != null)
					newUser.Organization = organization;
			}

			_dbContext.Users.Add(newUser);
			_dbContext.CommitTransaction();

			return newUser;
		}

		/// <summary>
		/// Update the external user.
		/// </summary>
		/// <param name="user"></param>
		public void Update(User user)
		{
			_dbContext.Update(user);
			_dbContext.CommitTransaction();
		}

		/// <summary>
		/// Get the external BCeID user with the specified user Id by making a request to the BCeID web services.
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public User GetBCeIDUser(string userId)
		{
			return _bceIdService.GetBusinessUserInfo(userId, _siteMinderService.CurrentUserGuid, _siteMinderService.CurrentUserType);
		}

		/// <summary>
		/// Get the external BCeID user with the specified user Id by making a request to the BCeID web services.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="requester"></param>
		/// <param name="requesterAccountType"></param>
		/// <returns></returns>
		public User GetBCeIDUser(string userId, Guid requester, BCeIDAccountTypeCodes requesterAccountType)
		{
			return _bceIdService.GetBusinessUserInfo(userId, requester, requesterAccountType);
		}

		/// <summary>
		/// Get the external BCeID user with the specified BCeID by making a request to the BCeID web services.
		/// </summary>
		/// <param name="userGuid"></param>
		/// <returns></returns>
		public User GetBCeIDUser(Guid userGuid)
		{
			return _bceIdService.GetBusinessUserInfo(userGuid, _siteMinderService.CurrentUserGuid, _siteMinderService.CurrentUserType);
		}

		public List<int> GetBCeIDUsers()
		{
			return _dbContext.Users.AsNoTracking().Where(x => x.AccountType == AccountTypes.External).Select(x => x.Id).ToList();
		}

		/// <summary>
		/// Get the external BCeID user with the specified BCeID by making a request to the BCeID web services.
		/// </summary>
		/// <param name="userGuid"></param>
		/// <param name="requester"></param>
		/// <param name="requesterAccountType"></param>
		/// <returns></returns>
		public User GetBCeIDUser(Guid userGuid, Guid requester, BCeIDAccountTypeCodes requesterAccountType)
		{
			return _bceIdService.GetBusinessUserInfo(userGuid, requester, requesterAccountType);
		}

		/// <summary>
		/// Get the external IDIR user with the specified BCeID by making a request to the BCeID web services.
		/// </summary>
		/// <param name="userGuid"></param>
		/// <returns></returns>
		public User GetIDIRUser(Guid userGuid)
		{
			return _bceIdService.GetInternalUserInfo(userGuid, _siteMinderService.CurrentUserGuid, _siteMinderService.CurrentUserType);
		}

		/// <summary>
		/// Get the <typeparamref name="AccountType"/> of the currently logged in user [Internal, External].
		/// </summary>
		/// <returns>The currently logged in user account type [Internal, External].</returns>
		public AccountTypes GetAccountType()
		{
			return _siteMinderService.CurrentUserType == BCeIDAccountTypeCodes.Internal ? AccountTypes.Internal : AccountTypes.External;
		}

		/// <summary>
		/// Get the external IDIR user with the specified IDIR by making a request to the BCeID web services.
		/// </summary>
		/// <param name="idir"></param>
		/// <returns></returns>
		public User GetIDIRUser(string idir)
		{
			return _bceIdService.GetInternalUserInfo(idir, _siteMinderService.CurrentUserGuid);
		}

		/// <summary>
		/// Update the currently logged in user's email by making a request to the BCeID web services.
		/// </summary>
		public bool UpdateUserFromBCeIDAccount(User currentUser = null)
		{
			try
			{
				if (currentUser == null)
					// get the current user from the db
					currentUser = GetUser(_siteMinderService.CurrentUserGuid);

				// get the BCeID user from the service
				bool.TryParse(ConfigurationManager.AppSettings["BCeIDWebService_Simulator"], out bool simulator);

				if (!simulator)
				{
					var bceIdUser = GetBCeIDUser(_siteMinderService.CurrentUserGuid);

					if (bceIdUser == null)
						throw new InvalidOperationException($"Can\'t find BCeID user based on user GUID: {_siteMinderService.CurrentUserGuid}");

					if (currentUser == null)
						return true;

					if (CopyBceIDUserOrganizationValues(bceIdUser, currentUser))
					{
						currentUser.Organization.RequiredProfileUpdate = true;
						Update(currentUser);
					}
				}

				return true;
			}
			catch (Exception ex)
			{
				_logger.Debug($"Unable to update User and Organization - '{_siteMinderService.CurrentUserName}':{_siteMinderService.CurrentUserGuid}. [Error: {ex.Message}]");
				return false;
			}
		}

		public bool SyncUserFromBCeIDAccount(User user)
		{
			// get the BCeID user from the service
			var bceIdUser = _bceIdService.GetBusinessUserInfo(user.BCeIDGuid, user.BCeIDGuid, BCeIDAccountTypeCodes.Business);

			if (bceIdUser == null)
				throw new InvalidOperationException($"Can't find BCeID user based on user GUID: {user.BCeIDGuid}");

			if (user == null)
				throw new ArgumentNullException($"Can't find user based on user GUID: {user.BCeIDGuid}");

			if (CopyBceIDUserOrganizationValues(bceIdUser, user))
			{
				user.Organization.RequiredProfileUpdate = true;
				Update(user);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Synchronize the organization information from BCeID with STG.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public bool SyncOrganizationFromBCeIDAccount(User user)
		{
			// get the BCeID user from the service
			var bceIdUser = _bceIdService.GetBusinessUserInfo(user.BCeIDGuid, user.BCeIDGuid, BCeIDAccountTypeCodes.Business);
			if (bceIdUser == null)
				throw new InvalidOperationException($"Can't find BCeID user based on user GUID: {user.BCeIDGuid}");

			// If the organization in BCeID doesn't match STG, then update.
			if (bceIdUser.Organization?.BCeIDGuid != user.Organization?.BCeIDGuid)
			{
				// Check if it exists in the database.
				// Assign the correct organization to the user.
				var organization = bceIdUser.Organization != null ? _dbContext.Organizations.FirstOrDefault(o => o.BCeIDGuid == bceIdUser.Organization.BCeIDGuid) : null;

				// Only one user can be the administrator.
				user.IsOrganizationProfileAdministrator = organization == null ? true : !_dbContext.Users.Any(u => u.OrganizationId == organization.Id && u.IsOrganizationProfileAdministrator);
				user.Organization = organization ?? user.Organization ?? bceIdUser.Organization;
				user.OrganizationId = organization?.Id ?? user.Organization?.Id ?? bceIdUser.Organization.Id;

				CopyBceIDOrganizationValues(bceIdUser, user);
				user.Organization.RequiredProfileUpdate = true;
				Update(user);
				return true;
			}
			else if (CopyBceIDOrganizationValues(bceIdUser, user))
			{
				user.Organization.RequiredProfileUpdate = true;
				Update(user);
				return true;
			}
			return false;
		}

		internal bool CopyBceIDUserOrganizationValues(User bceidUser, User targetUser)
		{
			if (bceidUser == null)
				throw new ArgumentNullException(nameof(bceidUser));

			if (targetUser == null)
				throw new ArgumentNullException(nameof(targetUser));

			// copy specific values for user if diferent
			var hasDifferentUserValues = CopyBceIDUserValues(bceidUser, targetUser);
			var hasDifferentOrganizationValues = SyncOrganizationFromBCeIDAccount(targetUser); // CopyBceIDOrganizationValues(bceidUser, targetUser);

			// update user entity if any values were changed
			return hasDifferentUserValues || hasDifferentOrganizationValues;
		}

		private bool CopyBceIDUserValues(User bceidUser, User targetUser)
		{
			if (bceidUser == null)
				throw new ArgumentNullException(nameof(bceidUser));

			if (targetUser == null)
				throw new ArgumentNullException(nameof(targetUser));

			// copy specific values for user if different
			// update user entity if any values were changed
			return bceidUser.CopyPropertiesTo(targetUser, o => new
			{
				o.FirstName,
				o.LastName,
				o.EmailAddress
			});
		}

		private bool CopyBceIDOrganizationValues(User bceidUser, User targetUser)
		{
			if (bceidUser == null)
				throw new ArgumentNullException(nameof(bceidUser));

			if (targetUser == null)
				throw new ArgumentNullException(nameof(targetUser));

			if (bceidUser.Organization == null) bceidUser.Organization = new Organization();

			return bceidUser.Organization.CopyPropertiesTo(targetUser.Organization, o => new
			{
				o.BCeIDGuid,
				o.LegalName,
				o.DoingBusinessAs,
				o.BusinessNumber,
				o.BusinessNumberVerified,
				o.BusinessType,
				o.StatementOfRegistrationNumber,
				o.IncorporationNumber,
				o.JurisdictionOfIncorporation
			});
		}

		public IEnumerable<User> GetUsersForOrganization(int organizationId)
		{
			return _dbContext.Users.AsNoTracking().Where(o => o.OrganizationId == organizationId).ToArray();
		}
		#endregion
	}
}

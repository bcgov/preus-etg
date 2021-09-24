using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.BCeID.WebService.BCeID;
using System;
using NLog;
using BCeIDAccountTypeCode = CJG.Infrastructure.BCeID.WebService.BCeID.BCeIDAccountTypeCode;

namespace CJG.Infrastructure.BCeID.WebService
{
	/// <summary>
	/// <typeparamref name="BCeIDRepository"/> class, provides a way to makes requests to the BCeID web service.
	/// </summary>
	public class BCeIDService : IBCeIDService
	{
		#region Variables
		protected readonly ILogger _logger = LogManager.GetCurrentClassLogger();
		#endregion

		/// <summary>
		/// Make a request to the BCeID web service for account information for the specified 'userId'.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="requesterUserId"></param>
		/// <param name="requesterAccountType"></param>
		/// <returns></returns>
		public User GetInternalUserInfo(Guid userId, Guid requesterUserId, BCeIDAccountTypeCodes requesterAccountType)
		{
			return GetUserInfo(userId, BCeIDAccountTypeCodes.Internal, requesterUserId, requesterAccountType);
		}

		/// <summary>
		/// Make a request to the BCeID web service for account information for the specified 'userId'.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="requesterUserId"></param>
		/// <param name="requesterAccountType"></param>
		/// <returns></returns>
		public User GetBusinessUserInfo(Guid userId, Guid requesterUserId, BCeIDAccountTypeCodes requesterAccountType)
		{
			return GetUserInfo(userId, BCeIDAccountTypeCodes.Business, requesterUserId, requesterAccountType);
		}

		/// <summary>
		/// Make a request to the BCeID web service for account information for the specified 'userId'.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="requesterUserId"></param>
		/// <param name="requesterAccountType"></param>
		/// <returns></returns>
		public User GetBusinessUserInfo(string userId, Guid requesterUserId, BCeIDAccountTypeCodes requesterAccountType)
		{
			AccountDetailResponse response = null;

			using (var client = new BCeIDServiceSoapClient("BCeIDServiceSoap"))
			{
				client.ClientCredentials.UserName.UserName = Configuration.AppSettings.BCeIDWebService_UserName;
				client.ClientCredentials.UserName.Password = Configuration.AppSettings.BCeIDWebService_Password;

				var request = new AccountDetailRequest
				{
					onlineServiceId = Configuration.AppSettings.BCeIDWebService_OnlineServiceId,
					userId = userId,
					accountTypeCode = (BCeIDAccountTypeCode)BCeIDAccountTypeCodes.Business,
					requesterUserGuid = requesterUserId.ToString("N"),
					requesterAccountTypeCode = (BCeIDAccountTypeCode)requesterAccountType
				};

				_logger.Debug($"BCeID Account Detail Request - Service Id:{Configuration.AppSettings.BCeIDWebService_OnlineServiceId}, User Id:{userId}, Account Type:{((BCeIDAccountTypeCode)BCeIDAccountTypeCodes.Business).ToString("g")}, Requester:{requesterUserId.ToString("N")}, Requester Account Type:{((BCeIDAccountTypeCode)requesterAccountType).ToString("g")}");
				//response = client.getAccountDetail(request);
				var action = new Func<AccountDetailResponse>(() =>
				{
					return client.getAccountDetail(request);
				});

				if (!action.TryExecute(Configuration.AppSettings.BCeIDWebService_Timeout, out response))
				{
					throw new TimeoutException("Forced timeout while attempting to request BCeID account details.");
				}
			}

			if (response.code == ResponseCode.Success)
			{
				return GetUser(response.account);
			}

			throw new BCeIDException(response);
		}

		/// <summary>
		/// Make a request to the BCeID web service for account information for the specified 'userId'.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="accountType"></param>
		/// <param name="requesterUserId"></param>
		/// <param name="requesterAccountType"></param>
		/// <returns></returns>
		private User GetUserInfo(
			Guid userId,
			BCeIDAccountTypeCodes accountType,
			Guid requesterUserId,
			BCeIDAccountTypeCodes requesterAccountType)
		{
			AccountDetailResponse response = null;

			using (var client = new BCeIDServiceSoapClient("BCeIDServiceSoap"))
			{
				client.ClientCredentials.UserName.UserName = Configuration.AppSettings.BCeIDWebService_UserName;
				client.ClientCredentials.UserName.Password = Configuration.AppSettings.BCeIDWebService_Password;

				var request = new AccountDetailRequest
				{
					onlineServiceId = Configuration.AppSettings.BCeIDWebService_OnlineServiceId,
					userGuid = userId.ToString("N"),
					accountTypeCode = (BCeIDAccountTypeCode)accountType,
					requesterUserGuid = requesterUserId.ToString("N"),
					requesterAccountTypeCode = (BCeIDAccountTypeCode)requesterAccountType
				};

				_logger.Debug($"BCeID Account Detail Request - Service Id:{Configuration.AppSettings.BCeIDWebService_OnlineServiceId}, User Guid:{userId.ToString("N")}, Account Type:{((BCeIDAccountTypeCode)Core.Entities.BCeIDAccountTypeCodes.Business).ToString("g")}, Requester:{requesterUserId.ToString("N")}, Requester Account Type:{((BCeIDAccountTypeCode)requesterAccountType).ToString("g")}");
				response = client.getAccountDetail(request);
			}

			if (response.code == ResponseCode.Success)
			{
				return GetUser(response.account);
			}

			throw new BCeIDException(response);
		}

		/// <summary>
		/// Initialize a new instance of a user with the BCeID account information.
		/// </summary>
		/// <param name="account"></param>
		/// <returns></returns>
		private static User GetUser(BCeIDAccount account)
		{
			var user = new User
			{
				BCeID = account.bceidDluid.value,
				BCeIDGuid = Guid.Parse(account.guid.value),
				AccountType = account.type.code == BCeIDAccountTypeCode.Internal ? AccountTypes.Internal : AccountTypes.External,
				FirstName = account.individualIdentity.name.firstname.value,
				LastName = account.individualIdentity.name.surname.value,
				EmailAddress = account.contact.email.value,
				PhoneNumber = account.contact.telephone.value
			};

			if (account.business?.guid?.value != "")
			{
				user.Organization = GetOrganization(account.business);
			}

			return user;
		}

		/// <summary>
		/// Create a new Organization entity and populate it with the information from BCeID.
		/// </summary>
		/// <param name="bceidBusiness"></param>
		/// <returns></returns>
		private static Organization GetOrganization(BCeIDBusiness bceidBusiness)
		{
			var organization = new Organization();
			organization.BCeIDGuid = Guid.Parse(bceidBusiness.guid.value);
			organization.LegalName = bceidBusiness.legalName.value;
			organization.DoingBusinessAs = bceidBusiness.doingBusinessAs.value;
			organization.BusinessNumber = bceidBusiness.businessNumber.value;
			organization.BusinessNumberVerified = bceidBusiness.businessNumberVerifiedFlag.value;
			organization.BusinessType = bceidBusiness.bnHubBusinessType.name;
			organization.StatementOfRegistrationNumber = bceidBusiness.statementOfRegistrationNumber.value;
			organization.IncorporationNumber = bceidBusiness.incorporationNumber.value;
			organization.JurisdictionOfIncorporation = bceidBusiness.jurisdictionOfIncorporation.value;
			return organization;
		}

		/// <summary>
		/// Make a request to the BCeID web service for account information for the specified 'idir'.
		/// </summary>
		/// <param name="idir"></param>
		/// <param name="requesterUserGuid"></param>
		/// <returns></returns>
		public User GetInternalUserInfo(string idir, Guid requesterUserGuid)
		{
			AccountDetailResponse response;

			using (var client = new BCeIDServiceSoapClient("BCeIDServiceSoap"))
			{
				client.ClientCredentials.UserName.UserName = Configuration.AppSettings.BCeIDWebService_UserName;
				client.ClientCredentials.UserName.Password = Configuration.AppSettings.BCeIDWebService_Password;

				var request = new AccountDetailRequest
				{
					onlineServiceId = Configuration.AppSettings.BCeIDWebService_OnlineServiceId,
					userId = idir,
					accountTypeCode = BCeIDAccountTypeCode.Internal,
					requesterUserGuid = requesterUserGuid.ToString("N"),
					requesterAccountTypeCode = BCeIDAccountTypeCode.Internal
				};

				response = client.getAccountDetail(request);
			}

			return response.code == ResponseCode.Success ? GetUser(response.account) : null;
		}
	}
}
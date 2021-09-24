using CJG.Application.Business.Models;
using CJG.Core.Entities;
using System;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
	public interface IUserService : IService
	{
		User GetUser(Guid userGuid);
		User GetUser(int id);
		InternalUser GetInternalUser(int id);
		InternalUser GetInternalUser(string userName);
		User RegisterUser(User newUser);
		void Update(User user);
		List<int> GetBCeIDUsers();
		User GetBCeIDUser(Guid userGuid);

		User GetBCeIDUser(Guid userGuid, Guid requester, BCeIDAccountTypeCodes requesterAccountType);

		User GetBCeIDUser(string userId);

		User GetBCeIDUser(string userId, Guid requester, BCeIDAccountTypeCodes requesterAccountType);

		User GetIDIRUser(Guid userGuid);
		User GetIDIRUser(string idir);
		AccountTypes GetAccountType();
		bool UpdateUserFromBCeIDAccount(User currentUser = null);
		bool SyncUserFromBCeIDAccount(User user);
		bool SyncOrganizationFromBCeIDAccount(User user);
		UserProfileConfirmationModel GetConfirmationDetails(Guid guid);
		UserProfileDetailModel GetUserProfileDetails(int id);
		void CreateUserProfile(Guid guid, UserProfileDetailModel userProfileDetails);
		void UpdateUserProfile(int userId, UserProfileDetailModel userProfileDetails);
		void UpdateUserDetails(int userId, UserProfileDetailModel userProfileDetails, bool standAloneTransaction = true);

		IEnumerable<User> GetUsersForOrganization(int organizationId);
	}
}

using CJG.Core.Entities;
using System;

namespace CJG.Core.Interfaces.Service
{
	public interface IBCeIDService
	{
		User GetInternalUserInfo(Guid userGuid, Guid requesterUserId, BCeIDAccountTypeCodes requesterAccountType);

		User GetInternalUserInfo(string idir, Guid requesterUserGuid);

		User GetBusinessUserInfo(Guid userGuid, Guid requesterUserId, BCeIDAccountTypeCodes requesterAccountType);

		User GetBusinessUserInfo(string userId, Guid requesterUserId, BCeIDAccountTypeCodes requesterAccountType);
	}
}
using System;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;

namespace CJG.Infrastructure.NotificationService.ServiceMocks
{
	public class MockedIBCeIDService : IBCeIDService
	{
		public User GetInternalUserInfo(Guid userGuid, Guid requesterUserId, BCeIDAccountTypeCodes requesterAccountType)
		{
			return null;
		}

		public User GetInternalUserInfo(string idir, Guid requesterUserGuid)
		{
			return null;
		}

		public User GetBusinessUserInfo(Guid userGuid, Guid requesterUserId, BCeIDAccountTypeCodes requesterAccountType)
		{
			return null;
		}

		public User GetBusinessUserInfo(string userId, Guid requesterUserId, BCeIDAccountTypeCodes requesterAccountType)
		{
			return null;
		}
	}
}
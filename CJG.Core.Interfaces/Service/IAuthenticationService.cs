using CJG.Core.Entities;
using System;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
	public interface IAuthenticationService : IService
	{
		List<KeyValuePair<string, string>> GetLogInOptions(AccountTypes accountType);
		void LogIn(Guid userGuid, string userType);
		void Refresh(User user);
		void LogInInternal(Guid userGuid);
		string LogOut();
	}
}

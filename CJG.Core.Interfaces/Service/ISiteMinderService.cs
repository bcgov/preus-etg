using CJG.Core.Entities;
using System;

namespace CJG.Core.Interfaces.Service
{
    public interface ISiteMinderService
    {
        Guid CurrentUserGuid { get; }
        string CurrentUserName { get; }
        BCeIDAccountTypeCodes CurrentUserType { get; }
        void LogIn(Guid userGuid, string userType);
        void LogInInternal(Guid userGuid);
        void LogOut();
    }
}

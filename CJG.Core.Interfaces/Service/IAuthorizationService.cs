using System.Collections.Generic;
using System.Security.Claims;
using CJG.Core.Entities;

namespace CJG.Core.Interfaces.Service
{
    public interface IAuthorizationService : IService
    {
        IEnumerable<InternalUser> GetAssessors();
        IEnumerable<string> GetPrivileges(string roleName);
        void UpdatePrivilegeClaimsOnIdentity(ClaimsIdentity identity);
        IEnumerable<string> GetRolesWithPrivilege(string privilegeName);
    }
}
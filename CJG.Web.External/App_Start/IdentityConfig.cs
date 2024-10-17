using System.Security.Claims;
using System.Threading.Tasks;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.EF;
using CJG.Infrastructure.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using NLog;
using CJG.Infrastructure.Entities;
using Microsoft.Owin.Security.Cookies;

namespace CJG.Web.External
{
    public class ApplicationUserStore : UserStore<ApplicationUser>
    {
        public ApplicationUserStore(IDataContext context)
            : base(context.Context)
        {
        }
    }

    public class ApplicationRoleStore : RoleStore<ApplicationRole>
    {
        public ApplicationRoleStore(IDataContext context)
            : base(context.Context)
        {
        }
    }

    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> store)
            : base(store)
        {
        }
    }

    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        private readonly IAuthorizationService _authorizationService;

        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager, IAuthorizationService authorizationService)
            : base(userManager, authenticationManager)
        {
            _authorizationService = authorizationService;
        }

        public override async Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            var claimsIdentity = await user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);

            _authorizationService.UpdatePrivilegeClaimsOnIdentity(claimsIdentity);

            return claimsIdentity;
        }
    }

    public class IdentityConfig
    {
        #region Variables
        static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext(() => new DataContext());

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie
            });
        }
    }
}
using System.Web;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using System.Web.Mvc;
using CJG.Application.Services;

namespace CJG.Web.External.Views.Shared
{
    public abstract class SiteMinderViewPage : WebViewPage
    {
        public IUserService UserService { get; set; }

        public ISiteMinderService SiteMinderService { get; set; }

        private User GetCurrentUser()
        {
            return UserService.GetUser(SiteMinderService.CurrentUserGuid);
        }

        public dynamic GetUserName()
        {
            if (HttpContext.Current != null)
            {
                var firstName = HttpContext.Current.User.GetFirstName();
                var lastName = HttpContext.Current.User.GetLastName();

                return string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName)
                    ? null
                    : $"{firstName} {lastName}";
            }

            var currentUser = GetCurrentUser();
            return currentUser != null ? $"{currentUser.FirstName} {currentUser.LastName}" : null;
        }

        public dynamic GetOrganizationName()
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.User.GetOrganizationLegalName();
            }

            return GetCurrentUser()?.Organization?.LegalName;
        }

        public bool ShowLoginLink()
        {
            if (Request.Url.PathAndQuery.StartsWith("/part/", System.StringComparison.OrdinalIgnoreCase) || SiteMinderService.CurrentUserGuid != System.Guid.Empty)
                return false;

            return true;
        }
    }
}

using System;
using System.Web.Mvc;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using NLog;
using Microsoft.Owin.Security;
using System.Security.Principal;
using System.Net;

namespace CJG.Web.External.Helpers.Filters
{
    /// <summary>
    /// <typeparamref name="IdentitySignOnFilterAttribute"/> class, provides a filter that automatically signs in users who have been authenticated by SiteMinder.
    /// </summary>
    public class IdentitySignOnFilterAttribute : IAuthorizationFilter
    {
        #region Variables

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Properties
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="IdentitySignOnFilterAttribute"/> object.
        /// </summary>
        public IdentitySignOnFilterAttribute()
        {
        }
        #endregion

        #region Methods
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            OnExecuting(filterContext);
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            OnExecuting(filterContext);
        }

        /// <summary>
        /// If SiteMinder has logged the user in then automatically sign-in the user
        /// </summary>
        /// <param name="filterContext"></param>
        protected void OnExecuting(ControllerContext filterContext)
        {
            var siteMinderService = DependencyResolver.Current.GetService<ISiteMinderService>() as ISiteMinderService;

            if (filterContext.IsInternal() && siteMinderService.CurrentUserGuid != Guid.Empty)
            {
                // Internal application will automatically sign the user in with FormsAuthentication if SiteMinder has already set the appropriate Session variables.
                var userManager = DependencyResolver.Current.GetService<ApplicationUserManager>();
                var user = userManager.FindById(siteMinderService.CurrentUserGuid.ToString());

                if (user != null
                    && user.Active.GetValueOrDefault()
                    && !filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    // Login with the internal user account automatically.
                    var signInManager = DependencyResolver.Current.GetService<ApplicationSignInManager>(); ;
                    signInManager.SignIn(user, true, false);

                    var authorization = filterContext as AuthorizationContext;
                    if (filterContext is ActionExecutedContext action && filterContext.HttpContext.Request.Url != null)
                        action.Result = new RedirectResult(filterContext.HttpContext.Request.Url.ToString());
                    else if (authorization != null && filterContext.HttpContext.Request.Url != null)
                        authorization.Result = new RedirectResult(filterContext.HttpContext.Request.Url.ToString());

                    // _logger.Debug($"SiteMinder authenticated user '{user.UserName}'.");
                }
                else if (user != null
                    && !user.Active.GetValueOrDefault())
                {
                    _logger.Debug($"SiteMinder User is no longer active in the database '{siteMinderService.CurrentUserGuid}':'{siteMinderService.CurrentUserName}'.");

                    if (filterContext is AuthorizationContext authorization)
                    {
                        if (authorization != null && authorization.ActionDescriptor.ActionName == "LogOut" && filterContext.HttpContext.Request.Url != null)
                        {
                            filterContext.HttpContext.User = null;
                            //authorization.Result = new RedirectResult(filterContext.HttpContext.Request.Url.ToString());
                        }
                        else
                        {
                            // force logout
                            siteMinderService.LogOut();
                            var authenticationManager = DependencyResolver.Current.GetService<IAuthenticationManager>();
                            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                            System.Web.Security.FormsAuthentication.SignOut();
                            filterContext.HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
                            filterContext.Controller.TempData["ForceLogout"] = true;
                            authorization.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden, "You are not permitted to perform this action").HttpStatusCodeResultWithAlert(filterContext.HttpContext.Response, AlertType.Warning);
                        }
                    }
                }
                else if (user == null)
                {
                    _logger.Debug($"SiteMinder User does not exist in database '{siteMinderService.CurrentUserGuid}':'{siteMinderService.CurrentUserName}'.");

                    // force logout
                    siteMinderService.LogOut();
                    var authenticationManager = DependencyResolver.Current.GetService<IAuthenticationManager>();
                    authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                }
            }
            else if (filterContext.IsExternal())
            {
                // Internally we maintain the user information in a Session variable.
                // External application will automatically sign the user in with Sessions if SiteMinder has already set the appropriate Session variables.
                var user = filterContext.HttpContext.Session["User"] as IPrincipal;
                if (siteMinderService.CurrentUserGuid != Guid.Empty
                    && !filterContext.HttpContext.User.Identity.IsAuthenticated
                    && user == null)
                {
                    _logger.Debug($"Automatic Identity Sign-On - external user: '{siteMinderService.CurrentUserName}', '{siteMinderService.CurrentUserGuid}'");
                    // Automatically login the external user.
                    var authenticationService = DependencyResolver.Current.GetService<IAuthenticationService>();
                    authenticationService.LogIn(siteMinderService.CurrentUserGuid, "Business");
                }
                else if (user != null)
                {
                    // Reset current authentication from session.
                    filterContext.HttpContext.User = user;
                }
            }
        }
        #endregion
    }
}
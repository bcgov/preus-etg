using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NLog;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;

namespace CJG.Web.External.Helpers.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AuthorizeActionAttribute : AuthorizeAttribute
    {
        #region Variables
        private readonly ApplicationWorkflowTrigger? _trigger;
        private readonly Privilege[] _privileges;
        private readonly IAuthorizationService _authorizationService;
        private readonly IGrantApplicationService _grantApplicationService;
        private readonly ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
        private bool redirect = false;
        #endregion

        #region Constructors
        public AuthorizeActionAttribute()
        {
            _authorizationService = DependencyResolver.Current.GetService<IAuthorizationService>();

            if (_authorizationService == null)
            {
                throw new ApplicationException("Can't create instance of IAuthorizationService");
            }

            _grantApplicationService = DependencyResolver.Current.GetService<IGrantApplicationService>();

            if (_grantApplicationService == null)
            {
                throw new ApplicationException("Can't create instance of IGrantApplicationService");
            }
        }

        public AuthorizeActionAttribute(params Privilege[] privileges) : base()
        {
            _privileges = privileges;
        }

        public AuthorizeActionAttribute(ApplicationWorkflowTrigger trigger, params Privilege[] privileges) : this(privileges)
        {
            _trigger = trigger;
        }
        #endregion

        #region Methods

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var privileges = httpContext.User.GetPrivileges();

            // _logger.Debug($"User '{httpContext.User.Identity?.Name}' Privileges: {(privileges.Count() > 0 ? privileges.Select(p => p.ToString()).Aggregate((a, b) => $"{a}, {b}") : null)}");

            return privileges != null && httpContext.User.HasPrivilege(_privileges) && base.AuthorizeCore(httpContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                _logger.Info("Handling unauthenticated request.");
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                if (!redirect)
                {
                    _logger.Info("Handling unauthorized request.");
                    filterContext.Controller.TempData["Message"] = "You don't have enough access privileges.";
                    filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { controller = "Home", action = "Index" }));
                }
            }
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            try
            {
                int grantApplicationId = 0;

                // if the grant application id isn't a parameter, catch the exception and continue
                var parameter = filterContext.Controller.ValueProvider.GetValue("grantApplicationId")?.AttemptedValue;
                
                if (int.TryParse(parameter, out grantApplicationId) && grantApplicationId != 0)
                {
                    var grantApplication = _grantApplicationService.Get(grantApplicationId);

                    var isAuthorized = filterContext.HttpContext.User.HasPrivilege(_privileges);

                    if (_trigger.HasValue)
                    {
                        isAuthorized = filterContext.HttpContext.User.CanPerformAction(grantApplication, _trigger.Value);
                    }

                    if (!isAuthorized)
                    {
                        _logger.Error($"User is not authorized to perform action - '{filterContext.HttpContext.User.GetUserName()}':{filterContext.GetAction()}.");
                        filterContext.Controller.TempData["Message"] = "You don't have enough access privileges.";
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Application", action = "ApplicationDetails", @id = grantApplicationId }));
                        redirect = true;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            base.OnAuthorization(filterContext);
        }
        #endregion
    }
}
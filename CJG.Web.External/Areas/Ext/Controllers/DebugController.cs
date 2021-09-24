using System.Web.Mvc;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Ext.Controllers
{
    /// <summary>
    /// DebugController class, MVC controller for debugging the external site.
    /// </summary>
    [ExternalFilter]
	public class DebugController : BaseController
	{
		#region Variables
		private readonly ISiteMinderService _siteMinderService;
		private readonly IUserService _userService;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a DebugController object.
		/// </summary>
		public DebugController(
			IControllerService controllerService) : base(controllerService.Logger)
		{
			_userService = controllerService.UserService;
			_siteMinderService = controllerService.SiteMinderService;
		}
		#endregion

		#region Endpoints
		/// <summary>
		/// Displays the current user SiteMinder information.
		/// </summary>
		/// <returns></returns>
		public ActionResult SiteMinder()
		{
			var model = new DebugViewModel(_userService.GetUser(_siteMinderService.CurrentUserGuid), new SiteMinderInfoViewModel(_siteMinderService.CurrentUserGuid, _siteMinderService.CurrentUserType));

			return View(model);
		}

		/// <summary>
		/// Display information about the application.
		/// </summary>
		/// <returns></returns>
		public ActionResult About()
		{
			var model = new AboutViewModel
			{
				Version = typeof(MvcApplication).Assembly.GetName().Version,
				Now = AppDateTime.Now
			};
			return View(model);
		}
		#endregion
	}
}
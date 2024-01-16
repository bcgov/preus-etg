using System;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using CJG.Core.Entities;
using CJG.Core.Entities.Attributes;
using CJG.Core.Interfaces;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NLog;

namespace CJG.Web.External
{
    /// <summary>
    /// <typeparamref name="MvcApplication"/> class, provides a way to configuration the application.
    /// </summary>
    public class MvcApplication : HttpApplication
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Register and configuration the application.
		/// </summary>
		protected void Application_Start()
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			Database.SetInitializer<IdentityDbContext<ApplicationUser>>(null);
			DependencyConfig.Configure(new ContainerBuilder(), typeof(MvcApplication).Assembly);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			MvcHandler.DisableMvcResponseHeader = true;
			Logger.Info("CJG Web Application Starting");

			// Makes it possible for Windows Authentication to work along side Form-based authentication.
			System.Security.Claims.ClaimsPrincipal.PrimaryIdentitySelector =
				(i) => i.FirstOrDefault(ci => ci.GetType() == typeof(System.Security.Claims.ClaimsIdentity));

			// ...The reason why you need this is the way RegularExpressionAttribute is implemented. It doesn't implement IClientValidatable 
			// interface but it rather has a RegularExpressionAttributeAdapter which is associated to it...
			// https://stackoverflow.com/questions/18666812/trying-to-inherit-regularexpressionattribute-no-longer-validates
			DataAnnotationsModelValidatorProvider.RegisterAdapter(
				typeof(NameValidationAttribute),
				typeof(RegularExpressionAttributeAdapter)
			);
		}

		/// <summary>
		/// Get the application date and time from the datasource.  This is used for testing time based scenarios.
		/// </summary>
		private void SetAppDateTime(ISettingService service)
		{
			var appDateTime = service.Get("AppDateTime")?.Value;
			if (appDateTime != null && DateTime.TryParse(appDateTime, CultureInfo.CreateSpecificCulture("en-CA"), DateTimeStyles.AssumeLocal, out DateTime now))
			{
				AppDateTime.SetNow(now);
			}
		}

		/// <summary>
		/// Handle all unhandled exceptions.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Application_Error(object sender, EventArgs e)
		{
			var exception = Server.GetLastError();

			Logger.Fatal(exception, "Caught unhandled exception: {0} - {1}", exception.Message, exception);

			if (IsDevelopmentMode())
			{
				Response.Write("<h2>Application Error</h2>\n");
				Response.Write("<p>" + exception.GetAllMessages() + "</p>\n");
				Response.Write("<p>" + exception + "</p>\n");
			}

			if (exception is HttpRequestValidationException)
			{
				var context = HttpContext.Current;
				context.Server.ClearError();
				Response.Clear();
				Response.StatusCode = 500;
				Response.AddHeader("Error-Type", "HttpRequestValidationException");
				Response.Write(@"<html><head></head><body>A potentially dangerous Request.Form value was detected from the client</body></html>");
				Response.End();
			}
		}

		private static bool IsDevelopmentMode()
		{
			return GetAppSettingsFlag("DevelopmentMode");
		}

		/// <summary>
		/// On every request do the following;
		/// - Add X-Frame-Options header.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Application_BeginRequest(object sender, EventArgs e)
		{
			HttpContext.Current.Response.AddHeader("X-Frame-Options", "DENY");

			var settingsService = (ISettingService)DependencyResolver.Current.GetService(typeof(ISettingService));
			if (settingsService == null) return;

			if ( IsDevelopmentMode() )
			{
				SetAppDateTime(settingsService);
			}
		}

		private static bool GetAppSettingsFlag(string flagName)
		{
			var setDateTimeSettings = ConfigurationManager.AppSettings[flagName];
			bool setAppDateTimeFlag;
			return !string.IsNullOrWhiteSpace(setDateTimeSettings) &&
				   bool.TryParse(setDateTimeSettings, out setAppDateTimeFlag) && setAppDateTimeFlag;
		}

		public override void Init()
		{
			base.Init();
			AcquireRequestState += ShowRouteValues;
		}

		protected void ShowRouteValues(object sender, EventArgs e)
		{
			var context = HttpContext.Current;
			if (context == null)
				return;

			var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(context));
		}
	}
}

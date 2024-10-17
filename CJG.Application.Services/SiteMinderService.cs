using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using NLog;
using System;
using System.Web;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="SiteMinderService"/> class, provides a way to login or logout of SiteMinder.
	/// </summary>
	public class SiteMinderService : ISiteMinderService
	{
		#region Variables
		private readonly ILogger _logger;
		private readonly HttpContextBase _httpContext;
		#endregion

		#region Properties
		/// <summary>
		/// This returns the Guid of the currently logged in user. It is assumed that someone is
		/// always logged in, so this never returns null. This should not be used on the log in
		/// page as that's the only page you can visit before being logged in, and even then
		/// only on a developer's workstation, outside of SiteMinder protection.
		/// </summary>
		public Guid CurrentUserGuid
		{
			get
			{
				var value = _httpContext.Request.ServerVariables?[Constants.SiteMinder_UserGuid_ServerVariableName] ?? _httpContext.Session?[Constants.SiteMinderSimulator_UserGuid_SessionVariableName]?.ToString();

				if (!Guid.TryParse(value, out Guid userGuid))
				{
					return Guid.Empty;
				}

				return userGuid;
			}
		}

		/// <summary>
		/// get - The current user's user Id.
		/// </summary>
		public string CurrentUserName
		{
			get
			{
				return _httpContext.Request.ServerVariables?[Constants.SiteMinder_UserName_ServerVariableName] ?? (string)_httpContext.Session?[Constants.SiteMinderSimulator_UserName_SessionVariableName];
			}
		}

		/// <summary>
		/// This returns the account type of the currently logged in user. It is assumed that someone is
		/// always logged in, so this never returns null. This should not be used on the log in
		/// page as that's the only page you can visit before being logged in, and even then
		/// only on a developer's workstation, outside of SiteMinder protection.
		/// </summary>
		public BCeIDAccountTypeCodes CurrentUserType
		{
			get
			{
				var value = _httpContext.Request.ServerVariables?[Constants.SiteMinder_UserType_ServerVariableName] ?? (string)_httpContext.Session?[Constants.SiteMinderSimulator_UserType_SessionVariableName];
				if (!Enum.TryParse<BCeIDAccountTypeCodes>(value, out BCeIDAccountTypeCodes accountType))
				{
					accountType = BCeIDAccountTypeCodes.Void;
				}
				return accountType;
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="SiteMinderService"/> class.
		/// </summary>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public SiteMinderService(HttpContextBase httpContext, ILogger logger)
		{
			_httpContext = httpContext;
			_logger = logger;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Login the external user by adding SiteMinder session variables.
		/// </summary>
		/// <param name="userGuid"></param>
		/// <param name="userType"></param>
		public void LogIn(Guid userGuid, string userType)
		{
			_httpContext.Session[Constants.SiteMinderSimulator_UserGuid_SessionVariableName] = userGuid;
			_httpContext.Session[Constants.SiteMinderSimulator_UserType_SessionVariableName] = userType;
			_logger.Info($"SiteMinder login for '{userGuid}'.");
			_logger.Debug($"SiteMinder Session values; {CurrentUserName}, {CurrentUserType}, {CurrentUserGuid}");
		}

		/// <summary>
		/// Login the internal user by adding SiteMinder session variables.
		/// </summary>
		/// <param name="userName"></param>
		public void LogInInternal(string userName)
		{
			_httpContext.Session[Constants.SiteMinderSimulator_UserName_SessionVariableName] = userName;
			_httpContext.Session[Constants.SiteMinderSimulator_UserType_SessionVariableName] = "Internal";
			_logger.Info($"SiteMinder internal login for '{userName}'.");
			_logger.Debug($"SiteMinder Session values; {CurrentUserName}, {CurrentUserType}, {CurrentUserGuid}");
		}

		/// <summary>
		/// Login the internal user by adding SiteMinder session variables.
		/// </summary>
		/// <param name="userId"></param>
		public void LogInInternal(Guid userId)
		{
			_httpContext.Session[Constants.SiteMinderSimulator_UserGuid_SessionVariableName] = userId;
			_httpContext.Session[Constants.SiteMinderSimulator_UserType_SessionVariableName] = "Internal";
			_logger.Info($"SiteMinder internal login for '{userId}'.");
			_logger.Debug($"SiteMinder Session values; {CurrentUserName}, {CurrentUserType}, {CurrentUserGuid}");
		}

		/// <summary>
		/// Logout the user by clearing the session.
		/// </summary>
		public void LogOut()
		{
			_logger.Info($"SiteMinder logout for '{CurrentUserGuid}'.");
			_httpContext.Session[Constants.SiteMinderSimulator_UserGuid_SessionVariableName] = null;
			_httpContext.Session[Constants.SiteMinderSimulator_UserType_SessionVariableName] = null;
			_httpContext.Session[Constants.SiteMinderSimulator_UserName_SessionVariableName] = null;
			_httpContext.Session.Abandon();
		}
		#endregion
	}
}

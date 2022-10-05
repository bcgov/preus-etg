using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="AuthenticationService"/> class, provides a way to manage authentication with SiteMinder.
	/// </summary>
	public class AuthenticationService : Service, IAuthenticationService
	{
		#region Variables
		private readonly ISiteMinderService _siteMinderService;
		private readonly IUserService _userService;
		#endregion

		#region Properties
		public Guid LoggedInUserGuid { private get; set; }
		public AccountTypes LoggedInUserType { private get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="AuthenticationService"/> class.
		/// </summary>
		/// <param name="siteMinderService"></param>
		/// <param name="userService"></param>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public AuthenticationService(ISiteMinderService siteMinderService, IUserService userService, IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
			_siteMinderService = siteMinderService;
			_userService = userService;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Fetch all the users for the specified account type.
		/// </summary>
		/// <param name="accountType"></param>
		/// <returns></returns>
		public List<KeyValuePair<string, string>> GetLogInOptions(AccountTypes accountType)
		{
			var users = _dbContext.Users
				.Where(u => u.AccountType == accountType)
				.ToList()
				.OrderBy(u => u.Organization?.LegalName)
				.ThenBy(u => u.LastName)
				.ThenBy(u => u.FirstName)
				.Select(x => new KeyValuePair<string, string>(x.BCeIDGuid.ToString(), $"{x.Organization?.LegalName}{(String.IsNullOrEmpty(x.Organization?.DoingBusinessAs) ? "" : "/" + x.Organization?.DoingBusinessAs)} - {x.LastName}, {x.FirstName}"))
				.ToList();

			return users;
		}

		/// <summary>
		/// Login the external user with SiteMinder.
		/// </summary>
		/// <param name="userGuid"></param>
		/// <param name="userType"></param>
		public void LogIn(Guid userGuid, string userType)
		{
			_siteMinderService.LogIn(userGuid, userType);

				// Every time a user logs-in to CJG, check if the current email address from BCeID is different, and if so, update the email address in their user profile
				bool.TryParse(ConfigurationManager.AppSettings["BCeIDWebService_Simulator"], out bool simulator);
				if (!simulator)
				{
					try
					{
						_userService.UpdateUserFromBCeIDAccount();
					}
					catch (Exception e)
					{
						_logger.Error(e);
					}
				}

				var user = _dbContext.Users.FirstOrDefault(u => u.BCeIDGuid == userGuid);

			Refresh(user);
		}

		/// <summary>
		/// Refresh the external user with Information.
		/// </summary>
		/// <param name="user"></param>
		public void Refresh(User user)
		{
			if (user != null)
			{
				var identity = new ClaimsIdentity("Forms");
				identity.AddClaim(new System.Security.Claims.Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "SiteMinder", ClaimValueTypes.String, "CJG"));
				identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.BCeIDGuid.ToString(), ClaimValueTypes.String, "CJG"));
				identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.BCeIDGuid.ToString(), ClaimValueTypes.String, "CJG"));
				identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.GivenName, user.FirstName, ClaimValueTypes.String, "CJG"));
				identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Surname, user.LastName, ClaimValueTypes.String, "CJG"));
				identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.EmailAddress, ClaimValueTypes.Email, "CJG"));
				identity.AddClaim(new System.Security.Claims.Claim(AppClaimTypes.AccountType, user.AccountType.ToString(), ClaimValueTypes.String, "CJG"));
				identity.AddClaim(new System.Security.Claims.Claim(AppClaimTypes.UserId, user.Id.ToString(), ClaimValueTypes.Integer, "CJG"));
				identity.AddClaim(new System.Security.Claims.Claim(AppClaimTypes.OrganizationAdministrator, user.IsOrganizationProfileAdministrator.ToString(), ClaimValueTypes.Boolean, "CJG"));
				identity.AddClaim(new System.Security.Claims.Claim(AppClaimTypes.OrganizationId, user.OrganizationId.ToString(), ClaimValueTypes.Integer, "CJG"));
				identity.AddClaim(new System.Security.Claims.Claim(AppClaimTypes.OrganizationName, user.Organization.LegalName, ClaimValueTypes.String, "CJG"));
				var principal = new GenericPrincipal(identity, new[] { "Application Administrator", "Employer Administrator" });
				_httpContext.User = principal;
				Thread.CurrentPrincipal = principal;
				_httpContext.Session["User"] = principal;
			}
		}

		/// <summary>
		/// Login the internal user with SiteMinder.
		/// </summary>
		/// <param name="userGuid"></param>
		public void LogInInternal(Guid userGuid)
		{
			try
			{
				_siteMinderService.LogInInternal(userGuid);
			}
			catch (Exception e)
			{
				_logger.Error(e, "Log in internal user: {0}", userGuid);
				throw;
			}
		}

		/// <summary>
		/// Logout the user from SiteMinder.
		/// </summary>
		/// <returns></returns>
		public string LogOut()
		{
			try
			{
				_siteMinderService.LogOut();
				System.Web.Security.FormsAuthentication.SignOut();

				if (ConfigurationManager.AppSettings[Constants.BCeID_WebService_Logoff_URL] != null)
				{
					return $"{ConfigurationManager.AppSettings[Constants.BCeID_WebService_Logoff_URL]}?returl={ConfigurationManager.AppSettings[Constants.BCeID_WebService_Logoff_Return_URL]}&retnow=1";
				}

				if (_httpContext.Request.Url.AbsolutePath.StartsWith("/Int", StringComparison.InvariantCultureIgnoreCase))
					return _httpContext.Request.Url.GetLeftPart(UriPartial.Authority) + "/Int";

				return _httpContext.Request.Url.GetLeftPart(UriPartial.Authority);
			}
			catch (Exception e)
			{
				_logger.Error(e, "Couldn't log out.");
				throw;
			}
		}
	}
	#endregion
}

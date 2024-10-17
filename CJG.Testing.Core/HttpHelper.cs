using CJG.Core.Entities;
using CJG.Infrastructure.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CJG.Testing.Core
{
	public static class HttpHelper
	{
		#region Methods
		/// <summary>
		/// Create an identity principal for the external user.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public static System.Security.Principal.IPrincipal CreateIdentity(this User user)
		{
			var identity = new System.Security.Claims.ClaimsIdentity("Forms");
			identity.AddClaim(new System.Security.Claims.Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "SiteMinder", System.Security.Claims.ClaimValueTypes.String, "CJG"));
			identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.BCeIDGuid.ToString(), System.Security.Claims.ClaimValueTypes.String, "CJG"));
			identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.ToString(), System.Security.Claims.ClaimValueTypes.String, "CJG"));
			identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.GivenName, user.FirstName, System.Security.Claims.ClaimValueTypes.String, "CJG"));
			identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Surname, user.LastName, System.Security.Claims.ClaimValueTypes.String, "CJG"));
			identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.EmailAddress, System.Security.Claims.ClaimValueTypes.Email, "CJG"));
			identity.AddClaim(new System.Security.Claims.Claim(AppClaimTypes.AccountType, user.AccountType.ToString(), System.Security.Claims.ClaimValueTypes.String, "CJG"));
			identity.AddClaim(new System.Security.Claims.Claim(AppClaimTypes.UserId, user.Id.ToString(), System.Security.Claims.ClaimValueTypes.Integer, "CJG"));
			identity.AddClaim(new System.Security.Claims.Claim(AppClaimTypes.OrganizationAdministrator, user.IsOrganizationProfileAdministrator.ToString(), System.Security.Claims.ClaimValueTypes.Boolean, "CJG"));
			identity.AddClaim(new System.Security.Claims.Claim(AppClaimTypes.OrganizationId, user.OrganizationId.ToString(), System.Security.Claims.ClaimValueTypes.Integer, "CJG"));
			identity.AddClaim(new System.Security.Claims.Claim(AppClaimTypes.OrganizationName, user.Organization.LegalName, System.Security.Claims.ClaimValueTypes.String, "CJG"));
			var principal = new System.Security.Principal.GenericPrincipal(identity, new[] { "Application Administrator", "Employer Administrator" });
			return principal;
		}

		/// <summary>
		/// Create an identity principal for the internal user having the specified roles.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="roles"></param>
		/// <returns></returns>
		public static System.Security.Principal.IPrincipal CreateIdentity(this InternalUser user, params string[] roles)
		{
			return user.CreateIdentity(roles, null);
		}

		/// <summary>
		/// Create an identity principal for the internal user having the specified roles.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="roles"></param>
		/// <returns></returns>
		public static System.Security.Principal.IPrincipal CreateIdentity(this InternalUser user, params ServiceHelper.Roles[] roles)
		{
			var role_names = new List<string>();
			foreach (var role in roles)
			{
				role_names.Add(role.GetDescription());
			}
			return user.CreateIdentity(role_names.ToArray());
		}

		/// <summary>
		/// Creates an identity principal for the internal user having the specified privileges.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="privileges"></param>
		/// <returns></returns>
		public static System.Security.Principal.IPrincipal CreateIdentity(this InternalUser user, params Privilege[] privileges)
		{
			return user.CreateIdentity(null, privileges);
		}


		/// <summary>
		/// Creates the identity principal for the internal user having the specified roles and privileges.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="roles"></param>
		/// <param name="privileges"></param>
		/// <returns></returns>
		private static System.Security.Principal.IPrincipal CreateIdentity(this InternalUser user, string[] roles = null, Privilege[] privileges = null)
		{
			var identity = new System.Security.Claims.ClaimsIdentity("Forms");
			identity.AddClaim(new System.Security.Claims.Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "SiteMinder", System.Security.Claims.ClaimValueTypes.String, "CJG"));
			identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.IDIR, System.Security.Claims.ClaimValueTypes.String, "CJG"));
			identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.ToString(), System.Security.Claims.ClaimValueTypes.String, "CJG"));
			identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.GivenName, user.FirstName, System.Security.Claims.ClaimValueTypes.String, "CJG"));
			identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Surname, user.LastName, System.Security.Claims.ClaimValueTypes.String, "CJG"));
			identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email, System.Security.Claims.ClaimValueTypes.Email, "CJG"));
			identity.AddClaim(new System.Security.Claims.Claim(AppClaimTypes.AccountType, AccountTypes.Internal.ToString(), System.Security.Claims.ClaimValueTypes.String, "CJG"));
			identity.AddClaim(new System.Security.Claims.Claim(AppClaimTypes.UserId, user.Id.ToString(), System.Security.Claims.ClaimValueTypes.Integer, "CJG"));
			if (roles != null)
			{
				foreach (var role in roles)
				{
					foreach (var privilege in ServiceHelper.RolePrivileges[role])
					{
						identity.AddClaim(new System.Security.Claims.Claim(AppClaimTypes.Privilege, privilege.ToString(), System.Security.Claims.ClaimValueTypes.String, "CJG"));
					}
				}
			}
			if (privileges != null)
			{
				foreach (var privilege in privileges)
				{
					identity.AddClaim(new System.Security.Claims.Claim(AppClaimTypes.Privilege, privilege.ToString(), System.Security.Claims.ClaimValueTypes.String, "CJG"));
				}
			}
			var principal = new System.Security.Principal.GenericPrincipal(identity, roles);
			return principal;
		}

		public static void SetupRequestUrl(this Mock<HttpRequestBase> mock, string url)
		{
			if (url == null)
				throw new ArgumentNullException("url");

			//if (!url.StartsWith("~/"))
			//	throw new ArgumentException("Sorry, we expect a virtual url starting with \"~/\".");

			mock.Setup(req => req.QueryString).Returns(GetQueryStringParameters(url));
			mock.Setup(req => req.AppRelativeCurrentExecutionFilePath).Returns(GetUrlFileName(url));
			mock.Setup(req => req.PathInfo).Returns(string.Empty);
		}

		static NameValueCollection GetQueryStringParameters(string url)
		{
			if (url.Contains("?"))
			{
				var parameters = new NameValueCollection();

				var parts = url.Split("?".ToCharArray());
				var keys = parts[1].Split("&".ToCharArray());

				foreach (var key in keys)
				{
					var part = key.Split("=".ToCharArray());
					parameters.Add(part[0], part[1]);
				}

				return parameters;
			}

			return null;
		}

		static string GetUrlFileName(string url)
		{
			return (url.Contains("?"))
				? url.Substring(0, url.IndexOf("?"))
				: url;
		}

		/// <summary>
		/// Mock an HttpContextBase and initialize it with the specified user.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="requestUrl"></param>
		/// <returns>Returns a new instance of a <typeparamref name="Mock[HttpContextBase]"/> object.</returns>
		public static Mock<HttpContextBase> MockHttpContext(this IPrincipal user, string requestUrl = "http://localhost/")
		{
			var helper = new MoqHelper();
			return MockHttpContext(helper, user, requestUrl);
		}

		/// <summary>
		/// Mock an HttpContextBase and initialize it with the specified user.
		/// </summary>
		/// <param name="helper"></param>
		/// <param name="user"></param>
		/// <param name="requestUrl"></param>
		/// <returns>Returns a new instance of a <typeparamref name="Mock[HttpContextBase]"/> object.</returns>
		public static Mock<HttpContextBase> MockHttpContext(this MoqHelper helper, IPrincipal user, string requestUrl = "http://localhost/")
		{
			var session = new Dictionary<string, object>();

			var httpContextMock = helper.SetMock<HttpContextBase>();
			var requestMock = helper.SetMock<HttpRequestBase>();
			requestMock.DefaultValue = DefaultValue.Mock;
			var responseMock = helper.SetMock<HttpResponseBase>();
			var sessionMock = helper.SetMock<HttpSessionStateBase>();
			var serverMock = helper.SetMock<HttpServerUtilityBase>();
			var urlHelperMock = helper.SetMock<UrlHelper>();
			urlHelperMock.CallBase = true;

			requestMock.Setup(r => r.AppRelativeCurrentExecutionFilePath).Returns("~/");
			requestMock.Setup(r => r.ApplicationPath).Returns("/");
			requestMock.Setup(m => m.RawUrl).Returns(requestUrl);
			requestMock.Setup(m => m.Url).Returns(new Uri(requestUrl));
			requestMock.SetupGet(x => x.Headers).Returns(new System.Net.WebHeaderCollection { { "X-Requested-With", "XMLHttpRequest" } });
			requestMock.Setup(x => x.Unvalidated.Form).Returns(new NameValueCollection());
			requestMock.Setup(x => x.ContentType).Returns(string.Empty);
			requestMock.Setup(x => x.QueryString).Returns(new NameValueCollection());
			requestMock.Setup(x => x.Form).Returns(new NameValueCollection());
			requestMock.Setup(m => m.UrlReferrer).Returns(new Uri(requestUrl));
			requestMock.Setup(m => m.PathInfo).Returns("");
			requestMock.SetupRequestUrl(requestUrl);
			requestMock.Setup(m => m.ServerVariables).Returns(new NameValueCollection());
			//requestMock.Setup(x => x.Files).Returns(new Mock<HttpFileCollectionBase>().Object);

			responseMock.SetupAllProperties();
			responseMock.Setup(s => s.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(s => string.IsNullOrEmpty(s) ? "~/" : s);
			responseMock.Setup(m => m.AddHeader(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>((name, value) =>
			{

			});
			//responseMock.SetupProperty(res => res.StatusCode, (int)System.Net.HttpStatusCode.OK);

			//httpContextMock.SetupAllProperties();
			httpContextMock.Setup(m => m.Request).Returns(requestMock.Object);
			//httpContextMock.Setup(m => m.Response).Returns(new HttpResponseWrapper(new HttpResponse(new StringWriter())));
			httpContextMock.Setup(m => m.Response).Returns(responseMock.Object);
			httpContextMock.Setup(m => m.Session).Returns(sessionMock.Object);
			httpContextMock.Setup(m => m.Session["attachmentId"]).Returns(1);
			httpContextMock.Setup(m => m.Session["attachmentRowVersion"]).Returns(Convert.FromBase64String("AgQGCAoMDhASFA=="));
			httpContextMock.Setup(m => m.Session["attachmentError"]).Returns("error");
			httpContextMock.Setup(m => m.Server).Returns(serverMock.Object);
			httpContextMock.Setup(m => m.User).Returns(user);

			httpContextMock.SetupSet(m => m.Session[Application.Services.Constants.SiteMinderSimulator_UserGuid_SessionVariableName] = It.IsAny<Object>())
				.Callback((string name, object val) => session[name] = val);
			httpContextMock.SetupGet(m => m.Session[Application.Services.Constants.SiteMinderSimulator_UserGuid_SessionVariableName])
				.Returns(() => session.ContainsKey(Application.Services.Constants.SiteMinderSimulator_UserGuid_SessionVariableName) ? session[Application.Services.Constants.SiteMinderSimulator_UserGuid_SessionVariableName] : null);

			httpContextMock.SetupSet(m => m.Session[Application.Services.Constants.SiteMinderSimulator_UserType_SessionVariableName] = It.IsAny<Object>())
				.Callback((string name, object val) => session[name] = val);
			httpContextMock.SetupGet(m => m.Session[Application.Services.Constants.SiteMinderSimulator_UserType_SessionVariableName])
				.Returns(() => session.ContainsKey(Application.Services.Constants.SiteMinderSimulator_UserType_SessionVariableName) ? session[Application.Services.Constants.SiteMinderSimulator_UserType_SessionVariableName] : null);

			httpContextMock.SetupSet(m => m.Session[Application.Services.Constants.SiteMinderSimulator_UserName_SessionVariableName] = It.IsAny<Object>())
				.Callback((string name, object val) => session[name] = val);
			httpContextMock.SetupGet(m => m.Session[Application.Services.Constants.SiteMinderSimulator_UserName_SessionVariableName])
				.Returns(() => session.ContainsKey(Application.Services.Constants.SiteMinderSimulator_UserName_SessionVariableName) ? session[Application.Services.Constants.SiteMinderSimulator_UserName_SessionVariableName] : null);
					   
			return httpContextMock;
		}

		public static Mock<HttpContextBase> MockHttpContext(this MoqHelper helper, string requestUrl = "http://localhost/")
		{
			var httpContextMock = helper.SetMock<HttpContextBase>();
			var requestMock = helper.SetMock<HttpRequestBase>();
			requestMock.DefaultValue = DefaultValue.Mock;

			httpContextMock.Setup(x => x.Request).Returns(requestMock.Object);

			return httpContextMock;
		}
		public static T CreateController<T>(this ServiceHelper helper) where T : ControllerBase
		{
			var controller = helper.Create<T>();
			controller.ControllerContext = new ControllerContext(helper.GetMock<HttpContextBase>().Object, new RouteData(), controller as ControllerBase);
			return controller;
		}
		#endregion
	}
}

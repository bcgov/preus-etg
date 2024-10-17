using System;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Mvc;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;

namespace CJG.Web.External.Views.Shared
{
    public abstract class SiteMinderViewPage : WebViewPage
	{
		public IUserService UserService { get; set; }

		public ISiteMinderService SiteMinderService { get; set; }

		private bool ShowSTGInfo { get; set; } = false;

		public string EnvironmentShortName { get; set; } = string.Empty;

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

		public bool ShowEnvironmentInfo()
		{
			CheckDevPreconditions();
			CheckQAPreconditions();

			return ShowSTGInfo;
		}

		public string GetCurrentEnvironment()
		{
			return EnvironmentShortName;
		}

		public string GetAppDateTime()
		{
			return AppDateTime.Now.ToStringLocalTime();
		}

		public string GetVersion()
		{
			return typeof(MvcApplication).Assembly.GetName().Version.ToString();
		}

		public string GetAssemblyBuildDate()
		{
			return GetLinkerTimestampUtc().ToStringLocalTime();
		}

		public string GetCurrentCulture()
		{
			return CultureInfo.CurrentCulture.Name;
		}

		public DateTime GetLinkerTimestampUtc()
		{
			var filePath = typeof(MvcApplication).Assembly.Location;

			const int peHeaderOffset = 60;
			const int linkerTimestampOffset = 8;
			var bytes = new byte[2048];

			using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				file.Read(bytes, 0, bytes.Length);
			}

			var headerPos = BitConverter.ToInt32(bytes, peHeaderOffset);
			var secondsSince1970 = BitConverter.ToInt32(bytes, headerPos + linkerTimestampOffset);
			var dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return dt.AddSeconds(secondsSince1970);
		}

		[System.Diagnostics.Conditional("DEBUG")]
		private void CheckDevPreconditions()
		{
			ShowSTGInfo = true;
			EnvironmentShortName = "ETG";
		}

		[System.Diagnostics.Conditional("DEBUG")]
		private void CheckQAPreconditions()
		{
			ShowSTGInfo = true;
			EnvironmentShortName = "ETG";
		}
	}
}

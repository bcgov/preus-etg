using System.Configuration;
using System.Web;

namespace CJG.Web.External.Helpers
{
	public class CanadaPostConfiguration : ICanadaPostConfiguration
	{
		/// <summary>
		/// Get CanadaPost Autocomplete Api Configuration settings.
		///  - If "CanadaPostAutoCompleteApiKey" is supplied in web.config AppSettings, it will supersede the logic in this class
		///  - Optionally, can also supply a configuration value of "CanadaPostAutoCompleteApiHttps" of true (for https) or false (for http) to use http/https
		/// </summary>

		public const string DevelopmentKey = "hd39-ma41-un93-wc41";    // Ministry Trial Key for Development - This key is bound to the host localhost:57777  Confirmation Key: RUZDT

		private readonly HttpContextBase _httpContextBase;
		private CanadaPostKey _canadaPostKey;

		public CanadaPostConfiguration(HttpContextBase context)
		{
			_httpContextBase = context;
		}

		public CanadaPostKey GetConfiguredKey()
		{
			var key = ConfigurationManager.AppSettings["CanadaPostAutoCompleteApiKey"];
			bool.TryParse(ConfigurationManager.AppSettings["CanadaPostAutoCompleteApiHttps"], out var isHttps);

			if (string.IsNullOrWhiteSpace(key))
				return null;

			return new CanadaPostKey
			{
				Key = key,
				IsHttps = isHttps
			};
		}

		public string GetJsPath()
		{
			var key = GetCanadaPostKey();
			var path = key.IsHttps
				? "https://ws1.postescanada-canadapost.ca/js/addresscomplete-2.30.min.js?key="
				: "https://ws1.postescanada-canadapost.ca/js/addresscomplete-2.30.min.js?key=";
			
			return $"{path}{key.Key}";
		}
	
		public string GetCssPath()
		{
			var key = GetCanadaPostKey();
			var path = key.IsHttps
				? "https://ws1.postescanada-canadapost.ca/css/addresscomplete-2.30.min.css?key="
				: "https://ws1.postescanada-canadapost.ca/css/addresscomplete-2.30.min.css?key=";
			
			return $"{path}{key.Key}";
		}
	
		public CanadaPostKey GetCanadaPostKey()
		{
			if (_canadaPostKey != null)
				return _canadaPostKey;

			var configuredKey = GetConfiguredKey();
			if (configuredKey != null)
			{
				_canadaPostKey = configuredKey;
				return _canadaPostKey;
			}

			var key = string.Empty;
			var isHttps = true;

			var requestUrl = _httpContextBase.Request.Url;
			if (requestUrl == null)
				return new CanadaPostKey
				{
					Key = key,
					IsHttps = false
				};

			var hostname = requestUrl.Host.ToLower();
			var port = requestUrl.Port;

			if (hostname.StartsWith("qa") || hostname.Equals("qa.skillstraininggrants.gov.bc.ca"))
			{
				key = port == 8080 ? "uj89-nx14-fn99-ne69" : "jr49-rt91-fm46-dp87";
				isHttps = false;
			}

			if (hostname.StartsWith("test"))
				key = "ng11-gy73-rr71-ta98";

			if (hostname.StartsWith("training"))
				key = "ce57-pg62-jt15-kp59";

			if (hostname.StartsWith("support"))
				key = "cz58-uj19-et92-zc83";

			// Production Configuration
			if (hostname.Equals("skillstraininggrants.gov.bc.ca"))
				key = "tf99-cf94-jh76-xd79";   // Confirmation Code: MBFHR

			if (string.IsNullOrWhiteSpace(key))
			{
				key = DevelopmentKey;
				isHttps = false;
			}

			_canadaPostKey = new CanadaPostKey
			{
				Key = key,
				IsHttps = isHttps
			};

			return _canadaPostKey;
		}

		public class CanadaPostKey
		{
			public string Key { get; set; }
			public bool IsHttps { get; set; }
		}
	}

	public interface ICanadaPostConfiguration
	{
	}
}
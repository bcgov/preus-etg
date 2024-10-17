using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Script.Serialization;

namespace CJG.Application.Services
{
	/// <summary>
	/// Provides a way to validate recaptcha response.
	/// </summary>
	public class ReCaptchaService : Service, IReCaptchaService
	{
		private readonly bool _enabled;
		private readonly string _url;
		private readonly string _secret;
		private readonly string _siteKey;
		private readonly bool _allowInvalidCertificate;

		/// <summary>
		/// Creates a new instance of a ReCaptchaService object.
		/// Initializes configuration property values.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public ReCaptchaService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
			bool.TryParse(ConfigurationManager.AppSettings["EnableReCaptcha"], out _enabled);
			bool.TryParse(ConfigurationManager.AppSettings["AcceptAllCertifications"], out _allowInvalidCertificate);
			_url = ConfigurationManager.AppSettings["ReCaptchaUrl"] ?? throw new ConfigurationErrorsException("Application Setting 'ReCaptchaUrl' is missing.");
			_secret = ConfigurationManager.AppSettings["ReCaptchaSecret"] ?? throw new ConfigurationErrorsException("Application Setting 'ReCaptchaSecret' is missing.");
			_siteKey = ConfigurationManager.AppSettings["ReCaptchaSiteKey"] ?? throw new ConfigurationErrorsException("Application Setting 'ReCaptchaSiteKey' is missing.");

			if (_allowInvalidCertificate)
			{
				System.Net.ServicePointManager.ServerCertificateValidationCallback = AcceptAllCertifications;
			}
		}

		/// <summary>
		/// Is the Recaptcha Service enabled or disabled
		/// </summary>
		/// <returns></returns>
		public bool IsEnabled()
		{
			return _enabled;
		}

		/// <summary>
		/// Get the site key for the Google Recaptcha element on the page
		/// </summary>
		/// <returns></returns>
		public string GetSiteKey()
		{
			return _siteKey;
		}

		/// <summary>
		/// Sends the ReCaptcha information to Google to verify that the user is human.
		/// </summary>
		/// <param name="encodedResponse"></param>
		/// <param name="errorCodes"></param>
		/// <returns></returns>
		public bool Validate(string encodedResponse, ref string errorCodes)
		{
			// Configuration option to fake ReCaptcha.  Doing to to unblock QA issues.
			if (!_enabled)
				return true;

			if (string.IsNullOrEmpty(encodedResponse))
				return false;

			string googleReply;
			using (var client = new System.Net.WebClient())
			{
				googleReply = client.DownloadString($"{_url}?secret={_secret}&response={encodedResponse}");
			}

			var reCaptcha = new JavaScriptSerializer().Deserialize<reCaptchaResponse>(googleReply);
			errorCodes = googleReply;
			return reCaptcha.success;
		}

		/// <summary>
		/// Provides a way to deserialize the ReCaptcha validation response.
		/// </summary>
		private class reCaptchaResponse
		{
			public bool success { get; set; }
			public string challenge_ts { get; set; }
			public string hostname { get; set; }
			public List<string> error_codes { get; set; }
		}

		/// <summary>
		/// Provides a way to ignore invalid SSL Certificates.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="certification"></param>
		/// <param name="chain"></param>
		/// <param name="sslPolicyErrors"></param>
		/// <returns></returns>
		public static bool AcceptAllCertifications(object sender, X509Certificate certification, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}
	}
}

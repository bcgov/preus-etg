using CJG.Core.Interfaces.Service.Settings;
using CJG.Infrastructure.NotificationService.Properties;

namespace CJG.Infrastructure.NotificationService
{
	/// <summary>
	/// <typeparamref name="NotificationSettings"/> sealed class, provides a way to configure the notification settings.
	/// </summary>
	internal sealed class NotificationSettings : INotificationSettings
	{
		#region Properties
		/// <summary>
		/// get - The default notification sender name.
		/// </summary>
		public string DefaultSenderName { get; }

		/// <summary>
		/// get - The default notification sender email.
		/// </summary>
		public string DefaultSenderAddress { get; }

		/// <summary>
		/// get - Whether emails are enabled.
		/// </summary>
		public bool EnableEmails { get; }

		/// <summary>
		/// get - Whether to throw an exception if an email fails to send.
		/// </summary>
		public bool ThrowOnSendEmailError { get; set; }

		/// <summary>
		/// get - The website URL.
		/// </summary>
		public string SiteUrl { get; }
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="NotificationSettings"/> object.
		/// </summary>
		/// <param name="appSettings"></param>
		public NotificationSettings(Settings appSettings)
		{
			DefaultSenderAddress = appSettings.DefaultSenderAddress;
			DefaultSenderName = appSettings.DefaultSenderName;
			bool.TryParse(appSettings.EnableEmails ?? "false", out bool enableEmails);
			EnableEmails = enableEmails;
			ThrowOnSendEmailError = false;
			SiteUrl = "https://skillstraininggrants.gov.bc.ca/";
		}
		#endregion
	}
}
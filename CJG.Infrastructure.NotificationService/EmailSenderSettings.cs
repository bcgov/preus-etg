using System;
using CJG.Core.Interfaces.Service.Settings;
using CJG.Infrastructure.NotificationService.Properties;

namespace CJG.Infrastructure.NotificationService
{
    /// <summary>
    /// <typeparamref name="EmailSenderSettings"/> sealed class, provides a way to setup and configure email sender settings.
    /// </summary>
    internal sealed class EmailSenderSettings : IEmailSenderSettings
    {
        #region Properties
        /// <summary>
        /// get - The SMTP server that will be used to send emails.
        /// </summary>
        public string SmtpServer { get; }

        /// <summary>
        /// get - The timeout in seconds before an email will fail.
        /// </summary>
        public TimeSpan SendTimeout { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="EmailSenderSettings"/> object.
        /// </summary>
        /// <param name="appSettings"></param>
        public EmailSenderSettings(Settings appSettings)
        {
            SmtpServer = appSettings.SmtpServer;
            SendTimeout = appSettings.EmailSenderTimeout;
        }
        #endregion  
    }
}
using System;
using System.Configuration;
using CJG.Core.Interfaces.Service.Settings;

namespace CJG.Web.External.Helpers.Settings
{
    public class NotificationSettings : INotificationSettings
    {
        public string SmtpServer { get; }
        public TimeSpan SendTimeout { get; }
        public string DefaultSenderName { get; }
        public string DefaultSenderAddress { get; }
        public bool EnableEmails { get; }
        public bool ThrowOnSendEmailError { get; set; }

        public NotificationSettings() : this(
            ConfigurationManager.AppSettings["SMTPServer"],
            TimeSpan.FromSeconds(10), 
            ConfigurationManager.AppSettings["EmailFromAddress"], 
            ConfigurationManager.AppSettings["EmailFromDisplayName"],
            ParseBoolean(ConfigurationManager.AppSettings["EnableEmails"]),
            ParseBoolean(ConfigurationManager.AppSettings["EmailErrorThrow"]))
        {
        }

        internal NotificationSettings(string smtpServer, TimeSpan timeout, string fromAddress, string fromName, bool allowEmails, bool throwOnSendEmailError)
        {
            SmtpServer = smtpServer;
            SendTimeout = timeout;
            DefaultSenderAddress = fromAddress;
            DefaultSenderName = fromName;
            EnableEmails = allowEmails;
            ThrowOnSendEmailError = throwOnSendEmailError;
        }

        private static bool ParseBoolean(string s)
        {
            bool result;
            return !string.IsNullOrWhiteSpace(s) && bool.TryParse(s, out result) && result;
        }

        
    }
}
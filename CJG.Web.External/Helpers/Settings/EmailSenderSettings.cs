using System;
using System.Configuration;
using CJG.Core.Interfaces.Service.Settings;

namespace CJG.Web.External.Helpers.Settings
{
    public class EmailSenderSettings : IEmailSenderSettings
    {
        public string SmtpServer { get; }
        public TimeSpan SendTimeout { get; }
        
        public EmailSenderSettings()
        {
            var settings = ConfigurationManager.AppSettings;
            SmtpServer = settings["SMTPServer"];
            SendTimeout = TimeSpan.FromSeconds(10);
        }
    }
}
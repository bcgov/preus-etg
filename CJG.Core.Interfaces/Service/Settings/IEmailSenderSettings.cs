using System;

namespace CJG.Core.Interfaces.Service.Settings
{
    public interface IEmailSenderSettings
    {
        string SmtpServer { get; }
        TimeSpan SendTimeout { get; }
    }
}
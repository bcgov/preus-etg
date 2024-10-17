namespace CJG.Core.Interfaces.Service.Settings
{
    public interface INotificationSettings
    {
        string DefaultSenderName { get; }
        string DefaultSenderAddress { get; }
        bool EnableEmails { get; }
        bool ThrowOnSendEmailError { get; set; }
    }
}
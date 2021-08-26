using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
    public interface IEmailSenderService
    {
        void Send(string recipient, string subject, string body, string senderName, string senderAddress);

        void Send(IEnumerable<string> recipients, string subject, string body, string senderName,
            string senderAddress);
    }
}
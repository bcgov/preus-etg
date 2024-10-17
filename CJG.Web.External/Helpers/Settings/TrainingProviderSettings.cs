using System.Configuration;
using CJG.Core.Interfaces.Service.Settings;

namespace CJG.Web.External.Helpers.Settings
{
    public class TrainingProviderSettings : ITrainingProviderSettings
    {
        public string  AllowFileAttachmentExtensions { get; }

        public TrainingProviderSettings()
        {
            AllowFileAttachmentExtensions = ConfigurationManager.AppSettings["PermittedAttachmentTypes"];
        }
    }
}
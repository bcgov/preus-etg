using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using CJG.Application.Services.Notifications;
using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.Notifications
{
    public class NotificationTypeDashboardViewModel : BaseViewModel
	{
		public PageList<NotificationTypeViewModel> NotificationTypes { get; set; }
		public IEnumerable<KeyValuePair<NotificationResendRules, string>> ResendRules { get; set; }
		public IEnumerable<KeyValuePair<NotificationApprovalRules, string>> ApprovalRules { get; set; }
		public IEnumerable<KeyValuePair<NotificationParticipantReportRules, string>> ParticipantReportRules { get; set; }
		public IEnumerable<KeyValuePair<NotificationClaimReportRules, string>> ClaimReportRules { get; set; }
		public IEnumerable<KeyValuePair<NotificationCompletionReportRules, string>> CompletionReportRules { get; set; }
		public IEnumerable<KeyValuePair<NotificationRecipientRules, string>> RecipientRules { get; set; }
		public bool EnableNotifications { get; set; }
		public bool AllowEnableNotifications { get; set; }
		public IEnumerable<KeyValuePair<string, string>> VariableKeywords { get; set; }

		public NotificationTypeDashboardViewModel() { }
		public NotificationTypeDashboardViewModel(PageList<NotificationType> notificationTypes, ISettingService settingService)
		{
			if (notificationTypes == null)
				throw new ArgumentNullException(nameof(notificationTypes));

			if (settingService == null)
				throw new ArgumentNullException(nameof(settingService));

			var pageList = new PageList<NotificationTypeViewModel>
			{
				Page = notificationTypes.Page,
				Quantity = notificationTypes.Quantity,
				Total = notificationTypes.Total,
				Items = notificationTypes.Items.Select(t => new NotificationTypeViewModel(t)).ToArray()
			};

			NotificationTypes = pageList;

			ResendRules = Enum.GetValues(typeof(NotificationResendRules)).Cast<NotificationResendRules>().Select(e => new KeyValuePair<NotificationResendRules, string>(e, e.GetDescription())).ToArray();
			ApprovalRules = Enum.GetValues(typeof(NotificationApprovalRules)).Cast<NotificationApprovalRules>().Select(e => new KeyValuePair<NotificationApprovalRules, string>(e, e.GetDescription())).ToArray();
			ParticipantReportRules = Enum.GetValues(typeof(NotificationParticipantReportRules)).Cast<NotificationParticipantReportRules>().Select(e => new KeyValuePair<NotificationParticipantReportRules, string>(e, e.GetDescription())).ToArray();
			ClaimReportRules = Enum.GetValues(typeof(NotificationClaimReportRules)).Cast<NotificationClaimReportRules>().Select(e => new KeyValuePair<NotificationClaimReportRules, string>(e, e.GetDescription())).ToArray();
			CompletionReportRules = Enum.GetValues(typeof(NotificationCompletionReportRules)).Cast<NotificationCompletionReportRules>().Select(e => new KeyValuePair<NotificationCompletionReportRules, string>(e, e.GetDescription())).ToArray();
			RecipientRules = Enum.GetValues(typeof(NotificationRecipientRules)).Cast<NotificationRecipientRules>().Select(e => new KeyValuePair<NotificationRecipientRules, string>(e, e.GetDescription())).ToArray();

			var setting = settingService.Get("EnableEmails")?.Value ?? "True";

			bool.TryParse(setting, out bool enableEmails);
			EnableNotifications = enableEmails;

			bool.TryParse(ConfigurationManager.AppSettings["EnableEmails"], out bool allowEnableEmails);
			AllowEnableNotifications = allowEnableEmails;

			var excludedProperties = new[] { "GrantApplication", "Applicant", "GrantApplicationId" };
			VariableKeywords = typeof(NotificationViewModel).GetPropertiesAsKeyValuePairs(excludedProperties);
		}
	}
}
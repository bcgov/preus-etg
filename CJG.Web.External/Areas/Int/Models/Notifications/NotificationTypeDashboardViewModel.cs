using CJG.Application.Services.Notifications;
using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace CJG.Web.External.Areas.Int.Models.Notifications
{
	public class NotificationTypeDashboardViewModel : BaseViewModel
	{
		#region Properties
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
		#endregion

		#region Constructors
		public NotificationTypeDashboardViewModel() { }
		public NotificationTypeDashboardViewModel(PageList<NotificationType> notificationTypes, ISettingService settingService)
		{
			if (notificationTypes == null) throw new ArgumentNullException(nameof(notificationTypes));
			if (settingService == null) throw new ArgumentNullException(nameof(settingService));

			var pageList = new PageList<NotificationTypeViewModel>
			{
				Page = notificationTypes.Page,
				Quantity = notificationTypes.Quantity,
				Total = notificationTypes.Total,
				Items = notificationTypes.Items.Select(t => new NotificationTypeViewModel(t)).ToArray()
			};

			this.NotificationTypes = pageList;

			this.ResendRules = Enum.GetValues(typeof(NotificationResendRules)).Cast<NotificationResendRules>().Select(e => new KeyValuePair<NotificationResendRules, string>(e, e.GetDescription())).ToArray();
			this.ApprovalRules = Enum.GetValues(typeof(NotificationApprovalRules)).Cast<NotificationApprovalRules>().Select(e => new KeyValuePair<NotificationApprovalRules, string>(e, e.GetDescription())).ToArray();
			this.ParticipantReportRules = Enum.GetValues(typeof(NotificationParticipantReportRules)).Cast<NotificationParticipantReportRules>().Select(e => new KeyValuePair<NotificationParticipantReportRules, string>(e, e.GetDescription())).ToArray();
			this.ClaimReportRules = Enum.GetValues(typeof(NotificationClaimReportRules)).Cast<NotificationClaimReportRules>().Select(e => new KeyValuePair<NotificationClaimReportRules, string>(e, e.GetDescription())).ToArray();
			this.CompletionReportRules = Enum.GetValues(typeof(NotificationCompletionReportRules)).Cast<NotificationCompletionReportRules>().Select(e => new KeyValuePair<NotificationCompletionReportRules, string>(e, e.GetDescription())).ToArray();
			this.RecipientRules = Enum.GetValues(typeof(NotificationRecipientRules)).Cast<NotificationRecipientRules>().Select(e => new KeyValuePair<NotificationRecipientRules, string>(e, e.GetDescription())).ToArray();

			var setting = settingService.Get("EnableEmails")?.Value ?? "True";
			bool.TryParse(setting, out bool enableEmails);
			this.EnableNotifications = enableEmails;
			bool.TryParse(ConfigurationManager.AppSettings["EnableEmails"], out bool allowEnableEmails);
			this.AllowEnableNotifications = allowEnableEmails;

			var excludedProperties = new[] { "GrantApplication", "Applicant", "GrantApplicationId" };
			this.VariableKeywords = typeof(NotificationViewModel).GetPropertiesAsKeyValuePairs(excludedProperties);
		}
		#endregion
	}
}
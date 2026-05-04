using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.Notifications
{
	public class NotificationTypeViewModel : BaseViewModel
	{
		[Required(ErrorMessage = "The Name field is required."), MaxLength(250)]
		public string Caption { get; set; }
		[MaxLength(500)]
		public string Description { get; set; }
		public bool IsActive { get; set; }
		public int RowSequence { get; set; }
		[Required]
		public NotificationTemplateViewModel NotificationTemplate { get; set; } = new NotificationTemplateViewModel();
		public ApplicationStateInternal? PreviousApplicationState { get; set; }
		public string PreviousApplicationStateCaption { get; set; }
		public ApplicationStateInternal? CurrentApplicationState { get; set; }
		public string CurrentApplicationStateCaption { get; set; }
		[Required(ErrorMessage = "The notification trigger type is required.")]
		public NotificationTriggerTypes? NotificationTriggerId { get; set; }
		public string NotificationTriggerCaption { get; set; }
		[Required]
		public NotificationMilestoneDateName MilestoneDateName { get; set; }
		public string MilestoneDateNameCaption { get; set; }
		[Required(ErrorMessage = "The milestone date offset is required.")]
		public int MilestoneDateOffset { get; set; }
		public int MilestoneDateExpires { get; set; }
		public int ResendDelayDays { get; set; }
		[Required]
		public NotificationResendRules ResendRule { get; set; }
		public string ResendRuleCaption { get; set; }
		[Required]
		public NotificationApprovalRules ApprovalRule { get; set; }
		public string ApprovalRuleCaption { get; set; }
		[Required]
		public NotificationParticipantReportRules ParticipantReportRule { get; set; }
		public string ParticipantReportRuleCaption { get; set; }
		[Required]
		public NotificationClaimReportRules ClaimReportRule { get; set; }
		public string ClaimReportRuleCaption { get; set; }
		[Required]
		public NotificationCompletionReportRules CompletionReportRule { get; set; }
		public string CompletionReportRuleCaption { get; set; }
		[Required]
		public NotificationRecipientRules RecipientRule { get; set; }
		public string RecipientRuleCaption { get; set; }
		public bool CanDelete { get; set; }

		public NotificationTypeViewModel() { }

		public NotificationTypeViewModel(NotificationType notificationType)
		{
			if (notificationType == null)
				throw new ArgumentNullException(nameof(notificationType));

			Utilities.MapProperties(notificationType, this);

			NotificationTemplate.Id = notificationType.NotificationTemplateId;
			NotificationTemplate.Caption = notificationType.Caption;
			NotificationTemplate.EmailBody = notificationType.NotificationTemplate.EmailBody;
			NotificationTemplate.EmailSubject = notificationType.NotificationTemplate.EmailSubject;

			MilestoneDateNameCaption = MilestoneDateName.GetDescription();
			ResendRuleCaption = notificationType.ResendRule.GetDescription();
			ApprovalRuleCaption = notificationType.ApprovalRule.GetDescription();
			ParticipantReportRuleCaption = notificationType.ParticipantReportRule.GetDescription();
			ClaimReportRuleCaption = notificationType.ClaimReportRule.GetDescription();
			CompletionReportRuleCaption = notificationType.CompletionReportRule.GetDescription();
			RecipientRuleCaption = notificationType.RecipientRule.GetDescription();
			NotificationTriggerCaption = notificationType.NotificationTrigger.Caption;

			PreviousApplicationStateCaption = notificationType.PreviousApplicationState != null
				? Enum.GetName(typeof(ApplicationStateInternal), notificationType.PreviousApplicationState)
				: "Not Applicable";

			CurrentApplicationStateCaption = notificationType.CurrentApplicationState != null
				? Enum.GetName(typeof(ApplicationStateInternal), notificationType.CurrentApplicationState)
				: "Not Applicable";

			CanDelete = !notificationType.NotificationQueue.Any();
		}
	}
}
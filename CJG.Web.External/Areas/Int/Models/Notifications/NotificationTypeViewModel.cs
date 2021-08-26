using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models.Notifications
{
	public class NotificationTypeViewModel : BaseViewModel
	{
		#region Properties
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
		#endregion

		#region Constructors
		public NotificationTypeViewModel() { }

		public NotificationTypeViewModel(NotificationType notificationType)
		{
			if (notificationType == null) throw new ArgumentNullException(nameof(notificationType));

			Utilities.MapProperties(notificationType, this);

			this.NotificationTemplate.Id = notificationType.NotificationTemplateId;
			this.NotificationTemplate.Caption = notificationType.Caption;
			this.NotificationTemplate.EmailBody = notificationType.NotificationTemplate.EmailBody;
			this.NotificationTemplate.EmailSubject = notificationType.NotificationTemplate.EmailSubject;

			this.MilestoneDateNameCaption = this.MilestoneDateName.GetDescription();
			this.ResendRuleCaption = notificationType.ResendRule.GetDescription();
			this.ApprovalRuleCaption = notificationType.ApprovalRule.GetDescription();
			this.ParticipantReportRuleCaption = notificationType.ParticipantReportRule.GetDescription();
			this.ClaimReportRuleCaption = notificationType.ClaimReportRule.GetDescription();
			this.CompletionReportRuleCaption = notificationType.CompletionReportRule.GetDescription();
			this.RecipientRuleCaption = notificationType.RecipientRule.GetDescription();
			this.NotificationTriggerCaption = notificationType.NotificationTrigger.Caption;
			this.PreviousApplicationStateCaption = notificationType.PreviousApplicationState != null ? Enum.GetName(typeof(ApplicationStateInternal), notificationType.PreviousApplicationState) : "Not Applicable";
			this.CurrentApplicationStateCaption = notificationType.CurrentApplicationState != null ? Enum.GetName(typeof(ApplicationStateInternal), notificationType.CurrentApplicationState) : "Not Applicable";

			this.CanDelete = !notificationType.NotificationQueue.Any();
		}
		#endregion
	}
}
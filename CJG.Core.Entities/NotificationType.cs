using DataAnnotationsExtensions;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	/// <summary>
	/// NotificationType class, provides a list of valid notification types.
	/// </summary>
	public class NotificationType : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - The primary key uses IDENTITY.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - The unique caption.
		/// </summary>
		[Required(AllowEmptyStrings = false), MaxLength(250, ErrorMessage = "The caption cannot be longer than 250 characters."), Index("IX_Caption", IsUnique = true)]
		public string Caption { get; set; }

		/// <summary>
		/// get/set - A description of this notification type.
		/// </summary>
		[MaxLength(500, ErrorMessage = "The description cannot be longer than 500 characters.")]
		public string Description { get; set; }

		/// <summary>
		/// get/set - Whether this item is active.
		/// </summary>
		[Required, DefaultValue(true), Index("IX_NotificationType", Order = 2)]
		public bool IsActive { get; set; }

		/// <summary>
		/// get/set - The primary key and foreign key to the notification template.
		/// </summary>
		[Required]
		public int NotificationTemplateId { get; set; }

		/// <summary>
		/// get/set - The parent notification template.
		/// </summary>
		[ForeignKey(nameof(NotificationTemplateId))]
		public virtual NotificationTemplate NotificationTemplate { get; set; }

		/// <summary>
		/// get/set - This notifications previous application state.
		/// </summary>
		[DefaultValue(true), Index("IX_NotificationType", Order = 3)]
		public ApplicationStateInternal? PreviousApplicationState { get; set; }

		/// <summary>
		/// get/set - This notifications current application state.
		/// </summary>
		[DefaultValue(true), Index("IX_NotificationType", Order = 4)]
		public ApplicationStateInternal? CurrentApplicationState { get; set; }

		/// <summary>
		/// get/set - The primary key and foreign key to the notification trigger.
		/// </summary>
		[Required]
		public NotificationTriggerTypes NotificationTriggerId { get; set; }

		/// <summary>
		/// get/set - The parent notification trigger.
		/// </summary>
		[ForeignKey(nameof(NotificationTriggerId))]
		public virtual NotificationTrigger NotificationTrigger { get; set; }

		/// <summary>
		/// get/set - The milestone date for this notification type.
		/// </summary>
		[MaxLength(64)]
		public string MilestoneDateName { get; set; }

		/// <summary>
		/// get/set - The milestone date offset for this notification type.
		/// </summary>
		[Required, DefaultValue(0)]
		public int MilestoneDateOffset { get; set; }

		/// <summary>
		/// get/set - The milestone date expires for this notification type.
		/// </summary>
		[Required, DefaultValue(0)]
		public int MilestoneDateExpires { get; set; }

		/// <summary>
		/// get/set - The delay to resend for this notification type.
		/// </summary>
		[Required, DefaultValue(0)]
		public int ResendDelayDays { get; set; }

		/// <summary>
		/// get/set - The resend rules for this notification type.
		/// </summary>
		[Required]
		public NotificationResendRules ResendRule { get; set; }

		/// <summary>
		/// get/set - The approval rules for this notification type.
		/// </summary>
		[Required]
		public NotificationApprovalRules ApprovalRule { get; set; }

		/// <summary>
		/// get/set - The participant report rules for this notification type.
		/// </summary>
		[Required]
		public NotificationParticipantReportRules ParticipantReportRule { get; set; }

		/// <summary>
		/// get/set - The claim report rules for this notification type.
		/// </summary>
		[Required]
		public NotificationClaimReportRules ClaimReportRule { get; set; }

		/// <summary>
		/// get/set - The completion report rules for this notification type.
		/// </summary>
		[Required]
		public NotificationCompletionReportRules CompletionReportRule { get; set; }

		/// <summary>
		/// get/set - The recipient rules for this notification type.
		/// </summary>
		[Required]
		public NotificationRecipientRules RecipientRule { get; set; }

		/// <summary>
		/// get/set - The sequence to display this item.
		/// </summary>
		[Required, Min(0), DefaultValue(0)]
		public int RowSequence { get; set; }

		/// <summary>
		/// get - A collection of all the notification queue.
		/// </summary>
		public virtual ICollection<NotificationQueue> NotificationQueue { get; set; } = new List<NotificationQueue>();
		#endregion

		#region Constructors
		public NotificationType() { }

		public NotificationType(NotificationTriggerTypes trigger, string caption, int rowSequence = 0) : base()
		{
			this.NotificationTriggerId = trigger;
			this.Caption = caption;
			this.RowSequence = rowSequence;
			this.IsActive = true;
		}

		public NotificationType(NotificationTriggerTypes trigger, string caption, string description, NotificationTemplate notificationTemplate) : this(trigger, caption)
		{
			this.Description = description;
			this.NotificationTemplateId = notificationTemplate.Id;
			this.NotificationTemplate = notificationTemplate;
		}
		#endregion
	}
}

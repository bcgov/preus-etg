using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CJG.Core.Entities.Attributes;

namespace CJG.Core.Entities
{
	/// <summary>
	/// NotificationQueue class, provides the ORM a way to manage the notification queue.
	/// </summary>
	public class NotificationQueue : EntityBase
	{
		/// <summary>
		/// get/set - Primary key identity.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - Foreign key to the notification type.
		/// </summary>
		public int? NotificationTypeId { get; set; }

		/// <summary>
		/// get/set - The NotificationType associated to this notification queue.
		/// </summary>
		[ForeignKey(nameof(NotificationTypeId))]
		public virtual NotificationType NotificationType { get; set; }

		/// <summary>
		/// get/set - Foreign key to the grant application.
		/// </summary>
		public int? GrantApplicationId { get; set; }

		/// <summary>
		/// get/set - The GrantApplication associated to this notification queue.
		/// </summary>
		[ForeignKey(nameof(GrantApplicationId))]
		public virtual GrantApplication GrantApplication { get; set; }

		/// <summary>
		/// get/set - Foreign key to the organization.
		/// </summary>
		[Required, ForeignKey(nameof(Organization))]
		public int OrganizationId { get; set; }

		/// <summary>
		/// get/set - The Organization associated to this notification queue.
		/// </summary>
		[ForeignKey(nameof(OrganizationId))]
		public virtual Organization Organization { get; set; }

		/// <summary>
		/// get/set- Provides a way to identify notification details.
		/// </summary>
		[Index("IX_NotificationQueue", 1)]
		[Required(AllowEmptyStrings = false), MaxLength(100)]
		public string BatchNumber { get; set; }

		/// <summary>
		/// get/set- Email subject of this notification queue.
		/// </summary>
		[Required(AllowEmptyStrings = false), MaxLength(500)]
		public string EmailSubject { get; set; }

		/// <summary>
		/// get/set- Email body of this notification queue.
		/// </summary>
		[Required(AllowEmptyStrings = false)]
		public string EmailBody { get; set; }

		/// <summary>
		/// get/set- Email recipients of this notification queue.
		/// </summary>
		[Required(AllowEmptyStrings = false), MaxLength(500)]
		public string EmailRecipients { get; set; }

		/// <summary>
		/// get/set- Email sender of this notification queue.
		/// </summary>
		[Required(AllowEmptyStrings = false), MaxLength(500)]
		public string EmailSender { get; set; }

		/// <summary>
		/// get/set- State of this notification queue.
		/// </summary>
		[Required, DefaultValue(0)]
		[Index("IX_NotificationQueue", 2)]
		public NotificationState State { get; set; }

		/// <summary>
		/// get/set- ErrorMessage of this notification queue.
		/// </summary>
		[MaxLength(1000)]
		public string ErrorMessage { get; set; }

		/// <summary>
		/// get/set Send date of this notification queue.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Column(TypeName = "DATETIME2")]
		public DateTime? SendDate { get; set; }

		/// <summary>
		/// Creates a new instance of a NotificationQueue object.
		/// </summary>
		public NotificationQueue() { }

		/// <summary>
		/// Creates a new instance of a NotificationQueue object, and initializes it with the specified properties.
		/// This generates a new notification for a program notification.
		/// </summary>
		/// <param name="programNotification"></param>
		/// <param name="applicant"></param>
		/// <param name="sender"></param>
		public NotificationQueue(ProgramNotification programNotification, User applicant, string sender)
		{
			if (programNotification == null)
				throw new ArgumentNullException(nameof(programNotification));
			if (applicant == null)
				throw new ArgumentNullException(nameof(applicant));
			if (string.IsNullOrWhiteSpace(sender))
				throw new ArgumentException($"The argument '{nameof(sender)}' is required.", nameof(sender));

			BatchNumber = $"PN:{programNotification.Id}";
			EmailSender = sender;
			EmailBody = programNotification.NotificationTemplate.EmailBody;
			EmailSubject = programNotification.NotificationTemplate.EmailSubject;
			EmailRecipients = applicant.EmailAddress;
			SendDate = programNotification.SendDate;
			OrganizationId = applicant.OrganizationId;
			State = NotificationState.Queued;
		}

		/// <summary>
		/// Creates a new instance of a NotificationQueue object, and initializes it with the specified properties.
		/// This generates a new notification for a grant application.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="applicant"></param>
		/// <param name="sender"></param>
		/// <param name="body"></param>
		/// <param name="subject"></param>
		/// <param name="type"></param>
		public NotificationQueue(GrantApplication grantApplication, User applicant, string sender, string body, string subject, NotificationType type)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));
			if (applicant == null)
				throw new ArgumentNullException(nameof(applicant));
			if (type == null)
				throw new ArgumentNullException(nameof(type));
			if (string.IsNullOrWhiteSpace(sender))
				throw new ArgumentException($"The argument '{nameof(sender)}' is required.", nameof(sender));
			if (string.IsNullOrWhiteSpace(body))
				throw new ArgumentException($"The argument '{nameof(body)}' is required.", nameof(body));
			if (string.IsNullOrWhiteSpace(subject))
				throw new ArgumentException($"The argument '{nameof(subject)}' is required.", nameof(subject));

			var claim = grantApplication.GetCurrentClaim();

			BatchNumber = $"N-G:{grantApplication.Id}" + (claim != null ? $"-C:{claim.Id}-CV:{claim.ClaimVersion}" : "");
			GrantApplicationId = grantApplication.Id;
			GrantApplication = grantApplication;
			NotificationTypeId = type.Id;
			NotificationType = type;
			EmailSender = sender;
			EmailBody = body;
			EmailSubject = subject;
			EmailRecipients = applicant.EmailAddress;
			OrganizationId = grantApplication.Organization.Id;
			Organization = grantApplication.Organization;
			State = NotificationState.Queued;
		}

		public NotificationQueue(GrantApplication grantApplication, ParticipantInvitation participantInvitation, string sender, string body, string subject, NotificationType type)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));
			if (participantInvitation == null)
				throw new ArgumentNullException(nameof(participantInvitation));
			if (type == null)
				throw new ArgumentNullException(nameof(type));
			if (string.IsNullOrWhiteSpace(sender))
				throw new ArgumentException($"The argument '{nameof(sender)}' is required.", nameof(sender));
			if (string.IsNullOrWhiteSpace(body))
				throw new ArgumentException($"The argument '{nameof(body)}' is required.", nameof(body));
			if (string.IsNullOrWhiteSpace(subject))
				throw new ArgumentException($"The argument '{nameof(subject)}' is required.", nameof(subject));

			BatchNumber = $"N-G:{grantApplication.Id}-PIF:{participantInvitation.Id}";
			GrantApplicationId = grantApplication.Id;
			GrantApplication = grantApplication;
			NotificationTypeId = type.Id;
			NotificationType = type;
			EmailSender = sender;
			EmailBody = body;
			EmailSubject = subject;
			EmailRecipients = participantInvitation.EmailAddress;
			OrganizationId = grantApplication.Organization.Id;
			Organization = grantApplication.Organization;
			State = NotificationState.Queued;
		}
	}
}

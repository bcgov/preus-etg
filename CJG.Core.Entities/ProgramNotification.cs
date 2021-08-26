using CJG.Core.Entities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="ProgramNotification"/> class, manages notifications that are associated with a specific grant program.
	/// </summary>
	public class ProgramNotification : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - The primary key uses IDENTITY.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

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
		/// get/set - The date the notification to be send.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Column(TypeName = "DATETIME2")]
		public DateTime? SendDate { get; set; }

		/// <summary>
		/// get/set - Whether the recipient is all applicants or not.
		/// </summary>
		[Required, DefaultValue(0)]
		public bool AllApplicants { get; set; }

		/// <summary>
		/// get/set - The unique caption.
		/// </summary>
		[Required(AllowEmptyStrings = false, ErrorMessage = "Name is required."), MaxLength(250, ErrorMessage = "The caption cannot be longer than 250 characters."), Index("IX_Caption", IsUnique = true)]
		public string Caption { get; set; }

		/// <summary>
		/// get/set - A description of this program notification.
		/// </summary>
		[MaxLength(500, ErrorMessage = "The description cannot be longer than 500 characters.")]
		public string Description { get; set; }

		/// <summary>
		/// get/set - A collection of the recipients associated to this program notification.
		/// </summary>
		public virtual ICollection<ProgramNotificationRecipient> ProgramNotificationRecipients { get; set; } = new List<ProgramNotificationRecipient>();
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="ProgramNotification"/> object.
		/// </summary>
		public ProgramNotification() { }

		/// <summary>
		/// Creates a new instance of a <typeparamref name="ProgramNotification"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="notificationTemplate"></param>
		public ProgramNotification(string caption, NotificationTemplate notificationTemplate)
		{
			if (notificationTemplate == null) throw new ArgumentNullException(nameof(notificationTemplate));

			this.Caption = caption;
			this.NotificationTemplate = notificationTemplate;
		}
		#endregion
	}
}
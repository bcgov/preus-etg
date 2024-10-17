using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	/// <summary>
	/// GrantProgramNotificationType class, provides a way to associate notification types with a grant program.
	/// </summary>
	public class GrantProgramNotificationType : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - The primary key and foreign key to the parent grant program.
		/// </summary>
		[Key, Column(Order = 1)]
		public int GrantProgramId { get; set; }

		/// <summary>
		/// get/set - The parent grant program.
		/// </summary>
		[ForeignKey(nameof(GrantProgramId))]
		public virtual GrantProgram GrantProgram { get; set; }

		/// <summary>
		/// get/set - The primary key and foreign key to the parent notification type.
		/// </summary>
		[Key, Column(Order = 2)]
		public int NotificationTypeId { get; set; }

		/// <summary>
		/// get/set - The parent notification type.
		/// </summary>
		[ForeignKey(nameof(NotificationTypeId))]
		public virtual NotificationType NotificationType { get; set; }

		/// <summary>
		/// get/set - The foreign key to the notification template.
		/// </summary>
		[Required]
		public int NotificationTemplateId { get; set; }

		/// <summary>
		/// get/set - The notification template.
		/// </summary>
		[ForeignKey(nameof(NotificationTemplateId))]
		[Index("IX_NotificationType", 1)]
		public virtual NotificationTemplate NotificationTemplate { get; set; }

		/// <summary>
		/// get/set - Whether this notification is active for the grant program.
		/// </summary>
		[Required, DefaultValue(true)]
		public bool IsActive { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="GrantProgramNotification"/> object.
		/// </summary>
		public GrantProgramNotificationType()
		{

		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="GrantProgramNotification"/> object.
		/// </summary>
		/// <param name="program"></param>
		/// <param name="type"></param>
		/// <param name="template"></param>
		public GrantProgramNotificationType(GrantProgram program, NotificationType type, NotificationTemplate template)
		{
			this.GrantProgram = program ?? throw new ArgumentNullException(nameof(program));
			this.GrantProgramId = program.Id;
			this.NotificationType = type ?? throw new ArgumentNullException(nameof(type));
			this.NotificationTypeId = type.Id;
			this.NotificationTemplateId = template?.Id ?? throw new ArgumentNullException(nameof(template));
			this.NotificationTemplate = template;
			this.IsActive = true;
		}
		#endregion
	}
}

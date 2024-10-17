using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="NotificationTrigger"/> class, provides the ORM a way to manage the notification triggers.
	/// </summary>
	public class NotificationTrigger : LookupTable<NotificationTriggerTypes>
	{
		#region Properties
		/// <summary>
		/// get/set - The unique caption.
		/// </summary>
		[MaxLength(500, ErrorMessage = "The description cannot be longer than 500 characters.")]
		public string Description { get; set; }

		/// <summary>
		/// get - All the notification types with this trigger type.
		/// </summary>
		public virtual ICollection<NotificationType> NotificationTypes { get; set; } = new List<NotificationType>();
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="NotificationTrigger"/> object.
		/// </summary>
		public NotificationTrigger()
		{ }

		/// <summary>
		/// Creates a new instance of a <typeparamref name="NotificationTrigger"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="rowSequence"></param>
		public NotificationTrigger(string caption, int rowSequence = 0) : base(caption, rowSequence)
		{

		}
		#endregion
	}
}

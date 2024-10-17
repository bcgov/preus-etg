using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="ProgramNotificationRecipient"/> class, manages which recipients will receive the program notification.
	/// </summary>
	public class ProgramNotificationRecipient : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - The primary key and foreign key to the program notification.
		/// </summary>
		[Key, ForeignKey(nameof(ProgramNotification)), Required, Column(Order = 0)]
		public int ProgramNotificationId { get; set; }

		/// <summary>
		/// get/set - The parent program notification.
		/// </summary>
		public virtual ProgramNotification ProgramNotification { get; set; }

		/// <summary>
		/// get/set - The primary key and foreign key to the grant program.
		/// </summary>
		[Key, ForeignKey(nameof(GrantProgram)), Required, Column(Order = 1)]
		public int GrantProgramId { get; set; }

		/// <summary>
		/// get/set - The parent grant program.
		/// </summary>
		public virtual GrantProgram GrantProgram { get; set; }

		/// <summary>
		/// get/set - Whether the recipient is an applicant or not.
		/// </summary>
		[Required, DefaultValue(0)]
		public bool ApplicantOnly { get; set; }

		/// <summary>
		/// get/set - Whether the recipient is a subscriber or not.
		/// </summary>
		[Required, DefaultValue(1)]
		public bool SubscriberOnly { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="ProgramNotificationRecipient"/> object.
		/// </summary>
		public ProgramNotificationRecipient() { }

		/// <summary>
		/// Creates a new instance of a <typeparamref name="ProgramNotificationRecipient"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="programNotification"></param>
		/// <param name="grantProgram"></param>
		public ProgramNotificationRecipient(ProgramNotification programNotification, GrantProgram grantProgram) {
			this.ProgramNotification = programNotification;
			this.GrantProgram = grantProgram;
		}

		#endregion
	}
}
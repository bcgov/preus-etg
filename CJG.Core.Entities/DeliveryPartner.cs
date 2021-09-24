using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	/// <summary>
	/// A DeliveryPartner class, provides a way to manage a list of delivery partners.
	/// </summary>
	public class DeliveryPartner : LookupTable<int>
	{
		#region Properties
		/// <summary>
		/// get/set - Foreign key to the grant program.
		/// </summary>
		public int GrantProgramId { get; set; }

		/// <summary>
		/// get/set - The grant program.
		/// </summary>
		[ForeignKey(nameof(GrantProgramId))]
		public virtual GrantProgram GrantProgram { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="DeliveryPartner"/> object.
		/// </summary>
		public DeliveryPartner() : base()
		{

		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="DeliveryPartner"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="grantProgram"></param>
		/// <param name="caption"></param>
		/// <param name="rowSequence"></param>
		public DeliveryPartner(GrantProgram grantProgram, string caption, int rowSequence = 0) : base(caption, rowSequence)
		{
			this.GrantProgramId = grantProgram?.Id ?? throw new ArgumentNullException(nameof(grantProgram));
			this.GrantProgram = grantProgram;
		}
		#endregion
	}
}

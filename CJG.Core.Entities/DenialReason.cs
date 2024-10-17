using DataAnnotationsExtensions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	/// <summary>
	/// A DenialReason class, provides a way to manage a list of Denial Reasons.
	/// </summary>

	public class DenialReason : EntityBase
	{
		#region Properties
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - The unique caption.
		/// </summary>
		[Required(AllowEmptyStrings = false), MaxLength(250, ErrorMessage = "The caption cannot be longer than 250 characters."), Index("IX_Caption", IsUnique = false)]
		public string Caption { get; set; }

		public bool IsActive { get; set; }

		[Required, Min(0), DefaultValue(0)]
		public int RowSequence { get; set; }

		public int GrantProgramId { get; set; }

		/// <summary>
		/// get/set - The parent grant program.
		/// </summary>
		[ForeignKey(nameof(GrantProgramId))]
		public virtual GrantProgram GrantProgram { get; set; }

		#endregion Properties

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="DenialReason"/> object.
		/// </summary>
		public DenialReason() : base()
		{

		}
		#endregion
	}
}
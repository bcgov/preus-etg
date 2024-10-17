using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace CJG.Core.Entities
{

	//[Table("ClaimParticipants", Schema ="dbo")]
    public class ClaimParticipant : EntityBase
	{
		#region Properties
		[Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ClaimId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ClaimVersion { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ParticipantFormId { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="ClaimParticipant"/> object.
		/// </summary>
		public ClaimParticipant()
		{ }

		/// <summary>
		/// Creates a new instance of a <typeparamref name="ClaimParticipant"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="claim"></param>
		/// <param name="participantForm"></param>
		public ClaimParticipant(Claim claim, ParticipantForm participantForm)
		{
			this.ClaimId = claim.Id;
			this.ClaimVersion = claim.ClaimVersion;
			this.ParticipantFormId = participantForm.Id;
		}
		#endregion
	}
}

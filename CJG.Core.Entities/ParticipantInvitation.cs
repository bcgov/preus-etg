using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	public class ParticipantInvitation : EntityBase
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public int? GrantApplicationId { get; set; }
		[ForeignKey(nameof(GrantApplicationId))]
		public virtual GrantApplication GrantApplication { get; set; }

		public int? ParticipantFormId { get; set; }
		[ForeignKey(nameof(ParticipantFormId))]
		public virtual ParticipantForm ParticipantForm { get; set; }

		public ParticipantInvitationStatus ParticipantInvitationStatus { get; set; }

		[Required]
		public Guid IndividualKey { get; set; }

		// The FirstName, LastName and Email are used on the invitation unless the PIF is completed - then load these values from the PIF
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string EmailAddress { get; set; }

		// Expected outcome is set ahead of time - copied down to the PIF when completed
		public ExpectedParticipantOutcome ExpectedParticipantOutcome { get; set; }
	}
}
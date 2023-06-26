using System.ComponentModel;

namespace CJG.Core.Entities
{
	public enum ParticipantInvitationStatus
	{
		Empty = 0,

		[Description("Not Sent")]
		NotSent = 1,

		Sent = 2,

		Completed = 3
	}
}
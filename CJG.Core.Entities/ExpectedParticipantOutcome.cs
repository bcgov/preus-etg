using System.ComponentModel;

namespace CJG.Core.Entities
{
	public enum ExpectedParticipantOutcome
	{
		[Description("Increased job security")]
		IncreasedJobSecurity = 1,

		[Description("Increased pay")]
		IncreasedPay = 2,

		[Description("Promotion or advancement to another position")]
		Promotion = 3,

		[Description("Move from part-time to full-time employment")]
		MoveFromPartTimeToFullTime = 4,

		[Description("Move from temporary/casual/seasonal employment to permanent employment")]
		MoveFromTransitionalToPermanent = 5,

		[Description("No outcome")]
		NoOutcome = 6
	}
}
using System.ComponentModel;

namespace CJG.Core.Entities
{
	/// <summary>
	/// NotificationParticipantReportRules enum, provides a way to identify notification participant reporting rules.
	/// </summary>
	public enum NotificationParticipantReportRules
	{
		/// <summary>
		/// NotApplicable - Participant reported not applicable.
		/// </summary>
		[Description("Not Applicable")]
		NotApplicable = 0,
		/// <summary>
		/// NoneReported - No participants were reported.
		/// </summary>
		[Description("None Reported")]
		NoneReported = 1,
		/// <summary>
		/// LessThanAgreedReported - Less participants than agreed were reported.
		/// </summary>
		[Description("Less Than Agreed Reported")]
		LessThanAgreedReported = 2,
		/// <summary>
		/// GreaterThanAgreedReported - More participants than agreed were reported.
		/// </summary>
		[Description("Greater Than Agreed Reported")]
		GreaterThanAgreedReported = 3,
		/// <summary>
		/// AllReported - All participants were reported.
		/// </summary>
		[Description("All Reported")]
		AllReported = 4
	}
}

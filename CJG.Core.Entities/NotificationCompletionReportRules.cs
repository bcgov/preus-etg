using System.ComponentModel;

namespace CJG.Core.Entities
{
	/// <summary>
	/// NotificationCompletionReportRules enum, provides a way to identify notification completion reporting rules.
	/// </summary>
	public enum NotificationCompletionReportRules
	{
		/// <summary>
		/// NotApplicable - Completion report not applicable.
		/// </summary>
		[Description("Not Applicable")]
		NotApplicable = 0,
		/// <summary>
		/// NotReported - Completion not reported.
		/// </summary>
		[Description("Not Reported")]
		NotReported = 1,
		/// <summary>
		/// Reported - Completion reported.
		/// </summary>
		[Description("Reported")]
		Reported = 2
	}
}

using System.ComponentModel;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="DocumentType"/> enum, provides a list of available document template types.
	/// </summary>
	public enum DocumentTypes
	{
		/// <summary>
		/// GrantAgreementCoverLetter - Approval Letter for a Grant Agreement.
		/// </summary>
		[Description("Grant Agreement Cover Letter")]
		GrantAgreementCoverLetter = 1,
		/// <summary>
		/// GrantAgreementScheduleA - Schedule A for a Grant Agreement.
		/// </summary>
		[Description("Grant Agreement Schedule A")]
		GrantAgreementScheduleA = 2,
		/// <summary>
		/// GrantAgreementScheduleB - Schedule B for a Grant Agreement.
		/// </summary>
		[Description("Grant Agreement Schedule B")]
		GrantAgreementScheduleB = 3,
		/// <summary>
		/// ApplicantDeclaration - Applicant Declaration statement.
		/// </summary>
		[Description("Applicant Declaration")]
		ApplicantDeclaration = 4,
		/// <summary>
		/// ParticipantConsent - Participant consent statement.
		/// </summary>
		[Description("Participant Consent")]
		ParticipantConsent = 5
	}
}

using System;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Int.Models
{
	public class ParticipantViewModel
	{
		public int? GrantApplicationId { get; set; }
		public int? ParticipantId { get; set; }

		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string EmailAddress { get; set; }
		public string PhoneNumber { get; set; }
		public decimal YTDFunded { get; set; } = 0;
		public string SIN { get; set; }
		public string ReportedBy { get; set; }
		public string ReportedOn { get; set; }
		public int? ConsentFormAttachmentId { get; set; }
		public bool? Approved { get; set; }
		public bool? Attended { get; set; }

		public bool? HasClaimWarnings { get; set; }

		public ParticipantViewModel() { }

		public ParticipantViewModel(ParticipantForm participant)
		{
			if (participant == null)
				throw new ArgumentNullException(nameof(participant));

			GrantApplicationId = participant.GrantApplicationId;
			ParticipantId = participant.Id;
			FirstName = participant.FirstName;
			LastName = participant.LastName;
			EmailAddress = participant.EmailAddress;
			PhoneNumber = participant.PhoneNumber1 ?? participant.PhoneNumber2;
			ReportedBy = participant.ParticipantConsentAttachmentId.HasValue ? "Applicant" : "Participant";
			ReportedOn = $"{ participant.DateAdded.ToLocalTime() }";
			ConsentFormAttachmentId = participant.ParticipantConsentAttachmentId;
			SIN = participant.SIN;
			Approved = participant.Approved;
			Attended = participant.Attended;
		}
	}
}
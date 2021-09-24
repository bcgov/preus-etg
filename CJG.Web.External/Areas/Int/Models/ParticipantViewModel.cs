using System;
using System.Collections.Generic;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Int.Models.Claims;

namespace CJG.Web.External.Areas.Int.Models
{
    public class ParticipantViewModel
    {
        #region Properties
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

		#endregion

		#region Constructors
		public ParticipantViewModel() { }
		

		public ParticipantViewModel(ParticipantForm participant)
        {
            if (participant == null) throw new ArgumentNullException(nameof(participant));

            this.GrantApplicationId = participant.GrantApplicationId;
            this.ParticipantId = participant.Id;
            this.FirstName = participant.FirstName;
            this.LastName = participant.LastName;
            this.EmailAddress = participant.EmailAddress;
            this.PhoneNumber = participant.PhoneNumber1 ?? participant.PhoneNumber2;
            this.ReportedBy = participant.ParticipantConsentAttachmentId.HasValue ? "Applicant" : "Participant";
            bool isReportedLate = (int)(participant.GrantApplication.StartDate - participant.DateAdded).TotalDays < 0;
            this.ReportedOn = $"{ participant.DateAdded.ToLocalTime() } { (isReportedLate ? " (Late)" : "") }";
            this.ConsentFormAttachmentId = participant.ParticipantConsentAttachmentId;
			this.SIN = participant.SIN;
			this.Approved = participant.Approved;
			this.Attended = participant.Attended;
		}
		#endregion
	}
}
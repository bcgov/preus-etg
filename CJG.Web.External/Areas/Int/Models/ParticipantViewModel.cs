using System;
using CJG.Core.Entities;


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

        public string ReportedBy { get; set; }

        public string ReportedOn { get; set; }

        public int? ConsentFormAttachmentId { get; set; }
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
        }
        #endregion  
    }
}
using CJG.Core.Entities;
using System;

namespace CJG.Web.External.Areas.Part.Models
{
    public class ParticipantInfoStep0ViewModel
    {
        public int Step { get; set; }

        public int GrantApplicationId { get; set; }

        public Guid InvitationKey { get; set; }

        public int DataCollected { get; set; }
        public DateTime AppDateTimeNow { get; set; } = AppDateTime.UtcNow;

        public bool ReportedByApplicant { get; set; }

        /// <summary>
        /// If the PIF form is filled in the by participant, there are six steps required.
        /// If the PIF form is filled in the by applicant, there are five steps required.
        /// </summary>
        public int TotalSteps => ReportedByApplicant ? 5 : 6;

        public bool HasConsentForm { get; set; }

		public DateTime ApplicationSubmissionDate { get; set; }

		public int? GrantProgramId { get; set; }
	}
}

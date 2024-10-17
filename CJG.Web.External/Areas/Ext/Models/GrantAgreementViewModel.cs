using CJG.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Web.External.Areas.Ext.Models
{
    public class GrantAgreementViewModel
    {
        #region Properties
        public int GrantApplicationId { get; set; }

        [NotMapped]
        public virtual GrantApplication GrantApplication { get; set; }

        public string DirectorNotes { get; set; }

        public int? CoverLetterId { get; set; }

        [NotMapped]
        public virtual Document CoverLetter { get; set; }

        public bool CoverLetterConfirmed { get; set; }

        public int? ScheduleAId { get; set; }

        [NotMapped]
        public virtual Document ScheduleA { get; set; }

        public bool ScheduleAConfirmed { get; set; }

        public int? ScheduleBId { get; set; }

        [NotMapped]
        public virtual Document ScheduleB { get; set; }

        public bool ScheduleBConfirmed { get; set; }

        [Required]
        public DateTime ParticipantReportingDueDate { get; set; }

        [Required]
        public DateTime ReimbursementClaimDueDate { get; set; }

        public DateTime? DateAccepted { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string IncompleteReason { get; set; }

        public byte[] RowVersion { get; set; }
        #endregion

        #region Constructors
        public GrantAgreementViewModel()
        { }

        public GrantAgreementViewModel(GrantAgreement grantAgreement)
        {
            if (grantAgreement == null)
                throw new ArgumentNullException(nameof(grantAgreement));

            this.GrantApplicationId = grantAgreement.GrantApplicationId;
            this.GrantApplication = grantAgreement.GrantApplication;
            this.DirectorNotes = grantAgreement.DirectorNotes;
            this.CoverLetterId = grantAgreement.CoverLetterId;
            this.CoverLetter = grantAgreement.CoverLetter;
            this.CoverLetterConfirmed = grantAgreement.CoverLetterConfirmed;
            this.ScheduleAId = grantAgreement.ScheduleAId;
            this.ScheduleA = grantAgreement.ScheduleA;
            this.ScheduleBId = grantAgreement.ScheduleBId;
            this.ScheduleB = grantAgreement.ScheduleB;
            this.ParticipantReportingDueDate = grantAgreement.ParticipantReportingDueDate;
            this.ReimbursementClaimDueDate = grantAgreement.ReimbursementClaimDueDate;
            this.DateAccepted = grantAgreement.DateAccepted;
            this.StartDate = grantAgreement.StartDate;
            this.EndDate = grantAgreement.EndDate;
            this.IncompleteReason = grantAgreement.GrantApplication.GetCancelledReason();
            this.RowVersion = grantAgreement.RowVersion;
        }
        #endregion

        #region Methods
        public static implicit operator GrantAgreement(GrantAgreementViewModel model)
        {
            if (model == null)
                return null;

            var grantAgreement = new GrantAgreement();
            grantAgreement.GrantApplicationId = model.GrantApplicationId;
            grantAgreement.GrantApplication = model.GrantApplication;
            grantAgreement.DirectorNotes = model.DirectorNotes;
            grantAgreement.CoverLetterId = model.CoverLetterId;
            grantAgreement.CoverLetter = model.CoverLetter;
            grantAgreement.CoverLetterConfirmed = model.CoverLetterConfirmed;
            grantAgreement.ScheduleAId = model.ScheduleAId;
            grantAgreement.ScheduleA = model.ScheduleA;
            grantAgreement.ScheduleBId = model.ScheduleBId;
            grantAgreement.ScheduleB = model.ScheduleB;
            grantAgreement.ParticipantReportingDueDate = model.ParticipantReportingDueDate;
            grantAgreement.ReimbursementClaimDueDate = model.ReimbursementClaimDueDate;
            grantAgreement.DateAccepted = model.DateAccepted;
            grantAgreement.StartDate = model.StartDate;
            grantAgreement.EndDate = model.EndDate;
            grantAgreement.RowVersion = model.RowVersion;
            return grantAgreement;
        }
        #endregion
    }
}
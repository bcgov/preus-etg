using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Collections.Generic;
using CJG.Core.Entities.Attributes;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="GrantAgreement"/> class, provides an ORM for Grant Agreemnt information.  A <typeparamref name="GrantAgreement"/> is a one-to-one relationship with GrantApplication.
	/// This is a one-to-one relationship with the grant application.
	/// </summary>
	public class GrantAgreement : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - The primary key of the Grant Agreement, which is the Grant Application Id (one-to-one relationship).
		/// </summary>
		[Key, ForeignKey(nameof(GrantApplication)), DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int GrantApplicationId { get; set; }

		/// <summary>
		/// get/set - The parent GrantApplication this Agreement belongs to.
		/// </summary>
		public virtual GrantApplication GrantApplication { get; set; }

		/// <summary>
		/// get/set - The Director's notes.
		/// </summary>
		public string DirectorNotes { get; set; }

		/// <summary>
		/// get/set - Foreign key Id for the Coverletter <typeparamref name="Document"/>.
		/// </summary>
		public int? CoverLetterId { get; set; }

		/// <summary>
		/// get/set - The Coverletter.
		/// </summary>
		[ForeignKey(nameof(CoverLetterId))]
		public virtual Document CoverLetter { get; set; }

		/// <summary>
		/// get/set - Whether the Application Administrator has confirmed the Coverletter.
		/// </summary>
		public bool CoverLetterConfirmed { get; set; }

		/// <summary>
		/// get/set - Foreign key Id for Schedule A <typeparamref name="Document"/>.
		/// </summary>
		public int? ScheduleAId { get; set; }

		/// <summary>
		/// get/set - Schedule A.
		/// </summary>
		[ForeignKey(nameof(ScheduleAId))]
		public virtual Document ScheduleA { get; set; }

		/// <summary>
		/// get/set - Whether the Application Administrator has confirmed Schedule A.
		/// </summary>
		public bool ScheduleAConfirmed { get; set; }

		/// <summary>
		/// get/set - Foreign key Id for Schedule B <typeparamref name="Document"/>.
		/// </summary>
		public int? ScheduleBId { get; set; }

		/// <summary>
		/// get/set - Schedule B.
		/// </summary>
		[ForeignKey(nameof(ScheduleBId))]
		public virtual Document ScheduleB { get; set; }

		/// <summary>
		/// get/set - Whether the Application Administrator has confirmed Schedul B.
		/// </summary>
		public bool ScheduleBConfirmed { get; set; }

		/// <summary>
		/// get/set - The date the Participant Reporting is due, is 5 days before the Training Program start date.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Required]
		[Index("IX_GrantAgreement", 4)]
		[Column(TypeName = "DATETIME2")]
		public DateTime ParticipantReportingDueDate { get; set; }

		/// <summary>
		/// get/set - The date the reimbursement Claim is due, is 30 days after the Training Program start date.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Required]
		[Index("IX_GrantAgreement", 5)]
		[Column(TypeName = "DATETIME2")]
		public DateTime ReimbursementClaimDueDate { get; set; }

		/// <summary>
		/// get/set - The date the Completion Report is due is 30 days after the Training Program end date.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Required]
		[Index("IX_GrantAgreement", 6)]
		[Column(TypeName = "DATETIME2")]
		public DateTime CompletionReportingDueDate { get; set; }

		/// <summary>
		/// get/set - The date the Agreement was accepted by the Application Administrator.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Index("IX_GrantAgreement", 3)]
		[Column(TypeName = "DATETIME2")]
		public DateTime? DateAccepted { get; set; }

		/// <summary>
		/// get/set - The Agreement start date represents the date it was issued.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Required]
		[Index("IX_GrantAgreement", 1)]
		[Column(TypeName = "DATETIME2")]
		public DateTime StartDate { get; set; }

		/// <summary>
		/// get/set - The Agreement end date is 60 days after the Training Program end date.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Required]
		[Index("IX_GrantAgreement", 2)]
		[Column(TypeName = "DATETIME2")]
		public DateTime EndDate { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of <typeparamref name="GrantAgreement"/> object.
		/// </summary>
		public GrantAgreement()
		{

		}

		/// <summary>
		/// Creates a new instance of <typeparamref name="GrantAgreement"/> object for the specified grant application.
		/// </summary>
		/// <param name="grantApplication"></param>
		public GrantAgreement(GrantApplication grantApplication)
		{
			ApplyTo(grantApplication);
		}
		#endregion

		#region Methods
		/// <summary>
		/// Update the GrantAgreement and attach it to the specified GrantApplication.
		/// Extracts relevant information from the GrantApplication and applies it this <typeparamref name="GrantAgreement"/>.
		/// </summary>
		/// <param name="grantApplication"></param>
		public void ApplyTo(GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			if (grantApplication.Id == 0)
				throw new InvalidOperationException("The grant application must already exist before creating the agreement.");

			if (grantApplication.TrainingPrograms.Count() == 0)
				throw new InvalidOperationException("The grant application must have at least one training program defined.");

			grantApplication.GrantAgreement = this;
			this.GrantApplication = grantApplication;
			this.GrantApplicationId = grantApplication.Id;

			// Agreement Term dates
			this.StartDate = AppDateTime.UtcNow;
			this.EndDate = grantApplication.EndDate.AddDays(60);

			var trainingStartDate = grantApplication.TrainingPrograms.Min(tp => tp.StartDate);
			var trainingEndDate = grantApplication.TrainingPrograms.Max(tp => tp.EndDate);

			this.ParticipantReportingDueDate = trainingStartDate.AddDays(-5);
			this.ReimbursementClaimDueDate = trainingStartDate.AddDays(30);
			this.CompletionReportingDueDate = trainingEndDate.AddDays(30);
		}

		/// <summary>
		/// Validate the GrantAgreement property values.
		/// </summary>
		/// <param name="validationContext"></param>
		/// <returns></returns>
		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			// Must be associated with a GrantApplication.
			if (this.GrantApplication == null && this.GrantApplicationId == 0)
				yield return new ValidationResult("The grant agreement must be associated with a grant application.", new[] { nameof(this.GrantApplication) });

			// Cannot have a DateAccepted unless related documents are confirmed; CoverLetterConfirmed, ScheduleAConfirmed, ScheduleBConfirmed.
			if (this.DateAccepted != null && (!this.CoverLetterConfirmed || !this.ScheduleAConfirmed || !this.ScheduleBConfirmed))
				yield return new ValidationResult("A grant agreement cannot be accepted before the cover letter, schedule A and schedule B are confirmed.", new[] { nameof(this.DateAccepted) });

			foreach (var validation in base.Validate(validationContext))
			{
				yield return validation;
			}
		}

		#endregion
	}

	public static class GrantAgreementExtensions
	{
		public static DateTime ConvertEndDateToLocalTime(this GrantAgreement grantAgreement)
		{
			DateTime time = grantAgreement.EndDate;

			if (time.Kind == DateTimeKind.Utc)
			{
				DateTime returnTime = new DateTime(time.Ticks, DateTimeKind.Local);
				returnTime += TimeZone.CurrentTimeZone.GetUtcOffset(returnTime);

				if (TimeZone.CurrentTimeZone.IsDaylightSavingTime(returnTime))
					returnTime -= new TimeSpan(1, 0, 0);
				return returnTime;
			}
			else
			{
				return time;
			}
		}

		public static DateTime GetClaimSubmissionDeadline(this GrantAgreement grantAgreement)
		{
			var FYEndDate = grantAgreement.GrantApplication.GrantOpening.TrainingPeriod.FiscalYear.EndDate.ToLocalTime();
			return new DateTime(FYEndDate.Year, FYEndDate.Month, 1);
		}

	}
}

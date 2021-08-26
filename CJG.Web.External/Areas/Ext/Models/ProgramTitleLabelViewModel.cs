using CJG.Core.Entities;
using CJG.Web.External.Helpers;
using System;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class ProgramTitleLabelViewModel
	{
		#region Properties
		public string FileNumber { get; set; }
		public string FileName { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? DateSubmitted { get; set; }
		public DateTime GrantOpeningDate { get; set; }
		public DateTime? GrantAgreementStartDate { get; set; }
		public string CancelText { get; set; }
		public ApplicationStateExternal ApplicationStateExternal { get; set; }
		public string GrantProgramName { get; set; }
		public string GrantStreamName { get; set; }
		public bool ShowReason { get; set; }
		public string ReasonHeading { get; set; }
		public string Reason { get; set; }
		#endregion

		#region Constructors
		public ProgramTitleLabelViewModel()
		{
		}

		public ProgramTitleLabelViewModel(GrantApplication grantApplication, bool showReason = true)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));

			this.FileNumber = grantApplication.FileNumber;
			this.FileName = grantApplication.GetFileName();
			this.StartDate = grantApplication.TrainingPrograms.Any() ? grantApplication.StartDate.ToLocalTime() : (DateTime?)null;
			this.DateSubmitted = grantApplication.DateSubmitted?.ToLocalTime(); 
			this.ApplicationStateExternal = grantApplication.ApplicationStateExternal;
			this.GrantProgramName = grantApplication.GrantOpening.GrantStream.GrantProgram.Name;
			this.GrantStreamName = grantApplication.GrantOpening.GrantStream.Name;
			this.GrantOpeningDate = grantApplication.GrantOpening.OpeningDate.ToLocalTime();
			this.GrantAgreementStartDate = grantApplication.GrantAgreement?.StartDate.AddDays(5).ToLocalTime();

			this.ShowReason = showReason;

			switch (grantApplication.ApplicationStateExternal)
			{
				case ApplicationStateExternal.CancelledByAgreementHolder:
					this.CancelText = "by you";
					break;
				case ApplicationStateExternal.CancelledByMinistry:
					this.CancelText = "by the Ministry";
					break;
			}

			if (grantApplication.ApplicationStateInternal.In(
				ApplicationStateInternal.AgreementRejected,
				ApplicationStateInternal.ApplicationDenied,
				ApplicationStateInternal.ApplicationWithdrawn,
				ApplicationStateInternal.CancelledByAgreementHolder,
				ApplicationStateInternal.CancelledByMinistry,
				ApplicationStateInternal.ChangeRequestDenied,
				ApplicationStateInternal.ClaimReturnedToApplicant,
				ApplicationStateInternal.ClaimDenied))
			{
				this.ReasonHeading = $"{grantApplication.ApplicationStateExternal.GetDescription()} Reason";
				this.Reason = grantApplication.GetReason(grantApplication.ApplicationStateInternal);
			}
		}
		#endregion
	}
}

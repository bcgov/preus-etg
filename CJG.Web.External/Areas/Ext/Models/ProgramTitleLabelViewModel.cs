using System;
using System.Linq;
using CJG.Core.Entities;
using CJG.Web.External.Helpers;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class ProgramTitleLabelViewModel
	{
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

		public ProgramTitleLabelViewModel()
		{
		}

		public ProgramTitleLabelViewModel(GrantApplication grantApplication, bool showReason = true)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			FileNumber = grantApplication.FileNumber;
			FileName = grantApplication.GetFileName();
			StartDate = grantApplication.TrainingPrograms.Any() ? grantApplication.StartDate.ToLocalTime() : (DateTime?)null;
			DateSubmitted = grantApplication.DateSubmitted?.ToLocalTime(); 
			ApplicationStateExternal = grantApplication.ApplicationStateExternal;
			GrantProgramName = grantApplication.GrantOpening.GrantStream.GrantProgram.Name;
			GrantStreamName = grantApplication.GrantOpening.GrantStream.Name;
			GrantOpeningDate = grantApplication.GrantOpening.OpeningDate.ToLocalTime();
			GrantAgreementStartDate = grantApplication.GrantAgreement?.StartDate.AddDays(5).ToLocalTime();

			ShowReason = showReason;

			switch (grantApplication.ApplicationStateExternal)
			{
				case ApplicationStateExternal.CancelledByAgreementHolder:
					CancelText = "by you";
					break;
				case ApplicationStateExternal.CancelledByMinistry:
					CancelText = "by the Ministry";
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
				ReasonHeading = $"{grantApplication.ApplicationStateExternal.GetDescription()} Reason";
				Reason = grantApplication.GetReason(grantApplication.ApplicationStateInternal);
			}
		}
	}
}

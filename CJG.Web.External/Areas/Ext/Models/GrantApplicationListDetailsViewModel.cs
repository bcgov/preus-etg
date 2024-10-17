using System;
using CJG.Core.Entities;
using CJG.Web.External.Helpers;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class GrantApplicationListDetailsViewModel
	{
		public int Id { get; set; }
		public string FileNumber { get; set; }
		public string FileName { get; set; }

		public bool ShowTrainingDate { get; set; }
		public string TrainingProgramStartDate { get; set; }

		public bool ShowSubmittedDate { get; set; }
		public string DateSubmitted { get; set; }
		public bool ShowGrantOpeningDate { get; set; }
		public string GrantOpeningDate { get; set; }
		public bool ShowAgreementStartDate { get; set; }
		public string AgreementStartDate { get; set; }


		public bool ShowOverviewLink { get; set; }
		public bool ShowViewLink { get; set; }
		public bool ShowReviewLink { get; set; }
		public bool ShowContinueGrantFilesLink { get; set; }
		public bool ShowViewGrantFilesLink { get; set; }

		public string StatusCssClass { get; set; }

		public ApplicationStateInternal ApplicationStateInternal { get; set; }
		public ApplicationStateExternal ApplicationStateExternal { get; set; }
		public string StatusText { get; set; }

		public string GrantProgramName { get; set; }
		public string GrantStreamName { get; set; }
		public string GrantProgramDescriptor { get; set; }
		public string RowVersion { get; set; }

		public GrantApplicationListDetailsViewModel(GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			Id = grantApplication.Id;
			FileNumber = grantApplication.FileNumber;
			FileName = grantApplication.GetFileName();
			GrantProgramName = grantApplication.GrantOpening.GrantStream.GrantProgram?.Name;
			GrantStreamName = grantApplication.GrantOpening.GrantStream.Name;

			GrantProgramDescriptor = GrantStreamName.ToLower() == "b.c. employer training grant" ?
											GrantProgramName:
											GrantProgramDescriptor = GrantProgramName + " - " + GrantStreamName;			

			ApplicationStateInternal = grantApplication.ApplicationStateInternal;
			ApplicationStateExternal = grantApplication.ApplicationStateExternal;
			RowVersion = Convert.ToBase64String(grantApplication.RowVersion);

			if (grantApplication.StartDate != null && grantApplication.StartDate != DateTime.Parse("1900-01-01"))
			{
				ShowTrainingDate = true;
				TrainingProgramStartDate = grantApplication.StartDate.ToStringLocalTime();
			}

			if (grantApplication.DateSubmitted != null && grantApplication.ApplicationStateExternal == ApplicationStateExternal.Submitted)
			{
				ShowSubmittedDate = true;
				DateSubmitted = grantApplication.DateSubmitted.Value.ToStringLocalTime();
			}
			else if (grantApplication.GrantOpening.OpeningDate != null
			  && (grantApplication.ApplicationStateExternal == ApplicationStateExternal.Incomplete || grantApplication.ApplicationStateExternal == ApplicationStateExternal.Complete))
			{
				ShowGrantOpeningDate = true;
				GrantOpeningDate = grantApplication.GrantOpening.OpeningDate.ToStringLocalTime();
			}
			else if (grantApplication.ApplicationStateExternal == ApplicationStateExternal.AcceptGrantAgreement && grantApplication.GrantAgreement != null)
			{
				ShowAgreementStartDate = true;
				AgreementStartDate = grantApplication.GrantAgreement.StartDate.AddDays(5).ToStringLocalTime();
			}

			SetStatusInfo(grantApplication.ApplicationStateExternal);
			ShowOverviewLink = grantApplication.ApplicationStateExternal.In(
									ApplicationStateExternal.NotStarted,
									ApplicationStateExternal.Incomplete,
									ApplicationStateExternal.Complete,
									ApplicationStateExternal.NotAccepted,
									ApplicationStateExternal.ApplicationWithdrawn);
			ShowViewLink = grantApplication.ApplicationStateExternal.In(
									ApplicationStateExternal.Submitted,
									ApplicationStateExternal.ApplicationDenied,
									ApplicationStateExternal.CancelledByMinistry,
									ApplicationStateExternal.CancelledByAgreementHolder,
									ApplicationStateExternal.AgreementWithdrawn,
									ApplicationStateExternal.AgreementRejected,
									ApplicationStateExternal.ReturnedUnassessed);
			ShowReviewLink = grantApplication.ApplicationStateExternal == ApplicationStateExternal.AcceptGrantAgreement;
			ShowContinueGrantFilesLink = grantApplication.ApplicationStateExternal.In(
									ApplicationStateExternal.Approved,
									ApplicationStateExternal.ChangeRequestSubmitted,
									ApplicationStateExternal.ChangeRequestApproved,
									ApplicationStateExternal.ChangeRequestDenied,
									ApplicationStateExternal.ClaimSubmitted,
									ApplicationStateExternal.ClaimApproved,
									ApplicationStateExternal.ClaimDenied,
									ApplicationStateExternal.ClaimReturned,
									ApplicationStateExternal.AmendClaim,
									ApplicationStateExternal.ReportCompletion);
			ShowViewGrantFilesLink = grantApplication.ApplicationStateExternal.In(ApplicationStateExternal.Closed);

		}

		private void SetStatusInfo(ApplicationStateExternal status)
		{
			switch (status)
			{
				case ApplicationStateExternal.NotStarted: StatusCssClass = "notstarted"; StatusText = "NOT STARTED"; break;
				case ApplicationStateExternal.Incomplete: StatusCssClass = "incomplete"; StatusText = "Incomplete"; break;
				case ApplicationStateExternal.Complete: StatusCssClass = "notsubmitted"; StatusText = "NOT SUBMITTED"; break;
				case ApplicationStateExternal.Submitted: StatusCssClass = "complete"; StatusText = "COMPLETE"; break;
				case ApplicationStateExternal.ApplicationWithdrawn: StatusCssClass = "notstarted"; StatusText = "APPLICATION WITHDRAWN"; break;
				case ApplicationStateExternal.Approved: StatusCssClass = "complete"; StatusText = "APPROVED"; break;
				case ApplicationStateExternal.ApplicationDenied: StatusCssClass = "notstarted"; StatusText = "APPLICATION DENIED"; break;
				case ApplicationStateExternal.CancelledByMinistry: StatusCssClass = "warning"; StatusText = "CANCELLED BY MINISTRY"; break;
				case ApplicationStateExternal.CancelledByAgreementHolder: StatusCssClass = "warning"; StatusText = "CANCELLED BY AGREEMENT HOLDER"; break;
				case ApplicationStateExternal.AcceptGrantAgreement: StatusCssClass = "danger"; StatusText = "ACCEPT GRANT AGREEMENT"; break;
				case ApplicationStateExternal.ChangeRequestSubmitted: StatusCssClass = "success"; StatusText = "CHANGE REQUEST SUBMITTED"; break;
				case ApplicationStateExternal.ChangeRequestApproved: StatusCssClass = "success"; StatusText = "CHANGE REQUEST APPROVED"; break;
				case ApplicationStateExternal.ChangeRequestDenied: StatusCssClass = "warning"; StatusText = "CHANGE REQUEST DENIED"; break;
				case ApplicationStateExternal.NotAccepted: StatusCssClass = "warning"; StatusText = "NOT ACCEPTED"; break;
				case ApplicationStateExternal.AgreementWithdrawn: StatusCssClass = "warning"; StatusText = "AGREEMENT WITHDRAWN"; break;
				case ApplicationStateExternal.AgreementRejected: StatusCssClass = "warning"; StatusText = "AGREEMENT REJECTED"; break;
				case ApplicationStateExternal.ClaimSubmitted: StatusCssClass = "warning"; StatusText = "CLAIM SUBMITTED"; break;
				case ApplicationStateExternal.ClaimReturned: StatusCssClass = "warning"; StatusText = "CLAIM RETURNED"; break;
				case ApplicationStateExternal.ClaimDenied: StatusCssClass = "warning"; StatusText = "CLAIM DENIED"; break;
				case ApplicationStateExternal.ClaimApproved: StatusCssClass = "complete"; StatusText = "CLAIM APPROVED"; break;
				case ApplicationStateExternal.AmendClaim: StatusCssClass = "warning"; StatusText = "AMEND CLAIM"; break;
				case ApplicationStateExternal.Closed: StatusCssClass = "warning"; StatusText = "CLOSED"; break;
				case ApplicationStateExternal.ReportCompletion: StatusCssClass = "complete"; StatusText = "REPORT COMPLETION"; break;
				case ApplicationStateExternal.ReturnedUnassessed: StatusCssClass = "warning"; StatusText = "RETURNED TO APPLICANT UNASSESSED"; break;
			}
		}
	}
}
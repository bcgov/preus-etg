using CJG.Core.Entities;
using CJG.Web.External.Helpers;
using System;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class GrantApplicationListDetailsViewModel
	{
		#region Properties
		public int Id { get; set; }
		public string FileNumber { get; set; }
		public string FileName { get; set; }

		public object Openingtimes { get; set; }
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
		public string RowVersion { get; set; }
		#endregion

		#region Constructors
		public GrantApplicationListDetailsViewModel(GrantApplication grantApplication)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));

			this.Id = grantApplication.Id;
			this.FileNumber = grantApplication.FileNumber;
			this.FileName = grantApplication.GetFileName();
			this.GrantProgramName = grantApplication.GrantOpening.GrantStream.GrantProgram?.Name;
			this.GrantStreamName = grantApplication.GrantOpening.GrantStream.Name;
			this.ApplicationStateInternal = grantApplication.ApplicationStateInternal;
			this.ApplicationStateExternal = grantApplication.ApplicationStateExternal;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);

			if (grantApplication.StartDate != null && grantApplication.StartDate != DateTime.Parse("1900-01-01"))
			{
				this.ShowTrainingDate = true;
				this.TrainingProgramStartDate = grantApplication.StartDate.ToStringLocalTime();
			}

			if (grantApplication.DateSubmitted != null && grantApplication.ApplicationStateExternal == ApplicationStateExternal.Submitted)
			{
				this.ShowSubmittedDate = true;
				this.DateSubmitted = grantApplication.DateSubmitted.Value.ToStringLocalTime();
			}
			else if (grantApplication.GrantOpening.OpeningDate != null
			  && (grantApplication.ApplicationStateExternal == ApplicationStateExternal.Incomplete || grantApplication.ApplicationStateExternal == ApplicationStateExternal.Complete))
			{
				this.ShowGrantOpeningDate = true;
				this.GrantOpeningDate = grantApplication.GrantOpening.OpeningDate.ToStringLocalTime();
			}
			else if (grantApplication.ApplicationStateExternal == ApplicationStateExternal.AcceptGrantAgreement && grantApplication.GrantAgreement != null)
			{
				this.ShowAgreementStartDate = true;
				this.AgreementStartDate = grantApplication.GrantAgreement.StartDate.AddDays(5).ToStringLocalTime();
			}

			this.SetStatusInfo(grantApplication.ApplicationStateExternal);
			this.ShowOverviewLink = grantApplication.ApplicationStateExternal.In<ApplicationStateExternal>(
									ApplicationStateExternal.NotStarted,
									ApplicationStateExternal.Incomplete,
									ApplicationStateExternal.Complete,
									ApplicationStateExternal.NotAccepted,
									ApplicationStateExternal.ApplicationWithdrawn);
			this.ShowViewLink = grantApplication.ApplicationStateExternal.In<ApplicationStateExternal>(
									ApplicationStateExternal.Submitted,
									ApplicationStateExternal.ApplicationDenied,
									ApplicationStateExternal.CancelledByMinistry,
									ApplicationStateExternal.CancelledByAgreementHolder,
									ApplicationStateExternal.AgreementWithdrawn,
									ApplicationStateExternal.AgreementRejected,
									ApplicationStateExternal.ReturnedUnassessed);
			this.ShowReviewLink = grantApplication.ApplicationStateExternal == ApplicationStateExternal.AcceptGrantAgreement;
			this.ShowContinueGrantFilesLink = grantApplication.ApplicationStateExternal.In<ApplicationStateExternal>(
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
			this.ShowViewGrantFilesLink = grantApplication.ApplicationStateExternal.In<ApplicationStateExternal>(
								ApplicationStateExternal.Closed);

		}
		#endregion

		#region Methods
		void SetStatusInfo(ApplicationStateExternal status)
		{
			switch (status)
			{
				case ApplicationStateExternal.NotStarted: this.StatusCssClass = "notstarted"; this.StatusText = "NOT STARTED"; break;
				case ApplicationStateExternal.Incomplete: this.StatusCssClass = "incomplete"; this.StatusText = "Incomplete"; break;
				case ApplicationStateExternal.Complete: this.StatusCssClass = "notsubmitted"; this.StatusText = "NOT SUBMITTED"; break;
				case ApplicationStateExternal.Submitted: this.StatusCssClass = "complete"; this.StatusText = "COMPLETE"; break;
				case ApplicationStateExternal.ApplicationWithdrawn: this.StatusCssClass = "notstarted"; this.StatusText = "APPLICATION WITHDRAWN"; break;
				case ApplicationStateExternal.Approved: this.StatusCssClass = "complete"; this.StatusText = "APPROVED"; break;
				case ApplicationStateExternal.ApplicationDenied: this.StatusCssClass = "notstarted"; this.StatusText = "APPLICATION DENIED"; break;
				case ApplicationStateExternal.CancelledByMinistry: this.StatusCssClass = "warning"; this.StatusText = "CANCELLED BY MINISTRY"; break;
				case ApplicationStateExternal.CancelledByAgreementHolder: this.StatusCssClass = "warning"; this.StatusText = "CANCELLED BY AGREEMENT HOLDER"; break;
				case ApplicationStateExternal.AcceptGrantAgreement: this.StatusCssClass = "danger"; this.StatusText = "ACCEPT GRANT AGREEMENT"; break;
				case ApplicationStateExternal.ChangeRequestSubmitted: this.StatusCssClass = "success"; this.StatusText = "CHANGE REQUEST SUBMITTED"; break;
				case ApplicationStateExternal.ChangeRequestApproved: this.StatusCssClass = "success"; this.StatusText = "CHANGE REQUEST APPROVED"; break;
				case ApplicationStateExternal.ChangeRequestDenied: this.StatusCssClass = "warning"; this.StatusText = "CHANGE REQUEST DENIED"; break;
				case ApplicationStateExternal.NotAccepted: this.StatusCssClass = "warning"; this.StatusText = "NOT ACCEPTED"; break;
				case ApplicationStateExternal.AgreementWithdrawn: this.StatusCssClass = "warning"; this.StatusText = "AGREEMENT WITHDRAWN"; break;
				case ApplicationStateExternal.AgreementRejected: this.StatusCssClass = "warning"; this.StatusText = "AGREEMENT REJECTED"; break;
				case ApplicationStateExternal.ClaimSubmitted: this.StatusCssClass = "warning"; this.StatusText = "CLAIM SUBMITTED"; break;
				case ApplicationStateExternal.ClaimReturned: this.StatusCssClass = "warning"; this.StatusText = "CLAIM RETURNED"; break;
				case ApplicationStateExternal.ClaimDenied: this.StatusCssClass = "warning"; this.StatusText = "CLAIM DENIED"; break;
				case ApplicationStateExternal.ClaimApproved: this.StatusCssClass = "complete"; this.StatusText = "CLAIM APPROVED"; break;
				case ApplicationStateExternal.AmendClaim: this.StatusCssClass = "warning"; this.StatusText = "AMEND CLAIM"; break;
				case ApplicationStateExternal.Closed: this.StatusCssClass = "warning"; this.StatusText = "CLOSED"; break;
				case ApplicationStateExternal.ReportCompletion: this.StatusCssClass = "complete"; this.StatusText = "REPORT COMPLETION"; break;
				case ApplicationStateExternal.ReturnedUnassessed: this.StatusCssClass = "warning"; this.StatusText = "RETURNED TO APPLICANT UNASSESSED"; break;

				default: break;
			}
		}
		#endregion
	}
}
using CJG.Core.Entities;
using System;
using System.Linq;
using System.Text;
using System.Web;

namespace CJG.Application.Services.Notifications
{
	public class NotificationViewModel
	{
		#region Properties
		public GrantApplication GrantApplication { get; set; }
		public User Applicant { get; set; }
		public int GrantApplicationId { get; set; }
		public string BaseURL { get; set; }
		public string RecipientFirstName { get; set; }
		public string RecipientLastName { get; set; }
		public string RecipientEmail { get; set; }
		public string ApplicantBusinessName { get; set; }
		public string ApplicantAddress { get; set; }
		public string FileNumber { get; set; }
		public string ProgramName { get; set; }
		public string ProgramCode { get; set; }
		public string ProgramEmail { get; set; }
		public string ProgramAbbreviation { get; set; }
		public string StreamName { get; set; }
		public string FullStreamName { get; set; }
		public string ProgramTitle { get; set; }
		public string TrainingPeriodStartDate { get; set; }
		public string TrainingPeriodEndDate { get; set; }
		public string ApplicationSubmitDate { get; set; }
		public string TrainingProgramTitle { get; set; }
		public string DeliveryStartDate { get; set; }
		public string DeliveryEndDate { get; set; }
		public string TrainingStartDate { get; set; }
		public string TrainingEndDate { get; set; }
		public int NumberOfParticipants { get; set; }
		public int MaximumNumberOfParticipants { get; set; }
		public int ParticipantsWithCompletionReport { get; set; }
		public string AgreementIssueDate { get; set; }
		public string AgreementAcceptanceDueDate { get; set; }
		public string ParticipantReportDueDate { get; set; }
		public string ClaimReportDueDate { get; set; }
		public string CompletionReportDueDate { get; set; }
		public string ChangeRequestDeniedReason { get; set; }
		public string ChangeRequestResults { get; set; }
		public string DeniedReason { get; set; }
		public string CancellationReason { get; set; }
		public decimal ReimbursementPayment { get; set; }
		public string ClaimDeniedReason { get; set; }
		public string ClaimReturnedReason { get; set; }
		public string ClaimApprovedReason { get; set; }
		public string NotificationTicketId { get; set; }
		public bool IsPayment { get; set; }
		#endregion

		#region Constructors
		public NotificationViewModel(GrantApplication grantApplication, User applicant, HttpContextBase httpContext = null)
		{
			this.GrantApplication = grantApplication ?? throw new ArgumentNullException(nameof(grantApplication));
			this.Applicant = applicant ?? throw new ArgumentNullException(nameof(applicant));

			this.GrantApplicationId = grantApplication.Id;
			this.BaseURL = GetBaseUrl(httpContext);
			this.NotificationTicketId = $"Notification-{grantApplication.Id}";

			this.RecipientFirstName = applicant.FirstName;
			this.RecipientLastName = applicant.LastName;
			this.RecipientEmail = applicant.EmailAddress;
			this.ApplicantBusinessName = grantApplication.OrganizationLegalName;
			this.ApplicantAddress = FormatAddressForHtml(grantApplication.ApplicantPhysicalAddress);

			this.FileNumber = grantApplication.FileNumber;
			this.ProgramName = grantApplication.GrantOpening?.GrantStream?.GrantProgram?.Name;
			this.ProgramCode = grantApplication.GrantOpening?.GrantStream?.GrantProgram?.ProgramCode;
			this.ProgramEmail = $"{this.ProgramCode}@gov.bc.ca";
			this.ProgramAbbreviation = grantApplication.GrantOpening?.GrantStream?.GrantProgram?.ProgramCode;
			this.StreamName = grantApplication.GrantOpening?.GrantStream?.Name;
			this.FullStreamName = grantApplication.GrantOpening?.GrantStream?.FullName;

			var defaultTrainingProgram = grantApplication.TrainingPrograms.FirstOrDefault(tp => (tp.EligibleCostBreakdown?.IsEligible ?? true));
			this.TrainingProgramTitle = defaultTrainingProgram?.CourseTitle;
			this.ProgramTitle = grantApplication.ProgramDescription?.Description ?? this.TrainingProgramTitle;
			this.TrainingPeriodStartDate = FormatDate(grantApplication.GrantOpening.TrainingPeriod.StartDate);
			this.TrainingPeriodEndDate = FormatDate(grantApplication.GrantOpening.TrainingPeriod.EndDate);
			this.ApplicationSubmitDate = FormatDate(grantApplication.DateSubmitted);
			this.DeliveryStartDate = FormatDate(grantApplication.StartDate);
			this.DeliveryEndDate = FormatDate(grantApplication.EndDate);
			this.TrainingStartDate = FormatDate(defaultTrainingProgram.StartDate);
			this.TrainingEndDate = FormatDate(defaultTrainingProgram.EndDate);
			this.NumberOfParticipants = grantApplication.ParticipantForms.Count();
			this.MaximumNumberOfParticipants = grantApplication.TrainingCost.AgreedParticipants;
			this.AgreementIssueDate = FormatDate(grantApplication.GrantAgreement?.StartDate);
			this.AgreementAcceptanceDueDate = FormatDate(grantApplication.GrantAgreement?.StartDate.AddDays(5));
			this.ParticipantReportDueDate = FormatDate(grantApplication.StartDate.AddDays(-5));
			this.ClaimReportDueDate = FormatDate(grantApplication.StartDate.AddDays(30));

			this.CompletionReportDueDate = FormatDate(grantApplication.EndDate.AddDays(30));
			this.ParticipantsWithCompletionReport = grantApplication.ParticipantForms.Where(pf => pf.ParticipantCompletionReportAnswers.Any()).Count();

			this.DeniedReason = grantApplication.GetReason(ApplicationStateInternal.ApplicationDenied);
			this.CancellationReason = grantApplication.GetReason(ApplicationStateInternal.CancelledByMinistry);
			this.IsPayment = IsReimbursementPayment(grantApplication);
			this.ReimbursementPayment = GetReimbursementPayment(grantApplication);
			this.ClaimDeniedReason = grantApplication.GetReason(ApplicationStateInternal.ClaimDenied);
			this.ClaimReturnedReason = grantApplication.GetReason(ApplicationStateInternal.ClaimReturnedToApplicant);
			this.ClaimApprovedReason = grantApplication.GetCurrentClaim()?.ClaimAssessmentNotes;

			var changeRequests = grantApplication.GetPreviousChangeRequest();
			if (grantApplication.ApplicationStateInternal == ApplicationStateInternal.ChangeRequestDenied)
			{
				var denied = changeRequests.Where(tp => tp.TrainingProviderState == TrainingProviderStates.Denied);
				this.ChangeRequestDeniedReason = grantApplication.GetReason(ApplicationStateInternal.ChangeRequestDenied);
				this.ChangeRequestResults = (denied.Count() > 0) ? $"Provider Changes Denied:<ul>{String.Join("", denied.Select(tp => $"<li>To \"{tp.Name}\" from \"{tp.GetPriorApproved()?.Name}\"</li>"))}</ul>" : "";
			}
			else
			{
				var denied = changeRequests.Where(tp => tp.TrainingProviderState == TrainingProviderStates.Denied);
				this.ChangeRequestResults = (denied.Count() > 0) ? $"Provider Changes Denied:<ul>{String.Join("", denied.Select(tp => $"<li>To \"{tp.Name}\" from \"{tp.GetPriorApproved()?.Name}\"</li>"))}</ul>" : "";

				var approved = changeRequests.Where(tp => tp.TrainingProviderState == TrainingProviderStates.Complete);
				this.ChangeRequestResults += (approved.Count() > 0) ? $"Provider Changes Approved:<ul>{String.Join("", approved.Select(tp => $"<li>To \"{tp.Name}\" from \"{tp.GetPriorApproved()?.Name}\"</li>"))}</ul>" : "";
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Format the date as a string.
		/// </summary>
		/// <param name="dateValue"></param>
		/// <returns></returns>
		private static string FormatDate(DateTime? dateValue)
		{
			return dateValue?.ToLocalTime().ToString("yyyy-MM-dd") ?? "<Unknown>";
		}

		/// <summary>
		/// Get the reimbursement payment amount.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		private static decimal GetReimbursementPayment(GrantApplication grantApplication)
		{
			var claim = grantApplication.GetCurrentClaim();
			return Math.Abs(claim?.AmountPaidOrOwing() ?? 0);
		}

		/// <summary>
		/// Determine if the current claim has a positive amount paid.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		private static bool IsReimbursementPayment(GrantApplication grantApplication)
		{
			var claim = grantApplication.GetCurrentClaim();
			return (claim?.AmountPaidOrOwing() ?? 0) >= 0;
		}

		/// <summary>
		/// Get the base url of the current application.
		/// </summary>
		/// <returns></returns>
		internal virtual string GetBaseUrl(HttpContextBase httpContext)
		{
			return httpContext?.Request.Url.GetLeftPart(UriPartial.Authority) ?? "EMPTY";
		}

		/// <summary>
		/// Formats the address for HTML.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		private static string FormatAddressForHtml(ApplicationAddress address)
		{
			if (address == null)
				return null;

			var str = new StringBuilder();

			str.Append(address.AddressLine1);
			str.Append("<br/>");
			if (!string.IsNullOrWhiteSpace(address.AddressLine2))
			{
				str.Append(address.AddressLine2);
				str.Append("<br/>");
			}
			if (!string.IsNullOrWhiteSpace(address.City))
			{
				str.Append(address.City);
				str.Append("<br/>");
			}
			if (address.Region != null)
			{
				str.Append(address.Region.Name);
				str.Append("<br/>");
			}
			if (address.Country != null)
			{
				str.Append(address.Country.Name);
				str.Append("<br/>");
			}
			if (!string.IsNullOrWhiteSpace(address.PostalCode))
			{
				str.Append(address.PostalCode);
				str.Append("<br/>");
			}

			return str.ToString();
		}
		#endregion
	}
}

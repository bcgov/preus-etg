using System;
using System.Linq;
using System.Text;
using System.Web;
using CJG.Core.Entities;

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
			GrantApplication = grantApplication ?? throw new ArgumentNullException(nameof(grantApplication));
			Applicant = applicant ?? throw new ArgumentNullException(nameof(applicant));

			GrantApplicationId = grantApplication.Id;
			BaseURL = GetBaseUrl(httpContext);
			NotificationTicketId = $"Notification-{grantApplication.Id}";

			RecipientFirstName = applicant.FirstName;
			RecipientLastName = applicant.LastName;
			RecipientEmail = applicant.EmailAddress;
			ApplicantBusinessName = grantApplication.OrganizationLegalName;
			ApplicantAddress = FormatAddressForHtml(grantApplication.ApplicantPhysicalAddress);

			FileNumber = grantApplication.FileNumber;
			ProgramName = grantApplication.GrantOpening?.GrantStream?.GrantProgram?.Name;
			ProgramCode = grantApplication.GrantOpening?.GrantStream?.GrantProgram?.ProgramCode;
			ProgramEmail = $"{ProgramCode}@gov.bc.ca";
			ProgramAbbreviation = grantApplication.GrantOpening?.GrantStream?.GrantProgram?.ProgramCode;
			StreamName = grantApplication.GrantOpening?.GrantStream?.Name;
			FullStreamName = grantApplication.GrantOpening?.GrantStream?.FullName;

			var defaultTrainingProgram = grantApplication.TrainingPrograms.FirstOrDefault(tp => (tp.EligibleCostBreakdown?.IsEligible ?? true));
			TrainingProgramTitle = defaultTrainingProgram?.CourseTitle;
			ProgramTitle = grantApplication.ProgramDescription?.Description ?? TrainingProgramTitle;
			TrainingPeriodStartDate = FormatDate(grantApplication.GrantOpening?.TrainingPeriod?.StartDate);
			TrainingPeriodEndDate = FormatDate(grantApplication.GrantOpening?.TrainingPeriod?.EndDate);
			ApplicationSubmitDate = FormatDate(grantApplication.DateSubmitted);
			DeliveryStartDate = FormatDate(grantApplication.StartDate);
			DeliveryEndDate = FormatDate(grantApplication.EndDate);
			TrainingStartDate = FormatDate(defaultTrainingProgram?.StartDate);
			TrainingEndDate = FormatDate(defaultTrainingProgram?.EndDate);
			NumberOfParticipants = grantApplication.ParticipantForms.Count();
			MaximumNumberOfParticipants = grantApplication.TrainingCost.AgreedParticipants;
			AgreementIssueDate = FormatDate(grantApplication.GrantAgreement?.StartDate);
			AgreementAcceptanceDueDate = FormatDate(grantApplication.GrantAgreement?.StartDate.AddDays(5));
			ParticipantReportDueDate = FormatDate(grantApplication.GetParticipantReportingDueDate());
			ClaimReportDueDate = FormatDate(grantApplication.StartDate.AddDays(30));

			CompletionReportDueDate = FormatDate(grantApplication.EndDate.AddDays(30));
			ParticipantsWithCompletionReport = grantApplication.ParticipantForms.Where(pf => pf.ParticipantCompletionReportAnswers.Any()).Count();

			DeniedReason = grantApplication.GetReason(ApplicationStateInternal.ApplicationDenied);
			CancellationReason = grantApplication.GetReason(ApplicationStateInternal.CancelledByMinistry);
			IsPayment = IsReimbursementPayment(grantApplication);
			ReimbursementPayment = GetReimbursementPayment(grantApplication);
			ClaimDeniedReason = grantApplication.GetReason(ApplicationStateInternal.ClaimDenied);
			ClaimReturnedReason = grantApplication.GetReason(ApplicationStateInternal.ClaimReturnedToApplicant);
			ClaimApprovedReason = grantApplication.GetCurrentClaim()?.ClaimAssessmentNotes;

			var changeRequests = grantApplication.GetPreviousChangeRequest().ToList();
			if (grantApplication.ApplicationStateInternal == ApplicationStateInternal.ChangeRequestDenied)
			{
				var denied = changeRequests.Where(tp => tp.TrainingProviderState == TrainingProviderStates.Denied).ToList();
				ChangeRequestDeniedReason = grantApplication.GetReason(ApplicationStateInternal.ChangeRequestDenied);
				ChangeRequestResults = denied.Any() ? $"Provider Changes Denied:<ul>{string.Join("", denied.Select(tp => $"<li>To \"{tp.Name}\" from \"{tp.GetPriorApproved()?.Name}\"</li>"))}</ul>" : "";
			}
			else
			{
				var denied = changeRequests.Where(tp => tp.TrainingProviderState == TrainingProviderStates.Denied).ToList();
				ChangeRequestResults = denied.Any() ? $"Provider Changes Denied:<ul>{string.Join("", denied.Select(tp => $"<li>To \"{tp.Name}\" from \"{tp.GetPriorApproved()?.Name}\"</li>"))}</ul>" : "";

				var approved = changeRequests.Where(tp => tp.TrainingProviderState == TrainingProviderStates.Complete).ToList();
				ChangeRequestResults += approved.Any() ? $"Provider Changes Approved:<ul>{string.Join("", approved.Select(tp => $"<li>To \"{tp.Name}\" from \"{tp.GetPriorApproved()?.Name}\"</li>"))}</ul>" : "";
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

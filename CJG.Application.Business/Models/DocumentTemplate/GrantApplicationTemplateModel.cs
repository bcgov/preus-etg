using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CJG.Application.Business.Models.DocumentTemplate
{
	public class GrantApplicationTemplateModel
	{
		#region Properties
		public int Id { get; set; }
		public string FileNumber { get; set; }
		public string ApplicantFirstName { get; set; }
		public string ApplicantLastName { get; set; }
		public string ApplicantPhysicalAddress { get; set; }
		public string ApplicantPhoneNumber { get; set; }
		public string ReimbursementRate { get; set; }
		public string MaxReimbursementAmt { get; set; }
		public string StartDate { get; set; }
		public string EndDate { get; set; }

		public string OrganizationLegalName { get; set; }

		public string GrantProgramName { get; set; }
		public string GrantProgramCode { get; set; }
		public string GrantStreamName { get; set; }
		public string GrantProgramEmail { get; set; }

		public string GrantAgreementStartDate { get; set; }
		public string GrantAgreementEndDate { get; set; }
		public string GrantAgreementParticipantReportingDueDate { get; set; }
		public string GrantAgreementReimbursementClaimDueDate { get; set; }

		public ProgramTypes ProgramType { get; set; }
		public bool ShowAgreedCosts { get; set; }
		public bool ShowEmployerContribution { get; set; }
		public string BaseURL { get; set; }

		public IEnumerable<TrainingProgramTemplateModel> TrainingPrograms { get; set; }
		public TrainingCostTemplateModel TrainingCost { get; set; }

		[RegularExpression("^[a-zA-Z '-]*$", ErrorMessage = "Invalid Format")]
		public string AlternateJobTitle { get; set; }
		public string AlternatePhoneExtension { get; set; }
		public string AlternatePhoneNumber { get; set; }

		[RegularExpression("^[a-zA-Z '-]*$", ErrorMessage = "Invalid Format")]
		public string AlternateLastName { get; set; }

		[RegularExpression("^[a-zA-Z '-]*$", ErrorMessage = "Invalid Format")]
		public string AlternateFirstName { get; set; }

		[RegularExpression("^[a-zA-Z '-]*$", ErrorMessage = "Invalid Format")]
		public string AlternateSalutation { get; set; }

		[RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
		public string AlternateEmail { get; set; }
		public string AlternatePhysicalAddress { get; set; }
		public string AlternateMailingAddress { get; set; }
		public bool IsAlternateContact { get; set; }
		public string ClaimDeadline { get; set; }
		public string AgreementFiscalYear { get; set; }
		public List<ParticipantFormModel> Participants { get; set; }
		public bool RequireAllParticipantsBeforeSubmission { get; set; }
		#endregion

		#region Constructors
		public GrantApplicationTemplateModel()
		{
		}

		public GrantApplicationTemplateModel(GrantApplication grantApplication, Action<GrantApplicationTemplateModel, GrantApplication> additionalSetup = null)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			this.FileNumber = grantApplication.FileNumber;
			this.ApplicantFirstName = grantApplication.ApplicantFirstName;
			this.ApplicantLastName = grantApplication.ApplicantLastName;
			this.ApplicantPhysicalAddress = grantApplication.ApplicantPhysicalAddress.ToString();
			this.ApplicantPhoneNumber = grantApplication.ApplicantPhoneNumber;
			this.MaxReimbursementAmt = grantApplication.MaxReimbursementAmt.ToString("C0");
			this.StartDate = grantApplication.StartDate.ToLocalTime().ToString("MMMM dd, yyyy");
			this.EndDate = grantApplication.EndDate.ToLocalTime().ToString("MMMM dd, yyyy");

			this.OrganizationLegalName = grantApplication.Organization.LegalName;

			this.GrantProgramName = grantApplication.GrantOpening.GrantStream.GrantProgram.Name;
			this.GrantProgramCode = grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramCode;
			this.GrantStreamName = grantApplication.GrantOpening.GrantStream.Name;
			this.GrantProgramEmail = $"{this.GrantProgramCode}@gov.bc.ca";

			this.GrantAgreementStartDate = grantApplication.GrantAgreement?.StartDate.ToLocalTime().ToString("MMMM dd, yyyy");
			this.GrantAgreementEndDate = grantApplication.GrantAgreement?.ConvertEndDateToLocalTime().ToString("MMMM dd, yyyy");
			this.GrantAgreementParticipantReportingDueDate = grantApplication.GrantAgreement?.ParticipantReportingDueDate.ToLocalTime().ToString("MMMM dd, yyyy");
			this.GrantAgreementReimbursementClaimDueDate = grantApplication.GrantAgreement?.ReimbursementClaimDueDate.ToLocalTime().ToString("MMMM dd, yyyy");

			this.ProgramType = grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId;
			this.ShowAgreedCosts = grantApplication.TrainingPrograms.FirstOrDefault().GrantApplication.ApplicationStateInternal.ShowAgreedCosts();
			this.ShowEmployerContribution = !((grantApplication.ReimbursementRate == 1) && (grantApplication.CalculateAgreedEmployerContribution() == 0));

			this.TrainingPrograms = grantApplication.TrainingPrograms.Where(tp => tp.EligibleCostBreakdown?.IsEligible ?? true).Select(o => new TrainingProgramTemplateModel(o)).ToArray();
			this.TrainingCost = new TrainingCostTemplateModel(grantApplication.TrainingCost, this.ShowAgreedCosts);

			this.AlternateJobTitle = grantApplication.AlternateJobTitle;
			this.AlternatePhoneExtension = grantApplication.AlternatePhoneExtension;
			this.AlternatePhoneNumber = grantApplication.AlternatePhoneNumber;
			this.AlternateLastName = grantApplication.AlternateLastName;
			this.AlternateFirstName = grantApplication.AlternateFirstName;
			this.AlternateSalutation = grantApplication.AlternateSalutation;
			this.AlternateEmail = grantApplication.AlternateEmail;
			this.IsAlternateContact = grantApplication.IsAlternateContact == null ? false : grantApplication.IsAlternateContact.Value;
			this.ClaimDeadline = grantApplication.GrantAgreement?.GetClaimSubmissionDeadline().ToString("MMMM dd, yyyy");
			this.AgreementFiscalYear = $"{grantApplication.GrantOpening.TrainingPeriod.FiscalYear.StartDate.ToLocalTime().ToString("MMMM dd, yyyy")} to {grantApplication.GrantOpening.TrainingPeriod.FiscalYear.EndDate.ToLocalTime().ToString("MMMM dd, yyyy")}";
			this.RequireAllParticipantsBeforeSubmission = grantApplication.RequireAllParticipantsBeforeSubmission;
			Participants = new List<ParticipantFormModel>();

			if (this.RequireAllParticipantsBeforeSubmission)
			{
				foreach (var participantForm in grantApplication.ParticipantForms.Where(w => w.Approved == true))
				{
					Participants.Add(new ParticipantFormModel(participantForm));
				}
			}
			else
			{
				foreach (var participantForm in grantApplication.ParticipantForms)
				{
					Participants.Add(new ParticipantFormModel(participantForm));
				}
			}

			additionalSetup?.Invoke(this, grantApplication);
		}
		#endregion
	}
}
using CJG.Core.Entities;
using CJG.Core.Entities.Attributes;
using CJG.Web.External.Areas.Ext.Models.TrainingPrograms;
using CJG.Web.External.Areas.Int.Models;
using CJG.Web.External.Helpers;
using CJG.Web.External.Models.Shared;
using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class GrantApplicationViewModel : BaseViewModel
	{
		#region Properties
		public int GrantOpeningId { get; set; }

		[NotMapped]
		public virtual GrantOpeningViewModel GrantOpening { get; set; }

		public int ApplicationTypeId { get; set; }

		[NotMapped]
		public virtual ApplicationType ApplicationType { get; set; }

		public int? AssessorId { get; set; }

		[NotMapped]
		public virtual InternalUser Assessor { get; set; }

		[MaxLength(50)]
		public string FileNumber { get; set; }

		public string FileName { get; set; }

		[Required, DefaultValue(ApplicationStateExternal.Incomplete)]
		public ApplicationStateExternal ApplicationStateExternal { get; set; } = ApplicationStateExternal.Incomplete;
		public ApplicationStateViewModel ApplicationStateExternalViewModel { get; set; }
		[Required, DefaultValue(ApplicationStateInternal.Draft)]
		public ApplicationStateInternal ApplicationStateInternal { get; set; } = ApplicationStateInternal.Draft;
		public ApplicationStateViewModel ApplicationStateInternalViewModel { get; set; }

		public bool HasAppliedForGrantBefore { get; set; }

		public bool WouldTrainEmployeesWithoutGrant { get; set; }

		public bool HostingTrainingProgram { get; set; }
		[Required]
		public Guid ApplicantBCeID { get; set; }

		[MaxLength(250)]
		public string ApplicantSalutation { get; set; }

		[Required, MaxLength(250)]
		[NameValidation]
		public string ApplicantFirstName { get; set; }

		[Required, MaxLength(250)]
		[NameValidation]
		public string ApplicantLastName { get; set; }

		[Required, MaxLength(20)]
		[RegularExpression(@"\(?[0-9]{3}\)?\s?[0-9]{3}\-?[0-9]{4}")]
		public string ApplicantPhoneNumber { get; set; }

		[MaxLength(20)]
		[RegularExpression(@"[0-9]*")]
		public string ApplicantPhoneExtension { get; set; }

		[MaxLength(500)]
		public string ApplicantJobTitle { get; set; }

		public int ApplicantPhysicalAddressId { get; set; }

		public virtual ApplicationAddressViewModel ApplicantPhysicalAddress { get; set; }

		public int? ApplicantMailingAddressId { get; set; }

		public virtual ApplicationAddressViewModel ApplicantMailingAddress { get; set; }

		public int OrganizationId { get; set; }

		[NotMapped]
		public virtual OrganizationViewModel Organization { get; set; }

		public Guid? OrganizationBCeID { get; set; }

		public int? OrganizationAddressId { get; set; }

		public virtual ApplicationAddressViewModel OrganizationAddress { get; set; }

		public int? OrganizationTypeId { get; set; }

		[NotMapped]
		public virtual OrganizationType OrganizationType { get; set; }

		public int? OrganizationLegalStructureId { get; set; }

		[NotMapped]
		public virtual LegalStructure OrganizationLegalStructure { get; set; }

		public int? OrganizationYearEstablished { get; set; }

		public int? OrganizationNumberOfEmployeesWorldwide { get; set; }

		public int? OrganizationNumberOfEmployeesInBC { get; set; }

		public decimal? OrganizationAnnualTrainingBudget { get; set; }

		public int? OrganizationAnnualEmployeesTrained { get; set; }

		[MaxLength(250)]
		public string OrganizationLegalName { get; set; }

		public int? PrioritySectorId { get; set; }

		[NotMapped]
		public virtual PrioritySector PrioritySector { get; set; }

		[MaxLength(500)]
		public string OrganizationDoingBusinessAs { get; set; }

		public int? NaicsId { get; set; }

		[NotMapped]
		public virtual NaIndustryClassificationSystem Naics { get; set; }

		[NotMapped]
		public virtual GrantAgreementViewModel GrantAgreement { get; set; }

		public DateTime? DateSubmitted { get; set; }

		public DateTime? DateUpdated { get; set; }


		public string WithdrawReason { get; set; }

		public string ReturnedToApplicantReason { get; set; }

		[MaxLength(500)]
		public string AssessorNote { get; set; }

		[Required, Min(0, ErrorMessage = "The maximum reimbursement amount must be greater than or equal to 0.")]
		public decimal MaxReimbursementAmt { get; set; }

		[Required, Min(0, ErrorMessage = "The reimbursement rate must be greater than or equal to 0."), Max(1, ErrorMessage = "The reimbursement rate must be less than or equal to 1.")]
		public double ReimbursementRate { get; set; }

		public RiskClassification RiskClassification { get; set; }

		public IEnumerable<GrantApplicationStateChange> StateChanges { get; set; }
		public IEnumerable<TrainingProgramViewModel> TrainingPrograms { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		public string RowVersion { get; set; }
		public int MaxParticipants { get; set; }
		public bool CanApplicantReportParticipants { get; set; }

		public bool? IsAlternateContact { get; set; }

		[MaxLength(250)]
		public string AlternateSalutation { get; set; }

		[MaxLength(250)]
		[NameValidation]
		public string AlternateFirstName { get; set; }

		[MaxLength(250)]
		[NameValidation]
		public string AlternateLastName { get; set; }

		[MaxLength(20)]
		[RegularExpression(@"\(?[0-9]{3}\)?\s?[0-9]{3}\-?[0-9]{4}")]
		public string AlternatePhoneNumber { get; set; }

		[MaxLength(20)]
		[RegularExpression(@"[0-9]*")]
		public string AlternatePhoneExtension { get; set; }

		[MaxLength(500)]
		public string AlternateJobTitle { get; set; }

		[MaxLength(500)]
		public string AlternateEmail { get; set; }

		public int? AlternatePhysicalAddressId { get; set; }

		public virtual ApplicationAddressViewModel AlternatePhysicalAddress { get; set; }

		public int? AlternateMailingAddressId { get; set; }

		public virtual ApplicationAddressViewModel AlternateMailingAddress { get; set; }

		public IEnumerable<KeyValuePair<string, string>> Provinces { get; set; }

		#endregion

		#region Constructors
		public GrantApplicationViewModel()
		{ }

		public GrantApplicationViewModel(GrantApplication grantApplication)
		{
			this.Id = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.ApplicationTypeId = grantApplication.ApplicationTypeId;
			this.ApplicationType = grantApplication.ApplicationType;

			this.AssessorId = grantApplication.AssessorId;
			this.Assessor = grantApplication.GetStateChange(ApplicationStateInternal.UnderAssessment)?.Assessor;
			this.ApplicationStateInternal = grantApplication.ApplicationStateInternal;
			this.ApplicationStateExternal = grantApplication.ApplicationStateExternal;
			var grantStream = grantApplication.GrantOpening.GrantStream;

			this.FileNumber = grantApplication.FileNumber;
			this.FileName = grantApplication.GetFileName();
			this.DateSubmitted = grantApplication.DateSubmitted?.ToLocalTime();
			this.DateUpdated = grantApplication.DateUpdated?.ToLocalTime();
			this.RiskClassification = grantApplication.RiskClassification;
			this.CanApplicantReportParticipants = grantApplication.CanApplicantReportParticipants;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.StartDate = grantApplication.StartDate.ToLocalTime();
			this.EndDate = grantApplication.EndDate.ToLocalTime();
			this.StateChanges = grantApplication.StateChanges.ToList();
			this.ReturnedToApplicantReason = grantApplication.GetClaimReturnedToApplicantReason();
			this.TrainingPrograms = grantApplication.TrainingPrograms.Select(tp => new TrainingProgramViewModel(tp));

			this.HasAppliedForGrantBefore = grantApplication.HasAppliedForGrantBefore;
			this.WouldTrainEmployeesWithoutGrant = grantApplication.WouldTrainEmployeesWithoutGrant;
			this.HostingTrainingProgram = grantApplication.HostingTrainingProgram;
			this.ApplicantBCeID = grantApplication.ApplicantBCeID;
			this.ApplicantSalutation = grantApplication.ApplicantSalutation;
			this.ApplicantFirstName = grantApplication.ApplicantFirstName;
			this.ApplicantLastName = grantApplication.ApplicantLastName;
			this.ApplicantPhoneNumber = grantApplication.ApplicantPhoneNumber;
			this.ApplicantPhoneExtension = grantApplication.ApplicantPhoneExtension;
			this.ApplicantJobTitle = grantApplication.ApplicantJobTitle;

			this.ApplicantPhysicalAddressId = grantApplication.ApplicantPhysicalAddressId;
			this.ApplicantPhysicalAddress = grantApplication.ApplicantPhysicalAddress != null ? new ApplicationAddressViewModel(grantApplication.ApplicantPhysicalAddress) : null;

			this.ApplicantMailingAddressId = grantApplication.ApplicantMailingAddressId;
			if (grantApplication.ApplicantMailingAddressId == null) { grantApplication.ApplicantMailingAddressId = 0; }
			this.ApplicantMailingAddress = grantApplication.ApplicantMailingAddress != null ? new ApplicationAddressViewModel(grantApplication.ApplicantMailingAddress) : null;

			this.AlternateSalutation = grantApplication.AlternateSalutation;
			this.AlternateFirstName = grantApplication.AlternateFirstName;
			this.AlternateLastName = grantApplication.AlternateLastName;
			this.AlternatePhoneNumber = grantApplication.AlternatePhoneNumber;
			this.AlternatePhoneExtension = grantApplication.AlternatePhoneExtension;
			this.AlternateJobTitle = grantApplication.AlternateJobTitle;
			this.AlternateEmail = grantApplication.AlternateEmail;

			//this.AlternatePhysicalAddressId = grantApplication.AlternatePhysicalAddressId;
			//this.AlternatePhysicalAddress = grantApplication.AlternatePhysicalAddress != null ? new ApplicationAddressViewModel(grantApplication.AlternatePhysicalAddress) : null;
			//if (grantApplication.AlternatePhysicalAddressId == null) { grantApplication.AlternatePhysicalAddressId = 0; }

			//this.AlternateMailingAddressId = grantApplication.AlternateMailingAddressId;
			//if (grantApplication.AlternateMailingAddressId == null) { grantApplication.AlternateMailingAddressId = 0; }
			//this.AlternateMailingAddress = grantApplication.AlternateMailingAddress != null ? new ApplicationAddressViewModel(grantApplication.AlternateMailingAddress) : null;

			this.ApplicationStateInternalViewModel = new ApplicationStateViewModel
			{
				Id = (int)ApplicationStateInternal,
				Name = ApplicationStateInternal.ToString(),
				Description = ApplicationStateInternal.GetDescription()
			};

			this.ApplicationStateExternalViewModel = new ApplicationStateViewModel
			{
				Id = (int)ApplicationStateExternal,
				Name = ApplicationStateExternal.ToString(),
				Description = ApplicationStateExternal.GetDescription()
			};

			this.OrganizationId = grantApplication.OrganizationId;
			this.Organization = grantApplication.Organization != null ? new OrganizationViewModel(grantApplication.Organization) : null;
			this.OrganizationBCeID = grantApplication.OrganizationBCeID;
			this.OrganizationAddressId = grantApplication.OrganizationAddressId;
			this.OrganizationAddress = grantApplication.OrganizationAddress != null ? new ApplicationAddressViewModel(grantApplication.OrganizationAddress) : null;
			this.OrganizationTypeId = grantApplication.OrganizationTypeId;
			this.OrganizationType = grantApplication.OrganizationType;
			this.OrganizationLegalStructureId = grantApplication.OrganizationLegalStructureId;
			this.OrganizationLegalStructure = grantApplication.OrganizationLegalStructure;
			this.OrganizationYearEstablished = grantApplication.OrganizationYearEstablished;
			this.OrganizationNumberOfEmployeesWorldwide = grantApplication.OrganizationNumberOfEmployeesWorldwide;
			this.OrganizationNumberOfEmployeesInBC = grantApplication.OrganizationNumberOfEmployeesInBC;
			this.OrganizationAnnualTrainingBudget = grantApplication.OrganizationAnnualTrainingBudget;
			this.OrganizationAnnualEmployeesTrained = grantApplication.OrganizationAnnualEmployeesTrained;
			this.OrganizationLegalName = grantApplication.OrganizationLegalName;
			this.PrioritySectorId = grantApplication.PrioritySectorId;
			this.OrganizationDoingBusinessAs = grantApplication.OrganizationDoingBusinessAs;

			this.GrantOpeningId = grantApplication.GrantOpeningId;
			this.GrantOpening = new GrantOpeningViewModel(grantApplication.GrantOpening);

			this.Naics = grantApplication.NAICS;
			this.NaicsId = grantApplication.NAICSId;
			this.GrantAgreement = grantApplication.GrantAgreement != null ? new GrantAgreementViewModel(grantApplication.GrantAgreement) : null;
			this.WithdrawReason = grantApplication.GetReason(ApplicationStateInternal.ApplicationWithdrawn, ApplicationStateInternal.OfferWithdrawn);
			this.AssessorNote = grantApplication.AssessorNote;
			this.MaxReimbursementAmt = grantApplication.MaxReimbursementAmt;
			this.ReimbursementRate = grantApplication.ReimbursementRate;

			this.MaxParticipants = grantApplication.TrainingCost.GetMaxParticipants();
		}
		#endregion
	}
}
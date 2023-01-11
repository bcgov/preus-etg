using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using CJG.Core.Entities.Attributes;
using DataAnnotationsExtensions;

namespace CJG.Core.Entities
{
	/// <summary>
	/// GrantApplication class, provides ORM for grant application information.
	/// </summary>
	public class GrantApplication : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - Primary key uses IDENTITY.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - Foreign key to the grant opening.
		/// </summary>
		[Index("IX_GrantApplication", 3)]
		public int GrantOpeningId { get; set; }

		/// <summary>
		/// get/set - The grant opening this grant application belongs to.
		/// </summary>
		[ForeignKey(nameof(GrantOpeningId))]
		public virtual GrantOpening GrantOpening { get; set; }

		/// <summary>
		/// get/set - Foreign key to the application type.
		/// </summary>
		[Index("IX_GrantApplication", 6)]
		public int ApplicationTypeId { get; set; }

		/// <summary>
		/// get/set - The application type.
		/// </summary>
		[ForeignKey(nameof(ApplicationTypeId))]
		public virtual ApplicationType ApplicationType { get; set; }

		/// <summary>
		/// get/set - Foreign key to the assessor.
		/// </summary>
		[Index("IX_GrantApplication", 4)]
		public int? AssessorId { get; set; }

		/// <summary>
		/// get/set - The assessor assigned to this grant application.
		/// </summary>
		[ForeignKey(nameof(AssessorId))]
		public virtual InternalUser Assessor { get; set; }

		/// <summary>
		/// get/set - The unqiue filenumber assigned to this grant application.
		/// </summary>
		[MaxLength(50)]
		public string FileNumber { get; set; }

		/// <summary>
		/// get/set - The external state of the application (this property was poorly designed, there should only be a single state for the application).
		/// </summary>
		[Required, DefaultValue(ApplicationStateExternal.Incomplete)]
		[Index("IX_GrantApplication", 2)]
		public ApplicationStateExternal ApplicationStateExternal { get; set; } = ApplicationStateExternal.Incomplete;

		/// <summary>
		/// get/set - The internal state of the application.
		/// </summary>
		[Required, DefaultValue(ApplicationStateInternal.Draft)]
		[Index("IX_GrantApplication", 1)]
		public ApplicationStateInternal ApplicationStateInternal { get; set; } = ApplicationStateInternal.Draft;

		/// <summary>
		/// get/set - Whether the applicant has applied for this grant before.
		/// </summary>
		public bool HasAppliedForGrantBefore { get; set; }

		/// <summary>
		/// get/set - Whether the applicant would train even without the grant.
		/// </summary>
		public bool WouldTrainEmployeesWithoutGrant { get; set; }

		/// <summary>
		/// get/set - Whether the applicant is hosting the training program.
		/// </summary>
		public bool HostingTrainingProgram { get; set; }

		/// <summary>
		/// get/set - The applicant BCeID.  This is used for historical reasons, it is copied from the applicant.
		/// </summary>
		[Required]
		public Guid ApplicantBCeID { get; set; }

		/// <summary>
		/// get/set - The applicant salutation.  This is used for historical reasons, it is copied from the applicant.
		/// </summary>
		[MaxLength(250)]
		public string ApplicantSalutation { get; set; }

		/// <summary>
		/// get/set - The applicant first name.  This is used for historical reasons, it is copied from the applicant.
		/// </summary>
		[Required, MaxLength(250)]
		public string ApplicantFirstName { get; set; }

		/// <summary>
		/// get/set - The applicant last name.  This is used for historical reasons, it is copied from the applicant.
		/// </summary>
		[Required, MaxLength(250)]
		public string ApplicantLastName { get; set; }

		/// <summary>
		/// get/set - The applicant phone number.  This is used for historical reasons, it is copied from the applicant.
		/// </summary>
		[Required, MaxLength(20)]
		[RegularExpression(@"\(?[0-9]{3}\)?\s?[0-9]{3}\-?[0-9]{4}")]
		public string ApplicantPhoneNumber { get; set; }

		/// <summary>
		/// get/set - The applicant phone extension.  This is used for historical reasons, it is copied from the applicant.
		/// </summary>
		[MaxLength(20)]
		[RegularExpression(@"[0-9]*")]
		public string ApplicantPhoneExtension { get; set; }

		/// <summary>
		/// get/set - The applicant job title.  This is used for historical reasons, it is copied from the applicant.
		/// </summary>
		[MaxLength(500)]
		public string ApplicantJobTitle { get; set; }

		/// <summary>
		/// get/set - Foreign key to an application address. The applicant physical address.  This is used for historical reasons, it is copied from the applicant.
		/// </summary>
		public int ApplicantPhysicalAddressId { get; set; }

		/// <summary>
		/// get/set - The applicant physical address.  This is used for historical reasons, it is copied from the applicant.
		/// </summary>
		[ForeignKey(nameof(ApplicantPhysicalAddressId))]
		public virtual ApplicationAddress ApplicantPhysicalAddress { get; set; }

		/// <summary>
		/// get/set - Foreign key to an application address.  The applicant mailing address.  This is used for historical reasons, it is copied from the applicant.
		/// </summary>
		public int? ApplicantMailingAddressId { get; set; }

		/// <summary>
		/// get/set - The applicant mailing address.  This is used for historical reasons, it is copied from the applicant.
		/// </summary>
		[ForeignKey(nameof(ApplicantMailingAddressId))]
		public virtual ApplicationAddress ApplicantMailingAddress { get; set; }

		/// <summary>
		/// get/set - Foreign key to the applicant organization.  This is used for historical reasons, it is copied from the applicant.
		/// </summary>
		public int OrganizationId { get; set; }

		/// <summary>
		/// get/set - The applicant organization.  This is used for historical reasons, it is copied from the applicant.
		/// </summary>
		[ForeignKey(nameof(OrganizationId))]
		public virtual Organization Organization { get; set; }

		/// <summary>
		/// get/set - The applicant organization BCeID.  This is used for historical reasons, it is copied from the applicant.
		/// </summary>
		public Guid? OrganizationBCeID { get; set; }

		/// <summary>
		/// get/set - Foreign key to the applicant organization address.  This is used for historical reasons, it is copied from the applicant.
		/// </summary>
		public int? OrganizationAddressId { get; set; }

		/// <summary>
		/// get/set - The applicant organization address.  This is used for historical reasons, it is copied from the applicant.
		/// </summary>
		[ForeignKey(nameof(OrganizationAddressId))]
		public virtual ApplicationAddress OrganizationAddress { get; set; }

		/// <summary>
		/// get/set - Foreign key to the applicant organization type.  This is used for historical reasons, it is copied from the applicant.
		/// </summary>
		public int? OrganizationTypeId { get; set; }

		/// <summary>
		/// get/set - The applicant organization type.  This is used for historical reasons, it is copied from the applicant.
		/// </summary>
		[ForeignKey(nameof(OrganizationTypeId))]
		public virtual OrganizationType OrganizationType { get; set; }

		/// <summary>
		/// get/set - Foreign key to the applicant organization legal structure.  This is used for historical reasons, it is copied from the applicant.
		/// </summary>
		public int? OrganizationLegalStructureId { get; set; }

		/// <summary>
		/// get/set - The applicant organization legal structure.  This is used for historical reasons, it is copied from the applicant.
		/// </summary>
		[ForeignKey(nameof(OrganizationLegalStructureId))]
		public virtual LegalStructure OrganizationLegalStructure { get; set; }

		/// <summary>
		/// get/set - The year the applicant organization was established.
		/// </summary>
		public int? OrganizationYearEstablished { get; set; }

		/// <summary>
		/// get/set - The number of employees worldwide in the applicant organization.
		/// </summary>
		public int? OrganizationNumberOfEmployeesWorldwide { get; set; }

		/// <summary>
		/// get/set - The number of employees in BC in the applicant organization.
		/// </summary>
		public int? OrganizationNumberOfEmployeesInBC { get; set; }

		/// <summary>
		/// get/set - The annual budget of the applicant organization.
		/// </summary>
		public decimal? OrganizationAnnualTrainingBudget { get; set; }

		/// <summary>
		/// get/set - The number of employees annually trained in the applicant organization.
		/// </summary>
		public int? OrganizationAnnualEmployeesTrained { get; set; }

		/// <summary>
		/// get/set - The applicant organization legal name.  This is used for historical reasons, it is copied from the applicant.
		/// </summary>
		[MaxLength(250)]
		[Index("IX_GrantApplication", 5)]
		public string OrganizationLegalName { get; set; }

		/// <summary>
		/// get/set - The business license number.
		/// </summary>
		[MaxLength(20)]
		public string OrganizationBusinessLicenseNumber { get; set; }

		/// <summary>
		/// get/set - The applicant organization is doing business as.  This is used for historical reasons, it is copied from the applicant.
		/// </summary>
		[MaxLength(500)]
		public string OrganizationDoingBusinessAs { get; set; }

		/// <summary>
		/// get/set - The foreign key to the risk classification for this grant application.
		/// </summary>
		public int? RiskClassificationId { get; set; }

		/// <summary>
		/// get/set - The risk classifiction for this grant application.
		/// </summary>
		[ForeignKey(nameof(RiskClassificationId))]
		public virtual RiskClassification RiskClassification { get; set; }

		/// <summary>
		/// get/set - When the application was submitted.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Column(TypeName = "DATETIME2")]
		public DateTime? DateSubmitted { get; set; }

		/// <summary>
		/// get/set - Assesor notes.
		/// </summary>
		[MaxLength(500)]
		public string AssessorNote { get; set; }

		/// <summary>
		/// get/set - The maximum reimbursement amount allowed per participant, per fiscal year as per the agreement.  This is used for historical reasons, it is copied from the grant stream.
		/// </summary>
		[Required, Min(0, ErrorMessage = "The maximum reimbursement amount must be greater than or equal to 0.")]
		public decimal MaxReimbursementAmt { get; set; }

		/// <summary>
		/// get/set - The reimbursement rate used to calculate the reimbursement.  This is used for historical reasons, it is copied from the grant stream.
		/// </summary>
		[Required, Min(0, ErrorMessage = "The reimbursement rate must be greater than or equal to 0."), Max(1, ErrorMessage = "The reimbursement rate must be less than or equal to 1.")]
		public double ReimbursementRate { get; set; }

		/// <summary>
		/// get/set - The foreign key to the priority sector.
		/// </summary>
		public int? PrioritySectorId { get; set; }

		/// <summary>
		/// get/set - The foreign key to the national industry classification system.
		/// </summary>
		public int? NAICSId { get; set; }

		/// <summary>
		/// get/set - The national industry classification system.
		/// </summary>
		[ForeignKey(nameof(NAICSId))]
		public virtual NaIndustryClassificationSystem NAICS { get; set; }

		/// <summary>
		/// get/set - The priority sector.
		/// </summary>
		[ForeignKey(nameof(PrioritySectorId))]
		public virtual PrioritySector PrioritySector { get; set; }

		/// <summary>
		/// get/set - Whether eligibility for the grant stream was confirmed.
		/// </summary>
		public bool EligibilityConfirmed { get; set; }

		/// <summary>
		/// get/set - Whether participant reporting is enabled after a claim has been submitted
		/// </summary>
		public bool CanReportParticipants { get; set; }

		/// <summary>
		/// get/set - Whether the applicant can current report participants.
		/// </summary>
		[Required, DefaultValue(false)]
		public bool CanApplicantReportParticipants { get; set; }

		/// <summary>
		/// get/set - The date the application was cancelled.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Column(TypeName = "DATETIME2")]
		public DateTime? DateCancelled { get; set; }

		/// <summary>
		/// get/set - The start date of the training program.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Index("IX_GrantApplication", 7)]
		[Column(TypeName = "DATETIME2")]
		public DateTime StartDate { get; set; } = AppDateTime.UtcNow;

		/// <summary>
		/// get/set - The end date of the training program.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Index("IX_GrantApplication", 8)]
		[Column(TypeName = "DATETIME2")]
		public DateTime EndDate { get; set; }

		/// <summary>
		/// get/set - The invitation key used to invite participants.
		/// </summary>
		[Index("IX_GrantApplication", 9)]
		public Guid InvitationKey { get; set; }

		/// <summary>
		/// get/set - When the invitation key will expire.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Index("IX_GrantApplication", 10)]
		[Column(TypeName = "DATETIME2")]
		public DateTime? InvitationExpiresOn { get; set; }

		/// <summary>
		/// get/set - Used to prevent payments when multiple training programs are present.
		/// </summary>
		public bool HoldPaymentRequests { get; set; } = false;

		/// <summary>
		/// get/set - The grant agreement associated with this grant application.  This is a one-to-one relationship.
		/// </summary>
		public virtual GrantAgreement GrantAgreement { get; set; }

		/// <summary>
		/// get/set - The training costs associated with this grant application.  This is a one-to-one relationship.
		/// </summary>
		public virtual TrainingCost TrainingCost { get; set; }

		/// <summary>
		/// get - The description of this training program.
		/// </summary>
		public virtual ProgramDescription ProgramDescription { get; set; }

		/// <summary>
		/// get - All of the notes associated with this application.
		/// </summary>
		public virtual ICollection<Note> Notes { get; set; } = new List<Note>();

		/// <summary>
		/// get - All of the training providers associated with this application (this relationship is problematic as it should be with the training program only).
		/// </summary>
		public virtual ICollection<TrainingProvider> TrainingProviders { get; set; } = new List<TrainingProvider>();

		/// <summary>
		/// get - All of the training programs associated with the application.
		/// </summary>
		public virtual ICollection<TrainingProgram> TrainingPrograms { get; set; } = new List<TrainingProgram>();

		/// <summary>
		/// get - All of the business contact roles associated with this application.  This identifies the Application Administrator and the Employer Administrator.
		/// </summary>
		public virtual ICollection<BusinessContactRole> BusinessContactRoles { get; set; } = new List<BusinessContactRole>();

		/// <summary>
		/// get - All of the state changes associated with this application.
		/// </summary>
		public virtual ICollection<GrantApplicationStateChange> StateChanges { get; set; } = new List<GrantApplicationStateChange>();

		/// <summary>
		/// get - All of the attachments associated with this application.
		/// </summary>
		public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

		/// <summary>
		/// get - All of the participant forms submitted for the grant application.
		/// </summary>
		public virtual ICollection<ParticipantForm> ParticipantForms { get; set; } = new List<ParticipantForm>();

		/// <summary>
		/// get - All of the claims associated with this grant application.
		/// </summary>
		public virtual ICollection<Claim> Claims { get; set; } = new List<Claim>();

		/// <summary>
		/// get - All of the payment requests associated with this grant application.
		/// </summary>
		public virtual ICollection<PaymentRequest> PaymentRequests { get; set; } = new List<PaymentRequest>();

		/// <summary>
		/// get - All of the participant completion report answers.
		/// </summary>
		public virtual ICollection<ParticipantCompletionReportAnswer> ParticipantCompletionReportAnswers { get; set; } = new List<ParticipantCompletionReportAnswer>();

		/// <summary>
		/// get - All of the employer completion report answers.
		/// </summary>
		public virtual ICollection<EmployerCompletionReportAnswer> EmployerCompletionReportAnswers { get; set; } = new List<EmployerCompletionReportAnswer>();

		/// <summary>
		/// get/set - All of the notifications for this grant application.
		/// </summary>
		public virtual ICollection<NotificationQueue> NotificationQueue { get; set; } = new List<NotificationQueue>();

		/// <summary>
		/// get - All of the eligibility answers.
		/// </summary>
		public virtual ICollection<GrantStreamEligibilityAnswer> GrantStreamEligibilityAnswers { get; set; } = new List<GrantStreamEligibilityAnswer>();

		/// <summary>
		/// get/set - Whether they used a delivery partner when creating the application for this training program.
		/// </summary>
		public bool? UsedDeliveryPartner { get; set; }

		/// <summary>
		/// get/set - The foreign key to the delivery partner.
		/// </summary>
		[Index("IX_GrantApplication", Order = 12)]
		public int? DeliveryPartnerId { get; set; }

		/// <summary>
		/// get/set - The delivery partner.
		/// </summary>
		[ForeignKey(nameof(DeliveryPartnerId))]
		public virtual DeliveryPartner DeliveryPartner { get; set; }

		/// <summary>
		/// get/set - All the delivery partner services provided.
		/// </summary>
		public virtual ICollection<DeliveryPartnerService> DeliveryPartnerServices { get; set; } = new List<DeliveryPartnerService>();

		/// <summary>
		/// get/set - Foreign key to the application completion report.
		/// </summary>
		public int CompletionReportId { get; set; }

		/// <summary>
		/// get/set - The application completion report.
		/// </summary>
		[ForeignKey(nameof(CompletionReportId))]
		public virtual CompletionReport CompletionReport { get; set; }

		/// <summary>
		/// get/set - Whether the applicant has scheduled notifications enabled
		/// </summary>
		[Required, DefaultValue(true)]
		[Index("IX_GrantApplication", Order = 13)]
		public bool ScheduledNotificationsEnabled { get; set; } = true;

		/// <summary>
		/// get/set - As the applicant, does your organization have the appropriate liability insurance to cover the skills-training project on your premises and/or other locations as required?
		/// Replaced by eligibility Answers. Existing applications will hold this data, new applications will set to NULL.
		/// Was used by CWRG only; other applications set this to NULL
		/// </summary>
		[Obsolete]
		public bool? InsuranceConfirmed { get; set; }

		// This is a CWRG-only question - not used in ETG
		public bool? HasRequestedAdditionalFunding { get; set; }

		public string DescriptionOfFundingRequested { get; set; }

		/// <summary>
		/// get/set - Check this box if someone other than the applicant or project coordinator has completed this application.
		/// </summary>
		[DefaultValue(false)]
		public bool? IsAlternateContact { get; set; } = false;

		/// <summary>
		/// get/set - The alternate applicant salutation.
		/// </summary>
		[MaxLength(250)]
		public string AlternateSalutation { get; set; }

		/// <summary>
		/// get/set - The alternate applicant first name.
		/// </summary>
		[MaxLength(250)]
		public string AlternateFirstName { get; set; }

		/// <summary>
		/// get/set - The alternate applicant last name.
		/// </summary>
		[MaxLength(250)]
		public string AlternateLastName { get; set; }

		/// <summary>
		/// get/set - The alternate applicant phone number.
		/// </summary>
		[MaxLength(20)]
		public string AlternatePhoneNumber { get; set; }

		/// <summary>
		/// get/set - The alternate applicant phone extension.
		/// </summary>
		[MaxLength(20)]
		[RegularExpression(@"[0-9]*")]
		public string AlternatePhoneExtension { get; set; }

		/// <summary>
		/// get/set - The alternate applicant job title.
		/// </summary>
		[MaxLength(500)]
		public string AlternateJobTitle { get; set; }

		/// <summary>
		/// get/set - The participant's email address.
		/// </summary>
		[MaxLength(500)]
		public string AlternateEmail { get; set; }

		/// <summary>
		/// get/set - The business case for ETG apps.
		/// </summary>
		public string BusinessCase { get; set; }

		/// <summary>
		/// get/set - The foreign key to the business case document for ETG apps.
		/// </summary>
		public int? BusinessCaseDocumentId { get; set; }

		/// <summary>
		/// get/set - The business case document for ETG apps.
		/// </summary>
		[ForeignKey(nameof(BusinessCaseDocumentId))]
		public virtual Attachment BusinessCaseDocument { get; set; }

		/// <summary>
		/// get/set - A collection of denial reasons associated with this grant file.
		/// </summary>
		public virtual ICollection<DenialReason> GrantApplicationDenialReasons { get; set; } = new List<DenialReason>();

		/// <summary>
		/// A summary field of the Priority Queue score. A breakdown of this score is kept in Pr
		/// </summary>
		public int PrioritizationScore { get; set; }

		public virtual PrioritizationScoreBreakdown PrioritizationScoreBreakdown { get; set; }

		/// <summary>
		/// get/set - Does the GrantApplication require all of the participants to be entered before allowing submission
		/// this property is inherited from the parent (GrantStream) so as to capture the state of the property when the app was created
		/// </summary>
		[Required]
		[DefaultValue(false)]
		public bool RequireAllParticipantsBeforeSubmission { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a GrantApplication object.
		/// </summary>
		public GrantApplication()
		{
		}

		/// <summary>
		/// Creates a new instance of a GrantApplication object and initializes it with the specified property values.
		/// Makes the specified user the Application Administrator.
		/// </summary>
		/// <param name="grantOpening"></param>
		/// <param name="applicationAdministrator"></param>
		/// <param name="applicationType"></param>
		public GrantApplication(GrantOpening grantOpening, User applicationAdministrator, ApplicationType applicationType)
		{
			if (applicationAdministrator == null)
				throw new ArgumentNullException(nameof(applicationAdministrator));

			ApplicationType = applicationType ?? throw new ArgumentNullException(nameof(applicationType)); ;
			ApplicationTypeId = applicationType.Id;
			GrantOpening = grantOpening ?? throw new ArgumentNullException(nameof(grantOpening)); ;
			GrantOpeningId = grantOpening.Id;
			MaxReimbursementAmt = grantOpening.GrantStream.MaxReimbursementAmt;
			ReimbursementRate = grantOpening.GrantStream.ReimbursementRate;
			ApplicationStateExternal = ApplicationStateExternal.Incomplete;
			ApplicationStateInternal = ApplicationStateInternal.Draft;

			this.CopyApplicant(applicationAdministrator);
			this.CopyOrganization(applicationAdministrator.Organization);
			this.AddApplicationAdministrator(applicationAdministrator);
		}
		#endregion

		#region Methods
		/// <summary>
		/// Validate this GrantApplication object.
		/// </summary>
		/// <param name="validationContext"></param>
		/// <returns></returns>
		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var context = validationContext.GetDbContext();
			var entry = validationContext.GetDbEntityEntry();
			var logger = validationContext.GetLogger();

			// This is done to stop errors from being thrown when developers use EF entities in ViewModels.
			if (entry == null)
				yield break;

			// Must be associated with a GrantOpening.
			if (GrantOpeningId == 0)
				yield return new ValidationResult("The grant application must be associated with a grant opening.", new[] { nameof(GrantOpening) });

			if (GrantOpening?.GrantStream == null || GrantOpening.TrainingPeriod == null)
				GrantOpening = context.Set<GrantOpening>()
					.Include(x => x.GrantStream)
					.Include(x => x.GrantStream.GrantProgram)
					.Include(x => x.TrainingPeriod)
					.SingleOrDefault(x => x.Id == GrantOpeningId);

			// If there is a DeliveryPartner there must be DeliveryPartnerServices.
			if (DeliveryPartner != null && (DeliveryPartnerServices == null || !DeliveryPartnerServices.Any()))
				yield return new ValidationResult("You must select delivery partner services", new[] { nameof(DeliveryPartnerServices) });

			var trainingPrograms = context.Set<TrainingProgram>()
					.Include(x => x.DeliveryMethods)
					.Include(tp => tp.TrainingProviders)
					.Where(x => x.GrantApplicationId == Id).ToArray();
			var trainingProviders = context.Set<TrainingProvider>().Where(tp => tp.GrantApplicationId == Id).ToArray();
			var grantAgreement = GrantAgreement ?? context.Set<GrantAgreement>().SingleOrDefault(x => x.GrantApplicationId == Id);

			// Must fall within TrainingPeriod.StartDate and TrainingPeriod.EndDate.
			var trainingPeriodStartDate = GrantOpening.TrainingPeriod.StartDate;
			var trainingPeriodEndDate = GrantOpening.TrainingPeriod.EndDate;
			var hasValidStartDate = this.HasValidStartDate();

			// StartDate must be before or on EndDate.
			if (hasValidStartDate && StartDate.ToLocalTime().Date > EndDate.ToLocalTime().Date)
				yield return new ValidationResult($"The end date must occur on or after the start date '{StartDate.ToLocalMorning():yyyy-MM-dd}'.", new[] { nameof(EndDate) });

			// EndDate cannot be greater than one year after the StartDate.
			if (hasValidStartDate && EndDate > StartDate.AddYears(1).ToUtcMidnight())
				yield return new ValidationResult($"The end date must occur within one year of the start date '{StartDate.ToLocalMorning():yyyy-MM-dd}'.", new[] { nameof(EndDate) });

			if (entry.State == EntityState.Added)
			{
				// Must begin with an external state of Incomplete.
				if (ApplicationStateExternal != ApplicationStateExternal.Incomplete)
					yield return new ValidationResult("A new grant application must have an external state of 'Incomplete'.", new[] { nameof(ApplicationStateExternal) });

				// Must begin with an internal state of Draft
				if (ApplicationStateInternal != ApplicationStateInternal.Draft)
					yield return new ValidationResult("A new grant application must have an internal state of 'Draft'.", new[] { nameof(ApplicationStateInternal) });

				// Must be associated with a BusinessContactRole that identifies the Application Administrator.
				if (BusinessContactRoles == null || !BusinessContactRoles.Any())
					yield return new ValidationResult("Grant application requires an Application Administrator to be associated with it.", new[] { nameof(BusinessContactRoles) });

				// Can't add a Grant Application to a Grant Opening in the wrong state.
				if (GrantOpening.State.In(GrantOpeningStates.Closed, GrantOpeningStates.Scheduled, GrantOpeningStates.Unscheduled))
					yield return new ValidationResult("Grant applications cannot be added to a grant opening that isn't published, or open.", new[] { nameof(GrantOpening) });

				// Must have valid dates.
				if (!hasValidStartDate)
					yield return new ValidationResult($"Your program start date must fall in the period '{trainingPeriodStartDate.ToLocalMorning():yyyy-MM-dd}' to '{trainingPeriodEndDate.ToLocalMidnight():yyyy-MM-dd}' " +
						"for the grant you have selected and it may not be before your application submission date.", new[] { nameof(StartDate) });
			}
			else if (entry.State == EntityState.Modified)
			{
				var originalApplicationStateExternal = (ApplicationStateExternal)entry.OriginalValues[nameof(ApplicationStateExternal)];
				var originalApplicationStateInternal = (ApplicationStateInternal)entry.OriginalValues[nameof(ApplicationStateInternal)];

				// Must have valid dates.
				if (!hasValidStartDate
					&& ((originalApplicationStateExternal == ApplicationStateExternal.Complete && ApplicationStateExternal != ApplicationStateExternal.Incomplete) // Must allow for transition from Complete to Incomplete.
						|| originalApplicationStateExternal == ApplicationStateExternal.Incomplete // Must not allow Applicant to enter invalid dates.
						|| ApplicationStateExternal > ApplicationStateExternal.Complete)) // Must not allow Assessors to enter invalid dates.
					yield return new ValidationResult($"Your program start date must fall in the period '{trainingPeriodStartDate.ToLocalMorning():yyyy-MM-dd}' to '{trainingPeriodEndDate.ToLocalMidnight():yyyy-MM-dd}' for the grant you have selected and it may not be before your application submission date.", new[] { nameof(StartDate) });

				// When an application is withdrawn it becomes possible to lock in a past date, this makes sure all start dates are today or the future.
				if (originalApplicationStateInternal == ApplicationStateInternal.ApplicationWithdrawn
					&& ApplicationStateInternal == ApplicationStateInternal.New
					&& StartDate < AppDateTime.UtcMorning)
					yield return new ValidationResult("Your program start date must not start before today.", new[] { nameof(StartDate) });

				var originalStartDate = (DateTime)entry.OriginalValues[nameof(StartDate)];
				var originalEndDate = (DateTime)entry.OriginalValues[nameof(EndDate)];

				// The Program dates must fall within the Delivery dates.
				if (trainingPrograms.Any(tp => tp.StartDate < StartDate) && originalStartDate != StartDate)
					yield return new ValidationResult("Skills training dates do not fall within your delivery period and will need to be rescheduled.  Make sure all your skills training dates are accurate to your plan.", new[] { nameof(StartDate) });

				if (trainingPrograms.Any(tp => tp.EndDate > EndDate) && originalEndDate != EndDate)
					yield return new ValidationResult("Skills training dates do not fall within your delivery period and will need to be rescheduled.  Make sure all your skills training dates are accurate to your plan.", new[] { nameof(EndDate) });

				// Before state can be OfferIssued it must have a GrantAgreement.
				if (ApplicationStateInternal == ApplicationStateInternal.OfferIssued && grantAgreement == null)
					yield return new ValidationResult("Grant application requires a grant agreement before an offer can be issued.", new[] { nameof(GrantAgreement) });
				// Before state can be New is must have a FileNumber.
				else if (ApplicationStateInternal == ApplicationStateInternal.New && String.IsNullOrEmpty(FileNumber))
					yield return new ValidationResult("Grant application must have a file number to identify it.", new[] { nameof(FileNumber) });

				// Validate the external state transitions.
				var originalExternalState = (ApplicationStateExternal)entry.OriginalValues[nameof(ApplicationStateExternal)];
				if (!originalExternalState.AllowStateTransition(ApplicationStateExternal))
					yield return new ValidationResult($"Grant application external state cannot go from '{originalExternalState.GetDescription()}' to '{ApplicationStateExternal.GetDescription()}'.", new[] { nameof(ApplicationStateExternal) });

				// Validate the internal state transitions.
				var originalInternalState = (ApplicationStateInternal)entry.OriginalValues[nameof(ApplicationStateInternal)];
				if (!originalInternalState.AllowStateTransition(ApplicationStateInternal))
					yield return new ValidationResult($"Grant application internal state cannot go from '{originalInternalState.GetDescription()}' to '{ApplicationStateInternal.GetDescription()}'.", new[] { nameof(ApplicationStateInternal) });

				if (ApplicationStateInternal == ApplicationStateInternal.CancelledByAgreementHolder)
				{
					// When state is CancelledByAgreementHolder it must have a CancellationReason and DateCancelled.
					context.Set<GrantApplicationStateChange>().Where(sc => sc.ToState == ApplicationStateInternal.CancelledByAgreementHolder).ToArray();
					if (string.IsNullOrWhiteSpace(this.GetReason(ApplicationStateInternal.CancelledByAgreementHolder)) || !DateCancelled.HasValue)
						yield return new ValidationResult($"Cancellation reason and date are required for {ApplicationStateInternal.GetDescription()}");
				}
				else if (ApplicationStateInternal == ApplicationStateInternal.CancelledByMinistry)
				{
					// When state is CancelledByMinistry, it must have a CancellationReason and DateCancelled.
					context.Set<GrantApplicationStateChange>().Where(sc => sc.ToState == ApplicationStateInternal.CancelledByMinistry).ToArray();
					if (string.IsNullOrWhiteSpace(this.GetReason(ApplicationStateInternal.CancelledByMinistry)) || !DateCancelled.HasValue)
						yield return new ValidationResult($"Cancellation reason and date are required for {ApplicationStateInternal.GetDescription()}");
				}
				else if (ApplicationStateExternal == ApplicationStateExternal.Submitted)
				{
					if (originalExternalState != ApplicationStateExternal.Submitted)
					{
						if (Attachments.Count < 1)
							context.Set<GrantApplication>().Where(o => o.Id == Id).Include(o => o.Attachments).ToArray();

						// Cannot submit unless the application is complete in all aspects.
						if (!this.SkillsTrainingConfirmed())
							yield return new ValidationResult("Skills training components are not complete, the application cannot be submitted.", new[] { nameof(ApplicationStateExternal) });
						else if (!this.EmploymentServicesAndSupportsConfirmed())
							yield return new ValidationResult("Employment services and supports components are not complete, the application cannot be submitted.", new[] { nameof(ApplicationStateExternal) });
						else if (ApplicationStateInternal != ApplicationStateInternal.ReturnedToAssessment
						         && ApplicationStateInternal != ApplicationStateInternal.UnderAssessment
						         && ApplicationStateInternal != ApplicationStateInternal.New
								 && !this.IsSubmittable())
							yield return new ValidationResult("Grant application is not complete and cannot be submitted.", new[] { nameof(ApplicationStateExternal) });
					}

					// Although the user shouldn't get as far as being able to submit the application without an organization, throw a validation exception once the state has been changed in the state machine
					if (OrganizationId == 0)
						yield return new ValidationResult("Organization is required before you can submit your application");

					// A training program is required for applications.
					if (trainingPrograms.FirstOrDefault() == null)
						yield return new ValidationResult("This grant application is missing a training program.", new[] { nameof(TrainingPrograms) });

					if (StartDate.ToLocalTime().Date < DateSubmitted.Value.ToLocalTime().Date)
						yield return new ValidationResult($"Your training start date must fall in the period '{trainingPeriodStartDate.ToLocalMorning():yyyy-MM-dd}' to '{trainingPeriodEndDate.ToLocalMidnight():yyyy-MM-dd}' for the grant you have selected and it may not be before your application submission date.", new[] { nameof(StartDate) });
				}
				else if (ApplicationStateInternal == ApplicationStateInternal.AgreementAccepted)
				{
					var agreement = context.Set<GrantAgreement>()
							.SingleOrDefault(x => x.GrantApplicationId == Id) ?? GrantAgreement;

					if (!agreement.DateAccepted.HasValue)
					{
						yield return new ValidationResult("Date Accepted is required when the agreement has been accepted", new[] { nameof(ApplicationStateInternal) });
					}
				}

				if (ApplicationStateInternal.In(ApplicationStateInternal.RecommendedForDenial, ApplicationStateInternal.ApplicationDenied, ApplicationStateInternal.ChangeForDenial, ApplicationStateInternal.ChangeRequestDenied, ApplicationStateInternal.ClaimDenied) || ApplicationStateExternal == ApplicationStateExternal.ApplicationWithdrawn)
				{
					var applicationStateChange = this.GetReason(ApplicationStateInternal);

					if (string.IsNullOrEmpty(applicationStateChange))
					{
						// need to pull the reason for the state change, since it's not available in the context
						applicationStateChange = context.Set<GrantApplicationStateChange>()
							.Where(x => x.GrantApplicationId == Id && x.ToState == ApplicationStateInternal).OrderByDescending(s => s.DateAdded).Select(s => s.Reason).FirstOrDefault();
					}

					if (string.IsNullOrEmpty(applicationStateChange))
					{
						if (ApplicationStateExternal == ApplicationStateExternal.ApplicationWithdrawn)
							yield return new ValidationResult("Please provide a reason for withdrawing your application", new[] { nameof(ApplicationStateInternal) });
						else
							yield return new ValidationResult("Please provide a reason for denial");
					}
				}

				// Assessor must provide a reason for returning the application.
				if (originalApplicationStateInternal != ApplicationStateInternal.ClaimReturnedToApplicant && ApplicationStateInternal == ApplicationStateInternal.ClaimReturnedToApplicant)
				{
					//get the latest state change
					var lastStateChange = this.GetStateChange(ApplicationStateInternal.ClaimReturnedToApplicant);

					//if the previous state was Cancelled by Ministry then a reason is not required, the user is "Reversing a Cancelled by Ministry"
					if (lastStateChange.FromState != ApplicationStateInternal.CancelledByMinistry)
					{
						var reason = lastStateChange.Reason ?? this.GetCurrentClaim()?.ClaimAssessmentNotes;

						if (String.IsNullOrWhiteSpace(reason))
							yield return new ValidationResult("Please provide a reason for returning the claim to the applicant.", new[] { nameof(ApplicationStateInternal) });
					}
				}

				// When changing the assessor we don't want to validation the following conditions.
				var isChangingAssessor = (int?)context.Entry(this).OriginalValues[nameof(AssessorId)] != AssessorId;
				if (!isChangingAssessor
					&& ApplicationStateInternal.In(ApplicationStateInternal.RecommendedForApproval, ApplicationStateInternal.OfferIssued, ApplicationStateInternal.AgreementAccepted)
					|| ApplicationStateInternal > ApplicationStateInternal.AgreementAccepted
					&& !ApplicationStateInternal.In(ApplicationStateInternal.ApplicationWithdrawn, ApplicationStateInternal.Unfunded, ApplicationStateInternal.ApplicationDenied))
				{
					// Must validate training providers if internal state change except ReturnUnassessed
					if (ApplicationStateInternal != ApplicationStateInternal.ReturnedUnassessed && originalApplicationStateInternal != ApplicationStateInternal && trainingPrograms.Any(tp => tp.TrainingProvider?.TrainingProviderInventoryId == null))
						yield return new ValidationResult($"All training providers must be validated before you can '{ApplicationStateInternal.GetDescription()}'.");

					var ids = TrainingCost?.EligibleCosts.Select(ec => ec.Id).ToArray() ?? context.Set<EligibleCost>().Where(ec => ec.GrantApplicationId == Id).Select(ec => ec.Id).ToArray();
					var eligibleCosts = context.Set<EligibleCost>().Include(m => m.EligibleExpenseType).Include(m => m.EligibleExpenseType.ServiceCategory).Include(m => m.Breakdowns).Include(m => m.TrainingProviders).Where(ec => ids.Any(y => y == ec.Id)).ToArray();
					// Must include costs if services are selected, and vise-versa.
					if (eligibleCosts.Any(ec =>
						ec.EligibleExpenseType.ServiceCategory?.ServiceTypeId == ServiceTypes.EmploymentServicesAndSupports
						&& (((ec.Breakdowns.Count == 0 && ec.AgreedMaxCost != 0) // There are no services selected, but there is a cost.
								|| (ec.Breakdowns.Count != 0 && ec.AgreedMaxCost == 0)) // There are services selected, but there is no cost.
							|| ((ec.EligibleExpenseType.MaxProviders > 0 && ec.TrainingProviders.Count == 0 && ec.AgreedMaxCost != 0) // There are no providers selected, but there is a cost.
								|| (ec.EligibleExpenseType.MaxProviders > 0 && ec.TrainingProviders.Count != 0 && ec.AgreedMaxCost == 0))) // There are providers selected, but there is no cost.
						)) // At least one eligible skills training must have a cost.
							yield return new ValidationResult("Employment services and supports components are not complete.", new[] { "Summary" });

					// Must include costs associated with skills training.
					if (eligibleCosts.Any(ec =>
						ec.EligibleExpenseType.ServiceCategory?.ServiceTypeId == ServiceTypes.SkillsTraining
							&& (ec.AgreedMaxCost == 0 || ec.Breakdowns.Any(ecb => ecb.IsEligible && ecb.AssessedCost == 0)))) // At least one eligible skills training must have a cost.
						yield return new ValidationResult("Skills training components are not complete.", new[] { "Summary" });
				}
			}

			// The applicant can only report participants after a claim has been submitted if the stream supports it.
			if (CanApplicantReportParticipants && !(GrantOpening?.GrantStream?.CanApplicantReportParticipants ?? false))
				yield return new ValidationResult("The grant stream does not allow reporting participants", new[] { nameof(CanApplicantReportParticipants) });

			foreach (var validation in base.Validate(validationContext))
			{
				yield return validation;
			}

			// If HasRequestedAdditionalFunding then must have DescriptionOfFundingRequested.
			if (HasRequestedAdditionalFunding == true && String.IsNullOrEmpty(DescriptionOfFundingRequested))
				yield return new ValidationResult("If you have received or requested additional funding you must include a description of the funding request.", new[] { nameof(DescriptionOfFundingRequested) });
		}
		#endregion
	}
}

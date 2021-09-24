using CJG.Core.Entities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data.Entity;

namespace CJG.Core.Entities
{
	/// <summary>
	/// GrantProgram provides a way to manage the properties of a grant program.  A grant program is a way to group grant streams and manage their implementation.
	/// </summary>
	public class GrantProgram : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - The primary key uses IDENTITY
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - The foreign key to the account codes.
		/// </summary>
		[Index("IX_GrantProgram", 1)]
		public int? AccountCodeId { get; set; }

		/// <summary>
		/// get/set - The account codes that will be used by this grant program.
		/// </summary>
		[ForeignKey(nameof(AccountCodeId))]
		public virtual AccountCode AccountCode { get; set; }

		/// <summary>
		/// get/set - A unique name to identify this grant program.
		/// </summary>
		[Required, MaxLength(100), Index("IX_GrantProgram", 2), Index("IX_GrantProgram_Name", IsUnique = true)]
		public string Name { get; set; }

		/// <summary>
		/// get/set - A unique code to identify this grant program.
		/// </summary>
		[Required, MaxLength(5), Index("IX_GrantProgram", 3), Index("IX_GrantProgram_Code", IsUnique = true)]
		public string ProgramCode { get; set; }

		/// <summary>
		/// get/set - A description that will be included in a batch payment request.
		/// </summary>
		[MaxLength(2000)]
		public string BatchRequestDescription { get; set; }

		/// <summary>
		/// get/set - A description that provides what is required for eligibility in this grant program.
		/// </summary>
		[MaxLength(2000)]
		public string EligibilityDescription { get; set; }

		/// <summary>
		/// get/set - A message that will be presented to applicants when selecting grant program.
		/// </summary>
		[MaxLength(1000)]
		public string Message { get; set; }

		/// <summary>
		/// get/set - Controls whether the message is visible to the applicants.
		/// </summary>
		[Required, DefaultValue(false)]
		public bool ShowMessage { get; set; }

		/// <summary>
		/// get/set - Controls whether this grant program supports allowing the applicant to report participants themselves.
		/// </summary>
		[Required, DefaultValue(false)]
		public bool CanApplicantReportParticipants { get; set; }

		/// <summary>
		/// get/set - Controls whether this grant program supports allowing the applicant to report sponsors.  NOT USED CURRENTLY.
		/// </summary>
		[Required, DefaultValue(false)]
		public bool CanReportSponsors { get; set; }

		/// <summary>
		/// get/set - Control whether this FIFO Fund Reservations.
		/// </summary>
		[Required, DefaultValue(true)]
		public bool UseFIFOReservation { get; set; }

		/// <summary>
		/// get/set - A way to control whether delivery partners are included in the application development process.
		/// </summary>
		[Required, DefaultValue(false)]
		public bool IncludeDeliveryPartner { get; set; }

		/// <summary>
		/// get/set - The current state of the grant program.
		/// </summary>
		[Required, DefaultValue(GrantProgramStates.NotImplemented), Index("IX_GrantProgram", 4)]
		public GrantProgramStates State { get; set; } = GrantProgramStates.NotImplemented;

		/// <summary>
		/// get/set - The foreign key to the applicant declation documente template.
		/// </summary>
		public int ApplicantDeclarationTemplateId { get; set; }

		/// <summary>
		/// get/set - The applicant declartion document template.
		/// </summary>
		[ForeignKey(nameof(ApplicantDeclarationTemplateId))]
		[InverseProperty("ApplicantDeclarationTemplates")]
		public virtual DocumentTemplate ApplicantDeclarationTemplate { get; set; }

		/// <summary>
		/// get/set - The foreign key to the applicant cover letter documente template.
		/// </summary>
		public int ApplicantCoverLetterTemplateId { get; set; }

		/// <summary>
		/// get/set - The applicant cover letter document template.
		/// </summary>
		[ForeignKey(nameof(ApplicantCoverLetterTemplateId))]
		[InverseProperty("ApplicantCoverLetterTemplates")]
		public virtual DocumentTemplate ApplicantCoverLetterTemplate { get; set; }

		/// <summary>
		/// get/set - The foreign key to the applicant schedule A documente template.
		/// </summary>
		public int ApplicantScheduleATemplateId { get; set; }

		/// <summary>
		/// get/set - The applicant schedule A document template.
		/// </summary>
		[ForeignKey(nameof(ApplicantScheduleATemplateId))]
		[InverseProperty("ApplicantScheduleATemplates")]
		public virtual DocumentTemplate ApplicantScheduleATemplate { get; set; }

		/// <summary>
		/// get/set - The foreign key to the applicant schedule B documente template.
		/// </summary>
		public int ApplicantScheduleBTemplateId { get; set; }

		/// <summary>
		/// get/set - The applicant schedule B document template.
		/// </summary>
		[ForeignKey(nameof(ApplicantScheduleBTemplateId))]
		[InverseProperty("ApplicantScheduleBTemplates")]
		public virtual DocumentTemplate ApplicantScheduleBTemplate { get; set; }

		/// <summary>
		/// get/set - The foreign key to the participant consent documente template.
		/// </summary>
		public int ParticipantConsentTemplateId { get; set; }

		/// <summary>
		/// get/set - The participant consent document template.
		/// </summary>
		[ForeignKey(nameof(ParticipantConsentTemplateId))]
		[InverseProperty("ParticipantConsentTemplates")]
		public virtual DocumentTemplate ParticipantConsentTemplate { get; set; }

		/// <summary>
		/// get/set - The foreign key to the internal user that is authorized to pay the grant.
		/// </summary>
		public int? ExpenseAuthorityId { get; set; }

		/// <summary>
		/// get/set - The internal user that is authorized to pay the grant.
		/// </summary>
		[ForeignKey(nameof(ExpenseAuthorityId))]
		public virtual InternalUser ExpenseAuthority { get; set; }

		/// <summary>
		/// get/set - The requested by information included in the payment request batch.
		/// </summary>
		[MaxLength(250)]
		public string RequestedBy { get; set; }

		/// <summary>
		/// get/set - The program phone included in the payment request batch.
		/// </summary>
		[MaxLength(50)]
		public string ProgramPhone { get; set; }

		/// <summary>
		/// get/set - A document prefix used in the payment request batch.
		/// </summary>
		[MaxLength(5)]
		public string DocumentPrefix { get; set; }

		/// <summary>
		/// get/set - The date this grant program was implemented.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Column(TypeName = "DATETIME2")]
		public DateTime? DateImplemented { get; set; }

		/// <summary>
		/// get/set - The foreign key that identifies which program configuration this grant program uses.
		/// </summary>
		public int ProgramConfigurationId { get; set; }

		/// <summary>
		/// get/set - The program configuration this grant program uses.
		/// </summary>
		[ForeignKey(nameof(ProgramConfigurationId))]
		public virtual ProgramConfiguration ProgramConfiguration { get; set; }

		/// <summary>
		/// get/set - The foreign key to the program type.
		/// </summary>
		public ProgramTypes ProgramTypeId { get; set; }

		/// <summary>
		/// get/set - The program type that controls the application process.
		/// </summary>
		[ForeignKey(nameof(ProgramTypeId))]
		public virtual ProgramType ProgramType { get; set; }

		/// <summary>
		/// get - The grant streams associated to this grant program.
		/// </summary>
		public virtual ICollection<GrantStream> GrantStreams { get; set; } = new List<GrantStream>();

		/// <summary>
		/// get - The notification types associated to this grant program.
		/// </summary>
		public virtual ICollection<GrantProgramNotificationType> GrantProgramNotificationTypes { get; set; } = new List<GrantProgramNotificationType>();

		public virtual ICollection<DenialReason> DenialReasons { get; set; } = new List<DenialReason>();

		/// <summary>
		/// get - The report rates used in the claim reporting dashboard.
		/// </summary>
		public virtual ICollection<ReportRate> ReportRates { get; set; } = new List<ReportRate>();

		/// <summary>
		/// get - All the user preferences related to this grant program.
		/// </summary>
		public virtual ICollection<UserGrantProgramPreference> UserGrantProgramPreferences { get; set; } = new List<UserGrantProgramPreference>();

		/// <summary>
		/// get - All the payment request batches associated with this grant program.
		/// </summary>
		public virtual ICollection<PaymentRequestBatch> PaymentRequestBatches { get; set; } = new List<PaymentRequestBatch>();

		/// <summary>
		/// get - All the delivery partners associated with this grant program.
		/// </summary>
		public virtual ICollection<DeliveryPartner> DeliveryPartners { get; set; } = new List<DeliveryPartner>();

		/// <summary>
		/// get - All the delivery partner services associated with this grant program.
		/// </summary>
		public virtual ICollection<DeliveryPartnerService> DeliveryPartnerServices { get; set; } = new List<DeliveryPartnerService>();
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="GrantProgram"/> object.
		/// </summary>
		public GrantProgram()
		{

		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="GrantProgram"/> object.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="programCode"></param>
		/// <param name="programType"></param>
		/// <param name="declarationTemplate"></param>
		/// <param name="coverLetterTemplate"></param>
		/// <param name="scheduleATemplate"></param>
		/// <param name="scheduleBTemplate"></param>
		/// <param name="participantConsentTemplate"></param>
		/// <param name="expenseAuthority"></param>
		/// <param name="code"></param>
		public GrantProgram(string name, string programCode, ProgramTypes programType, DocumentTemplate declarationTemplate, DocumentTemplate coverLetterTemplate, DocumentTemplate scheduleATemplate, DocumentTemplate scheduleBTemplate, DocumentTemplate participantConsentTemplate, InternalUser expenseAuthority = null, AccountCode code = null)
		{
			this.Name = name ?? throw new ArgumentNullException(nameof(name));
			this.ProgramCode = programCode ?? throw new ArgumentNullException(nameof(programCode));
			this.ProgramTypeId = programType;
			this.ApplicantDeclarationTemplate = declarationTemplate ?? throw new ArgumentNullException(nameof(declarationTemplate));
			this.ApplicantDeclarationTemplateId = declarationTemplate.Id;
			this.ApplicantCoverLetterTemplate = coverLetterTemplate ?? throw new ArgumentNullException(nameof(coverLetterTemplate));
			this.ApplicantCoverLetterTemplateId = coverLetterTemplate.Id;
			this.ApplicantScheduleATemplate = scheduleATemplate ?? throw new ArgumentNullException(nameof(scheduleATemplate));
			this.ApplicantScheduleATemplateId = scheduleATemplate.Id;
			this.ApplicantScheduleBTemplate = scheduleBTemplate ?? throw new ArgumentNullException(nameof(scheduleBTemplate));
			this.ApplicantScheduleBTemplateId = scheduleBTemplate.Id;
			this.ParticipantConsentTemplate = participantConsentTemplate ?? throw new ArgumentNullException(nameof(participantConsentTemplate));
			this.ParticipantConsentTemplateId = participantConsentTemplate.Id;
			this.ExpenseAuthority = expenseAuthority;
			this.ExpenseAuthorityId = expenseAuthority?.Id;
			this.AccountCode = code;
			this.AccountCodeId = code?.Id;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Validates the grant program properties before saving to the datasource.
		/// </summary>
		/// <param name="validationContext"></param>
		/// <returns></returns>
		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var context = validationContext.GetDbContext();
			var entry = validationContext.GetDbEntityEntry();

			// This is done to stop errors from being thrown when developers use EF entities in ViewModels.
			if (entry == null)
				yield break;

			if (entry.State == System.Data.Entity.EntityState.Modified)
			{
				var originalConfigurationId = entry.GetDatabaseValues().GetValue<int>(nameof(this.ProgramConfigurationId));

				// Do not allow changing the program configuration once the grant program has been implemented.
				if (originalConfigurationId != this.ProgramConfigurationId
					&& (this.State == GrantProgramStates.Implemented || this.GrantStreams.SelectMany(t => t.GrantOpenings).Any(t => t.GrantApplications.Any())))
					yield return new ValidationResult("The program configuration cannot be changed after the grant program has been implemented.", new[] { nameof(this.ProgramConfigurationId) });

				if (this.State == GrantProgramStates.Implemented)
				{
					var programConfiguration = context.Set<ProgramConfiguration>().Include(x => x.EligibleExpenseTypes.Select(o => o.Breakdowns)).FirstOrDefault(x => x.Id == this.ProgramConfigurationId);
					switch (programConfiguration.ClaimTypeId)
					{
						case ClaimTypes.SingleAmendableClaim:
							if (!programConfiguration.EligibleExpenseTypes.Any(x => x.IsActive))
							{
								yield return new ValidationResult("The program configuration must have at least one in-scope and active expense type when the grant program is implemented.", new[] { nameof(this.ProgramConfigurationId) });
							}
							break;

						case ClaimTypes.MultipleClaimsWithoutAmendments:
							if (!programConfiguration.EligibleExpenseTypes.Any(x => x.IsActive && x.Breakdowns.Any(o => o.IsActive)))
							{
								yield return new ValidationResult("The program configuration must have at least one in-scope and active service category and service line when the grant program is implemented.", new[] { nameof(this.ProgramConfigurationId) });
							}
							break;
					}
				}

				var grantProgramNotificationTypes = context.Set<GrantProgramNotificationType>().Where(x => x.GrantProgramId == this.Id);
				if (grantProgramNotificationTypes.GroupBy(x => x.NotificationTypeId).Any(g => g.Count() > 1))
					yield return new ValidationResult("Grant program can't contain more than one of each notification type.", new[] { nameof(this.GrantProgramNotificationTypes) });
			}

			if (this.IncludeDeliveryPartner)
			{
				if (this.DeliveryPartners.Count() == 0) context.Set<DeliveryPartner>().Where(dp => dp.GrantProgramId == this.Id).ToArray();
				var hasDeliveryPartners = this.DeliveryPartners.Any(dp => dp.IsActive && dp.GrantProgramId == this.Id);
				if (!hasDeliveryPartners)
					yield return new ValidationResult("There must be at least one active delivery partner.", new[] { nameof(this.DeliveryPartners) });

				if (this.DeliveryPartnerServices.Count() == 0) context.Set<DeliveryPartnerService>().Where(dp => dp.GrantProgramId == this.Id).ToArray();
				var hasDeliveryPartnerServices = this.DeliveryPartnerServices.Any(dp => dp.IsActive && dp.GrantProgramId == this.Id);
				if (!hasDeliveryPartnerServices)
					yield return new ValidationResult("There must be at least one active delivery partner service.", new[] { nameof(this.DeliveryPartnerServices) });
			}

			foreach (var validation in base.Validate(validationContext))
			{
				yield return validation;
			}
		}
		#endregion
	}
}

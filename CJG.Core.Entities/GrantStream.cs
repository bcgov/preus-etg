using CJG.Core.Entities.Attributes;
using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="GrantStream"/> class, provides ORM a way to manage stream grants.
	/// </summary>
	public class GrantStream : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - Primary key uses IDENTITY.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - Foreign key to the parent grant program.
		/// </summary>
		[Index("IX_GrantStream", 1)]
		public int GrantProgramId { get; set; }

		/// <summary>
		/// get/set - The parent grant program.
		/// </summary>
		[ForeignKey(nameof(GrantProgramId))]
		public virtual GrantProgram GrantProgram { get; set; }

		/// <summary>
		/// get/set - Foreign key to the account codes.  By default they will be the same as the parent grant program.
		/// </summary>
		[Index("IX_GrantStream", 2)]
		public int? AccountCodeId { get; set; }

		/// <summary>
		/// get/set - The account codes to use.
		/// </summary>
		[ForeignKey(nameof(AccountCodeId))]
		public virtual AccountCode AccountCode { get; set; }

		/// <summary>
		/// get/set - The unique name of the grant stream.
		/// </summary>
		[Required, MaxLength(250)]
		public string Name { get; set; }

		/// <summary>
		/// get - The full name including the grant program name.
		/// </summary>
		[NotMapped]
		public string FullName { get { return $"{this.GrantProgram.Name} \u2013 {this.Name}"; } }

		/// <summary>
		/// get - The full name using the grant program code.
		/// </summary>
		[NotMapped]
		public string FullNameProgramCode { get { return $"{this.GrantProgram.ProgramCode} \u2013 {this.Name}"; } }

		/// <summary>
		/// get/set - The grant stream objective.
		/// </summary>
		[MaxLength(2500)]
		public string Objective { get; set; }

		/// <summary>
		/// get/set - Whether attachments are enabled.
		/// </summary>
		public bool AttachmentsIsEnabled { get; set; }

		/// <summary>
		/// get/set - The attachment section header.
		/// </summary>
		[MaxLength(200)]
		public string AttachmentsHeader { get; set; }

		/// <summary>
		/// get/set - User guidance information for the attachment section.
		/// </summary>
		[MaxLength(2500)]
		public string AttachmentsUserGuidance { get; set; }

		/// <summary>
		/// get/set - The maximum number of attachments allowed.
		/// </summary>
		[Required, DefaultValue(10), Min(0)]
		public int AttachmentsMaximum { get; set; }

		/// <summary>
		/// get/set - Whether an attachment is required to be added to the grant application.
		/// </summary>
		public Boolean AttachmentsRequired { get; set; } = false;

		/// <summary>
		/// get/set - A description of the eligibility requirements.
		/// </summary>
		[MaxLength(2000)]
		public string EligibilityRequirements { get; set; }

		/// <summary>
		/// get/set - Whether the eligibility section is enabled.
		/// </summary>
		public bool EligibilityEnabled { get; set; }

		/// <summary>
		/// get/set - The eligibility question.
		/// </summary>
		[MaxLength(2000)]
		public string EligibilityQuestion { get; set; }

		/// <summary>
		/// get/set - Whether they must agree to the eligibility for this grant stream.
		/// </summary>
		public bool EligibilityRequired { get; set; }

		/// <summary>
		/// get/set - The maximum reimbursement amount per participant per fiscal year within this grant stream.
		/// </summary>
		[Min(0, ErrorMessage = "The maximum reimbursement amount must be greater than or equal to 0.")]
		[Max(999999.99, ErrorMessage = "The maximum reimbursement amount must be less than or equal to 999999.99.")]
		public decimal MaxReimbursementAmt { get; set; }

		/// <summary>
		/// get/set - The reimbursement rate to use
		/// </summary>
		[Min(0.05, ErrorMessage = "The reimbursement rate must be greater than or equal to 0.05."), Max(1, ErrorMessage = "The reimbursement rate must be less than or equal to 1.")]
		public double ReimbursementRate { get; set; }

		/// <summary>
		/// get/set - The default denied rate for reporting.
		/// </summary>
		[Min(0, ErrorMessage = "The denied rate must be greater than or equal to 0."), Max(1, ErrorMessage = "The denied rate must be less than or equal to 1.")]
		public double DefaultDeniedRate { get; set; }

		/// <summary>
		/// get/set - The default withdrawn rate for reporting.
		/// </summary>
		[Min(0, ErrorMessage = "The withdrawn rate must be greater than or equal to 0."), Max(1, ErrorMessage = "The withdrawn rate must be less than or equal to 1.")]
		public double DefaultWithdrawnRate { get; set; }

		/// <summary>
		/// get/set - The default reduction reate for reporting.
		/// </summary>
		[Min(0, ErrorMessage = "The reduction rate must be greater than or equal to 0."), Max(1, ErrorMessage = "The reduction rate must be less than or equal to 1.")]
		public double DefaultReductionRate { get; set; }

		/// <summary>
		/// get/set - The default slippage rate for reporting.
		/// </summary>
		[Min(0, ErrorMessage = "The slippage rate must be greater than or equal to 0."), Max(1, ErrorMessage = "The slippage rate must be less than or equal to 1.")]
		public double DefaultSlippageRate { get; set; }

		/// <summary>
		/// get/set - The default cancellation rate for reporting.
		/// </summary>
		[Min(0, ErrorMessage = "The cancellation rate must be greater than or equal to 0."), Max(1, ErrorMessage = "The cancellation rate must be less than or equal to 1.")]
		public double DefaultCancellationRate { get; set; }

		/// <summary>
		/// get/set - Whether this grant stream is available for grant openings.
		/// </summary
		[Required]
		[Index("IX_GrantStream", 3), DefaultValue(true)]
		public bool IsActive { get; set; } = true;

		/// <summary>
		/// get/set - Whether to include the delivery partner step in application submission.
		/// </summary>
		[Required]
		[DefaultValue(true)]
		public bool IncludeDeliveryPartner { get; set; } = true;
		/// <summary>
		/// get/set - Whether an applicant can report on participants.
		/// </summary>
		[Required]
		[DefaultValue(false)]
		public bool CanApplicantReportParticipants { get; set; } = false;

		/// <summary>
		/// get/set - The date this grant stream was first made available for grant openings.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Column(TypeName = "DATETIME2")]
		public DateTime? DateFirstUsed { get; set; }

		/// <summary>
		/// get/set - The foreign key to the program configuration.  By default it should be inherited from the parent grant program.
		/// </summary>
		public int ProgramConfigurationId { get; set; }

		/// <summary>
		/// get/set - The program configuration for this grant stream.  By default it should be inherited from the parent grant program.
		/// </summary>
		[ForeignKey(nameof(ProgramConfigurationId))]
		public virtual ProgramConfiguration ProgramConfiguration { get; set; }

		/// <summary>
		/// get - All grant openings belonging to this grant stream.
		/// </summary>
		public virtual ICollection<GrantOpening> GrantOpenings { get; set; } = new List<GrantOpening>();

		/// <summary>
		/// get/set - Provides a way to manage and configure grant stream settings
		/// </summary>
		[Required]
		[DefaultValue(false)]
		public bool HasParticipantOutcomeReporting { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="GrantStream"/> object.
		/// </summary>
		public GrantStream()
		{

		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="GrantStream"/> object.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="objective"></param>
		/// <param name="program"></param>
		public GrantStream(string name, string objective, GrantProgram program) : this(name, objective, program, program.AccountCode)
		{
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="GrantStream"/> object.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="objective"></param>
		/// <param name="program"></param>
		/// <param name="payment"></param>
		public GrantStream(string name, string objective, GrantProgram program, AccountCode code)
		{
			this.Name = name ?? throw new ArgumentNullException(nameof(name));
			this.Objective = objective ?? throw new ArgumentNullException(nameof(objective));
			this.GrantProgram = program ?? throw new ArgumentNullException(nameof(program));
			this.GrantProgramId = program.Id;
			this.ProgramConfiguration = program.ProgramConfiguration;
			this.ProgramConfigurationId = program.ProgramConfigurationId;
			this.AccountCode = code ?? throw new ArgumentNullException(nameof(code));
			this.AccountCodeId = code?.Id;
			this.IsActive = true;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Validate this grant stream before making changes to the datasource.
		/// </summary>
		/// <param name="validationContext"></param>
		/// <returns></returns>
		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var context = validationContext.GetDbContext();
			var entry = validationContext.GetDbEntityEntry();

			// This is done to stop errors from being thrown when developers use EF entities in ViewModels.
			if (entry == null || context == null)
				yield break;

			if (this.GrantProgramId > 0 && this.GrantProgram == null)
				this.GrantProgram = context.Set<GrantProgram>().Find(this.GrantProgramId);

			// Must be linked to an existing grant program.
			if (this.GrantProgramId == 0)
				yield return new ValidationResult("The grant stream must be associated to an existing grant program.", new[] { nameof(this.GrantProgramId) });

			// Must be linked to account codes.
			if ((this.AccountCodeId == 0 || this.AccountCodeId == null) && this.AccountCode == null)
				yield return new ValidationResult("The account codes must be configured.", new[] { nameof(this.AccountCodeId) });

			// Must be linked to program configuration.
			if (this.ProgramConfigurationId == 0 && this.ProgramConfiguration == null)
				yield return new ValidationResult("The program configuration must be configured.", new[] { nameof(this.ProgramConfigurationId) });

			// Manually enforce a unique program name when creating new streams.
			if (entry.State == System.Data.Entity.EntityState.Added && context.Set<GrantStream>().Any(gs => gs.GrantProgramId == this.GrantProgramId && gs.Name == this.Name))
				yield return new ValidationResult($"The grant stream '{this.Name}' already exists and must remain unique within a grant program.", new[] { nameof(this.Name) });

			// If active it must have an objective.
			if (this.IsActive && string.IsNullOrEmpty(this.Objective))
				yield return new ValidationResult("The grant stream must have an objective defined.", new[] { nameof(this.Objective) });

			// If Eligibility is enabled it must have eligibility requirements.
			if (this.EligibilityEnabled && string.IsNullOrEmpty(this.EligibilityRequirements))
				yield return new ValidationResult("The grant stream must have an Eligibility Requirement defined.", new[] { nameof(this.EligibilityRequirements) });

			// If active it must be assocated with a ImplementedGrantProgram.
			if (this.IsActive && (this.Name == null ||
								  this.Objective == null ||
								  this.MaxReimbursementAmt == 0 ||
								  this.ReimbursementRate == 0 || (
									  (this.AccountCodeId == null || this.AccountCodeId == 0) && (
									  this.AccountCode?.GLClientNumber == null ||
									  this.AccountCode?.GLRESP == null ||
									  this.AccountCode?.GLServiceLine == null ||
									  this.AccountCode?.GLSTOBNormal == null ||
									  this.AccountCode?.GLSTOBAccrual == null ||
									  this.AccountCode?.GLProjectCode == null)
								  ) ||
								  this.GrantProgram.State == GrantProgramStates.NotImplemented))
				yield return new ValidationResult("A grant stream cannot be made active until it has an objective, maximum reimbursement amount, reimbursement rate and the Grant Program with which it is associated is implemented.", new[] { nameof(this.IsActive) });

			// Cannot enable attachments without a header.
			if (this.AttachmentsIsEnabled && string.IsNullOrEmpty(this.AttachmentsHeader))
				yield return new ValidationResult("The grant stream must have an attachements header defined.", new[] { nameof(this.AttachmentsHeader) });

			// Cannot enable attachments without user guidance.
			if (this.AttachmentsIsEnabled &&  string.IsNullOrEmpty(this.AttachmentsUserGuidance))
				yield return new ValidationResult("The grant stream must have an attachements user guidance defined.", new[] { nameof(this.AttachmentsUserGuidance) });

			// When eligibility is enabled it must have an eligiblity question.
			if (this.EligibilityEnabled && string.IsNullOrEmpty(this.EligibilityQuestion))
				yield return new ValidationResult("The grant stream must have an eligibility question defined.", new[] { nameof(this.EligibilityQuestion) });

			// Maximum number of attachments allowed.
			if (this.AttachmentsMaximum > 50)
				yield return new ValidationResult("Maximum Number of Attachment Permitted is 50.", new[] { nameof(this.AttachmentsMaximum) });

			// If the attachments are enabled they must be allowed to add at least 1.
			if (this.AttachmentsMaximum == 0 && this.AttachmentsIsEnabled)
				yield return new ValidationResult("Attachment is enabled, attachment maximum can not be 0.", new[] { nameof(this.AttachmentsMaximum) });

			foreach (var validation in base.Validate(validationContext))
			{
				yield return validation;
			}
		}
		#endregion
	}
}

using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data.Entity;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="ParticipantCost"/> class, provides the ORM a way to manage participant costs in claims.  Every claim eligible cost is broken down by participant.
	/// </summary>
	public class ParticipantCost : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - The primary key for this participant cost.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - The foreign key to the claim eligible cost this participant cost belongs to.
		/// </summary>
		public int ClaimEligibleCostId { get; set; }

		/// <summary>
		/// get/set - The claim eligible cost this participant cost belongs to.
		/// </summary>
		[ForeignKey(nameof(ClaimEligibleCostId))]
		public virtual ClaimEligibleCost ClaimEligibleCost { get; set; }

		/// <summary>
		/// get/set - The foreign key to the participant enrollment that is associated to this participant cost.  All participant costs must belong to a participant.
		/// </summary>
		public int ParticipantFormId { get; set; }

		/// <summary>
		/// get/set - The participant enrollment this participant cost belongs to.
		/// </summary>
		[ForeignKey(nameof(ParticipantFormId))]
		public virtual ParticipantForm ParticipantForm { get; set; }

		/// <summary>
		/// get/set - The actual cost for this participant cost.
		/// </summary>
		[Required, Min(0, ErrorMessage = "The participant cost must be greater than or equal to 0.")]
		public decimal ClaimParticipantCost { get; set; }

		/// <summary>
		/// get/set - The requested reimbursement for this participant cost.  This is calculated (ClaimParticipantCost * GrantApplication.ReimbursementRate).
		/// </summary>
		[Required, Min(0, ErrorMessage = "The reimbursement must be greater than or equal to 0.")]
		public decimal ClaimReimbursement { get; set; }

		/// <summary>
		/// get/set - The amount the employer will contribute for this participant cost.  This is calculated (ClaimParticipantCost - ClaimReimbursement).
		/// </summary>
		[Required, Min(0, ErrorMessage = "The employer contribution must be greater than or equal to 0.")]
		public decimal ClaimEmployerContribution { get; set; }

		#region Assessment
		/// <summary>
		/// get/set - The assessed cost for this participant cost.
		/// </summary>
		[Required, Min(0, ErrorMessage = "The participant cost must be greater than or equal to 0.")]
		public decimal AssessedParticipantCost { get; set; }

		/// <summary>
		/// get/set - The assessed reimbursement for this participant cost.  This is calculated (AssessedParticipantCost * GrantApplication.ReimbursementRate).
		/// </summary>
		[Required, Min(0, ErrorMessage = "The reimbursement must be greater than or equal to 0.")]
		public decimal AssessedReimbursement { get; set; }

		/// <summary>
		/// get/set - The assessed amount the employer will contribute for this participant cost.  This is calculated (AssessedParticipantCost - AssessedParticipantCost).
		/// </summary>
		[Required, Min(0, ErrorMessage = "The employer contribution must be greater than or equal to 0.")]
		public decimal AssessedEmployerContribution { get; set; }
		#endregion
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="ParticipantCost"/> object.
		/// </summary>
		public ParticipantCost()
		{

		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="ParticipantCost"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="eligibleCost"></param>
		/// <param name="participantFormId"></param>
		/// <param name="cost"></param>
		public ParticipantCost(ClaimEligibleCost eligibleCost, int participantFormId, decimal cost = 0)
		{
			if (cost < 0)
				throw new ArgumentException("The participant claim cost must be greater than or equal to 0.", nameof(cost));

			this.ClaimEligibleCost = eligibleCost ?? throw new ArgumentNullException(nameof(eligibleCost)); ;
			this.ClaimEligibleCostId = eligibleCost.Id;
			this.ParticipantFormId = participantFormId;
			this.ClaimParticipantCost = cost;
			if (eligibleCost.Claim.GrantApplication.GetProgramType() == ProgramTypes.EmployerGrant)
				this.RecalculateClaimParticipantCostETG();
			else
				this.RecalculateClaimCost();
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="ParticipantCost"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="eligibleCost"></param>
		/// <param name="participantForm"></param>
		/// <param name="cost"></param>
		public ParticipantCost(ClaimEligibleCost eligibleCost, ParticipantForm participantForm, decimal cost = 0) : this(eligibleCost, participantForm?.Id ?? 0, cost)
		{
			this.ParticipantForm = participantForm ?? throw new ArgumentNullException(nameof(participantForm));
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="ParticipantCost"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="eligibleCost"></param>
		/// <param name="participantCost"></param>
		public ParticipantCost(ClaimEligibleCost eligibleCost, ParticipantCost participantCost)
		{
			if (participantCost == null)
				throw new ArgumentNullException(nameof(participantCost));

			this.ClaimEligibleCost = eligibleCost ?? throw new ArgumentNullException(nameof(eligibleCost));
			this.ClaimEligibleCostId = eligibleCost.Id;
			this.ParticipantForm = participantCost.ParticipantForm;
			this.ParticipantFormId = participantCost.ParticipantFormId;
			this.ClaimParticipantCost = participantCost.AssessedParticipantCost;
			this.ClaimEmployerContribution = participantCost.AssessedEmployerContribution;
			this.ClaimReimbursement = participantCost.AssessedReimbursement;
			this.RecalculateClaimCost();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Validate the participant cost properties.
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

			// Must be associated with ClaimEligibleCost.
			if (this.ClaimEligibleCost == null && this.ClaimEligibleCostId == 0)
				yield return new ValidationResult("The participant cost must be associated with a claim eligible cost.", new[] { nameof(this.ClaimEligibleCost) });

			// Must be associated with ParticipantEnrollment.
			if (this.ParticipantForm == null && this.ParticipantFormId == 0)
				yield return new ValidationResult("The participant cost must be associated with a participant form.", new[] { nameof(this.ParticipantForm) });

			if (this.ClaimEligibleCost == null && this.ClaimEligibleCostId != 0)
				this.ClaimEligibleCost = context.Set<ClaimEligibleCost>().Include(m => m.Claim).Include(m => m.Claim.GrantApplication).First(cec => cec.Id == this.ClaimEligibleCostId);

			if (this.ClaimEligibleCost != null && this.ClaimEligibleCost?.EligibleCost == null && this.ClaimEligibleCost?.EligibleCostId != 0)
				this.ClaimEligibleCost.EligibleCost = context.Set<EligibleCost>().Find(this.ClaimEligibleCost.EligibleCostId);

			// Claim.
			if (this.ClaimEligibleCost.Claim.GrantApplication.GetProgramType() != ProgramTypes.EmployerGrant)
			{
				// The ClaimParticipantCost cannot be greater than the agreement maximum.
				if (this.ClaimParticipantCost > this.ClaimEligibleCost?.ClaimMaxParticipantCost)
					yield return new ValidationResult($"The participant cost must be less than or equal to the claimed maximum participant cost.", new[] { nameof(this.ClaimParticipantCost) });

				// The ClaimParticipantCost cannot be greater than the agreement maximum.
				if (this.ClaimParticipantCost > (this.ClaimEligibleCost?.EligibleCost?.AgreedMaxParticipantCost ?? this.ClaimEligibleCost?.Source?.AssessedMaxParticipantCost ?? this.ClaimEligibleCost?.AssessedMaxParticipantCost))
					yield return new ValidationResult($"The participant cost must be less than or equal to the agreed maximum participant cost.", new[] { nameof(this.ClaimParticipantCost) });

				// The ClaimParticipantCost must equal the calculated total ClaimReimbursement + ClaimEmployerContribution.
				if (this.ClaimParticipantCost - Math.Round(this.ClaimReimbursement + this.ClaimEmployerContribution, 2) > 0)
					yield return new ValidationResult("The participant cost must equal the reimbursement amount and employer contribution.", new[] { nameof(this.ClaimParticipantCost) });

				// The ClaimReimbursement amount cannot be greater than the GrantApplication.MaxReimbursementAmt.
				if (this.ClaimReimbursement > this.ClaimEligibleCost?.Claim.GrantApplication.MaxReimbursementAmt)
					yield return new ValidationResult($"Actual Cost for Participant exceeds Maximum Cost per Participant.", new[] { nameof(this.ClaimReimbursement) });

				// The ClaimReimbursement cannot be greater than the agreement maximum.
				if (this.ClaimEligibleCost != null && this.ClaimReimbursement > this.CalculateClaimReimbursement())
					yield return new ValidationResult($"The participant reimbursement must be less than or equal to the agreed maximum participant reimbursement.", new[] { nameof(this.ClaimReimbursement) });

				// Assessment.

				// The AssessedParticipantCost must be less than or equal to the assessed maximum participant cost.
				if (this.AssessedParticipantCost > this.ClaimEligibleCost?.AssessedMaxParticipantCost)
					yield return new ValidationResult("The assessed participant cost must be less than or equal to the assessed maximum participant cost.", new[] { nameof(this.AssessedParticipantCost) });

				if (this.AssessedParticipantCost > (this.ClaimEligibleCost?.EligibleCost?.AgreedMaxParticipantCost ?? this.ClaimEligibleCost?.Source?.AssessedMaxParticipantCost ?? this.ClaimEligibleCost?.AssessedMaxParticipantCost))
					yield return new ValidationResult($"The participant cost must be less than or equal to the agreed maximum participant cost.", new[] { nameof(this.AssessedParticipantCost) });

				// The AssessedParticipantCost must equal the calculated total AssessedReimbursement + AssessedEmployerContribution.
				if (this.AssessedParticipantCost - Math.Round(this.AssessedReimbursement + this.AssessedEmployerContribution, 2) > 0)
					yield return new ValidationResult("The assessed participant cost must equal the assessed reimbursement amount and assessed employer contribution.", new[] { nameof(this.AssessedParticipantCost) });

				// The ClaimReimbursement cannot be greater than the agreement maximum.
				if (this.AssessedReimbursement > this.ClaimEligibleCost?.AssessedMaxParticipantReimbursementCost)
					yield return new ValidationResult($"The participant reimbursement must be less than or equal to the agreed maximum participant reimbursement.", new[] { nameof(this.ClaimReimbursement) });
			}

			if (this.ClaimEligibleCost.Claim.GrantApplication.GetProgramType() == ProgramTypes.EmployerGrant)
			{
				if (this.ClaimEligibleCost?.ParticipantCosts.Sum(pc => pc.AssessedParticipantCost) > this.ClaimEligibleCost.AssessedCost)
				{
					var eligibleExpenseType = context.Set<EligibleExpenseType>().FirstOrDefault(eet => eet.Id == this.ClaimEligibleCost.EligibleExpenseTypeId);
					yield return new ValidationResult($"The sum of all participant costs exceeds the Assessed Amount for the expense type '{eligibleExpenseType?.Caption}'.", new[] { nameof(this.ClaimEligibleCost.AssessedCost) });
				}
			}

			foreach (var validation in base.Validate(validationContext))
			{
				yield return validation;
			}
		}
		#endregion
	}
}
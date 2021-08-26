using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="EligibleCostBreakdown"/> class, provides the ORM a way to manage eligible cost breakdowns for a grant application.
	/// </summary>
	public class EligibleCostBreakdown : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - The primary key uses IDENTITY
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - The foreign key to the parent eligible cost.
		/// </summary>
		public int EligibleCostId { get; set; }

		/// <summary>
		/// get/set - The parent eligible cost.
		/// </summary>
		[ForeignKey(nameof(EligibleCostId))]
		public virtual EligibleCost EligibleCost { get; set; }

		/// <summary>
		/// get/set - The foreign key to the eligible expense breakdown.
		/// </summary>
		public int EligibleExpenseBreakdownId { get; set; }

		/// <summary>
		/// get/set - The eligible expense breakdown.
		/// </summary>
		[ForeignKey(nameof(EligibleExpenseBreakdownId))]
		public virtual EligibleExpenseBreakdown EligibleExpenseBreakdown { get; set; }

		/// <summary>
		/// get/set - The estimated cost for this breakdown.
		/// </summary>
		[Required]
		[DisplayName("Breakdown Cost")]
		public decimal EstimatedCost { get; set; }

		/// <summary>
		/// get/set - The assessed cost for this breakdown.
		/// </summary>
		[DisplayName("Breakdown Cost")]
		public decimal AssessedCost { get; set; }

		/// <summary>
		/// get/set - Whether this breakdown is eligible or not.
		/// </summary>
		public bool IsEligible { get; set; } = true;

		/// <summary>
		/// get/set - Whether this breakdown was added by an assessor.
		/// </summary>
		public bool AddedByAssessor { get; set; } = false;

		/// <summary>
		/// get/set - All of the training programs associated with this breakdown (there should only ever be one).
		/// </summary>
		public virtual ICollection<TrainingProgram> TrainingPrograms { get; set; } = new List<TrainingProgram>();

		/// <summary>
		/// get - All the participants associated with this eligible cost breakdown.
		/// </summary>
		public virtual ICollection<ParticipantForm> ParticipantForms { get; set; } = new List<ParticipantForm>();

		/// <summary>
		/// get - All the related claim breakdown costs associated with this breakdown.
		/// </summary>
		public virtual ICollection<ClaimBreakdownCost> ClaimBreakdownCosts { get; set; } = new List<ClaimBreakdownCost>();
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of an EligibleCostBreakdown object.
		/// </summary>
		public EligibleCostBreakdown()
		{

		}

		/// <summary>
		/// Creates a new instance of an EligibleCostBreakdown object and initializes it.
		/// </summary>
		/// <param name="eligibleCost"></param>
		/// <param name="eligibleExpenseBreakdown"></param>
		/// <param name="estimatedCost"></param>
		public EligibleCostBreakdown(EligibleCost eligibleCost, EligibleExpenseBreakdown eligibleExpenseBreakdown, decimal estimatedCost)
		{
			this.EligibleCost = eligibleCost ?? throw new ArgumentNullException(nameof(eligibleCost));
			this.EligibleCostId = eligibleCost.Id;
			this.EligibleExpenseBreakdown = eligibleExpenseBreakdown ?? throw new ArgumentNullException(nameof(eligibleExpenseBreakdown));
			this.EligibleExpenseBreakdownId = eligibleExpenseBreakdown.Id;
			this.EstimatedCost = estimatedCost;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Validates the EligibleCostBreakdown properties.
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
			
			// The breakdown cannot be greater than the parent estimated cost.
			if (this.EstimatedCost > this.EligibleCost.EstimatedCost)
				yield return new ValidationResult($"The estimated cost cannot exceed {this.EligibleCost.EstimatedCost.ToString("c2")}.", new[] { nameof(this.EstimatedCost) });

			// The breakdown cannot be greater than the parent assessed cost.
			if (this.AssessedCost > this.EligibleCost.AgreedMaxCost)
				yield return new ValidationResult($"The agreed maximum cost cannot exceed {this.EligibleCost.AgreedMaxCost.ToString("c2")}.", new[] { nameof(this.AssessedCost) });

			foreach (var validation in base.Validate(validationContext))
			{
				yield return validation;
			}
		}
		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CJG.Core.Entities
{
	/// <summary>
	/// ClaimBreakdownCost class, provides a way to manage the amount claimed and assessed in a breakdown of a claim eligible cost.
	/// </summary>
	public class ClaimBreakdownCost : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - Primary key uses IDENTITY.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - Foreign key to the eligible expense breakdown.
		/// </summary>
		public int EligibleExpenseBreakdownId { get; set; }

		/// <summary>
		/// get/set - The eligible expense breakdown.
		/// </summary>
		[ForeignKey(nameof(EligibleExpenseBreakdownId))]
		public virtual EligibleExpenseBreakdown EligibleExpenseBreakdown { get; set; }

		/// <summary>
		/// get/set - Foreign key to the claim eligible cost.
		/// </summary>
		public int ClaimEligibleCostId { get; set; }

		/// <summary>
		/// get/set - The claim eligible cost.
		/// </summary>
		[ForeignKey(nameof(ClaimEligibleCostId))]
		public virtual ClaimEligibleCost ClaimEligibleCost { get; set; }

		/// <summary>
		/// get/set - Foreign key to the claim eligible expense breakdown.
		/// </summary>
		public int EligibleCostBreakdownId { get; set; }

		/// <summary>
		/// get/set - The claim eligible cost beakdown.
		/// </summary>
		[ForeignKey(nameof(EligibleCostBreakdownId))]
		public virtual EligibleCostBreakdown EligibleCostBreakdown { get; set; }


		/// <summary>
		/// get/set - The amount claimed.
		/// </summary>
		public decimal ClaimCost { get; set; }
	  
		/// <summary>
		/// get/set - the amount assessed.
		/// </summary>
		public decimal AssessedCost { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="ClaimBreakdownCost"/> object.
		/// </summary>
		public ClaimBreakdownCost() : base()
		{

		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="ClaimBreakdownCost"/> object.
		/// </summary>
		/// <param name="expenseBreakdown"></param>
		/// <param name="eligibleCost"></param>
		/// <param name="claimCost"></param>
		public ClaimBreakdownCost(EligibleExpenseBreakdown expenseBreakdown, ClaimEligibleCost eligibleCost, decimal claimCost = 0)
		{
			this.EligibleExpenseBreakdown = expenseBreakdown ?? throw new ArgumentNullException(nameof(expenseBreakdown));
			this.EligibleExpenseBreakdownId = expenseBreakdown.Id;
			this.ClaimEligibleCost = eligibleCost ?? throw new ArgumentNullException(nameof(eligibleCost));
			this.ClaimEligibleCostId = eligibleCost.Id;
			this.ClaimCost = claimCost;
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="ClaimBreakdownCost"/> object.
		/// </summary>
		/// <param name="eligibleCostBreakdown"></param>
		/// <param name="eligibleCost"></param>
		/// <param name="claimCost"></param>
		public ClaimBreakdownCost(EligibleCostBreakdown eligibleCostBreakdown, ClaimEligibleCost eligibleCost, decimal claimCost = 0)
		{
			this.EligibleCostBreakdown = eligibleCostBreakdown ?? throw new ArgumentNullException(nameof(eligibleCostBreakdown));
			this.EligibleCostBreakdownId = eligibleCostBreakdown.Id;
			this.EligibleExpenseBreakdown = eligibleCostBreakdown.EligibleExpenseBreakdown;
			this.EligibleExpenseBreakdownId = eligibleCostBreakdown.EligibleExpenseBreakdownId;
			this.ClaimEligibleCost = eligibleCost ?? throw new ArgumentNullException(nameof(eligibleCost));
			this.ClaimEligibleCostId = eligibleCost.Id;
			this.ClaimCost = claimCost;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Validates the claim breakdown cost.
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

			var eligibleExpenseBreakdown = this.EligibleExpenseBreakdown ?? context.Set<EligibleExpenseBreakdown>().Find(this.EligibleExpenseBreakdownId);
			var eligibleExpenseType = this.ClaimEligibleCost.EligibleExpenseType ?? context.Set<EligibleExpenseType>().Find(this.ClaimEligibleCost.EligibleExpenseTypeId);
			var breakdowns = this.ClaimEligibleCost.Breakdowns.Count() == 0 ? context.Set<ClaimBreakdownCost>().Where(b => b.ClaimEligibleCostId == this.ClaimEligibleCostId).ToArray() : this.ClaimEligibleCost.Breakdowns;

			// Can't exceed the claim costs.
			if (this.ClaimEligibleCost.ClaimCost < this.ClaimCost)
				yield return new ValidationResult($"The breakdown '{eligibleExpenseBreakdown.Caption}' claimed cost cannot exceed the '{eligibleExpenseType.Caption}' limit of {this.ClaimEligibleCost.ClaimCost:C}.", new[] { nameof(this.ClaimCost) });

			// Can't exceed the assessed costs.
			if (this.ClaimEligibleCost.AssessedCost < this.AssessedCost)
				yield return new ValidationResult($"The breakdown '{eligibleExpenseBreakdown.Caption}' assessed cost cannot exceed the '{eligibleExpenseType.Caption}' limit of {this.ClaimEligibleCost.AssessedCost:C}.", new[] { nameof(this.AssessedCost) });

			// Can't exceed the claim costs.
			if (this.ClaimEligibleCost.ClaimCost < breakdowns.Sum(b => b.ClaimCost))
				yield return new ValidationResult($"The sum of the '{eligibleExpenseType.Caption}' breakdown claimed costs cannot exceed the limit of {this.ClaimEligibleCost.ClaimCost:C}.", new[] { nameof(this.ClaimCost) });

			// Can't exceed the assessed costs.
			if (this.ClaimEligibleCost.AssessedCost < breakdowns.Sum(b => b.AssessedCost))
				yield return new ValidationResult($"The sum of the '{eligibleExpenseType.Caption}' breakdown assessed costs cannot exceed the limit of {this.ClaimEligibleCost.AssessedCost:C}.", new[] { nameof(this.AssessedCost) });

			foreach (var validation in base.Validate(validationContext))
			{
				yield return validation;
			}
		}
		#endregion
	}
}

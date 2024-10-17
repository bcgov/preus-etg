using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CJG.Core.Entities
{
	/// <summary>
	/// EligibleExpenseBreakdown class, provides a way to manage the breakdowns of an eligible expense type.
	/// </summary>
	public class EligibleExpenseBreakdown : LookupTable<int>
	{
		#region Properties
		/// <summary>
		/// get/set - A description for this eligible expense breakdown.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// get/set - Foreign key to the eligible expense type.
		/// </summary>
		[Index("IX_Caption", 2, IsUnique = true)]
		public int EligibleExpenseTypeId { get; set; }

		/// <summary>
		/// get/set - The eligible expense type.
		/// </summary>
		[ForeignKey(nameof(EligibleExpenseTypeId))]
		public virtual EligibleExpenseType ExpenseType { get; set; }

		/// <summary>
		/// get/set - The foreign key to the service line associated with this eligible expense breakdown.
		/// </summary>
		public int? ServiceLineId { get; set; }

		/// <summary>
		/// get/set - The service line associated with this eligible expense breakdown.
		/// </summary>
		[ForeignKey(nameof(ServiceLineId))]
		public virtual ServiceLine ServiceLine { get; set; }

		/// <summary>
		/// get/set - Whether the costs will be entered by the applicant for this eligible expense breakdown.
		/// </summary>
		[DefaultValue(true)]
		public bool EnableCost { get; set; } = true;

		/// <summary>
		/// get - The eligible cost breakdowns associated with this expense breakdown type.
		/// </summary>
		public virtual ICollection<EligibleCostBreakdown> EligibleCostBreakdowns { get; set; } = new List<EligibleCostBreakdown>();

		/// <summary>
		/// get - The claimed amounts associated with this breakdown.
		/// </summary>
		public virtual ICollection<ClaimBreakdownCost> ClaimBreakdownCosts { get; set; } = new List<ClaimBreakdownCost>();
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="EligibleExpenseBreakdown"/> object.
		/// </summary>
		public EligibleExpenseBreakdown() : base()
		{

		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="EligibleExpenseBreakdown"/> object.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="expenseType"></param>
		/// <param name="isActive"></param>
		public EligibleExpenseBreakdown(string caption, EligibleExpenseType expenseType, bool isActive = true, int rowSequence = 0) : base(caption, rowSequence)
		{
			this.ExpenseType = expenseType ?? throw new ArgumentNullException(nameof(expenseType));
			this.EligibleExpenseTypeId = expenseType.Id;
			this.IsActive = isActive;
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="EligibleExpenseBreakdown"/> object.
		/// </summary>
		/// <param name="serviceLine"></param>
		/// <param name="expenseType"></param>
		public EligibleExpenseBreakdown(ServiceLine serviceLine, EligibleExpenseType expenseType) : this(serviceLine?.Caption, expenseType, serviceLine?.IsActive ?? false, serviceLine?.RowSequence ?? 0)
		{
			this.ServiceLineId = serviceLine?.Id ?? throw new ArgumentNullException(nameof(serviceLine));
			this.Description = serviceLine.Description;
			this.EnableCost = serviceLine.ServiceCategory.ServiceTypeId == ServiceTypes.SkillsTraining ? true : false;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Validates the EligibleExpenseBreakdown property values.
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

			// Must have a unique caption within an expense type.
			if (this.ExpenseType.Breakdowns.Any(b => b.Caption == this.Caption && b.Id != this.Id))
				yield return new ValidationResult("The caption must be unique within an expense type.", new[] { nameof(this.Caption) });

			foreach (var validation in base.Validate(validationContext))
			{
				yield return validation;
			}
		}
		#endregion
	}
}

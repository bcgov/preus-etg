using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="EligibleCost"/> class, provides the ORM a way to manage eligible costs for a training program.
	/// </summary>
	public class EligibleCost : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - The primary key to identify this eligible cost.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - The foreign key to the parent training costs.
		/// </summary>
		public int GrantApplicationId { get; set; }

		/// <summary>
		/// get/set - The parent training cost.
		/// </summary>
		[ForeignKey(nameof(GrantApplicationId))]
		public virtual TrainingCost TrainingCost { get; set; }

		/// <summary>
		/// get/set - The foreign key to the eligible expense type.
		/// </summary>
		public int EligibleExpenseTypeId { get; set; }

		/// <summary>
		/// get/set - The associated eligible expense type this eligible cost belongs to.
		/// </summary>
		[ForeignKey(nameof(EligibleExpenseTypeId))]
		public virtual EligibleExpenseType EligibleExpenseType { get; set; }

		/// <summary>
		/// get/set - The estimated total cost.
		/// </summary>
		[DisplayName("Expense Cost")]
		[Required, Min(0, ErrorMessage = "The estimated cost must be greater than or equal to 0.")]
		public decimal EstimatedCost { get; set; }

		/// <summary>
		/// get/set - The estimated number of participants.
		/// </summary>
		[DisplayName("Number of Participants")]
		[Required, Min(1, ErrorMessage = "The estimated participants must be greater than or equal to 1.")]
		public int EstimatedParticipants { get; set; }

		/// <summary>
		/// get/set - The estimated cost per participant.  This is a calculated amount (EstimatedCost / EstimatedParticipants).
		/// </summary>
		[DisplayName("Cost per Participant")]
		[Required, Min(0, ErrorMessage = "The estimated participant cost must be greater than or equal to 0.")]
		public decimal EstimatedParticipantCost { get; set; }

		/// <summary>
		/// get/set - The estimated reimbursement amount for this eligible cost.  This is a calculated amount (EstimatedParticipantCost * GrantApplication.ReimbursementRate).
		/// </summary>
		[DisplayName("Government Contribution")]
		[Required, Min(0, ErrorMessage = "The estimated reimbursement must be greater than or equal to 0.")]
		public decimal EstimatedReimbursement { get; set; }

		/// <summary>
		/// get/set - The estimated amount the employer will contribute for this eligible cost.  This is a calculated amount (EstimatedCost - EstimatedReimbursement).
		/// </summary>
		[DisplayName("Employment Contribution")]
		[Required, Min(0, ErrorMessage = "The estimated employer contribution must be greater than or equal to 0.")]
		public decimal EstimatedEmployerContribution { get; set; }

		/// <summary>
		/// get - All of the child eligible cost breakdowns.
		/// </summary>
		public virtual ICollection<EligibleCostBreakdown> Breakdowns { get; set; } = new List<EligibleCostBreakdown>();
		public virtual ICollection<TrainingProvider> TrainingProviders { get; set; } = new List<TrainingProvider>();

		#region Assessment
		/// <summary>
		/// get/set - The agreed maximum cost.
		/// </summary>
		[DisplayName("Expense Cost")]
		[Required, Min(0, ErrorMessage = "The agreed max cost must be greater than or equal to 0.")]
		public decimal AgreedMaxCost { get; set; }

		/// <summary>
		/// get/set - The agreed maximum number of participants.
		/// </summary>
		[DisplayName("Number of Participants")]
		[Required, Min(0, ErrorMessage = "The agreed max participants must be greater than or equal to 0.")]
		public int AgreedMaxParticipants { get; set; }

		/// <summary>
		/// get/set - The agreed cost per participant.  This is a calculated amount (AgreedMaxCost / AgreedMaxParticipants).
		/// </summary>
		[DisplayName("Cost per Participant")]
		[Required, Min(0, ErrorMessage = "The agreed max participant cost must be greater than or equal to 0.")]
		public decimal AgreedMaxParticipantCost { get; set; }

		/// <summary>
		/// get/set - The agreed reimbursement amount for this eligible cost.  This is a calculated amount (AgreedMaxCost * GrantApplication.ReimbursementRate).
		/// </summary>
		[DisplayName("Government Contribution")]
		[Required, Min(0, ErrorMessage = "The agreed max reimbursement must be greater than or equal to 0.")]
		public decimal AgreedMaxReimbursement { get; set; }

		/// <summary>
		/// get/set - The agreed amount the employer will contribute for this eligible cost.  This is a calculated amount (AgreedMaxCost - AgreedMaxReimbursement).
		/// </summary>
		[DisplayName("Employment Contribution")]
		[Required, Min(0, ErrorMessage = "The agreed employer contribution must be greater than or equal to 0.")]
		public decimal AgreedEmployerContribution { get; set; }

		/// <summary>
		/// get/set - Flag to indicate that the eligible cost was added by the assessor after the application has been submitted
		/// </summary>
		public bool AddedByAssessor { get; set; } = false;
		#endregion
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="EligibleCost"/> object.
		/// </summary>
		public EligibleCost()
		{

		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="EligibleCost"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="trainingCost"></param>
		/// <param name="expenseType"></param>
		/// <param name="estimatedCost"></param>
		/// <param name="estimatedParticipants"></param>
		public EligibleCost(TrainingCost trainingCost, EligibleExpenseType expenseType, decimal estimatedCost, int estimatedParticipants)
		{
			this.TrainingCost = trainingCost ?? throw new ArgumentNullException(nameof(trainingCost)); ;
			this.GrantApplicationId = trainingCost.GrantApplicationId;
			this.EligibleExpenseType = expenseType ?? throw new ArgumentNullException(nameof(expenseType));
			this.EligibleExpenseTypeId = expenseType.Id;
			this.EstimatedCost = estimatedCost;
			this.EstimatedParticipants = estimatedParticipants;

			if (this.EstimatedParticipants == 0)
				this.EstimatedParticipantCost = 0;
			else
				this.EstimatedParticipantCost = Math.Round(estimatedCost / EstimatedParticipants, 2);

			var rate = trainingCost.GrantApplication.GrantOpening.GrantStream.ReimbursementRate;
			this.EstimatedReimbursement = Math.Round(estimatedCost * (decimal)rate, 2);
			this.EstimatedEmployerContribution = estimatedCost - this.EstimatedReimbursement;
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="EligibleCost"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="expenseType"></param>
		/// <param name="estimatedCost"></param>
		/// <param name="estimatedParticipants"></param>
		public EligibleCost(GrantApplication grantApplication, EligibleExpenseType expenseType, decimal estimatedCost, int estimatedParticipants)
			: this(grantApplication.TrainingCost, expenseType, estimatedCost, estimatedParticipants)
		{

		}

		#endregion

		#region Methods
		/// <summary>
		/// Validates the eligible cost.
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

			var grantApplication = context.Set<GrantApplication>().Include(m => m.GrantOpening).Include(m => m.GrantOpening.GrantStream).Include(m => m.GrantOpening.GrantStream.GrantProgram).Include(m => m.GrantOpening.GrantStream.ProgramConfiguration).FirstOrDefault(ga => ga.Id == this.TrainingCost.GrantApplicationId);

			var eligibleExpenseType = context.Set<EligibleExpenseType>().Include(m => m.ServiceCategory).FirstOrDefault(x => x.Id == this.EligibleExpenseTypeId);
			var reimbursementRate = this.TrainingCost.GrantApplication.ReimbursementRate;
			var maxReimbursementAmount = this.TrainingCost.GrantApplication.MaxReimbursementAmt;

			// Ensure EligibleExpenseType is in context.
			if (this.EligibleExpenseType == null)
				this.EligibleExpenseType = eligibleExpenseType;

			// AgreedParticipants cannot be greater than any eligible cost AgreedMaxParticipants.
			if (this.TrainingCost.AgreedParticipants < this.AgreedMaxParticipants)
				yield return new ValidationResult($"The number of participants for an eligible cost cannot be greater than the application maximum number of participants.", new[] { nameof(this.AgreedMaxParticipants) });

			// EstimatedCost must be equal to calculated EstimatedReimbursement + EstimatedEmployerContribution.
			if (Math.Round(this.EstimatedReimbursement + this.EstimatedEmployerContribution, 2) > Math.Round(this.EstimatedCost, 2))
				yield return new ValidationResult("The estimated reimbursement and employer contribution cannot exceed the estimated cost.", new[] { nameof(this.EstimatedCost), nameof(this.EstimatedReimbursement), nameof(this.EstimatedEmployerContribution) });

			// AgreedMaxCost must be equal to the calculated AgreedMaxReimbursement + AgreedEmployerContribution.
			if (Math.Round(this.AgreedMaxReimbursement + this.AgreedEmployerContribution, 2) > Math.Round(this.AgreedMaxCost, 2))
				yield return new ValidationResult("The agreed reimbursement and employer contribution cannot exceed the agreed cost.", new[] { nameof(this.AgreedMaxCost), nameof(this.EstimatedReimbursement), nameof(this.EstimatedEmployerContribution) });

			// Cannot enter more participants than specified in the training program.
			if (this.AddedByAssessor ? this.AgreedMaxParticipants > this.TrainingCost.AgreedParticipants : this.EstimatedParticipants > this.TrainingCost.EstimatedParticipants)
				yield return new ValidationResult($"The number of participants for expense type '{this.EligibleExpenseType.Caption}' cannot exceed the number of participants you entered in part 1, which was '{this.TrainingCost.EstimatedParticipants}'", new[] { nameof(this.EstimatedParticipants) });

			// Cannot add multiple expenses of the same type if they are not allowed.
			if (!this.EligibleExpenseType.AllowMultiple && context.Set<EligibleCost>().Count(x => x.GrantApplicationId == this.GrantApplicationId && x.EligibleExpenseTypeId == this.EligibleExpenseTypeId) > 1)
				yield return new ValidationResult($"Cannot add multiple expenses of type '{this.EligibleExpenseType.Caption}'.", new[] { nameof(this.EligibleExpenseType) });

			// Cannot exceed the estimated maximum cost limit.
			if (this.EligibleExpenseType.ExpenseTypeId.In(ExpenseTypes.AutoLimitEstimatedCosts))
			{
				this.EligibleExpenseType = this.EligibleExpenseType ?? context.Set<EligibleExpenseType>().SingleOrDefault(x => x.Id == this.EligibleExpenseTypeId);
				var maxEstimatedCost = this.TrainingCost.CalculateEstimatedCostLimit(this);
				if (maxEstimatedCost < this.EstimatedCost)
					yield return new ValidationResult($"Estimated cost exceeded maximum allowed '{maxEstimatedCost.ToString("c2")}' for expense type '{this.EligibleExpenseType.Caption}'", new[] { nameof(this.EstimatedCost) });
			}

			// An auto limit estimated costs must not exceed its limit.
			if (this.EligibleExpenseType.ExpenseTypeId.In(ExpenseTypes.AutoLimitEstimatedCosts))
			{
				var otherCost = this.TrainingCost.EligibleCosts.Sum(t => t.EstimatedCost) - this.EstimatedCost;
				if (this.EstimatedCost > otherCost * (decimal)(EligibleExpenseType.Rate ?? 0))
					yield return new ValidationResult($"{EligibleExpenseType.Caption} Total Cost exceeds the limit for your program total cost.", new[] { nameof(this.EstimatedCost) });
			}

			// The sum of the eligible cost breakdown must not be greater than the total.
			if (this.Breakdowns.Sum(ecb => ecb.EstimatedCost) > this.EstimatedCost)
				yield return new ValidationResult($"The sum of the breakdown costs must not exceed the estimated cost of {this.EstimatedCost.ToString("c2")}.", new[] { nameof(this.EstimatedCost) });

			// The sum of the eligible cost breakdown must not be greater than the total.
			if (this.Breakdowns.Sum(ecb => ecb.AssessedCost) > this.AgreedMaxCost)
				yield return new ValidationResult($"The sum of the breakdown costs must not exceed the agreed maximum cost of {this.AgreedMaxCost.ToString("c2")}.", new[] { nameof(this.AgreedMaxCost) });

			var program = this.TrainingCost.GrantApplication.GrantOpening.GrantStream.GrantProgram ?? context.Set<GrantProgram>().FirstOrDefault(x => x.Id == this.TrainingCost.GrantApplication.GrantOpening.GrantStream.GrantProgramId);

			if (program.ProgramTypeId == ProgramTypes.WDAService)
			{
				switch (this.EligibleExpenseType.ServiceCategory?.ServiceTypeId)
				{
					// Cannot exceed limit for skills training costs.
					case ServiceTypes.SkillsTraining:
						var skillTrainingMax = this.TrainingCost.GrantApplication.GrantOpening.GrantStream.ProgramConfiguration.SkillsTrainingMaxEstimatedParticipantCosts;
						if (this.CalculateEstimatedPerParticipantReimbursement() > skillTrainingMax)
							yield return new ValidationResult($"{EligibleExpenseType.Caption} average reimbursement cost per participant may not exceed '{skillTrainingMax.ToString("c2")}'.", new[] { nameof(this.EstimatedParticipantCost) });
						else if (this.CalculateAgreedPerParticipantReimbursement() > skillTrainingMax)
							yield return new ValidationResult($"{EligibleExpenseType.Caption} average reimbursement cost per participant may not exceed '{skillTrainingMax.ToString("c2")}'.", new[] { nameof(this.AgreedMaxParticipantCost) });
						break;
					// Cannot exceed limit for employment services and supports costs.
					case ServiceTypes.EmploymentServicesAndSupports:
						var essMax = this.TrainingCost.GrantApplication.GrantOpening.GrantStream.ProgramConfiguration.ESSMaxEstimatedParticipantCost;
						if (this.CalculateEstimatedPerParticipantReimbursement() > essMax)
							yield return new ValidationResult($"{EligibleExpenseType.Caption} average reimbursement cost per participant may not exceed '{essMax.ToString("c2")}'.", new[] { nameof(this.EstimatedParticipantCost) });
						else if (this.CalculateAgreedPerParticipantReimbursement() > essMax)
							yield return new ValidationResult($"{EligibleExpenseType.Caption} average reimbursement cost per participant may not exceed '{essMax.ToString("c2")}'.", new[] { nameof(this.AgreedMaxParticipantCost) });
						break;
				}
			}


			//Cannot exceed the per participant fiscal year maximum reimbursement allowed.
			if (this.TrainingCost.GrantApplication.GetProgramType() != ProgramTypes.EmployerGrant
				&& this.EligibleExpenseType.ExpenseTypeId.In(ExpenseTypes.ParticipantLimited, ExpenseTypes.ParticipantAssigned)
				&& TrainingCostExtensions.CalculateRoundedReimbursementAmount(this.EstimatedParticipantCost, reimbursementRate) > maxReimbursementAmount)
				yield return new ValidationResult($"The total requested government contribution exceeds the per participant fiscal year maximum reimbursement allowed of '{maxReimbursementAmount.ToString("c2")}'", new[] { nameof(this.EstimatedReimbursement) });

			foreach (var validation in base.Validate(validationContext))
			{
				yield return validation;
			}
		}

		public void Clone(EligibleCost ec)
		{
			EligibleExpenseType = ec.EligibleExpenseType;
			EligibleExpenseTypeId = ec.EligibleExpenseTypeId;
			EstimatedCost = ec.EstimatedCost;
			EstimatedParticipants = ec.EstimatedParticipants;
			EstimatedParticipantCost = ec.EstimatedParticipantCost;
			EstimatedReimbursement = ec.EstimatedReimbursement;
			EstimatedEmployerContribution = ec.EstimatedEmployerContribution;
			AgreedMaxCost = ec.AgreedMaxCost;
			AgreedMaxParticipants = ec.AgreedMaxParticipants;
			AgreedMaxParticipantCost = ec.AgreedMaxParticipantCost;
			AgreedMaxReimbursement = ec.AgreedMaxReimbursement;
			AgreedEmployerContribution = ec.AgreedEmployerContribution;		
		}
		#endregion
	}
}

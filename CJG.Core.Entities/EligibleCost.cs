using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using DataAnnotationsExtensions;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="EligibleCost"/> class, provides the ORM a way to manage eligible costs for a training program.
	/// </summary>
	public class EligibleCost : EntityBase
	{
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

		[DisplayName("Expense Description")]
		public string ExpenseExplanation { get; set; }

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
			TrainingCost = trainingCost ?? throw new ArgumentNullException(nameof(trainingCost)); ;
			GrantApplicationId = trainingCost.GrantApplicationId;
			EligibleExpenseType = expenseType ?? throw new ArgumentNullException(nameof(expenseType));
			EligibleExpenseTypeId = expenseType.Id;
			EstimatedCost = estimatedCost;
			EstimatedParticipants = estimatedParticipants;

			if (EstimatedParticipants == 0)
				EstimatedParticipantCost = 0;
			else
				EstimatedParticipantCost = Math.Round(estimatedCost / EstimatedParticipants, 2);

			var rate = trainingCost.GrantApplication.GrantOpening.GrantStream.ReimbursementRate;
			EstimatedReimbursement = Math.Round(estimatedCost * (decimal)rate, 2);
			EstimatedEmployerContribution = estimatedCost - EstimatedReimbursement;
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

			var grantApplication = context.Set<GrantApplication>().Include(m => m.GrantOpening).Include(m => m.GrantOpening.GrantStream).Include(m => m.GrantOpening.GrantStream.GrantProgram).Include(m => m.GrantOpening.GrantStream.ProgramConfiguration).FirstOrDefault(ga => ga.Id == TrainingCost.GrantApplicationId);

			var eligibleExpenseType = context.Set<EligibleExpenseType>().Include(m => m.ServiceCategory).FirstOrDefault(x => x.Id == EligibleExpenseTypeId);
			var reimbursementRate = TrainingCost.GrantApplication.ReimbursementRate;
			var maxReimbursementAmount = TrainingCost.GrantApplication.MaxReimbursementAmt;

			// Ensure EligibleExpenseType is in context.
			if (EligibleExpenseType == null)
				EligibleExpenseType = eligibleExpenseType;

			// AgreedParticipants cannot be greater than any eligible cost AgreedMaxParticipants.
			if (TrainingCost.AgreedParticipants < AgreedMaxParticipants)
				yield return new ValidationResult("The number of participants for an eligible cost cannot be greater than the application maximum number of participants.", new[] { nameof(AgreedMaxParticipants) });

			// EstimatedCost must be equal to calculated EstimatedReimbursement + EstimatedEmployerContribution.
			if (Math.Round(EstimatedReimbursement + EstimatedEmployerContribution, 2) > Math.Round(EstimatedCost, 2))
				yield return new ValidationResult("The estimated reimbursement and employer contribution cannot exceed the estimated cost.", new[] { nameof(EstimatedCost), nameof(EstimatedReimbursement), nameof(EstimatedEmployerContribution) });

			// AgreedMaxCost must be equal to the calculated AgreedMaxReimbursement + AgreedEmployerContribution.
			if (Math.Round(AgreedMaxReimbursement + AgreedEmployerContribution, 2) > Math.Round(AgreedMaxCost, 2))
				yield return new ValidationResult("The agreed reimbursement and employer contribution cannot exceed the agreed cost.", new[] { nameof(AgreedMaxCost), nameof(EstimatedReimbursement), nameof(EstimatedEmployerContribution) });

			// Cannot enter more participants than specified in the training program.
			if (AddedByAssessor ? AgreedMaxParticipants > TrainingCost.AgreedParticipants : EstimatedParticipants > TrainingCost.EstimatedParticipants)
				yield return new ValidationResult($"The number of participants for expense type '{EligibleExpenseType.Caption}' cannot exceed the number of participants you entered above, which was '{TrainingCost.EstimatedParticipants}'", new[] { nameof(EstimatedParticipants) });

			// Cannot add multiple expenses of the same type if they are not allowed.
			if (!EligibleExpenseType.AllowMultiple && context.Set<EligibleCost>().Count(x => x.GrantApplicationId == GrantApplicationId && x.EligibleExpenseTypeId == EligibleExpenseTypeId) > 1)
				yield return new ValidationResult($"Cannot add multiple expenses of type '{EligibleExpenseType.Caption}'.", new[] { nameof(EligibleExpenseType) });

			// Cannot exceed the estimated maximum cost limit.
			if (EligibleExpenseType.ExpenseTypeId.In(ExpenseTypes.AutoLimitEstimatedCosts))
			{
				EligibleExpenseType = EligibleExpenseType ?? context.Set<EligibleExpenseType>().SingleOrDefault(x => x.Id == EligibleExpenseTypeId);
				var maxEstimatedCost = TrainingCost.CalculateEstimatedCostLimit(this);
				if (maxEstimatedCost < EstimatedCost)
					yield return new ValidationResult($"Estimated cost exceeded maximum allowed '{maxEstimatedCost.ToString("c2")}' for expense type '{EligibleExpenseType.Caption}'", new[] { nameof(EstimatedCost) });
			}

			// An auto limit estimated costs must not exceed its limit.
			if (EligibleExpenseType.ExpenseTypeId.In(ExpenseTypes.AutoLimitEstimatedCosts))
			{
				var otherCost = TrainingCost.EligibleCosts.Sum(t => t.EstimatedCost) - EstimatedCost;
				if (EstimatedCost > otherCost * (decimal)(EligibleExpenseType.Rate ?? 0))
					yield return new ValidationResult($"{EligibleExpenseType.Caption} Total Cost exceeds the limit for your program total cost.", new[] { nameof(EstimatedCost) });
			}

			// The sum of the eligible cost breakdown must not be greater than the total.
			if (Breakdowns.Sum(ecb => ecb.EstimatedCost) > EstimatedCost)
				yield return new ValidationResult($"The sum of the breakdown costs must not exceed the estimated cost of {EstimatedCost.ToString("c2")}.", new[] { nameof(EstimatedCost) });

			// The sum of the eligible cost breakdown must not be greater than the total.
			if (Breakdowns.Sum(ecb => ecb.AssessedCost) > AgreedMaxCost)
				yield return new ValidationResult($"The sum of the breakdown costs must not exceed the agreed maximum cost of {AgreedMaxCost.ToString("c2")}.", new[] { nameof(AgreedMaxCost) });

			var program = TrainingCost.GrantApplication.GrantOpening.GrantStream.GrantProgram ?? context.Set<GrantProgram>().FirstOrDefault(x => x.Id == TrainingCost.GrantApplication.GrantOpening.GrantStream.GrantProgramId);

			if (program.ProgramTypeId == ProgramTypes.WDAService)
			{
				switch (EligibleExpenseType.ServiceCategory?.ServiceTypeId)
				{
					// Cannot exceed limit for skills training costs.
					case ServiceTypes.SkillsTraining:
						var skillTrainingMax = TrainingCost.GrantApplication.GrantOpening.GrantStream.ProgramConfiguration.SkillsTrainingMaxEstimatedParticipantCosts;
						if (this.CalculateEstimatedPerParticipantReimbursement() > skillTrainingMax)
							yield return new ValidationResult($"{EligibleExpenseType.Caption} average reimbursement cost per participant may not exceed '{skillTrainingMax.ToString("c2")}'.", new[] { nameof(EstimatedParticipantCost) });
						else if (this.CalculateAgreedPerParticipantReimbursement() > skillTrainingMax)
							yield return new ValidationResult($"{EligibleExpenseType.Caption} average reimbursement cost per participant may not exceed '{skillTrainingMax.ToString("c2")}'.", new[] { nameof(AgreedMaxParticipantCost) });
						break;
					// Cannot exceed limit for employment services and supports costs.
					case ServiceTypes.EmploymentServicesAndSupports:
						var essMax = TrainingCost.GrantApplication.GrantOpening.GrantStream.ProgramConfiguration.ESSMaxEstimatedParticipantCost;
						if (this.CalculateEstimatedPerParticipantReimbursement() > essMax)
							yield return new ValidationResult($"{EligibleExpenseType.Caption} average reimbursement cost per participant may not exceed '{essMax.ToString("c2")}'.", new[] { nameof(EstimatedParticipantCost) });
						else if (this.CalculateAgreedPerParticipantReimbursement() > essMax)
							yield return new ValidationResult($"{EligibleExpenseType.Caption} average reimbursement cost per participant may not exceed '{essMax.ToString("c2")}'.", new[] { nameof(AgreedMaxParticipantCost) });
						break;
				}
			}

			//Cannot exceed the per participant fiscal year maximum reimbursement allowed.
			if (TrainingCost.GrantApplication.GetProgramType() != ProgramTypes.EmployerGrant
				&& EligibleExpenseType.ExpenseTypeId.In(ExpenseTypes.ParticipantLimited, ExpenseTypes.ParticipantAssigned)
				&& TrainingCostExtensions.CalculateRoundedReimbursementAmount(EstimatedParticipantCost, reimbursementRate) > maxReimbursementAmount)
				yield return new ValidationResult($"The total requested government contribution exceeds the per participant fiscal year maximum reimbursement allowed of '{maxReimbursementAmount.ToString("c2")}'", new[] { nameof(EstimatedReimbursement) });

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

			ExpenseExplanation = ec.ExpenseExplanation;
		}
	}
}

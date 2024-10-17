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
	/// TrainingCost class, provides a way to manage the costs associated with a grant application.
	/// This is a one-to-one relationship with the grant application.
	/// </summary>
	public class TrainingCost : EntityBase
	{
		/// <summary>
		/// get/set - The primary key, which is the foreign key to the parent grant application.
		/// </summary>
		[Required, Key, ForeignKey(nameof(GrantApplication)), DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int GrantApplicationId { get; set; }

		/// <summary>
		/// get/set - The parent grant application.
		/// </summary>
		public virtual GrantApplication GrantApplication { get; set; }

		/// <summary>
		/// get/set - The current state of the cost estimate.
		/// </summary>
		[DisplayName("State")]
		[DefaultValue(TrainingCostStates.Incomplete)]
		public TrainingCostStates TrainingCostState { get; set; }

		/// <summary>
		/// get/set - Estimated number of participants.
		/// </summary>
		[DisplayName("Number of Participants")]
		[Required, Min(0, ErrorMessage = "The estimated number of participants must be greater than or equal to 0."), Max(1500, ErrorMessage = "The estimated number of participants must be less than or equal to 1500.")]
		public int EstimatedParticipants { get; set; }

		/// <summary>
		/// get/set - Estimated total cost of all eligible costs.
		/// </summary>
		[Required]
		[DisplayName("Total Training Cost")]
		public decimal TotalEstimatedCost { get; set; }

		/// <summary>
		/// get/set - Estimated total reimbursement of all eligible costs.
		/// </summary>
		[Required]
		[DisplayName("Total Government Contribution")]
		public decimal TotalEstimatedReimbursement { get; set; }

		/// <summary>
		/// get/set - Agreed number of participants.
		/// </summary>
		[DisplayName("Number of Participants")]
		[Min(0, ErrorMessage = "The agreed number of participants must be greater than or equal to 0.")]
		public int AgreedParticipants { get; set; }

		/// <summary>
		/// get/set - Agreed total maximum cost of all eligible costs.
		/// </summary>
		[DisplayName("Total Training Cost")]
		public decimal TotalAgreedMaxCost { get; set; }

		/// <summary>
		/// get/set - Agreed reimbursement commitment of all eligible costs.
		/// </summary>
		[DisplayName("Total Government Contribution")]
		public decimal AgreedCommitment { get; set; }

		public int? TravelExpenseDocumentId { get; set; }

		[ForeignKey(nameof(TravelExpenseDocumentId))]
		public virtual Attachment TravelExpenseDocument { get; set; }

		/// <summary>
		/// get - All the eligible cost line items associated with this grant application.
		/// </summary>
		public virtual ICollection<EligibleCost> EligibleCosts { get; set; } = new List<EligibleCost>();

		/// <summary>
		/// Creates a new instance of a TrainingCost object.
		/// </summary>
		public TrainingCost()
		{ }

		/// <summary>
		/// Creates a new instance of
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="estimatedParticipants"></param>
		public TrainingCost(GrantApplication grantApplication, int estimatedParticipants)
		{
			GrantApplication = grantApplication ?? throw new ArgumentNullException(nameof(grantApplication));
			GrantApplicationId = grantApplication.Id;
			EstimatedParticipants = estimatedParticipants;
		}

		/// <summary>
		/// Validate the TrainingCost property values.
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

			var grantApplication = context.Set<GrantApplication>()
				.Include(m => m.GrantOpening)
				.Include(m => m.GrantOpening.GrantStream)
				.Include(m => m.GrantOpening.GrantStream.GrantProgram)
				.Include(m => m.GrantOpening.GrantStream.ProgramConfiguration)
				.Include(m => m.TrainingCost.EligibleCosts)
				.FirstOrDefault(ga => ga.Id == GrantApplicationId);

			// TotalEstimatedCost must not be greater than sum of EligibleCosts.
			var totalEstimatedCost = EligibleCosts.Sum(ec => ec.EstimatedCost);
			if (totalEstimatedCost != TotalEstimatedCost)
				yield return new ValidationResult($"The total estimated cost is incorrect and should be {totalEstimatedCost.ToString("c2")}", new[] { nameof(TotalEstimatedCost) });

			if (GrantApplication.GetProgramType() == ProgramTypes.EmployerGrant && TotalEstimatedReimbursement > GrantApplication.MaxReimbursementAmt * EstimatedParticipants)
				yield return new ValidationResult($"The total estimated reimbursement is incorrect and should be {(GrantApplication.MaxReimbursementAmt * EstimatedParticipants).ToString("c2")}", new[] { nameof(TotalEstimatedReimbursement) });

			// TotalAgreedMaxCost must not be greater than sum of EligibleCosts.
			var totalAgreedMaxCost = EligibleCosts.Sum(ec => ec.AgreedMaxCost);
			if (totalAgreedMaxCost != TotalAgreedMaxCost)
				yield return new ValidationResult($"The total agreed maximum cost is incorrect and should be {totalAgreedMaxCost.ToString("c2")}", new[] { nameof(TotalAgreedMaxCost) });

			// TotalAgreedMaxCost must not be greater than sum of EligibleCosts.
			//var totalAgreedCommitment = EligibleCosts.Sum(ec => ec.AgreedMaxReimbursement);
			//if (GrantApplication.GetProgramType() != ProgramTypes.EmployerGrant && totalAgreedCommitment != AgreedCommitment)
			//	yield return new ValidationResult($"The total agreed commitment is incorrect and should be {totalAgreedCommitment.ToString("c2")}", new[] { nameof(AgreedCommitment) });

			// Only test EligibleCosts if the participant value has been set.
			if (!GrantApplication.IsUnderAssessment() && EstimatedParticipants <= 0 && TrainingCostState == TrainingCostStates.Complete)
				yield return new ValidationResult("You must enter the number of participants.", new[] { nameof(EstimatedParticipants) });

			if (EligibleCosts.Any())
			{
				// EstimatedParticipants must be equal to or greater than EligibleCosts.Participants.
				if (EligibleCosts.Any(ec => ec.AddedByAssessor ? ec.AgreedMaxParticipants > AgreedParticipants : ec.EstimatedParticipants > EstimatedParticipants))
					yield return new ValidationResult($"The number of participants for one expense type cannot exceed the number of participants you entered above, which was {EstimatedParticipants}.", new[] { nameof(EstimatedParticipants) });

				// AgreedParticipants must be greater 0.
				//if (!GrantApplication.ApplicationStateInternal.In(ApplicationStateInternal.Draft, ApplicationStateInternal.ApplicationDenied, ApplicationStateInternal.ApplicationWithdrawn, ApplicationStateInternal.Unfunded)
				//	&& AgreedParticipants < 1)
				//	yield return new ValidationResult("The number of participants must be greater than 0.", new[] { nameof(AgreedParticipants) });

				// AgreedParticipants cannot be greater than any eligible cost AgreedMaxParticipants.
				// Text has been updated as par Ian's request - http://tfs.jtst.gov.bc.ca/tfs/Economy/CJG%20-%20Canada%20Job%20Grant/_workitems?_a=edit&id=5145
				if (EligibleCosts.Any(ec => ec.AgreedMaxParticipants > AgreedParticipants))
					yield return new ValidationResult($"The number of participants for an eligible cost cannot be greater than the application maximum number of participants, which was {AgreedParticipants}.", new[] { nameof(AgreedParticipants) });

				var ids = EligibleCosts.Select(x => x.Id)
					.ToList();
				var eligibleCosts = context.Set<EligibleCost>()
					.Include(m => m.EligibleExpenseType)
					.Include(m => m.EligibleExpenseType.ServiceCategory)
					.Where(ec => ids.Any(y => y == ec.Id))
					.ToArray();

				// Cannot exceed maximum participant costs employment services and supports components.
				var totalEstimatedESSAverageParticipantCost = TrainingCostExtensions.CalculatePerParticipantReimbursement(EligibleCosts.Where(t => t.EligibleExpenseType.ServiceCategory?.ServiceTypeId == ServiceTypes.EmploymentServicesAndSupports).Sum(t => t.EstimatedReimbursement), EstimatedParticipants);
				var totalAgreedESSAverageParticipantCost = TrainingCostExtensions.CalculatePerParticipantReimbursement(EligibleCosts.Where(t => t.EligibleExpenseType.ServiceCategory?.ServiceTypeId == ServiceTypes.EmploymentServicesAndSupports).Sum(t => t.AgreedMaxReimbursement), AgreedParticipants);
				var maxESSParticipantCost = GrantApplication.GrantOpening.GrantStream.ProgramConfiguration.ESSMaxEstimatedParticipantCost;
				var essServiceCategories = string.Join(" and ", context.Set<ServiceCategory>().Where(t => t.ServiceTypeId == ServiceTypes.EmploymentServicesAndSupports && t.IsActive).Select(t => t.Caption).ToArray());

				if (totalEstimatedESSAverageParticipantCost > maxESSParticipantCost)
					yield return new ValidationResult($"The total average cost per participant for {essServiceCategories} combined may not exceed {maxESSParticipantCost.ToString("c2")}.", new[] { nameof(TotalEstimatedReimbursement) });
				else if (totalAgreedESSAverageParticipantCost > maxESSParticipantCost)
					yield return new ValidationResult($"The total average cost per participant for {essServiceCategories} combined may not exceed {maxESSParticipantCost.ToString("c2")}.", new[] { nameof(AgreedCommitment) });

				// Cannot exceed maximum participant costs for skills training components.
				var totalEstimatedSkillsTrainingAverageParticipantCost = TrainingCostExtensions.CalculatePerParticipantReimbursement(EligibleCosts.Where(t => t.EligibleExpenseType.ServiceCategory?.ServiceTypeId == ServiceTypes.SkillsTraining).Sum(t => t.EstimatedReimbursement), EstimatedParticipants);
				var totalAgreedSkillsTrainingAverageParticipantCost = TrainingCostExtensions.CalculatePerParticipantReimbursement(EligibleCosts.Where(t => t.EligibleExpenseType.ServiceCategory?.ServiceTypeId == ServiceTypes.SkillsTraining).Sum(t => t.AgreedMaxReimbursement), AgreedParticipants);
				var maxSkillsTrainingParticipantCost = GrantApplication.GrantOpening.GrantStream.ProgramConfiguration.SkillsTrainingMaxEstimatedParticipantCosts;
				if (totalEstimatedSkillsTrainingAverageParticipantCost > maxSkillsTrainingParticipantCost)
					yield return new ValidationResult($"The total average cost per participant for skills training components may not exceed {maxSkillsTrainingParticipantCost.ToString("c2")}.", new[] { nameof(TotalEstimatedReimbursement) });
				else if (totalAgreedSkillsTrainingAverageParticipantCost > maxSkillsTrainingParticipantCost)
					yield return new ValidationResult($"The total average cost per participant for skills training components may not exceed {maxSkillsTrainingParticipantCost.ToString("c2")}.", new[] { nameof(AgreedCommitment) });

				var maxReimbursementAmount = GrantApplication.MaxReimbursementAmt;
				// TotalEstimatedReimbursement must less than or equal to agreed MaxReimbursementAmount.
				// Validate only if before and during application submit OR if the estimated values are updated.

				if (GrantApplication.GetProgramType() == ProgramTypes.EmployerGrant && EstimatedParticipants > 0 && TotalEstimatedReimbursement > maxReimbursementAmount * EstimatedParticipants)
					yield return new ValidationResult($"The total requested government contribution exceeds the per participant fiscal year maximum reimbursement allowed of '${maxReimbursementAmount:#,##0.00}'.", new[] { nameof(TotalEstimatedReimbursement) });

				// Cannot exceed grant opening budget.
				if (GrantApplication.ApplicationStateInternal != ApplicationStateInternal.New && AgreedCommitment > GrantApplication.GrantOpening.BudgetAllocationAmt)
					yield return new ValidationResult($"The total agreed commitment may not exceed the grant opening budget allocation of {GrantApplication.GrantOpening.BudgetAllocationAmt.ToString("c2")}.", new[] { nameof(AgreedCommitment) });

				// Ignore the following validation conditions when returning a denied application to assessment.
				var originalState = (ApplicationStateInternal)context.OriginalValue(GrantApplication, nameof(GrantApplication.ApplicationStateInternal));
				if (originalState != ApplicationStateInternal.ApplicationDenied && GrantApplication.ApplicationStateInternal != ApplicationStateInternal.ReturnedToAssessment)
				{
					// Need to force load any of the related entities.
					context.Set<GrantOpening>().Include(m => m.GrantStream).Include(m => m.GrantStream.ProgramConfiguration).Include(m => m.GrantStream.ProgramConfiguration.EligibleExpenseTypes).FirstOrDefault(g => g.Id == GrantApplication.GrantOpeningId);
					var eligibleExpenseTypeIds = GrantApplication.GrantOpening.GrantStream.ProgramConfiguration.EligibleExpenseTypes.Select(eet => eet.Id).ToArray();

					foreach (var eligibleCost in EligibleCosts)
					{
						// Validate whether the selected expense types are valid for the specified grant opening.
						if (!GrantApplication.GrantOpening.GrantStream.ProgramConfiguration.EligibleExpenseTypes.Any(eet => eet.Id == eligibleCost.EligibleExpenseTypeId))
							yield return new ValidationResult($"The expense type '{eligibleCost.EligibleExpenseType.Caption}' is not valid for the selected grant opening.", new[] { nameof(EligibleCosts) });

						if (eligibleCost.EligibleExpenseType.ServiceCategory?.ServiceTypeId != ServiceTypes.SkillsTraining)
							continue;

						// There must be at least # eligible skills training component.
						if (GrantApplication.ApplicationStateInternal.In(ApplicationStateInternal.Draft, ApplicationStateInternal.New, ApplicationStateInternal.ApplicationDenied, ApplicationStateInternal.Unfunded, ApplicationStateInternal.ApplicationWithdrawn))
							continue;

						if (eligibleCost.Breakdowns.Count(b => b.IsEligible) < eligibleCost.EligibleExpenseType?.ServiceCategory.MinPrograms)
							yield return new ValidationResult($"{eligibleCost.EligibleExpenseType.Caption} expenses require at least {eligibleCost.EligibleExpenseType?.ServiceCategory.MinPrograms} components to be eligible.", new[] { nameof(EligibleCostBreakdown.IsEligible) });
						else if (eligibleCost.Breakdowns.Count(b => b.IsEligible && b.AssessedCost == 0) > 0)
							yield return new ValidationResult("Eligible Skill Training require cost larger than 0.", new[] { nameof(EligibleCostBreakdown.IsEligible) });
					}

				}
			}

			foreach (var validation in base.Validate(validationContext))
				yield return validation;
		}

		public void Clone(TrainingCost tc)
		{
			TrainingCostState = tc.TrainingCostState;

			EstimatedParticipants = tc.EstimatedParticipants;
			AgreedParticipants = tc.AgreedParticipants;
			AgreedCommitment = tc.AgreedCommitment;
			TotalEstimatedCost = tc.TotalEstimatedCost;
			TotalEstimatedReimbursement = tc.TotalEstimatedReimbursement;
			TotalAgreedMaxCost = tc.TotalAgreedMaxCost;
		}
	}
}

using DataAnnotationsExtensions;
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
	/// TrainingCost class, provides a way to manage the costs associated with a grant application.
	/// This is a one-to-one relationship with the grant application.
	/// </summary>
	public class TrainingCost : EntityBase
	{
		#region Properties
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

		/// <summary>
		/// get - All the eligible cost line items associated with this grant application.
		/// </summary>
		public virtual ICollection<EligibleCost> EligibleCosts { get; set; } = new List<EligibleCost>();

		#endregion

		#region Constructors
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
			this.GrantApplication = grantApplication ?? throw new ArgumentNullException(nameof(grantApplication));
			this.GrantApplicationId = grantApplication.Id;
			this.EstimatedParticipants = estimatedParticipants;
		}
		#endregion

		#region Methods
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

			var grantApplication = context.Set<GrantApplication>().Include(m => m.GrantOpening).Include(m => m.GrantOpening.GrantStream).Include(m => m.GrantOpening.GrantStream.GrantProgram).Include(m => m.GrantOpening.GrantStream.ProgramConfiguration).Include(m => m.TrainingCost.EligibleCosts).FirstOrDefault(ga => ga.Id == this.GrantApplicationId);

			// TotalEstimatedCost must not be greater than sum of EligibleCosts.
			var totalEstimatedCost = this.EligibleCosts.Sum(ec => ec.EstimatedCost);
			if (totalEstimatedCost != this.TotalEstimatedCost)
				yield return new ValidationResult($"The total estimated cost is incorrect and should be {totalEstimatedCost.ToString("c2")}", new[] { nameof(this.TotalEstimatedCost) });

			// TotalEstimatedReimbursement must not be greater than sum of EligibleCosts. This is now only applied to CWRG. Validation is no longer applied to eligibile cost level, but at higher level on training cost for ETG
			var totalEstimatedReimbursement = this.EligibleCosts.Sum(ec => ec.EstimatedReimbursement);
			if (this.GrantApplication.GetProgramType() == ProgramTypes.EmployerGrant && this.TotalEstimatedReimbursement > this.GrantApplication.MaxReimbursementAmt * this.EstimatedParticipants)
				yield return new ValidationResult($"The total estimated reimbursement is incorrect and should be {(this.GrantApplication.MaxReimbursementAmt * this.EstimatedParticipants).ToString("c2")}", new[] { nameof(this.TotalEstimatedReimbursement) });

			if (this.GrantApplication.GetProgramType() != ProgramTypes.EmployerGrant && totalEstimatedReimbursement != this.TotalEstimatedReimbursement)
				yield return new ValidationResult($"The total estimated reimbursement is incorrect and should be {totalEstimatedReimbursement.ToString("c2")}", new[] { nameof(this.TotalEstimatedReimbursement) });

			// TotalAgreedMaxCost must not be greater than sum of EligibleCosts.
			var totalAgreedMaxCost = this.EligibleCosts.Sum(ec => ec.AgreedMaxCost);
			if (totalAgreedMaxCost != this.TotalAgreedMaxCost)
				yield return new ValidationResult($"The total agreed maximum cost is incorrect and should be {totalAgreedMaxCost.ToString("c2")}", new[] { nameof(this.TotalAgreedMaxCost) });

			// TotalAgreedMaxCost must not be greater than sum of EligibleCosts.
			var totalAgreedCommitment = this.EligibleCosts.Sum(ec => ec.AgreedMaxReimbursement);
			if (this.GrantApplication.GetProgramType() != ProgramTypes.EmployerGrant && totalAgreedCommitment != this.AgreedCommitment)
				yield return new ValidationResult($"The total agreed commitment is incorrect and should be {totalAgreedCommitment.ToString("c2")}", new[] { nameof(this.AgreedCommitment) });

			// Only test EligibleCosts if the participant value has been set.
			if (!this.GrantApplication.IsUnderAssessment() && this.EstimatedParticipants <= 0 && this.TrainingCostState == TrainingCostStates.Complete)
				yield return new ValidationResult("You must enter the number of participants.", new[] { nameof(this.EstimatedParticipants) });

			if (this.EligibleCosts.Any())
			{
				// EstimatedParticipants must be equal to or greater than EligibleCosts.Participants.
				if (this.EligibleCosts.Any(ec => ec.AddedByAssessor ? ec.AgreedMaxParticipants > this.AgreedParticipants : ec.EstimatedParticipants > this.EstimatedParticipants))
					yield return new ValidationResult($"The number of participants for one expense type cannot exceed the number of participants you entered in part 1, which was {this.EstimatedParticipants}.", new[] { nameof(this.EstimatedParticipants) });

				// AgreedParticipants must be greater 0.
				if (!this.GrantApplication.ApplicationStateInternal.In(ApplicationStateInternal.Draft, ApplicationStateInternal.ApplicationDenied, ApplicationStateInternal.ApplicationWithdrawn, ApplicationStateInternal.Unfunded)
					&& this.AgreedParticipants < 1)
					yield return new ValidationResult("The number of participants must be greater than 0.", new[] { nameof(this.EstimatedParticipants) });

				// AgreedParticipants cannot be greater than any eligible cost AgreedMaxParticipants.
				// Text has been updated as par Ian's request - http://tfs.jtst.gov.bc.ca/tfs/Economy/CJG%20-%20Canada%20Job%20Grant/_workitems?_a=edit&id=5145
				if (this.EligibleCosts.Any(ec => ec.AgreedMaxParticipants > this.AgreedParticipants))
					yield return new ValidationResult($"The number of participants for an eligible cost cannot be greater than the application maximum number of participants, which was {this.AgreedParticipants}.", new[] { nameof(this.AgreedParticipants) });

				var ids = this.EligibleCosts.Select(x => x.Id).ToList();
				var eligibleCosts = context.Set<EligibleCost>().Include(m => m.EligibleExpenseType).Include(m => m.EligibleExpenseType.ServiceCategory).Where(ec => ids.Any(y => y == ec.Id)).ToArray();

				// Cannot exceed maximum participant costs employment services and supports components.
				var totalEstimatedESSAverageParticipantCost = TrainingCostExtensions.CalculatePerParticipantReimbursement(this.EligibleCosts.Where(t => t.EligibleExpenseType.ServiceCategory?.ServiceTypeId == ServiceTypes.EmploymentServicesAndSupports).Sum(t => t.EstimatedReimbursement), this.EstimatedParticipants);
				var totalAgreedESSAverageParticipantCost = TrainingCostExtensions.CalculatePerParticipantReimbursement(this.EligibleCosts.Where(t => t.EligibleExpenseType.ServiceCategory?.ServiceTypeId == ServiceTypes.EmploymentServicesAndSupports).Sum(t => t.AgreedMaxReimbursement), this.AgreedParticipants);
				var maxESSParticipantCost = this.GrantApplication.GrantOpening.GrantStream.ProgramConfiguration.ESSMaxEstimatedParticipantCost;
				var essServiceCategories = string.Join(" and ", context.Set<ServiceCategory>().Where(t => t.ServiceTypeId == ServiceTypes.EmploymentServicesAndSupports && t.IsActive).Select(t => t.Caption).ToArray());

				if (totalEstimatedESSAverageParticipantCost > maxESSParticipantCost)
					yield return new ValidationResult($"The total average cost per participant for {essServiceCategories} combined may not exceed {maxESSParticipantCost.ToString("c2")}.", new[] { nameof(this.TotalEstimatedReimbursement) });
				else if (totalAgreedESSAverageParticipantCost > maxESSParticipantCost)
					yield return new ValidationResult($"The total average cost per participant for {essServiceCategories} combined may not exceed {maxESSParticipantCost.ToString("c2")}.", new[] { nameof(this.AgreedCommitment) });

				// Cannot exceed maximum participant costs for skills training components.
				var totalEstimatedSkillsTrainingAverageParticipantCost = TrainingCostExtensions.CalculatePerParticipantReimbursement(this.EligibleCosts.Where(t => t.EligibleExpenseType.ServiceCategory?.ServiceTypeId == ServiceTypes.SkillsTraining).Sum(t => t.EstimatedReimbursement), this.EstimatedParticipants);
				var totalAgreedSkillsTrainingAverageParticipantCost = TrainingCostExtensions.CalculatePerParticipantReimbursement(this.EligibleCosts.Where(t => t.EligibleExpenseType.ServiceCategory?.ServiceTypeId == ServiceTypes.SkillsTraining).Sum(t => t.AgreedMaxReimbursement), this.AgreedParticipants);
				var maxSkillsTrainingParticipantCost = this.GrantApplication.GrantOpening.GrantStream.ProgramConfiguration.SkillsTrainingMaxEstimatedParticipantCosts;
				if (totalEstimatedSkillsTrainingAverageParticipantCost > maxSkillsTrainingParticipantCost)
					yield return new ValidationResult($"The total average cost per participant for skills training components may not exceed {maxSkillsTrainingParticipantCost.ToString("c2")}.", new[] { nameof(this.TotalEstimatedReimbursement) });
				else if (totalAgreedSkillsTrainingAverageParticipantCost > maxSkillsTrainingParticipantCost)
					yield return new ValidationResult($"The total average cost per participant for skills training components may not exceed {maxSkillsTrainingParticipantCost.ToString("c2")}.", new[] { nameof(this.AgreedCommitment) });

				var maxReimbursementAmount = this.GrantApplication.MaxReimbursementAmt;
				// TotalEstimatedReimbursement must less than or equal to agreed MaxReimbursementAmount.
				// Validate only if before and during application submit OR if the estimated values are updated.
				if (this.GrantApplication.GetProgramType() != ProgramTypes.EmployerGrant && this.EstimatedParticipants > 0 && totalEstimatedReimbursement > maxReimbursementAmount * this.EstimatedParticipants)
					yield return new ValidationResult($"The total requested government contribution exceeds the per participant fiscal year maximum reimbursement allowed of '${maxReimbursementAmount:#,##0.00}'.", new[] { nameof(this.TotalEstimatedReimbursement) });

				if (this.GrantApplication.GetProgramType() == ProgramTypes.EmployerGrant && this.EstimatedParticipants > 0 && this.TotalEstimatedReimbursement > maxReimbursementAmount * this.EstimatedParticipants)
					yield return new ValidationResult($"The total requested government contribution exceeds the per participant fiscal year maximum reimbursement allowed of '${maxReimbursementAmount:#,##0.00}'.", new[] { nameof(this.TotalEstimatedReimbursement) });

				// Cannot exceed grant opening budget.
				if (this.GrantApplication.ApplicationStateInternal != ApplicationStateInternal.New && this.AgreedCommitment > this.GrantApplication.GrantOpening.BudgetAllocationAmt)
					yield return new ValidationResult($"The total agreed commitment may not exceed the grant opening budget allocation of {this.GrantApplication.GrantOpening.BudgetAllocationAmt.ToString("c2")}.", new[] { nameof(this.AgreedCommitment) });

				// Ignore the following validation conditions when returning a denied application to assessment.
				var originalState = (ApplicationStateInternal)context.OriginalValue(this.GrantApplication, nameof(GrantApplication.ApplicationStateInternal));
				if (originalState != ApplicationStateInternal.ApplicationDenied && this.GrantApplication.ApplicationStateInternal != ApplicationStateInternal.ReturnedToAssessment)
				{
					// Need to force load any of the related entities.
					context.Set<GrantOpening>().Include(m => m.GrantStream).Include(m => m.GrantStream.ProgramConfiguration).Include(m => m.GrantStream.ProgramConfiguration.EligibleExpenseTypes).FirstOrDefault(g => g.Id == this.GrantApplication.GrantOpeningId);
					var eligibleExpenseTypeIds = this.GrantApplication.GrantOpening.GrantStream.ProgramConfiguration.EligibleExpenseTypes.Select(eet => eet.Id).ToArray();

					foreach (var eligibleCost in this.EligibleCosts)
					{
						// Validate whether the selected expense types are valid for the specified grant opening.
						if (!this.GrantApplication.GrantOpening.GrantStream.ProgramConfiguration.EligibleExpenseTypes.Any(eet => eet.Id == eligibleCost.EligibleExpenseTypeId))
							yield return new ValidationResult($"The expense type '{eligibleCost.EligibleExpenseType.Caption}' is not valid for the selected grant opening.", new[] { nameof(this.EligibleCosts) });

						if (eligibleCost.EligibleExpenseType.ServiceCategory?.ServiceTypeId == ServiceTypes.SkillsTraining)
						{
							// There must be at least # eligible skills training component.
							if (!this.GrantApplication.ApplicationStateInternal.In(ApplicationStateInternal.Draft, ApplicationStateInternal.New, ApplicationStateInternal.ApplicationDenied, ApplicationStateInternal.Unfunded, ApplicationStateInternal.ApplicationWithdrawn))
							{
								if (eligibleCost.Breakdowns.Count(b => b.IsEligible) < eligibleCost.EligibleExpenseType?.ServiceCategory.MinPrograms)
								{
									yield return new ValidationResult($"{eligibleCost.EligibleExpenseType.Caption} expenses require at least {eligibleCost.EligibleExpenseType?.ServiceCategory.MinPrograms} components to be eligible.", new[] { nameof(EligibleCostBreakdown.IsEligible) });
								}
								else if (eligibleCost.Breakdowns.Count(b => b.IsEligible && b.AssessedCost == 0) > 0)
								{
									yield return new ValidationResult($"Eligible Skill Training require cost larger than 0.", new[] { nameof(EligibleCostBreakdown.IsEligible) });
								}
							}
						}
					}

				}
			}

			foreach (var validation in base.Validate(validationContext))
			{
				yield return validation;
			}
		}

		public void Clone(TrainingCost tc)
		{
			TrainingCostState = tc.TrainingCostState;
			EstimatedParticipants = tc.EstimatedParticipants;
			TotalEstimatedCost = tc.TotalEstimatedCost;
			TotalEstimatedReimbursement = tc.TotalEstimatedReimbursement;
			AgreedParticipants = tc.AgreedParticipants;
			TotalAgreedMaxCost = tc.TotalAgreedMaxCost;
			AgreedCommitment = tc.AgreedCommitment;
		}
		#endregion
	}
}

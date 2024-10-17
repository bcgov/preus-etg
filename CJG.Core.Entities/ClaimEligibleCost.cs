using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using DataAnnotationsExtensions;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="ClaimEligibleCost"/> class, provides the ORM a way to manage claim eligible costs.
	/// </summary>
	public class ClaimEligibleCost : EntityBase
	{
		/// <summary>
		/// get/set - Primary key uses IDENTITY
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column(Order = 0)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - Foreign key to the Claim.
		/// </summary>
		[ForeignKey(nameof(Claim)), Column(Order = 1)]
		public int ClaimId { get; set; }

		/// <summary>
		/// get/set - Foreign key to the Claim version.
		/// </summary>
		[ForeignKey(nameof(Claim)), Column(Order = 2)]
		public int ClaimVersion { get; set; }

		/// <summary>
		/// get/set - The claim this eligible cost is associated with.
		/// </summary>
		public virtual Claim Claim { get; set; }

		/// <summary>
		/// get/set - The foreign key to the eligible expense type.
		/// </summary>
		public int EligibleExpenseTypeId { get; set; }

		/// <summary>
		/// get/set - The eligible expense type this claimed value is associated with.
		/// </summary>
		[ForeignKey(nameof(EligibleExpenseTypeId))]
		public virtual EligibleExpenseType EligibleExpenseType { get; set; }

		/// <summary>
		/// get/set - The foreign key to the eligible cost.
		/// </summary>
		public int? EligibleCostId { get; set; }

		/// <summary>
		/// get/set - The EligibleCost line item this ClaimEligibleCost line item is associated with.  This is not enforced for the Assessor.
		/// </summary>
		[ForeignKey(nameof(EligibleCostId))]
		public virtual EligibleCost EligibleCost { get; set; }

		/// <summary>
		/// get/set - The foreign key to the ClaimEligibleCost this was copied from.
		/// </summary>
		public int? SourceId { get; set; }

		/// <summary>
		/// get/set - The ClaimEligibleCost this was copied from.
		/// </summary>
		[ForeignKey(nameof(SourceId))]
		public virtual ClaimEligibleCost Source { get; set; }

		/// <summary>
		/// get/set - The cost associated with this expense type.
		/// </summary>
		public decimal ClaimCost { get; set; }

		/// <summary>
		/// get/set - The number of participants for this eligible cost.
		/// </summary>
		[Required, Min(0, ErrorMessage = "The claim participants must be greater than or equal to 0.")]
		public int ClaimParticipants { get; set; }

		/// <summary>
		/// get/set - This is calculated.  The cost divided by the number of participants (ClaimCost / ClaimParticipants).
		/// </summary>
		public decimal ClaimMaxParticipantCost { get; set; }

		/// <summary>
		/// get/set - This is calculated.  The maximum reimbursed amount per participant (ClaimMaxParticipantCost * GrantApplication.ReimbursementRate).
		/// </summary>
		public decimal ClaimMaxParticipantReimbursementCost { get; set; }

		/// <summary>
		/// get/set - This is calculated.  The amount the employer must contributed per participant (ClaimMaxParticipantCost - ClaimMaxParticipantReimbursementCost).
		/// </summary>
		public decimal ClaimParticipantEmployerContribution { get; set; }

		/// <summary>
		/// get/set - This is calculated.  The amount claimed to be reimbursed for this eligible cost (ClaimCost * ReimbursementRate).
		/// </summary>
		public decimal ClaimReimbursementCost { get; set; }

		/// <summary>
		/// get/set - The assessed cost associated with this expense type.
		/// </summary>
		public decimal AssessedCost { get; set; }

		/// <summary>
		/// get/set - The assessed number of participants for this eligible cost.
		/// </summary>
		[Required, Min(0, ErrorMessage = "The assessed participants must be greater than or equal to 0.")]
		public int AssessedParticipants { get; set; }

		/// <summary>
		/// get/set - This is calculated.  The assessed cost divided by the number of assessed participants (AssessedCost / AssessedParticipants).
		/// </summary>
		public decimal AssessedMaxParticipantCost { get; set; }

		/// <summary>
		/// get/set - This is calculated.  The assessed reimbursement amount per participant (AssessedMaxParticipantCost * GrantApplication.ReimbursementRate).
		/// </summary>
		public decimal AssessedMaxParticipantReimbursementCost { get; set; }

		/// <summary>
		/// get/set - This is calculated.  The assessed amount the employer will contribute per participant (AssessedMaxParticipantCost - AssessedMaxParticipantReimbursementCost).
		/// </summary>
		public decimal AssessedParticipantEmployerContribution { get; set; }

		/// <summary>
		/// get/set - This is calculated.  The amount assesed to be reimbursed for this eligible cost (AssessedCost * ReimbursementRate).
		/// </summary>
		public decimal AssessedReimbursementCost { get; set; }

		/// <summary>
		/// get/set - Flag to indicate that the eligible cost was added by the assessor after the application has been submitted
		/// </summary>
		public bool AddedByAssessor { get; set; } = false;

		/// <summary>
		/// get - All of the participant costs associated with this claimed cost.
		/// </summary>
		public virtual ICollection<ParticipantCost> ParticipantCosts { get; set; } = new List<ParticipantCost>();

		/// <summary>
		/// get - All of the breakdowns associated with this claimed cost.
		/// </summary>
		public virtual ICollection<ClaimBreakdownCost> Breakdowns { get; set; } = new List<ClaimBreakdownCost>();

		/// <summary>
		/// Creates new instance of a <typeparamref name="ClaimEligibleCost"/> class.
		/// </summary>
		public ClaimEligibleCost()
		{
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="ClaimEligibleCost"/> class.
		/// </summary>
		/// <param name="claim"></param>
		public ClaimEligibleCost(Claim claim)
		{
			Claim = claim ?? throw new ArgumentNullException(nameof(claim));
			ClaimId = claim.Id;
			ClaimVersion = claim.ClaimVersion;
		}

		/// <summary>
		/// Creates new instance of a <typeparamref name="ClaimEligibleCost"/> class and initializes it with the specified property values.
		/// </summary>
		/// <param name="claim"></param>
		/// <param name="eligibleCost"></param>
		public ClaimEligibleCost(Claim claim, EligibleCost eligibleCost) : this(claim)
		{
			EligibleCost = eligibleCost ?? throw new ArgumentNullException(nameof(eligibleCost));
			EligibleCostId = eligibleCost.Id;
			EligibleExpenseType = eligibleCost.EligibleExpenseType;
			EligibleExpenseTypeId = eligibleCost.EligibleExpenseTypeId;

			foreach (var breakdown in eligibleCost.Breakdowns.Where(b => b.IsEligible))
			{
				var claimBreakdownCost = new ClaimBreakdownCost(breakdown, this);
				Breakdowns.Add(claimBreakdownCost);
			}

			if (eligibleCost.EligibleExpenseType.ExpenseTypeId == ExpenseTypes.ParticipantAssigned)
			{
				List<ParticipantForm> participants;

				if (claim.GrantApplication.RequireAllParticipantsBeforeSubmission)
				{
					//participants must be approved and attended
					participants = claim.GrantApplication.ParticipantForms.Where(pf => !pf.IsExcludedFromClaim && pf.Approved.HasValue && pf.Approved.Value && pf.Attended.HasValue && pf.Attended.Value).ToList();
				}
				else
				{
					participants = claim.GrantApplication.ParticipantForms.Where(pf => !pf.IsExcludedFromClaim).ToList();
				}

				foreach (var participant in participants)
				{
					var participantCost = new ParticipantCost(this, participant);
					ParticipantCosts.Add(participantCost);
				}
			}
			else if (eligibleCost.EligibleExpenseType.ExpenseTypeId == ExpenseTypes.NotParticipantLimited)
			{
				ClaimParticipants = claim.GrantApplication.ParticipantForms.Count(pf => !pf.IsExcludedFromClaim);
			}
		}

		/// <summary>
		/// Creates new instance of a <typeparamref name="ClaimEligibleCost"/> class and initializes it with the specified property values.
		/// </summary>
		/// <param name="claim"></param>
		/// <param name="eligibleCost"></param>
		public ClaimEligibleCost(Claim claim, ClaimEligibleCost eligibleCost) : this(claim)
		{
			if (eligibleCost == null)
				throw new ArgumentNullException(nameof(eligibleCost));

			ClaimCost = eligibleCost.AssessedCost;
			ClaimMaxParticipantCost = eligibleCost.AssessedMaxParticipantCost;
			ClaimMaxParticipantReimbursementCost = eligibleCost.AssessedMaxParticipantReimbursementCost;
			ClaimParticipantEmployerContribution = eligibleCost.AssessedParticipantEmployerContribution;
			ClaimParticipants = eligibleCost.AssessedParticipants;

			// We copy the assessed values because they are the new agreement limits for the line item.
			AssessedCost = eligibleCost.AssessedCost;
			AssessedMaxParticipantCost = eligibleCost.AssessedMaxParticipantCost;
			AssessedMaxParticipantReimbursementCost = eligibleCost.AssessedMaxParticipantReimbursementCost;
			AssessedParticipantEmployerContribution = eligibleCost.AssessedParticipantEmployerContribution;
			AssessedParticipants = eligibleCost.AssessedParticipants;

			EligibleCostId = eligibleCost.EligibleCostId;
			EligibleCost = eligibleCost.EligibleCost;
			EligibleExpenseType = eligibleCost.EligibleExpenseType;
			EligibleExpenseTypeId = eligibleCost.EligibleExpenseTypeId;

			// Capture the original source this line item was copied from.
			SourceId = eligibleCost.Id;
			Source = eligibleCost;

			foreach (var participantCost in eligibleCost.ParticipantCosts)
			{
				ParticipantCosts.Add(new ParticipantCost(this, participantCost));
			}
		}

		/// <summary>
		/// Validates the claim eligible cost before updating the datasource.
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

			// Refresh EF cache.
			context.Set<Claim>().Include(m => m.EligibleCosts).Where(x => x.GrantApplicationId == Claim.GrantApplicationId).ToList();
			context.Set<ParticipantCost>().Where(pc => pc.ClaimEligibleCostId == Id);
			var eligibleCost = EligibleCost ?? context.Set<EligibleCost>().SingleOrDefault(ec => ec.Id == EligibleCostId);
			//var eligibleCostExpenseType = context.Set<EligibleExpenseType>().SingleOrDefault(eet => eet.Id == this.EligibleExpenseTypeId);

			// Must be associated to a Claim.
			//if (this.Claim == null && this.ClaimId == 0)
			//    yield return new ValidationResult("The claim eligible cost must be associated with a claim.", new[] { nameof(this.Claim) });

			// Must have a EligibleExpenseType.
			if (EligibleExpenseType == null && EligibleExpenseTypeId == 0)
				yield return new ValidationResult("The claim eligible cost must be associated with a eligible expense type.", new[] { nameof(EligibleExpenseType) });

			// ClaimEligibleCost must be associated with an eligible expense if they are the first claim version.
			// Amended Claims can have ClaimEligibleCosts that were added by the Assessor in the previous version.
			if (eligibleCost == null && Source == null && !AddedByAssessor && Claim.ClaimVersion == 1)
				yield return new ValidationResult("The claim eligible cost must be associated with an eligible expense.", new[] { nameof(EligibleCost) });

			// Maximum values are either from the prior claims assessed, or the agreed limits, or the assesed values for the new line item.
			var rate = Claim.GrantApplication.ReimbursementRate;
			var maxClaimParticipants = this.MaxClaimParticipants();
			var maxClaimCost = EligibleCost?.AgreedMaxCost ?? Source?.AssessedCost ?? AssessedCost;
			var maxParticipantCost = EligibleCost?.AgreedMaxParticipantCost ?? Source?.AssessedMaxParticipantCost ?? AssessedMaxParticipantCost;
			var validClaimParticipantCost = this.CalculateClaimParticipantCost();
			var validClaimParticipantReimbursement = this.CalculateClaimMaxParticipantReimbursement();

			if (validClaimParticipantCost > maxParticipantCost)
			{
				validClaimParticipantCost = maxParticipantCost;
				validClaimParticipantReimbursement = Math.Truncate(validClaimParticipantCost * (decimal)rate * 100) / 100;
			}

			var trainingCost = Claim.GrantApplication.TrainingCost ?? context.Set<TrainingCost>().SingleOrDefault(ec => ec.GrantApplicationId == Claim.GrantApplicationId);
			var maxAssessedParticipants = EligibleCost?.AgreedMaxParticipants ?? Source?.AssessedParticipants ?? trainingCost.AgreedParticipants;
			var maxAssessedCost = EligibleCost?.AgreedMaxCost ?? Source?.AssessedCost ?? Claim.GrantApplication.TrainingCost.TotalAgreedMaxCost;
			var validAssessedParticipantCost = this.CalculateAssessedParticipantCost();
			var maxAssessedParticipantCost = EligibleCost?.AgreedMaxParticipantCost ?? Source?.AssessedMaxParticipantCost ?? validAssessedParticipantCost;
			var maxAssessedParticipantReimbursement = maxAssessedParticipantCost; // Allow for 100%.
			var totalAssessedToDate = this.GetTotalAssessed();
			var remainingToClaimed = maxClaimCost - totalAssessedToDate;
			var remainingToAssessed = maxAssessedCost - totalAssessedToDate;
			var maxGovContribution = this.CalculateClaimReimbursement();
			var remainingGovContribution = TrainingCostExtensions.CalculateRoundedReimbursementAmount(remainingToClaimed, rate);

			if ((validAssessedParticipantCost > maxParticipantCost) && (EligibleExpenseTypeId == (int)ExpenseTypes.ParticipantAssigned))
			{
				validAssessedParticipantCost = maxParticipantCost;
			}

			// Claimed

			// Cannot claim more participants than the agreed maximum amount.
			if (Claim.GrantApplication.GetProgramType() != ProgramTypes.EmployerGrant && ClaimParticipants > maxClaimParticipants)
				yield return new ValidationResult("Number of participants claimed cannot exceed the agreed maximum number of participants.", new[] { nameof(ClaimParticipants) });

			// Ensure the claimed amount does not exceed the remaining amount available.
			if (Claim.GrantApplication.GetProgramType() != ProgramTypes.EmployerGrant)
			{
				if (ClaimCost < 0 && remainingToClaimed - ClaimCost > maxClaimCost)
					yield return new ValidationResult($"The negative new claim Total Cost cannot exceed the amount owing {(maxClaimCost - remainingToClaimed):C} for remaining to be claimed in {EligibleExpenseType.Caption}.", new[] { nameof(ClaimCost) });
				else if (ClaimCost > remainingToClaimed)
					yield return new ValidationResult($"The new claim Total Cost cannot exceed the remaining to be claimed amount of {remainingToClaimed:C} for {EligibleExpenseType.Caption}.", new[] { nameof(ClaimCost) });
			}

			// CJG-736 : This logic is nolonger valid. AgreedMaximumReimbursement is not calculated and enforced at expense level.
			// Cannot exceed reimbursement limit.
			var agreedMaximumReimbursement = EligibleCost?.AgreedMaxReimbursement ?? AssessedReimbursementCost;
			if (Claim.GrantApplication.GetProgramType() != ProgramTypes.EmployerGrant && AssessedReimbursementCost > agreedMaximumReimbursement)
				yield return new ValidationResult($"The assessed government contribution cannot exceed the agreed maximum of {agreedMaximumReimbursement:C}", new[] { nameof(AssessedReimbursementCost) });

			// The sum of the breakdown cannot be greater than the agreed amount.
			var breakdowns = context.Set<ClaimBreakdownCost>().Where(b => b.ClaimEligibleCostId == Id).ToArray();
			var sumOfBreakdown = Breakdowns.Count() == 0 ? breakdowns.Sum(b => b.ClaimCost) : Breakdowns.Sum(b => b.ClaimCost);
			if (sumOfBreakdown > (eligibleCost?.AgreedMaxCost ?? AssessedCost))
				yield return new ValidationResult($"The sum of the breakdown costs cannot exceed the agreed amount {ClaimCost:C}.", new[] { nameof(ClaimCost) });

			if (Claim.GrantApplication.GetProgramType() != ProgramTypes.EmployerGrant && maxGovContribution > remainingGovContribution)
				yield return new ValidationResult($"The Maximum Government Contribution cannot exceed the remaining to be claimed amount for {EligibleExpenseType.Caption}.", new[] { nameof(ClaimCost) });

			if (EligibleExpenseTypeId == (int)ExpenseTypes.ParticipantAssigned)
			{
				// The max participant cost must be (ClaimCost / ClaimParticipants).
				if (Claim.GrantApplication.GetProgramType() != ProgramTypes.EmployerGrant && ClaimMaxParticipantCost != validClaimParticipantCost)
					yield return new ValidationResult($"The claim max participant cost is invalid and should be {validClaimParticipantCost:C}.", new[] { nameof(ClaimMaxParticipantCost) });

				// The max participant reimbursement cost must be (ClaimMaxParticipantCost * GrantApplication.ReimbursementRate).
				if (Claim.GrantApplication.GetProgramType() != ProgramTypes.EmployerGrant && ClaimMaxParticipantReimbursementCost != validClaimParticipantReimbursement)
					yield return new ValidationResult($"The claim max participant reimbursement cost is invalid and should be {validClaimParticipantReimbursement:C}.", new[] { nameof(ClaimMaxParticipantReimbursementCost) });

				if (Claim.GrantApplication.GetProgramType() != ProgramTypes.EmployerGrant && AssessedMaxParticipantReimbursementCost > maxAssessedParticipantReimbursement)
					yield return new ValidationResult("The assessed max participant reimbursement is greater than the agreed max participant reimbursement.", new[] { nameof(AssessedMaxParticipantReimbursementCost) });
			}
			// The total of the reimbursement and employer contribution must be equal to the max participant cost.
			if (Claim.GrantApplication.GetProgramType() != ProgramTypes.EmployerGrant && ClaimMaxParticipantCost != ClaimMaxParticipantReimbursementCost + ClaimParticipantEmployerContribution)
				yield return new ValidationResult("The claim max participant cost must equal the reimbursement + employer contribution.", new[] { nameof(ClaimMaxParticipantReimbursementCost) });

			// Each participant cost must be less than or equal to the maximum amounts.
			if (Claim.GrantApplication.GetProgramType() != ProgramTypes.EmployerGrant && ParticipantCosts.Any(pc => pc.ClaimParticipantCost > validClaimParticipantCost || pc.ClaimReimbursement > validClaimParticipantReimbursement))
			{
				var eligibleExpenseType = context.Set<EligibleExpenseType>().FirstOrDefault(eet => eet.Id == EligibleExpenseTypeId);
				yield return new ValidationResult($"You may not exceed the maximum cost or reimbursement per participant for the expense type '{eligibleExpenseType?.Caption}'.", new[] { nameof(ClaimMaxParticipantCost) });
			}

			if (Claim.GrantApplication.GetProgramType() == ProgramTypes.EmployerGrant)
			{
				/// ISSUE: CJG-1590 experiencing issues here
				if (ParticipantCosts.Sum(pc => pc.ClaimParticipantCost) > ClaimCost)
				{
					var eligibleExpenseType = context.Set<EligibleExpenseType>().FirstOrDefault(eet => eet.Id == EligibleExpenseTypeId);
					yield return new ValidationResult($"The sum of all participant costs exceeds the Paid Amount for the expense type '{eligibleExpenseType?.Caption}'.", new[] { nameof(ClaimCost) });
				}
				if (ParticipantCosts.Where(pc => pc.ClaimParticipantCost > 0).Count() > this.MaxClaimParticipants()
					|| ParticipantCosts.Where(pc => pc.AssessedParticipantCost > 0).Count() > this.MaxClaimParticipants())
				{
					var eligibleExpenseType = context.Set<EligibleExpenseType>().FirstOrDefault(eet => eet.Id == EligibleExpenseTypeId);
					yield return new ValidationResult($"Number of participants with assigned cost exceed Agreement limit for the expense type '{eligibleExpenseType?.Caption}'.", new[] { nameof(ClaimParticipants) });
				}
			}
			// Assessed

			// Cannot approve more participants than the agreed maximum amount.
			if (AssessedParticipants > maxAssessedParticipants)
				yield return new ValidationResult("Number of Participants cannot exceed Agreement Number of Participants.", new[] { nameof(ClaimParticipants) });

			// Cannot approve more cost than the agreed maximum.
			if (AssessedCost < 0)
			{
				if (remainingToAssessed - AssessedCost > maxAssessedCost)
					yield return new ValidationResult($"The negative assessed Total Cost cannot exceed the amount owing {(maxAssessedCost - remainingToAssessed):C} for remaining to be claimed in {EligibleExpenseType.Caption}.", new[] { nameof(AssessedMaxParticipantCost) });
			}
			else
			{
				//CJG-736: Total Cost can be a larger number for an ETG claim, this logic is no longer applied.
				if (Claim.GrantApplication.GetProgramType() != ProgramTypes.EmployerGrant && AssessedCost > remainingToAssessed)
					yield return new ValidationResult($"The assessed Total Cost cannot exceed the remaining to be claimed amount of {remainingToAssessed:C} for {EligibleExpenseType.Caption}.", new[] { nameof(AssessedMaxParticipantCost) });
			}

			// The max participant cost must be (AssessedCost / AssessedParticipants).
			if (Claim.GrantApplication.GetProgramType() != ProgramTypes.EmployerGrant && AssessedMaxParticipantCost != validAssessedParticipantCost)
				yield return new ValidationResult($"The assessed max participant cost is invalid and should be {validAssessedParticipantCost:C}.", new[] { nameof(AssessedMaxParticipantCost) });

			if (Claim.GrantApplication.GetProgramType() != ProgramTypes.EmployerGrant && (AssessedMaxParticipantCost > 0 && AssessedMaxParticipantReimbursementCost > AssessedMaxParticipantCost) || (AssessedMaxParticipantCost < 0 && AssessedMaxParticipantReimbursementCost < AssessedMaxParticipantCost))
				yield return new ValidationResult("The assessed max participant reimbursement must be less than or equal to the assessed max participant cost.", new[] { nameof(AssessedMaxParticipantReimbursementCost) });

			// The AssessedMaxParticipantCost must equal (AssessedMaxParticipantReimbursementCost + AssessedParticipantEmployerContribution).
			if (Claim.GrantApplication.GetProgramType() != ProgramTypes.EmployerGrant && AssessedMaxParticipantCost != AssessedMaxParticipantReimbursementCost + AssessedParticipantEmployerContribution)
				yield return new ValidationResult("The assessed max participant cost does not equal the sum of the assessed reimbursement and employer contribution.", new[] { nameof(AssessedMaxParticipantReimbursementCost) });

			// Each participant cost must be less than or equal to the maximum amounts.
			if (Claim.GrantApplication.GetProgramType() != ProgramTypes.EmployerGrant && ParticipantCosts.Any(pc => pc.AssessedParticipantCost > maxAssessedParticipantCost || pc.AssessedReimbursement > maxAssessedParticipantReimbursement || pc.AssessedReimbursement > AssessedMaxParticipantReimbursementCost))
			{
				var eligibleExpenseType = context.Set<EligibleExpenseType>().FirstOrDefault(eet => eet.Id == EligibleExpenseTypeId);
				yield return new ValidationResult($"You may not exceed the maximum cost or reimbursement per participant for the expense type '{eligibleExpenseType?.Caption}'.", new[] { nameof(AssessedMaxParticipantCost) });
			}

			if (Claim.ClaimTypeId == ClaimTypes.SingleAmendableClaim)
			{
				if (AssessedCost < 0)
					yield return new ValidationResult("The assessed cost must be greater than or equal to 0.", new[] { nameof(AssessedCost) });

				if (AssessedMaxParticipantCost < 0)
					yield return new ValidationResult("The assessed max participant cost must be greater than or equal to 0.", new[] { nameof(AssessedMaxParticipantCost) });

				if (AssessedMaxParticipantReimbursementCost < 0)
					yield return new ValidationResult("The assessed max participant reimbursement must be greater than or equal to 0.", new[] { nameof(AssessedMaxParticipantReimbursementCost) });

				if (ClaimCost < 0)
					yield return new ValidationResult("The claim cost must be greater than or equal to 0.", new[] { nameof(ClaimCost) });

				if (ClaimMaxParticipantCost < 0)
					yield return new ValidationResult("The claim maximum participant cost must be greater than or equal to 0.", new[] { nameof(ClaimMaxParticipantCost) });

				if (ClaimMaxParticipantReimbursementCost < 0)
					yield return new ValidationResult("The claim maximum reimbursement cost must be greater than or equal to 0.", new[] { nameof(ClaimMaxParticipantReimbursementCost) });
			}

			// There must only ever be one unique participant cost for each eligible cost.  A specific participant can't have multiple costs per expense type.
			var hasMultipleParticipantLineItems = (
				from pc in ParticipantCosts
				group pc by pc.ParticipantFormId into g
				let count = g.Count()
				where count > 1
				select count).Any();

			if (hasMultipleParticipantLineItems)
				yield return new ValidationResult("A single claim eligible cost must not have more than one participant cost associated to a single participant.", new[] { nameof(ParticipantCosts) });

			if (Claim.GrantApplication.GetProgramType() == ProgramTypes.EmployerGrant)
			{
				/// ISSUE: CJG-1590 experiencing issues here
				if (ParticipantCosts.Sum(pc => pc.AssessedParticipantCost) > AssessedCost)
				{
					var eligibleExpenseType = context.Set<EligibleExpenseType>().FirstOrDefault(eet => eet.Id == EligibleExpenseTypeId);
					yield return new ValidationResult($"The sum of all participant costs exceeds the Assessed Amount for the expense type '{eligibleExpenseType?.Caption}'.", new[] { nameof(AssessedCost) });
				}
			}

			foreach (var validation in base.Validate(validationContext))
			{
				yield return validation;
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using CJG.Core.Entities.Attributes;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="Claim"/> class, provides the ORM a way to manage claims.
	/// </summary>
	public class Claim : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - The primary key identity.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None), Column(Order = 0)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - The version of the claim.
		/// </summary>
		[Key, Column(Order = 1)]
		public int ClaimVersion { get; set; }

		/// <summary>
		/// get/set - The foreign key to the internal user who is the current assessor.
		/// </summary>
		[Index("IX_Claim", 1)]
		public int? AssessorId { get; set; }

		/// <summary>
		/// get/set - The internal user who is the current assessor.
		/// </summary>
		[ForeignKey(nameof(AssessorId))]
		public virtual InternalUser Assessor { get; set; }

		/// <summary>
		/// get/set - The foreign key to the parent grant application.
		/// </summary>
		public int GrantApplicationId { get; set; }

		/// <summary>
		/// get/set - The grant application this claim belongs to.
		/// </summary>
		[ForeignKey(nameof(GrantApplicationId))]
		public virtual GrantApplication GrantApplication { get; set; }

		/// <summary>
		/// get/set - The foreign key to the claim type.
		/// </summary>
		public ClaimTypes ClaimTypeId { get; set; }

		/// <summary>
		/// get/set - The claim type.
		/// </summary>
		[ForeignKey(nameof(ClaimTypeId))]
		public ClaimType ClaimType { get; set; }

		/// <summary>
		/// get/set - The file number associated with the grant application.
		/// </summary>
		[Required, MaxLength(50), Index("IX_Claim", 3)]
		public string ClaimNumber { get; set; }

		/// <summary>
		/// get/set - The current state of the claim.
		/// </summary>
		[Required, DefaultValue(ClaimState.Incomplete), Index("IX_Claim", 2)]
		public ClaimState ClaimState { get; set; } = ClaimState.Incomplete;

		/// <summary>
		/// get/set - This is the calculated actual total of all ClaimEligibleCosts.
		/// </summary>
		public decimal TotalClaimReimbursement { get; set; }

		/// <summary>
		/// get/set - This is the calculated actual total of all Assessed ClaimEligibleCosts.
		/// </summary>
		public decimal TotalAssessedReimbursement { get; set; }

		/// <summary>
		/// get/set - The assessor notes that will be shared with the Application Administrator.
		/// </summary>
		[MaxLength(2000)]
		public string ClaimAssessmentNotes { get; set; }

		/// <summary>
		/// get/set - The assessor notes about the eligibility assessment.  For internal use only.
		/// </summary>
		[MaxLength(2000)]
		public string EligibilityAssessmentNotes { get; set; }

		/// <summary>
		/// get/set - The assessor notes about the reimbursement assessment.  For internal use only.
		/// </summary>
		[MaxLength(2000)]
		public string ReimbursementAssessmentNotes { get; set; }

		/// <summary>
		/// get/set - Whether the current claim is the final claim for the related grant application.
		/// </summary>
		[DefaultValue(false)]
		public bool IsFinalClaim { get; set; }

		/// <summary>
		/// get/set - The date the claim was submitted.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Index("IX_Claim", 4)]
		[Column(TypeName = "DATETIME2")]
		public DateTime? DateSubmitted { get; set; }

		/// <summary>
		/// get/set - The date the claim was assessed.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Index("IX_Claim", 5)]
		[Column(TypeName = "DATETIME2")]
		public DateTime? DateAssessed { get; set; }

		/// <summary>
		/// get/set - A collection of the eligible costs associated with this claim.
		/// </summary>
		public virtual ICollection<ClaimEligibleCost> EligibleCosts { get; set; } = new List<ClaimEligibleCost>();

		/// <summary>
		/// get/set - A collection of attachments which are receipts for this claim.
		/// </summary>
		public virtual ICollection<Attachment> Receipts { get; set; } = new List<Attachment>();

		/// <summary>
		/// get/set - A collection of payments for this claim.
		/// </summary>
		public virtual ICollection<ReconciliationPayment> Payments { get; set; } = new List<ReconciliationPayment>();

		/// <summary>
		/// get - The payment requests associated with this claim.
		/// </summary>
		public virtual ICollection<PaymentRequest> PaymentRequests { get; set; } = new List<PaymentRequest>();

		/// <summary>
		/// get - The participantForms associated with this claim.
		/// </summary>
		public virtual ICollection<ParticipantForm> ParticipantForms { get; set; } = new List<ParticipantForm>();

		/// <summary>
		/// get - All of the claim participants associated with this claim.
		/// </summary>
		public virtual ICollection<ClaimParticipant> ClaimParticipants { get; set; } = new List<ClaimParticipant>();

		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="Claim"/> object.
		/// </summary>
		public Claim()
		{

		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="Claim"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="version"></param>
		/// <param name="grantApplication"></param>
		public Claim(int id, int version, GrantApplication grantApplication) : this(id, version, grantApplication, grantApplication?.GrantOpening?.GrantStream?.ProgramConfiguration?.ClaimTypeId ?? ClaimTypes.SingleAmendableClaim)
		{
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="Claim"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="version"></param>
		/// <param name="grantApplication"></param>
		/// <param name="type"></param>
		public Claim(int id, int version, GrantApplication grantApplication, ClaimTypes type)
		{
			if (id <= 0)
				throw new ArgumentException("The claim id must be greater than 0.", nameof(id));

			if (version <= 0)
				throw new ArgumentException("The claim version must be greater than 0.", nameof(version));

			Id = id;
			ClaimVersion = version;
			GrantApplication = grantApplication ?? throw new ArgumentNullException(nameof(grantApplication));
			GrantApplicationId = grantApplication.Id;
			ClaimNumber = type == ClaimTypes.SingleAmendableClaim ? $"{grantApplication.FileNumber}-{version:0#}" : $"{grantApplication.FileNumber}-{grantApplication.Claims.Count() + 1:0#}";
			ClaimState = ClaimState.Incomplete;
			ClaimTypeId = type;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Validate the properties of this claim.
		/// </summary>
		/// <param name="validationContext"></param>
		/// <returns></returns>
		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var entry = validationContext.GetDbEntityEntry();
			var context = validationContext.GetDbContext();

			// This is done to stop errors from being thrown when developers use EF entities in ViewModels.
			if (entry == null || context == null)
				yield break;

			if (GrantApplication == null && GrantApplicationId == 0)
				yield return new ValidationResult("The claim must be associated with a training program.", new[] { nameof(GrantApplication) });

			if (GrantApplication == null) GrantApplication = context.GrantApplications.Find(GrantApplicationId);

			if (entry.State == EntityState.Added)
			{
				var claims = context.Set<Claim>().Where(c => c.Id == Id);

				if (claims.Count() > 0)
				{
					// When there are claims, ensure the version number is correct.
					var max_version = claims?.Max(c => c.ClaimVersion);
					if (max_version + 1 != ClaimVersion)
						yield return new ValidationResult($"The next available claim version is {max_version + 1}, you cannot set it to {ClaimVersion}.", new[] { nameof(ClaimVersion) });
				}
				// The first claim must be version 1.
				else if (ClaimVersion != 1)
					yield return new ValidationResult($"The next available claim version is 1, you cannot set it to {ClaimVersion}.", new[] { nameof(ClaimVersion) });

				// Claims must begin with a state of Incomplete or Complete.
				if (!ClaimState.In(ClaimState.Incomplete, ClaimState.Complete))
					yield return new ValidationResult("A new claim must begin with state Incomplete or Complete", new[] { nameof(ClaimState) });

				// A Claim cannot be created until after the Training Program Start Date.
				if (AppDateTime.UtcNow < GrantApplication.StartDate)
					yield return new ValidationResult("Your claim may be created only on or after the training start date.");
			}
			else if (entry.State == EntityState.Modified)
			{
				// The claim version can never change.
				var original_version = (int)entry.OriginalValues[nameof(ClaimVersion)];
				if (original_version != ClaimVersion)
					yield return new ValidationResult("The claim version cannot be changed.", new[] { nameof(ClaimVersion) });

				// A submitted claim (NewClaim) must have a FileNumber.
				if (ClaimState == ClaimState.Unassessed && String.IsNullOrEmpty(ClaimNumber))
					yield return new ValidationResult("The claim must have a file number before it can be submitted for assessment.", new[] { nameof(ClaimNumber), nameof(ClaimState) });

				// A submitted claim (NewClaim) must have a DateSubmitted.
				var original_state = (ClaimState)entry.OriginalValues[nameof(ClaimState)];
				if (original_state.In(ClaimState.Incomplete, ClaimState.Complete) && ClaimState == ClaimState.Unassessed && (!DateSubmitted.HasValue || DateSubmitted.Value.Date < AppDateTime.UtcNow.Date))
					yield return new ValidationResult("The claim cannot be submitted for assessment without a submitted date, and the date cannot be in the past.", new[] { nameof(DateSubmitted), nameof(ClaimState) });

				// A claim under assessment must have an Assessor.
				if ((ClaimState == ClaimState.ClaimApproved || ClaimState == ClaimState.ClaimDenied) && Assessor == null && AssessorId == 0)
					yield return new ValidationResult("The claim cannot be approved or denied without an assessor.", new[] { nameof(Assessor), nameof(ClaimState) });

				// A claim approved must have a DateAssessed.
				if ((ClaimState == ClaimState.ClaimApproved || ClaimState == ClaimState.ClaimDenied) && (!DateAssessed.HasValue || DateAssessed.Value.Date < AppDateTime.UtcNow.Date))
					yield return new ValidationResult("The claim cannot be approved without an assessed date, and the date cannot be in the past.", new[] { nameof(DateAssessed), nameof(ClaimState) });

				// A denied claim must have an assessed value of $0.
				if (ClaimState == ClaimState.ClaimDenied && TotalAssessedReimbursement != 0)
					yield return new ValidationResult("The claim has been denied and the assessed value must be $0.", new[] { nameof(TotalAssessedReimbursement) });
				// A claim cannot be recommended for approval or approved with $0 assessment.
				else if (GrantApplication.ApplicationStateInternal == ApplicationStateInternal.ClaimApproved && TotalAssessedReimbursement == 0)
					yield return new ValidationResult("The claim cannot have an assessed value of $0.00", new[] { nameof(TotalAssessedReimbursement) });
			}

			// Refresh EF cache.
			context.Set<ClaimEligibleCost>().Where(ec => ec.ClaimId == Id && ec.ClaimVersion == ClaimVersion).Include(ec => ec.ParticipantCosts).ToList(); //NOSONAR
			context.Set<TrainingProgram>().Where(tp => tp.GrantApplicationId == GrantApplicationId).Include(tp => tp.GrantApplication).Include(tp => tp.GrantApplication.TrainingCost).ToList();
			context.Set<GrantApplication>().Where(tp => tp.Id == GrantApplicationId).Include(tp => tp.TrainingCost).ToList();

			var participantsWithCostsAssigned = this.ParticipantsWithEligibleCosts();
			var participantsWithoutCostsAssigned = this.ParticipantsWithoutEligibleCosts(excludeClaimReported: true);
			// At least one participant needs to have costs assigned before allowing to submit.
			// All participants report must have at least one cost associated with them.

			if (ClaimTypeId == ClaimTypes.SingleAmendableClaim && ClaimState == ClaimState.Unassessed && (participantsWithCostsAssigned == 0 || participantsWithoutCostsAssigned > 0))
				yield return new ValidationResult("You have participants reported that you have not assigned expenses to in your claim. " +
					"If a participant has not attended training then they should be removed from your participant list. " +
					"If participant expenses are not assigned correctly then you should edit your claim and correct them before you submit it.", new[] { nameof(EligibleCosts) });

			if (ClaimState == ClaimState.Unassessed)
			{
				var participantsOnClaim = (from pc in EligibleCosts.SelectMany(ec => ec.ParticipantCosts)
										   select pc.ParticipantFormId).ToArray().Distinct().Count();

				if (participantsOnClaim > GrantApplication.TrainingCost.AgreedParticipants)
					yield return new ValidationResult($"The number of participants ({participantsOnClaim}) included in this claim is more than the agreed number of participants ({GrantApplication.TrainingCost.AgreedParticipants}).", new[] { nameof(GrantApplication.TrainingCost.AgreedParticipants) });
			}

			if (ClaimTypeId == ClaimTypes.SingleAmendableClaim)
			{
				if (TotalAssessedReimbursement < 0)
					yield return new ValidationResult("The assessed total claim reimbursement must be greater than or equal to 0.", new[] { nameof(TotalAssessedReimbursement) });

				if (TotalClaimReimbursement < 0)
					yield return new ValidationResult("The total claim reimbursement must be greater than or equal to 0.", new[] { nameof(TotalClaimReimbursement) });
			}

			// If there are eligible costs.
			if (EligibleCosts != null && EligibleCosts.Any())
			{
				var listOfEEPIds = EligibleCosts.Select(x => x.EligibleExpenseTypeId).Distinct().ToList();
				context.Set<EligibleExpenseType>().Where(x => listOfEEPIds.Contains(x.Id)).Include(tp => tp.ExpenseType).ToList();
				var sum_reimbursements = this.CalculateTotalClaimReimbursement();
				var sum_assessed_reimbursements = this.CalculateTotalAssessedReimbursement();

				// The TotalClaimReimbursement must equal the sum of the eligible cost ClaimMaxReimbursementCost.
				if (TotalClaimReimbursement != sum_reimbursements)
					yield return new ValidationResult($"The total claim must be equal to the sum of the participant reimbursements ${sum_reimbursements}.", new[] { nameof(TotalClaimReimbursement) });

				// The TotalAssessedReimbursement must equal to the sum of the eligible cost AssessedMaxReimbursementCost.
				if ((ClaimState != ClaimState.ClaimDenied) && (TotalAssessedReimbursement != sum_assessed_reimbursements))
					yield return new ValidationResult($"The total claim assessed reimbursement must be equal to the sum of the assessed participant reimbursements ${sum_assessed_reimbursements}.", new[] { nameof(TotalAssessedReimbursement) });

				// load TrainingCost before validating it
				if (GrantApplication.TrainingCost == null) GrantApplication.TrainingCost = context.Set<TrainingCost>().SingleOrDefault(x => x.GrantApplicationId == GrantApplication.Id);
				if (sum_reimbursements > GrantApplication.TrainingCost.AgreedCommitment)
					yield return new ValidationResult($"Total claimed reimbursement ${sum_reimbursements} must not exceed the agreement amount ${GrantApplication.TrainingCost.AgreedCommitment}.", new[] { nameof(TotalClaimReimbursement) });

				// You cannot report more participants than agreed.
				if (participantsWithCostsAssigned > GrantApplication.TrainingCost.AgreedParticipants)
					yield return new ValidationResult("You have reported more participants than approved in your grant agreement. " +
						"The number of participants you are reporting (with costs assigned) cannot exceed " +
						"the maximum number of participants approved in your grant agreement.", new[] { nameof(EligibleCosts) });

				// We couldn't fix claims that were approved or denied, which means we can't validate them when a claim is paid.
				if (DateAssessed >= new DateTime(2018, 03, 14))
				{
					foreach (var eligibleCost in EligibleCosts)
					{
						foreach (var result in eligibleCost.Validate(validationContext))
						{
							yield return result;
						}
					}
				}
			}

			if (Receipts.Count > Constants.MaximumNumberOfAttachmentsPerClaim)
				yield return new ValidationResult("Average assessed participant reimbursement cannot exceed the stream participant annual limit.", new[] { nameof(Receipts) });

			// the following check is required only on "Review and Submit" action by external users
			if (ClaimState == ClaimState.Unassessed && EligibleCosts.Sum(item => item.ClaimCost) == 0)
				yield return new ValidationResult("At least one of the claimed costs must have a non-zero amount.", new[] { nameof(EligibleCosts) });

			foreach (var validation in base.Validate(validationContext))
				yield return validation;
		}
		#endregion
	}
}

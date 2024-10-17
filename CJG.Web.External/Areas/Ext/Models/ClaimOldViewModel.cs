using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class ClaimOldViewModel
	{
		#region Properties
		public int Id { get; set; }

		public int ClaimVersion { get; set; }

		public string ClaimNumber { get; set; }

		public int GrantApplicationId { get; set; }

		public ApplicationStateExternal ApplicationStateExternal { get; set; }

		public GrantApplication GrantApplication { get; set; }

		public int TrainingProgramId { get; set; }

		public TrainingProgram TrainingProgram { get; set; }

		public bool ClaimConfirmed { get; set; }

		public DateTime? DateSubmitted { get; set; }

		[DefaultValue(ClaimState.Incomplete)]
		public ClaimState ClaimState { get; set; } = ClaimState.Incomplete;

		public decimal TotalClaimReimbursement { get; set; }

		public decimal TotalAssessedReimbursement { get; set; }

		public int CountParticipantsWithCostsAssigned { get; set; }

		public int CountParticipants { get; set; }

		public int MaximumParticipants { get; set; }

		public string ClaimStateDescription { get; set; }

		public decimal ClaimAmount { get; set; }

		public int? AssessorId { get; set; }

		public DateTime? DateAssessed { get; set; }

		public InternalUser Assessor { get; set; }

		[MaxLength(2000)]
		public string ClaimAssessmentNotes { get; set; }

		[MaxLength(2000)]
		public string EligibilityAssessmentNotes { get; set; }

		[MaxLength(2000)]
		public string ReimbursementAssessmentNotes { get; set; }

		public byte[] RowVersion { get; set; }

		public IList<Claims.ClaimEligibleCostViewModel> EligibleCosts { get; set; } = new List<Claims.ClaimEligibleCostViewModel>();

		public IList<Attachment> Receipts { get; set; } = new List<Attachment>();

		public ViewDataDictionary ViewData { get; set; } = new ViewDataDictionary();

		public SidebarViewModel SidebarViewModel { get; set; }
		#endregion

		#region Constructors
		public ClaimOldViewModel()
		{

		}

		public ClaimOldViewModel(Claim claim)
		{
			if (claim == null)
				throw new ArgumentNullException(nameof(claim));

			this.Id = claim.Id;
			this.ClaimVersion = claim.ClaimVersion;
			this.ClaimNumber = claim.ClaimNumber;
			this.Assessor = claim.Assessor;
			this.AssessorId = claim.AssessorId;
			this.GrantApplicationId = claim.GrantApplicationId;
			this.ApplicationStateExternal = claim.GrantApplication.ApplicationStateExternal;
			this.GrantApplication = claim.GrantApplication;
			this.TrainingProgramId = claim.GrantApplication.TrainingPrograms.FirstOrDefault().Id;
			this.TrainingProgram = claim.GrantApplication.TrainingPrograms.FirstOrDefault();
			this.ClaimState = claim.ClaimState;
			this.TotalClaimReimbursement = claim.TotalClaimReimbursement;
			this.TotalAssessedReimbursement = claim.TotalAssessedReimbursement;
			this.EligibleCosts = claim.EligibleCosts.Select(ec => new Claims.ClaimEligibleCostViewModel(ec)).ToList();
			this.Receipts = claim.Receipts.ToList();
			this.CountParticipantsWithCostsAssigned = claim.ParticipantsWithEligibleCosts();
			this.CountParticipants = claim.GrantApplication.ParticipantForms.Count;
			this.MaximumParticipants = claim.GrantApplication.TrainingCost.AgreedParticipants;
			this.ClaimAssessmentNotes = claim.ClaimAssessmentNotes;
			this.EligibilityAssessmentNotes = claim.EligibilityAssessmentNotes;
			this.ReimbursementAssessmentNotes = claim.ReimbursementAssessmentNotes;
			this.DateSubmitted = claim.DateSubmitted;
			this.ClaimStateDescription = claim.GetClaimStateDescription();
			this.ClaimAmount = claim.IsApproved() ? claim.TotalAssessedReimbursement : claim.TotalClaimReimbursement;
			this.DateAssessed = claim.DateAssessed;
			this.RowVersion = claim.RowVersion;

			ViewData.Add("claimId", Id);
			ViewData.Add("claimVersion", ClaimVersion);
		}
		#endregion

		#region Methods
		public static implicit operator Claim(ClaimOldViewModel model)
		{
			var claim = new Claim
			{
				Id = model.Id,
				ClaimVersion = model.ClaimVersion,
				ClaimNumber = model.ClaimNumber,
				GrantApplicationId = model.TrainingProgramId,
				ClaimState = model.ClaimState,
				RowVersion = model.RowVersion,
				EligibleCosts = model.EligibleCosts.Select(ec => (ClaimEligibleCost)ec).ToList(),
				DateSubmitted = model.DateSubmitted,
				Assessor = model.Assessor,
				AssessorId = model.AssessorId,
				DateAssessed = model.DateAssessed
			};
			return claim;
		}
		#endregion
	}
}
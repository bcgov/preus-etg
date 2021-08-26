using CJG.Core.Entities;
using System;

namespace CJG.Web.External.Areas.Int.Models
{
	public class ClaimWorkflowViewModel
	{
		#region Properties
		public int Id { get; set; }
		public int ClaimVersion { get; set; }
		public string RowVersion { get; set; }
		public string EligibilityAssessmentNotes { get; set; }
		public string ReimbursementAssessmentNotes { get; set; }
		public string ClaimAssessmentNotes { get; set; }
		#endregion

		#region Constructors
		public ClaimWorkflowViewModel()
		{
		}

		public ClaimWorkflowViewModel(Claim claim)
		{
			if (claim == null) throw new ArgumentNullException(nameof(claim));

			this.Id = claim.Id;
			this.ClaimVersion = claim.ClaimVersion;
			this.RowVersion = Convert.ToBase64String(claim.RowVersion);
			this.EligibilityAssessmentNotes = claim.EligibilityAssessmentNotes;
			this.ReimbursementAssessmentNotes = claim.ReimbursementAssessmentNotes;
			this.ClaimAssessmentNotes = claim.ClaimAssessmentNotes;
		}
		#endregion
	}
}
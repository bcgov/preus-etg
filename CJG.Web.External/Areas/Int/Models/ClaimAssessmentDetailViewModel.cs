using CJG.Core.Entities;
using CJG.Web.External.Areas.Ext.Models;
using System;
using System.Collections.Generic;

namespace CJG.Web.External.Areas.Int.Models
{
	public class ClaimAssessmentDetailViewModel : ViewModelBase
	{
		#region Properties
		public int Id { get; set; }

		public GrantApplicationViewModel GrantApplication { get; set; }

		public ClaimOldViewModel Claim { get; set; } = new ClaimOldViewModel();

		public bool IsEditMode { get; set; } = false;

		public IEnumerable<ParticipantForm> ParticipantForms { get; set; }

		public bool ReadOnly { get; set; } = false;

		public bool CanUnlock { get; set; } = false;

		public string ReimbursementRate { get; set; }

		public bool HasPriorApprovedClaim { get; set; } = false;

		public string AllowedFileAttachmentExtensions { get; set; }
		#endregion

		#region Constructors
		public ClaimAssessmentDetailViewModel(GrantApplication grantApplication, ClaimOldViewModel claim, string allowedFileAttachmentExtensions)
		{
			if (grantApplication == null)
			{
				throw new ArgumentNullException(nameof(grantApplication));
			}

			this.GrantApplication = new GrantApplicationViewModel(grantApplication);

			this.Claim = claim;
			this.IsEditMode = true;
			this.AllowedFileAttachmentExtensions = allowedFileAttachmentExtensions;
		}
		#endregion
	}
}
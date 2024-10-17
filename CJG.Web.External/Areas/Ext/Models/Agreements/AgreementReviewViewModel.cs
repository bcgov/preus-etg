using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace CJG.Web.External.Areas.Ext.Models.Agreements
{
	public class AgreementReviewViewModel : BaseViewModel
	{
		#region Properties
		public int GrantApplicationId { get; set; }
		public string RowVersion { get; set; }
		public string DirectorNotes { get; set; }
		public bool CoverLetterConfirmed { get; set; }
		public bool ScheduleAConfirmed { get; set; }
		public bool ScheduleBConfirmed { get; set; }
		public ApplicationStateInternal ApplicationStateInternal { get; set; }
		public ApplicationStateExternal ApplicationStateExternal { get; set; }
		public ProgramTitleLabelViewModel GrantAgreementApplicationViewModel { get; set; }

		[Required(ErrorMessage = "A reason is required")]
		public string IncompleteReason { get; set; }
		#endregion

		#region Constructors
		public AgreementReviewViewModel() { }

		public AgreementReviewViewModel(GrantApplication grantApplication)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));

			this.GrantApplicationId = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.ApplicationStateInternal = grantApplication.ApplicationStateInternal;
			this.ApplicationStateExternal = grantApplication.ApplicationStateExternal;
			this.CoverLetterConfirmed = grantApplication.GrantAgreement.CoverLetterConfirmed;
			this.ScheduleAConfirmed = grantApplication.GrantAgreement.ScheduleAConfirmed;
			this.ScheduleBConfirmed = grantApplication.GrantAgreement.ScheduleBConfirmed;

			this.DirectorNotes = grantApplication.GrantAgreement?.DirectorNotes;
			this.GrantAgreementApplicationViewModel = new ProgramTitleLabelViewModel(grantApplication);
		}
		#endregion
	}
}
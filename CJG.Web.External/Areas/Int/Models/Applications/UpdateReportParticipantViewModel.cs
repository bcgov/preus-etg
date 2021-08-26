using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;

namespace CJG.Web.External.Areas.Int.Models.Applications
{
	public class UpdateReportParticipantViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }
		public bool CanApplicantReportParticipants { get; set; }
		#endregion

		#region Constructors
		public UpdateReportParticipantViewModel() { }

		public UpdateReportParticipantViewModel(GrantApplication grantApplication)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));

			this.Id = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.CanApplicantReportParticipants = grantApplication.CanApplicantReportParticipants;
		}
		#endregion
	}
}
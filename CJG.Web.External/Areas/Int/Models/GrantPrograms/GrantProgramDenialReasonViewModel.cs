using CJG.Core.Entities;
using CJG.Web.External.Areas.Int.Models.Notifications;
using CJG.Web.External.Models.Shared;
using System;

namespace CJG.Web.External.Areas.Int.Models.GrantPrograms
{
	public class GrantProgramDenialReasonViewModel : BaseViewModel
	{
		#region Properties
		public int GrantProgramId { get; set; }
		public string Caption { get; set; }
		public bool IsActive { get; set; }
		#endregion

		#region Constructors
		public GrantProgramDenialReasonViewModel() { }

		public GrantProgramDenialReasonViewModel(DenialReason denialReason)
		{
			if (denialReason == null) throw new ArgumentNullException(nameof(denialReason));

			this.GrantProgramId = denialReason.GrantProgramId;
			this.Caption = denialReason.Caption;
			this.IsActive = denialReason.IsActive;
			this.Id = denialReason.Id;
		}
		#endregion
	}
}
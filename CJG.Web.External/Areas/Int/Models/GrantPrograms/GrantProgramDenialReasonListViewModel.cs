using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models.GrantPrograms
{
	public class GrantProgramDenialReasonListViewModel : BaseViewModel
	{
		#region Properties
		public IEnumerable<GrantProgramDenialReasonViewModel> GrantProgramDenialReasons { get; set; }
		#endregion Properties

		#region Constructors
		public GrantProgramDenialReasonListViewModel() { }

		public GrantProgramDenialReasonListViewModel(IEnumerable<DenialReason> denialReasons, int grantProgramId)
		{
			if (denialReasons == null) throw new ArgumentNullException(nameof(denialReasons));

			this.GrantProgramDenialReasons = denialReasons.Select(r => new GrantProgramDenialReasonViewModel(r)).ToArray();
			this.Id = grantProgramId;
		}
		#endregion
	}
}
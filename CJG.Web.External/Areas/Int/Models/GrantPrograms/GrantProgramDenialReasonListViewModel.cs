using System;
using System.Collections.Generic;
using System.Linq;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.GrantPrograms
{
	public class GrantProgramDenialReasonListViewModel : BaseViewModel
	{
		public IEnumerable<GrantProgramDenialReasonViewModel> GrantProgramDenialReasons { get; set; }

		public GrantProgramDenialReasonListViewModel() { }

		public GrantProgramDenialReasonListViewModel(IEnumerable<DenialReason> denialReasons, int grantProgramId)
		{
			if (denialReasons == null)
				throw new ArgumentNullException(nameof(denialReasons));

			Id = grantProgramId;
			GrantProgramDenialReasons = denialReasons
				.OrderBy(dr => dr.RowSequence)
				.Select(r => new GrantProgramDenialReasonViewModel(r))
				.ToArray();
		}
	}
}
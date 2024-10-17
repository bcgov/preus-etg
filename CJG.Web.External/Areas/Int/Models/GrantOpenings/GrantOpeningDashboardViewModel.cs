using CJG.Core.Entities;
using CJG.Web.External.Helpers;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;

namespace CJG.Web.External.Areas.Int.Models.GrantOpenings
{
	public class GrantOpeningDashboardViewModel : BaseViewModel
	{
		public int? SelectedFiscalYearId { get; set; }
		public int? SelectedGrantProgramId { get; set; }
	}
}

using System.Collections.Generic;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.Prioritization
{
	public class PrioritizationScoresViewModel : BaseViewModel
	{
		public IEnumerable<ScoreViewModel> Regions { get; set; } = new List<ScoreViewModel>();
		public IEnumerable<ScoreViewModel> Industries { get; set; } = new List<ScoreViewModel>();
	}

	public class ScoreViewModel
	{
		public string Name { get; set; }
		public decimal Score { get; set; }
		public bool IsPriority { get; set; }
		public string Code { get; set; }
		public int PostalCodeCount { get; set; }
	}
}
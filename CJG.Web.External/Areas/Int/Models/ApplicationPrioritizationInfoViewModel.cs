using System;
using System.Collections.Generic;
using System.Linq;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models
{
	public class ApplicationPrioritizationInfoViewModel : BaseViewModel
	{
		public string RowVersion { get; set; }

		public string IndustryName { get; set; }
		public string IndustryCode { get; set; }
		public int IndustryScore { get; set; }

		public string RegionalName { get; set; }
		public int RegionalScore { get; set; }

		public int SmallBusinessScore { get; set; }
		public int FirstTimeApplicantScore { get; set; }

		public List<Tuple<string, int>> QuestionScores { get; set; } = new List<Tuple<string, int>>();

		public int TotalScore { get; set; }

		public ApplicationPrioritizationInfoViewModel()
		{
		}

		public ApplicationPrioritizationInfoViewModel(GrantApplication grantApplication)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			Id = grantApplication.Id;
			RowVersion = Convert.ToBase64String(grantApplication.RowVersion);

			var breakdown = grantApplication.PrioritizationScoreBreakdown ?? new PrioritizationScoreBreakdown();

			IndustryName = breakdown.IndustryName;
			IndustryCode = breakdown.IndustryCode;
			IndustryScore = breakdown.IndustryScore;

			RegionalName = breakdown.RegionalName;
			RegionalScore = breakdown.RegionalScore;

			SmallBusinessScore = breakdown.SmallBusinessScore;
			FirstTimeApplicantScore = breakdown.FirstTimeApplicantScore;

			QuestionScores = breakdown.EligibilityAnswerScores
				.Where(a => a.QuestionScore > 0)
				.Select(q => new Tuple<string, int>(q.QuestionedAnswered.EligibilityQuestion, q.QuestionScore))
				.ToList();

			TotalScore = breakdown.GetTotalScore();
		}
	}
}
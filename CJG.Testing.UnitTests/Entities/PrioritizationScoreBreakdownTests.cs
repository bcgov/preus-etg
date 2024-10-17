using System.Collections.Generic;
using CJG.Core.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CJG.Testing.UnitTests.Entities
{
	[TestClass]
	public class PrioritizationScoreBreakdownTests
	{
		private PrioritizationScoreBreakdown _breakdown;

		[TestInitialize]
		public void Setup()
		{
			_breakdown = new PrioritizationScoreBreakdown
			{
				FirstTimeApplicantScore = 1,
				IndustryScore = 10,
				HighOpportunityOccupationScore = 100,
				RegionalScore = 1000,
				SmallBusinessScore = 10000,
				EligibilityAnswerScores = new List<PrioritizationScoreBreakdownAnswer>
				{
					new PrioritizationScoreBreakdownAnswer
					{
						QuestionScore = 100000
					}
				}
			};
		}

		[TestMethod, TestCategory("PrioritizationScoreBreakdown"), TestCategory("Methods")]
		public void GetTotalScore_Sums_Everything()
		{
			Assert.AreEqual(111111, _breakdown.GetTotalScore());
		}
	}
}
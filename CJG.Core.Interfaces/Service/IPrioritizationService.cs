using System.Collections.Generic;
using System.IO;
using CJG.Core.Entities;

namespace CJG.Core.Interfaces.Service
{
	public interface IPrioritizationService : IService
	{
		PrioritizationThreshold GetThresholds();
		void UpdateThresholds(PrioritizationThreshold threshold);
		void SetRegionException(GrantApplication grantApplication, int prioritizationRegionId);

		PrioritizationScoreBreakdown GetBreakdown(GrantApplication grantApplication);
		IEnumerable<PrioritizationRegion> GetPrioritizationRegions();
		IEnumerable<PrioritizationIndustryScore> GetPrioritizationIndustryScores();

		bool UpdateIndustryScores(Stream stream);
		bool UpdateRegionScores(Stream stream);
		void RecalculatePriorityScores(int? grantApplicationId = null, bool allowUnderAssessment = false);
	}
}
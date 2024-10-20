﻿using System;
using System.Collections.Generic;
using System.IO;
using CJG.Core.Entities;

namespace CJG.Core.Interfaces.Service
{
	public interface IPrioritizationService : IService
	{
		PrioritizationThreshold GetThresholds();
		void UpdateThresholds(PrioritizationThreshold threshold);
		string SetRegionException(GrantApplication grantApplication, int prioritizationRegionId);

		PrioritizationScoreBreakdown GetBreakdown(GrantApplication grantApplication);
		IEnumerable<PrioritizationRegion> GetPrioritizationRegions();
		IEnumerable<PrioritizationIndustryScore> GetPrioritizationIndustryScores();
		IEnumerable<PrioritizationHighOpportunityOccupationScore> GetPrioritizationHighOpportunityOccupationScores();
		IEnumerable<Tuple<int, int>> GetRegionPostalCodeCounts();

		bool UpdateIndustryScores(Stream stream);
		bool UpdateHighOpportunityOccupationScores(Stream stream);
		bool UpdateRegionScores(Stream stream);
		void RecalculatePriorityScores(int? grantApplicationId = null, bool allowUnderAssessment = false);

		void AddPostalCodeToRegion(GrantApplication grantApplication, int regionId);

		List<string> GetHighOpportunityOccupationCodesAndNames(string nocs);
	}
}
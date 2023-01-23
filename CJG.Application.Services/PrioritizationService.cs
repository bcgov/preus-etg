using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Web;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;

namespace CJG.Application.Services
{
	public class PrioritizationService : Service, IPrioritizationService
	{
		public PrioritizationService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}

		public IEnumerable<PrioritizationRegion> GetPrioritizationRegions()
		{
			return _dbContext.PrioritizationRegions
				.OrderBy(r => r.Name)
				.ToList();
		}

		public IEnumerable<PrioritizationIndustryScore> GetPrioritizationIndustryScores()
		{
			return _dbContext.PrioritizationIndustryScores
				.OrderBy(r => r.NaicsCode)
				.ToList();
		}

		public PrioritizationThreshold GetThresholds()
		{
			return _dbContext.PrioritizationThresholds.SingleOrDefault() ?? new PrioritizationThreshold();
		}

		public void UpdateThresholds(PrioritizationThreshold threshold)
		{
			if (threshold == null)
				throw new ArgumentNullException(nameof(threshold));

			_dbContext.Update(threshold);
			_dbContext.Commit();
		}

		public void SetRegionException(GrantApplication grantApplication, int prioritizationRegionId)
		{
			var threshold = GetThresholds();
			if (threshold == null)
				throw new ObjectNotFoundException("Default Prioritization Threshold");

			var region = _dbContext.PrioritizationRegions.Find(prioritizationRegionId);

			var applicationScoreBreakdown = grantApplication.PrioritizationScoreBreakdown;

			if (region == null || applicationScoreBreakdown == null)
				return;

			var regionalResult = GetRegionalResult(region, threshold.RegionalThreshold);

			applicationScoreBreakdown.RegionalScore = regionalResult.Score;
			applicationScoreBreakdown.RegionalName = regionalResult.Name;

			grantApplication.PrioritizationScore = applicationScoreBreakdown.GetTotalScore();
		}

		public PrioritizationScoreBreakdown GetBreakdown(GrantApplication grantApplication)
		{
			var threshold = GetThresholds();
			if (threshold == null)
				throw new ObjectNotFoundException("Default Prioritization Threshold");

			var regionalResult = GetRegionalScore(grantApplication, threshold);
			var industryResult = GetIndustryScore(grantApplication, threshold);
			var smallBusinessScore = grantApplication.OrganizationNumberOfEmployeesInBC <= threshold.EmployeeCountThreshold ? 1 : 0;
			var firstTimeApplicantScore = GetFirstTimeApplicantScore(grantApplication);

			var breakdown = grantApplication.PrioritizationScoreBreakdown ?? new PrioritizationScoreBreakdown();

			breakdown.RegionalScore = regionalResult.Score;
			breakdown.RegionalName = regionalResult.Name;

			breakdown.IndustryScore = industryResult.Score;
			breakdown.IndustryName = industryResult.Name;
			breakdown.IndustryCode = industryResult.Code;

			breakdown.SmallBusinessScore = smallBusinessScore;
			breakdown.FirstTimeApplicantScore = firstTimeApplicantScore;

			foreach (var answer in breakdown.EligibilityAnswerScores.ToList())
				_dbContext.PrioritizationScoreBreakdownAnswers.Remove(answer);

			breakdown.EligibilityAnswerScores = GetEligibilityQuestionAnswers(grantApplication);

			return breakdown;
		}

		private RegionalResult GetRegionalScore(GrantApplication grantApplication, PrioritizationThreshold threshold)
		{
			var result = new RegionalResult();
			if (grantApplication.ApplicantPhysicalAddress == null)
				return result;

			var postalCode = grantApplication.ApplicantPhysicalAddress.PostalCode;
			if (string.IsNullOrWhiteSpace(postalCode))
				return result;

			var foundRegion = GetPriorityRegion(postalCode);
			if (foundRegion == null)
				return result;

			return GetRegionalResult(foundRegion, threshold.RegionalThreshold);
		}

		private static RegionalResult GetRegionalResult(PrioritizationRegion region, decimal regionalThreshold)
		{
			var regionalScore = region.RegionalScore >= regionalThreshold ? 1 : 0;

			return new RegionalResult
			{
				Score = regionalScore,
				Name = region.Name
			};
		}

		private PrioritizationRegion GetPriorityRegion(string postalCode)
		{
			if (string.IsNullOrWhiteSpace(postalCode) || postalCode.Length < 6 || postalCode.Length > 7) // Have to allow for "X0X X0X"
				return new PrioritizationRegion
				{
					Name = string.Empty,
					RegionalScore = 0
				};

			var postalCodeLookup = postalCode.ToUpper().Replace(" ", string.Empty);
			var postalCodeFound = _dbContext.PrioritizationPostalCodes
				.FirstOrDefault(a => a.PostalCode == postalCodeLookup);

			if (postalCodeFound?.Region == null)
				return new PrioritizationRegion
				{
					Name = string.Empty,
					RegionalScore = 0
				};

			return new PrioritizationRegion
			{
				Name = postalCodeFound.Region.Name,
				RegionalScore = postalCodeFound.Region.RegionalScore
			};
		}

		private IndustryResult GetIndustryScore(GrantApplication grantApplication, PrioritizationThreshold threshold)
		{
			var industryThreshold = threshold.IndustryThreshold;
			var result = new IndustryResult();

			if (grantApplication.NAICS == null)
				return result;

			var matchingIndustryScore = GetPrioritizationIndustryScoreByNaics(grantApplication);

			// Put in potential for matching lower than 4 digit NAICS codes here
			if (matchingIndustryScore == null)
				return result;


			var industryScore = matchingIndustryScore.IndustryScore <= industryThreshold ? 1 : 0;
			result.Score = industryScore;
			result.Name = matchingIndustryScore.Name;
			result.Code = matchingIndustryScore.NaicsCode;

			return result;
		}

		private PrioritizationIndustryScore GetPrioritizationIndustryScoreByNaics(GrantApplication grantApplication)
		{
			if (grantApplication.NAICS == null)
				return null;

			var applicationNaicsCode = grantApplication.NAICS.Code;

			PrioritizationIndustryScore matchingIndustryScore = null;

			if (applicationNaicsCode.Length > 4)
				applicationNaicsCode = applicationNaicsCode.Substring(0, 4);

			matchingIndustryScore = _dbContext.PrioritizationIndustryScores.FirstOrDefault(s => s.NaicsCode == applicationNaicsCode);

			// If we didn't find an industry code, try searching the 3 digit codes
			if (matchingIndustryScore == null)
			{
				if (applicationNaicsCode.Length > 3)
					applicationNaicsCode = applicationNaicsCode.Substring(0, 3);

				matchingIndustryScore = _dbContext.PrioritizationIndustryScores.FirstOrDefault(s => s.NaicsCode == applicationNaicsCode);
			}

			// If we didn't find an industry code, try searching the 2 digit codes
			if (matchingIndustryScore == null)
			{
				if (applicationNaicsCode.Length > 2)
					applicationNaicsCode = applicationNaicsCode.Substring(0, 2);

				matchingIndustryScore = _dbContext.PrioritizationIndustryScores.FirstOrDefault(s => s.NaicsCode == applicationNaicsCode);
			}

			return matchingIndustryScore;
		}

		private int GetFirstTimeApplicantScore(GrantApplication grantApplication)
		{
			var existingApplications = _dbContext.GrantApplications
				.Where(ga => ga.OrganizationId == grantApplication.OrganizationId)
				.Where(ga => ga.Id != grantApplication.Id);

			// List of statuses that would invalidate the "first-time" state
			// From doc:
			//   Note that applications that have been declined, withdrawn, or not yet accepted do not prevent the applicant from receiving a point.
			var existingStatus = new List<ApplicationStateInternal>
			{
				// General flow states
				ApplicationStateInternal.RecommendedForDenial,
				ApplicationStateInternal.RecommendedForApproval,
				ApplicationStateInternal.OfferIssued,
				ApplicationStateInternal.AgreementAccepted,
				ApplicationStateInternal.Closed,
				ApplicationStateInternal.ChangeRequest,
				ApplicationStateInternal.CompletionReporting,
				
				// Claim States
				ApplicationStateInternal.NewClaim,
				ApplicationStateInternal.ClaimApproved,
				ApplicationStateInternal.ClaimAssessEligibility,
				ApplicationStateInternal.ClaimAssessReimbursement,
				ApplicationStateInternal.ClaimDenied,
				ApplicationStateInternal.ClaimReturnedToApplicant
			};

			if (existingApplications.Any(ia => existingStatus.Contains(ia.ApplicationStateInternal)))
				return 0;

			return 1;
		}

		private static List<PrioritizationScoreBreakdownAnswer> GetEligibilityQuestionAnswers(GrantApplication grantApplication)
		{
			return grantApplication.GrantStreamEligibilityAnswers
				.Where(a => a.EligibilityAnswer)
				.Select(question => new PrioritizationScoreBreakdownAnswer
				{
					DateAdded = AppDateTime.UtcNow,
					QuestionedAnswered = question.GrantStreamEligibilityQuestions,
					QuestionScore = question.GrantStreamEligibilityQuestions.EligibilityPositiveAnswerPriorityScore
				}).ToList();
		}
	}

	internal class RegionalResult
	{
		public int Score { get; set; }
		public string Name { get; set; }
	}

	internal class IndustryResult
	{
		public int Score { get; set; }
		public string Name { get; set; }
		public string Code { get; set; }
	}
}
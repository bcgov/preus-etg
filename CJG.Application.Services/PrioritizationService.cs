using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Web;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using NLog;

namespace CJG.Application.Services
{
	public class PrioritizationService : Service, IPrioritizationService
	{
		private readonly INoteService _noteService;

		public PrioritizationService(IDataContext context, HttpContextBase httpContext, INoteService noteService, ILogger logger)
			: base(context, httpContext, logger)
		{
			_noteService = noteService;
		}

		public IEnumerable<PrioritizationRegion> GetPrioritizationRegions()
		{
			return _dbContext.PrioritizationRegions
				.OrderBy(r => r.Name)
				.Take(2000)  // Results aren't paginated, artificially page to stop long loads
				.ToList();
		}

		public IEnumerable<PrioritizationIndustryScore> GetPrioritizationIndustryScores()
		{
			return _dbContext.PrioritizationIndustryScores
				.OrderBy(r => r.NaicsCode)
				.Take(2000)  // Results aren't paginated, artificially page to stop long loads
				.ToList();
		}

		public IEnumerable<Tuple<int, int>> GetRegionPostalCodeCounts()
		{
			var counts = _dbContext.PrioritizationPostalCodes
				.GroupBy(pc => pc.RegionId)
				.Select(pc => new { RegionId = pc.Key, PostalCodeCount = pc.Count() })
				.AsEnumerable()
				.Select(an => new Tuple<int, int>(an.RegionId, an.PostalCodeCount));

			return counts;
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

			var regionalResult = GetRegionalResult(region, threshold);

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
			var smallBusinessScore = grantApplication.OrganizationNumberOfEmployeesInBC <= threshold.EmployeeCountThreshold
				? threshold.EmployeeCountAssignedScore
				: 0;
			var firstTimeApplicantScore = GetFirstTimeApplicantScore(grantApplication, threshold);

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
		
		public bool UpdateIndustryScores(Stream stream)
		{
			var importRows = new List<Tuple<string, string, int>>();
			try
			{
				using (var document = SpreadsheetDocument.Open(stream, false))
				{
					var workbookPart = document.WorkbookPart;
					var sheet = workbookPart.WorksheetParts.FirstOrDefault() ?? throw new InvalidOperationException("Excel file contains no worksheets.");
					var rows = sheet.Worksheet.Descendants<Row>().ToList();

					const string nameColumn = "A";
					const string codeColumn = "B";
					const string scoreColumn = "C";

					var firstRowSkipped = false;

					foreach (var row in rows)
					{
						if (!firstRowSkipped)
						{
							firstRowSkipped = true;
							continue;
						}

						var cells = row.Descendants<Cell>().ToList();
						var rowIndex = row.RowIndex;

						var name = GetCellValue<string>(workbookPart, cells.FirstOrDefault(c => c.CellReference == $"{nameColumn}{rowIndex}"));
						var code = GetCellValue<string>(workbookPart, cells.FirstOrDefault(c => c.CellReference == $"{codeColumn}{rowIndex}"));
						var score = GetCellValue<int>(workbookPart, cells.FirstOrDefault(c => c.CellReference == $"{scoreColumn}{rowIndex}"));

						importRows.Add(new Tuple<string, string, int>(name, code, score));
					}

					document.Close();
				}

				if (!importRows.Any())
					return false;

				var industries = _dbContext.PrioritizationIndustryScores.ToList();
				_dbContext.PrioritizationIndustryScores.RemoveRange(industries);

				var newIndustries = importRows.Select(i => new PrioritizationIndustryScore
				{
					Name = i.Item1,
					NaicsCode = i.Item2,
					IndustryScore = i.Item3,
					DateAdded = AppDateTime.UtcNow
				});
				_dbContext.PrioritizationIndustryScores.AddRange(newIndustries);

				_dbContext.CommitTransaction();
				return true;
			}
			catch
			{
				throw;
			}
		}

		public bool UpdateRegionScores(Stream stream)
		{
			var importRows = new List<Tuple<int, string, decimal>>();
			try
			{
				using (var document = SpreadsheetDocument.Open(stream, false))
				{
					var workbookPart = document.WorkbookPart;
					var sheet = workbookPart.WorksheetParts.FirstOrDefault() ?? throw new InvalidOperationException("Excel file contains no worksheets.");
					var rows = sheet.Worksheet.Descendants<Row>().ToList();

					const string nameColumn = "A";
					const string codeColumn = "B";
					const string scoreColumn = "C";

					var firstRowSkipped = false;

					foreach (var row in rows)
					{
						if (!firstRowSkipped)
						{
							firstRowSkipped = true;
							continue;
						}

						var cells = row.Descendants<Cell>().ToList();
						var rowIndex = row.RowIndex;

						var firstCellText = cells.FirstOrDefault(c => c.CellReference == $"{nameColumn}{rowIndex}")?.CellValue.Text;

						if (string.IsNullOrWhiteSpace(firstCellText))
							break;

						var regionId = GetCellValue<int>(workbookPart, cells.FirstOrDefault(c => c.CellReference == $"{nameColumn}{rowIndex}"));
						var name = GetCellValue<string>(workbookPart, cells.FirstOrDefault(c => c.CellReference == $"{codeColumn}{rowIndex}"));
						var score = GetCellValue<decimal>(workbookPart, cells.FirstOrDefault(c => c.CellReference == $"{scoreColumn}{rowIndex}"));

						importRows.Add(new Tuple<int, string, decimal>(regionId, name, score));
					}

					document.Close();
				}

				if (!importRows.Any())
					return false;

				var regions = _dbContext.PrioritizationRegions.ToList();

				var newRegions = importRows.Select(i => new PrioritizationRegion
				{
					Id = i.Item1,
					Name = i.Item2,
					RegionalScore = i.Item3,
					DateAdded = AppDateTime.UtcNow
				})
					.ToList();

				var newRegionsIds = newRegions
					.Select(r => r.Id)
					.ToList();

				var regionsToDelete = regions.Where(r => !newRegionsIds.Contains(r.Id))
					.ToList();

				foreach (var newRegion in newRegions)
				{
					var existingRegion = regions.FirstOrDefault(r => r.Id == newRegion.Id);
					if (existingRegion == null)
					{
						existingRegion = new PrioritizationRegion
						{
							Id = newRegion.Id,
							DateAdded = DateTime.UtcNow
						};
						_dbContext.PrioritizationRegions.Add(existingRegion);
					}

					existingRegion.Name = newRegion.Name;
					existingRegion.RegionalScore = newRegion.RegionalScore;
				}

				if (regionsToDelete.Any())
					_dbContext.PrioritizationRegions.RemoveRange(regionsToDelete);

				_dbContext.CommitTransaction();

				return true;
			}
			catch
			{
				throw;
			}
		}

		private static T GetCellValue<T>(WorkbookPart workbookPart, Cell cell, int? position = null)
		{
			if (cell == null)
				return default;

			try
			{
				var value = cell.InnerText;
				if (cell?.DataType?.Value == CellValues.SharedString)
				{
					var stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();

					if (stringTable != null)
					{
						return TruncateOrRound((T)System.Convert.ChangeType(stringTable.SharedStringTable.ElementAt(int.Parse(value)).InnerText, typeof(T)), position);
					}
				}

				if (typeof(T) == typeof(DateTime))
					return (T)System.Convert.ChangeType(DateTime.FromOADate(double.Parse(value)).ToLocalTime().ToUniversalTime(), typeof(T));

				if (typeof(T) == typeof(DateTime?))
				{
					if (value == null)
						return default;
					return (T)System.Convert.ChangeType(DateTime.FromOADate(double.Parse(value)).ToLocalTime().ToUniversalTime(), typeof(T));
				}

				return TruncateOrRound((T)System.Convert.ChangeType(value, typeof(T)), position);
			}
			catch
			{
				throw new InvalidOperationException($"The data in cell '{cell.CellReference}' was not valid.");
			}
		}

		/// <summary>
		/// Truncate the value to the specified position if it's a string, or round if it's a decimal.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <param name="position">When it's a string it will truncate to this many characters.  If it is a decimal it will truncate to this many decimal points.</param>
		/// <returns></returns>
		private static T TruncateOrRound<T>(T value, int? position = null)
		{
			if (position != null && value != null)
			{
				if (position < 0)
					throw new InvalidOperationException($"The argument '{position}' must be equal to or greater than 0.");

				if (typeof(T) == typeof(string))
					return (T)System.Convert.ChangeType(((string)(object)value).Substring(0, position.Value), typeof(T));

				var decimals = (decimal)Math.Pow(10, position.Value); // Should convert 2 to 100.
				if (typeof(T) == typeof(decimal)
					|| typeof(T) == typeof(float)
					|| typeof(T) == typeof(double))
					return (T)System.Convert.ChangeType(Math.Round((decimal)(object)value, position.Value), typeof(T));
				//return (T)System.Convert.ChangeType(Math.Truncate((decimal)(object)value * decimals) / decimals, typeof(T));
			}

			return value;
		}

		public void RecalculatePriorityScores(int? grantApplicationId = null, bool allowUnderAssessment = false)
		{
			var validExceptionStates = new List<ApplicationStateInternal>
			{
				ApplicationStateInternal.New,
				ApplicationStateInternal.PendingAssessment
			};

			if (allowUnderAssessment)
				validExceptionStates.Add(ApplicationStateInternal.UnderAssessment);

			var openApplications = _dbContext.GrantApplications
				.Where(ga => validExceptionStates.Contains(ga.ApplicationStateInternal));

			if (grantApplicationId.HasValue)
				openApplications = openApplications.Where(ga => ga.Id == grantApplicationId.Value);

			foreach (var grantApplication in openApplications)
			{
				var originalScore = grantApplication.PrioritizationScore;

				var breakdown = GetBreakdown(grantApplication);
				grantApplication.PrioritizationScoreBreakdown = breakdown;
				grantApplication.PrioritizationScore = breakdown.GetTotalScore();

				_noteService.AddWorkflowNote(grantApplication, $"Grant Application Priority Score recalculated. Previous Score: {originalScore}. New Score: {grantApplication.PrioritizationScore}.");

				if (grantApplication.PrioritizationScoreBreakdown.HasRegionalException())
					_noteService.AddWorkflowNote(grantApplication, "Priority list region lookup Exception. Postal Code not found in region list.");
			}

			_dbContext.CommitTransaction();
		}

		public void AddPostalCodeToRegion(GrantApplication grantApplication, int regionId)
		{
			var postalCode = GetPriorityPostalCode(grantApplication);
			postalCode = postalCode.ToUpper().Replace(" ", string.Empty);

			var existingPostalCode = _dbContext.PrioritizationPostalCodes.FirstOrDefault(p => p.PostalCode == postalCode);

			if (existingPostalCode != null)
				return;

			var region = _dbContext.PrioritizationRegions.Find(regionId);

			var newPostalCode = new PrioritizationPostalCode
			{
				PostalCode = postalCode,
				Region = region,
				DateAdded = AppDateTime.UtcNow,
			};

			_dbContext.PrioritizationPostalCodes.Add(newPostalCode);
			_dbContext.Commit();
		}

		private RegionalResult GetRegionalScore(GrantApplication grantApplication, PrioritizationThreshold threshold)
		{
			var result = new RegionalResult();
			if (grantApplication.ApplicantPhysicalAddress == null)
				return result;

			var postalCode = GetPriorityPostalCode(grantApplication);
			if (string.IsNullOrWhiteSpace(postalCode))
				return result;

			if (IsOutOfProvince(postalCode))
				return new RegionalResult
				{
					Name = "Out of Province",
					Score = 0
				};

			var foundRegion = GetPriorityRegion(postalCode);
			if (foundRegion == null)
				return result;

			return GetRegionalResult(foundRegion, threshold);
		}

		private bool IsOutOfProvince(string postalCode)
		{
			if (string.IsNullOrWhiteSpace(postalCode))
				return false;

			var postal = postalCode.Trim().ToUpper();

			return !postal.StartsWith("V");
		}

		private static string GetPriorityPostalCode(GrantApplication grantApplication)
		{
			return grantApplication.ApplicantPhysicalAddress.PostalCode;
		}

		private static RegionalResult GetRegionalResult(PrioritizationRegion region, PrioritizationThreshold threshold)
		{
			var regionalScore = region.RegionalScore >= threshold.RegionalThreshold ? threshold.RegionalThresholdAssignedScore : 0;

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


			var industryScore = matchingIndustryScore.IndustryScore <= industryThreshold ? threshold.IndustryAssignedScore : 0;
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

		private int GetFirstTimeApplicantScore(GrantApplication grantApplication, PrioritizationThreshold threshold)
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

			return threshold.FirstTimeApplicantAssignedScore;
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
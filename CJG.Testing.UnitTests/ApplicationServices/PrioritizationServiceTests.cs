using System.Collections.Generic;
using System.Linq;
using System.Web;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
	public class PrioritizationServiceTests
	{
		private PrioritizationService _service;
		private GrantApplication _grantApplication;
		private ServiceHelper _helper;

		private static PrioritizationRegion _regionInDemand;
		private static PrioritizationRegion _regionNoDemand;

		private GrantOpening _grantOpening;
		private List<GrantApplication> _existingApplications;

		[TestInitialize]
		public void Setup()
		{
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var user = new User { FirstName = "test", Id = 1, AccountType = AccountTypes.Internal };
			applicationAdministrator.AccountType = AccountTypes.External;
			var identity = applicationAdministrator.CreateIdentity();
			_helper = new ServiceHelper(typeof(PrioritizationService), identity);

			var threshold = new PrioritizationThreshold
			{
				IndustryThreshold = 2,
				HighOpportunityOccupationThreshold = 2,
				RegionalThreshold = 2.5m,
				EmployeeCountThreshold = 50,

				IndustryAssignedScore = 2,
				HighOpportunityOccupationAssignedScore = 15,
				RegionalThresholdAssignedScore = 3,
				EmployeeCountAssignedScore = 4,
				FirstTimeApplicantAssignedScore = 5
			};

			_helper.MockDbSet(GetPrioritizationIndustryScores());
			_helper.MockDbSet(GetPrioritizationHighOpportunityOccupationScores());
			_helper.MockDbSet(GetPrioritizationRegions());
			_helper.MockDbSet(GetPrioritizationPostalCodes());
			_helper.MockDbSet(applicationAdministrator);
			_helper.MockDbSet(user);
			_helper.MockDbSet(threshold);

			_grantOpening = new GrantOpening
			{
				GrantStream = new GrantStream
				{
					GrantProgramId = 1
				}
			};
			_grantApplication = GetFilledGrantApplication();

			_existingApplications = new List<GrantApplication>
			{
				new GrantApplication { Id = 2, ApplicationStateInternal = ApplicationStateInternal.UnderAssessment, OrganizationId = 57, GrantOpening = _grantOpening},
				new GrantApplication { Id = 5, ApplicationStateInternal = ApplicationStateInternal.New, OrganizationId = 57, GrantOpening = _grantOpening},
				//new GrantApplication { Id = 5, ApplicationStateInternal = ApplicationStateInternal.New, OrganizationId = 57 },
				_grantApplication
			};

			var grantPrograms = new List<GrantProgram>
			{
				new GrantProgram {Id = 1, Name = "ETG Program", ProgramCode = "ETG"},
				new GrantProgram { Id = 2, Name = "Other Programs", ProgramCode = "OTHER"}
			};

			_helper.MockDbSet(grantPrograms);
			_helper.MockDbSet(_existingApplications);

			_helper.GetMock<IUserService>().Setup(m => m.GetUser(It.IsAny<int>())).Returns(user);

			var httpContext = _helper.GetMock<HttpContextBase>();
			httpContext.Setup(x => x.User).Returns(identity);
			_service = _helper.Create<PrioritizationService>();
		}

		private static List<PrioritizationRegion> GetPrioritizationRegions()
		{
			_regionInDemand = new PrioritizationRegion
			{
				Id = 12,
				Name = "In-Demand Region",
				RegionalScore = 3.8m
			};
			_regionNoDemand = new PrioritizationRegion
			{
				Id = 24,
				Name = "No Demand Region",
				RegionalScore = 1.2m
			};

			return new List<PrioritizationRegion>
			{
				_regionInDemand,
				_regionNoDemand
			};
		}

		private static List<PrioritizationPostalCode> GetPrioritizationPostalCodes()
		{
			var postal1 = new PrioritizationPostalCode
			{
				Id = 15,
				Region = _regionInDemand,
				RegionId = _regionInDemand.Id,
				PostalCode = "V8T4G2"
			};
			var postal2 = new PrioritizationPostalCode
			{
				Id = 16,
				Region = _regionNoDemand,
				RegionId = _regionNoDemand.Id,
				PostalCode = "V912A3"
			};

			return new List<PrioritizationPostalCode>
			{
				postal1,
				postal2
			};
		}

		private static List<PrioritizationIndustryScore> GetPrioritizationIndustryScores()
		{
			var fakeHighIndustryScore = new PrioritizationIndustryScore { NaicsCode = "1234", IndustryScore = 1, DateAdded = AppDateTime.Now };
			var fakeLowIndustryScore = new PrioritizationIndustryScore { NaicsCode = "4321", Name = "Named Industry", IndustryScore = 3, DateAdded = AppDateTime.Now };
			var threeDigitIndustryScore = new PrioritizationIndustryScore { NaicsCode = "201", IndustryScore = 2, DateAdded = AppDateTime.Now };
			var twoDigitIndustryScore = new PrioritizationIndustryScore { NaicsCode = "40", IndustryScore = 1, DateAdded = AppDateTime.Now };

			var prioritizationIndustryScores = new List<PrioritizationIndustryScore>
			{
				fakeLowIndustryScore,
				fakeHighIndustryScore,
				threeDigitIndustryScore,
				twoDigitIndustryScore
			};

			return prioritizationIndustryScores;
		}

		private static List<PrioritizationHighOpportunityOccupationScore> GetPrioritizationHighOpportunityOccupationScores()
		{
			var fakeHighScore = new PrioritizationHighOpportunityOccupationScore { NOCCode = "12345", HighOpportunityOccupationScore = 1, DateAdded = AppDateTime.Now };
			var fakeMidScore = new PrioritizationHighOpportunityOccupationScore { NOCCode = "33333", HighOpportunityOccupationScore = 2, DateAdded = AppDateTime.Now };
			var fakeLowScore = new PrioritizationHighOpportunityOccupationScore { NOCCode = "54321", HighOpportunityOccupationScore = 3, DateAdded = AppDateTime.Now };

			var prioritizationHighOpportunityOccupationScores = new List<PrioritizationHighOpportunityOccupationScore>
			{
				fakeHighScore,
				fakeMidScore,
				fakeLowScore,
			};

			return prioritizationHighOpportunityOccupationScores;
		}

		[TestMethod, TestCategory("Prioritization Service Methods")]
		public void GetBreakdown_Runs()
		{
			var result = _service.GetBreakdown(_grantApplication);

			Assert.IsNotNull(result);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("Small Business Score")]
		public void GetBreakdown_SetsBusinessSize_On()
		{
			_grantApplication.OrganizationNumberOfEmployeesInBC = 10;
			_grantApplication.OrganizationNumberOfEmployeesWorldwide = 25;
			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual(4, result.SmallBusinessScore);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("Small Business Score")]
		public void GetBreakdown_SetsBusinessExactSize_Off()
		{
			_grantApplication.OrganizationNumberOfEmployeesInBC = 15;
			_grantApplication.OrganizationNumberOfEmployeesWorldwide = 50;
			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual(4, result.SmallBusinessScore);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("Small Business Score")]
		public void GetBreakdown_SetsBusinessBiggerSize_Off()
		{
			_grantApplication.OrganizationNumberOfEmployeesInBC = 28;
			_grantApplication.OrganizationNumberOfEmployeesWorldwide = 75;
			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual(0, result.SmallBusinessScore);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("Eligible Question Score")]
		public void GetBreakdown_SetsQuestionScore()
		{
			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual(3, result.QuestionScoreTotal);

			Assert.AreEqual(1, result.EligibilityAnswerScores.Count, "We are expecting only one answer as positive");

			var positiveAnswer = result.EligibilityAnswerScores.First();
			Assert.AreEqual(3, positiveAnswer.QuestionScore);
			Assert.AreEqual("This is a test of positives", positiveAnswer.QuestionedAnswered.EligibilityQuestion);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("First-time Score")]
		public void GetBreakdown_SetsFirstTimeScore_NoApplications_On()
		{
			_helper.MockDbSet(_grantApplication);
			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual(5, result.FirstTimeApplicantScore);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("First-time Score")]
		public void GetBreakdown_SetsFirstTimeScore_NoneComplete_On()
		{
			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual(5, result.FirstTimeApplicantScore);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("First-time Score")]
		public void GetBreakdown_SetsFirstTimeScore_ExistingComplete_Off()
		{
			var existingApplications = new List<GrantApplication>
			{
				new GrantApplication { Id = 2, ApplicationStateInternal = ApplicationStateInternal.UnderAssessment, OrganizationId = 57, GrantOpening = _grantOpening },
				new GrantApplication { Id = 5, ApplicationStateInternal = ApplicationStateInternal.OfferIssued, OrganizationId = 57, GrantOpening = _grantOpening },
			};

			_existingApplications.AddRange(existingApplications);

			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual(0, result.FirstTimeApplicantScore);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("First-time Score")]
		public void GetBreakdown_SetsFirstTimeScore_IgnoresOtherGrantPrograms_On()
		{
			var otherGrantOpening = new GrantOpening
			{
				GrantStream = new GrantStream
				{
					GrantProgramId = 2
				}
			};

			var existingApplications = new List<GrantApplication>
			{
				new GrantApplication { Id = 5, ApplicationStateInternal = ApplicationStateInternal.OfferIssued, OrganizationId = 57, GrantOpening = otherGrantOpening },
				_grantApplication
			};

			_helper.MockDbSet(existingApplications);
			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual(5, result.FirstTimeApplicantScore);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("Industry Score")]
		public void GetBreakdown_SetsIndustry_HighNeed_TwoDigit_On()
		{
			_grantApplication.NAICS = new NaIndustryClassificationSystem
			{
				Code = "40",
				Description = "Test 40",
				Level = 3,
				NAICSVersion = 2017
			};

			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual(2, result.IndustryScore);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("Industry Score")]
		public void GetBreakdown_SetsIndustry_HighNeed_ThreeDigit_On()
		{
			_grantApplication.NAICS = new NaIndustryClassificationSystem
			{
				Code = "201",
				Description = "Test 201",
				Level = 3,
				NAICSVersion = 2017
			};

			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual(2, result.IndustryScore);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("Industry Score")]
		public void GetBreakdown_SetsIndustry_HighNeed_FourDigit_On()
		{
			_grantApplication.NAICS = new NaIndustryClassificationSystem
			{
				Code = "1234",
				Description = "Test 1234",
				Level = 3,
				NAICSVersion = 2017
			};

			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual(2, result.IndustryScore);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("Industry Score")]
		public void GetBreakdown_SetsIndustry_HighNeed_FiveDigit_On()
		{
			_grantApplication.NAICS = new NaIndustryClassificationSystem
			{
				Code = "12345",
				Description = "Test 12345",
				Level = 3,
				NAICSVersion = 2017
			};

			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual(2, result.IndustryScore);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("Industry Score")]
		public void GetBreakdown_SetsIndustry_HighNeed_SixDigit_On()
		{
			_grantApplication.NAICS = new NaIndustryClassificationSystem
			{
				Code = "123456",
				Description = "Test 123456",
				Level = 3,
				NAICSVersion = 2017
			};

			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual(2, result.IndustryScore);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("Industry Score")]
		public void GetBreakdown_SetsIndustry_LowNeed_Off()
		{
			_grantApplication.NAICS = new NaIndustryClassificationSystem
			{
				Code = "4321",
				Description = "Test 4321",
				Level = 3,
				NAICSVersion = 2017
			};

			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual(0, result.IndustryScore);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("Industry Score")]
		public void GetBreakdown_SetsIndustry_Name()
		{
			_grantApplication.NAICS = new NaIndustryClassificationSystem
			{
				Code = "4321",
				Description = "Test 4321",
				Level = 3,
				NAICSVersion = 2017
			};

			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual("Named Industry", result.IndustryName);
			Assert.AreEqual("4321", result.IndustryCode);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("High Opportunity Score")]
		public void GetBreakdown_SetsHighOccupationOpportunity_NoMatchingNOC()
		{
			AddPIFWithNoc(_grantApplication, "44444");

			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual(0, result.HighOpportunityOccupationScore);
			Assert.AreEqual(string.Empty, result.HighOpportunityOccupationCode);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("High Opportunity Score")]
		public void GetBreakdown_SetsHighOccupationOpportunity_MatchingNOC_HighScore()
		{
			AddPIFWithNoc(_grantApplication, "12345");

			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual(15, result.HighOpportunityOccupationScore);
			Assert.AreEqual("12345", result.HighOpportunityOccupationCode);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("High Opportunity Score")]
		public void GetBreakdown_SetsHighOccupationOpportunity_MatchingNOC_LowScore()
		{
			AddPIFWithNoc(_grantApplication, "54321");

			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual(0, result.HighOpportunityOccupationScore);
			Assert.AreEqual(string.Empty, result.HighOpportunityOccupationCode);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("High Opportunity Score")]
		public void GetBreakdown_SetsHighOccupationOpportunity_OneMatchingNOC_ManyPIFs()
		{
			AddPIFWithNoc(_grantApplication, "12345");
			AddPIFWithNoc(_grantApplication, "45456");
			AddPIFWithNoc(_grantApplication, "54321");
			AddPIFWithNoc(_grantApplication, "54443");
			AddPIFWithNoc(_grantApplication, "00001");

			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual(15, result.HighOpportunityOccupationScore);
			Assert.AreEqual("12345", result.HighOpportunityOccupationCode);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("High Opportunity Score")]
		public void GetBreakdown_SetsHighOccupationOpportunity_ThreeMatchingNOC_ManyPIFs()
		{
			AddPIFWithNoc(_grantApplication, "33333");
			AddPIFWithNoc(_grantApplication, "33333");
			AddPIFWithNoc(_grantApplication, "12345");
			AddPIFWithNoc(_grantApplication, "11111");
			AddPIFWithNoc(_grantApplication, "54443");

			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual(15, result.HighOpportunityOccupationScore);
			Assert.AreEqual("12345, 33333", result.HighOpportunityOccupationCode);
		}

		private ParticipantForm AddPIFWithNoc(GrantApplication grantApplication, string nocCode)
		{
			var noc = new NationalOccupationalClassification { Code = nocCode };
			var participantForm = new ParticipantForm
			{
				FutureNoc = noc
			};
			grantApplication.ParticipantForms.Add(participantForm);

			return participantForm;
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("Regional Score")]
		public void GetBreakdown_SetsRegion_HighNeed_On()
		{
			_grantApplication.ApplicantPhysicalAddress.PostalCode = "V8T4G2";
			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual(3, result.RegionalScore);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("Regional Score")]
		public void GetBreakdown_SetsRegion_WithCasesAndSpaces()
		{
			_grantApplication.ApplicantPhysicalAddress.PostalCode = "v8t 4g2";
			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual(3, result.RegionalScore);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("Regional Score")]
		public void GetBreakdown_SetsRegion_LowNeed_Off()
		{
			_grantApplication.ApplicantPhysicalAddress.PostalCode = "V91 2A3";
			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual(0, result.RegionalScore);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("Regional Score")]
		public void GetBreakdown_SetsRegion_Name()
		{
			_grantApplication.ApplicantPhysicalAddress.PostalCode = "V91 2A3";
			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual("No Demand Region", result.RegionalName);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("Regional Score")]
		public void GetBreakdown_SetsRegion_OutOfProvince()
		{
			_grantApplication.ApplicantPhysicalAddress.PostalCode = "m7D 7A3";
			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual("Out of Province", result.RegionalName);
			Assert.AreEqual(0, result.RegionalScore);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("Regional Score")]
		public void GetBreakdown_SetsRegion_Exception_WhenNotPostalCodeMatch()
		{
			_grantApplication.ApplicantPhysicalAddress.PostalCode = "V7A3Y1";
			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual("", result.RegionalName);
			Assert.AreEqual(0, result.RegionalScore);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("Regional Score")]
		public void SetRegionException_UpdatesRegion()
		{
			_grantApplication.PrioritizationScoreBreakdown = new PrioritizationScoreBreakdown
			{
				RegionalScore = 0,
				RegionalName = string.Empty,
				EligibilityAnswerScores = new List<PrioritizationScoreBreakdownAnswer>()
			};

			Assert.AreEqual("", _grantApplication.PrioritizationScoreBreakdown.RegionalName);
			Assert.AreEqual(0, _grantApplication.PrioritizationScore);

			_service.SetRegionException(_grantApplication, 12);

			Assert.AreEqual("In-Demand Region", _grantApplication.PrioritizationScoreBreakdown.RegionalName);
			Assert.AreEqual(3, _grantApplication.PrioritizationScoreBreakdown.RegionalScore);
			Assert.AreEqual(3, _grantApplication.PrioritizationScore);
		}

		private GrantApplication GetFilledGrantApplication()
		{
			var filledGrantApplication = new GrantApplication
			{
				Id = 37,
				OrganizationId = 57,
				ApplicationStateInternal = ApplicationStateInternal.New,
				ApplicantPhysicalAddress = new ApplicationAddress
				{
					AddressLine1 = "1 Smith St.",
					City = "Victoria",
					PostalCode = "V8T4G2", // Have to allow for space or no-space
					Region = new Region(), // This is province/state
					Country = new Country()
				},
				GrantOpening = _grantOpening
			};

			// Add the Eligibility Questions and Answers to the application that we'll be checking for Positive responses
			var question1 = new GrantStreamEligibilityQuestion
			{
				EligibilityQuestion = "This is a test of positives",
				EligibilityPositiveAnswerPriorityScore = 3,
				EligibilityPositiveAnswerRequired = true,  // Irrelevant to score outcome
				IsActive = true
			};
			var answer1 = new GrantStreamEligibilityAnswer
			{
				GrantApplication = filledGrantApplication,
				GrantStreamEligibilityQuestions = question1,
				EligibilityAnswer = true
			};
			var question2 = new GrantStreamEligibilityQuestion
			{
				EligibilityQuestion = "This is a test of negatives",
				EligibilityPositiveAnswerPriorityScore = 2,
				EligibilityPositiveAnswerRequired = false,
				IsActive = true
			};
			var answer2 = new GrantStreamEligibilityAnswer
			{
				GrantApplication = filledGrantApplication,
				GrantStreamEligibilityQuestions = question2,
				EligibilityAnswer = false
			};
			filledGrantApplication.GrantStreamEligibilityAnswers.Add(answer1);
			filledGrantApplication.GrantStreamEligibilityAnswers.Add(answer2);

			return filledGrantApplication;
		}
	}
}

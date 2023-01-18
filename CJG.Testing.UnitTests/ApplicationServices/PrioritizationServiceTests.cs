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
				RegionalThreshold = 2.5m,
				EmployeeCountThreshold = 50
			};

			_helper.MockDbSet(GetPrioritizationIndustryScores());
			_helper.MockDbSet(GetPrioritizationRegions());
			_helper.MockDbSet(applicationAdministrator);
			_helper.MockDbSet(user);
			_helper.MockDbSet(threshold);

			_grantApplication = GetFilledGrantApplication();

			var existingApplications = new List<GrantApplication>
			{
				new GrantApplication { Id = 2, ApplicationStateInternal = ApplicationStateInternal.UnderAssessment, OrganizationId = 57 },
				new GrantApplication { Id = 5, ApplicationStateInternal = ApplicationStateInternal.New, OrganizationId = 57 },
				_grantApplication
			};

			_helper.MockDbSet(existingApplications);

			_helper.GetMock<IUserService>().Setup(m => m.GetUser(It.IsAny<int>())).Returns(user);

			var httpContext = _helper.GetMock<HttpContextBase>();
			httpContext.Setup(x => x.User).Returns(identity);
			_service = _helper.Create<PrioritizationService>();
		}

		private static List<PrioritizationRegion> GetPrioritizationRegions()
		{
			var regionInDemand = new PrioritizationRegion
			{
				Id = 12,
				Name = "In-Demand Region",
				RegionalScore = 3.8m
			};
			var regionNoDemand = new PrioritizationRegion
			{
				Id = 24,
				Name = "No Demand Region",
				RegionalScore = 1.2m
			};

			return new List<PrioritizationRegion>
			{
				regionInDemand,
				regionNoDemand
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

		[TestMethod, TestCategory("Prioritization Service Methods")]
		public void GetBreakdown_Runs()
		{
			var result = _service.GetBreakdown(_grantApplication);

			Assert.IsNotNull(result);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("Small Business Score")]
		public void GetBreakdown_SetsBusinessSize_On()
		{
			_grantApplication.OrganizationNumberOfEmployeesInBC = 25;
			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual(1, result.SmallBusinessScore);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("Small Business Score")]
		public void GetBreakdown_SetsBusinessExactSize_Off()
		{
			_grantApplication.OrganizationNumberOfEmployeesInBC = 50;
			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual(1, result.SmallBusinessScore);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("Small Business Score")]
		public void GetBreakdown_SetsBusinessBiggerSize_Off()
		{
			_grantApplication.OrganizationNumberOfEmployeesInBC = 75;
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

			Assert.AreEqual(1, result.FirstTimeApplicantScore);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("First-time Score")]
		public void GetBreakdown_SetsFirstTimeScore_NoneComplete_On()
		{
			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual(1, result.FirstTimeApplicantScore);
		}

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("First-time Score")]
		public void GetBreakdown_SetsFirstTimeScore_ExistingComplete_Off()
		{
			var existingApplications = new List<GrantApplication>
			{
				new GrantApplication { Id = 2, ApplicationStateInternal = ApplicationStateInternal.UnderAssessment, OrganizationId = 57 },
				new GrantApplication { Id = 5, ApplicationStateInternal = ApplicationStateInternal.OfferIssued, OrganizationId = 57 },
				_grantApplication
			};

			_helper.MockDbSet(existingApplications);
			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual(0, result.FirstTimeApplicantScore);
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

			Assert.AreEqual(1, result.IndustryScore);
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

			Assert.AreEqual(1, result.IndustryScore);
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

			Assert.AreEqual(1, result.IndustryScore);
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

			Assert.AreEqual(1, result.IndustryScore);
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

			Assert.AreEqual(1, result.IndustryScore);
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

		[TestMethod, TestCategory("Prioritization Service Methods"), TestCategory("Regional Score")]
		public void GetBreakdown_SetsRegion_HighNeed_On()
		{
			_grantApplication.ApplicantPhysicalAddress.PostalCode = "V8T4G2";
			var result = _service.GetBreakdown(_grantApplication);

			Assert.AreEqual(1, result.RegionalScore);
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
			Assert.AreEqual(1, _grantApplication.PrioritizationScoreBreakdown.RegionalScore);
			Assert.AreEqual(1, _grantApplication.PrioritizationScore);
		}

		private static GrantApplication GetFilledGrantApplication()
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
				}
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

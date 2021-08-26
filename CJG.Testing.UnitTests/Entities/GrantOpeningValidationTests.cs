using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Testing.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace CJG.Testing.UnitTests.Entities
{
	[TestClass()]
	public class GrantOpeningValidationTests
	{
		ServiceHelper helper { get; set; }

		[TestInitialize]
		public void Setup()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();

			helper = new ServiceHelper(typeof(GrantOpeningService), user);
			helper.MockContext();
			helper.MockDbSet<GrantOpening>();
			helper.MockDbSet<TrainingPeriod>();
		}

		[TestMethod, TestCategory("GrantOpening"), TestCategory("Validate")]
		public void Validate_When_GrantOpening_Not_Associated_With_Valid_Training_Program()
		{
			var grantOpening = EntityHelper.CreateGrantOpening();
			var service = helper.Create<GrantOpeningService>();

			// Act
			var validationResults = service.Validate(grantOpening).ToArray();

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The grant opening must be associated with a valid training period."));
		}

		[TestMethod, TestCategory("GrantOpening"), TestCategory("Validate")]
		public void Validate_When_GrantOpening_PublishDate_Lessthan_TodayOrLater_Or_Greaterthan_Or_Equal_To_TrainingPeriodEndDate()
		{
			// Arrange
			AppDateTime.SetNow(DateTime.Now);
			var grantOpening = new GrantOpening()
			{
				Id = 1,
				State = GrantOpeningStates.Scheduled,
				OpeningDate = AppDateTime.UtcNow.AddDays(1),
				PublishDate = AppDateTime.UtcMorning.AddDays(-1),
				TrainingPeriod = new TrainingPeriod()
				{
					EndDate = AppDateTime.UtcMorning.AddDays(-5)
				}
			};

			var service = helper.Create<GrantOpeningService>();

			// Act
			var validationResults = service.Validate(grantOpening).ToArray();

			string validationMsg = "Publish date must be today or later, and the same or before the training period end date '" + grantOpening.TrainingPeriod?.EndDate.ToLocalMidnight().ToString("yyyy-MM-dd") + "'.";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validationMsg));
		}

		[TestMethod, TestCategory("GrantOpening"), TestCategory("Validate")]
		public void Validate_When_GrantOpening_OpeningDate_Lessthan_PublishDate_And_Greaterthan_Or_Equal_To_TrainingPeriodEndDate()
		{
			AppDateTime.SetNow(DateTime.Now);
			var grantOpening = new GrantOpening()
			{
				Id = 1,
				State = GrantOpeningStates.Scheduled,
				OpeningDate = AppDateTime.UtcNow.AddDays(1),
				PublishDate = AppDateTime.UtcNow.AddDays(5),
				TrainingPeriod = new TrainingPeriod()
				{
					EndDate = AppDateTime.UtcNow.AddDays(-5)
				}
			};

			var service = helper.Create<GrantOpeningService>();

			// Act
			var validationResults = service.Validate(grantOpening).ToArray();

			string validationMsg = "Opening date must be the same or later than the publish date, and before the closing date.";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validationMsg));
		}

		[TestMethod, TestCategory("GrantOpening"), TestCategory("Validate")]
		public void Validate_When_GrantOpening_ClosingDate_Less_OpeningDate_And_Greaterthan_Or_Equal_To_TrainingPeriodEndDate()
		{
			AppDateTime.SetNow(DateTime.Now);
			var grantOpening = new GrantOpening()
			{
				Id = 1,
				State = GrantOpeningStates.Scheduled,
				OpeningDate = AppDateTime.UtcNow.AddDays(1),
				ClosingDate = AppDateTime.UtcNow.AddDays(-1),
				TrainingPeriodId = 1,
				TrainingPeriod = new TrainingPeriod()
				{
					Id = 1,
					EndDate = AppDateTime.UtcNow.AddDays(-2)
				}
			};

			var service = helper.Create<GrantOpeningService>();

			// Act
			var validationResults = service.Validate(grantOpening).ToArray();

			string validationMsg = "Closing date must be the same or later than the opening date, and during the training period of '" + grantOpening.TrainingPeriod?.StartDate.ToLocalMidnight().ToString("yyyy-MM-dd") + " to " + grantOpening.TrainingPeriod?.EndDate.ToLocalMidnight().ToString("yyyy-MM-dd") + "'.";

			// Assert
			Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validationMsg));
		}
	}
}
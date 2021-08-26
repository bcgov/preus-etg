using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using CJG.Testing.Core;
using CJG.Core.Entities;
using CJG.Application.Services;
using System;

namespace CJG.Testing.UnitTests.Entities
{
    [TestClass()]
    public class TrainingPeriodValidationTests
    {
        ServiceHelper helper { get; set; }
        DateTime StartDate = DateTime.UtcNow.AddDays(2);
        DateTime EndDate = DateTime.UtcNow.AddDays(-2);
        DateTime DefaultPublishDate = DateTime.UtcNow.AddDays(4);
        DateTime DefaultOpeningDate = DateTime.UtcNow.AddDays(-4);

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            helper = new ServiceHelper(typeof(TrainingProgramService), user);
            helper.MockContext();
        }

        [TestMethod, TestCategory("TrainingPeriod"), TestCategory("Validate")]
        public void Validate_When_TrainingPeriod()
        {
            var trainingPeriod = new TrainingPeriod() { Caption = "Intake Period 1" };

            helper.MockDbSet<TrainingPeriod>(new[] { trainingPeriod });

            var service = helper.Create<TrainingProgramService>();

            // Act
            var validationResults = service.Validate(trainingPeriod).ToArray();

            string validateMsg = "The training period must be associated with a fiscal year.";

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
        }

        [TestMethod, TestCategory("TrainingPeriod"), TestCategory("Validate")]
        public void Validate_When_TrainingPeriod_StartDate_Not_Before_EndDate()
        {
            var trainingPeriod = new TrainingPeriod() { Caption = "Intake Period 1", StartDate = StartDate, EndDate = EndDate };

            helper.MockDbSet<TrainingPeriod>(new[] { trainingPeriod });

            var service = helper.Create<TrainingProgramService>();

            // Act
            var validationResults = service.Validate(trainingPeriod).ToArray();

            string validateMsg = "The start date must be before the end date '" + EndDate.ToLocalMidnight().ToString("yyyy-MM-dd") + "'.";

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
        }

        [TestMethod, TestCategory("TrainingPeriod"), TestCategory("Validate")]
        public void Validate_When_TrainingPeriod_PublishDate_Not_Before_OpeningDate()
        {
            var trainingPeriod = new TrainingPeriod() { Caption = "Intake Period 1", DefaultPublishDate = DefaultPublishDate, DefaultOpeningDate = DefaultOpeningDate };

            helper.MockDbSet<TrainingPeriod>(new[] { trainingPeriod });

            var service = helper.Create<TrainingProgramService>();

            // Act
            var validationResults = service.Validate(trainingPeriod).ToArray();

            string validateMsg = "The default publish date cannot be after the default opening date '" + DefaultOpeningDate.ToLocalMorning().ToString("yyyy-MM-dd") + "'.";

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
        }

        [TestMethod, TestCategory("TrainingPeriod"), TestCategory("Validate")]
        public void Validate_When_TrainingPeriod_PublishDate_Not_Before_EndDate()
        {
            var trainingPeriod = new TrainingPeriod() { Caption = "Intake Period 1", DefaultPublishDate = DefaultPublishDate, EndDate = EndDate };

            helper.MockDbSet<TrainingPeriod>(new[] { trainingPeriod });

            var service = helper.Create<TrainingProgramService>();

            // Act
            var validationResults = service.Validate(trainingPeriod).ToArray();

            string validateMsg = "The default publish date must be before the end date '" + EndDate.ToLocalMidnight().ToString("yyyy-MM-dd") + "'.";

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
        }

        [TestMethod, TestCategory("TrainingPeriod"), TestCategory("Validate")]
        public void Validate_When_TrainingPeriod_OpeningDate_Not_Between_PublishDate_And_EndDate()
        {
            var trainingPeriod = new TrainingPeriod() { Caption = "Intake Period 1", DefaultPublishDate = DefaultPublishDate, EndDate = EndDate, DefaultOpeningDate = DefaultOpeningDate };

            helper.MockDbSet<TrainingPeriod>(new[] { trainingPeriod });

            var service = helper.Create<TrainingProgramService>();

            // Act
            var validationResults = service.Validate(trainingPeriod).ToArray();

            string validateMsg = "The default opening date must be between '" + DefaultPublishDate.ToLocalMorning().ToString("yyyy-MM-dd") + "' and '" + EndDate.ToLocalMidnight().ToString("yyyy-MM-dd") + "'.";

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validateMsg));
        }
    }
}
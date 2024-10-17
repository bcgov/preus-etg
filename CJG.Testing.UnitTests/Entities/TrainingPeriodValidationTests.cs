using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Infrastructure.Entities;
using CJG.Testing.Core;

namespace CJG.Testing.UnitTests.Entities
{
    [TestClass]
    public class TrainingPeriodValidationTests
    {
	    private ServiceHelper Helper { get; set; }
	    private readonly DateTime _startDate = DateTime.UtcNow.AddDays(2);
	    private readonly DateTime _endDate = DateTime.UtcNow.AddDays(-2);
	    private readonly DateTime _defaultPublishDate = DateTime.UtcNow.AddDays(4);
	    private readonly DateTime _defaultOpeningDate = DateTime.UtcNow.AddDays(-4);

	    private TrainingProgramService _service;
	    private TrainingPeriod _trainingPeriod;
	    private Mock<DbSet<TrainingPeriod>> _mockedTrainingPeriods;
	    private Mock<IDataContext> _mockContext;

	    [TestInitialize]
        public void Setup()
        {
			// Arrange
			var user = EntityHelper.CreateExternalUser();
            Helper = new ServiceHelper(typeof(TrainingProgramService), user);
            _mockContext = Helper.MockContext();

            _service = Helper.Create<TrainingProgramService>();
            _trainingPeriod = new TrainingPeriod
            {
	            Caption = "Intake Period 1",
            };
		
            _mockedTrainingPeriods = Helper.MockDbSet(new List<TrainingPeriod> { _trainingPeriod });
			
		}

		[TestMethod, TestCategory("TrainingPeriod"), TestCategory("Validate")]
        public void Validate_When_TrainingPeriod()
        {
            var validateMsg = "The training period must be associated with a fiscal year.";
			ValidateHasErrors(_trainingPeriod, validateMsg);
        }

		[TestMethod, TestCategory("TrainingPeriod"), TestCategory("Validate")]
        public void Validate_When_TrainingPeriod_StartDate_Not_Before_EndDate()
        {
	        _trainingPeriod.StartDate = _startDate;
	        _trainingPeriod.EndDate = _endDate;
        
			var validateMsg = "The start date must be before the end date '" + _endDate.ToLocalMidnight().ToString("yyyy-MM-dd") + "'.";
            ValidateHasErrors(_trainingPeriod, validateMsg);
        }

        [TestMethod, TestCategory("TrainingPeriod"), TestCategory("Validate")]
        public void Validate_When_TrainingPeriod_PublishDate_Not_Before_OpeningDate()
        {
	        _trainingPeriod.DefaultPublishDate = _defaultPublishDate;
	        _trainingPeriod.DefaultOpeningDate = _defaultOpeningDate;

            var validateMsg = "The default publish date cannot be after the default opening date '" + _defaultOpeningDate.ToLocalMorning().ToString("yyyy-MM-dd") + "'.";
            ValidateHasErrors(_trainingPeriod, validateMsg);
		}

        [TestMethod, TestCategory("TrainingPeriod"), TestCategory("Validate")]
        public void Validate_When_TrainingPeriod_PublishDate_Not_Before_EndDate()
        {
            _trainingPeriod.DefaultPublishDate = _defaultPublishDate;
            _trainingPeriod.EndDate = _endDate;

            var validateMsg = "The default publish date must be before the end date '" + _endDate.ToLocalMidnight().ToString("yyyy-MM-dd") + "'.";
            ValidateHasErrors(_trainingPeriod, validateMsg);
        }

        [TestMethod, TestCategory("TrainingPeriod"), TestCategory("Validate")]
        public void Validate_When_TrainingPeriod_OpeningDate_Not_Between_PublishDate_And_EndDate()
        {
	        _trainingPeriod.DefaultPublishDate = _defaultPublishDate;
	        _trainingPeriod.EndDate = _endDate;
	        _trainingPeriod.DefaultOpeningDate = _defaultOpeningDate;

			
            var validateMsg = "The default opening date must be between '" + _defaultPublishDate.ToLocalMorning().ToString("yyyy-MM-dd") + "' and '" + _endDate.ToLocalMidnight().ToString("yyyy-MM-dd") + "'.";
            ValidateHasErrors(_trainingPeriod, validateMsg);
        }

        [TestMethod, TestCategory("TrainingPeriod"), TestCategory("Validate")]
        public void Validate_When_TrainingPeriod_StartDate_Before_FiscalStartDate()
        {
	        _trainingPeriod.StartDate = _startDate;
			_trainingPeriod.FiscalYear = new FiscalYear
			{
				StartDate = _startDate.AddDays(2)
			};

            var validateMsg = "The start date cannot be before the fiscal year start date.";
            ValidateHasErrors(_trainingPeriod, validateMsg);
        }

        [TestMethod, TestCategory("TrainingPeriod"), TestCategory("Validate")]
        public void Validate_When_TrainingPeriod_EndDate_Past_FiscalEndDate()
        {
	        _trainingPeriod.EndDate = _endDate;
			_trainingPeriod.FiscalYear = new FiscalYear
			{
				EndDate = _endDate.AddDays(-5)
			};

            var validateMsg = "The end date cannot be past the fiscal year end date.";
            ValidateHasErrors(_trainingPeriod, validateMsg);
        }

		[TestMethod, TestCategory("TrainingPeriod"), TestCategory("Validate")]
        public void Validate_When_TrainingPeriod_EndDate_InThePast()  // Should fire on add only
        {
	        _trainingPeriod.EndDate = DateTime.UtcNow.AddDays(-15);
			_trainingPeriod.FiscalYear = new FiscalYear
			{
				EndDate = _endDate.AddMonths(5)
			};
			var grantStream = GetGrantStream();
			
			_trainingPeriod.GrantStream = grantStream;
			_trainingPeriod.GrantStreamId = grantStream.Id;


			var validateMsg = "The end date cannot be in the past.";

			_mockContext.Object.Entry(_trainingPeriod).State = EntityState.Modified;
			ValidateHasErrors(_trainingPeriod, validateMsg, false);

			_mockContext.Object.Entry(_trainingPeriod).State = EntityState.Added;
            ValidateHasErrors(_trainingPeriod, validateMsg, true);
		}

		[TestMethod, TestCategory("TrainingPeriod"), TestCategory("Validate")]
        public void Validate_When_TrainingPeriod_MatchesExistingPeriod()
        {
	        var grantStream = GetGrantStream();
	        var fiscalYear = new FiscalYear
	        {
		        Id = 7,
		        EndDate = _endDate.AddDays(-5)
	        };

			SetupComplexTrainingPeriod(grantStream, fiscalYear);

			var validateMsg = "The dates entered match the existing intake period 'Test Intake Period'.";
            ValidateHasErrors(_trainingPeriod, validateMsg, false);  // We don't expect an error yet

			var existingPeriod = new TrainingPeriod
			{
				Id = 56,
				Caption = "Test Intake Period",
				StartDate = _startDate,
				EndDate = _endDate,
				IsActive = true,
				GrantStream = grantStream,
				GrantStreamId = grantStream.Id,
				FiscalYear = fiscalYear,
				FiscalYearId = fiscalYear.Id
			};
			_mockedTrainingPeriods.Object.Add(existingPeriod);

			ValidateHasErrors(_trainingPeriod, validateMsg, true);  // We now expect an error
        }

		[TestMethod, TestCategory("TrainingPeriod"), TestCategory("Validate")]
        public void Validate_When_TrainingPeriod_HasClosedGrantOpening()
        {
	        var grantStream = GetGrantStream();
	        var fiscalYear = new FiscalYear
	        {
		        Id = 7,
		        EndDate = _endDate.AddDays(-5)
	        };

			SetupComplexTrainingPeriod(grantStream, fiscalYear);

			_trainingPeriod.GrantOpenings = new List<GrantOpening>
			{
				new GrantOpening
				{
					State = GrantOpeningStates.Closed,
					TrainingPeriod = _trainingPeriod,
					TrainingPeriodId = _trainingPeriod.Id,
				}
			};

			// We have to update the start date to trip the bypass of the date validation
			var updatedStartDate = _trainingPeriod.StartDate.AddHours(5);
			_mockContext.Object.Entry(_trainingPeriod).State = EntityState.Modified;
			_mockContext.Object.Entry(_trainingPeriod).OriginalValues[nameof(_trainingPeriod.StartDate)] = updatedStartDate;

			var validateMsg = "This Intake Period is associated with a closed grant opening. The start and end dates cannot be modified.";
            ValidateHasErrors(_trainingPeriod, validateMsg);
        }

		[TestMethod, TestCategory("TrainingPeriod"), TestCategory("Validate")]
        public void Validate_When_TrainingPeriod_HasOpenGrantOpening()
        {
	        var grantStream = GetGrantStream();
	        var fiscalYear = new FiscalYear
	        {
		        Id = 7,
		        EndDate = _endDate.AddDays(-5)
	        };

			SetupComplexTrainingPeriod(grantStream, fiscalYear);

			_trainingPeriod.GrantOpenings = new List<GrantOpening>
			{
				new GrantOpening
				{
					State = GrantOpeningStates.Closed,
					TrainingPeriod = _trainingPeriod,
					TrainingPeriodId = _trainingPeriod.Id,
				}
			};

			// We have to update the start date to trip the bypass of the date validation
			var updatedStartDate = _trainingPeriod.StartDate.AddHours(5);
			_mockContext.Object.Entry(_trainingPeriod).State = EntityState.Modified;
			_mockContext.Object.Entry(_trainingPeriod).OriginalValues[nameof(_trainingPeriod.StartDate)] = updatedStartDate;

			var validateMsg = "This Intake Period is associated with a closed grant opening. The start and end dates cannot be modified.";
            ValidateHasErrors(_trainingPeriod, validateMsg);
        }

        private void SetupComplexTrainingPeriod(GrantStream grantStream, FiscalYear fiscalYear)
        {
	        _trainingPeriod.Id = 55;
	        _trainingPeriod.StartDate = _startDate;
	        _trainingPeriod.EndDate = _endDate;
	        _trainingPeriod.IsActive = true;
	        _trainingPeriod.GrantStream = grantStream;
	        _trainingPeriod.GrantStreamId = grantStream.Id;
	        _trainingPeriod.FiscalYear = fiscalYear;
	        _trainingPeriod.FiscalYearId = fiscalYear.Id;
        }

        private static GrantStream GetGrantStream()
        {
	        var grantProgram = new GrantProgram
	        {
		        Id = 37
	        };

	        var grantStream = new GrantStream
	        {
		        Id = 12,
		        GrantProgram = grantProgram,
				GrantProgramId = grantProgram.Id
	        };
			grantProgram.GrantStreams.Add(grantStream);

	        return grantStream;
        }

        private void ValidateHasErrors(TrainingPeriod trainingPeriod, string validateMsg, bool expectError = true)
        {
	        var validationResults = _service.Validate(trainingPeriod);
	        Assert.AreEqual(expectError, validationResults.Any(x => x.ErrorMessage == validateMsg));
        }
	}
}
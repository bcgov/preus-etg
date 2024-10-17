using System;
using System.Collections.Generic;
using CJG.Core.Entities;
using CJG.Web.External.Areas.Int.Models.IntakePeriods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CJG.Testing.UnitTests.Entities
{
	[TestClass]
	public class IntakePeriodsTrainingPeriodModelExtensionsTests
	{
		private IntakePeriodsTrainingPeriodModel _model;
		private TrainingPeriod _intakePeriod;

		[TestInitialize]
		public void Setup()
		{
			_model = new IntakePeriodsTrainingPeriodModel();
			_intakePeriod = new TrainingPeriod
			{
				GrantStreamId = 7,
				GrantStream = new GrantStream
				{
					Id = 7,
					GrantProgram = new GrantProgram
					{
						Id = 10
					}
				},
				GrantOpenings = new List<GrantOpening>()
			};
		}

		[TestMethod]
		public void NewModel_DisallowsEditing_When_FiscalPast()
		{
			AppDateTime.SetNow(new DateTime(2021, 10, 1));

			var fiscalYear = new FiscalYear
			{
				Id = 5,
				StartDate = new DateTime(2020, 4, 1).ToUtcMorning(),
				EndDate = new DateTime(2021, 3, 31).ToUtcMidnight(),
				Caption = "FY 2020/2021"
			};

			_intakePeriod.FiscalYearId = fiscalYear.Id;
			_intakePeriod.StartDate = new DateTime(2021, 10, 1);  // These dates are irrelevant
			_intakePeriod.EndDate = new DateTime(2021, 10, 1).AddMonths(2); // These dates are irrelevant

			_model.GetEmptyModel(fiscalYear, 1, 1, 1);

			Assert.AreEqual("Intake periods in past fiscal years cannot be adjusted.", _model.WarningMessage);
			Assert.AreEqual(true, _model.StartDateDisabled);
			Assert.AreEqual(true, _model.EndDateDisabled);
		}

		[TestMethod]
		public void LoadModel_DisallowsEditing_When_FiscalPast()
		{
			AppDateTime.SetNow(new DateTime(2021, 10, 1));

			var fiscalYear = new FiscalYear
			{
				Id = 5,
				StartDate = new DateTime(2020, 4, 1).ToUtcMorning(),
				EndDate = new DateTime(2021, 3, 31).ToUtcMidnight(),
				Caption = "FY 2020/2021"
			};

			_intakePeriod.FiscalYearId = fiscalYear.Id;
			_intakePeriod.StartDate = new DateTime(2021, 10, 1);  // These dates are irrelevant
			_intakePeriod.EndDate = new DateTime(2021, 10, 1).AddMonths(2); // These dates are irrelevant

			_model.LoadModel(_intakePeriod, fiscalYear);

			Assert.AreEqual("Intake periods in past fiscal years cannot be adjusted.", _model.WarningMessage);
			Assert.AreEqual(true, _model.StartDateDisabled);
			Assert.AreEqual(true, _model.EndDateDisabled);
		}

		[TestMethod]
		public void LoadModel_AllowsEditing_When_FiscalCurrent()
		{
			AppDateTime.SetNow(new DateTime(2020, 7, 1));

			var fiscalYear = new FiscalYear
			{
				Id = 5,
				StartDate = new DateTime(2020, 4, 1).ToUtcMorning(),
				EndDate = new DateTime(2021, 3, 31).ToUtcMidnight(),
				Caption = "FY 2020/2021"
			};

			_intakePeriod.FiscalYearId = fiscalYear.Id;
			_intakePeriod.StartDate = new DateTime(2021, 10, 1);  // These dates are irrelevant
			_intakePeriod.EndDate = new DateTime(2021, 10, 1).AddMonths(2); // These dates are irrelevant

			_model.LoadModel(_intakePeriod, fiscalYear);

			Assert.AreEqual(true, string.IsNullOrWhiteSpace(_model.WarningMessage));
			Assert.AreEqual(false, _model.StartDateDisabled);
			Assert.AreEqual(false, _model.EndDateDisabled);
		}
	}
}
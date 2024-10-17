using System;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Testing.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
	public class TrainingPeriodServiceTests : ServiceUnitTestBase
	{
		[TestInitialize]
		public void Setup()
		{

		}

		// Using original insert as sample data:
		//		([FiscalYearId],	[Caption],			[StartDate],																[EndDate],																	[DefaultPublishDate],													[DefaultOpeningDate]) VALUES
		// 		 (@Incrementor,		N'Intake Period 1', CAST(CAST(@IntValue AS VARCHAR) + N'-04-01T07:00:00.000' AS DateTime),		CAST(CAST(@IntValue AS VARCHAR) + N'-08-31T07:00:00.000' AS DateTime),		CAST(CAST(@IntValue AS VARCHAR) + N'-01-01T08:00:00.000' AS DateTime),	CAST(CAST(@IntValue AS VARCHAR) + N'-02-01T08:00:00.000' AS DateTime))
		[TestMethod, TestCategory("TrainingPeriod"), TestCategory("Service")]
		public void SetDefaultDatesAdjustsCorrectly()
		{
			var trainingPeriod = new TrainingPeriod
			{
				StartDate = new DateTime(2021, 4, 1)
			};

			var helper = new ServiceHelper(typeof(TrainingPeriodService));
			var service = helper.Create<TrainingPeriodService>();

			service.SetDefaultDates(trainingPeriod);

			trainingPeriod.DefaultPublishDate.Should().Be(new DateTime(2021, 1, 1));
			trainingPeriod.DefaultOpeningDate.Should().Be(new DateTime(2021, 2, 1));
		}
	}
}
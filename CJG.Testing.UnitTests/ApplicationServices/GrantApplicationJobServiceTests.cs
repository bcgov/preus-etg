using System;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Testing.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
	public class GrantApplicationJobServiceTests : ServiceUnitTestBase
	{
		private ServiceHelper _helper;
		private GrantApplication _grantApplication;
		private GrantApplicationJobService _service;
		private DateTime _now;

		[TestInitialize]
		public void Setup()
		{
			AppDateTime.SetNow(DateTime.UtcNow);
			_now = AppDateTime.UtcNow;

			var applicationAdministrator = EntityHelper.CreateExternalUser();

			_helper = new ServiceHelper(typeof(GrantApplicationJobService), applicationAdministrator);

			var grantProgram = new GrantProgram { Id = 1, ProgramCode = "ETG" };
			_helper.MockDbSet(grantProgram);

			var grantStream = new GrantStream { IsActive = true, GrantProgram = grantProgram };
			var grantOpening = EntityHelper.CreateGrantOpening();
			grantOpening.GrantStream = grantStream;

			_grantApplication = EntityHelper.CreateGrantApplication(grantOpening, applicationAdministrator, ApplicationStateInternal.New);

			_helper.MockDbSet<GrantApplication>(_grantApplication);
			_service = _helper.Create<GrantApplicationJobService>();
		}

		[TestMethod, TestCategory("Grant Application"), TestCategory("Service")]
		public void GetUnassessedGrantApplications_NoneReturned_WrongState()
		{
			_grantApplication.DateSubmitted = _now.AddDays(-65);
			_grantApplication.ApplicationStateInternal = ApplicationStateInternal.UnderAssessment;
			var actual = _service.GetUnassessedGrantApplications();

			actual.Should().HaveCount(0);
		}

		[TestMethod, TestCategory("Grant Application"), TestCategory("Service")]
		public void GetUnassessedGrantApplications_NoneReturned_TooEarly()
		{
			_grantApplication.DateSubmitted = _now.AddDays(-5);
			var actual = _service.GetUnassessedGrantApplications();

			actual.Should().HaveCount(0);
		}

		[TestMethod, TestCategory("Grant Application"), TestCategory("Service")]
		public void GetUnassessedGrantApplications_NoneReturned_DateOf()
		{
			_grantApplication.DateSubmitted = _now.AddDays(-60);
			var actual = _service.GetUnassessedGrantApplications();

			actual.Should().HaveCount(0);
		}

		[TestMethod, TestCategory("Grant Application"), TestCategory("Service")]
		public void GetUnassessedGrantApplications_OneReturned_DatePassed()
		{
			_grantApplication.DateSubmitted = _now.AddDays(-62);
			var actual = _service.GetUnassessedGrantApplications();

			actual.Should().HaveCount(1);
		}
	}
}
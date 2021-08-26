using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Testing.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
	public class SiteMinderServiceTests : ServiceUnitTestBase
	{
		[TestInitialize]
		public void Setup()
		{
		}

		[TestMethod, TestCategory("SiteMinder"), TestCategory("Service")]
		public void LogIn_WithNewGuid_CreatesSessionRecordWithExpectedUserType()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(SiteMinderService), user);
			var service = helper.Create<SiteMinderService>();
			var userType = "Business";

			// Act
			service.LogIn(Guid.NewGuid(), userType);

			// Assert
			service.CurrentUserType
				.Should()
				.Be(BCeIDAccountTypeCodes.Business);
		}

		[TestMethod, TestCategory("SiteMinder"), TestCategory("Service")]
		public void LogInInternal_WithTestGuid_CreatesSessionRecordTestGuid()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(SiteMinderService), user);
			var service = helper.Create<SiteMinderService>();
			var testGuid = Guid.NewGuid();

			// Act
			service.LogInInternal(testGuid);

			// Assert
			service.CurrentUserGuid
				.Should()
				.Be(testGuid);
		}

		[TestMethod, TestCategory("SiteMinder"), TestCategory("Service")]
		public void LogOut_WithCurrntUser_RemovesUserFromSession()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(SiteMinderService), user);
			var service = helper.Create<SiteMinderService>();
			var userType = BCeIDAccountTypeCodes.Business.ToString();

			// Act
			service.LogIn(Guid.NewGuid(), userType);

			// Assert
			service.CurrentUserGuid
				.Should()
				.NotBeEmpty();

			service.LogOut();

			service.CurrentUserGuid
				.Should()
				.BeEmpty();

			service.CurrentUserName
				.Should()
				.BeNull();

			service.CurrentUserType
				.Should()
				.Be(BCeIDAccountTypeCodes.Void);
		}

		[TestMethod, TestCategory("SiteMinder"), TestCategory("Service")]
		public void CurrentUserType_WithCurrntUserWithBusinessType_ReturnsBusinessType()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(SiteMinderService), user);
			var service = helper.Create<SiteMinderService>();
			var userType = BCeIDAccountTypeCodes.Business.ToString();

			// Act
			service.LogIn(Guid.NewGuid(), userType);

			// Assert
			service.CurrentUserType.Should().Be(BCeIDAccountTypeCodes.Business);
		}

		[TestMethod, TestCategory("SiteMinder"), TestCategory("Service")]
		public void CurrentUserName_WithCurrntUserWithBusinessType_ReturnsBusinessType()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(SiteMinderService), user);
			var service = helper.Create<SiteMinderService>();
			var userType = BCeIDAccountTypeCodes.Business.ToString();

			// Act
			service.LogIn(Guid.NewGuid(), userType);

			// Assert
			service.CurrentUserName.Should().BeNullOrEmpty();
		}

		[TestMethod, TestCategory("SiteMinder"), TestCategory("Service")]
		public void CurrentUserGuid_WithCurrntUserWithBusinessType_ReturnsBusinessType()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(SiteMinderService), user);
			var service = helper.Create<SiteMinderService>();
			var userType = BCeIDAccountTypeCodes.Business.ToString();
			var userGuid = Guid.NewGuid();

			// Act
			service.LogIn(userGuid, userType);

			// Assert
			service.CurrentUserGuid.Should().Be(userGuid);
		}
	}
}
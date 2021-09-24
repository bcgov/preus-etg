using System;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Infrastructure.Entities;
using CJG.Testing.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using System.Web;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
	public class OrganizationServiceTests : ServiceUnitTestBase
	{
		private User _user;
		private ServiceHelper _helper;
		private Mock<ILogger> _loggerMock;
		private Mock<HttpContextBase> _httpContextMock;
		private Mock<IDataContext> _dbContextMock;
		private OrganizationService _service;

		[TestInitialize]
		public void Setup()
		{
			_loggerMock = new Mock<ILogger>();
			_httpContextMock = new Mock<HttpContextBase>();
			_dbContextMock = new Mock<IDataContext>();
			_service = new OrganizationService(_dbContextMock.Object, _httpContextMock.Object, _loggerMock.Object);

			_user = EntityHelper.CreateExternalUser();
			_helper = new ServiceHelper(typeof(OrganizationService), _user);
		}

		[TestMethod, TestCategory("Organization"), TestCategory("Service")]
		public void CanFindOrganizationProfileAdminUserTest()
		{
			// Arrange
			var user1 = new User {
				FirstName = "FirstName1",
				LastName = "LastName1",
				Id = 1,
				OrganizationId = 1,
				IsOrganizationProfileAdministrator = true
			};

			_helper.MockDbSet<User>(user1);

			var localService = _helper.Create<OrganizationService>();

			// Act
			var results = localService.GetOrganizationProfileAdminUserId(user1.Id);

			// Assert
			results.Should().Be(1);
		}

		[TestMethod, TestCategory("Organization"), TestCategory("Service")]
		public void CanNotFindOrganizationProfileAdminUserTest()
		{
			// Arrange
			var dbSetMock = _helper.MockDbSet<User>((User)null);
			_dbContextMock.Setup(x => x.Users).Returns(dbSetMock.Object);

			// Act
			var results = _service.GetOrganizationProfileAdminUserId(1);

			// Assert
			results.Should().Be(0);
		}

		[TestMethod, TestCategory("Organization"), TestCategory("Service")]
		public void CanAddOrganizationProfile()
		{
			// Arrange
			var organizationMock = _helper.MockDbSet<Organization>();
			_dbContextMock.Setup(x => x.Organizations).Returns(organizationMock.Object);

			// Act
			_service.AddOrganizationProfile(new Organization());

			// Assert
			organizationMock.Verify(x => x.Add(It.IsAny<Organization>()), Times.Exactly(1));
			_dbContextMock.Verify(x => x.Commit(), Times.Exactly(1));
		}

		[TestMethod, TestCategory("Organization"), TestCategory("Service")]
		public void CanUpdateOrganizationProfile()
		{
			// Arrange
			var organizationMock = _helper.MockDbSet<Organization>();
			_dbContextMock.Setup(x => x.Organizations).Returns(organizationMock.Object);

			// Act
			_service.UpdateOrganization(new Organization());

			// Assert
			_dbContextMock.Verify(x => x.Update(It.IsAny<Organization>()), Times.Exactly(1));
			_dbContextMock.Verify(x => x.Commit(), Times.Exactly(1));
		}

		[TestMethod, TestCategory("Organization"), TestCategory("Service")]
		public void OrganizationProfileNeedsToBeUpdated()
		{
			var now = DateTime.UtcNow;
			AppDateTime.SetNow(now);

			// Arrange
			var organization = new Organization
			{
				DateUpdated = now.AddMonths(-12)
			};

			// Act
			var result = _service.ProfileSubjectToVerification(organization);

			Assert.AreEqual(true, result);
		}

		[TestMethod, TestCategory("Organization"), TestCategory("Service")]
		public void OrganizationProfileDoesNotNeedsToBeUpdated()
		{
			var now = DateTime.UtcNow;
			AppDateTime.SetNow(now);

			// Arrange
			var organization = new Organization
			{
				DateUpdated = now.AddMonths(-6)
			};

			// Act
			var result = _service.ProfileSubjectToVerification(organization);

			Assert.AreEqual(false, result);
		}

		[TestMethod, TestCategory("Organization"), TestCategory("Service")]
		public void CanGetOrganizationType()
		{
			// Arrange
			var organizationTypeMock = _helper.MockDbSet<OrganizationType>();
			_dbContextMock.Setup(x => x.OrganizationTypes).Returns(organizationTypeMock.Object);

			// Act
			_service.GetOrganizationType(1);

			// Assert
			organizationTypeMock.Verify(x => x.Find(It.IsAny<int>()), Times.Exactly(1));
		}

		[TestMethod, TestCategory("Organization"), TestCategory("Service")]
		public void CanGetDefaultOrganizationType()
		{
			// Arrange
			var organizationTypeMock = _helper.MockDbSet<OrganizationType>();
			_dbContextMock.Setup(x => x.OrganizationTypes).Returns(organizationTypeMock.Object);

			// Act
			_service.GetDefaultOrganizationType();

			// Assert
			organizationTypeMock.Verify(x => x.Find(It.IsAny<int>()), Times.Exactly(1));
		}

		[TestMethod, TestCategory("Organization"), TestCategory("Service")]
		public void CanGetDefaultPrioritySector()
		{
			// Arrange
			var prioritySectorMock = _helper.MockDbSet<PrioritySector>();
			_dbContextMock.Setup(x => x.PrioritySectors).Returns(prioritySectorMock.Object);

			// Act
			_service.GetDefaultPrioritySector();

			// Assert
			prioritySectorMock.Verify(x => x.Find(It.IsAny<int>()), Times.Exactly(1));
		}

		[TestMethod, TestCategory("Organization"), TestCategory("Service")]
		public void CanGetDefaultLegalStructure()
		{
			// Arrange
			var legalStructureMock = _helper.MockDbSet<LegalStructure>();
			_dbContextMock.Setup(x => x.LegalStructures).Returns(legalStructureMock.Object);

			// Act
			_service.GetDefaultLegalStructure();

			// Assert
			legalStructureMock.Verify(x => x.Find(It.IsAny<int>()), Times.Exactly(1));
		}
	}
}

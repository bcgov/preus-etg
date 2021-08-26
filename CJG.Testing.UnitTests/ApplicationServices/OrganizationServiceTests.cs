using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CJG.Core.Entities;
using FluentAssertions;
using CJG.Application.Services;
using NLog;
using CJG.Infrastructure.Entities;
using System.Web;
using CJG.Testing.Core;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
	public class OrganizationServiceTests : ServiceUnitTestBase
	{
		private User user;
		private ServiceHelper helper;

		[TestInitialize]
		public void Setup()
		{
			user = EntityHelper.CreateExternalUser();
			helper = new ServiceHelper(typeof(OrganizationService), user);
		}

		[TestMethod, TestCategory("Organization"), TestCategory("Service")]
		public void CanFindOrganizationProfileAdminUserTest()
		{
			// Arrange
			var user1 = new User() {
				FirstName = "FirstName1",
				LastName = "LastName1",
				Id = 1,
				OrganizationId = 1,
				IsOrganizationProfileAdministrator = true
			};

			helper.MockDbSet<User>(user1);

			var service = helper.Create<OrganizationService>();

			// Act
			var results = service.GetOrganizationProfileAdminUserId(user1.Id);

			// Assert
			results.Should().Be(1);
		}

		[TestMethod, TestCategory("Organization"), TestCategory("Service")]
		public void CanNotFindOrganizationProfileAdminUserTest()
		{
			// Arrange
			var loggerMock = new Mock<ILogger>();
			var httpContextMock = new Mock<HttpContextBase>();
			var dbSetMock = helper.MockDbSet<User>((User)null);
			var dbContextMock = new Mock<IDataContext>();
			dbContextMock.Setup(x => x.Users).Returns(dbSetMock.Object);
			var service = new OrganizationService(dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

			// Act
			var results = service.GetOrganizationProfileAdminUserId(1);

			// Assert
			results.Should().Be(0);
		}

		[TestMethod, TestCategory("Organization"), TestCategory("Service")]
		public void CanAddOrganizationProfile()
		{
			// Arrange
			var loggerMock = new Mock<ILogger>();
			var httpContextMock = new Mock<HttpContextBase>();
			var organizationMock = helper.MockDbSet<Organization>();
			var dbContextMock = new Mock<IDataContext>();
			dbContextMock.Setup(x => x.Organizations).Returns(organizationMock.Object);
			var service = new OrganizationService(dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

			// Act
			service.AddOrganizationProfile(new Organization());

			// Assert
			organizationMock.Verify(x => x.Add(It.IsAny<Organization>()), Times.Exactly(1));
			dbContextMock.Verify(x => x.Commit(), Times.Exactly(1));
		}

		[TestMethod, TestCategory("Organization"), TestCategory("Service")]
		public void CanUpdateOrganizationProfile()
		{
			// Arrange
			var httpContextMock = new Mock<HttpContextBase>();
			var loggerMock = new Mock<ILogger>();
			var organizationMock = helper.MockDbSet<Organization>();
			var dbContextMock = new Mock<IDataContext>();
			dbContextMock.Setup(x => x.Organizations).Returns(organizationMock.Object);
			var service = new OrganizationService(dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

			// Act
			service.UpdateOrganization(new Organization());

			// Assert
			dbContextMock.Verify(x => x.Update(It.IsAny<Organization>()), Times.Exactly(1));
			dbContextMock.Verify(x => x.Commit(), Times.Exactly(1));
		}
		 
		[TestMethod, TestCategory("Organization"), TestCategory("Service")]
		public void CanGetOrganizationType()
		{
			// Arrange
			var httpContextMock = new Mock<HttpContextBase>();
			var loggerMock = new Mock<ILogger>();
			var organizationTypeMock = helper.MockDbSet<OrganizationType>();
			var dbContextMock = new Mock<IDataContext>();
			dbContextMock.Setup(x => x.OrganizationTypes).Returns(organizationTypeMock.Object);
			var service = new OrganizationService(dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

			// Act
			service.GetOrganizationType(1);

			// Assert
			organizationTypeMock.Verify(x => x.Find(It.IsAny<int>()), Times.Exactly(1));
		}

		[TestMethod, TestCategory("Organization"), TestCategory("Service")]
		public void CanGetDefaultOrganizationType()
		{
			// Arrange
			var httpContextMock = new Mock<HttpContextBase>();
			var loggerMock = new Mock<ILogger>();
			var organizationTypeMock = helper.MockDbSet<OrganizationType>();
			var dbContextMock = new Mock<IDataContext>();
			dbContextMock.Setup(x => x.OrganizationTypes).Returns(organizationTypeMock.Object);
			var service = new OrganizationService(dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

			// Act
			service.GetDefaultOrganizationType();

			// Assert
			organizationTypeMock.Verify(x => x.Find(It.IsAny<int>()), Times.Exactly(1));
		}

		[TestMethod, TestCategory("Organization"), TestCategory("Service")]
		public void CanGetDefaultPrioritySector()
		{
			// Arrange
			var httpContextMock = new Mock<HttpContextBase>();
			var loggerMock = new Mock<ILogger>();
			var prioritySectorMock = helper.MockDbSet<PrioritySector>();
			var dbContextMock = new Mock<IDataContext>();
			dbContextMock.Setup(x => x.PrioritySectors).Returns(prioritySectorMock.Object);
			var service = new OrganizationService(dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

			// Act
			service.GetDefaultPrioritySector();

			// Assert
			prioritySectorMock.Verify(x => x.Find(It.IsAny<int>()), Times.Exactly(1));
		}

		[TestMethod, TestCategory("Organization"), TestCategory("Service")]
		public void CanGetDefaultLegalStructure()
		{
			// Arrange
			var httpContextMock = new Mock<HttpContextBase>();
			var loggerMock = new Mock<ILogger>();
			var legalStructureMock = helper.MockDbSet<LegalStructure>();
			var dbContextMock = new Mock<IDataContext>();
			dbContextMock.Setup(x => x.LegalStructures).Returns(legalStructureMock.Object);
			var service = new OrganizationService(dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

			// Act
			service.GetDefaultLegalStructure();

			// Assert
			legalStructureMock.Verify(x => x.Find(It.IsAny<int>()), Times.Exactly(1));
		}
	}
}

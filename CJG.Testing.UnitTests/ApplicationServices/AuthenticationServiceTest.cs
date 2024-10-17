using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using CJG.Testing.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
	public class AuthenticationServiceTest : ServiceUnitTestBase
	{
		[TestInitialize]
		public void Setup()
		{
		}

		[TestMethod, TestCategory("Authentication"), TestCategory("Service")]
		public void CanGetLogInOptionsTest()
		{
			// Arrange
			var user1 = new User()
			{
				FirstName = "CJG011",
				LastName = "Test01",
				BCeIDGuid = Guid.NewGuid(),
				AccountType = AccountTypes.External,
				Id = 1
			};
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var helper = new ServiceHelper(typeof(AuthenticationService), identity);
			var users = new List<User>() { user1 };
			helper.MockDbSet(users);
			var service = helper.Create<AuthenticationService>();

			// Act
			var results = service.GetLogInOptions(AccountTypes.External);

			// Assert
			results.Count().Should().Be(1);
		}

		[TestMethod, TestCategory("Authentication"), TestCategory("Service")]
		public void CanLogInTest()
		{
			// Arrange
			var externalUser = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(externalUser);
			var httpContextMock = HttpHelper.MockHttpContext(identity);
			var loggerMock = new Mock<ILogger>();
			var dbContextMock = new Mock<IDataContext>();
			var siteMinderServiceMock = new Mock<ISiteMinderService>();
			var userServiceMock = new Mock<IUserService>();
			var testUser = new User();
			var helper = new ServiceHelper(typeof(AuthenticationService), externalUser);
			var dbSetMock = helper.MockDbSet(testUser);
			var mockDbSet = new Mock<DbSet<User>>(dbSetMock);
			dbContextMock.Setup(m => m.Users).Returns(dbSetMock.Object);
			var service = new AuthenticationService(siteMinderServiceMock.Object, userServiceMock.Object, dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

			// Act
			service.LogIn(Guid.NewGuid(), "");

			// Assert
			siteMinderServiceMock.Verify(x => x.LogIn(It.IsAny<Guid>(),It.IsAny<string>()), Times.Exactly(1));
		}

		[TestMethod, TestCategory("Authentication"), TestCategory("Service")]
		public void CanLogInInternalWithGuidTest()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var httpContextMock = HttpHelper.MockHttpContext(identity);
			var loggerMock = new Mock<ILogger>();
			var dbContextMock = new Mock<IDataContext>();
			var siteMinderServiceMock = new Mock<ISiteMinderService>();
			var userServiceMock = new Mock<IUserService>();
			var service = new AuthenticationService(siteMinderServiceMock.Object, userServiceMock.Object, dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

			// Act
			service.LogInInternal(Guid.NewGuid());

			// Assert
			siteMinderServiceMock.Verify(x => x.LogInInternal(It.IsAny<Guid>()), Times.Exactly(1));
		}

		[TestMethod, TestCategory("Authentication"), TestCategory("Service")]
		[ExpectedException(typeof(NullReferenceException),"HttpContext.Current is null, can't verify here")]
		public void CanLogOutTest()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var httpContextMock = HttpHelper.MockHttpContext(identity);
			var loggerMock = new Mock<ILogger>();
			var dbContextMock = new Mock<IDataContext>();
			var siteMinderServiceMock = new Mock<ISiteMinderService>();
			var userServiceMock = new Mock<IUserService>();
			var service = new AuthenticationService(siteMinderServiceMock.Object, userServiceMock.Object, dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

			// Act
			var result = service.LogOut();
		   
			// Assert
			siteMinderServiceMock.Verify(x => x.LogOut(), Times.Exactly(1));
		}
	}
}

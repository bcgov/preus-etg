using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Infrastructure.Entities;
using CJG.Infrastructure.Identity;
using CJG.Testing.Core;
using FluentAssertions;
using Microsoft.AspNet.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
    public class AuthorizationServiceTest : ServiceUnitTestBase
    {
        [TestInitialize]
        public void Setup()
        {
        }

        [TestMethod, TestCategory("Authorization"), TestCategory("Service")]
        public void GetPrivileges_RoleAsParameter_ShouldReturnClaimValues()
        {
            // Arrange
            var applicationClaims = new List<ApplicationClaim>()
            {
                new ApplicationClaim()
                {
                    ClaimType = "Privilege",
                    ClaimValue = "IA1"
                }
            };
            var applicationRoles = new List<ApplicationRole>()
            {
                 new ApplicationRole()
                 {
                     Id = "1",
                     Name = "Assessor",
                     ApplicationClaims = applicationClaims
                }
             };
			var user = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(ApplicationRole), user);
			var applicationRoleDbSetMock = helper.MockDbSet(applicationRoles);

            var userStore = new Mock<IUserStore<ApplicationUser>>();
            var applicationUserManager = new Mock<ApplicationUserManager>(userStore.Object);
			var dbContextMock = new Mock<IDataContext>();
			var httpContextMock = new Mock<HttpContextBase>();
			var loggerMock = new Mock<ILogger>();
			dbContextMock.Setup(x => x.ApplicationRoles).Returns(applicationRoleDbSetMock.Object);

			var service = new AuthorizationService(applicationUserManager.Object, dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

            // Act
            var results = service.GetPrivileges("Assessor");

			// Assert
			results.Count().Should().Be(1);
			Assert.AreEqual(applicationClaims.FirstOrDefault().ClaimValue, results.FirstOrDefault());
        }

        [TestMethod, TestCategory("Authorization"), TestCategory("Service")]
        public void GetPrivileges_UserAsParameter_ReturnAllPrivileges()
        {
            // Arrange
            var claims = new List<System.Security.Claims.Claim>()
            {
                new System.Security.Claims.Claim("Privilege","IA1"),
                new System.Security.Claims.Claim("Privilege","IA2"),
            };
            var loggerMock = new Mock<ILogger>();
            var claimsIdentityMock = new Mock<ClaimsIdentity>();
            claimsIdentityMock.SetupGet(x => x.Claims).Returns(claims);
            var userMock = new Mock<IPrincipal>();
            userMock.SetupGet(x => x.Identity).Returns(claimsIdentityMock.Object);
            var dbContextMock = new Mock<IDataContext>();
            var httpContextMock = new Mock<HttpContextBase>();
            var userManager = new Mock<IUserManagerAdapter>();
			var userStore = new Mock<IUserStore<ApplicationUser>>();
            var applicationUserManager = new Mock<ApplicationUserManager>(userStore.Object);
			var service = new AuthorizationService(applicationUserManager.Object, dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

			// Act
			var results = userMock.Object.GetPrivileges();

            // Assert
            results.Count().Should().Be(2);
           
        }

        [TestMethod, TestCategory("Authorization"), TestCategory("Service")]
        public void HasPrivilege_PassInExistingPrivilege_ShouldReturnTrue()
        {
            // Arrange
            var claims = new List<System.Security.Claims.Claim>()
            {
                new System.Security.Claims.Claim("Privilege","AM1"),
                new System.Security.Claims.Claim("Privilege","AM2"),
            };
            var loggerMock = new Mock<ILogger>();
            var claimsIdentityMock = new Mock<ClaimsIdentity>();
            claimsIdentityMock.SetupGet(x => x.Claims).Returns(claims);
            var userMock = new Mock<IPrincipal>();
            userMock.SetupGet(x => x.Identity).Returns(claimsIdentityMock.Object);
            var dbContextMock = new Mock<IDataContext>();
            var httpContextMock = new Mock<HttpContextBase>();
            var userManager = new Mock<IUserManagerAdapter>();
			var userStore = new Mock<IUserStore<ApplicationUser>>();
            var applicationUserManager = new Mock<ApplicationUserManager>(userStore.Object);

			var service = new AuthorizationService(applicationUserManager.Object, dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

			// Act
			var results = userMock.Object.HasPrivilege(Privilege.AM1);

            // Assert
            results.Should().Be(true);
        }

        [TestMethod, TestCategory("Authorization"), TestCategory("Service")]
        public void HasPrivilege_PassINNonExistingPrivilege_ShouldReturnFalse()
        {
            // Arrange
            var claims = new List<System.Security.Claims.Claim>()
            {
                new System.Security.Claims.Claim("Privilege","AM1"),
                new System.Security.Claims.Claim("Privilege","AM2"),
            };
            var loggerMock = new Mock<ILogger>();
            var claimsIdentityMock = new Mock<ClaimsIdentity>();
            claimsIdentityMock.SetupGet(x => x.Claims).Returns(claims);
            var userMock = new Mock<IPrincipal>();
            userMock.SetupGet(x => x.Identity).Returns(claimsIdentityMock.Object);
            var dbContextMock = new Mock<IDataContext>();
            var httpContextMock = new Mock<HttpContextBase>();
            var userManager = new Mock<IUserManagerAdapter>();
			var userStore = new Mock<IUserStore<ApplicationUser>>();
            var applicationUserManager = new Mock<ApplicationUserManager>(userStore.Object);

			var service = new AuthorizationService(applicationUserManager.Object, dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

			// Act
			var results = userMock.Object.HasPrivilege(Privilege.AM5);

            // Assert
            results.Should().Be(false);
        }

        [TestMethod, TestCategory("Authorization"), TestCategory("Service")]
        public void HasPrivilege_WhenExistingPrivilegeParameter_ShouldReturnTrue()
        {
            // Arrange
            var claims = new List<System.Security.Claims.Claim>()
            {
                new System.Security.Claims.Claim("Privilege","AM1"),
                new System.Security.Claims.Claim("Privilege","AM2"),
            };
            var loggerMock = new Mock<ILogger>();
            var claimsIdentityMock = new Mock<ClaimsIdentity>();
            claimsIdentityMock.SetupGet(x => x.Claims).Returns(claims);
            var userMock = new Mock<IPrincipal>();
            userMock.SetupGet(x => x.Identity).Returns(claimsIdentityMock.Object);
            var dbContextMock = new Mock<IDataContext>();
            var httpContextMock = new Mock<HttpContextBase>();
            var userManager = new Mock<IUserManagerAdapter>();
			var userStore = new Mock<IUserStore<ApplicationUser>>();
            var applicationUserManager = new Mock<ApplicationUserManager>(userStore.Object);

			var service = new AuthorizationService(applicationUserManager.Object, dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

			// Act
			var results = userMock.Object.HasPrivilege(Privilege.AM1);

            // Assert
            results.Should().Be(true);
        }

        [TestMethod, TestCategory("Authorization"), TestCategory("Service")]
        public void HasPrivilege_WhenNonExistingPrivilegeParameter_ShouldReturnFalse()
        {
            // Arrange
            var claims = new List<System.Security.Claims.Claim>()
            {
                new System.Security.Claims.Claim("Privilege","AM1"),
                new System.Security.Claims.Claim("Privilege","AM2"),
            };
            var loggerMock = new Mock<ILogger>();
            var claimsIdentityMock = new Mock<ClaimsIdentity>();
            claimsIdentityMock.SetupGet(x => x.Claims).Returns(claims);
            var userMock = new Mock<IPrincipal>();
            userMock.SetupGet(x => x.Identity).Returns(claimsIdentityMock.Object);
            var dbContextMock = new Mock<IDataContext>();
            var httpContextMock = new Mock<HttpContextBase>();
            var userManager = new Mock<IUserManagerAdapter>();
			var userStore = new Mock<IUserStore<ApplicationUser>>();
            var applicationUserManager = new Mock<ApplicationUserManager>(userStore.Object);

			var service = new AuthorizationService(applicationUserManager.Object, dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

			// Act
			var results = userMock.Object.HasPrivilege(Privilege.AM3);

            // Assert
            results.Should().Be(false);
        }

        [TestMethod, TestCategory("Authorization"), TestCategory("Service")]
        public void GetUserRoles_WhenUserAsParameter_ShouldReturnAllUserRoles()
        {
            // Arrange
            var claims = new List<System.Security.Claims.Claim>()
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role,"Assessor"),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role,"Director")
            };
            var loggerMock = new Mock<ILogger>();
            var claimsIdentityMock = new Mock<ClaimsIdentity>();
            claimsIdentityMock.SetupGet(x => x.Claims).Returns(claims);
            var dbContextMock = new Mock<IDataContext>();
            var httpContextMock = new Mock<HttpContextBase>();
            var userManager = new Mock<IUserManagerAdapter>();
			var userStore = new Mock<IUserStore<ApplicationUser>>();
            var applicationUserManager = new Mock<ApplicationUserManager>(userStore.Object);

			var service = new AuthorizationService(applicationUserManager.Object, dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

			// Act
			var results = service.GetUserRoles(claimsIdentityMock.Object);

            // Assert
            results.Count().Should().Be(2);
            results.Contains("Director");
        }

		[TestMethod, TestCategory("Authorization"), TestCategory("Service")]
		public void UpdatePrivilegeClaimsOnIdentity_WhenModifiedClaimIdentityAsParameter_ShouldUpdateTheModifiedClaimIdentity()
		{
			// Arrange
			var claims = new List<System.Security.Claims.Claim>()
			{
				new System.Security.Claims.Claim("Privilege","AM1"),
				new System.Security.Claims.Claim("Privilege","AM1"),
				new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role,"Assessor"),
				new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role,"Assessor", "test", "test", "test")
			};
			var applicationRoles = new List<ApplicationRole>()
			{
				new ApplicationRole()
				{
					Name = "Assessor",
					ApplicationClaims = new List<ApplicationClaim>()
					{
						new ApplicationClaim() { ClaimType = "Privilege", ClaimValue = "AM1"}
					}
				 }
			};
			var loggerMock = new Mock<ILogger>();
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
			var helper = new ServiceHelper(typeof(AuthenticationService), identity);
			var dbContextMock = new Mock<IDataContext>();
			var httpContextMock = new Mock<HttpContextBase>();
			var userManager = new Mock<IUserManagerAdapter>();
			var userStore = new Mock<IUserStore<ApplicationUser>>();
			var applicationUserManager = new Mock<ApplicationUserManager>(userStore.Object);
			var intUser = new InternalUser() { IDIR = "Brock", Id = 1, FirstName = "A", LastName = "B", Email = "a@a.com" };
			var dbSetMock = helper.MockDbSet(intUser);
			var dbSetMockAppRoles = helper.MockDbSet(applicationRoles);
			dbContextMock.Setup(x => x.InternalUsers).Returns(dbSetMock.Object);
			dbContextMock.Setup(x => x.ApplicationRoles).Returns(dbSetMockAppRoles.Object);
			claims.Add(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "Brock"));
			claims.Add(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, "brockallen@gmail.com"));
			var claimsUser = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
			
			var service = new AuthorizationService(applicationUserManager.Object, dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

			// Act
			service.UpdatePrivilegeClaimsOnIdentity(claimsUser);

			// Assert
			var internalUser = claimsUser.Claims.ToList();
			Assert.AreEqual(internalUser.Count(), 11);
			Assert.IsNotNull(internalUser.Where(x => x.Type == AppClaimTypes.UserId && x.Value == intUser.Id.ToString()).First());
			Assert.IsNotNull(internalUser.Where(x => x.Type == System.Security.Claims.ClaimTypes.GivenName && x.Value == intUser.FirstName).First());
			Assert.IsNotNull(internalUser.Where(x => x.Type == System.Security.Claims.ClaimTypes.Surname && x.Value == intUser.LastName).First());
			Assert.IsNotNull(internalUser.Where(x => x.Type == System.Security.Claims.ClaimTypes.Email && x.Value == intUser.Email).First());
		}
	}
}

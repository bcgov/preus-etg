using System.Linq;
using System.Web.Mvc;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Areas.Ext.Controllers;
using CJG.Web.External.Areas.Ext.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CJG.Testing.Core;
using CJG.Infrastructure.Repositories;
using CJG.Core.Interfaces.Repository;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Helpers;
using Moq;
using NLog;
using System;

namespace CJG.Testing.IntegrationTests.Controllers
{
    [TestClass]
    public class UserProfileControllerTests : TransactionalTestBase
    {
        #region Variables
        private static int _applicationAdministratorId;
        #endregion

        #region Methods
        protected ServiceHelper CreateHelper(User applicationAdministrator)
        {
            var helper = new ServiceHelper(typeof(UserProfileController), applicationAdministrator);
            var unitOfWork = helper.SetMockAs<UnitOfWork, IUnitOfWork>(this.Context).Object;
            helper.SetMockAs<Repository<User>, IRepository<User>>(this.Context);
            helper.SetMockAs<Repository<Organization>, IRepository<Organization>>(this.Context);
            helper.SetMockAs<Repository<NaIndustryClassificationSystem>, IRepository<NaIndustryClassificationSystem>>(this.Context);
            var bceIdRepositoryMock = helper.SetMock(new Mock<IBCeIDRepository>());
            bceIdRepositoryMock.Setup(m => m.GetBusinessUserInfo(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BCeIDAccountTypeCodes>())).Returns(applicationAdministrator);
            bceIdRepositoryMock.Setup(m => m.GetBusinessUserInfo(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<BCeIDAccountTypeCodes>())).Returns(applicationAdministrator);

            helper.SetMockAs<UserService, IUserService>();
            var controllerServiceMock = helper.SetMock(new Mock<IControllerService>());
            controllerServiceMock.Setup(m => m.UserService).Returns(helper.GetMock<IUserService>().Object);
            controllerServiceMock.Setup(m => m.SiteMinderService).Returns(helper.GetMock<ISiteMinderService>().Object);
            controllerServiceMock.Setup(m => m.StaticDataService).Returns(helper.GetMock<IStaticDataService>().Object);
            controllerServiceMock.Setup(m => m.Logger).Returns(helper.GetMock<ILogger>().Object);

            return helper;
        }

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            var dbContext = TransactionalTestBase.CreateContext();

            _applicationAdministratorId = dbContext.AddExternalUser().Id;

            dbContext.Dispose();
        }
        #endregion

        #region Tests
        [TestMethod, TestCategory("Controller"), TestCategory("Integration")]
        public void UserProfileController_ConfirmDetails_GET()
        {
            // Arrange
            var applicationAdministrator = this.Context.Users.Find(_applicationAdministratorId);
            var controller = CreateHelper(applicationAdministrator).CreateController<UserProfileController>();

            // Act
            var result = (User)((ViewResult)controller.ConfirmDetails()).Model;

            // Assert
            result.Should().NotBe(null);
        }

        [TestMethod, TestCategory("Controller"), TestCategory("Integration")]
        public void UserProfileController_CreateUserProfile_POST()
        {
            // Arrange
            //var applicationAdministrator = this.Context.CreateExternalUser();
            //var helper = CreateHelper(applicationAdministrator);
            //var controller = helper.CreateController<UserProfileController>();

            //// Act
            //var model = (GrantProgramPreferencesViewModel)((ViewResult)controller.CreateUserProfile()).Model;
            //model.Position = "TEST";
            //model.ContactNumberSection1 = "123";
            //model.ContactNumberSection2 = "456";
            //model.ContactNumberSection3 = "7890";
            //model.PhysicalAddressLine1 = "123 test st.";
            //model.PhysicalCity = "TestCity";
            //model.PhysicalRegionId = "BC";
            //model.PhysicalPostalCode = "A1B2C3";
            //var result = ((RedirectToRouteResult)controller.CreateUserProfile(model)).RouteValues;

            //// Assert
            //result.Should().NotBeEmpty();

            //// Verify that applicationAdministrator was redirected to create organization profile
            //result["action"].Should().Be("CreateOrganizationProfile");
            //result["controller"].Should().Be("OrganizationProfile");

            //// Verify that applicationAdministrator profile was updated in db
            //var updatedUser = this.Context.Users.Where(x => x.BCeIDGuid == applicationAdministrator.BCeIDGuid).Single();
            //updatedUser.Should().NotBeNull();
            //updatedUser.JobTitle.Should().Be("TEST");
            //updatedUser.PhoneNumber.Should().Be("(123) 456-7890");
        }

        [TestMethod, TestCategory("Controller"), TestCategory("Integration")]
        public void UserProfileController_EditUserProfile_GET()
        {
            // Arrange
            //var applicationAdministrator = this.Context.Users.Find(_applicationAdministratorId);
            //var controller = CreateHelper(applicationAdministrator).CreateController<UserProfileController>();

            //// Act
            //var result = (GrantProgramPreferencesViewModel)((ViewResult)controller.EditUserProfile()).Model;

            //// Assert
            //result.Should().NotBe(null);
            //result.UserInfo.Should().NotBe(null);
            //result.UserInfo.FirstName.Should().Be(applicationAdministrator.FirstName);
            //result.UserInfo.LastName.Should().Be(applicationAdministrator.LastName);
        }

        [TestMethod, TestCategory("Controller"), TestCategory("Integration")]
        public void UserProfileController_EditUserProfile_POST()
        {
            // Arrange
            //var applicationAdministrator = this.Context.Users.Find(_applicationAdministratorId);
            //var helper = CreateHelper(applicationAdministrator);
            //var controller = helper.CreateController<UserProfileController>();
            //controller.TempData["BackURL"] = "/Ext/Home/";

            //// Act
            //var model = (GrantProgramPreferencesViewModel)((ViewResult)controller.UpdateUserProfileView()).Model;
            //model.Position = "TEST";
            //model.ContactNumberSection1 = "123";
            //model.ContactNumberSection2 = "456";
            //model.ContactNumberSection3 = "7890";
            //model.PhysicalAddressLine1 = "123 test st.";
            //model.PhysicalCity = "TestCity";
            //model.PhysicalRegionId = "BC";
            //model.PhysicalPostalCode = "A1B2C3";
            //model.MailingAddressSameAsBusiness = true;
            //var url = ((RedirectResult)controller.UpdateUserProfileView(model)).Url;

            //// Assert
            //url.Should().Be("/Ext/Home/");

            //var foundUser = this.Context.Users.Find(applicationAdministrator.Id);
            //foundUser.Should().NotBeNull();
            //foundUser.JobTitle.Should().Be("TEST");
            //foundUser.PhoneNumber.Should().Be("(123) 456-7890");
        }
        #endregion
    }
}
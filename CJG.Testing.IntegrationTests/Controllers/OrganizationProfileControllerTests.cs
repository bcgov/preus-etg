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

namespace CJG.Testing.IntegrationTests.Controllers
{
    [TestClass]
    public class OrganizationProfileControllerTests : TransactionalTestBase
    {
        #region Variables
        private static int _applicationAdministratorId;
        #endregion

        #region Methods
        protected ServiceHelper CreateHelper(User applicationAdministrator)
        {
            var helper = new ServiceHelper(typeof(OrganizationProfileController), applicationAdministrator);
            var unitOfWork = helper.SetMockAs<UnitOfWork, IUnitOfWork>(this.Context).Object;
            helper.SetMockAs<Repository<User>, IRepository<User>>(this.Context);
            helper.SetMockAs<Repository<Organization>, IRepository<Organization>>(this.Context);
            helper.SetMockAs<Repository<NaIndustryClassificationSystem>, IRepository<NaIndustryClassificationSystem>>(this.Context);

            helper.SetMockAs<UserService, IUserService>();
            helper.SetMockAs<OrganizationService, IOrganizationService>();
            helper.SetMockAs<NaIndustryClassificationSystemService, INaIndustryClassificationSystemService>();
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
        public void OrganizationProfileController_CreateOrganizationProfile_GET()
        {
            // Arrange
            var applicationAdministrator = this.Context.Users.Find(_applicationAdministratorId);
            var controller = CreateHelper(applicationAdministrator).CreateController<OrganizationProfileController>();

            // Act
            var result = (OrganizationProfileViewModel)((ViewResult)controller.CreateOrganizationProfile()).Model;

            // Assert
            result.Should().NotBeNull();
            result.Organization.LegalStructureId.Should().Be(applicationAdministrator.Organization.LegalStructureId);
            result.Organization.YearEstablished.Should().Be(applicationAdministrator.Organization.YearEstablished);
            result.Organization.NumberOfEmployeesWorldwide.Should().Be(applicationAdministrator.Organization.NumberOfEmployeesWorldwide);
            result.Organization.NumberOfEmployeesInBC.Should().Be(applicationAdministrator.Organization.NumberOfEmployeesInBC);
            result.Organization.AnnualTrainingBudget.Should().Be(applicationAdministrator.Organization.AnnualTrainingBudget);
            result.Organization.AnnualEmployeesTrained.Should().Be(applicationAdministrator.Organization.AnnualEmployeesTrained);
            result.Organization.OrganizationTypeId.Should().Be(applicationAdministrator.Organization.OrganizationTypeId);
            result.IsOrganizationProfileAdministrator.Should().Be(true);
        }

        [TestMethod, TestCategory("Controller"), TestCategory("Integration")]
        public void OrganizationProfileController_CreateOrganizationProfile_POST()
        {
            // Arrange
            var applicationAdministrator = this.Context.Users.Find(_applicationAdministratorId);
            var helper = CreateHelper(applicationAdministrator);
            var controller = helper.CreateController<OrganizationProfileController>();
            applicationAdministrator.Organization.YearEstablished = 1998;
            var model = new OrganizationProfileViewModel(applicationAdministrator, applicationAdministrator.Organization);

            // Act
            var result = ((RedirectToRouteResult)controller.CreateOrganizationProfile(model));

            // Assert
            result.Should().NotBeNull();

            var organization = helper.GetMock<IUnitOfWork>().Object.Organizations.Where(x => x.BCeIDGuid == applicationAdministrator.Organization.BCeIDGuid).Single();
            organization.YearEstablished.Should().Be(model.Organization.YearEstablished);
        }


        [TestMethod, TestCategory("Controller"), TestCategory("Integration")]
        public void OrganizationProfileController_EditOrganizationProfile_GET()
        {
            // Arrange
            var applicationAdministrator = this.Context.Users.Find(_applicationAdministratorId);
            var controller = CreateHelper(applicationAdministrator).CreateController<OrganizationProfileController>();

            // Act
            var result = (OrganizationProfileViewModel)((ViewResult)controller.EditOrganizationProfile()).Model;

            // Assert
            result.Should().NotBeNull();
            result.Organization.LegalStructureId.Should().Be(applicationAdministrator.Organization.LegalStructureId);
            result.Organization.YearEstablished.Should().Be(applicationAdministrator.Organization.YearEstablished);
            result.Organization.NumberOfEmployeesWorldwide.Should().Be(applicationAdministrator.Organization.NumberOfEmployeesWorldwide);
            result.Organization.NumberOfEmployeesInBC.Should().Be(applicationAdministrator.Organization.NumberOfEmployeesInBC);
            result.Organization.AnnualTrainingBudget.Should().Be(applicationAdministrator.Organization.AnnualTrainingBudget);
            result.Organization.AnnualEmployeesTrained.Should().Be(applicationAdministrator.Organization.AnnualEmployeesTrained);
            result.Organization.OrganizationTypeId.Should().Be(applicationAdministrator.Organization.OrganizationTypeId);
        }

        [TestMethod, TestCategory("Controller"), TestCategory("Integration")]
        public void OrganizationProfileController_EditOrganizationProfile_POST()
        {
            // Arrange
            var applicationAdministrator = this.Context.Users.Find(_applicationAdministratorId);
            var helper = CreateHelper(applicationAdministrator);
            var controller = helper.CreateController<OrganizationProfileController>();
            controller.TempData["BackURL"] = "/Ext/Home/";

            applicationAdministrator.Organization.YearEstablished = 1998;
            var model = new OrganizationProfileViewModel(applicationAdministrator, applicationAdministrator.Organization);

            // Act
            var url = ((RedirectResult)controller.EditOrganizationProfile(model)).Url;

            // Assert
            url.Should().BeEquivalentTo("/Ext/Home/");
            var organization = helper.GetMock<IUnitOfWork>().Object.Organizations.Where(x => x.BCeIDGuid == applicationAdministrator.Organization.BCeIDGuid).Single();
            organization.YearEstablished.Should().Be(model.Organization.YearEstablished);
        }

        [TestMethod, TestCategory("Controller"), TestCategory("Integration")]
        public void OrganizationProfileController_NaicsCodeSelected_POST()
        {
            // Arrange
            var applicationAdministrator = this.Context.Users.Find(_applicationAdministratorId);
            var helper = CreateHelper(applicationAdministrator);
            var controller = helper.CreateController<OrganizationProfileController>();

            // Act
            var json = (JsonResult)controller.NaicsCodeSelected(1, 0);

            // Assert

            ((OrganizationProfileViewModel) json.Data).NaicsLevel1Codes.Should().NotBeNullOrEmpty();

            var level1 = ((OrganizationProfileViewModel) json.Data).NaicsLevel1Codes;

            json = (JsonResult)controller.NaicsCodeSelected(2, level1.First(x=>x.Key.HasValue).Key.Value);

            ((OrganizationProfileViewModel)json.Data).NaicsLevel2Codes.Should().NotBeNullOrEmpty();

            var level2 = ((OrganizationProfileViewModel)json.Data).NaicsLevel2Codes;

            json = (JsonResult)controller.NaicsCodeSelected(3, level2.First(x => x.Key.HasValue).Key.Value);

            ((OrganizationProfileViewModel)json.Data).NaicsLevel3Codes.Should().NotBeNullOrEmpty();
            
        }
        #endregion
    }
}

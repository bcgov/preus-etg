using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Int.Controllers;
using CJG.Web.External.Areas.Int.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static CJG.Testing.Core.ServiceHelper;

namespace CJG.Testing.UnitTests.Controllers.Int
{
	[TestClass]
	public class ApplicantControllerTests
	{
		#region GetApplicant
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicantController))]
		public void GetApplicant()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicantController>(user, Roles.Assessor);
			var controller = helper.Create();

			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(user, ApplicationStateInternal.AgreementAccepted);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);

			var naIndustryClassificationSystems = new [] { EntityHelper.CreateNaIndustryClassificationSystem() };
			helper.GetMock<INaIndustryClassificationSystemService>().Setup(m => m.GetNaIndustryClassificationSystems(It.IsAny<int?>())).Returns(naIndustryClassificationSystems);

			// Act
			var result = controller.GetApplicant(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicantViewModel>();
			var data = result.Data as ApplicantViewModel;
			data.Id.Should().Be(grantApplication.Id);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<INaIndustryClassificationSystemService>().Verify(m => m.GetNaIndustryClassificationSystems(It.IsAny<int?>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicantController))]
		public void GetApplicant_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicantController>(user, Roles.Assessor);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetApplicant(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region GetCommunities
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicantController))]
		public void GetCommunities()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicantController>(user, Roles.Assessor);
			var controller = helper.Create();

			var communities = new[] { EntityHelper.CreateCommunity("test") };
			helper.GetMock<ICommunityService>().Setup(m => m.GetAll()).Returns(communities);

			// Act
			var result = controller.GetCommunities();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<KeyValuePair<int, string>[]>();
			var data = result.Data as IEnumerable<KeyValuePair<int, string>>;
			data.Count().Should().Be(1);
			data.First().Key.Should().Be(1);
			helper.GetMock<ICommunityService>().Verify(m => m.GetAll(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicantController))]
		public void GetCommunities_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicantController>(user, Roles.Assessor);

			helper.GetMock<ICommunityService>().Setup(m => m.GetAll()).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetCommunities();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region GetNAICS
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicantController))]
		public void GetNAICS()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicantController>(user, Roles.Assessor);
			var controller = helper.Create();

			var naics = new[] { EntityHelper.CreateNaIndustryClassificationSystem(1, "test", 1) };
			helper.GetMock<INaIndustryClassificationSystemService>().Setup(m => m.GetNaIndustryClassificationSystemChildren(It.IsAny<int>(), It.IsAny<int>())).Returns(naics);

			// Act
			var result = controller.GetNAICS(1, null);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			var data = result.Data as IEnumerable<object>;
			var item = data.First();
			item.GetReflectedProperty("Key").Should().Be(1);
			item.GetReflectedProperty("Code").Should().Be("test");
			helper.GetMock<INaIndustryClassificationSystemService>().Verify(m => m.GetNaIndustryClassificationSystemChildren(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicantController))]
		public void GetNAICS_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicantController>(user, Roles.Assessor);

			helper.GetMock<INaIndustryClassificationSystemService>().Setup(m => m.GetNaIndustryClassificationSystemChildren(It.IsAny<int>(), It.IsAny<int>())).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetNAICS(1, null);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region UpdateApplicant
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicantController))]
		public void UpdateApplicant()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicantController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IStaticDataService>().Setup(m => m.GetCountry(It.IsAny<string>())).Returns(EntityHelper.CreateCountry("CA"));
			helper.GetMock<IStaticDataService>().Setup(m => m.GetRegion(It.IsAny<string>(), It.IsAny<string>())).Returns(EntityHelper.CreateRegion("Victoria"));
			var mockNaIndustryClassificationSystemService = helper.GetMock<INaIndustryClassificationSystemService>();
			var viewModel = new ApplicantViewModel(grantApplication, mockNaIndustryClassificationSystemService.Object)
			{
				NAICSLevel1Id = grantApplication.NAICSId
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateApplicant(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicantViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), ApplicationWorkflowTrigger.EditApplicant, null), Times.Once);
			var model = result.Data as ApplicantViewModel;
			model.Id.Should().Be(grantApplication.Id);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicantController))]
		public void UpdateApplicant_InValidModel()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicantController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IStaticDataService>().Setup(m => m.GetCountry(It.IsAny<string>())).Returns(EntityHelper.CreateCountry("CA"));
			helper.GetMock<IStaticDataService>().Setup(m => m.GetRegion(It.IsAny<string>(), It.IsAny<string>())).Returns(EntityHelper.CreateRegion("Victoria"));
			var mockNaIndustryClassificationSystemService = helper.GetMock<INaIndustryClassificationSystemService>();
			var viewModel = new ApplicantViewModel()
			{
				BusinessLicenseNumber = "123456789012345678901"
			};
			var controller = helper.CreateWithModel(viewModel);

			// Act
			var result = controller.UpdateApplicant(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicantViewModel>();
			var validationErrors = (result.Data as ApplicantViewModel).ValidationErrors;
			validationErrors.Count().Should().Be(16);
			validationErrors.Any(l => l.Key == "OrganizationLegalName").Should().BeTrue();
			validationErrors.Any(l => l.Key == "OrganizationYearEstablished").Should().BeTrue();
			validationErrors.Any(l => l.Key == "OrganizationNumberOfEmployeesWorldwide").Should().BeTrue();
			validationErrors.Any(l => l.Key == "OrganizationNumberOfEmployeesBC").Should().BeTrue();
			validationErrors.Any(l => l.Key == "OrganizationAnnualTrainingBudget").Should().BeTrue();
			validationErrors.Any(l => l.Key == "OrganizationAnnualEmployeesTrained").Should().BeTrue();
			validationErrors.Any(l => l.Key == "BusinessLicenseNumber").Should().BeTrue();
			validationErrors.Any(l => l.Key == "AddressLine1").Should().BeTrue();
			validationErrors.Any(l => l.Key == "City").Should().BeTrue();
			validationErrors.Any(l => l.Key == "Region").Should().BeTrue();
			validationErrors.Any(l => l.Key == "PostalCode").Should().BeTrue();
			validationErrors.Any(l => l.Key == "Country").Should().BeTrue();
			validationErrors.Any(l => l.Key == "NAICSLevel1Id").Should().BeTrue();
			validationErrors.Any(l => l.Key == "NAICSLevel2Id").Should().BeTrue();
			validationErrors.Any(l => l.Key == "NAICSLevel3Id").Should().BeTrue();

			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), ApplicationWorkflowTrigger.EditApplicant, null), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicantController))]
		public void UpdateApplicant_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicantController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();

			var mockNaIndustryClassificationSystemService = helper.GetMock<INaIndustryClassificationSystemService>();
			var viewModel = new ApplicantViewModel(grantApplication, mockNaIndustryClassificationSystemService.Object);
			var controller = helper.Create();

			// Act
			var result = controller.UpdateApplicant(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicantViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion
	}
}

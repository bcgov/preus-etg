using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Ext.Controllers;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Areas.Ext.Models.TrainingProviders;
using CJG.Web.External.Models.Shared.SkillsTrainings;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CJG.Testing.UnitTests.Controllers.Ext
{
	[TestClass]
	public class ServiceProviderControllerTest
	{
		#region ServiceProviderView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ServiceProviderController))]
		public void ServiceProviderView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ServiceProviderController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.ServiceProviderView(grantApplication.Id, eligibleExpenseType.Id, trainingProvider.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			eligibleExpenseType.Id.Should().Be(controller.ViewBag.EligibleExpenseTypeId);
			trainingProvider.Id.Should().Be(controller.ViewBag.TrainingProviderId);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ServiceProviderController))]
		public void ServiceProviderController_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ServiceProviderController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NotAuthorizedException>(() => controller.ServiceProviderView(1, 1, 1));
		}
		#endregion

		#region GetServiceProvider
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ServiceProviderController))]
		public void GetServiceProvider()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ServiceProviderController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			helper.GetMock<IEligibleExpenseTypeService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(eligibleExpenseType);
			var controller = helper.Create();

			// Act
			var result = controller.GetServiceProvider(0, grantApplication.Id, eligibleExpenseType.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingServiceProviderViewModel>();
			var model = result.Data as TrainingServiceProviderViewModel;
			model.GrantApplicationId.Should().Be(grantApplication.Id);
			model.ServiceProvider.EligibleExpenseTypeId.Should().Be(eligibleExpenseType.Id);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IEligibleExpenseTypeService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ServiceProviderController))]
		public void GetServiceProvider_TrainingProvider()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ServiceProviderController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProvider);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			helper.GetMock<IEligibleExpenseTypeService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(eligibleExpenseType);
			var controller = helper.Create();

			// Act
			var result = controller.GetServiceProvider(trainingProvider.Id, grantApplication.Id, eligibleExpenseType.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingServiceProviderViewModel>();
			var model = result.Data as TrainingServiceProviderViewModel;
			model.GrantApplicationId.Should().Be(grantApplication.Id);
			model.ServiceProvider.EligibleExpenseTypeId.Should().Be(eligibleExpenseType.Id);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<IEligibleExpenseTypeService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ServiceProviderController))]
		public void GetServiceProvider_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ServiceProviderController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetServiceProvider(0, 1, 1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingServiceProviderViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IEligibleExpenseTypeService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ServiceProviderController))]
		public void GetApplicationDetails_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ServiceProviderController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NoContentException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetServiceProvider(0, 1, 1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingServiceProviderViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IEligibleExpenseTypeService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region UpdateServiceProvider
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ServiceProviderController))]
		public void UpdateServiceProvider()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ServiceProviderController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("test");
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProvider);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetTrainingProviderType(It.IsAny<int>()))
				.Returns(trainingProviderType);
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();
			var serviceProviderViewModel = new ServiceProviderDetailsViewModel()
			{
				Id = 1,
				EligibleExpenseTypeId = eligibleExpenseType.Id,
				TrainingProviderTypeId = 1,
				Caption = eligibleExpenseType.Caption,
				ContactPhoneAreaCode = "604",
				ContactPhoneExchange = "123",
				ContactPhoneNumber = "4567",
				Name = "test",
				ChangeRequestReason = "reason",
				GrantApplicationId = grantApplication.Id,
				TrainingProviderInventory = trainingProviderInventory,
				TrainingProviderInventoryId = trainingProviderInventory.Id,
				PrivateSectorValidationType = TrainingProviderPrivateSectorValidationTypes.Never,
				ContactFirstName = "first name",
				ContactLastName = "Last Name",
				ContactEmail = "test@dfad.ca",
				AddressLine1 = "address",
				PostalCode = "V3V3V3",
				City = "city",
				RowVersion = "AgQGCAoMDhASFA==",
			};
			var viewModel = new TrainingServiceProviderViewModel()
			{
				ServiceProvider = serviceProviderViewModel,
				GrantApplicationId = grantApplication.Id,
				EligibleCostBreakdownRowVersion = "AgQGCAoMDhASFA ==",
				RowVersion = "AgQGCAoMDhASFA==",
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateServiceProvider(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(viewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingServiceProviderViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Update(It.IsAny<TrainingProvider>()), Times.Once);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderType(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ServiceProviderController))]
		public void UpdateServiceProvider_InvalidModel()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ServiceProviderController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("test");
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProvider);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetTrainingProviderType(It.IsAny<int>()))
				.Returns(trainingProviderType);
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();
			var serviceProviderViewModel = new ServiceProviderDetailsViewModel()
			{
				Id = 1,
				RowVersion = "AgQGCAoMDhASFA==",
			};
			var viewModel = new TrainingServiceProviderViewModel()
			{
				ServiceProvider = serviceProviderViewModel,
				GrantApplicationId = grantApplication.Id,
				EligibleCostBreakdownRowVersion = "AgQGCAoMDhASFA ==",
				RowVersion = "AgQGCAoMDhASFA==",
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateServiceProvider(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(viewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingServiceProviderViewModel>();
			var validErrors = (result.Data as TrainingServiceProviderViewModel).ValidationErrors;
			validErrors.Count().Should().Be(9);
			validErrors.Any(l => l.Key == "Name").Should().BeTrue();
			validErrors.Any(l => l.Key == "TrainingProviderTypeId").Should().BeTrue();
			validErrors.Any(l => l.Key == "ContactFirstName").Should().BeTrue();
			validErrors.Any(l => l.Key == "ContactLastName").Should().BeTrue();
			validErrors.Any(l => l.Key == "ContactEmail").Should().BeTrue();
			validErrors.Any(l => l.Key == "ContactPhone").Should().BeTrue();
			validErrors.Any(l => l.Key == "City").Should().BeTrue();
			validErrors.Any(l => l.Key == "AddressLine1").Should().BeTrue();
			validErrors.Any(l => l.Key == "PostalCode").Should().BeTrue();

			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Update(It.IsAny<TrainingProvider>()), Times.Never);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderType(It.IsAny<int>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ServiceProviderController))]
		public void UpdateServiceProvider_InvalidModel_IsCanadianAddress()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ServiceProviderController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("test");
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProvider);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetTrainingProviderType(It.IsAny<int>()))
				.Returns(trainingProviderType);
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();
			var serviceProviderViewModel = new ServiceProviderDetailsViewModel()
			{
				Id = 1,
				RowVersion = "AgQGCAoMDhASFA==",
				IsCanadianAddress = false
			};
			var viewModel = new TrainingServiceProviderViewModel()
			{
				ServiceProvider = serviceProviderViewModel,
				GrantApplicationId = grantApplication.Id,
				EligibleCostBreakdownRowVersion = "AgQGCAoMDhASFA ==",
				RowVersion = "AgQGCAoMDhASFA==",
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateServiceProvider(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(viewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingServiceProviderViewModel>();
			var validErrors = (result.Data as TrainingServiceProviderViewModel).ValidationErrors;
			validErrors.Count().Should().Be(10);
			validErrors.Any(l => l.Key == "Name").Should().BeTrue();
			validErrors.Any(l => l.Key == "TrainingProviderTypeId").Should().BeTrue();
			validErrors.Any(l => l.Key == "ContactFirstName").Should().BeTrue();
			validErrors.Any(l => l.Key == "ContactLastName").Should().BeTrue();
			validErrors.Any(l => l.Key == "ContactEmail").Should().BeTrue();
			validErrors.Any(l => l.Key == "ContactPhone").Should().BeTrue();
			validErrors.Any(l => l.Key == "City").Should().BeTrue();
			validErrors.Any(l => l.Key == "AddressLine1").Should().BeTrue();
			validErrors.Any(l => l.Key == "PostalCode").Should().BeFalse();
			validErrors.Any(l => l.Key == "OtherZipCode").Should().BeTrue();
			validErrors.Any(l => l.Key == "OtherRegion").Should().BeTrue();

			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Add(It.IsAny<TrainingProvider>()), Times.Never);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderType(It.IsAny<int>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ServiceProviderController))]
		public void UpdateServiceProvider_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ServiceProviderController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();

			var viewModel = new TrainingServiceProviderViewModel()
			{
				GrantApplicationId = grantApplication.Id,
				EligibleCostBreakdownRowVersion = "AgQGCAoMDhASFA ==",
				RowVersion = "AgQGCAoMDhASFA==",
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateServiceProvider(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(viewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingServiceProviderViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Update(It.IsAny<TrainingProvider>()), Times.Never);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderType(It.IsAny<int>()), Times.Never);
		}
		#endregion;

		#region AddServiceProvider
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ServiceProviderController))]
		public void AddServiceProvider()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ServiceProviderController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("test");
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProvider);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetTrainingProviderType(It.IsAny<int>()))
				.Returns(trainingProviderType);
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();
			var serviceProviderViewModel = new ServiceProviderDetailsViewModel()
			{
				Id = 1,
				EligibleExpenseTypeId = eligibleExpenseType.Id,
				TrainingProviderTypeId = 1,
				Caption = eligibleExpenseType.Caption,
				ContactPhoneAreaCode = "604",
				ContactPhoneExchange = "123",
				ContactPhoneNumber = "4567",
				Name = "test",
				ChangeRequestReason = "reason",
				GrantApplicationId = grantApplication.Id,
				TrainingProviderInventory = trainingProviderInventory,
				TrainingProviderInventoryId = trainingProviderInventory.Id,
				PrivateSectorValidationType = TrainingProviderPrivateSectorValidationTypes.Never,
				ContactFirstName = "first name",
				ContactLastName = "Last Name",
				ContactEmail = "test@dfad.ca",
				AddressLine1 = "address",
				PostalCode = "V3V3V3",
				City = "city",
				RowVersion = "AgQGCAoMDhASFA==",
			};
			var viewModel = new TrainingServiceProviderViewModel()
			{
				ServiceProvider = serviceProviderViewModel,
				GrantApplicationId = grantApplication.Id,
				EligibleCostBreakdownRowVersion = "AgQGCAoMDhASFA ==",
				RowVersion = "AgQGCAoMDhASFA==",
			};
			var controller = helper.Create();

			// Act
			var result = controller.AddServiceProvider(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(viewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingServiceProviderViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Add(It.IsAny<TrainingProvider>()), Times.Once);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderType(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ServiceProviderController))]
		public void AddServiceProvider_InvalidModel()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ServiceProviderController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("test");
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProvider);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetTrainingProviderType(It.IsAny<int>()))
				.Returns(trainingProviderType);
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();
			var serviceProviderViewModel = new ServiceProviderDetailsViewModel()
			{
				Id = 1,
				RowVersion = "AgQGCAoMDhASFA==",
			};
			var viewModel = new TrainingServiceProviderViewModel()
			{
				ServiceProvider = serviceProviderViewModel,
				GrantApplicationId = grantApplication.Id,
				EligibleCostBreakdownRowVersion = "AgQGCAoMDhASFA ==",
				RowVersion = "AgQGCAoMDhASFA==",
			};
			var controller = helper.Create();

			// Act
			var result = controller.AddServiceProvider(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(viewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingServiceProviderViewModel>();
			var validErrors = (result.Data as TrainingServiceProviderViewModel).ValidationErrors;
			validErrors.Count().Should().Be(9);
			validErrors.Any(l => l.Key == "Name").Should().BeTrue();
			validErrors.Any(l => l.Key == "TrainingProviderTypeId").Should().BeTrue();
			validErrors.Any(l => l.Key == "ContactFirstName").Should().BeTrue();
			validErrors.Any(l => l.Key == "ContactLastName").Should().BeTrue();
			validErrors.Any(l => l.Key == "ContactEmail").Should().BeTrue();
			validErrors.Any(l => l.Key == "ContactPhone").Should().BeTrue();
			validErrors.Any(l => l.Key == "City").Should().BeTrue();
			validErrors.Any(l => l.Key == "AddressLine1").Should().BeTrue();
			validErrors.Any(l => l.Key == "PostalCode").Should().BeTrue();
			validErrors.Any(l => l.Key == "OtherZipCode").Should().BeFalse();
			validErrors.Any(l => l.Key == "OtherRegion").Should().BeFalse();

			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Add(It.IsAny<TrainingProvider>()), Times.Never);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderType(It.IsAny<int>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ServiceProviderController))]
		public void AddServiceProvider_InvalidModel_IsCanadianAddress()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ServiceProviderController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("test");
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProvider);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetTrainingProviderType(It.IsAny<int>()))
				.Returns(trainingProviderType);
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();
			var serviceProviderViewModel = new ServiceProviderDetailsViewModel()
			{
				Id = 1,
				RowVersion = "AgQGCAoMDhASFA==",
				IsCanadianAddress = false
			};
			var viewModel = new TrainingServiceProviderViewModel()
			{
				ServiceProvider = serviceProviderViewModel,
				GrantApplicationId = grantApplication.Id,
				EligibleCostBreakdownRowVersion = "AgQGCAoMDhASFA ==",
				RowVersion = "AgQGCAoMDhASFA==",
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateServiceProvider(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(viewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingServiceProviderViewModel>();
			var validErrors = (result.Data as TrainingServiceProviderViewModel).ValidationErrors;
			validErrors.Count().Should().Be(10);
			validErrors.Any(l => l.Key == "Name").Should().BeTrue();
			validErrors.Any(l => l.Key == "TrainingProviderTypeId").Should().BeTrue();
			validErrors.Any(l => l.Key == "ContactFirstName").Should().BeTrue();
			validErrors.Any(l => l.Key == "ContactLastName").Should().BeTrue();
			validErrors.Any(l => l.Key == "ContactEmail").Should().BeTrue();
			validErrors.Any(l => l.Key == "ContactPhone").Should().BeTrue();
			validErrors.Any(l => l.Key == "City").Should().BeTrue();
			validErrors.Any(l => l.Key == "AddressLine1").Should().BeTrue();
			validErrors.Any(l => l.Key == "PostalCode").Should().BeFalse();
			validErrors.Any(l => l.Key == "OtherZipCode").Should().BeTrue();
			validErrors.Any(l => l.Key == "OtherRegion").Should().BeTrue();

			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Add(It.IsAny<TrainingProvider>()), Times.Never);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderType(It.IsAny<int>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ServiceProviderController))]
		public void AddServiceProvider_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ServiceProviderController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();

			var viewModel = new TrainingServiceProviderViewModel()
			{
				GrantApplicationId = grantApplication.Id,
				EligibleCostBreakdownRowVersion = "AgQGCAoMDhASFA ==",
				RowVersion = "AgQGCAoMDhASFA==",
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateServiceProvider(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(viewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingServiceProviderViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Update(It.IsAny<TrainingProvider>()), Times.Never);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderType(It.IsAny<int>()), Times.Never);
		}
		#endregion;

		#region GetTrainingProviderTypes
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ServiceProviderController))]
		public void GetTrainingProviderTypes()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ServiceProviderController>(user);

			var trainingProviderType = EntityHelper.CreateTrainingProviderType("test");
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetTrainingProviderTypes())
				.Returns(new List<TrainingProviderType>() { trainingProviderType });

			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingProviderTypes();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<TrainingProviderTypeViewModel>>();
			var model = result.Data as List<TrainingProviderTypeViewModel>;
			model[0].Id.Should().Be(trainingProviderType.Id);
			model[0].Caption.Should().Be(trainingProviderType.Caption); 
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderTypes(), Times.Once);
		}
		#endregion
	}
}

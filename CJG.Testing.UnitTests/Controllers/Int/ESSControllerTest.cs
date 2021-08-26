using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Int.Controllers;
using CJG.Web.External.Areas.Int.Models.ESS;
using CJG.Web.External.Models.Shared;
using CJG.Web.External.Models.Shared.SkillsTrainings;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static CJG.Testing.Core.ServiceHelper;

namespace CJG.Testing.UnitTests.Controllers.Int
{
	[TestClass]
	public class ESSControllerTest
	{

		#region GetServices
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ESSController))]
		public void GetServices()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ESSController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			helper.GetMock<IEligibleCostService>().Setup(m => m.Get(It.IsAny<int>())).Returns(eligibleCost);

			// Act
			var result = controller.GetServices(eligibleCost.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<EmploymentServicesViewModel>();
			var data = result.Data as EmploymentServicesViewModel;
			data.Id.Should().Be(eligibleCost.Id);
			helper.GetMock<IEligibleCostService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ESSController))]
		public void GetServices_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ESSController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IEligibleCostService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();

			// Act
			var result = controller.GetServices(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region GetServiceLines
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ESSController))]
		public void GetServiceLines()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ESSController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var eligibleExpenseBreakdown = EntityHelper.CreateEligibleExpenseBreakdown(eligibleExpenseType);
			helper.GetMock<IEligibleExpenseBreakdownService>()
				.Setup(m => m.GetAllForEligibleExpenseType(It.IsAny<int>()))
				.Returns(new List<EligibleExpenseBreakdown>() { eligibleExpenseBreakdown });

			// Act
			var result = controller.GetServiceLines(eligibleExpenseType.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<KeyValueListItem<int, string>[]>();
			var data = result.Data as KeyValueListItem<int, string>[];
			data[0].Key.Should().Be(eligibleExpenseBreakdown.Id);
			data[0].Value.Should().Be(eligibleExpenseBreakdown.Caption);
			helper.GetMock<IEligibleExpenseBreakdownService>()
				.Verify(m => m.GetAllForEligibleExpenseType(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ESSController))]
		public void GetServiceLines_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ESSController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();

			helper.GetMock<IEligibleExpenseBreakdownService>()
				.Setup(m => m.GetAllForEligibleExpenseType(It.IsAny<int>())).Throws<NoContentException>();

			// Act
			var result = controller.GetServiceLines(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region UpdateServices
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ESSController))]
		public void UpdateServices()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ESSController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var eligibleExpenseBreakdown = EntityHelper.CreateEligibleExpenseBreakdown(eligibleExpenseType);
			var eligibleCostBreakdown = EntityHelper.CreateEligibleCostBreakdown(eligibleCost, eligibleExpenseBreakdown);
			eligibleCost.Breakdowns.Add(eligibleCostBreakdown);
			helper.GetMock<IEligibleCostService>().Setup(m => m.Get(It.IsAny<int>())).Returns(eligibleCost);
			helper.GetMock<IEligibleExpenseBreakdownService>().Setup(m => m.Get(It.IsAny<int>()))
				.Returns( eligibleExpenseBreakdown);

			// Act
			var model = new EmploymentServicesViewModel()
			{
				Id = eligibleCost.Id,
				RowVersion = "AgQGCAoMDhASFA==",
				SelectedServiceLineIds = new List<int>() { 1, 2 },

			};
			var result = controller.UpdateServices(model);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<EmploymentServicesViewModel>();
			var data = result.Data as EmploymentServicesViewModel;
			data.Id.Should().Be(eligibleCost.Id);
			helper.GetMock<IEligibleCostService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IEligibleExpenseBreakdownService>()
				.Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(model.SelectedServiceLineIds.Count()));
			helper.GetMock<IGrantApplicationService>()
				.Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), It.IsAny<Func<ApplicationWorkflowTrigger, bool>>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ESSController))]
		public void UpdateServices_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ESSController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();

			helper.GetMock<IEligibleCostService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();

			// Act
			var model = new EmploymentServicesViewModel()
			{
				Id = 1,
				RowVersion = "AgQGCAoMDhASFA==",
				SelectedServiceLineIds = new List<int>() { 1, 2 },

			};
			var result = controller.UpdateServices(model);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region GetProvider
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ESSController))]
		public void GetProvider()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ESSController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			helper.GetMock<ITrainingProviderService>().Setup(m => m.Get(It.IsAny<int>())).Returns(trainingProvider);

			// Act
			var result = controller.GetProvider(trainingProvider.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ProviderViewModel>();
			var data = result.Data as ProviderViewModel;
			data.Id.Should().Be(trainingProvider.Id);
			data.Name.Should().Be(trainingProvider.Name);
			data.GrantApplicationId.Should().Be(grantApplication.Id);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ESSController))]
		public void GetProvider_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ESSController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<ITrainingProviderService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();

			// Act
			var result = controller.GetProvider(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region UpdateProvider
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ESSController))]
		public void UpdateProvider()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ESSController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.UnderAssessment;
			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			trainingProvider.TrainingProviderInventoryId = 1;
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("test");
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();
			helper.GetMock<IEligibleCostService>().Setup(m => m.Get(It.IsAny<int>())).Returns(eligibleCost);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<ITrainingProgramService>().Setup(m => m.Get(It.IsAny<int>())).Returns(trainingProgram);
			helper.GetMock<ITrainingProviderService>().Setup(m => m.Get(It.IsAny<int>())).Returns(trainingProvider);
			helper.GetMock<IStaticDataService>().Setup(m => m.GetTrainingProviderType(It.IsAny<int>()))
				.Returns(trainingProviderType);

			// Act
			var model = new ServiceProviderDetailsViewModel()
			{
				Id = eligibleCost.Id,
				RowVersion = "AgQGCAoMDhASFA==",
				EligibleCostId = eligibleCost.Id,
				Name = "Name Test",
				ChangeRequestReason = "ChangeRequestReason Test",
				GrantApplicationId = grantApplication.Id,
				TrainingProgramId = trainingProgram.Id,
				TrainingProviderTypeId = trainingProviderType.Id,
				TrainingAddressId = 1,
				TrainingProviderState = TrainingProviderStates.Complete,
				TrainingProviderInventoryId = trainingProviderInventory.Id,
				ContactFirstName = "first name",
				ContactLastName = "Last Name",
				ContactEmail = "test@dfad.ca",
				AddressLine1 = "address",
				ContactPhone = "6041234567",
				ContactPhoneAreaCode = "604",
				ContactPhoneExchange = "123",
				ContactPhoneNumber = "4567",
				City = "city",
				PostalCode = "V9C9C9",
				Caption = "ServiceProviderDetailsViewModel Caption",
				PrivateSectorValidationType = TrainingProviderPrivateSectorValidationTypes.Never,
				IsServiceProvider = true,
				EligibleExpenseTypeId = 1
			};
			var result = controller.UpdateProvider(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(model));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ProviderViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Update(It.IsAny<TrainingProvider>()), Times.Once);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderType(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ESSController))]
		public void UpdateProvider_InvalidModel()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ESSController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.UnderAssessment;
			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			trainingProvider.TrainingProviderInventoryId = 1;
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("test");
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();

			// Act
			var model = new ServiceProviderDetailsViewModel();
			var result = controller.UpdateProvider(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(model));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ProviderViewModel>();
			var validationErrors = (result.Data as ProviderViewModel).ValidationErrors;
			validationErrors.Count().Should().Be(9);
			validationErrors.Any(l => l.Key == "Name").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TrainingProviderTypeId").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactFirstName").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactLastName").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactEmail").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactPhone").Should().BeTrue();
			validationErrors.Any(l => l.Key == "AddressLine1").Should().BeTrue();
			validationErrors.Any(l => l.Key == "City").Should().BeTrue();
			validationErrors.Any(l => l.Key == "PostalCode").Should().BeTrue();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Update(It.IsAny<TrainingProvider>()), Times.Never);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderType(It.IsAny<int>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ESSController))]
		public void UpdateProvider_NotAuthorizedException()
		{
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ESSController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.UnderAssessment;
			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			trainingProvider.TrainingProviderInventoryId = 1;
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("test");
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			// Act
			var model = new ServiceProviderDetailsViewModel()
			{
				Id = eligibleCost.Id,
				RowVersion = "AgQGCAoMDhASFA==",
				EligibleCostId = eligibleCost.Id,
				Name = "Name Test",
				ChangeRequestReason = "ChangeRequestReason Test",
				GrantApplicationId = grantApplication.Id,
				TrainingProgramId = trainingProgram.Id,
				TrainingProviderTypeId = trainingProviderType.Id,
				TrainingAddressId = 1,
				TrainingProviderState = TrainingProviderStates.Complete,
				TrainingProviderInventoryId = trainingProviderInventory.Id,
				ContactFirstName = "first name",
				ContactLastName = "Last Name",
				ContactEmail = "test@dfad.ca",
				AddressLine1 = "address",
				ContactPhone = "6041234567",
				ContactPhoneAreaCode = "604",
				ContactPhoneExchange = "123",
				ContactPhoneNumber = "4567",
				City = "city",
				PostalCode = "V9C9C9",
				Caption = "ServiceProviderDetailsViewModel Caption",
				PrivateSectorValidationType = TrainingProviderPrivateSectorValidationTypes.Never,
				IsServiceProvider = true,
				EligibleExpenseTypeId = 1
			};
			var result = controller.UpdateProvider(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(model));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ProviderViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Update(It.IsAny<TrainingProvider>()), Times.Never);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderType(It.IsAny<int>()), Times.Never);
		}
		#endregion

		#region AddProvider
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ESSController))]
		public void AddProvider()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ESSController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.UnderAssessment;
			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			trainingProvider.TrainingProviderInventoryId = 1;
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("test");
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();
			helper.GetMock<IEligibleCostService>().Setup(m => m.Get(It.IsAny<int>())).Returns(eligibleCost);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<ITrainingProgramService>().Setup(m => m.Get(It.IsAny<int>())).Returns(trainingProgram);
			helper.GetMock<ITrainingProviderService>().Setup(m => m.Get(It.IsAny<int>())).Returns(trainingProvider);
			helper.GetMock<IStaticDataService>().Setup(m => m.GetTrainingProviderType(It.IsAny<int>()))
				.Returns(trainingProviderType);

			// Act
			var model = new ServiceProviderDetailsViewModel()
			{
				Id = eligibleCost.Id,
				RowVersion = "AgQGCAoMDhASFA==",
				EligibleCostId = eligibleCost.Id,
				Name = "Name Test",
				ChangeRequestReason = "ChangeRequestReason Test",
				GrantApplicationId = grantApplication.Id,
				TrainingProgramId = trainingProgram.Id,
				TrainingProviderTypeId = trainingProviderType.Id,
				TrainingAddressId = 1,
				TrainingProviderState = TrainingProviderStates.Complete,
				TrainingProviderInventoryId = trainingProviderInventory.Id,
				ContactFirstName = "first name",
				ContactLastName = "Last Name",
				ContactEmail = "test@dfad.ca",
				AddressLine1 = "address",
				ContactPhone = "6041234567",
				ContactPhoneAreaCode = "604",
				ContactPhoneExchange = "123",
				ContactPhoneNumber = "4567",
				City = "city",
				PostalCode = "V9C9C9",
				Caption = "ServiceProviderDetailsViewModel Caption",
				PrivateSectorValidationType = TrainingProviderPrivateSectorValidationTypes.Never,
				IsServiceProvider = true,
				EligibleExpenseTypeId = 1
			};
			var result = controller.AddProvider(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(model));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ProviderViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Add(It.IsAny<TrainingProvider>()), Times.Once);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderType(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ESSController))]
		public void AddProvider_InvalidModel()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ESSController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.UnderAssessment;
			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			trainingProvider.TrainingProviderInventoryId = 1;
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("test");
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();

			// Act
			var model = new ServiceProviderDetailsViewModel();
			var result = controller.AddProvider(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(model));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ProviderViewModel>();
			var validationErrors = (result.Data as ProviderViewModel).ValidationErrors;
			validationErrors.Count().Should().Be(9);
			validationErrors.Any(l => l.Key == "Name").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TrainingProviderTypeId").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactFirstName").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactLastName").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactEmail").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactPhone").Should().BeTrue();
			validationErrors.Any(l => l.Key == "AddressLine1").Should().BeTrue();
			validationErrors.Any(l => l.Key == "City").Should().BeTrue();
			validationErrors.Any(l => l.Key == "PostalCode").Should().BeTrue();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Add(It.IsAny<TrainingProvider>()), Times.Never);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderType(It.IsAny<int>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ESSController))]
		public void AddProvider_NotAuthorizedException()
		{
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ESSController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.UnderAssessment;
			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			trainingProvider.TrainingProviderInventoryId = 1;
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("test");
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			// Act
			var model = new ServiceProviderDetailsViewModel()
			{
				Id = eligibleCost.Id,
				RowVersion = "AgQGCAoMDhASFA==",
				EligibleCostId = eligibleCost.Id,
				Name = "Name Test",
				ChangeRequestReason = "ChangeRequestReason Test",
				GrantApplicationId = grantApplication.Id,
				TrainingProgramId = trainingProgram.Id,
				TrainingProviderTypeId = trainingProviderType.Id,
				TrainingAddressId = 1,
				TrainingProviderState = TrainingProviderStates.Complete,
				TrainingProviderInventoryId = trainingProviderInventory.Id,
				ContactFirstName = "first name",
				ContactLastName = "Last Name",
				ContactEmail = "test@dfad.ca",
				AddressLine1 = "address",
				ContactPhone = "6041234567",
				ContactPhoneAreaCode = "604",
				ContactPhoneExchange = "123",
				ContactPhoneNumber = "4567",
				City = "city",
				PostalCode = "V9C9C9",
				Caption = "ServiceProviderDetailsViewModel Caption",
				PrivateSectorValidationType = TrainingProviderPrivateSectorValidationTypes.Never,
				IsServiceProvider = true,
				EligibleExpenseTypeId = 1
			};
			var result = controller.AddProvider(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(model));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ProviderViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Update(It.IsAny<TrainingProvider>()), Times.Never);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderType(It.IsAny<int>()), Times.Never);
		}
		#endregion

		#region DeleteProvider
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ESSController))]
		public void DeleteProvider()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ESSController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			helper.GetMock<ITrainingProviderService>().Setup(m => m.Get(It.IsAny<int>())).Returns(trainingProvider);

			// Act
			var result = controller.DeleteProvider(trainingProvider.Id, Convert.ToBase64String(trainingProvider.RowVersion));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<BaseViewModel>();
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Delete(It.IsAny<TrainingProvider>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ESSController))]
		public void DeleteProvider_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ESSController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			helper.GetMock<ITrainingProviderService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();

			// Act
			var result = controller.DeleteProvider(trainingProvider.Id, Convert.ToBase64String(trainingProvider.RowVersion));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Delete(It.IsAny<TrainingProvider>()), Times.Never);
		}
		#endregion

		#region ApproveProvider
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ESSController))]
		public void ApproveProvider()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ESSController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			trainingProvider.TrainingProviderState = TrainingProviderStates.RequestApproved;
			helper.GetMock<ITrainingProviderService>().Setup(m => m.Get(It.IsAny<int>())).Returns(trainingProvider);

			var model = new ProviderViewModel()
			{
				Id = trainingProvider.Id,
				RowVersion = Convert.ToBase64String(trainingProvider.RowVersion),
			};
			// Act
			var result = controller.ApproveProvider(model);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ProviderViewModel>();
			var data = result.Data as ProviderViewModel;
			data.Id.Should().Be(trainingProvider.Id);
			data.TrainingProviderState.Should().Be(TrainingProviderStates.Requested);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Update(It.IsAny<TrainingProvider>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ESSController))]
		public void ApproveProvider_NotRequestAprroved()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ESSController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			helper.GetMock<ITrainingProviderService>().Setup(m => m.Get(It.IsAny<int>())).Returns(trainingProvider);

			var model = new ProviderViewModel()
			{
				Id = trainingProvider.Id,
				RowVersion = Convert.ToBase64String(trainingProvider.RowVersion),
			};
			// Act
			var result = controller.ApproveProvider(model);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ProviderViewModel>();
			var data = result.Data as ProviderViewModel;
			data.Id.Should().Be(trainingProvider.Id);
			data.TrainingProviderState.Should().Be(TrainingProviderStates.RequestApproved);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Update(It.IsAny<TrainingProvider>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ESSController))]
		public void ApproveProvider_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ESSController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			helper.GetMock<ITrainingProviderService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();

			var model = new ProviderViewModel()
			{
				Id = trainingProvider.Id,
				RowVersion = Convert.ToBase64String(trainingProvider.RowVersion),
				TrainingProviderState = TrainingProviderStates.RequestApproved
			};
			// Act
			var result = controller.ApproveProvider(model);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			var data = result.Data as ProviderViewModel;
			data.Id.Should().Be(trainingProvider.Id);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Update(It.IsAny<TrainingProvider>()), Times.Never);
		}
		#endregion

		#region DenyProvider
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ESSController))]
		public void DenyProvider()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ESSController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			trainingProvider.TrainingProviderState = TrainingProviderStates.RequestDenied;
			helper.GetMock<ITrainingProviderService>().Setup(m => m.Get(It.IsAny<int>())).Returns(trainingProvider);

			var model = new ProviderViewModel()
			{
				Id = trainingProvider.Id,
				RowVersion = Convert.ToBase64String(trainingProvider.RowVersion),
			};
			// Act
			var result = controller.DenyProvider(model);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ProviderViewModel>();
			var data = result.Data as ProviderViewModel;
			data.Id.Should().Be(trainingProvider.Id);
			data.TrainingProviderState.Should().Be(TrainingProviderStates.Requested);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Update(It.IsAny<TrainingProvider>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ESSController))]
		public void DenyProvider_NotRequestDenied()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ESSController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			helper.GetMock<ITrainingProviderService>().Setup(m => m.Get(It.IsAny<int>())).Returns(trainingProvider);

			var model = new ProviderViewModel()
			{
				Id = trainingProvider.Id,
				RowVersion = Convert.ToBase64String(trainingProvider.RowVersion),
			};
			// Act
			var result = controller.DenyProvider(model);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ProviderViewModel>();
			var data = result.Data as ProviderViewModel;
			data.Id.Should().Be(trainingProvider.Id);
			data.TrainingProviderState.Should().Be(TrainingProviderStates.RequestDenied);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Update(It.IsAny<TrainingProvider>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ESSController))]
		public void DenyProvider_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ESSController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			helper.GetMock<ITrainingProviderService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();

			var model = new ProviderViewModel()
			{
				Id = trainingProvider.Id,
				RowVersion = Convert.ToBase64String(trainingProvider.RowVersion),
				TrainingProviderState = TrainingProviderStates.RequestApproved
			};
			// Act
			var result = controller.DenyProvider(model);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			var data = result.Data as ProviderViewModel;
			data.Id.Should().Be(trainingProvider.Id);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Update(It.IsAny<TrainingProvider>()), Times.Never);
		}
		#endregion
	}
}

using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Ext.Controllers;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Areas.Ext.Models.TrainingProviders;
using CJG.Web.External.Areas.Ext.Models.Applications;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CJG.Testing.UnitTests.Controllers.Ext
{
	[TestClass]
	public class TrainingProviderControllerTest
	{

		#region TrainingProviderView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void TrainingProviderView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.TrainingProviderView(grantApplication.Id, trainingProvider.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			trainingProvider.Id.Should().Be(controller.ViewBag.TrainingProviderId);
		}
		#endregion

		#region GetTrainingProvider
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void GetTrainingProvider()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.TrainingPrograms = new List<TrainingProgram>();
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingProvider(grantApplication.Id, 0);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<Web.External.Areas.Ext.Models.TrainingProviders.TrainingProviderViewModel>();
			var model = result.Data as Web.External.Areas.Ext.Models.TrainingProviders.TrainingProviderViewModel;
			model.GrantApplicationId.Should().Be(grantApplication.Id);
			model.TrainingProviderState.Should().Be(TrainingProviderStates.Incomplete);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void GetTrainingProvider_ExistTrainingProgram()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProvider);
			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingProvider(grantApplication.Id, trainingProvider.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<Web.External.Areas.Ext.Models.TrainingProviders.TrainingProviderViewModel>();
			var model = result.Data as Web.External.Areas.Ext.Models.TrainingProviders.TrainingProviderViewModel;
			model.Id.Should().Be(trainingProvider.Id);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void GetTrainingProvider_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingProvider(1, 0);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<Web.External.Areas.Ext.Models.TrainingProviders.TrainingProviderViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void GetTrainingProvider_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NoContentException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingProvider(1, 1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<Web.External.Areas.Ext.Models.TrainingProviders.TrainingProviderViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
		}
		#endregion

		#region AddTrainingProvider
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void AddTrainingProvider()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("test");
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<ITrainingProgramService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProgram);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetTrainingProviderType(It.IsAny<int>()))
				.Returns(trainingProviderType);
			var trainingProviderViewModel = new Web.External.Areas.Ext.Models.TrainingProviders.TrainingProviderViewModel()
			{
				GrantApplicationRowVersion = "AgQGCAoMDhASFA==",
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
				TrainingOutsideBC = false
			};
			var controller = helper.Create();

			// Act
			var result = controller.AddTrainingProvider(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(trainingProviderViewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<Web.External.Areas.Ext.Models.TrainingProviders.TrainingProviderViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Add(It.IsAny<TrainingProvider>()), Times.Once);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderType(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void AddTrainingProvider_InvalidModel()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("test");
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<ITrainingProgramService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProgram);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetTrainingProviderType(It.IsAny<int>()))
				.Returns(trainingProviderType);
			var trainingProviderViewModel = new Web.External.Areas.Ext.Models.TrainingProviders.TrainingProviderViewModel()
			{
				GrantApplicationRowVersion = "AgQGCAoMDhASFA==",
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var controller = helper.Create();

			// Act
			var result = controller.AddTrainingProvider(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(trainingProviderViewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<Web.External.Areas.Ext.Models.TrainingProviders.TrainingProviderViewModel>();
			var validationErrors = (result.Data as Web.External.Areas.Ext.Models.TrainingProviders.TrainingProviderViewModel).ValidationErrors;
			validationErrors.Count().Should().Be(10);			
			validationErrors.Any(l => l.Key == "Name").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TrainingProviderTypeId").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactFirstName").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactLastName").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactEmail").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactPhone").Should().BeTrue();
			validationErrors.Any(l => l.Key == "AddressLine1").Should().BeTrue();
			validationErrors.Any(l => l.Key == "City").Should().BeTrue();
			validationErrors.Any(l => l.Key == "PostalCode").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TrainingOutsideBC").Should().BeTrue();
			validationErrors.Any(l => l.Key == "OtherZipCode").Should().BeFalse();
			validationErrors.Any(l => l.Key == "OtherRegion").Should().BeFalse();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Add(It.IsAny<TrainingProvider>()), Times.Never);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderType(It.IsAny<int>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void AddTrainingProvider_InvalidModel_NotCanadianAddress()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("test");
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<ITrainingProgramService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProgram);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetTrainingProviderType(It.IsAny<int>()))
				.Returns(trainingProviderType);
			var trainingProviderViewModel = new Web.External.Areas.Ext.Models.TrainingProviders.TrainingProviderViewModel()
			{
				GrantApplicationRowVersion = "AgQGCAoMDhASFA==",
				RowVersion = "AgQGCAoMDhASFA==",
				IsCanadianAddress = false
			};
			var controller = helper.Create();

			// Act
			var result = controller.AddTrainingProvider(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(trainingProviderViewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<Web.External.Areas.Ext.Models.TrainingProviders.TrainingProviderViewModel>();
			var validationErrors = (result.Data as Web.External.Areas.Ext.Models.TrainingProviders.TrainingProviderViewModel).ValidationErrors;
			validationErrors.Count().Should().Be(11);
			validationErrors.Any(l => l.Key == "Name").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TrainingProviderTypeId").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactFirstName").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactLastName").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactEmail").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactPhone").Should().BeTrue();
			validationErrors.Any(l => l.Key == "AddressLine1").Should().BeTrue();
			validationErrors.Any(l => l.Key == "City").Should().BeTrue();
			validationErrors.Any(l => l.Key == "PostalCode").Should().BeFalse();
			validationErrors.Any(l => l.Key == "TrainingOutsideBC").Should().BeTrue();
			validationErrors.Any(l => l.Key == "OtherZipCode").Should().BeTrue();
			validationErrors.Any(l => l.Key == "OtherRegion").Should().BeTrue();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Add(It.IsAny<TrainingProvider>()), Times.Never);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderType(It.IsAny<int>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void AddSkillsTraining_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();

			var trainingProviderViewModel = new Web.External.Areas.Ext.Models.TrainingProviders.TrainingProviderViewModel()
			{
				GrantApplicationRowVersion = "AgQGCAoMDhASFA==",
				RowVersion = "AgQGCAoMDhASFA==",
				EligibleCostId = 1,
				Name = "Name Test",
				ChangeRequestReason = "ChangeRequestReason Test",
				GrantApplicationId = 1,
				TrainingProgramId = 1,
				TrainingProviderTypeId = 1,
				TrainingAddressId = 1,
				TrainingProviderState = TrainingProviderStates.Complete,
				TrainingProviderInventoryId = 1,
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
				TrainingOutsideBC = false
			};
			var controller = helper.Create();

			// Act
			var result = controller.AddTrainingProvider(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(trainingProviderViewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<Web.External.Areas.Ext.Models.TrainingProviders.TrainingProviderViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion

		#region UpdateTrainingProvider
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void UpdateTrainingProvider()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("test");
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<ITrainingProgramService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProgram);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetTrainingProviderType(It.IsAny<int>()))
				.Returns(trainingProviderType);
			var trainingProviderViewModel = new Web.External.Areas.Ext.Models.TrainingProviders.TrainingProviderViewModel()
			{
				GrantApplicationRowVersion = "AgQGCAoMDhASFA==",
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
				TrainingOutsideBC = false
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateTrainingProvider(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(trainingProviderViewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<Web.External.Areas.Ext.Models.TrainingProviders.TrainingProviderViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Update(It.IsAny<TrainingProvider>()), Times.Once);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderType(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void UpdateTrainingProvider_InvalidModel()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("test");
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<ITrainingProgramService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProgram);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetTrainingProviderType(It.IsAny<int>()))
				.Returns(trainingProviderType);
			var trainingProviderViewModel = new Web.External.Areas.Ext.Models.TrainingProviders.TrainingProviderViewModel()
			{
				GrantApplicationRowVersion = "AgQGCAoMDhASFA==",
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateTrainingProvider(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(trainingProviderViewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<Web.External.Areas.Ext.Models.TrainingProviders.TrainingProviderViewModel>();
			var validationErrors = (result.Data as Web.External.Areas.Ext.Models.TrainingProviders.TrainingProviderViewModel).ValidationErrors;
			validationErrors.Count().Should().Be(10);
			validationErrors.Any(l => l.Key == "Name").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TrainingProviderTypeId").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactFirstName").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactLastName").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactEmail").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactPhone").Should().BeTrue();
			validationErrors.Any(l => l.Key == "AddressLine1").Should().BeTrue();
			validationErrors.Any(l => l.Key == "City").Should().BeTrue();
			validationErrors.Any(l => l.Key == "PostalCode").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TrainingOutsideBC").Should().BeTrue();
			validationErrors.Any(l => l.Key == "OtherZipCode").Should().BeFalse();
			validationErrors.Any(l => l.Key == "OtherRegion").Should().BeFalse();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Update(It.IsAny<TrainingProvider>()), Times.Never);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderType(It.IsAny<int>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void UpdateTrainingProvider_InvalidModel_NotCanadianAddress()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("test");
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<ITrainingProgramService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProgram);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetTrainingProviderType(It.IsAny<int>()))
				.Returns(trainingProviderType);
			var trainingProviderViewModel = new Web.External.Areas.Ext.Models.TrainingProviders.TrainingProviderViewModel()
			{
				GrantApplicationRowVersion = "AgQGCAoMDhASFA==",
				RowVersion = "AgQGCAoMDhASFA==",
				IsCanadianAddress = false
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateTrainingProvider(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(trainingProviderViewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<Web.External.Areas.Ext.Models.TrainingProviders.TrainingProviderViewModel>();
			var validationErrors = (result.Data as Web.External.Areas.Ext.Models.TrainingProviders.TrainingProviderViewModel).ValidationErrors;
			validationErrors.Count().Should().Be(11);
			validationErrors.Any(l => l.Key == "Name").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TrainingProviderTypeId").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactFirstName").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactLastName").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactEmail").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactPhone").Should().BeTrue();
			validationErrors.Any(l => l.Key == "AddressLine1").Should().BeTrue();
			validationErrors.Any(l => l.Key == "City").Should().BeTrue();
			validationErrors.Any(l => l.Key == "PostalCode").Should().BeFalse();
			validationErrors.Any(l => l.Key == "TrainingOutsideBC").Should().BeTrue();
			validationErrors.Any(l => l.Key == "OtherZipCode").Should().BeTrue();
			validationErrors.Any(l => l.Key == "OtherRegion").Should().BeTrue();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Update(It.IsAny<TrainingProvider>()), Times.Never);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderType(It.IsAny<int>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void UpdateTrainingProvider_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();

			var trainingProviderViewModel = new Web.External.Areas.Ext.Models.TrainingProviders.TrainingProviderViewModel()
			{
				GrantApplicationRowVersion = "AgQGCAoMDhASFA==",
				RowVersion = "AgQGCAoMDhASFA==",
				EligibleCostId = 1,
				Name = "Name Test",
				ChangeRequestReason = "ChangeRequestReason Test",
				GrantApplicationId = 1,
				TrainingProgramId = 1,
				TrainingProviderTypeId = 1,
				TrainingAddressId = 1,
				TrainingProviderState = TrainingProviderStates.Complete,
				TrainingProviderInventoryId = 1,
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
				TrainingOutsideBC = false
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateTrainingProvider(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(trainingProviderViewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<Web.External.Areas.Ext.Models.TrainingProviders.TrainingProviderViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion

		#region GetTrainingProviderTypes
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void GetTrainingProviderTypes()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
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
			model.Count().Should().Be(1);
			model[0].PrivateSectorValidationType.Should().Be(trainingProviderType.PrivateSectorValidationType);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderTypes(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void GetTrainingProviderTypes_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetTrainingProviderTypes())
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingProviderTypes();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<TrainingProviderTypeViewModel>>();
			var model = result.Data as List<TrainingProviderTypeViewModel>;
			model.Count().Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderTypes(), Times.Once);
		}
		#endregion

		#region DownloadAttachment
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void DownloadAttachment()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			var attachment = EntityHelper.CreateAttachment();
			trainingProvider.BusinessCaseDocumentId = attachment.Id;
			helper.GetMock<IAttachmentService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(attachment);
			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProvider);
			var controller = helper.Create();

			// Act
			var result = controller.DownloadAttachment(trainingProvider.Id, attachment.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<FileContentResult>();
			var model = result as FileContentResult;
			model.FileDownloadName.Should().Be(attachment.FileName + attachment.FileExtension);
			helper.GetMock<IAttachmentService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}
		#endregion
	}
}
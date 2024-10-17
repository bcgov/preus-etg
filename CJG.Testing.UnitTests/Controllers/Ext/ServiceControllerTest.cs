using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Ext.Controllers;
using CJG.Web.External.Areas.Ext.Models.Services;
using CJG.Web.External.Areas.Ext.Models.TrainingCosts;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Testing.UnitTests.Controllers.Ext
{
	[TestClass]
	public class ServiceControllerTest
	{
		#region EmploymentServicesAndSupportsView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ServiceController))]
		public void EmploymentServicesAndSupportsView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ServiceController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);

			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			helper.GetMock<IEligibleExpenseTypeService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(eligibleExpenseType);

			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);

			var controller = helper.Create();

			// Act
			var result = controller.EmploymentServicesAndSupportsView(grantApplication.Id, eligibleExpenseType.Id, eligibleCost.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			eligibleExpenseType.Id.Should().Be(controller.ViewBag.EligibleExpenseTypeId);
			eligibleCost.Id.Should().Be(controller.ViewBag.EligibleCostId);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IEligibleExpenseTypeService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ServiceController))]
		public void EmploymentServicesAndSupportsView_NoPermission()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ServiceController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.OfferIssued;
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);

			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			helper.GetMock<IEligibleExpenseTypeService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(eligibleExpenseType);

			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);

			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NotAuthorizedException>(() => controller.EmploymentServicesAndSupportsView(1, 1, 1));
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IEligibleExpenseTypeService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ServiceController))]
		public void EmploymentServicesAndSupportsView_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ServiceController>(user);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();

			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NotAuthorizedException>(() => controller.EmploymentServicesAndSupportsView(1, 1, 1));
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IEligibleExpenseTypeService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
		}
		#endregion

		#region GetEmploymentServicesAndSupports
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ServiceController))]
		public void GetEmploymentServicesAndSupports()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ServiceController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);

			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			helper.GetMock<IEligibleExpenseTypeService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(eligibleExpenseType);

			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetEmploymentServicesAndSupports(grantApplication.Id, eligibleExpenseType.Id, eligibleCost.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<EmploymentServiceViewModel>();
			var model = result.Data as EmploymentServiceViewModel;
			model.GrantApplicationId.Should().Be(grantApplication.Id);
			model.EligibleCostId.Should().Be(eligibleCost.Id);
			model.EstimatedCost.Should().Be(eligibleCost.EstimatedCost);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IEligibleExpenseTypeService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ServiceController))]
		public void GetEmploymentServicesAndSupports_NoPermission()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ServiceController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.OfferIssued;
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);

			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			helper.GetMock<IEligibleExpenseTypeService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(eligibleExpenseType);

			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetEmploymentServicesAndSupports(grantApplication.Id, eligibleExpenseType.Id, eligibleCost.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<EmploymentServiceViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IEligibleExpenseTypeService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ServiceController))]
		public void GetEmploymentServicesAndSupports_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ServiceController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetEmploymentServicesAndSupports(1, 1, 1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<EmploymentServiceViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IEligibleExpenseTypeService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion

		#region UpdateEmploymentServicesAndSupports
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ServiceController))]
		public void UpdateEmploymentServicesAndSupports()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ServiceController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);

			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			helper.GetMock<IEligibleExpenseTypeService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(eligibleExpenseType);

			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var controller = helper.Create();
			var eligibleExpenseBreakdownViewModel = new EligibleExpenseBreakdownViewModel()
			{
				Id = 1,
				RowVersion = "AgQGCAoMDhASFA=="
			};

			var eligibleExpenseTypeViewModel = new EligibleExpenseTypeViewModel()
            {
                Id = 1,
                Caption = "EligibleExpenseTypeViewModel Test Caption",
                Description = "EligibleExpenseTypeViewModel Test Description",
                ExpenseTypeId = ExpenseTypes.NotParticipantLimited,
				EligibleExpenseBreakdowns = new List<EligibleExpenseBreakdownViewModel>() { eligibleExpenseBreakdownViewModel }
            };
            var viewModel = new EmploymentServiceViewModel()
            {
                EligibleExpenseType = eligibleExpenseTypeViewModel,
                GrantApplicationId = grantApplication.Id,
                EligibleCostId = eligibleCost.Id,
				RowVersion = "AgQGCAoMDhASFA=="
			};
			// Act
			var result = controller.UpdateEmploymentServicesAndSupports(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			((JsonResult)result).Data.Should().NotBeNull().And.BeOfType<EmploymentServiceViewModel>();
			var model = ((JsonResult)result).Data as EmploymentServiceViewModel;
			model.GrantApplicationId.Should().Be(grantApplication.Id);
			model.EligibleCostId.Should().Be(eligibleCost.Id);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IEligibleExpenseTypeService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ServiceController))]
		public void UpdateEmploymentServicesAndSupports_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ServiceController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();

			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			helper.GetMock<IEligibleExpenseTypeService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(eligibleExpenseType);

			var controller = helper.Create();
			var eligibleExpenseBreakdownViewModel = new EligibleExpenseBreakdownViewModel()
			{
				Id = 1,
				RowVersion = "AgQGCAoMDhASFA=="
			};

			var eligibleExpenseTypeViewModel = new EligibleExpenseTypeViewModel()
			{
				Id = 1,
				Caption = "EligibleExpenseTypeViewModel Test Caption",
				Description = "EligibleExpenseTypeViewModel Test Description",
				ExpenseTypeId = ExpenseTypes.NotParticipantLimited,
				EligibleExpenseBreakdowns = new List<EligibleExpenseBreakdownViewModel>() { eligibleExpenseBreakdownViewModel }
			};
			var viewModel = new EmploymentServiceViewModel()
			{
				EligibleExpenseType = eligibleExpenseTypeViewModel,
				GrantApplicationId = 1,
				RowVersion = "AgQGCAoMDhASFA=="
			};
			// Act
			var result = controller.UpdateEmploymentServicesAndSupports(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			((JsonResult)result).Data.Should().NotBeNull().And.BeOfType<EmploymentServiceViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IEligibleExpenseTypeService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			controller.HttpContext.Response.StatusCode.Should().Be(403);

		}

		#endregion
	}
}

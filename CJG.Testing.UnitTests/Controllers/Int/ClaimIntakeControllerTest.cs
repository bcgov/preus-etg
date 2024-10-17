using System;
using CJG.Testing.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CJG.Web.External.Areas.Int.Controllers;
using static CJG.Testing.Core.ServiceHelper;
using CJG.Web.External.Areas.Int.Models.ClaimDashboard;
using FluentAssertions;
using System.Web.Mvc;

namespace CJG.Testing.UnitTests.Controllers.Int
{
	[TestClass]
	public class ClaimIntakeControllerTest
	{
		#region TestAuth
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ClaimIntakeController))]
		public void SaveOverpaymentsTestAuthAssessor()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ClaimIntakeController>(user, Roles.Assessor);
			var controller = helper.Create();
			ClaimDashboardViewModel viewModel = new ClaimDashboardViewModel();

			// Act
			var result = controller.SaveOverpayments(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ClaimDashboardViewModel>();
			controller.ControllerContext.HttpContext.Response.StatusCode.Should().Be(400);
		}
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ClaimIntakeController))]
		public void SaveOverpaymentsTestAuthDirector()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ClaimIntakeController>(user, Roles.Director);
			var controller = helper.Create();
			ClaimDashboardViewModel viewModel = new ClaimDashboardViewModel();

			// Act
			var result = controller.SaveOverpayments(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ClaimDashboardViewModel>();
			controller.ControllerContext.HttpContext.Response.StatusCode.Should().Be(400);
		}


		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ClaimIntakeController))]
		public void SaveOverpaymentsTestAuthDirectorOfFinance()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ClaimIntakeController>(user, Roles.DirectorOfFinance);
			var controller = helper.Create();
			ClaimDashboardViewModel viewModel = new ClaimDashboardViewModel();

			// Act
			var result = controller.SaveOverpayments(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ClaimDashboardViewModel>();
			controller.ControllerContext.HttpContext.Response.StatusCode.Should().Be(403);
		}
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ClaimIntakeController))]
		public void SaveOverpaymentsTestAuthFinancialClerk()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ClaimIntakeController>(user, Roles.FinancialClerk);
			var controller = helper.Create();
			ClaimDashboardViewModel viewModel = new ClaimDashboardViewModel();

			// Act
			var result = controller.SaveOverpayments(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ClaimDashboardViewModel>();
			controller.ControllerContext.HttpContext.Response.StatusCode.Should().Be(400);
		}
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ClaimIntakeController))]
		public void SaveOverpaymentsTestAuthMeasurementAndReporting()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ClaimIntakeController>(user, Roles.MeasurementAndReporting);
			var controller = helper.Create();
			ClaimDashboardViewModel viewModel = new ClaimDashboardViewModel();

			// Act
			var result = controller.SaveOverpayments(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ClaimDashboardViewModel>();
			controller.ControllerContext.HttpContext.Response.StatusCode.Should().Be(403);
		}
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ClaimIntakeController))]
		public void SaveOverpaymentsTestAuthOperationsManager()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ClaimIntakeController>(user, Roles.OperationsManager);
			var controller = helper.Create();
			ClaimDashboardViewModel viewModel = new ClaimDashboardViewModel();

			// Act
			var result = controller.SaveOverpayments(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ClaimDashboardViewModel>();
			controller.ControllerContext.HttpContext.Response.StatusCode.Should().Be(403);
		}
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ClaimIntakeController))]
		public void SaveOverpaymentsTestAuthSystemAdministrator()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ClaimIntakeController>(user, Roles.SystemAdministrator);
			var controller = helper.Create();
			ClaimDashboardViewModel viewModel = new ClaimDashboardViewModel();

			// Act
			var result = controller.SaveOverpayments(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ClaimDashboardViewModel>();
			controller.ControllerContext.HttpContext.Response.StatusCode.Should().Be(400);
		}

		#endregion
	}
}

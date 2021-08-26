using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Ext.Controllers;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Areas.Ext.Models.Applications;
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
	public class ApplicationViewControllerTest
	{

		#region ApplicationDetailsView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationViewController))]
		public void ApplicationDetailsView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationViewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationDetailsView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationViewController))]
		public void ApplicationDetailsView_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationViewController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NotAuthorizedException>(() => controller.ApplicationDetailsView(1));
		}
		#endregion

		#region GetApplicationDetails
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationViewController))]
		public void GetApplicationDetails()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationViewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetApplicationDetails(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationViewModel>();
			var model = result.Data as ApplicationViewModel;
			model.Id.Should().Be(grantApplication.Id);
			model.ApplicantContact.ApplicantEmail.Should().Be(user.EmailAddress);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationViewController))]
		public void GetApplicationDetails_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationViewController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetApplicationDetails(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().BeNull();
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationViewController))]
		public void GetApplicationDetails_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationViewController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NoContentException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetApplicationDetails(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().BeNull();
		}
		#endregion

		#region WithdrawApplicationView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationViewController))]
		public void WithdrawApplicationView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationViewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.WithdrawApplicationView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<PartialViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationViewController))]
		public void WithdrawApplicationView_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationViewController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NotAuthorizedException>(() => controller.WithdrawApplicationView(1));
		}
		#endregion

		#region WithdrawApplication
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationViewController))]
		public void WithdrawApplication()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationViewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var viewModel = new WithdrawApplicationViewModel(grantApplication)
			{
				WithdrawReason = "test"
			};
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.WithdrawApplication(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<WithdrawApplicationViewModel>();
			var model = result.Data as WithdrawApplicationViewModel;
			model.Id.Should().Be(grantApplication.Id);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>()
				.Verify(m => m.Withdraw(It.IsAny<GrantApplication>(), It.IsAny<string>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationViewController))]
		public void WithdrawApplication_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationViewController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var viewModel = new WithdrawApplicationViewModel()
			{
				WithdrawReason = "test"
			};
			var controller = helper.Create();

			// Act
			var result = controller.WithdrawApplication(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<WithdrawApplicationViewModel>();
			var model = result.Data as WithdrawApplicationViewModel;
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationViewController))]
		public void WithdrawApplication_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationViewController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NoContentException>();
			var viewModel = new WithdrawApplicationViewModel()
			{
				WithdrawReason = "test"
			};
			var controller = helper.Create();

			// Act
			var result = controller.WithdrawApplication(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<WithdrawApplicationViewModel>();
			var model = result.Data as WithdrawApplicationViewModel;
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationViewController))]
		public void WithdrawApplication_ArgumentNullException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationViewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var viewModel = new WithdrawApplicationViewModel(grantApplication)
			{
				WithdrawReason = "test"
			};
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Withdraw(It.IsAny<GrantApplication>(), It.IsAny<string>()))
				.Throws<ArgumentNullException>();
			var controller = helper.Create();

			// Act
			var result = controller.WithdrawApplication(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<WithdrawApplicationViewModel>();
			var model = result.Data as WithdrawApplicationViewModel;
			controller.HttpContext.Response.StatusCode.Should().Be(500);
		}
		#endregion
	}
}

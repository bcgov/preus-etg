using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Ext.Controllers;
using CJG.Web.External.Areas.Ext.Models.Applications;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CJG.Testing.UnitTests.Controllers.Ext
{
	[TestClass]
	public class ApplicationReviewControllerTest
	{
		#region ApplicationReviewView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewView_WithOrganizationAddress()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(1);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IOrganizationService>().Verify(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewView_WithoutOrganizationAddress()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.OrganizationAddress = null;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(1);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			grantApplication.OrganizationAddress.Should().NotBeNull();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Exactly(2));
			helper.GetMock<IOrganizationService>().Verify(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewView_NoOrganizationAddress()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			user.Organization.HeadOfficeAddress = null;
			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.OrganizationAddress = null;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(OrganizationProfileController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(OrganizationProfileController.OrganizationProfileView));
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewView_GrantOpeningClosed()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.GrantOpening.State = GrantOpeningStates.Closed;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("The application cannot be submitted until the grant stream is open.");
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Incomplete);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewView_GrantOpeningNotOpen()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.GrantOpening.State = GrantOpeningStates.Published;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("The application cannot be submitted until the grant stream is open.");
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewView_NoOrganizationAdministrator()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller",""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("An Organization profile must be completed before you can submit a grant application.");
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IOrganizationService>().Verify(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewView_NotSubmittable()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.GrantOpening.GrantStream.AttachmentsIsEnabled = true;
			grantApplication.GrantOpening.GrantStream.AttachmentsRequired = true;
			grantApplication.Attachments.Clear();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("The application Review page is not available when in current state.");
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewView_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser(2);
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NotAuthorizedException>(() => controller.ApplicationReviewView(1));
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewView_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser(2);
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NoContentException>(() => controller.ApplicationReviewView(1));
		}
		#endregion

		#region GetApplicationReview
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void GetApplicationReview()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetApplicationReview(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationReviewViewModel>();
			var model = result.Data as ApplicationReviewViewModel;
			model.Id.Should().Be(grantApplication.Id);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void GetApplicationReview_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetApplicationReview(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationReviewViewModel>();
			var model = result.Data as ApplicationReviewViewModel;
			model.Id.Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void GetApplicationReview_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetApplicationReview(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationReviewViewModel>();
			var model = result.Data as ApplicationReviewViewModel;
			model.Id.Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion


		#region ApplicationReviewGrantProgramView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewGrantProgramView_WithOrganizationAddress()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(1);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewGrantProgramView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IOrganizationService>().Verify(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewGrantProgramView_WithoutOrganizationAddress()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.OrganizationAddress = null;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(1);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewGrantProgramView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			grantApplication.OrganizationAddress.Should().NotBeNull();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IOrganizationService>().Verify(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewGrantProgramView_GrantOpeningClosed()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.GrantOpening.State = GrantOpeningStates.Closed;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewGrantProgramView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("The application cannot be submitted until the grant stream is open.");
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Incomplete);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewGrantProgramView_GrantOpeningNotOpen()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.GrantOpening.State = GrantOpeningStates.Published;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewGrantProgramView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("The application cannot be submitted until the grant stream is open.");
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewGrantProgramView_NoOrganizationAdministrator()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewGrantProgramView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("An Organization profile must be completed before you can submit a grant application.");
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IOrganizationService>().Verify(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewGrantProgramView_NotSubmittable()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.GrantOpening.GrantStream.AttachmentsIsEnabled = true;
			grantApplication.GrantOpening.GrantStream.AttachmentsRequired = true;
			grantApplication.Attachments.Clear();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewGrantProgramView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("The application Review page is not available when in current state.");
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewGrantProgramView_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser(2);
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NotAuthorizedException>(() => controller.ApplicationReviewGrantProgramView(1));
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewGrantProgramView_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser(2);
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NoContentException>(() => controller.ApplicationReviewGrantProgramView(1));
		}
		#endregion

		#region GetApplicationReviewGrantProgram
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void GetApplicationReviewGrantProgram()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetApplicationReviewGrantProgram(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationGrantProgramReviewViewModel>();
			var model = result.Data as ApplicationGrantProgramReviewViewModel;
			model.Id.Should().Be(grantApplication.Id);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void GetApplicationReviewGrantProgram_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetApplicationReviewGrantProgram(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationGrantProgramReviewViewModel>();
			var model = result.Data as ApplicationGrantProgramReviewViewModel;
			model.Id.Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void GetApplicationReviewGrantProgram_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetApplicationReviewGrantProgram(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationGrantProgramReviewViewModel>();
			var model = result.Data as ApplicationGrantProgramReviewViewModel;
			model.Id.Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion


		#region ApplicationReviewSkillsTrainingView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewSkillsTrainingView_WithOrganizationAddress()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(user);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType(1);
			grantApplication.CompleteGrantApplication();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(1);
			helper.GetMock<IEligibleExpenseTypeService>().Setup(m => m.Get(It.IsAny<int>())).Returns(eligibleExpenseType);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewSkillsTrainingView(grantApplication.Id, eligibleExpenseType.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IEligibleExpenseTypeService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IOrganizationService>().Verify(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewSkillsTrainingView_WithoutOrganizationAddress()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(user);
			grantApplication.OrganizationAddress = null;
			grantApplication.CompleteGrantApplication();
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType(1);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(1);
			helper.GetMock<IEligibleExpenseTypeService>().Setup(m => m.Get(It.IsAny<int>())).Returns(eligibleExpenseType);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewSkillsTrainingView(grantApplication.Id, eligibleExpenseType.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			grantApplication.OrganizationAddress.Should().NotBeNull();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IEligibleExpenseTypeService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IOrganizationService>().Verify(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewSkillsTrainingView_GrantOpeningClosed()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(user);
			grantApplication.CompleteGrantApplication();
			grantApplication.GrantOpening.State = GrantOpeningStates.Closed;
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType(1);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IEligibleExpenseTypeService>().Setup(m => m.Get(It.IsAny<int>())).Returns(eligibleExpenseType);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewSkillsTrainingView(grantApplication.Id, eligibleExpenseType.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("The application cannot be submitted until the grant stream is open.");
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Incomplete);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IEligibleExpenseTypeService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewSkillsTrainingView_GrantOpeningNotOpen()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(user);
			grantApplication.CompleteGrantApplication();
			grantApplication.GrantOpening.State = GrantOpeningStates.Published;
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType(1);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IEligibleExpenseTypeService>().Setup(m => m.Get(It.IsAny<int>())).Returns(eligibleExpenseType);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewSkillsTrainingView(grantApplication.Id, eligibleExpenseType.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("The application cannot be submitted until the grant stream is open.");
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IEligibleExpenseTypeService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewSkillsTrainingView_NoOrganizationAdministrator()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(user);
			grantApplication.CompleteGrantApplication();
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType(1);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IEligibleExpenseTypeService>().Setup(m => m.Get(It.IsAny<int>())).Returns(eligibleExpenseType);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewSkillsTrainingView(grantApplication.Id, eligibleExpenseType.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("An Organization profile must be completed before you can submit a grant application.");
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IEligibleExpenseTypeService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IOrganizationService>().Verify(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewSkillsTrainingView_NotSubmittable()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(user);
			grantApplication.GrantOpening.GrantStream.AttachmentsIsEnabled = true;
			grantApplication.GrantOpening.GrantStream.AttachmentsRequired = true;
			grantApplication.Attachments.Clear();
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType(1);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IEligibleExpenseTypeService>().Setup(m => m.Get(It.IsAny<int>())).Returns(eligibleExpenseType);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewSkillsTrainingView(grantApplication.Id, eligibleExpenseType.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("The application Review page is not available when in current state.");
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IEligibleExpenseTypeService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewSkillsTrainingView_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser(2);
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NotAuthorizedException>(() => controller.ApplicationReviewSkillsTrainingView(1, 1));
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewSkillsTrainingView_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser(2);
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NoContentException>(() => controller.ApplicationReviewSkillsTrainingView(1, 1));
		}
		#endregion

		#region GetApplicationReviewSkillsTraining
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void GetApplicationReviewSkillsTraining()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType(1);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IEligibleExpenseTypeService>().Setup(m => m.Get(It.IsAny<int>())).Returns(eligibleExpenseType);
			var controller = helper.Create();

			// Act
			var result = controller.GetApplicationReviewSkillsTraining(grantApplication.Id, eligibleExpenseType.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationSkillsReviewViewModel>();
			var model = result.Data as ApplicationSkillsReviewViewModel;
			model.Id.Should().Be(grantApplication.Id);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IEligibleExpenseTypeService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void GetApplicationReviewSkillsTraining_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetApplicationReviewSkillsTraining(1, 1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationSkillsReviewViewModel>();
			var model = result.Data as ApplicationSkillsReviewViewModel;
			model.Id.Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void GetApplicationReviewSkillsTraining_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetApplicationReviewSkillsTraining(1, 1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationSkillsReviewViewModel>();
			var model = result.Data as ApplicationSkillsReviewViewModel;
			model.Id.Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion


		#region ApplicationReviewESSView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewESSView_WithOrganizationAddress()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(1);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewESSView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IOrganizationService>().Verify(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewESSView_WithoutOrganizationAddress()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.OrganizationAddress = null;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(1);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewESSView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			grantApplication.OrganizationAddress.Should().NotBeNull();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IOrganizationService>().Verify(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewESSView_GrantOpeningClosed()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.GrantOpening.State = GrantOpeningStates.Closed;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewESSView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("The application cannot be submitted until the grant stream is open.");
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Incomplete);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewESSView_GrantOpeningNotOpen()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.GrantOpening.State = GrantOpeningStates.Published;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewESSView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("The application cannot be submitted until the grant stream is open.");
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewESSView_NoOrganizationAdministrator()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewESSView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("An Organization profile must be completed before you can submit a grant application.");
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IOrganizationService>().Verify(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewESSView_NotSubmittable()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.GrantOpening.GrantStream.AttachmentsIsEnabled = true;
			grantApplication.GrantOpening.GrantStream.AttachmentsRequired = true;
			grantApplication.Attachments.Clear();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewESSView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("The application Review page is not available when in current state.");
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewESSView_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser(2);
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NotAuthorizedException>(() => controller.ApplicationReviewESSView(1));
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewESSView_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser(2);
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NoContentException>(() => controller.ApplicationReviewESSView(1));
		}
		#endregion

		#region GetApplicationReviewESS
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void GetApplicationReviewESS()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetApplicationReviewESS(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationESSReviewViewModel>();
			var model = result.Data as ApplicationESSReviewViewModel;
			model.Id.Should().Be(grantApplication.Id);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void GetApplicationReviewESS_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetApplicationReviewESS(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationESSReviewViewModel>();
			var model = result.Data as ApplicationESSReviewViewModel;
			model.Id.Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void GetApplicationReviewESS_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetApplicationReviewESS(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationESSReviewViewModel>();
			var model = result.Data as ApplicationESSReviewViewModel;
			model.Id.Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion


		#region ApplicationReviewTrainingCostView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewTrainingCostView_WithOrganizationAddress()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(1);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewTrainingCostView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IOrganizationService>().Verify(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewTrainingCostView_WithoutOrganizationAddress()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.OrganizationAddress = null;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(1);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewTrainingCostView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			grantApplication.OrganizationAddress.Should().NotBeNull();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IOrganizationService>().Verify(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewTrainingCostView_GrantOpeningClosed()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.GrantOpening.State = GrantOpeningStates.Closed;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewTrainingCostView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("The application cannot be submitted until the grant stream is open.");
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Incomplete);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewTrainingCostView_GrantOpeningNotOpen()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.GrantOpening.State = GrantOpeningStates.Published;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewTrainingCostView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("The application cannot be submitted until the grant stream is open.");
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewTrainingCostView_NoOrganizationAdministrator()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewTrainingCostView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("An Organization profile must be completed before you can submit a grant application.");
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IOrganizationService>().Verify(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewTrainingCostView_NotSubmittable()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.GrantOpening.GrantStream.AttachmentsIsEnabled = true;
			grantApplication.GrantOpening.GrantStream.AttachmentsRequired = true;
			grantApplication.Attachments.Clear();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewTrainingCostView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("The application Review page is not available when in current state.");
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewTrainingCostView_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser(2);
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NotAuthorizedException>(() => controller.ApplicationReviewTrainingCostView(1));
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewTrainingCostView_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser(2);
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NoContentException>(() => controller.ApplicationReviewTrainingCostView(1));
		}
		#endregion

		#region GetApplicationReviewTrainingCost
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void GetApplicationReviewTrainingCost()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetApplicationReviewTrainingCost(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationTrainingCostReviewViewModel>();
			var model = result.Data as ApplicationTrainingCostReviewViewModel;
			model.Id.Should().Be(grantApplication.Id);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void GetApplicationReviewTrainingCost_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetApplicationReviewTrainingCost(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationTrainingCostReviewViewModel>();
			var model = result.Data as ApplicationTrainingCostReviewViewModel;
			model.Id.Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void GetApplicationReviewTrainingCost_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetApplicationReviewTrainingCost(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationTrainingCostReviewViewModel>();
			var model = result.Data as ApplicationTrainingCostReviewViewModel;
			model.Id.Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion


		#region ApplicationReviewDeliveryPartnerView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewDeliveryPartnerView_WithOrganizationAddress()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.GrantOpening.GrantStream.IncludeDeliveryPartner = true;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(1);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewDeliveryPartnerView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IOrganizationService>().Verify(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewDeliveryPartnerView_WithoutOrganizationAddress()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.GrantOpening.GrantStream.IncludeDeliveryPartner = true;
			grantApplication.OrganizationAddress = null;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(1);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewDeliveryPartnerView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			grantApplication.OrganizationAddress.Should().NotBeNull();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IOrganizationService>().Verify(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewDeliveryPartnerView_GrantOpeningClosed()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.GrantOpening.GrantStream.IncludeDeliveryPartner = true;
			grantApplication.GrantOpening.State = GrantOpeningStates.Closed;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewDeliveryPartnerView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("The application cannot be submitted until the grant stream is open.");
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Incomplete);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewDeliveryPartnerView_GrantOpeningNotOpen()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.GrantOpening.GrantStream.IncludeDeliveryPartner = true;
			grantApplication.GrantOpening.State = GrantOpeningStates.Published;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewDeliveryPartnerView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("The application cannot be submitted until the grant stream is open.");
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewDeliveryPartnerView_NoOrganizationAdministrator()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.GrantOpening.GrantStream.IncludeDeliveryPartner = true;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewDeliveryPartnerView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("An Organization profile must be completed before you can submit a grant application.");
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IOrganizationService>().Verify(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewDeliveryPartnerView_NotSubmittable()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.GrantOpening.GrantStream.IncludeDeliveryPartner = true;
			grantApplication.GrantOpening.GrantStream.AttachmentsIsEnabled = true;
			grantApplication.GrantOpening.GrantStream.AttachmentsRequired = true;
			grantApplication.Attachments.Clear();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewDeliveryPartnerView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("The application Review page is not available when in current state.");
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewDeliveryPartnerView_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser(2);
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NotAuthorizedException>(() => controller.ApplicationReviewDeliveryPartnerView(1));
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewDeliveryPartnerView_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser(2);
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NoContentException>(() => controller.ApplicationReviewDeliveryPartnerView(1));
		}
		#endregion

		#region GetApplicationDeliveryPartner
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void GetApplicationDeliveryPartner()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetApplicationDeliveryPartner(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationDeliveryViewModel>();
			var model = result.Data as ApplicationDeliveryViewModel;
			model.Id.Should().Be(grantApplication.Id);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void GetApplicationDeliveryPartner_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetApplicationDeliveryPartner(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationDeliveryViewModel>();
			var model = result.Data as ApplicationDeliveryViewModel;
			model.Id.Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void GetApplicationDeliveryPartner_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetApplicationDeliveryPartner(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationDeliveryViewModel>();
			var model = result.Data as ApplicationDeliveryViewModel;
			model.Id.Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region UpdateApplicationDeliveryPartner
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void UpdateApplicationDeliveryPartner()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var viewModel = new ApplicationDeliveryViewModel(grantApplication);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.UpdateApplicationDeliveryPartner(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationDeliveryViewModel>();
			var model = result.Data as ApplicationDeliveryViewModel;
			model.Id.Should().Be(grantApplication.Id);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.UpdateDeliveryPartner(It.IsAny<GrantApplication>(), It.IsAny<int?>(), It.IsAny<IEnumerable<int>>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void UpdateApplicationDeliveryPartner_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var viewModel = new ApplicationDeliveryViewModel(grantApplication);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.UpdateApplicationDeliveryPartner(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationDeliveryViewModel>();
			var model = result.Data as ApplicationDeliveryViewModel;
			model.Id.Should().Be(1);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void UpdateApplicationDeliveryPartner_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var viewModel = new ApplicationDeliveryViewModel(grantApplication);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var controller = helper.Create();

			// Act
			var result = controller.UpdateApplicationDeliveryPartner(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationDeliveryViewModel>();
			var model = result.Data as ApplicationDeliveryViewModel;
			model.Id.Should().Be(1);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion


		#region ApplicationReviewApplicantDeclarationView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewApplicantDeclarationView_WithOrganizationAddress()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.GrantOpening.GrantStream.IncludeDeliveryPartner = false;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(1);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewApplicantDeclarationView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Exactly(2));
			helper.GetMock<IOrganizationService>().Verify(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewApplicantDeclarationView_WithoutOrganizationAddress()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.OrganizationAddress = null;
			grantApplication.GrantOpening.GrantStream.IncludeDeliveryPartner = false;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(1);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewApplicantDeclarationView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			grantApplication.OrganizationAddress.Should().NotBeNull();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Exactly(2));
			helper.GetMock<IOrganizationService>().Verify(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewApplicantDeclarationView_GrantOpeningClosed()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.GrantOpening.State = GrantOpeningStates.Closed;
			grantApplication.GrantOpening.GrantStream.IncludeDeliveryPartner = false;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewApplicantDeclarationView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("The application cannot be submitted until the grant stream is open.");
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Incomplete);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Exactly(2));
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewApplicantDeclarationView_GrantOpeningNotOpen()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.GrantOpening.State = GrantOpeningStates.Published;
			grantApplication.GrantOpening.GrantStream.IncludeDeliveryPartner = false;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewApplicantDeclarationView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("The application cannot be submitted until the grant stream is open.");
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Exactly(2));
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewApplicantDeclarationView_NoOrganizationAdministrator()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.GrantOpening.GrantStream.IncludeDeliveryPartner = false;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewApplicantDeclarationView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("An Organization profile must be completed before you can submit a grant application.");
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Exactly(2));
			helper.GetMock<IOrganizationService>().Verify(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewApplicantDeclarationView_NotSubmittable()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.GrantOpening.GrantStream.IncludeDeliveryPartner = false;
			grantApplication.GrantOpening.GrantStream.AttachmentsIsEnabled = true;
			grantApplication.GrantOpening.GrantStream.AttachmentsRequired = true;
			grantApplication.Attachments.Clear();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(0);
			helper.GetMock<IGrantStreamService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationReviewApplicantDeclarationView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			((RedirectToRouteResult)result).RouteValues["Controller"].Should().Be(nameof(HomeController).Replace("Controller", ""));
			((RedirectToRouteResult)result).RouteValues["Action"].Should().Be(nameof(HomeController.Index));
			controller.TempData["Message"].Should().Be("The application Review page is not available when in current state.");
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IUserService>().Verify(m => m.UpdateUserFromBCeIDAccount(It.IsAny<User>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Exactly(2));
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewApplicantDeclarationView_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser(2);
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NotAuthorizedException>(() => controller.ApplicationReviewApplicantDeclarationView(1));
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void ApplicationReviewApplicantDeclarationView_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser(2);
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NoContentException>(() => controller.ApplicationReviewApplicantDeclarationView(1));
		}
		#endregion

		#region GetApplicantDeclaration
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void GetApplicantDeclaration()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var applicantDeclaration = new Document("Applicant Declaration", "tesT", new DocumentTemplate(DocumentTypes.ApplicantDeclaration, "test", "test"));
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IGrantProgramService>().Setup(m => m.GenerateApplicantDeclarationBody(It.IsAny<GrantApplication>())).Returns(applicantDeclaration.Body);
			var controller = helper.Create();

			// Act
			var result = controller.GetApplicantDeclaration(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicantDeclarationViewModel>();
			var model = result.Data as ApplicantDeclarationViewModel;
			model.Id.Should().Be(grantApplication.Id);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IGrantProgramService>().Verify(m => m.GenerateApplicantDeclarationBody(It.IsAny<GrantApplication>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void GetApplicantDeclaration_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetApplicantDeclaration(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicantDeclarationViewModel>();
			var model = result.Data as ApplicantDeclarationViewModel;
			model.Id.Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void GetApplicantDeclaration_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetApplicantDeclaration(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicantDeclarationViewModel>();
			var model = result.Data as ApplicantDeclarationViewModel;
			model.Id.Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion


		#region SubmitApplication
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void SubmitApplication()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var applicantDeclaration = new Document("Applicant Declaration", "tesT", new DocumentTemplate(DocumentTypes.ApplicantDeclaration, "test", "test"));
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IGrantProgramService>().Setup(m => m.GenerateApplicantDeclarationBody(It.IsAny<GrantApplication>())).Returns(applicantDeclaration.Body);
			var viewModel = new ApplicantDeclarationViewModel(grantApplication, user, helper.GetMock<IGrantProgramService>().Object)
			{
				DeclarationConfirmed = true
			};
			var controller = helper.Create();

			// Act
			var result = controller.SubmitApplication(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicantDeclarationViewModel>();
			var model = result.Data as ApplicantDeclarationViewModel;
			model.Id.Should().Be(grantApplication.Id);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Submit(It.IsAny<GrantApplication>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void SubmitApplication_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var applicantDeclaration = new Document("Applicant Declaration", "tesT", new DocumentTemplate(DocumentTypes.ApplicantDeclaration, "test", "test"));
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			helper.GetMock<IGrantProgramService>().Setup(m => m.GenerateApplicantDeclarationBody(It.IsAny<GrantApplication>())).Returns(applicantDeclaration.Body);
			var viewModel = new ApplicantDeclarationViewModel(grantApplication, user, helper.GetMock<IGrantProgramService>().Object)
			{
				DeclarationConfirmed = true
			};
			var controller = helper.Create();

			// Act
			var result = controller.SubmitApplication(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicantDeclarationViewModel>();
			var model = result.Data as ApplicantDeclarationViewModel;
			model.Id.Should().Be(1);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationReviewController))]
		public void SubmitApplication_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationReviewController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var applicantDeclaration = new Document("Applicant Declaration", "tesT", new DocumentTemplate(DocumentTypes.ApplicantDeclaration, "test", "test"));
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			helper.GetMock<IGrantProgramService>().Setup(m => m.GenerateApplicantDeclarationBody(It.IsAny<GrantApplication>())).Returns(applicantDeclaration.Body);
			var viewModel = new ApplicantDeclarationViewModel(grantApplication, user, helper.GetMock<IGrantProgramService>().Object)
			{
				DeclarationConfirmed = true
			};
			var controller = helper.Create();

			// Act
			var result = controller.SubmitApplication(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicantDeclarationViewModel>();
			var model = result.Data as ApplicantDeclarationViewModel;
			model.Id.Should().Be(1);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion
	}
}

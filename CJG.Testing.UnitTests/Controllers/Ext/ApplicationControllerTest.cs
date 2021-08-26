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
using System.Linq;
using System.Web.Mvc;

namespace CJG.Testing.UnitTests.Controllers.Ext
{
	[TestClass]
	public class ApplicationControllerTest
	{
		#region GrantSelectionView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void GrantSelectionView_NewApplication()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			var controller = helper.Create();

			// Act
			var result = controller.GrantSelectionView(0, 1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			0.Should().Be(controller.ViewBag.GrantApplicationId);
			1.Should().Be(controller.ViewBag.GrantProgramId);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void GrantSelectionView_ExistingApplication()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GrantSelectionView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			0.Should().Be(controller.ViewBag.GrantProgramId);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void GrantSelectionView_NoAddress()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			user.PhysicalAddress = null;
			user.PhysicalAddressId = null;
			var helper = new ControllerHelper<ApplicationController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GrantSelectionView(0, 1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			var route = result as RedirectToRouteResult;
			route.RouteValues["action"].Should().Be(nameof(UserProfileController.UpdateUserProfileView));
			route.RouteValues["controller"].Should().Be(nameof(UserProfileController).Replace("Controller", ""));
			controller.TempData["Message"].Should().Be("Business Address is required to start new application");
			controller.TempData["MessageType"].Should().Be("warning");
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void GrantSelectionView_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser(2);
			var helper = new ControllerHelper<ApplicationController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NotAuthorizedException>(() => controller.GrantSelectionView(1));
		}
		#endregion

		#region GetGrantSelection
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void GetGrantSelection_NewApplication()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			var fiscalYear = EntityHelper.CreateFiscalYear();
			helper.GetMock<IFiscalYearService>().Setup(m => m.GetFiscalYear(It.IsAny<DateTime>())).Returns(fiscalYear);
			helper.GetMock<IGrantOpeningManageScheduledService>().Setup(m => m.ManageStateTransitions(It.IsAny<int>()));

			var grantOpening = EntityHelper.CreateGrantOpening();
			helper.GetMock<IGrantOpeningService>().Setup(m => m.GetGrantOpenings(It.IsAny<DateTime>(), It.IsAny<int>())).Returns(new[] { grantOpening });
			helper.GetMock<IGrantProgramService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantOpening.GrantStream.GrantProgram);
			var controller = helper.Create();

			// Act
			var result = controller.GetGrantSelection(0, grantOpening.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationStartViewModel>();
			var model = result.Data as ApplicationStartViewModel;
			model.Id.Should().Be(0);
			helper.GetMock<IFiscalYearService>().Verify(m => m.GetFiscalYear(It.IsAny<DateTime>()), Times.Once);
			helper.GetMock<IGrantOpeningManageScheduledService>().Verify(m => m.ManageStateTransitions(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<IGrantOpeningService>().Verify(m => m.GetGrantOpenings(It.IsAny<DateTime>(), It.IsAny<int>()), Times.Once);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetPrioritySectors(), Times.Once);
			helper.GetMock<IGrantProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void GetGrantSelection_ExistingApplication()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);

			var fiscalYear = EntityHelper.CreateFiscalYear();
			helper.GetMock<IFiscalYearService>().Setup(m => m.GetFiscalYear(It.IsAny<DateTime>())).Returns(fiscalYear);
			helper.GetMock<IGrantOpeningManageScheduledService>().Setup(m => m.ManageStateTransitions(It.IsAny<int>()));
			helper.GetMock<IGrantOpeningService>().Setup(m => m.GetGrantOpenings(It.IsAny<DateTime>(), It.IsAny<int>())).Returns(new[] { grantApplication.GrantOpening });
			helper.GetMock<IGrantProgramService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream.GrantProgram);
			var controller = helper.Create();

			// Act
			var result = controller.GetGrantSelection(grantApplication.Id, grantApplication.GrantOpeningId);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationStartViewModel>();
			var model = result.Data as ApplicationStartViewModel;
			model.Id.Should().Be(grantApplication.Id);
			helper.GetMock<IFiscalYearService>().Verify(m => m.GetFiscalYear(It.IsAny<DateTime>()), Times.Once);
			helper.GetMock<IGrantOpeningManageScheduledService>().Verify(m => m.ManageStateTransitions(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantOpeningService>().Verify(m => m.GetGrantOpenings(It.IsAny<DateTime>(), It.IsAny<int>()), Times.Once);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetPrioritySectors(), Times.Once);
			helper.GetMock<IGrantProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void GetGrantSelection_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			var fiscalYear = EntityHelper.CreateFiscalYear();
			helper.GetMock<IFiscalYearService>().Setup(m => m.GetFiscalYear(It.IsAny<DateTime>())).Returns(fiscalYear);
			helper.GetMock<IGrantOpeningManageScheduledService>().Setup(m => m.ManageStateTransitions(It.IsAny<int>()));
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetGrantSelection(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationStartViewModel>();
			var model = result.Data as ApplicationStartViewModel;
			model.Id.Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void GetGrantSelection_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			var fiscalYear = EntityHelper.CreateFiscalYear();
			helper.GetMock<IFiscalYearService>().Setup(m => m.GetFiscalYear(It.IsAny<DateTime>())).Returns(fiscalYear);
			helper.GetMock<IGrantOpeningManageScheduledService>().Setup(m => m.ManageStateTransitions(It.IsAny<int>()));
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetGrantSelection(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationStartViewModel>();
			var model = result.Data as ApplicationStartViewModel;
			model.Id.Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region GetStreamEligibilityRequirements
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void GetStreamEligibilityRequirements()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			var grantOpening = EntityHelper.CreateGrantOpening();
			helper.GetMock<IGrantOpeningService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantOpening);
			var controller = helper.Create();

			// Act
			var result = controller.GetStreamEligibilityRequirements(grantOpening.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantStreamEligibilityViewModel>();
			var model = result.Data as GrantStreamEligibilityViewModel;
			model.Name.Should().Be(grantOpening.GrantStream.Name);
			helper.GetMock<IGrantOpeningService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void GetStreamEligibilityRequirements_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			helper.GetMock<IGrantOpeningService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetStreamEligibilityRequirements(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantStreamEligibilityViewModel>();
			var model = result.Data as GrantStreamEligibilityViewModel;
			model.Name.Should().BeNull();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void GetStreamEligibilityRequirements_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			helper.GetMock<IGrantOpeningService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetStreamEligibilityRequirements(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantStreamEligibilityViewModel>();
			var model = result.Data as GrantStreamEligibilityViewModel;
			model.Name.Should().BeNull();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region AddGrantApplication
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void AddGrantApplication()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var mockGrantOpeningService = helper.GetMock<IGrantOpeningService>();
			mockGrantOpeningService.Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening);
			var mockGrantProgramService = helper.GetMock<IGrantProgramService>();
			mockGrantProgramService.Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream.GrantProgram);
			var mockStaticDataService = helper.GetMock<IStaticDataService>();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.GetDefaultApplicationType()).Returns(grantApplication.ApplicationType);

			var data = new ApplicationStartViewModel(grantApplication, mockGrantOpeningService.Object, mockGrantProgramService.Object, mockStaticDataService.Object);
			var controller = helper.Create();

			// Act
			var result = controller.AddGrantApplication(data);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationStartViewModel>();
			var model = result.Data as ApplicationStartViewModel;
			model.Id.Should().Be(grantApplication.Id);
			helper.GetMock<IGrantOpeningService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Add(It.IsAny<GrantApplication>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void AddGrantApplication_EligiblityNotConfirmed()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.GrantOpening.GrantStream.EligibilityEnabled = true;
			grantApplication.GrantOpening.GrantStream.EligibilityRequired = true;
			grantApplication.EligibilityConfirmed = false;
			var mockGrantOpeningService = helper.GetMock<IGrantOpeningService>();
			mockGrantOpeningService.Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening);
			var mockGrantProgramService = helper.GetMock<IGrantProgramService>();
			mockGrantProgramService.Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream.GrantProgram);
			var mockStaticDataService = helper.GetMock<IStaticDataService>();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.GetDefaultApplicationType()).Returns(grantApplication.ApplicationType);

			var data = new ApplicationStartViewModel(grantApplication, mockGrantOpeningService.Object, mockGrantProgramService.Object, mockStaticDataService.Object);
			var controller = helper.Create();

			// Act
			var result = controller.AddGrantApplication(data);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationStartViewModel>();
			var model = result.Data as ApplicationStartViewModel;
			model.Id.Should().Be(grantApplication.Id);
			model.ValidationErrors.Count().Should().Be(1);
			helper.GetMock<IGrantOpeningService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Add(It.IsAny<GrantApplication>()), Times.Never);
			controller.ControllerContext.HttpContext.Response.StatusCode.Should().Be(400);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void AddGrantApplication_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var mockGrantOpeningService = helper.GetMock<IGrantOpeningService>();
			mockGrantOpeningService.Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var mockGrantProgramService = helper.GetMock<IGrantProgramService>();
			mockGrantProgramService.Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream.GrantProgram);
			var mockStaticDataService = helper.GetMock<IStaticDataService>();

			var data = new ApplicationStartViewModel(grantApplication, mockGrantOpeningService.Object, mockGrantProgramService.Object, mockStaticDataService.Object);
			var controller = helper.Create();

			// Act
			var result = controller.AddGrantApplication(data);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationStartViewModel>();
			var model = result.Data as ApplicationStartViewModel;
			model.Id.Should().Be(grantApplication.Id);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Add(It.IsAny<GrantApplication>()), Times.Never);
		}
		#endregion

		#region UpdateGrantApplication
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void UpdateGrantApplication_WithChanges()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var mockGrantOpeningService = helper.GetMock<IGrantOpeningService>();
			mockGrantOpeningService.Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening);
			var mockGrantProgramService = helper.GetMock<IGrantProgramService>();
			mockGrantProgramService.Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream.GrantProgram);
			var mockStaticDataService = helper.GetMock<IStaticDataService>();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.GetDefaultApplicationType()).Returns(grantApplication.ApplicationType);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.OriginalValue(It.IsAny<GrantApplication>(), ga => ga.StartDate)).Returns(grantApplication.StartDate);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.OriginalValue(It.IsAny<GrantApplication>(), ga => ga.EndDate)).Returns(grantApplication.EndDate);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.OriginalValue(It.IsAny<GrantApplication>(), ga => ga.GrantOpeningId)).Returns(10);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.OriginalValue(It.IsAny<GrantApplication>(), ga => ga.EligibilityConfirmed)).Returns(grantApplication.EligibilityConfirmed);

			var data = new ApplicationStartViewModel(grantApplication, mockGrantOpeningService.Object, mockGrantProgramService.Object, mockStaticDataService.Object);
			var controller = helper.Create();

			// Act
			var result = controller.UpdateGrantApplication(data);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationStartViewModel>();
			var model = result.Data as ApplicationStartViewModel;
			model.Id.Should().Be(grantApplication.Id);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.ChangeGrantOpening(It.IsAny<GrantApplication>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), ApplicationWorkflowTrigger.EditApplication, null), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void UpdateGrantApplication_NoChanges()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var mockGrantOpeningService = helper.GetMock<IGrantOpeningService>();
			mockGrantOpeningService.Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening);
			var mockGrantProgramService = helper.GetMock<IGrantProgramService>();
			mockGrantProgramService.Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream.GrantProgram);
			var mockStaticDataService = helper.GetMock<IStaticDataService>();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.GetDefaultApplicationType()).Returns(grantApplication.ApplicationType);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.OriginalValue(It.IsAny<GrantApplication>(), ga => ga.StartDate)).Returns(grantApplication.StartDate);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.OriginalValue(It.IsAny<GrantApplication>(), ga => ga.EndDate)).Returns(grantApplication.EndDate);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.OriginalValue(It.IsAny<GrantApplication>(), ga => ga.GrantOpeningId)).Returns(grantApplication.GrantOpeningId);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.OriginalValue(It.IsAny<GrantApplication>(), ga => ga.EligibilityConfirmed)).Returns(grantApplication.EligibilityConfirmed);

			var data = new ApplicationStartViewModel(grantApplication, mockGrantOpeningService.Object, mockGrantProgramService.Object, mockStaticDataService.Object);
			var controller = helper.Create();

			// Act
			var result = controller.UpdateGrantApplication(data);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationStartViewModel>();
			var model = result.Data as ApplicationStartViewModel;
			model.Id.Should().Be(grantApplication.Id);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.ChangeGrantOpening(It.IsAny<GrantApplication>()), Times.Never);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), ApplicationWorkflowTrigger.EditApplication, null), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void UpdateGrantApplication_NoChanges_AfterWithdrawn()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.ApplicationStateExternal = ApplicationStateExternal.ApplicationWithdrawn;
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.ApplicationWithdrawn;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var mockGrantOpeningService = helper.GetMock<IGrantOpeningService>();
			mockGrantOpeningService.Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening);
			var mockGrantProgramService = helper.GetMock<IGrantProgramService>();
			mockGrantProgramService.Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream.GrantProgram);
			var mockStaticDataService = helper.GetMock<IStaticDataService>();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.GetDefaultApplicationType()).Returns(grantApplication.ApplicationType);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.OriginalValue(It.IsAny<GrantApplication>(), ga => ga.StartDate)).Returns(grantApplication.StartDate);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.OriginalValue(It.IsAny<GrantApplication>(), ga => ga.EndDate)).Returns(grantApplication.EndDate);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.OriginalValue(It.IsAny<GrantApplication>(), ga => ga.GrantOpeningId)).Returns(grantApplication.GrantOpeningId);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.OriginalValue(It.IsAny<GrantApplication>(), ga => ga.EligibilityConfirmed)).Returns(grantApplication.EligibilityConfirmed);

			var data = new ApplicationStartViewModel(grantApplication, mockGrantOpeningService.Object, mockGrantProgramService.Object, mockStaticDataService.Object);
			var controller = helper.Create();

			// Act
			var result = controller.UpdateGrantApplication(data);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationStartViewModel>();
			var model = result.Data as ApplicationStartViewModel;
			model.Id.Should().Be(grantApplication.Id);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.ChangeGrantOpening(It.IsAny<GrantApplication>()), Times.Never);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), ApplicationWorkflowTrigger.EditApplication, null), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void UpdateGrantApplication_EligiblityNotConfirmed()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.GrantOpening.GrantStream.EligibilityEnabled = true;
			grantApplication.GrantOpening.GrantStream.EligibilityRequired = true;
			grantApplication.EligibilityConfirmed = false;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var mockGrantOpeningService = helper.GetMock<IGrantOpeningService>();
			mockGrantOpeningService.Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening);
			var mockGrantProgramService = helper.GetMock<IGrantProgramService>();
			mockGrantProgramService.Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream.GrantProgram);
			var mockStaticDataService = helper.GetMock<IStaticDataService>();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.GetDefaultApplicationType()).Returns(grantApplication.ApplicationType);

			var data = new ApplicationStartViewModel(grantApplication, mockGrantOpeningService.Object, mockGrantProgramService.Object, mockStaticDataService.Object);
			var controller = helper.Create();

			// Act
			var result = controller.UpdateGrantApplication(data);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationStartViewModel>();
			var model = result.Data as ApplicationStartViewModel;
			model.Id.Should().Be(grantApplication.Id);
			model.ValidationErrors.Count().Should().Be(1);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.ChangeGrantOpening(It.IsAny<GrantApplication>()), Times.Never);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), ApplicationWorkflowTrigger.EditApplication, null), Times.Never);
			controller.ControllerContext.HttpContext.Response.StatusCode.Should().Be(400);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void UpdateGrantApplication_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var mockGrantOpeningService = helper.GetMock<IGrantOpeningService>();
			mockGrantOpeningService.Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening);
			var mockGrantProgramService = helper.GetMock<IGrantProgramService>();
			mockGrantProgramService.Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream.GrantProgram);
			var mockStaticDataService = helper.GetMock<IStaticDataService>();

			var data = new ApplicationStartViewModel(grantApplication, mockGrantOpeningService.Object, mockGrantProgramService.Object, mockStaticDataService.Object);
			var controller = helper.Create();

			// Act
			var result = controller.UpdateGrantApplication(data);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationStartViewModel>();
			var model = result.Data as ApplicationStartViewModel;
			model.Id.Should().Be(grantApplication.Id);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), ApplicationWorkflowTrigger.EditApplication, null), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void UpdateGrantApplication_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var mockGrantOpeningService = helper.GetMock<IGrantOpeningService>();
			mockGrantOpeningService.Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening);
			var mockGrantProgramService = helper.GetMock<IGrantProgramService>();
			mockGrantProgramService.Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication.GrantOpening.GrantStream.GrantProgram);
			var mockStaticDataService = helper.GetMock<IStaticDataService>();

			var data = new ApplicationStartViewModel(grantApplication, mockGrantOpeningService.Object, mockGrantProgramService.Object, mockStaticDataService.Object);
			var controller = helper.Create();

			// Act
			var result = controller.UpdateGrantApplication(data);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationStartViewModel>();
			var model = result.Data as ApplicationStartViewModel;
			model.Id.Should().Be(grantApplication.Id);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), ApplicationWorkflowTrigger.EditApplication, null), Times.Never);
		}
		#endregion

		#region ApplicationOverviewView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void ApplicationOverviewView_Submitable()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationOverviewView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Complete);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.Draft);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void ApplicationOverviewView_NotSubmitable()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.ApplicationStateExternal = ApplicationStateExternal.Complete;
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.Draft);
			grantApplication.GrantOpening.GrantStream.AttachmentsRequired = true;
			grantApplication.GrantOpening.GrantStream.AttachmentsIsEnabled = true;
			grantApplication.Attachments.Clear();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationOverviewView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Incomplete);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.Draft);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void ApplicationOverviewView_NotSubmitable_TrainingPrograms()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.ApplicationStateExternal = ApplicationStateExternal.Complete;
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.Draft);
			grantApplication.TrainingPrograms.First().TrainingProgramState = TrainingProgramStates.Incomplete;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationOverviewView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Incomplete);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.Draft);
			$"{controller.ViewBag.Message}".Should().Be("Skills training dates do not fall within your delivery period and will need to be rescheduled. Make sure all your skills training dates are accurate to your plan.");
			$"{controller.ViewBag.MessageType}".Should().Be("warning");
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void ApplicationOverviewView_Redirect()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.ApplicationStateExternal = ApplicationStateExternal.Submitted;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.ApplicationOverviewView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			var route = result as RedirectToRouteResult;
			route.RouteValues["action"].Should().Be("Index");
			route.RouteValues["controller"].Should().Be("Home");
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void ApplicationOverviewView_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NotAuthorizedException>(() => controller.ApplicationOverviewView(1));
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void ApplicationOverviewView_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NoContentException>(() => controller.ApplicationOverviewView(1));
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}
		#endregion

		#region GetApplicationOverview
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void GetApplicationOverview()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(user);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetApplicationOverview(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationOverviewViewModel>();
			var model = result.Data as ApplicationOverviewViewModel;
			model.Id.Should().Be(grantApplication.Id);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void GetApplicationOverview_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetApplicationOverview(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationOverviewViewModel>();
			var model = result.Data as ApplicationOverviewViewModel;
			model.Id.Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void GetApplicationOverview_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetApplicationOverview(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationOverviewViewModel>();
			var model = result.Data as ApplicationOverviewViewModel;
			model.Id.Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}
		#endregion

		#region DeleteApplication
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void DeleteApplication()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.DeleteApplication(grantApplication.Id, Convert.ToBase64String(grantApplication.RowVersion));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<BaseApplicationViewModel>();
			var model = result.Data as BaseApplicationViewModel;
			model.Id.Should().Be(0);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Delete(It.IsAny<GrantApplication>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void DeleteApplication_InvalidOperationException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.ApplicationStateExternal = ApplicationStateExternal.Submitted;
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.New;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.DeleteApplication(grantApplication.Id, Convert.ToBase64String(grantApplication.RowVersion));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<BaseApplicationViewModel>();
			var model = result.Data as BaseApplicationViewModel;
			model.Id.Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Delete(It.IsAny<GrantApplication>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void DeleteApplication_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.DeleteApplication(grantApplication.Id, Convert.ToBase64String(grantApplication.RowVersion));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<BaseApplicationViewModel>();
			var model = result.Data as BaseApplicationViewModel;
			model.Id.Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Delete(It.IsAny<GrantApplication>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationController))]
		public void DeleteApplication_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var controller = helper.Create();

			// Act
			var result = controller.DeleteApplication(grantApplication.Id, Convert.ToBase64String(grantApplication.RowVersion));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<BaseApplicationViewModel>();
			var model = result.Data as BaseApplicationViewModel;
			model.Id.Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Delete(It.IsAny<GrantApplication>()), Times.Never);
		}
		#endregion
	}
}

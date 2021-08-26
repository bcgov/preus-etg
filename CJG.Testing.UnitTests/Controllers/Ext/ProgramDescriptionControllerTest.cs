using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Ext.Controllers;
using CJG.Web.External.Areas.Ext.Models.ProgramDescriptions;
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
	public class ProgramDescriptionControllerTest
	{
		#region ProgramDescriptionView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ProgramDescriptionController))]
		public void ProgramDescriptionView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ProgramDescriptionController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.ProgramDescriptionView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(AddressController))]
		public void ProgramDescriptionView_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ProgramDescriptionController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NotAuthorizedException>(() => controller.ProgramDescriptionView(1));
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(AddressController))]
		public void ProgramDescriptionView_NotAuthorized_EditApplication()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ProgramDescriptionController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.OfferIssued;
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NotAuthorizedException>(() => controller.ProgramDescriptionView(1));
		}
		#endregion

		#region GetProgramDescription
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ProgramDescriptionController))]
		public void GetProgramDescription()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ProgramDescriptionController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetProgramDescription(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ProgramDescriptionExtViewModel>();
			var model = result.Data as ProgramDescriptionExtViewModel;
			model.Id.Should().Be(grantApplication.Id);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ProgramDescriptionController))]
		public void GetProgramDescription_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ProgramDescriptionController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetProgramDescription(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ProgramDescriptionExtViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ProgramDescriptionController))]
		public void GetProgramDescription_ArgumentNullException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ProgramDescriptionController>(user);

			GrantApplication grantApplication = null;
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetProgramDescription(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ProgramDescriptionExtViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			controller.HttpContext.Response.StatusCode.Should().Be(500);
		}
		#endregion

		#region UpdateProgramDescription
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ProgramDescriptionController))]
		public void UpdateProgramDescription()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ProgramDescriptionController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var viewModel = new ProgramDescriptionExtViewModel()
			{
				Id = grantApplication.Id,
				NumberOfParticipants = 5
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateProgramDescription(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ProgramDescriptionExtViewModel>();
			var model = result.Data as ProgramDescriptionExtViewModel;
			model.Id.Should().Be(grantApplication.Id);
			model.NumberOfParticipants.Should().Be(viewModel.NumberOfParticipants);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>()
				.Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ProgramDescriptionController))]
		public void UpdateProgramDescription_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ProgramDescriptionController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var viewModel = new ProgramDescriptionExtViewModel()
			{
				Id = 1,
				NumberOfParticipants = 5
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateProgramDescription(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ProgramDescriptionExtViewModel>();
			var model = result.Data as ProgramDescriptionExtViewModel;
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>()
				.Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Never);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ProgramDescriptionController))]
		public void UpdateProgramDescription_ArgumentNullException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ProgramDescriptionController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<ArgumentNullException>();
			var viewModel = new ProgramDescriptionExtViewModel()
			{
				Id = 1,
				NumberOfParticipants = 5
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateProgramDescription(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ProgramDescriptionExtViewModel>();
			var model = result.Data as ProgramDescriptionExtViewModel;
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>()
				.Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), null), Times.Never);
			controller.HttpContext.Response.StatusCode.Should().Be(500);
		}
		#endregion

		#region GetMaxCosts
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ProgramDescriptionController))]
		public void GetMaxCosts()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ProgramDescriptionController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetMaxCosts(grantApplication.Id, 5);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			var data = result.Data;
			data.GetReflectedProperty("CostExceeded").Should().Be(true);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ProgramDescriptionController))]
		public void GetMaxCosts_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ProgramDescriptionController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetMaxCosts(1, 1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ProgramDescriptionController))]
		public void GetMaxCosts_ArgumentNullException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ProgramDescriptionController>(user);

			GrantApplication grantApplication = null;
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetMaxCosts(1, 1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			controller.HttpContext.Response.StatusCode.Should().Be(500);
		}
		#endregion
	}
}

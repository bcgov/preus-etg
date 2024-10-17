using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Ext.Controllers;
using CJG.Web.External.Areas.Ext.Models.Agreements;
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
	public class GrantAgreementControllerTest
	{

		#region AgreementReviewView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void AgreementReviewView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.AgreementReviewView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void AgreementReviewView_RedirectToHome()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.GrantAgreement = null;
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.AgreementReviewView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			var routeResult = result as RedirectToRouteResult;
			routeResult.RouteValues.ContainsValue("Index").Should().BeTrue();
			routeResult.RouteValues.ContainsValue("Home").Should().BeTrue();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void AgreementReviewView_RedirectToApplicationDetailsView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.OfferWithdrawn;
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.AgreementReviewView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			var routeResult = result as RedirectToRouteResult;
			routeResult.RouteValues.ContainsValue("ApplicationDetailsView").Should().BeTrue();
			routeResult.RouteValues.ContainsValue("ApplicationView").Should().BeTrue();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}
		#endregion

		#region GetAgreementReview
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void GetAgreementReview()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetAgreementReview(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<AgreementReviewViewModel>();
			var model = result.Data as AgreementReviewViewModel;
			model.GrantApplicationId.Should().Be(grantApplication.Id);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void GetAgreementReview_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetAgreementReview(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<AgreementReviewViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			controller.HttpContext.Response.StatusCode.Should().Be(403);

		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void GetAgreementReview_ArgumentNullException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			GrantApplication grantApplication = null;
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetAgreementReview(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<AgreementReviewViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			controller.HttpContext.Response.StatusCode.Should().Be(500);
		}
		#endregion

		#region CoverLetterView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void CoverLetterView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.CoverLetterView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void CoverLetterView_RedirectToApplicationDetailsView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.OfferWithdrawn;
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.CoverLetterView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			var routeResult = result as RedirectToRouteResult;
			routeResult.RouteValues.ContainsValue("ApplicationDetailsView").Should().BeTrue();
			routeResult.RouteValues.ContainsValue("ApplicationView").Should().BeTrue();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}
		#endregion

		#region ScheduleAView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void ScheduleAView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.ScheduleAView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void ScheduleAView_RedirectToApplicationDetailsView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.OfferWithdrawn;
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.ScheduleAView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			var routeResult = result as RedirectToRouteResult;
			routeResult.RouteValues.ContainsValue("ApplicationDetailsView").Should().BeTrue();
			routeResult.RouteValues.ContainsValue("ApplicationView").Should().BeTrue();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}
		#endregion

		#region ScheduleBView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void ScheduleBView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.ScheduleBView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void ScheduleBView_RedirectToApplicationDetailsView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.OfferWithdrawn;
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.ScheduleBView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			var routeResult = result as RedirectToRouteResult;
			routeResult.RouteValues.ContainsValue("ApplicationDetailsView").Should().BeTrue();
			routeResult.RouteValues.ContainsValue("ApplicationView").Should().BeTrue();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}
		#endregion

		#region GetAgreementDocument
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void GetAgreementDocument_CoverLetter()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetAgreementDocument(grantApplication.Id, "CoverLetter");

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantAgreementDocumentViewModel>();
			var model = result.Data as GrantAgreementDocumentViewModel;
			model.Body.Should().Be(grantApplication.GrantAgreement?.CoverLetter?.Body);
			model.Confirmation.Should().Be(grantApplication.GrantAgreement.CoverLetterConfirmed);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void GetAgreementDocument_ScheduleA()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetAgreementDocument(grantApplication.Id, "ScheduleA");

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantAgreementDocumentViewModel>();
			var model = result.Data as GrantAgreementDocumentViewModel;
			model.Confirmation.Should().Be(grantApplication.GrantAgreement.ScheduleAConfirmed);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void GetAgreementDocument_ScheduleB()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetAgreementDocument(grantApplication.Id, "ScheduleB");

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantAgreementDocumentViewModel>();
			var model = result.Data as GrantAgreementDocumentViewModel;
			model.Body.Should().Be(grantApplication.GrantAgreement?.ScheduleB?.Body);
			model.Confirmation.Should().Be(grantApplication.GrantAgreement.ScheduleBConfirmed);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void GetAgreementDocument()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetAgreementDocument(grantApplication.Id, "test");

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantAgreementDocumentViewModel>();
			var model = result.Data as GrantAgreementDocumentViewModel;
			model.Body.Should().Be(null);
			model.Confirmation.Should().Be(false);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void GetAgreementDocument_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetAgreementDocument(1, "CoverLetter");

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantAgreementDocumentViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void GetAgreementDocument_ArgumentNullException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			GrantApplication grantApplication = null;
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetAgreementDocument(1, "CoverLetter");

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantAgreementDocumentViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			controller.HttpContext.Response.StatusCode.Should().Be(500);
		}
		#endregion

		#region AcceptDocument
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void AcceptDocument_CoverLetter()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var viewModel = new GrantAgreementDocumentViewModel()
			{
				Confirmation = true,
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var controller = helper.Create();

			// Act
			var result = controller.AcceptDocument("CoverLetter", viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantAgreementDocumentViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantAgreementService>().Verify(m => m.Update(It.IsAny<GrantAgreement>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void AcceptDocument_CoverLetter_Error()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var viewModel = new GrantAgreementDocumentViewModel()
			{
				Confirmation = false,
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var controller = helper.Create();

			// Act
			var result = controller.AcceptDocument("CoverLetter", viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantAgreementDocumentViewModel>();
			var model = result.Data as GrantAgreementDocumentViewModel;
			model.ValidationErrors.Count().Should().Be(1);
			model.ValidationErrors[0].Key.Should().Be("Confirmation");
			model.ValidationErrors[0].Value.Should().Be("You must confirm the coverletter.");
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantAgreementService>().Verify(m => m.Update(It.IsAny<GrantAgreement>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void AcceptDocument_ScheduleA()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var viewModel = new GrantAgreementDocumentViewModel()
			{
				Confirmation = true,
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var controller = helper.Create();

			// Act
			var result = controller.AcceptDocument("ScheduleA", viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantAgreementDocumentViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantAgreementService>().Verify(m => m.Update(It.IsAny<GrantAgreement>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void AcceptDocument_ScheduleA_Error()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var viewModel = new GrantAgreementDocumentViewModel()
			{
				Confirmation = false,
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var controller = helper.Create();

			// Act
			var result = controller.AcceptDocument("ScheduleA", viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantAgreementDocumentViewModel>();
			var model = result.Data as GrantAgreementDocumentViewModel;
			model.ValidationErrors.Count().Should().Be(1);
			model.ValidationErrors[0].Key.Should().Be("Confirmation");
			model.ValidationErrors[0].Value.Should().Be("You must confirm schedule A.");
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantAgreementService>().Verify(m => m.Update(It.IsAny<GrantAgreement>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void AcceptDocument_ScheduleB()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var viewModel = new GrantAgreementDocumentViewModel()
			{
				Confirmation = true,
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var controller = helper.Create();

			// Act
			var result = controller.AcceptDocument("ScheduleB", viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantAgreementDocumentViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantAgreementService>().Verify(m => m.Update(It.IsAny<GrantAgreement>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void AcceptDocument_ScheduleB_Error()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var viewModel = new GrantAgreementDocumentViewModel()
			{
				Confirmation = false,
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var controller = helper.Create();

			// Act
			var result = controller.AcceptDocument("ScheduleB", viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantAgreementDocumentViewModel>();
			var model = result.Data as GrantAgreementDocumentViewModel;
			model.ValidationErrors.Count().Should().Be(1);
			model.ValidationErrors[0].Key.Should().Be("Confirmation");
			model.ValidationErrors[0].Value.Should().Be("You must confirm schedule B.");
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantAgreementService>().Verify(m => m.Update(It.IsAny<GrantAgreement>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void AcceptDocument_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var viewModel = new GrantAgreementDocumentViewModel()
			{
				Confirmation = false,
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var controller = helper.Create();

			// Act
			var result = controller.AcceptDocument("ScheduleB", viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantAgreementDocumentViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantAgreementService>().Verify(m => m.Update(It.IsAny<GrantAgreement>()), Times.Never);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void AcceptDocument_ArgumentNullException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<IGrantAgreementService>()
				.Setup(m => m.Update(It.IsAny<GrantAgreement>()))
				.Throws<ArgumentNullException>();
			var viewModel = new GrantAgreementDocumentViewModel()
			{
				Confirmation = false,
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var controller = helper.Create();

			// Act
			var result = controller.AcceptDocument("ScheduleB", viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantAgreementDocumentViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantAgreementService>().Verify(m => m.Update(It.IsAny<GrantAgreement>()), Times.Once);
			controller.HttpContext.Response.StatusCode.Should().Be(500);
		}
		#endregion

		#region AcceptAgreement
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void AcceptAgreement()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var viewModel = new AgreementReviewViewModel()
			{
				CoverLetterConfirmed = true,
				ScheduleAConfirmed = true,
				ScheduleBConfirmed = true,
				GrantApplicationId = grantApplication.Id,
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var controller = helper.Create();

			// Act
			var result = controller.AcceptAgreement(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<AgreementReviewViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantAgreementService>().Verify(m => m.AcceptGrantAgreement(It.IsAny<GrantApplication>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void AcceptAgreement_Error()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var viewModel = new AgreementReviewViewModel()
			{
				CoverLetterConfirmed = false,
				ScheduleAConfirmed = false,
				ScheduleBConfirmed = false,
				GrantApplicationId = grantApplication.Id,
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var controller = helper.Create();

			// Act
			var result = controller.AcceptAgreement(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<AgreementReviewViewModel>();
			var model = result.Data as AgreementReviewViewModel;
			model.ValidationErrors.Count().Should().Be(3);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<IGrantAgreementService>()
				.Verify(m => m.AcceptGrantAgreement(It.IsAny<GrantApplication>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void AcceptAgreement_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var viewModel = new AgreementReviewViewModel()
			{
				CoverLetterConfirmed = true,
				ScheduleAConfirmed = true,
				ScheduleBConfirmed = true,
				GrantApplicationId = 1,
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var controller = helper.Create();

			// Act
			var result = controller.AcceptAgreement(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<AgreementReviewViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantAgreementService>()
				.Verify(m => m.AcceptGrantAgreement(It.IsAny<GrantApplication>()), Times.Never);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void AcceptAgreement_ArgumentNullException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<IGrantAgreementService>()
				.Setup(m => m.AcceptGrantAgreement(It.IsAny<GrantApplication>()))
				.Throws<ArgumentNullException>();
			var viewModel = new AgreementReviewViewModel()
			{
				CoverLetterConfirmed = true,
				ScheduleAConfirmed = true,
				ScheduleBConfirmed = true,
				GrantApplicationId = 1,
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var controller = helper.Create();

			// Act
			var result = controller.AcceptAgreement(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<AgreementReviewViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantAgreementService>()
				.Verify(m => m.AcceptGrantAgreement(It.IsAny<GrantApplication>()), Times.Once);
			controller.HttpContext.Response.StatusCode.Should().Be(500);
		}
		#endregion

		#region AgreementOverviewView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void AgreementOverviewView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.AgreementOverviewView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void AgreementOverviewView_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NotAuthorizedException>(() => controller.AgreementOverviewView(1));
		}
		#endregion

		#region CancelAgreementView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void CancelAgreementView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.CancelAgreementView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<PartialViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
		}
		#endregion

		#region CancelAgreement
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void CancelAgreement()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var viewModel = new CancelAgreementViewModal()
			{
				Id = 1,
				RowVersion = "AgQGCAoMDhASFA==",
				CancelReason = "test"
			};
			var controller = helper.Create();

			// Act
			var result = controller.CancelAgreement(viewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<CancelAgreementViewModal>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantAgreementService>().Verify(
				m => m.CancelGrantAgreement(It.IsAny<GrantApplication>(), It.IsAny<string>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void CancelAgreement_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var viewModel = new CancelAgreementViewModal()
			{
				Id = 1,
				RowVersion = "AgQGCAoMDhASFA==",
				CancelReason = "test"
			};
			var controller = helper.Create();

			// Act
			var result = controller.CancelAgreement(viewModel);
			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<CancelAgreementViewModal>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantAgreementService>().Verify(
							m => m.CancelGrantAgreement(It.IsAny<GrantApplication>(), It.IsAny<string>()), Times.Never);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion
	}
}

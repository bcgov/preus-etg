using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Ext.Controllers;
using CJG.Web.External.Areas.Ext.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Testing.UnitTests.Controllers.Ext
{
	[TestClass]
	public class ReportingControllerTest
	{

		#region GrantFileView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ReportingController))]
		public void GrantFileView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ReportingController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.AgreementAccepted;
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GrantFileView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			grantApplication.GetCurrentClaim().Id.Should().Be(controller.ViewBag.ClaimId);
			grantApplication.GetCurrentClaim().ClaimVersion.Should().Be(controller.ViewBag.ClaimVersion);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ReportingController))]
		public void GrantFileView_Redirect()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ReportingController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GrantFileView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			var routeResult = result as RedirectToRouteResult;
			routeResult.RouteValues.ContainsValue("Index").Should().BeTrue();
			routeResult.RouteValues.ContainsValue("Home").Should().BeTrue();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ReportingController))]
		public void GrantFileView_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ReportingController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NotAuthorizedException>(() => controller.GrantFileView(1));
		}
		#endregion

		#region GetGrantFile
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ReportingController))]
		public void GetGrantFile()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ReportingController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<ICompletionReportService>()
				.Setup(m => m.GetCompletionReportStatus(It.IsAny<int>()))
				.Returns("Incomplete");
			var controller = helper.Create();

			// Act
			var result = controller.GetGrantFile(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantFileViewModel>();
			var model = result.Data as GrantFileViewModel;
			model.Id.Should().Be(grantApplication.Id);
			model.EnableSubmit.Should().Be(false);
			model.HasClaim.Should().Be(true);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ReportingController))]
		public void GetGrantFile_AgreementAccepted()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ReportingController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.ParticipantForms.Add(EntityHelper.CreateParticipantForm(grantApplication));
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.AgreementAccepted;
			grantApplication.GetCurrentClaim().ClaimState = ClaimState.Complete;
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<ICompletionReportService>()
				.Setup(m => m.GetCompletionReportStatus(It.IsAny<int>()))
				.Returns("Submitted");
			var controller = helper.Create();

			// Act
			var result = controller.GetGrantFile(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantFileViewModel>();
			var model = result.Data as GrantFileViewModel;
			model.Id.Should().Be(grantApplication.Id);
			model.EnableSubmit.Should().Be(true);
			model.AllowReviewAndSubmit.Should().Be(true);
			model.AllowClaimReport.Should().Be(true);
			model.AllowParticipantReport.Should().Be(true);
			model.HasClaim.Should().Be(true);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ReportingController))]
		public void GetGrantFile_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ReportingController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetGrantFile(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantFileViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion
	}
}

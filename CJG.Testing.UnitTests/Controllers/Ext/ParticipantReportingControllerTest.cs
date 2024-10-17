using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Ext.Controllers;
using CJG.Web.External.Areas.Ext.Models.ParticipantReporting;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Testing.UnitTests.Controllers.Ext
{
	[TestClass]
	public class ParticipantReportingControllerTest
	{

		#region ParticipantReportingView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ParticipantReportingController))]
		public void ParticipantReportingView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ParticipantReportingController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.AgreementAccepted;
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.ParticipantReportingView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ParticipantReportingController))]
		public void ParticipantReportingView_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ParticipantReportingController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NotAuthorizedException>(() => controller.ParticipantReportingView(1));
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ParticipantReportingController))]
		public void ParticipantReportingView_Redirect()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ParticipantReportingController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.AgreementRejected;
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.ParticipantReportingView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			var routeResult = result as RedirectToRouteResult;
			routeResult.RouteValues.ContainsValue("ApplicationDetailsView").Should().BeTrue();
			routeResult.RouteValues.ContainsValue("ApplicationView").Should().BeTrue();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}
		#endregion

		#region GetParticipantReporting
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ParticipantReportingController))]
		public void GetParticipantReporting()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ParticipantReportingController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.AgreementAccepted;
			var participantForms = EntityHelper.CreateParticipantForm(grantApplication);
			grantApplication.ParticipantForms = new List<ParticipantForm>() { participantForms };
			helper.GetMock<IGrantApplicationService>()
					.Setup(m => m.Get(It.IsAny<int>()))
					.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetParticipantReporting(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ReportingViewModel>();
			var model = result.Data as ReportingViewModel;
			model.GrantApplicationId.Should().Be(grantApplication.Id);
			model.GrantProgramName.Should().Be(grantApplication.GrantOpening.GrantStream.GrantProgram.Name);
			model.MaxParticipantsAllowed.Should().Be(grantApplication.GetMaxParticipants());
			model.InvitationKey.Should().Be(grantApplication.InvitationKey);
			model.Participants.Count().Should().Be(grantApplication.ParticipantForms.Count());
			model.Participants.First().LastName.Should().Be(participantForms.LastName);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ParticipantReportingController))]
		public void GetParticipantReporting_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ParticipantReportingController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetParticipantReporting(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ReportingViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ParticipantReportingController))]
		public void GetParticipantReporting_NotViewParticipant()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ParticipantReportingController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.AgreementRejected;
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetParticipantReporting(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ReportingViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion

		#region RemoveParticipant
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ParticipantReportingController))]
		public void RemoveParticipant()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ParticipantReportingController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.AgreementAccepted;
			var participantForm = EntityHelper.CreateParticipantForm(grantApplication);

			helper.GetMock<IParticipantService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(participantForm);
			var controller = helper.Create();
			var model = new ParticipantViewModel()
			{
				Id = 1,
				RowVersion = "AgQGCAoMDhASFA=="
			};

			// Act
			var result = controller.RemoveParticipant(model);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			var resultModel = ((JsonResult)result).Data as ReportingViewModel;
			resultModel.GrantApplicationId.Should().Be(grantApplication.Id);
			helper.GetMock<IParticipantService>()
				.Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IParticipantService>()
				.Verify(m => m.RemoveParticipant(It.IsAny<ParticipantForm>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ParticipantReportingController))]
		public void RemoveParticipant_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ParticipantReportingController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication();
			var participantForm = EntityHelper.CreateParticipantForm(grantApplication);

			helper.GetMock<IParticipantService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();
			var model = new ParticipantViewModel()
			{
				Id = 1,
				RowVersion = "AgQGCAoMDhASFA=="
			};

			// Act
			var result = controller.RemoveParticipant(model);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			var resultModel = ((JsonResult)result).Data as ReportingViewModel;
			helper.GetMock<IParticipantService>()
				.Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IParticipantService>()
				.Verify(m => m.RemoveParticipant(It.IsAny<ParticipantForm>()), Times.Never);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion

		#region ToggleParticipant
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ParticipantReportingController))]
		public void ToggleParticipant_Exclude()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ParticipantReportingController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication();
			var participantForm = EntityHelper.CreateParticipantForm(grantApplication);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<IParticipantService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(participantForm);
			var controller = helper.Create();
			var model = new IncludeParticipantsViewModel()
			{
				Id = 1,
				GrantApplicationId = 1,
				ClaimRowVersion = "AgQGCAoMDhASFA==",
				ParticipantFormIds = new int[] { 1 },
				Include = false
			};

			// Act
			var result = controller.ToggleParticipant(model);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			var resultModel = result.Data as ReportingViewModel;
			resultModel.GrantApplicationId.Should().Be(grantApplication.Id);
			resultModel.Participants.Count().Should().Be(0);
			helper.GetMock<IGrantApplicationService>()
				.Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IParticipantService>()
				.Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IParticipantService>()
				.Verify(m => m.IncludeParticipant(It.IsAny<ParticipantForm>()), Times.Never);
			helper.GetMock<IParticipantService>()
				.Verify(m => m.ExcludeParticipant(It.IsAny<ParticipantForm>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ParticipantReportingController))]
		public void ToggleParticipant_Include()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ParticipantReportingController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication();
			var participantForm = EntityHelper.CreateParticipantForm(grantApplication);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<IParticipantService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(participantForm);
			var controller = helper.Create();
			var model = new IncludeParticipantsViewModel()
			{
				Id = 1,
				GrantApplicationId = 1,
				ClaimRowVersion = "AgQGCAoMDhASFA==",
				ParticipantFormIds = new int[] { 1 },
				Include = true
			};

			// Act
			var result = controller.ToggleParticipant(model);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			var resultModel = result.Data as ReportingViewModel;
			resultModel.GrantApplicationId.Should().Be(grantApplication.Id);
			helper.GetMock<IGrantApplicationService>()
				.Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IParticipantService>()
				.Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IParticipantService>()
				.Verify(m => m.IncludeParticipant(It.IsAny<ParticipantForm>()), Times.Once);
			helper.GetMock<IParticipantService>()
				.Verify(m => m.ExcludeParticipant(It.IsAny<ParticipantForm>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ParticipantReportingController))]
		public void ToggleParticipant_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ParticipantReportingController>(user);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();
			var model = new IncludeParticipantsViewModel()
			{
				Id = 1,
				GrantApplicationId = 1,
				ClaimRowVersion = "AgQGCAoMDhASFA==",
				ParticipantFormIds = new int[] { 1 },
				Include = false
			};

			// Act
			var result = controller.ToggleParticipant(model);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			var resultModel = result.Data as ReportingViewModel;
			helper.GetMock<IGrantApplicationService>()
				.Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IParticipantService>()
				.Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<IParticipantService>()
				.Verify(m => m.IncludeParticipant(It.IsAny<ParticipantForm>()), Times.Never);
			helper.GetMock<IParticipantService>()
				.Verify(m => m.ExcludeParticipant(It.IsAny<ParticipantForm>()), Times.Never);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion
	}
}

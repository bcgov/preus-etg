using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Ext.Controllers;
using CJG.Web.External.Areas.Ext.Models.Reports;
using CJG.Web.External.Models.Shared.Reports;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CJG.Testing.UnitTests.Controllers.Ext
{
	[TestClass]
	public class CompletionReportingControllerTest
	{
		#region CompletionReportView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(CompletionReportingController))]
		public void CompletionReportView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<CompletionReportingController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.CompletionReporting;
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.CompletionReportView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(CompletionReportingController))]
		public void CompletionReportView_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<CompletionReportingController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NotAuthorizedException>(() => controller.CompletionReportView(1));
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(CompletionReportingController))]
		public void CompletionReportView_Closed()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<CompletionReportingController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.ApplicationStateExternal = ApplicationStateExternal.Closed;
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.CompletionReportView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			var routeResult = result as RedirectToRouteResult;
			routeResult.RouteValues.ContainsValue("CompletionReportDetailsView").Should().BeTrue();
			routeResult.RouteValues.ContainsValue("Error").Should().BeTrue();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(CompletionReportingController))]
		public void CompletionReportView_UnAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<CompletionReportingController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.CompletionReportView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			var routeResult = result as RedirectToRouteResult;
			routeResult.RouteValues.ContainsValue("GrantFileView").Should().BeTrue();
			routeResult.RouteValues.ContainsValue("Reporting").Should().BeTrue();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}
		#endregion

		#region GetCompletionReport
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(CompletionReportingController))]
		public void GetCompletionReport()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<CompletionReportingController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetCompletionReport(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<CompletionReportViewModel>();
			var model = result.Data as CompletionReportViewModel;
			model.GrantApplicationId.Should().Be(grantApplication.Id);
			model.ProgramName.Should().Be(grantApplication.GrantOpening.GrantStream.GrantProgram.Name);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(CompletionReportingController))]
		public void GetCompletionReport_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<CompletionReportingController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetCompletionReport(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<CompletionReportViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion

		#region GetCompletionReportGroup
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(CompletionReportingController))]
		public void GetCompletionReportGroup()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<CompletionReportingController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			var completionReportGroup = EntityHelper.CreateCompletionReportGroup();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<ICompletionReportService>()
				.Setup(m => m.GetCompletionReportGroup(It.IsAny<int>(), It.IsAny<int>()))
				.Returns(completionReportGroup);
			var controller = helper.Create();

			// Act
			var result = controller.GetCompletionReportGroup(grantApplication.Id, completionReportGroup.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<CompletionReportGroupViewModel>();
			var model = result.Data as CompletionReportGroupViewModel;
			model.GrantApplicationId.Should().Be(grantApplication.Id);
			model.Title.Should().Be(completionReportGroup.Title);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ICompletionReportService>().Verify(m => m.GetCompletionReportGroup(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(CompletionReportingController))]
		public void GetCompletionReportGroup_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<CompletionReportingController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetCompletionReportGroup(1, 1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<CompletionReportGroupViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ICompletionReportService>().Verify(m => m.GetCompletionReportGroup(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
		}
		#endregion

		#region GetEmploymentServicesAndSupports
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(CompletionReportingController))]
		public void GetEmploymentServicesAndSupports()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<CompletionReportingController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetEmploymentServicesAndSupports(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<CompletionReportDynamicCheckboxViewModel>();
			var model = result.Data as CompletionReportDynamicCheckboxViewModel;
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(CompletionReportingController))]
		public void GetEmploymentServicesAndSupports_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<CompletionReportingController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetEmploymentServicesAndSupports(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<CompletionReportDynamicCheckboxViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}
		#endregion

		#region GetNAICS
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(CompletionReportingController))]
		public void GetNAICS()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<CompletionReportingController>(user);

			var NAICS = EntityHelper.CreateNaIndustryClassificationSystem();
			var naIndustryClassificationSystemObjects =
				new List<NaIndustryClassificationSystem>()
			{
				NAICS
			};
			helper.GetMock<INaIndustryClassificationSystemService>()
				.Setup(m => m.GetNaIndustryClassificationSystemChildren(It.IsAny<int>(), It.IsAny<int>()))
				.Returns(naIndustryClassificationSystemObjects);
			var controller = helper.Create();

			// Act
			var result = controller.GetNAICS(1, 1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			helper.GetMock<INaIndustryClassificationSystemService>()
				.Verify(m => m.GetNaIndustryClassificationSystemChildren(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
		}
		#endregion

		#region GetNOCs
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(CompletionReportingController))]
		public void GetNOCs()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<CompletionReportingController>(user);

			var NOC = EntityHelper.CreateNationalOccupationalClassification();
			var nationalOccupationalClassificationObjects =
				new List<NationalOccupationalClassification>()
			{
				NOC
			};
			helper.GetMock<INationalOccupationalClassificationService>()
				.Setup(m => m.GetNationalOccupationalClassificationChildren(It.IsAny<int>(), It.IsAny<int>()))
				.Returns(nationalOccupationalClassificationObjects);
			var controller = helper.Create();

			// Act
			var result = controller.GetNOCs(1, 1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			helper.GetMock<INationalOccupationalClassificationService>()
				.Verify(m => m.GetNationalOccupationalClassificationChildren(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
		}
		#endregion

		#region CompletionReportDetailsView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(CompletionReportingController))]
		public void CompletionReportDetailsView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<CompletionReportingController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.CompletionReportDetailsView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
		}
		#endregion

		#region GetCompletionReportDetails
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(CompletionReportingController))]
		public void GetCompletionReportDetails()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<CompletionReportingController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetCompletionReportDetails(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<CompletionReportDetailsViewModel>();
			var model = result.Data as CompletionReportDetailsViewModel;
			model.ProgramTitleLabel.FileNumber.Should().Be(grantApplication.FileNumber);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(CompletionReportingController))]
		public void GetCompletionReportDetails_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<CompletionReportingController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetCompletionReportDetails(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<CompletionReportDetailsViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion

		#region ShowCompletionReportStatus
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(CompletionReportingController))]
		public void ShowCompletionReportStatus()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<CompletionReportingController>(user);

			helper.GetMock<ICompletionReportService>()
				.Setup(m => m.GetCompletionReportStatus(It.IsAny<int>()))
				.Returns("Incomplete");
			var controller = helper.Create();

			// Act
			var result = controller.ShowCompletionReportStatus(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<PartialViewResult>();
			var routeResult = result as PartialViewResult;

			helper.GetMock<ICompletionReportService>()
				.Verify(m => m.GetCompletionReportStatus(It.IsAny<int>()), Times.Once);
		}
		#endregion

		#region UpdateCompletionReport
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(CompletionReportingController))]
		public void UpdateCompletionReport()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<CompletionReportingController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.CompletionReporting;
			grantApplication.CompletionReportId = CJG.Core.Entities.Constants.CompletionReportETG;
			var completionReportGroup = EntityHelper.CreateCompletionReportGroup();
			var completionReport = EntityHelper.CreateCompletionReport();
			var completionReportQuestionsForSteps = new List<CompletionReportQuestion>()
			{
				EntityHelper.CreateCompletionReportQuestion()
			};
			var model = new CompletionReportGroupViewModel()
			{
				Title = completionReportGroup.Title,
				GrantApplicationId = grantApplication.Id,
				Id = 1,
				Questions = new List<CompletionReportQuestionViewModel>()
			};
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<ICompletionReportService>()
				.Setup(m => m.GetCompletionReportForParticipants(It.IsAny<int[]>()))
				.Returns(completionReport);
			helper.GetMock<ICompletionReportService>()
				.Setup(m => m.GetCompletionReportQuestionsForStep(It.IsAny<int>(), It.IsAny<int>()))
				.Returns(completionReportQuestionsForSteps);
			helper.GetMock<ICompletionReportService>()
				.Setup(m => m.RecordCompletionReportAnswersForStep(
					It.IsAny<int>(),
					It.IsAny<IEnumerable<ParticipantCompletionReportAnswer>>(),
					It.IsAny<IEnumerable<EmployerCompletionReportAnswer>>(),
					It.IsAny<int>(),
					It.IsAny<int[]>()))
				.Returns(false);
			var controller = helper.Create();

			// Act
			var result = controller.UpdateCompletionReport(model);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			var resultModel = result.Data as CompletionReportGroupViewModel;
			resultModel.Id.Should().Be(model.Id);
			helper.GetMock<IGrantApplicationService>()
				.Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ICompletionReportService>()
				.Verify(m => m.DeleteAnswersFor(It.IsAny<int[]>(), It.IsAny<int[]>()), Times.Exactly(2));
			helper.GetMock<ICompletionReportService>()
				.Verify(m => m.GetCompletionReportForParticipants(It.IsAny<int[]>()), Times.Once);
			helper.GetMock<ICompletionReportService>()
				.Verify(m => m.GetCompletionReportQuestionsForStep(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(CompletionReportingController))]
		public void UpdateCompletionReport_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<CompletionReportingController>(user);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();

			var controller = helper.Create();
			var model = new CompletionReportGroupViewModel()
			{
				Title = "test",
				GrantApplicationId = 1,
				Id = 1,
				Questions = new List<CompletionReportQuestionViewModel>()
			};
			// Act
			var result = controller.UpdateCompletionReport(model);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			var resultModel = result.Data as CompletionReportGroupViewModel;
			resultModel.Id.Should().Be(model.Id);
			helper.GetMock<IGrantApplicationService>()
				.Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ICompletionReportService>()
				.Verify(m => m.DeleteAnswersFor(It.IsAny<int[]>(), It.IsAny<int[]>()), Times.Never);
			helper.GetMock<ICompletionReportService>()
				.Verify(m => m.GetCompletionReportForParticipants(It.IsAny<int[]>()), Times.Never);
			helper.GetMock<ICompletionReportService>()
				.Verify(m => m.GetCompletionReportQuestionsForStep(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion
	}
}

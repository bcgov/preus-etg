using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Int.Controllers;
using CJG.Web.External.Areas.Int.Models.BatchApprovals;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static CJG.Testing.Core.ServiceHelper;

namespace CJG.Testing.UnitTests.Controllers.Int
{
	[TestClass]
	public class DirectorDashboardControllerTest
	{
		#region ApplicationBatchApprovalView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(DirectorDashboardController))]
		public void ApplicationBatchApprovalView()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<DirectorDashboardController>(user, Roles.Assessor);

			var controller = helper.Create();

			// Act
			var result = controller.ApplicationBatchApprovalView();

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
		}
		#endregion

		#region GetAssessors
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(DirectorDashboardController))]
		public void GetAssessors()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<DirectorDashboardController>(user, Roles.Assessor);
			var controller = helper.Create();

			var internalUser = EntityHelper.CreateInternalUser();
			helper.GetMock<IAuthorizationService>().Setup(m => m.GetAssessors())
				.Returns(new List<InternalUser>() { internalUser });

			// Act
			var result = controller.GetAssessors();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<KeyValuePair<int, string>[]>();
			var data = result.Data as KeyValuePair<int, string>[];
			data.Count().Should().Be(1);
			data.First().Key.Should().Be(internalUser.Id);
			data.First().Value.Should().Be(internalUser.FirstName + ' ' + internalUser.LastName);
			helper.GetMock<IAuthorizationService>().Verify(m => m.GetAssessors(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(DirectorDashboardController))]
		public void GetAssessors_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<DirectorDashboardController>(user, Roles.Assessor);

			helper.GetMock<IAuthorizationService>().Setup(m => m.GetAssessors()).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetAssessors();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
			helper.GetMock<IAuthorizationService>().Verify(m => m.GetAssessors(), Times.Once);
		}
		#endregion

		#region GetFiscalYears
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(DirectorDashboardController))]
		public void GetFiscalYears()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<DirectorDashboardController>(user, Roles.Assessor);
			var controller = helper.Create();
			var fiscalYear = EntityHelper.CreateFiscalYear();
			helper.GetMock<IFiscalYearService>().Setup(m => m.GetFiscalYears())
				.Returns(new List<FiscalYear>() { fiscalYear });

			// Act
			var result = controller.GetFiscalYears();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<KeyValuePair<int, string>[]>();
			var data = result.Data as KeyValuePair<int, string>[];
			data.Count().Should().Be(1);
			data.First().Key.Should().Be(fiscalYear.Id);
			data.First().Value.Should().Be(fiscalYear.Caption);
			helper.GetMock<IFiscalYearService>().Verify(m => m.GetFiscalYears(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(DirectorDashboardController))]
		public void GetFiscalYears_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<DirectorDashboardController>(user, Roles.Assessor);

			helper.GetMock<IFiscalYearService>().Setup(m => m.GetFiscalYears()).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetFiscalYears();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
			helper.GetMock<IFiscalYearService>().Verify(m => m.GetFiscalYears(), Times.Once);
		}
		#endregion

		#region GetTrainingPeriods
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(DirectorDashboardController))]
		public void GetTrainingPeriods()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<DirectorDashboardController>(user, Roles.Assessor);
			var controller = helper.Create();
			var trainingPeriod = EntityHelper.CreateTrainingPeriod(DateTime.UtcNow.AddDays(-3), DateTime.UtcNow);
			trainingPeriod.FiscalYear = EntityHelper.CreateFiscalYear();
			helper.GetMock<IFiscalYearService>().Setup(m => m.GetTrainingPeriods(It.IsAny<int>()))
				.Returns(new List<TrainingPeriod>() { trainingPeriod });

			// Act
			var result = controller.GetTrainingPeriods(trainingPeriod.FiscalYear.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<KeyValuePair<int, string>[]>();
			var data = result.Data as KeyValuePair<int, string>[];
			data.Count().Should().Be(1);
			data.First().Key.Should().Be(trainingPeriod.Id);
			data.First().Value.Should().Be(trainingPeriod.Caption);
			helper.GetMock<IFiscalYearService>().Verify(m => m.GetTrainingPeriods(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(DirectorDashboardController))]
		public void GetTrainingPeriods_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<DirectorDashboardController>(user, Roles.Assessor);

			helper.GetMock<IFiscalYearService>().Setup(m => m.GetTrainingPeriods(It.IsAny<int>())).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingPeriods(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
			helper.GetMock<IFiscalYearService>().Verify(m => m.GetTrainingPeriods(It.IsAny<int>()), Times.Once);
		}
		#endregion

		#region GetGrantPrograms
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(DirectorDashboardController))]
		public void GetGrantPrograms()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<DirectorDashboardController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantProgram = EntityHelper.CreateGrantProgram();
			helper.GetMock<IGrantProgramService>().Setup(m => m.GetAll(It.IsAny<GrantProgramStates?>()))
				.Returns(new List<GrantProgram>() { grantProgram });

			// Act
			var result = controller.GetGrantPrograms();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<KeyValuePair<int, string>[]>();
			var data = result.Data as KeyValuePair<int, string>[];
			data.Count().Should().Be(1);
			data.First().Key.Should().Be(grantProgram.Id);
			data.First().Value.Should().Be(grantProgram.Name);
			helper.GetMock<IGrantProgramService>().Verify(m => m.GetAll(It.IsAny<GrantProgramStates?>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(DirectorDashboardController))]
		public void GetGrantPrograms_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<DirectorDashboardController>(user, Roles.Assessor);

			helper.GetMock<IGrantProgramService>().Setup(m => m.GetAll(It.IsAny<GrantProgramStates?>())).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetGrantPrograms();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
			helper.GetMock<IGrantProgramService>().Verify(m => m.GetAll(It.IsAny<GrantProgramStates?>()), Times.Once);
		}
		#endregion

		#region GetGrantStreams
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(DirectorDashboardController))]
		public void GetGrantStreams()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<DirectorDashboardController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantProgram = EntityHelper.CreateGrantProgram();
			var grantStream = EntityHelper.CreateGrantStream(grantProgram);
			
			helper.GetMock<IGrantStreamService>()
				.Setup(m => m.GetGrantStreamsForProgram(It.IsAny<int>(), It.IsAny<bool>()))
				.Returns(new List<GrantStream>() { grantStream });

			// Act
			var result = controller.GetGrantStreams(grantProgram.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<KeyValuePair<int, string>[]>();
			var data = result.Data as KeyValuePair<int, string>[];
			data.Count().Should().Be(1);
			data.First().Key.Should().Be(grantStream.Id);
			data.First().Value.Should().Be(grantStream.Name);
			helper.GetMock<IGrantStreamService>().Verify(m => m.GetGrantStreamsForProgram(It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(DirectorDashboardController))]
		public void GetGrantStreams_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<DirectorDashboardController>(user, Roles.Assessor);

			helper.GetMock<IGrantStreamService>()
				.Setup(m => m.GetGrantStreamsForProgram(It.IsAny<int>(), It.IsAny<bool>()))
				.Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetGrantStreams(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
			helper.GetMock<IGrantStreamService>().Verify(m => m.GetGrantStreamsForProgram(It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
		}
		#endregion

		#region GetGrantApplications
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(DirectorDashboardController))]
		public void GetGrantApplications()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<DirectorDashboardController>(user, Roles.Assessor);
			var controller = helper.Create();
			controller.Request.QueryString["page"] = "1";
			controller.Request.QueryString["quantity"] = "5";
			var grantApplication = EntityHelper.CreateGrantApplication();
			var pageList = new PageList<GrantApplication>()
			{
				Items = new List<GrantApplication>() { grantApplication },
				Page = 1,
				Quantity = 1,
				Total = 1
			};
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.GetGrantApplications(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ApplicationFilter>()))
				.Returns(pageList);
			var filter = new Web.External.Areas.Int.Models.BatchApprovals.BatchApprovalFilterViewModel()
			{
				AssessorId = 1,
				FiscalYearId = 1,
				GrantProgramId = 1,
				GrantStreamId = 1,
				TrainingPeriodId = 1
			};
			// Act
			var result = controller.GetGrantApplications(filter);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<PageList<Web.External.Areas.Int.Models.BatchApprovals.GrantApplicationViewModel>>();
			var data = result.Data as PageList<Web.External.Areas.Int.Models.BatchApprovals.GrantApplicationViewModel>;
			data.Page.Should().Be(pageList.Page);
			data.Quantity.Should().Be(pageList.Quantity);
			data.Total.Should().Be(pageList.Total);
			data.Items.First().Id.Should().Be(grantApplication.Id);
			helper.GetMock<IGrantApplicationService>()
				.Verify(m => m.GetGrantApplications(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ApplicationFilter>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(DirectorDashboardController))]
		public void GetGrantApplications_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<DirectorDashboardController>(user, Roles.Assessor);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.GetGrantApplications(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ApplicationFilter>()))
				.Throws<NoContentException>();
			var controller = helper.Create();
			controller.Request.QueryString["page"] = "1";
			controller.Request.QueryString["quantity"] = "5";
			var filter = new Web.External.Areas.Int.Models.BatchApprovals.BatchApprovalFilterViewModel()
			{
				AssessorId = 1,
				FiscalYearId = 1,
				GrantProgramId = 1,
				GrantStreamId = 1,
				TrainingPeriodId = 1
			};
			// Act
			var result = controller.GetGrantApplications(filter);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
			helper.GetMock<IGrantApplicationService>()
							.Verify(m => m.GetGrantApplications(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ApplicationFilter>()), Times.Once);
		}
		#endregion

		#region IssueOffers
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(DirectorDashboardController))]
		public void IssueOffers()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<DirectorDashboardController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			var issueOfferViewModel = new IssueOfferViewModel()
			{
				Id = grantApplication.Id,
				RowVersion = Convert.ToBase64String(grantApplication.RowVersion),
				FileNumber = grantApplication.FileNumber
			};

			var batchApprovalViewModel = new Web.External.Areas.Int.Models.BatchApprovals.BatchApprovalViewModel()
			{
				SelectAll = true,
				AssessorId = 1,
				FiscalYearId = 1,
				GrantProgramId = 1,
				GrantStreamId = 1,
				TrainingPeriodId = 1,
				Total = 1,
				GrantApplications = new List<IssueOfferViewModel>() { issueOfferViewModel }
			};

			var pageList = new PageList<GrantApplication>()
			{
				Items = new List<GrantApplication>() { grantApplication },
				Page = 1,
				Quantity = 1,
				Total = 1
			};
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.GetGrantApplications(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ApplicationFilter>()))
				.Returns(pageList);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var filter = new Web.External.Areas.Int.Models.BatchApprovals.BatchApprovalFilterViewModel()
			{
				AssessorId = 1,
				FiscalYearId = 1,
				GrantProgramId = 1,
				GrantStreamId = 1,
				TrainingPeriodId = 1
			};
			// Act
			var result = controller.IssueOffers(batchApprovalViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<Web.External.Areas.Int.Models.BatchApprovals.BatchApprovalViewModel>();
			var data = result.Data as Web.External.Areas.Int.Models.BatchApprovals.BatchApprovalViewModel;
			data.GrantApplications.First().Id.Should().Be(grantApplication.Id);
			data.GrantApplications.First().FileNumber.Should().Be(grantApplication.FileNumber);
			helper.GetMock<IGrantApplicationService>()
				.Verify(m => m.GetGrantApplications(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ApplicationFilter>()), Times.Once);
			helper.GetMock<IGrantApplicationService>()
				.Verify(m => m.IssueOffer(It.IsAny<GrantApplication>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(DirectorDashboardController))]
		public void IssueOffers_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<DirectorDashboardController>(user, Roles.Assessor);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.GetGrantApplications(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ApplicationFilter>()))
				.Throws<NoContentException>();
			var controller = helper.Create();
			controller.Request.QueryString["page"] = "1";
			controller.Request.QueryString["quantity"] = "5";
			var filter = new Web.External.Areas.Int.Models.BatchApprovals.BatchApprovalFilterViewModel()
			{
				AssessorId = 1,
				FiscalYearId = 1,
				GrantProgramId = 1,
				GrantStreamId = 1,
				TrainingPeriodId = 1
			};
			// Act
			var result = controller.GetGrantApplications(filter);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
			helper.GetMock<IGrantApplicationService>()
							.Verify(m => m.GetGrantApplications(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ApplicationFilter>()), Times.Once);
		}
		#endregion
	}
}


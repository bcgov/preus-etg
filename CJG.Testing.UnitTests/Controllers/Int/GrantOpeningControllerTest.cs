using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Int.Controllers;
using CJG.Web.External.Areas.Int.Models.Agreements;
using CJG.Web.External.Areas.Int.Models.GrantOpenings;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static CJG.Testing.Core.ServiceHelper;

namespace CJG.Testing.UnitTests.Controllers.Int
{
	[TestClass]
	public class GrantOpeningControllerTest
	{
		#region GrantOpeningView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantOpeningController))]
		public void GrantOpeningView()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<GrantOpeningController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantProgram = EntityHelper.CreateGrantProgram();
			var fiscalYear = EntityHelper.CreateFiscalYear();
			helper.GetMock<IStaticDataService>().Setup(m => m.GetFiscalYear(It.IsAny<int>())).Returns(fiscalYear);

			// Act
			var result = controller.GrantOpeningView(null, grantProgram.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantProgram.Id.Should().Be(controller.ViewBag.GrantProgramId);
			fiscalYear.Id.Should().Be(controller.ViewBag.FiscalYearId);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetFiscalYear(It.IsAny<int>()), Times.Once);
		}
		#endregion

		#region GetGrantOpenings
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantOpeningController))]
		public void GetGrantOpenings()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<GrantOpeningController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantOpening = EntityHelper.CreateGrantOpening();
			var grantProgram = EntityHelper.CreateGrantProgram();
			var fiscalYear = EntityHelper.CreateFiscalYear();
			helper.GetMock<IGrantOpeningService>().Setup(m => m.GetGrantOpening(It.IsAny<int>(), It.IsAny<int>())).Returns(grantOpening);
			helper.GetMock<IGrantProgramService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantProgram);
			helper.GetMock<IStaticDataService>().Setup(m => m.GetFiscalYear(It.IsAny<int>())).Returns(fiscalYear);

			// Act
			var result = controller.GetGrantOpenings(fiscalYear.Id, grantProgram.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<FiscalGrantOpeningViewModel>();
			var model = result.Data as FiscalGrantOpeningViewModel;
			helper.GetMock<IStaticDataService>().Verify(m => m.GetFiscalYear(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantOpeningController))]
		public void GetGrantOpenings_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<GrantOpeningController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantProgram = EntityHelper.CreateGrantProgram();
			var fiscalYear = EntityHelper.CreateFiscalYear();
			helper.GetMock<IStaticDataService>().Setup(m => m.GetFiscalYear(It.IsAny<int>())).Throws<NotAuthorizedException>();

			// Act
			var result = controller.GetGrantOpenings(fiscalYear.Id, grantProgram.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<FiscalGrantOpeningViewModel>();
			helper.GetMock<IStaticDataService>().Verify(m => m.GetFiscalYear(It.IsAny<int>()), Times.Once);
			controller.ControllerContext.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion

		#region GetGrantOpeningModalView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantOpeningController))]
		public void GetGrantOpeningModalView()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<GrantOpeningController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();

			// Act
			var result = controller.GetGrantOpeningModalView();

			// Assert
			result.Should().NotBeNull().And.BeOfType<PartialViewResult>();
		}
		#endregion

		#region GetInitialGrantOpening
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantOpeningController))]
		public void GetInitialGrantOpening()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<GrantOpeningController>(user, Roles.Assessor);
			var controller = helper.Create();
			var trainingPeriod = EntityHelper.CreateTrainingPeriod(AppDateTime.Now, AppDateTime.Now);
			var grantProgram = EntityHelper.CreateGrantProgram();
			var grantStream = EntityHelper.CreateGrantStream(grantProgram);
			helper.GetMock<IGrantOpeningService>().Setup(m => m.CheckGrantOpeningByFiscalAndStream(It.IsAny<int>(), It.IsAny<int>())).Returns(false);

			// Act
			var result = controller.GetInitialGrantOpening(trainingPeriod.Id, grantStream.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantOpeningViewModel>();
			var model = result.Data as GrantOpeningViewModel;
			helper.GetMock<IGrantOpeningService>().Verify(m => m.CheckGrantOpeningByFiscalAndStream(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
		}
		#endregion

		#region GetGrantOpening
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantOpeningController))]
		public void GetGrantOpening()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantOpeningController>(user);

			var grantOpening = EntityHelper.CreateGrantOpening();
			helper.GetMock<IGrantOpeningService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantOpening);

			var controller = helper.Create();

			// Act
			var result = controller.GetGrantOpening(grantOpening.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantOpeningViewModel>();
			var model = result.Data as GrantOpeningViewModel;
			model.Id.Should().Be(grantOpening.Id);
			helper.GetMock<IGrantOpeningService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantOpeningController))]
		public void GetGrantOpening_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantOpeningController>(user);

			helper.GetMock<IGrantOpeningService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetGrantOpening(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantOpeningViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IGrantOpeningService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantOpeningController))]
		public void GetGrantOpening_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantOpeningController>(user);

			helper.GetMock<IGrantOpeningService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NoContentException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetGrantOpening(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantOpeningViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
			helper.GetMock<IGrantOpeningService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}
		#endregion

		#region AddGrantOpening
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantOpeningController))]
		public void AddGrantOpening()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantOpeningController>(user);
			var trainingPeriod = EntityHelper.CreateTrainingPeriod(AppDateTime.Now, AppDateTime.Now);
			var grantProgram = EntityHelper.CreateGrantProgram();
			var grantStream = EntityHelper.CreateGrantStream(grantProgram);

			helper.GetMock<IGrantOpeningService>()
				.Setup(m => m.CheckGrantOpeningByFiscalAndStream(It.IsAny<int>(), It.IsAny<int>()))
				.Returns(false);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetTrainingPeriod(It.IsAny<int>()))
				.Returns(trainingPeriod);
			helper.GetMock<IGrantStreamService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantStream);
			var grantOpeningViewModel = new GrantOpeningViewModel()
			{
				Id = 0,
				RowVersion = "AgQGCAoMDhASFA==",
				TrainingPeriodId = 1,
				GrantStreamId = 1,
				DeniedAmt = 0,
				WithdrawnAmt = 0,
				ReductionAmt = 0,
				SlippageAmt = 0,
				CancellationAmt = 0,
				IntakeTargetRate = 0,
				IsScheduleEnabled = true,
				IsFinancialEnabled = true,
				IsPublishDateEnabled = true,
				IsOpeningDateEnabled = true,
				IsClosingDateEnabled = true,
				IsReturnRefundEnabled = true,
				NumberUnfundedApplications = 1,
				AllowDeleteGrantOpening = true,
				IsUserGM1 = true,
				IsTraingEndDateInThePass = true,
				GrantStreamName = "",
				TrainingPeriodCaption = "",
				TrainingPeriodStartDate = AppDateTime.Now,
				TrainingPeriodEndDate = AppDateTime.Now,
				State = GrantOpeningStates.Open,
				ScheduleStartDate = AppDateTime.Now,
				ScheduleEndDate = AppDateTime.Now,
				PublishDate = AppDateTime.Now,
				OpeningDate = AppDateTime.Now,
				ClosingDate = AppDateTime.Now,
				OriginalBudgetAllocationAmt = 0,
				BudgetAllocationAmt = 0,
				OriginalPlanDeniedRate = 0,
				PlanDeniedRate = 0,
				OriginalPlanWithdrawnRate = 0,
				PlanWithdrawnRate = 0,
				OriginalPlanReductionRate = 0,
				PlanReductionRate = 0,
				OriginalPlanSlippageRate = 0,
				PlanSlippageRate = 0,
				OriginalPlanCancellationRate = 0,
				PlanCancellationRate = 0,
				IntakeTargetAmt = 0
			};
			var controller = helper.Create();

			// Act
			var result = controller.AddGrantOpening(grantOpeningViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantOpeningViewModel>();
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingPeriod(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantStreamService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantOpeningService>().Verify(m => m.Add(It.IsAny<GrantOpening>()), Times.Once);
		}
		#endregion

		#region UpdateGrantOpening
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantOpeningController))]
		public void UpdateGrantOpening()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<GrantOpeningController>(user);
			var grantOpening = EntityHelper.CreateGrantOpening();
			var trainingPeriod = EntityHelper.CreateTrainingPeriod(AppDateTime.Now, AppDateTime.Now);
			var grantProgram = EntityHelper.CreateGrantProgram();
			var grantStream = EntityHelper.CreateGrantStream(grantProgram);

			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetTrainingPeriod(It.IsAny<int>()))
				.Returns(trainingPeriod);
			helper.GetMock<IGrantStreamService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantStream);
			helper.GetMock<IGrantOpeningService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantOpening);
			var grantOpeningViewModel = new GrantOpeningViewModel()
			{
				Id = 1,
				RowVersion = "AgQGCAoMDhASFA==",
				TrainingPeriodId = 1,
				GrantStreamId = 1,
				DeniedAmt = 0,
				WithdrawnAmt = 0,
				ReductionAmt = 0,
				SlippageAmt = 0,
				CancellationAmt = 0,
				IntakeTargetRate = 0,
				IsScheduleEnabled = true,
				IsFinancialEnabled = true,
				IsPublishDateEnabled = true,
				IsOpeningDateEnabled = true,
				IsClosingDateEnabled = true,
				IsReturnRefundEnabled = true,
				NumberUnfundedApplications = 1,
				AllowDeleteGrantOpening = true,
				IsUserGM1 = true,
				IsTraingEndDateInThePass = true,
				GrantStreamName = "",
				TrainingPeriodCaption = "",
				TrainingPeriodStartDate = AppDateTime.Now,
				TrainingPeriodEndDate = AppDateTime.Now,
				State = GrantOpeningStates.Open,
				ScheduleStartDate = AppDateTime.Now,
				ScheduleEndDate = AppDateTime.Now,
				PublishDate = AppDateTime.Now,
				OpeningDate = AppDateTime.Now,
				ClosingDate = AppDateTime.Now,
				OriginalBudgetAllocationAmt = 0,
				BudgetAllocationAmt = 0,
				OriginalPlanDeniedRate = 0,
				PlanDeniedRate = 0,
				OriginalPlanWithdrawnRate = 0,
				PlanWithdrawnRate = 0,
				OriginalPlanReductionRate = 0,
				PlanReductionRate = 0,
				OriginalPlanSlippageRate = 0,
				PlanSlippageRate = 0,
				OriginalPlanCancellationRate = 0,
				PlanCancellationRate = 0,
				IntakeTargetAmt = 0
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateGrantOpening(grantOpeningViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantOpeningViewModel>();
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingPeriod(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantStreamService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantOpeningService>().Verify(m => m.Update(It.IsAny<GrantOpening>()), Times.Once);
		}
		#endregion
	}
}

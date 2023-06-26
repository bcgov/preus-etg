using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Ext.Controllers;
using CJG.Web.External.Areas.Ext.Models.TrainingCosts;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace CJG.Testing.UnitTests.Controllers.Ext
{
	[TestClass]
	public class TrainingCostControllerTest
	{
		#region TrainingCostView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingCostController))]
		public void TrainingCostView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingCostController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.TrainingCostView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingCostController))]
		public void TrainingCostView_NoContentxception()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser(2);
			var helper = new ControllerHelper<TrainingCostController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NoContentException>(() => controller.TrainingCostView(1));
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingCostController))]
		public void TrainingCostView_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser(2);
			var helper = new ControllerHelper<TrainingCostController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NotAuthorizedException>(() => controller.TrainingCostView(1));
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingCostController))]
		public void TrainingCostView_EditTrainingCosts_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser(2);
			var helper = new ControllerHelper<TrainingCostController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(EntityHelper.CreateExternalUser(1));
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NotAuthorizedException>(() => controller.TrainingCostView(1));
		}
		#endregion

		#region GetTrainingCosts
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingCostController))]
		public void GetTrainingCosts()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingCostController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);

			var eligibleExpenseTypes = new[] { EntityHelper.CreateEligibleExpenseType(1) };
			helper.GetMock<IGrantStreamService>().Setup(m => m.GetAutoIncludeActiveEligibleExpenseTypes(It.IsAny<int>())).Returns(eligibleExpenseTypes);

			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingCosts(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingCostViewModel>();
			var model = result.Data as TrainingCostViewModel;
			model.GrantApplicationId.Should().Be(grantApplication.Id);
			helper.GetMock<IGrantStreamService>().Verify(m => m.GetAutoIncludeActiveEligibleExpenseTypes(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			model.EligibleCosts.Should().HaveCount(1);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingCostController))]
		public void GetTrainingCosts_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingCostController>(user);

			var fiscalYear = EntityHelper.CreateFiscalYear();
			helper.GetMock<IFiscalYearService>().Setup(m => m.GetFiscalYear(It.IsAny<DateTime>())).Returns(fiscalYear);
			helper.GetMock<IGrantOpeningManageScheduledService>().Setup(m => m.ManageStateTransitions(It.IsAny<int>()));
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingCosts(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingCostViewModel>();
			var model = result.Data as TrainingCostViewModel;
			model.Id.Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingCostController))]
		public void GetTrainingCosts_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingCostController>(user);

			var fiscalYear = EntityHelper.CreateFiscalYear();
			helper.GetMock<IFiscalYearService>().Setup(m => m.GetFiscalYear(It.IsAny<DateTime>())).Returns(fiscalYear);
			helper.GetMock<IGrantOpeningManageScheduledService>().Setup(m => m.ManageStateTransitions(It.IsAny<int>()));
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingCosts(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingCostViewModel>();
			var model = result.Data as TrainingCostViewModel;
			model.Id.Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region UpdateTrainingCosts
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingCostController))]
		public void UpdateTrainingCosts()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ControllerHelper<TrainingCostController>(identity);

			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(user);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var mockGrantStreamService = helper.GetMock<IGrantStreamService>();
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var eligibleExpenseTypes = new[] { eligibleExpenseType };
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get<EligibleExpenseType>(It.IsAny<int>())).Returns(eligibleExpenseType);
			mockGrantStreamService.Setup(m => m.GetAutoIncludeActiveEligibleExpenseTypes(It.IsAny<int>())).Returns(eligibleExpenseTypes);

			var data = new TrainingCostViewModel(grantApplication, identity, mockGrantStreamService.Object);
			var controller = helper.Create();

			var jsonData = JsonConvert.SerializeObject(data);
			// Act
			var result = controller.UpdateTrainingCosts(jsonData, null);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingCostViewModel>();
			var model = result.Data as TrainingCostViewModel;
			model.GrantApplicationId.Should().Be(grantApplication.Id);
			helper.GetMock<IGrantStreamService>().Verify(m => m.GetAutoIncludeActiveEligibleExpenseTypes(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get<EligibleExpenseType>(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get<EligibleCostBreakdown>(It.IsAny<int>()), Times.Never);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.UpdateTrainingCosts(It.IsAny<GrantApplication>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingCostController))]
		public void UpdateTrainingCosts_InvalidModel()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ControllerHelper<TrainingCostController>(identity);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var mockGrantStreamService = helper.GetMock<IGrantStreamService>();

			var data = new TrainingCostViewModel(grantApplication, identity, mockGrantStreamService.Object);
			var controller = helper.CreateWithModel(data);

			// Act
			var jsonData = JsonConvert.SerializeObject(data);
			var result = controller.UpdateTrainingCosts(jsonData, null);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingCostViewModel>();
			var model = result.Data as TrainingCostViewModel;
			model.GrantApplicationId.Should().Be(grantApplication.Id);
			model.ValidationErrors.Should().HaveCount(1);
			helper.GetMock<IGrantStreamService>().Verify(m => m.GetAutoIncludeActiveEligibleExpenseTypes(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get<EligibleExpenseType>(It.IsAny<int>()), Times.Never);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get<EligibleCostBreakdown>(It.IsAny<int>()), Times.Never);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.UpdateTrainingCosts(It.IsAny<GrantApplication>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingCostController))]
		[Ignore]
		public void UpdateTrainingCosts_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ControllerHelper<TrainingCostController>(identity);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var mockGrantStreamService = helper.GetMock<IGrantStreamService>();

			var data = new TrainingCostViewModel(grantApplication, identity, mockGrantStreamService.Object);
			var controller = helper.Create();

			// Act
			var jsonData = JsonConvert.SerializeObject(data);
			var result = controller.UpdateTrainingCosts(jsonData, null);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingCostViewModel>();
			var model = result.Data as TrainingCostViewModel;
			model.GrantApplicationId.Should().Be(grantApplication.Id);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), ApplicationWorkflowTrigger.EditApplication, null), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingCostController))]
		[Ignore]
		public void UpdateTrainingCosts_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ControllerHelper<TrainingCostController>(identity);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var mockGrantStreamService = helper.GetMock<IGrantStreamService>();

			var data = new TrainingCostViewModel(grantApplication, identity, mockGrantStreamService.Object);
			var controller = helper.Create();

			// Act
			var jsonData = JsonConvert.SerializeObject(data);
			var result = controller.UpdateTrainingCosts(jsonData, null);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingCostViewModel>();
			var model = result.Data as TrainingCostViewModel;
			model.GrantApplicationId.Should().Be(grantApplication.Id);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Update(It.IsAny<GrantApplication>(), ApplicationWorkflowTrigger.EditApplication, null), Times.Never);
		}
		#endregion

		#region GetEligibleExpenseTypes
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingCostController))]
		public void GetEligibleExpenseTypes()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingCostController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var expenseTypes = new[] { EntityHelper.CreateEligibleExpenseType() };
			helper.GetMock<IGrantStreamService>().Setup(m => m.GetAllActiveEligibleExpenseTypes(It.IsAny<int>())).Returns(expenseTypes);
			var controller = helper.Create();

			// Act
			var result = controller.GetEligibleExpenseTypes(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<EligibleExpenseTypeModel>>();
			var model = result.Data as List<EligibleExpenseTypeModel>;
			model.Should().HaveCount(1);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantStreamService>().Verify(m => m.GetAllActiveEligibleExpenseTypes(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingCostController))]
		public void GetEligibleExpenseTypes_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingCostController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetEligibleExpenseTypes(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<EligibleExpenseTypeModel>>();
			var model = result.Data as List<EligibleExpenseTypeModel>;
			model.Should().HaveCount(0);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingCostController))]
		public void GetEligibleExpenseTypes_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingCostController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetEligibleExpenseTypes(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<EligibleExpenseTypeModel>>();
			var model = result.Data as List<EligibleExpenseTypeModel>;
			model.Should().HaveCount(0);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion
	}
}

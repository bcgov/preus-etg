using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Ext.Controllers;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Areas.Ext.Models.Agreements;
using CJG.Web.External.Areas.Ext.Models.ChangeRequest;
using CJG.Web.External.Areas.Ext.Models.Reports;
using CJG.Web.External.Models.Shared.Reports;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CJG.Testing.UnitTests.Controllers.Ext
{
	[TestClass]
	public class ChangeRequestControllerTest
	{
		#region SubmitChangeRequest
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ChangeRequestController))]
		public void SubmitChangeRequest()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = user.CreateIdentity();
			var helper = new ControllerHelper<ChangeRequestController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var agreementOverviewViewModel = new AgreementOverviewViewModel(grantApplication, identity);
			var controller = helper.Create();

			// Act
			var result = controller.SubmitChangeRequest(agreementOverviewViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.SubmitChangeRequest(grantApplication, ""), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ChangeRequestController))]
		public void SubmitChangeRequest_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = user.CreateIdentity();
			var helper = new ControllerHelper<ChangeRequestController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var agreementOverviewViewModel = new AgreementOverviewViewModel(grantApplication, identity);
			var controller = helper.Create();

			// Act
			var result = controller.SubmitChangeRequest(agreementOverviewViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<AgreementOverviewViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion

		#region CancelChangeRequest
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ChangeRequestController))]
		public void CancelChangeRequest_ProgramTypeEmployerGrant()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = user.CreateIdentity();
			var helper = new ControllerHelper<ChangeRequestController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			grantApplication.TrainingPrograms.FirstOrDefault().TrainingProvider.TrainingProviderState = TrainingProviderStates.RequestApproved;
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var agreementOverviewViewModel = new AgreementOverviewViewModel(grantApplication, identity);
			var controller = helper.Create();

			// Act
			var result = controller.CancelChangeRequest(agreementOverviewViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<AgreementOverviewViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.AtLeastOnce);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.DeleteRequestedTrainingProvider(It.IsAny<TrainingProvider>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ChangeRequestController))]
		public void CancelChangeRequest_ProgramTypeWDAService()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = user.CreateIdentity();
			var helper = new ControllerHelper<ChangeRequestController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication, trainingProvider);
			trainingProgram.EligibleCostBreakdown =
				EntityHelper.CreateEligibleCostBreakdown(EntityHelper.CreateEligibleCost(grantApplication), new EligibleExpenseBreakdown());
			grantApplication.TrainingPrograms.Add(trainingProgram);
			grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId = ProgramTypes.WDAService;
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var agreementOverviewViewModel = new AgreementOverviewViewModel(grantApplication, identity);
			var controller = helper.Create();

			// Act
			var result = controller.CancelChangeRequest(agreementOverviewViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<AgreementOverviewViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.AtLeastOnce);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.DeleteRequestedTrainingProvider(It.IsAny<TrainingProvider>()), Times.Never);
		}


		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ChangeRequestController))]
		public void CancelChangeRequest_ProgramTypeWDAService_DeleteRequestedTrainingProvider()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = user.CreateIdentity();
			var helper = new ControllerHelper<ChangeRequestController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			trainingProvider.TrainingProviderState = TrainingProviderStates.RequestApproved;
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication, trainingProvider);
			trainingProgram.TrainingProviders.Add(trainingProvider);			
			trainingProgram.EligibleCostBreakdown =
				EntityHelper.CreateEligibleCostBreakdown(EntityHelper.CreateEligibleCost(grantApplication), new EligibleExpenseBreakdown());
			grantApplication.TrainingPrograms.Add(trainingProgram);
			grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId = ProgramTypes.WDAService;

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var agreementOverviewViewModel = new AgreementOverviewViewModel(grantApplication, identity);
			var controller = helper.Create();

			// Act
			var result = controller.CancelChangeRequest(agreementOverviewViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<AgreementOverviewViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.AtLeastOnce);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.DeleteRequestedTrainingProvider(It.IsAny<TrainingProvider>()), Times.AtLeastOnce);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ChangeRequestController))]
		public void CancelChangeRequest_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = user.CreateIdentity();
			var helper = new ControllerHelper<ChangeRequestController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var agreementOverviewViewModel = new AgreementOverviewViewModel(grantApplication, identity);
			var controller = helper.Create();

			// Act
			var result = controller.CancelChangeRequest(agreementOverviewViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<AgreementOverviewViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion region

		#region ChangeDeliveryDatesView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ChangeRequestController))]
		public void ChangeDeliveryDatesView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ChangeRequestController>(user);
			var controller = helper.Create();

			// Act
			var result = controller.ChangeDeliveryDatesView();

			// Assert
			result.Should().NotBeNull().And.BeOfType<PartialViewResult>();
			var viewResult = result as PartialViewResult;
			viewResult.ViewName.Should().Be("_DeliveryDatesView");
		}
		#endregion

		#region ChangeDeliveryDates
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ChangeRequestController))]
		public void ChangeDeliveryDates()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = user.CreateIdentity();
			var helper = new ControllerHelper<ChangeRequestController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.Claims.First().ClaimState = ClaimState.Unassessed;
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.AgreementAccepted;
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication, trainingProvider);
			trainingProvider.TrainingProviderState = TrainingProviderStates.RequestApproved;
			trainingProgram.TrainingProviders.Add(trainingProvider);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var model = new DeliveryDateViewModel();
			model.RowVersion = "AgQGCAoMDhASFA==";
			var controller = helper.Create();

			// Act
			var result = controller.ChangeDeliveryDates(model);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<AgreementOverviewViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.UpdateDeliveryDates(It.IsAny<GrantApplication>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ChangeRequestController))]
		public void ChangeDeliveryDates_InvalidOperationException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ChangeRequestController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication();			
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var model = new DeliveryDateViewModel();
			var controller = helper.Create();

			// Act
			var result = controller.ChangeDeliveryDates(model);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			var resultModel = result.Data as AgreementOverviewViewModel;
			resultModel.ValidationErrors.Should().NotBeEmpty();
			resultModel.ValidationErrors.First().Value.Should().Contain("A claim is currently being processed, you cannot submit a change request.");
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ChangeRequestController))]
		public void ChangeDeliveryDates_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ChangeRequestController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var model = new DeliveryDateViewModel();
			var controller = helper.Create();

			// Act
			var result = controller.ChangeDeliveryDates(model);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<AgreementOverviewViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion

		#region GetProgramDatesView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ChangeRequestController))]
		public void GetProgramDatesView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ChangeRequestController>(user);
			var controller = helper.Create();

			// Act
			var result = controller.GetProgramDatesView();

			// Assert
			result.Should().NotBeNull().And.BeOfType<PartialViewResult>();
			var viewResult = result as PartialViewResult;
			viewResult.ViewName.Should().Be("_ProgramDatesView");
		}
		#endregion

		#region ChangeProgramDates
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ChangeRequestController))]
		public void ChangeProgramDates()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = user.CreateIdentity();
			var helper = new ControllerHelper<ChangeRequestController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication, trainingProvider);
			trainingProvider.TrainingProviderState = TrainingProviderStates.RequestApproved;
			trainingProgram.TrainingProviders.Add(trainingProvider);
			helper.GetMock<ITrainingProgramService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProgram);
			var model = new ChangeRequestProgramTrainingDateViewModel();
			model.RowVersion = "AgQGCAoMDhASFA==";
			var controller = helper.Create();

			// Act
			var result = controller.ChangeProgramDates(model);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<AgreementOverviewViewModel>();
			helper.GetMock<ITrainingProgramService>().Verify(m => m.UpdateProgramDates(It.IsAny<TrainingProgram>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ChangeRequestController))]
		public void ChangeProgramDates_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ChangeRequestController>(user);
			helper.GetMock<ITrainingProgramService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var model = new ChangeRequestProgramTrainingDateViewModel();
			var controller = helper.Create();

			// Act
			var result = controller.ChangeProgramDates(model);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<AgreementOverviewViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion

		#region ChangeTrainingProviderView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ChangeRequestController))]
		public void ChangeTrainingProviderView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ChangeRequestController>(user);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			helper.GetMock<ITrainingProviderService>()
				.Setup(x => x.Get(It.IsAny<int>()))
				.Returns(trainingProvider);

			// Act
			var result = controller.ChangeTrainingProviderView(trainingProvider.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<PartialViewResult>();
			var viewResult = result as PartialViewResult;
			viewResult.ViewName.Should().Be("_ChangeTrainingProviderView");
			trainingProvider.Id.Should().Be(controller.ViewBag.OriginalTrainingProviderId);
			Assert.AreEqual(0, controller.ViewBag.TrainingProviderId);
		}
		#endregion

		#region GetTrainingProvider
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ChangeRequestController))]
		public void GetTrainingProvider()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ChangeRequestController>(user);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			helper.GetMock<ITrainingProviderService>()
				.Setup(x => x.Get(It.IsAny<int>()))
				.Returns(trainingProvider);

			// Act
			var result = controller.GetTrainingProvider(trainingProvider.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<RequestedTrainingProviderViewModel>();
			var viewResult = result.Data as RequestedTrainingProviderViewModel;
			viewResult.OriginalTrainingProviderId = trainingProvider.Id;
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ChangeRequestController))]
		public void GetTrainingProvider_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ChangeRequestController>(user);
			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingProvider(It.IsAny<int>());

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<RequestedTrainingProviderViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion

		#region DeleteTrainingProvider
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ChangeRequestController))]
		public void DeleteTrainingProvider()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ChangeRequestController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			helper.GetMock<ITrainingProviderService>()
				.Setup(x => x.Get(It.IsAny<int>()))
				.Returns(trainingProvider);
			var controller = helper.Create();
			var model = new RequestedTrainingProviderViewModel();
			model.RowVersion = "AgQGCAoMDhASFA==";

			// Act
			var result = controller.DeleteTrainingProvider(model);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<AgreementOverviewViewModel>();
			helper.GetMock<ITrainingProviderService>().Verify(x => x.DeleteRequestedTrainingProvider(trainingProvider), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ChangeRequestController))]
		public void DeleteTrainingProvider_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ChangeRequestController>(user);
			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();
			var model = new RequestedTrainingProviderViewModel();

			// Act
			var result = controller.DeleteTrainingProvider(model);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<AgreementOverviewViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion

		#region AddTrainingProvider
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ChangeRequestController))]
		public void AddTrainingProvider_NullException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ChangeRequestController>(user);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			var model = new RequestedTrainingProviderViewModel(trainingProvider);
			var jsonModel = JsonConvert.SerializeObject(model);

			// Act
			var result = controller.AddTrainingProvider(It.IsAny<HttpPostedFileBase[]>(), jsonModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<AgreementOverviewViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(500);
		}
		#endregion

		#region UpdateTrainingProvider
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ChangeRequestController))]
		public void UpdateTrainingProvider_NullException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ChangeRequestController>(user);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			var model = new RequestedTrainingProviderViewModel(trainingProvider);
			var jsonObject = JsonConvert.SerializeObject(model);

			// Act
			var result = controller.UpdateTrainingProvider(It.IsAny<HttpPostedFileBase[]>(), jsonObject);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<AgreementOverviewViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(500);
		}
		#endregion

		#region ChangeServiceProviderView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ChangeRequestController))]
		public void ChangeServiceProviderView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ChangeRequestController>(user);
			var trainingProvider = new TrainingProvider();
			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProvider);
			var controller = helper.Create();

			// Act
			var result = controller.ChangeServiceProviderView(trainingProvider.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<PartialViewResult>();
			var viewResult = result as PartialViewResult;
			viewResult.ViewName.Should().Be("_ChangeServiceProviderView");
			trainingProvider.Id.Should().Be(controller.ViewBag.OriginalTrainingProviderId);
			Assert.AreEqual(0, controller.ViewBag.TrainingProviderId);
		}
		#endregion

		#region GetServiceProvider
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ChangeRequestController))]
		public void GetServiceProvider()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ChangeRequestController>(user);
			var trainingProvider = new TrainingProvider();
			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProvider);
			var controller = helper.Create();

			// Act
			var result = controller.GetServiceProvider(trainingProvider.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<RequestedTrainingProviderViewModel>();
			helper.GetMock<ITrainingProviderService>().Verify(x => x.Get(trainingProvider.Id), Times.Once);
			var resultModel = result.Data as RequestedTrainingProviderViewModel;
			resultModel.TrainingOutsideBC.Should().Be(false);
			Assert.AreEqual(resultModel.OriginalTrainingProviderId, trainingProvider.RequestedTrainingProvider.Id);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ChangeRequestController))]
		public void GetServiceProvider_Exception()
		{
			//Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ChangeRequestController>(user);
			var trainingProvider = new TrainingProvider();
			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetServiceProvider(trainingProvider.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<RequestedTrainingProviderViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion
	}
}

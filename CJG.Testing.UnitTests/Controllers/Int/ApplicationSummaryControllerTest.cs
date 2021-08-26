using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Int.Controllers;
using CJG.Web.External.Areas.Int.Models.Applications;
using CJG.Web.External.Areas.Int.Models.Organizations;
using CJG.Web.External.Areas.Int.Models.TrainingPrograms;
using CJG.Web.External.Models.Shared;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static CJG.Testing.Core.ServiceHelper;

namespace CJG.Testing.UnitTests.Controllers.Int
{
	[TestClass]
	public class ApplicationSummaryControllerTest
	{
		#region GetSummary
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationSummaryController))]
		public void GetSummary()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicationSummaryController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetSummary(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationSummaryViewModel>();
			var model = result.Data as ApplicationSummaryViewModel;
			model.Id.Should().Be(grantApplication.Id);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void GetSummary_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicationSummaryController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetSummary(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationSummaryViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}
		#endregion

		#region GetDeliveryPartners
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationSummaryController))]
		public void GetDeliveryPartners()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicationSummaryController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var grantProgram = EntityHelper.CreateGrantProgram();
			var deliveryPartner = EntityHelper.CreateDeliveryPartner(grantProgram);
			helper.GetMock<IDeliveryPartnerService>()
				.Setup(m => m.GetDeliveryPartners(It.IsAny<int>()))
				.Returns(new List<DeliveryPartner>() { deliveryPartner });
			var controller = helper.Create();

			// Act
			var result = controller.GetDeliveryPartners(grantProgram.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<KeyValuePair<int, string>[]>();
			var model = result.Data as KeyValuePair<int, string>[];
			model.Count().Should().Be(1);
			model[0].Key.Should().Be(deliveryPartner.Id);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void GetDeliveryPartners_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicationSummaryController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var grantProgram = EntityHelper.CreateGrantProgram();
			var deliveryPartner = EntityHelper.CreateDeliveryPartner(grantProgram);
			helper.GetMock<IDeliveryPartnerService>()
				.Setup(m => m.GetDeliveryPartners(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetDeliveryPartners(grantProgram.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<KeyValuePair<int, string>[]>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IDeliveryPartnerService>().Verify(m => m.GetDeliveryPartners(It.IsAny<int>()), Times.Once);
		}
		#endregion

		#region GetDeliveryPartnerServices
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationSummaryController))]
		public void GetDeliveryPartnerServices()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicationSummaryController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var grantProgram = EntityHelper.CreateGrantProgram();
			var deliveryPartnerService = EntityHelper.CreateDeliveryPartnerService(grantProgram);
			helper.GetMock<IDeliveryPartnerService>()
				.Setup(m => m.GetDeliveryPartnerServices(It.IsAny<int>()))
				.Returns(new List<CJG.Core.Entities.DeliveryPartnerService>() { deliveryPartnerService });
			var controller = helper.Create();

			// Act
			var result = controller.GetDeliveryPartnerServices(grantProgram.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<KeyValuePair<int, string>[]>();
			var model = result.Data as KeyValuePair<int, string>[];
			model.Count().Should().Be(1);
			model[0].Key.Should().Be(deliveryPartnerService.Id);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void GetDeliveryPartnerServices_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicationSummaryController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var grantProgram = EntityHelper.CreateGrantProgram();
			var deliveryPartnerService = EntityHelper.CreateDeliveryPartnerService(grantProgram);
			helper.GetMock<IDeliveryPartnerService>()
				.Setup(m => m.GetDeliveryPartnerServices(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetDeliveryPartnerServices(grantProgram.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<KeyValuePair<int, string>[]>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IDeliveryPartnerService>().Verify(m => m.GetDeliveryPartnerServices(It.IsAny<int>()), Times.Once);
		}
		#endregion

		#region Assign
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationSummaryController))]
		public void Assign()
		{
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicationSummaryController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();
			var applicationSummaryViewModel = new ApplicationSummaryViewModel()
			{
				Id = grantApplication.Id,
				RowVersion = "AgQGCAoMDhASFA==",
				AssessorId = 1
			};
			// Act
			var result = controller.Assign(applicationSummaryViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationSummaryViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>()
				.Verify(m => m.AssignAssessor(It.IsAny<GrantApplication>(), It.IsAny<int?>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void Assign_Exception()
		{
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicationSummaryController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();
			var applicationSummaryViewModel = new ApplicationSummaryViewModel()
			{
				Id = grantApplication.Id
			};
			// Act
			var result = controller.Assign(applicationSummaryViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationSummaryViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>()
				.Verify(m => m.AssignAssessor(It.IsAny<GrantApplication>(), It.IsAny<int?>()), Times.Never);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion

		#region UpdateSummary
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationSummaryController))]
		public void UpdateSummary()
		{
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicationSummaryController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.DateSubmitted = DateTime.UtcNow.AddMonths(-1);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();
			var applicationSummaryViewModel = new ApplicationSummaryViewModel()
			{
				Id = grantApplication.Id,
				RowVersion = "AgQGCAoMDhASFA==",
				AssessorId = 1,
				DeliveryStartDate = DateTime.UtcNow,
				DeliveryEndDate = DateTime.UtcNow.AddMonths(3)
			};
			// Act
			var result = controller.UpdateSummary(applicationSummaryViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationSummaryViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantAgreementService>()
				.Verify(m => m.AgreementUpdateRequired(It.IsAny<GrantApplication>()), Times.Once);
			helper.GetMock<IGrantApplicationService>()
				.Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), It.IsAny<Func<ApplicationWorkflowTrigger, bool>>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void UpdateSummary_Exception()
		{
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicationSummaryController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.DateSubmitted = DateTime.UtcNow.AddMonths(-1);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();
			var applicationSummaryViewModel = new ApplicationSummaryViewModel()
			{
				Id = grantApplication.Id
			};
			// Act
			var result = controller.Assign(applicationSummaryViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicationSummaryViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantAgreementService>()
				.Verify(m => m.AgreementUpdateRequired(It.IsAny<GrantApplication>()), Times.Never);
			helper.GetMock<IGrantApplicationService>()
				.Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), It.IsAny<Func<ApplicationWorkflowTrigger, bool>>()), Times.Never);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion
	}
}


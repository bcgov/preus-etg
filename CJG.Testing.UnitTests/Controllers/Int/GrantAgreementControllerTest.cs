using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Int.Controllers;
using CJG.Web.External.Areas.Int.Models.Agreements;
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
	public class GrantAgreementControllerTest
	{

		#region AgreementView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void AgreementView()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();

			// Act
			var result = controller.AgreementView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
		}
		#endregion

		#region GetAgreement
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void GetAgreement()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);

			// Act
			var result = controller.GetAgreement(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantAgreementViewModel>();
			var model = result.Data as GrantAgreementViewModel;
			model.Id.Should().Be(grantApplication.Id);
			model.ScheduleAConfirmed.Should().Be(grantApplication.GrantAgreement.ScheduleAConfirmed);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void GetAgreement_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();

			// Act
			var result = controller.GetAgreement(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantAgreementViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			controller.ControllerContext.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion

		#region CoverLetterView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void CoverLetterView()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();

			// Act
			var result = controller.CoverLetterView();

			// Assert
			result.Should().NotBeNull().And.BeOfType<PartialViewResult>();
		}
		#endregion

		#region GetCoverLetter
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void GetCoverLetter()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);

			// Act
			var result = controller.GetCoverLetter(grantApplication.Id, 1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantAgreementCoverLetterViewModel>();
			var model = result.Data as GrantAgreementCoverLetterViewModel;
			model.GrantApplicationId.Should().Be(grantApplication.Id);
			model.Body.Should().Be(grantApplication.GrantAgreement.CoverLetter.Body);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void GetCoverLetter_HasScheduleA()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			var versionedDocument = new VersionedDocument()
			{
				VersionNumber = 1,
				Body = "VersionedDocument"
			};
			grantApplication.GrantAgreement.ScheduleA.Versions.Add(versionedDocument);
			grantApplication.GrantAgreement.CoverLetter.Versions.Add(versionedDocument);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);

			// Act
			var result = controller.GetCoverLetter(grantApplication.Id, 1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantAgreementCoverLetterViewModel>();
			var model = result.Data as GrantAgreementCoverLetterViewModel;
			model.GrantApplicationId.Should().Be(grantApplication.Id);
			model.Body.Should().Be(versionedDocument.Body);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void GetCoverLetter_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();

			// Act
			var result = controller.GetCoverLetter(grantApplication.Id, 1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantAgreementCoverLetterViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			controller.ControllerContext.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion

		#region ScheduleAView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void ScheduleAView()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();

			// Act
			var result = controller.ScheduleAView();

			// Assert
			result.Should().NotBeNull().And.BeOfType<PartialViewResult>();
		}
		#endregion

		#region GetScheduleA
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void GetScheduleA()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);

			// Act
			var result = controller.GetScheduleA(grantApplication.Id, 1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantAgreementScheduleAViewModel>();
			var model = result.Data as GrantAgreementScheduleAViewModel;
			model.GrantApplicationId.Should().Be(grantApplication.Id);
			model.Body.Should().Be(grantApplication.GrantAgreement.ScheduleA.Body);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void GetScheduleA_HasScheduleA()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			var versionedDocument = new VersionedDocument()
			{
				VersionNumber = 1,
				Body = "VersionedDocument"
			};
			grantApplication.GrantAgreement.ScheduleA.Versions.Add(versionedDocument);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);

			// Act
			var result = controller.GetScheduleA(grantApplication.Id, 1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantAgreementScheduleAViewModel>();
			var model = result.Data as GrantAgreementScheduleAViewModel;
			model.GrantApplicationId.Should().Be(grantApplication.Id);
			model.Body.Should().Be(versionedDocument.Body);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void GetScheduleA_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();

			// Act
			var result = controller.GetScheduleA(grantApplication.Id, 1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantAgreementScheduleAViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			controller.ControllerContext.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion

		#region ScheduleBView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void ScheduleBView()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();

			// Act
			var result = controller.ScheduleBView();

			// Assert
			result.Should().NotBeNull().And.BeOfType<PartialViewResult>();
		}
		#endregion

		#region GetScheduleB
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void GetScheduleB()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);

			// Act
			var result = controller.GetScheduleB(grantApplication.Id, 1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantAgreementScheduleBViewModel>();
			var model = result.Data as GrantAgreementScheduleBViewModel;
			model.GrantApplicationId.Should().Be(grantApplication.Id);
			model.Body.Should().Be(grantApplication.GrantAgreement.ScheduleB.Body);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void GetScheduleB_HasScheduleA()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			var versionedDocument = new VersionedDocument()
			{
				VersionNumber = 1,
				Body = "VersionedDocument"
			};
			grantApplication.GrantAgreement.ScheduleA.Versions.Add(versionedDocument);
			grantApplication.GrantAgreement.ScheduleB.Versions.Add(versionedDocument);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);

			// Act
			var result = controller.GetScheduleB(grantApplication.Id, 1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantAgreementScheduleBViewModel>();
			var model = result.Data as GrantAgreementScheduleBViewModel;
			model.GrantApplicationId.Should().Be(grantApplication.Id);
			model.Body.Should().Be(versionedDocument.Body);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(GrantAgreementController))]
		public void GetScheduleB_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<GrantAgreementController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();

			// Act
			var result = controller.GetScheduleB(grantApplication.Id, 1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantAgreementScheduleBViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			controller.ControllerContext.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion
	}
}

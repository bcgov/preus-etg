using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Ext.Controllers;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Models.Shared;
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
	public class HomeControllerTest
	{
		#region Index
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(HomeController))]
		public void Index()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<HomeController>(user);

			helper.GetMock<IUserService>().Setup(m => m.GetUser(It.IsAny<Guid>())).Returns(user);
			var controller = helper.Create();

			// Act
			var result = controller.Index();

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(HomeController))]
		public void Index_MustSelectGrantProgramPreferences()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<HomeController>(user);

			helper.GetMock<IUserService>().Setup(m => m.GetUser(It.IsAny<Guid>())).Returns(user);
			helper.GetMock<IUserService>().Setup(m => m.MustSelectGrantProgramPreferences(It.IsAny<int>())).Returns(true);
			var controller = helper.Create();

			// Act
			var result = controller.Index();

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.MustSelectGrantProgramPreferences(It.IsAny<int>()), Times.Once);
		}
		#endregion

		#region GetGrantApplications
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(HomeController))]
		public void GetGrantApplications()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<HomeController>(user);
			var controller = helper.Create();

			var grantApplications = new PageList<GrantApplication>(new[] { EntityHelper.CreateGrantApplication() });
			helper.GetMock<IGrantApplicationService>().Setup(m => m.GetGrantApplications(user, 0, 10, x => x.DateUpdated ?? x.DateAdded)).Returns(grantApplications);

			// Act
			var result = controller.GetGrantApplications(0, 10);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull();
			var data = result.Data;
			data.GetReflectedProperty("RecordsFiltered").Should().Be(1);
			(data.GetReflectedProperty("Data") as GrantApplicationListDetailsViewModel[]).First().Id.Should().Be(grantApplications.Items.First().Id);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.GetGrantApplications(user, 0, 10, x => x.DateUpdated ?? x.DateAdded), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(HomeController))]
		public void GetGrantApplications_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<HomeController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.GetGrantApplications(user, 0, 10, x => x.DateUpdated ?? x.DateAdded)).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetGrantApplications(0, 10);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<BaseViewModel>();
			var data = result.Data as BaseViewModel;
			data.ValidationErrors.Count().Should().NotBe(0);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region GetGrantPrograms
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(HomeController))]
		public void GetGrantPrograms()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<HomeController>(user);
			var controller = helper.Create();

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var mockGrantOpeningService = helper.GetMock<IGrantOpeningService>();
			mockGrantOpeningService.Setup(m => m.GetGrantOpenings(It.IsAny<GrantProgram>())).Returns(new List<GrantOpening> { grantApplication.GrantOpening }.AsQueryable());
			var mockGrantProgramService = helper.GetMock<IGrantProgramService>();
			mockGrantProgramService.Setup(m => m.GetUserProgramPreferences()).Returns(new List<GrantProgram> { grantApplication.GrantOpening.GrantStream.GrantProgram });

			// Act
			var result = controller.GetGrantPrograms();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull();
			var data = result.Data as object[];
			data.First().GetReflectedProperty("GrantOpenings").Should().NotBeNull();
			data.First().GetReflectedProperty("Id").Should().Be(grantApplication.GrantOpening.GrantStream.GrantProgram.Id);
			helper.GetMock<IGrantProgramService>().Verify(m => m.GetUserProgramPreferences(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(HomeController))]
		public void GetGrantPrograms_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<HomeController>(user);

			helper.GetMock<IGrantProgramService>().Setup(m => m.GetUserProgramPreferences()).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetGrantPrograms();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<BaseViewModel>();
			var data = result.Data as BaseViewModel;
			data.ValidationErrors.Count().Should().NotBe(0);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion
	}
}

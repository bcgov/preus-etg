using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Ext.Controllers;
using CJG.Web.External.Areas.Ext.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Web.Mvc;

namespace CJG.Testing.UnitTests.Controllers.Ext
{
	[TestClass]
	public class AuthControllerTest
	{

		#region LogInGet
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(AuthController))]
		public void LogInGet()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<AuthController>(user);
			var controller = helper.Create();

			// Act
			var result = controller.LogIn();

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			helper.GetMock<IAuthenticationService>()
				.Verify(m => m.GetLogInOptions(AccountTypes.External), Times.Once);
		}
		#endregion

		#region LogInPost
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(AuthController))]
		public void LogInPost_ModelInvalid()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<AuthController>(user);

			var controller = helper.Create();
			var model = new LogInViewModel();
			controller.ModelState.AddModelError("SelectedUser", "SelectedUser is required");
			// Act
			var result = controller.LogIn(model);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			var routeResult = result as RedirectToRouteResult;
			routeResult.RouteValues.ContainsValue("Login").Should().BeTrue();
			routeResult.RouteValues.ContainsValue("Error").Should().BeTrue();
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(AuthController))]
		public void LogInPost()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<AuthController>(user);
			var controller = helper.Create();
			var model = new LogInViewModel()
			{
				SelectedUser = user.BCeIDGuid.ToString()
			};
			// Act
			var result = controller.LogIn(model);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			var routeResult = result as RedirectToRouteResult;
			routeResult.RouteValues.ContainsValue("Index").Should().BeTrue();
			routeResult.RouteValues.ContainsValue("Home").Should().BeTrue();
			helper.GetMock<IAuthenticationService>()
				.Verify(m => m.LogIn(It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(AuthController))]
		public void LogInPost_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<AuthController>(user);
			var controller = helper.Create();
			var model = new LogInViewModel()
			{
				SelectedUser = "wrong guid"
			};
			// Act
			var result = controller.LogIn(model);

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			var routeResult = result as RedirectToRouteResult;
			routeResult.RouteValues.ContainsValue("LogIn").Should().BeTrue();
			routeResult.RouteValues.ContainsValue("Error").Should().BeTrue();
		}
		#endregion

		#region LogOut
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(AuthController))]
		public void LogOut()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<AuthController>(user);
			helper.GetMock<IAuthenticationService>()
				.Setup(m => m.LogOut()).Returns("test");

			// Act
			var controller = helper.Create();
			var result = controller.LogOut();

			// Assert
			result.Should().NotBeNull().And.BeOfType<RedirectResult>();
			var routeResult = result as RedirectResult;
			helper.GetMock<IAuthenticationService>()
				.Verify(m => m.LogOut(), Times.Once);
			routeResult.Url.Should().Equals("test");
		}
		#endregion
	}
}

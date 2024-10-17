using CJG.Application.Business.Models;
using CJG.Application.Services;
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
	public class DebugControllerTest
	{

		#region SiteMinder
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(DebugController))]
		public void SiteMinder()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<DebugController>(user);

			helper.GetMock<IUserService>()
				.Setup(m => m.GetUser(It.IsAny<Guid>()))
				.Returns(user);
			var controller = helper.Create();

			// Act
			var result = controller.SiteMinder();

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			result.GetReflectedProperty("Model").Should().NotBeNull().And.BeOfType<DebugViewModel>();
			var model = result.GetReflectedProperty("Model") as DebugViewModel;
			model.User.BCeID.Should().Be(user.BCeID);
			model.User.BCeIDGuid.Should().Be(user.BCeIDGuid);
		}
		#endregion

		#region About
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(DebugController))]
		public void About()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<DebugController>(user);
			var controller = helper.Create();

			// Act
			var result = controller.About();

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			result.GetReflectedProperty("Model").Should().NotBeNull().And.BeOfType<AboutViewModel>();
			var model = result.GetReflectedProperty("Model") as AboutViewModel;
			model.Version.Should().Be(typeof(CJG.Web.External.MvcApplication).Assembly.GetName().Version);
		}
		#endregion
	}
}

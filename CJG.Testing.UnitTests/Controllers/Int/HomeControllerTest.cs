using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Int.Controllers;
using CJG.Web.External.Areas.Int.Models;
using CJG.Web.External.Helpers.Filters;
using FluentAssertions;
using Microsoft.Owin.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Reflection;
using System.Web.Mvc;
using static CJG.Testing.Core.ServiceHelper;

namespace CJG.Testing.UnitTests.Controllers.Int
{
	[TestClass]
	public class HomeControllerTest
	{
		#region Index
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(HomeController))]
		public void Index()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<HomeController>(user, Roles.Assessor);
			var controller = helper.Create();


			// Act
			var response = controller.Index();

			// Assert
			response.Should().NotBeNull().And.BeOfType<ViewResult>();
			var viewResult = response as ViewResult;
			viewResult.Model.Should().BeOfType<IndexModel>();
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(HomeController))]
		public void Index_AuthorizeAttribute()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<HomeController>(user, Roles.Assessor);

			var controller = helper.Create();

			// Act
			var type = controller.GetType();
			var methodInfo = type.GetMethod(nameof(HomeController.Index));
			var authorize = methodInfo.GetCustomAttribute<AuthorizeAttribute>();

			// Assert
			authorize.Should().NotBeNull();
		}
		#endregion

		#region Logout
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(HomeController))]
		public void Logout()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<HomeController>(user, Roles.Assessor);
			var controller = helper.Create();

			// Act
			var response = controller.Logout();

			// Assert
			response.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			var result = response as RedirectToRouteResult;
			result.RouteValues["action"].Should().Be("Index");
			result.RouteValues["controller"].Should().Be("Home");
			helper.GetMock<IAuthenticationManager>().Verify(m => m.SignOut(), Times.Once);
			helper.GetMock<ISiteMinderService>().Verify(m => m.LogOut(), Times.Once);
		}
		#endregion

		#region MyFiles
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(HomeController))]
		public void MyFiles()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<HomeController>(user, Roles.Assessor);
			var controller = helper.Create();

			// Act
			var response = controller.MyFiles();

			// Assert
			response.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>();
			var result = response as RedirectToRouteResult;
			result.RouteName.Should().Be(nameof(WorkQueueController.WorkQueueView));
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(HomeController))]
		public void MyFiles_AuthorizeAttribute()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<HomeController>(user, Roles.Assessor);

			var controller = helper.Create();

			// Act
			var type = controller.GetType();
			var methodInfo = type.GetMethod(nameof(HomeController.MyFiles));
			var authorize = methodInfo.GetCustomAttribute<AuthorizeActionAttribute>();

			// Assert
			authorize.Should().NotBeNull();
		}
		#endregion

		#region InternalHeaderPartial
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(HomeController))]
		public void InternalHeaderPartial()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<HomeController>(user, Roles.Assessor);
			var controller = helper.Create();

			// Act
			var response = controller.InternalHeaderPartial();

			// Assert
			response.Should().NotBeNull().And.BeOfType<PartialViewResult>();
			var result = response as PartialViewResult;
			result.ViewName.Should().Be("_InternalHeader");
		}
		#endregion
	}
}

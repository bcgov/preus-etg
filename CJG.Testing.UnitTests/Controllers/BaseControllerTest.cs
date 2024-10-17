using CJG.Application.Services;
using CJG.Testing.Core;
using CJG.Web.External.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Net;
using System.Web.Mvc;

namespace CJG.Testing.UnitTests.Controllers
{
	[TestClass]
	public class BaseControllerTest<T>
		where T : BaseController
	{
		[TestMethod, TestCategory("OnException")]
		public void ControllerTest_NoContentException_returns_BadRequest()
		{
			// Arrange				
			var mockExceptionContext = new Mock<ExceptionContext>(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
			mockExceptionContext.Setup(x => x.Exception).Returns(new NoContentException());

			var helper = typeof(T).FullName.Contains(".Ext.") ? new ControllerHelper<T>(EntityHelper.CreateExternalUser()) : new ControllerHelper<T>(EntityHelper.CreateInternalUser(), Core.ServiceHelper.Roles.Assessor);
			var controller = helper.Create();

			// Act
			((IExceptionFilter)controller).OnException(mockExceptionContext.Object);
			var actual = mockExceptionContext.Object.Result;

			// Assert
			Assert.IsTrue(actual is HttpStatusCodeResult);
			Assert.AreEqual((int)HttpStatusCode.BadRequest, ((HttpStatusCodeResult)actual).StatusCode);
		}

		[TestMethod, TestCategory("OnException")]
		public void ControllerTest_NotAuthorizedException_returns_Unauthorized()
		{
			// Arrange				
			var mockExceptionContext = new Mock<ExceptionContext>();
			mockExceptionContext.Setup(x => x.Exception).Returns(new NotAuthorizedException());

			var helper = typeof(T).FullName.Contains(".Ext.") ? new ControllerHelper<T>(EntityHelper.CreateExternalUser()) : new ControllerHelper<T>(EntityHelper.CreateInternalUser(), Core.ServiceHelper.Roles.Assessor);
			var controller = helper.Create();

			// Act
			((IExceptionFilter)controller).OnException(mockExceptionContext.Object);
			var actual = mockExceptionContext.Object.Result;

			// Assert
			Assert.IsTrue(actual is HttpUnauthorizedResult);
			Assert.AreEqual((int)HttpStatusCode.Unauthorized, ((HttpStatusCodeResult)actual).StatusCode);
		}

		[TestMethod, TestCategory("OnException")]
		public void ControllerTest_ConcurrencyException_returns_RedirectResult()
		{
			// Arrange				
			var mockExceptionContext = new Mock<ExceptionContext>();
			mockExceptionContext.Setup(x => x.Exception).Returns(new ConcurrencyException());

			var helper = typeof(T).FullName.Contains(".Ext.") ? new ControllerHelper<T>(EntityHelper.CreateExternalUser()) : new ControllerHelper<T>(EntityHelper.CreateInternalUser(), Core.ServiceHelper.Roles.Assessor);
			var controller = helper.Create();

			// Act
			((IExceptionFilter)controller).OnException(mockExceptionContext.Object);
			var actual = mockExceptionContext.Object.Result;

			// Assert
			Assert.IsTrue(actual is RedirectResult);
			Assert.AreEqual("/Error", ((RedirectResult)actual).Url);
		}

		[TestMethod, TestCategory("OnException")]
		public void ControllerTest_DbEntityValidationException_returns_RedirectResult()
		{
			// Arrange				
			var mockExceptionContext = new Mock<ExceptionContext>();
			mockExceptionContext.Setup(x => x.Exception).Returns(new DbEntityValidationException());

			var helper = typeof(T).FullName.Contains(".Ext.") ? new ControllerHelper<T>(EntityHelper.CreateExternalUser()) : new ControllerHelper<T>(EntityHelper.CreateInternalUser(), Core.ServiceHelper.Roles.Assessor);
			var controller = helper.Create();

			// Act
			((IExceptionFilter)controller).OnException(mockExceptionContext.Object);
			var actual = mockExceptionContext.Object.Result;

			// Assert
			Assert.IsTrue(actual is RedirectToRouteResult);
			Assert.AreEqual("Home", ((RedirectToRouteResult)actual).RouteValues["controller"]);
			Assert.AreEqual("Index", ((RedirectToRouteResult)actual).RouteValues["action"]);
		}

		[TestMethod, TestCategory("OnException")]
		public void ControllerTest_DbUpdateConcurrencyException_returns_RedirectResult()
		{
			// Arrange				
			var mockExceptionContext = new Mock<ExceptionContext>();
			mockExceptionContext.Setup(x => x.Exception).Returns(new DbUpdateConcurrencyException());

			var helper = typeof(T).FullName.Contains(".Ext.") ? new ControllerHelper<T>(EntityHelper.CreateExternalUser()) : new ControllerHelper<T>(EntityHelper.CreateInternalUser(), Core.ServiceHelper.Roles.Assessor);
			var controller = helper.Create();

			// Act
			((IExceptionFilter)controller).OnException(mockExceptionContext.Object);
			var actual = mockExceptionContext.Object.Result;

			// Assert
			Assert.IsTrue(actual is RedirectToRouteResult);
			Assert.AreEqual("Home", ((RedirectToRouteResult)actual).RouteValues["controller"]);
			Assert.AreEqual("Index", ((RedirectToRouteResult)actual).RouteValues["action"]);
		}

		[TestMethod, TestCategory("OnException")]
		public void ControllerTest_NoNullAllowedException_returns_RedirectResult()
		{
			// Arrange				
			var mockExceptionContext = new Mock<ExceptionContext>();
			mockExceptionContext.Setup(x => x.Exception).Returns(new NoNullAllowedException());

			var helper = typeof(T).FullName.Contains(".Ext.") ? new ControllerHelper<T>(EntityHelper.CreateExternalUser()) : new ControllerHelper<T>(EntityHelper.CreateInternalUser(), Core.ServiceHelper.Roles.Assessor);
			var controller = helper.Create();

			// Act
			((IExceptionFilter)controller).OnException(mockExceptionContext.Object);
			var actual = mockExceptionContext.Object.Result;

			// Assert
			Assert.IsTrue(actual is RedirectToRouteResult);
			Assert.AreEqual("Home", ((RedirectToRouteResult)actual).RouteValues["controller"]);
			Assert.AreEqual("Index", ((RedirectToRouteResult)actual).RouteValues["action"]);
		}

		// TODO: create unit tests for ActionFilter
	}
}

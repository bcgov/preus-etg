using CJG.Testing.Core;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using System.Web.Mvc;

namespace CJG.Testing.UnitTests.Controllers.Ext
{
	[TestClass]
	public class ExtControllerTest<T> : BaseControllerTest<T>
		where T : BaseController
	{
		// TODO: Reconsider if this test needs to be moved somewhere else. This test is getting replicated in each child test class, but does not appear to be testing different contexts.
		[TestMethod, TestCategory(nameof(ExternalFilter))]
		public void ExternalFilterTest_Redirect_InternalUsers()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<T>(user, ServiceHelper.Roles.Assessor);
			var controller = helper.Create();

			var mockActionExecutingcontext = helper.GetMock<ActionExecutingContext>();
			mockActionExecutingcontext.Setup(x => x.HttpContext).Returns(helper.GetMock<HttpContextBase>().Object);
			mockActionExecutingcontext.Setup(x => x.Controller).Returns(controller);

			var filter = new ExternalFilter();

			// Act
			filter.OnActionExecuting(mockActionExecutingcontext.Object);
			var actual = mockActionExecutingcontext.Object;

			// Assert
			Assert.IsNotNull(actual);
			Assert.AreEqual("You must log out of the internal application before logging into the external.", actual.Controller.TempData["Message"]);
			Assert.IsTrue(actual.Result is RedirectToRouteResult);
			Assert.AreEqual("Int", ((RedirectToRouteResult)actual.Result).RouteValues["area"]);
		}
	}
}

using System.Web;
using System.Web.Mvc;

namespace CJG.Web.External.Helpers.Filters
{
    public class RefreshDetectFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cookie = filterContext.HttpContext.Request.Cookies["RefreshFilter"];
            filterContext.RouteData.Values[RouteExtensions.IsRefreshedName] = cookie != null && cookie.Value == GetHash(filterContext);
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.SetCookie(new HttpCookie("RefreshFilter", GetHash(filterContext)));
            base.OnActionExecuted(filterContext);
        }

        private static string GetHash(ControllerContext filterContext)
        {
            return filterContext.HttpContext?.Request.Form.ToString().GetHashCode().ToString();
        }
    }
}
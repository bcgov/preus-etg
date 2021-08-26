using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace CJG.Web.External.Helpers
{
    public class TransferToRouteResult : ActionResult
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string RouteName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }

        public TransferToRouteResult(RouteValueDictionary routeValues)
            : this(null, routeValues)
        {
        }

        public TransferToRouteResult(string routeName, RouteValueDictionary routeValues)
        {
            this.RouteName = routeName ?? string.Empty;
            this.RouteValues = routeValues ?? new RouteValueDictionary();
        }

        public TransferToRouteResult(string actionName, string controllerName = null, RouteValueDictionary routeValues = null)
        {
            this.ActionName = actionName;
            this.ControllerName = controllerName;
            this.RouteValues = RouteValues ?? new RouteValueDictionary();
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var urlHelper = new UrlHelper(context.RequestContext);
            string url;

            if (!String.IsNullOrEmpty(this.ControllerName))
            {
                url = urlHelper.Action(this.ActionName, this.ControllerName, this.RouteValues);
            }
            else if (!String.IsNullOrEmpty(this.ActionName))
            {
                url = urlHelper.Action(this.ActionName, this.RouteValues);
            }
            else
            {
                url = urlHelper.RouteUrl(this.RouteName, this.RouteValues);
            }

            var actualResult = new TransferResult(url);
            actualResult.ExecuteResult(context);
        }
    }
}
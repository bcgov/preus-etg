using CJG.Core.Entities;
using System;
using System.Web.Mvc;

namespace CJG.Web.External.Helpers.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ResponseTimeFilterAttribute : ActionFilterAttribute
    {
        private readonly DateTime _requestedAt = AppDateTime.UtcNow;

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            TimeSpan time = AppDateTime.UtcNow - _requestedAt;
            filterContext.HttpContext.Response.Headers.Add("X-Response-Time", $"{time.Milliseconds} ms");
            base.OnActionExecuted(filterContext);
        }
    }
}
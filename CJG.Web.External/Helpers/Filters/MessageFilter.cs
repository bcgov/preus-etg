using System;
using System.Web.Mvc;

namespace CJG.Web.External.Helpers.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class MessageFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //  get the view data
            var viewData = filterContext.Controller.ViewData;

            // build an array of the different view bag properties to copy from temp data to view data
            var properties = new string[] { "Message", "MessageType", "ChangeType" };

            foreach (var property in properties)
            {
                if (filterContext.Controller.TempData[property] != null)
                {
                    viewData.Add(property, filterContext.Controller.TempData[property]);
                    filterContext.Controller.TempData[property] = null;
                }
            }
        }
    }
}
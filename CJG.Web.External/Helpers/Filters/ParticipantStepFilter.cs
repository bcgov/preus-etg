using CJG.Web.External.Areas.Part.Models;
using NLog;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace CJG.Web.External.Helpers.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ParticipantStepFilter : ActionFilterAttribute
    {
        private readonly ILogger _logger;

        public ParticipantStepFilter ()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.Controller.TempData["ParticipantInfoViewModel"] != null && filterContext.HttpContext.Request.UrlReferrer == null)
            {
                try
                {
                    var serverSideViewModel = (ParticipantInfoViewModel)filterContext.Controller.TempData["ParticipantInfoViewModel"];

                    // get the current step from the view model
                    var stepPath = $"Step{serverSideViewModel.ParticipantInfoStep0ViewModel.Step}";

                    // get the step from the path of the current request
                    var stepPathRequested = filterContext.HttpContext.Request.Url.Segments[filterContext.HttpContext.Request.Url.Segments.Length - 1];

                    // if the two don't match (i.e. someone has manually changed the step in the browser address bar), then redirect to the current one
                    if (stepPath != stepPathRequested)
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = stepPath }));
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                }
            }
        }
    }
}
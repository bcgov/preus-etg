using NLog;
using System;
using System.Net;
using System.Web.Helpers;
using System.Web.Mvc;

namespace CJG.Web.External.Helpers.Filters
{
    public class ValidateRequestHeader : ActionFilterAttribute
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cookieToken = string.Empty;
            var formToken = string.Empty;

            var headers = filterContext.HttpContext.Request.Headers;
            var form = filterContext.HttpContext.Request.Form;
            var requestVerificationToken = string.Empty;

            // if the request verification token is added to the header
            if (headers["RequestVerificationToken"] != null)
            {
                requestVerificationToken = headers["RequestVerificationToken"];
            }

            if (!string.IsNullOrEmpty(requestVerificationToken))
            {
                var tokens = requestVerificationToken.Split(':');
                if (tokens.Length == 2)
                {
                    cookieToken = tokens[0].Trim();
                    formToken = tokens[1].Trim();
                }
            }

            try
            {
                AntiForgery.Validate(cookieToken, formToken);
            }
            catch(Exception e)
            {
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                logger.Error(e);
            }
        }
    }
}
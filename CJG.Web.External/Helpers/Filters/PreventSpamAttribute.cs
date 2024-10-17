using System;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Caching;
using System.Web.Mvc;

namespace CJG.Web.External.Helpers.Filters
{
    /// <summary>
    /// <typeparamref name="PreventSpamAttribute"/> class, provides a way to decorate Controller endpoints to stop multiple requests that are too quickly requested from the same user.
    /// </summary>
    public class PreventSpamAttribute : ActionFilterAttribute
    {
        #region Variables
        #endregion

        #region Properties
        /// <summary>
        /// get/set - The number of seconds that must occur before another identical request can be made.
        /// </summary>
        public int DelayRequest {
            get
            {
                int delayRequest = 5;

                if (ConfigurationManager.AppSettings["SpamFilterDelayRequest"] != null)
                {
                    int.TryParse(ConfigurationManager.AppSettings["SpamFilterDelayRequest"], out delayRequest);
                }

                return delayRequest;
            }
        }

        /// <summary>
        /// get/set - The error message to return if request spamming is detected.
        /// </summary>
        public string ErrorMessage { get; set; } = "Excessive request attempts detected.";
        #endregion

        #region Methods
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var origination_info = request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress;
            origination_info += request.UserAgent;
            var target_info = request.RawUrl + request.QueryString;

            var hash = String.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(origination_info + target_info)).Select(s => s.ToString("x2")));

            var cache = filterContext.HttpContext.Cache;
            if (cache[hash] != null)
            {
                filterContext.Controller.ViewData.ModelState.AddModelError("ExessiveRequests", ErrorMessage);
            }
            else
            {
                cache.Add(hash, String.Empty, null, DateTime.Now.AddSeconds(DelayRequest), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }
            base.OnActionExecuting(filterContext);
        }
        #endregion
    }
}
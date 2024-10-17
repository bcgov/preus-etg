using CJG.Application.Services;
using CJG.Core.Interfaces;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace CJG.Web.External.Helpers
{
    /// <summary>
    /// <typeparamref name="HttpStatusCodeResultOnAttribute"/> attribute class, provides a way to generically handle returning the appropriate HttpStatusCode for specific exception types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class HttpStatusCodeResultOnAttribute : Attribute
    {
        #region Variables
        #endregion

        #region Properties
        public Type[] ExceptionType { get; }
        public HttpStatusCode StatusCode { get; } = HttpStatusCode.InternalServerError;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="RedirectOnAttribute"/> class.
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="exceptionType"></param>
        public HttpStatusCodeResultOnAttribute(HttpStatusCode statusCode, Type exceptionType)
        {
            this.StatusCode = statusCode;
            this.ExceptionType = new[] { exceptionType };
        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="RedirectOnAttribute"/> class.
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="exceptionType"></param>
        public HttpStatusCodeResultOnAttribute(HttpStatusCode statusCode, params Type[] exceptionType)
        {
            this.StatusCode = statusCode;
            this.ExceptionType = exceptionType;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates a <typeparamref name="RedirectToRouteResult"/> object and initializes it with appropriate route data.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ActionResult HttpStatusCodeResult(ExceptionContext context)
        {
            string message;
            if (context.Exception.GetType() == typeof(DbEntityValidationException))
                message = ((DbEntityValidationException)context.Exception).GetValidationMessages();
            else
#if DEBUG
                message = context.Exception.GetAllMessages();
#else
                message = "An error occured.  If you continue to see this behaviour please contact support.";
#endif
            var type = context.Exception.GetType();

            if (this.ExceptionType.Any(t => t == type))
                return new HttpStatusCodeResult(this.StatusCode, message).HttpStatusCodeResultWithAlert(context.HttpContext.Response, AlertType.Warning); ;

            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, message).HttpStatusCodeResultWithAlert(context.HttpContext.Response, AlertType.Error); ;
        }
#endregion
    }
}
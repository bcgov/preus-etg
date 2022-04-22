using System.IO;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using CJG.Application.Services;
using System;
using CJG.Core.Interfaces;
using System.Web;
using System.Globalization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Data.Entity.Infrastructure;
using System.Net;
using CJG.Infrastructure.BCeID.WebService;
using CJG.Core.Entities;

namespace CJG.Web.External.Helpers
{
    /// <summary>
    /// <typeparamref name="ControllerExtensions"/> static class, provides extension methods for controllers.
    /// </summary>
    public static class ControllerExtensions
    {
        public static string RenderRazorViewToString(this ControllerContext context, string viewName, object model)
        {
            var viewData = context.Controller.ViewData;
            var tempData = context.Controller.TempData;

            viewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                var viewContext = new ViewContext(context, viewResult.View,
                                                viewData, tempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(context, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public static string RenderRazorViewToString(this Controller controller, string viewName, object model)
        {
            var viewData = controller.ViewData;
            var tempData = controller.TempData;

            viewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View,
                                                viewData, tempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// Get the action name for the current request.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetAction(this ControllerContext context)
        {
            return (string)context.RouteData.Values["action"];
        }

        /// <summary>
        /// Get the area name for the current request.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetArea(this ControllerContext context)
        {
            return (string)context.RouteData.DataTokens["area"];
        }

        /// <summary>
        /// Get the area name for the current request.
        /// </summary>
        /// <returns></returns>
        public static string GetArea()
        {
            return (string)HttpContext.Current.Request.RequestContext.RouteData.DataTokens["area"];
        }

        /// <summary>
        /// Determine if the current request is an external site request.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsExternal(this ControllerContext context)
        {
            return String.Compare(context.GetArea(), "ext", true, CultureInfo.InvariantCulture) == 0;
        }

        /// <summary>
        /// Determine if the current request is an internal site request.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsInternal(this ControllerContext context)
        {
            return String.Compare(context.GetArea(), "int", true, CultureInfo.InvariantCulture) == 0;
        }

        /// <summary>
        /// An easy way to get a decent error message based on the exception type and whether in DEBUG mode.
        /// </summary>
        /// <param name="error"></param>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="delimiter">The delimiter that separates multiple exception messages.  Default is a single space " ".</param>
        private static void GetAlert(Exception error, out string message, out AlertType messageType, string delimiter = " ")
        {
            var validation_error = error as DbEntityValidationException;
            var not_authorized_error = error as NotAuthorizedException;
            messageType = validation_error == null && not_authorized_error == null ? AlertType.Error : AlertType.Warning;

            var type = error.GetType();
            if (type == typeof(DbEntityValidationException))
            {
                messageType = AlertType.Warning;
                message = validation_error.GetValidationMessages(delimiter);
            }
            else if (type == typeof(DbUpdateConcurrencyException))
            {
                messageType = AlertType.Error;
                message = "The record has been updated by someone else, please reload the page.";
            }
            else if (type == typeof(NotAuthorizedException))
            {
                messageType = AlertType.Warning;
                message = "You are not authorized to access this page and/or information.";
            }
            else if (type == typeof(BCeIDException))
            {
                messageType = AlertType.Error;
                message = $"An error occured while attempting to communicate with BCeID, please contact support. {error.GetAllMessages()}";
            }
            else
            {
                messageType = AlertType.Error;
#if DEBUG
                message = error.GetAllMessages();
#else
                message = "An error has occured, please contact support.";
#endif
            }
        }

        /// <summary>
        /// Adds one alert message to the session so that they can be displayed in the View.
        /// Views should display a Message based on the MessageType at the top of the View.
        /// If called multiple times it will overwrite previous alerts.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        /// <param name="messageType">[Error|Warning|Success]</param>
        /// <param name="isRedirect">A redirect adds messages to TempData, otherwise it adds to the ViewBag.</param>
        public static void SetAlert(this ControllerContext context, string message, AlertType messageType, bool isRedirect = false)
        {
            if (isRedirect)
            {
                var tempData = context.Controller.TempData;
                if (tempData != null)
                {
                    tempData["Message"] = message;
                    tempData["MessageType"] = messageType.ToString().ToLower();
                }
            }
            else
            {
                var viewBag = context.Controller.ViewBag;
                if (viewBag != null)
                {
                    viewBag.Message = message;
                    viewBag.MessageType = messageType.ToString().ToLower();
                }
            }
        }

        /// <summary>
        /// Adds a generic message to indicate an exception, or if in DEBUG it will return the whole exception message.
        /// Adds one alert message to the session so that they can be displayed in the View.
        /// Views should display a Message based on the MessageType at the top of the View.
        /// If called multiple times it will overwrite previous alerts.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="error"></param>
        /// <param name="delimiter">The delimiter that separates multiple exception messages.</param>
        /// <param name="isRedirect">A redirect adds messages to TempData, otherwise it adds to the ViewBag.</param>
        public static void SetAlert(this ControllerContext context, Exception error, bool isRedirect = false, string delimiter = " ")
        {
            string message;
            AlertType message_type;
            GetAlert(error, out message, out message_type, delimiter);
            context.SetAlert(message, message_type, isRedirect);
        }

        /// <summary>
        /// Adds a generic message to indicate an exception, or if in DEBUG it will return the whole exception message.
        /// Adds one alert message to the session so that they can be displayed in the View.
        /// Views should display a Message based on the MessageType at the top of the View.
        /// If called multiple times it will overwrite previous alerts.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="error"></param>
        /// <param name="messageType"></param>
        /// <param name="delimiter">The delimiter that separates multiple exception messages.</param>
        /// <param name="isRedirect">A redirect adds messages to TempData, otherwise it adds to the ViewBag.</param>
        public static void SetAlert(this ControllerContext context, Exception error, AlertType messageType, bool isRedirect = false, string delimiter = " ")
        {
            string message;
            AlertType message_type;
            GetAlert(error, out message, out message_type, delimiter);
            context.SetAlert(message, messageType, isRedirect);
        }

        /// <summary>
        /// Adds one alert message to the session so that they can be displayed in the View.
        /// Views should display a Message based on the MessageType at the top of the View.
        /// If called multiple times it will overwrite previous alerts.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="isRedirect">A redirect adds messages to TempData, otherwise it adds to the ViewBag.</param>
        public static void SetAlert(this Controller controller, string message, AlertType messageType, bool isRedirect = false)
        {
            controller.ControllerContext.SetAlert(message, messageType, isRedirect);
        }

        /// <summary>
        /// Adds on alert message to the session so that they can be displayed in the View.
        /// Views should display a Message based on the MessageType at the top of the View.
        /// If called multiple times it will overwrite previous alerts.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="validationResults"></param>
        /// <param name="isRedirect"></param>
        /// <param name="delimiter"></param>
        public static void SetAlert(this Controller controller, IEnumerable<ValidationResult> validationResults, bool isRedirect = false, string delimiter = "</br>")
        {
            var message = validationResults.Select(vr => vr.ErrorMessage).Aggregate((a, b) => $"{a}{delimiter}{b}");
            controller.ControllerContext.SetAlert(message, AlertType.Warning, isRedirect);
        }

        /// <summary>
        /// Adds a generic message to indicate an exception, or if in DEBUG it will return the whole exception message.
        /// Adds one alert message to the session so that they can be displayed in the View.
        /// Views should display a Message based on the MessageType at the top of the View.
        /// If called multiple times it will overwrite previous alerts.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="error"></param>
        /// <param name="delimiter">The delimiter that separates multiple exception messages.</param>
        /// <param name="isRedirect">A redirect adds messages to TempData, otherwise it adds to the ViewBag.</param>
        public static void SetAlert(this Controller controller, Exception error, bool isRedirect = false, string delimiter = " ")
        {
            controller.ControllerContext.SetAlert(error, isRedirect, delimiter);
        }

        /// <summary>
        /// Adds a generic message to indicate an exception, or if in DEBUG it will return the whole exception message.
        /// Adds one alert message to the session so that they can be displayed in the View.
        /// Views should display a Message based on the MessageType at the top of the View.
        /// If called multiple times it will overwrite previous alerts.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="error"></param>
        /// <param name="messageType"></param>
        /// <param name="delimiter">The delimiter that separates multiple exception messages.</param>
        /// <param name="isRedirect">A redirect adds messages to TempData, otherwise it adds to the ViewBag.</param>
        public static void SetAlert(this Controller controller, Exception error, AlertType messageType, bool isRedirect = false, string delimiter = " ")
        {
            controller.ControllerContext.SetAlert(error, messageType, isRedirect, delimiter);
        }

        /// <summary>
        /// Adds alert messages to the session so that they can be displayed in the View.
        /// If called multiple times it will append the alert messages of the same type.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="message"></param>
        /// <param name="messageType">[Error|Warning|Success]</param>
        public static void SetAlerts(this Controller controller, string message, AlertType messageType, bool isRedirect = false)
        {
	        if (isRedirect)
	        {
		        var tempData = controller.TempData;
		        if (tempData != null)
		        {
			        var baggedAlerts = tempData["BaggedAlerts"] as List<Alert>;
			        if (baggedAlerts == null)
			        {
				        baggedAlerts = new List<Alert>();
				        tempData["BaggedAlerts"] = baggedAlerts;
			        }

			        baggedAlerts.Add(new Alert
			        {
				        Message = message,
				        MessageType = messageType.ToString().ToLower(),
				        AlertType = messageType
			        });

			        tempData["BaggedAlerts"] = baggedAlerts;
		        }
	        }
	        else
	        {
		        var viewBag = controller.ViewBag;
		        var baggedAlerts = viewBag.BaggedAlerts as List<Alert>;
		        if (baggedAlerts == null)
		        {
			        baggedAlerts = new List<Alert>();
			        viewBag.BaggedAlerts = baggedAlerts;
		        }

		        baggedAlerts.Add(new Alert
		        {
			        Message = message,
			        MessageType = messageType.ToString().ToLower(),
			        AlertType = messageType
		        });
	        }
        }

        public struct Alert
        {
	        public string Message { get; set; }
			public string MessageType { get; set; }
			public AlertType AlertType { get; set; }
		}

        /// <summary>
        /// Generic way to handle when the user is not authorized to view or perform an action.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ActionResult UnauthorizedResult(this Controller controller, string message = "You are not authorized to view this resource, or perform this action.")
        {
            return new HttpUnauthorizedResult(message).HttpStatusCodeResultWithAlert(controller.Response, AlertType.Warning);
        }

        /// <summary>
        /// Generic way to handle when a user requests content that does not exist.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ActionResult NoContentResult(this Controller controller, string message = "The resource requested does not exist, or you do not have permission to view it.")
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, message).HttpStatusCodeResultWithAlert(controller.Response, AlertType.Warning);
        }

        /// <summary>
        /// Generic way to redirect when an exception occurs.
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="error"></param>
        /// <param name="action"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static ActionResult ExceptionResult(this Controller controller, Exception error)
        {
#if DEBUG
            string message = error.GetAllMessages();
#else
            string message = "An error occured while attempting to perform the action.";
#endif
            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, message).HttpStatusCodeResultWithAlert(controller.Response, AlertType.Error);
        }

        public static void SetConcurrencyAlert(this Controller controller, ApplicationWorkflowTrigger action, ApplicationStateExternal state, bool isRedirect = false)
        {
            controller.SetAlert(
                ConcurrencyException.GetHtmlMessage(action, state),
                AlertType.Error, isRedirect);
        }

        public static void SetConcurrencyAlert(this Controller controller, ApplicationWorkflowTrigger action, ApplicationStateInternal state, bool isRedirect = false)
        {
            controller.SetAlert(
                ConcurrencyException.GetHtmlMessage(action, state),
                AlertType.Error, isRedirect);
        }

        public static void SetConcurrencyAlert(this Controller controller, string action, ApplicationStateInternal state, bool isRedirect = false)
        {
            try
            {
                // try to get the description from the action
                var trigger = (ApplicationWorkflowTrigger)Enum.Parse(typeof(ApplicationWorkflowTrigger), action);
                action = trigger.GetDescription();
            }
            catch { }

            controller.SetAlert(
                ConcurrencyException.GetHtmlMessage(action, state),
                AlertType.Error, isRedirect);
        }

        public static void ValidateConcurrency(this Controller controller, EntityBase entity, byte[] originalRowVersion, ApplicationWorkflowTrigger action, ApplicationStateExternal state, bool isRedirect = false)
        {
            if (!entity.IsMatchRowVersion(originalRowVersion))
            {
                controller.SetConcurrencyAlert(action, state, isRedirect);
                throw new ConcurrencyException(action, state);
            }
        }

        public static void ValidateConcurrency(this Controller controller, EntityBase entity, byte[] originalRowVersion, ApplicationWorkflowTrigger action, ApplicationStateInternal state, bool isRedirect = false)
        {
            if (!entity.IsMatchRowVersion(originalRowVersion))
            {
                controller.SetConcurrencyAlert(action, state, isRedirect);
                throw new ConcurrencyException(action, state);
            }
        }

        public static void ValidateConcurrency(this Controller controller, GrantApplication grantApplication, byte[] originalRowVersion, ApplicationWorkflowTrigger action, bool isRedirect = false)
        {
            if (!grantApplication.IsMatchRowVersion(originalRowVersion))
            {
                controller.SetConcurrencyAlert(action, grantApplication.ApplicationStateInternal, isRedirect);
                throw new ConcurrencyException(action, grantApplication.ApplicationStateInternal);
            }
        }

        public static void ValidateConcurrency(this Controller controller, GrantApplication grantApplication, byte[] originalRowVersion, string action, bool isRedirect = false)
        {
            if (!grantApplication.IsMatchRowVersion(originalRowVersion))
            {
                controller.SetConcurrencyAlert(action, grantApplication.ApplicationStateInternal, isRedirect);
                throw new ConcurrencyException(action, grantApplication.ApplicationStateInternal);
            }
        }
    }
}
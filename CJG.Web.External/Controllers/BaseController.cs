using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces;
using CJG.Web.External.Helpers;
using CJG.Web.External.Models.Shared;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace CJG.Web.External.Controllers
{
	/// <summary>
	/// <typeparamref name="BaseController"/> abstract class, provides shared implementation for all controllers.
	/// </summary>
	public abstract class BaseController : Controller
	{
		#region Properties
		protected readonly ILogger _logger;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="BaseController"/> class.
		/// </summary>
		/// <param name="logger"></param>
		public BaseController(ILogger logger)
		{
			_logger = logger;
		}
		#endregion

		#region Methods


		protected override void OnActionExecuting(ActionExecutingContext filterContext){

		}

		protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
		{
			return new JsonResult()
			{
				Data = data,
				ContentType = contentType,
				ContentEncoding = contentEncoding,
				JsonRequestBehavior = behavior,
				MaxJsonLength = Int32.MaxValue
			};
		}

		/// <summary>
		/// Handle unhandled exceptions.
		/// </summary>
		/// <param name="filterContext"></param>
		protected override void OnException(ExceptionContext filterContext)
		{
			if (filterContext.ExceptionHandled)
				return;

			_logger.Info("Unhandled controller exception occured.");

			var error = filterContext.Exception;
			var type = filterContext.Exception.GetType();

			// Get the RedirectOnAttribute to control what happens on specific exception types.
			var method = GetCallingMethod(filterContext);
			var redirectOn = method?.GetCustomAttributes<RedirectOnAttribute>();
			var redirect = redirectOn?.FirstOrDefault(a => a.ExceptionType == type);

			var httpStatusCodes = method?.GetCustomAttributes<HttpStatusCodeResultOnAttribute>();
			var httpStatusCode = httpStatusCodes?.FirstOrDefault(a => a.ExceptionType.Any(t => t == type));

			if (redirect != null)
			{
				this.SetAlert(error, redirect.AlertType, true);
				filterContext.Result = redirect.RedirectResult(filterContext);
			}
			else if (httpStatusCode != null)
			{
				this.SetAlert(error, true);
				filterContext.Result = httpStatusCode.HttpStatusCodeResult(filterContext);
			}
			else if (type == typeof(NotAuthorizedException))
			{
				this.SetAlert(error, true);
				_logger.Info(error);
				filterContext.Result = this.UnauthorizedResult(error.Message);
			}
			else if (type == typeof(NoContentException))
			{
				this.SetAlert(error.Message, AlertType.Warning, true);
				_logger.Info(error);
				filterContext.Result = this.NoContentResult(error.Message);
			}
			else if (type == typeof(ConcurrencyException))
			{
				_logger.Debug(error);
				this.SetAlert(error, AlertType.Warning, true);
				filterContext.Result = new RedirectResult("/Error");
			}
			else if (type == typeof(DbEntityValidationException)
				|| type == typeof(DbUpdateConcurrencyException)
				|| type == typeof(NoNullAllowedException))
			{
				this.SetAlert(error, true);
				filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
				{
					{ "action", "Index" },
					{ "controller", "Home" }
				});
			}
			else
			{
				_logger.Error(error);
				this.SetAlert(error, true);
				filterContext.Result = new RedirectResult("/Error");
				// filterContext.Result = this.ExceptionResult(error);
			}
			filterContext.ExceptionHandled = true;
			base.OnException(filterContext);
		}

		/// <summary>
		/// Get the Controller method that originated the exception.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		private MethodBase GetCallingMethod(ExceptionContext context)
		{
			var frames = new StackTrace(context.Exception).GetFrames();
			return frames?.FirstOrDefault(f => typeof(IController).IsAssignableFrom(f.GetMethod()?.DeclaringType))?.GetMethod();
		}

		/// <summary>
		/// Extracts all the model state errors and places them in a collection of key value pairs.
		/// </summary>
		/// <returns></returns>
		protected List<KeyValuePair<string, string>> GetClientErrors()
		{
			var result = new List<KeyValuePair<string, string>>();
			foreach (var key in ModelState.Keys)
			{
				if (ModelState[key].Errors.Count() > 0)
				{
					result.Add(new KeyValuePair<string, string>(key, string.Join(",", ModelState[key].Errors.Select(t => t.ErrorMessage))));
				}
			}

			return result;
		}

		/// <summary>
		/// Adds a generic error summary message to the model.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="error"></param>
		/// <param name="code"></param>
		/// <param name="summary"></param>
		protected void AddGenericError(BaseViewModel model, string error = null, int code = 400)
		{
			AddAngularError(model, "Summary", error, code);
		}

		/// <summary>
		/// Add an error message to the model.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="key"></param>
		/// <param name="error"></param>
		/// <param name="code"></param>
		protected void AddAngularError(BaseViewModel model, string key, string error, int code = 400)
		{
			Response.StatusCode = code;
			Response.TrySkipIisCustomErrors = true;
			if (!String.IsNullOrWhiteSpace(error)) model.AddError(key, error);
		}

		/// <summary>
		/// Provides a generic way to handle a redirect action in an AJAX request.
		/// </summary>
		/// <param name="code"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		protected HttpStatusCodeResult RedirectToActionAjax(int code, string action)
		{
			Response.StatusCode = code;
			Response.TrySkipIisCustomErrors = true;
			return new HttpStatusCodeResult(code, action);
		}

		/// <summary>
		/// If model state is invalid then add the errors to the model.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="summary"></param>
		protected void HandleModelStateValidation(BaseViewModel model, string summary = null)
		{
			if (model.ValidationErrors == null) model.ValidationErrors = GetClientErrors();
			else model.ValidationErrors.AddRange(GetClientErrors());
			AddGenericError(model, summary);
		}

		/// <summary>
		/// Provides a generic way to handle the majority of exceptions and ensure the JSON response is consistent and valid.
		/// </summary>
		/// <param name="ex"></param>
		/// <param name="viewModel"></param>
		/// <param name="postback"></param>
		protected void HandleAngularException(Exception ex, BaseViewModel viewModel = null, Action postback = null)
		{
			var errorMsg = ex.Message;
			if (ex is DbUpdateConcurrencyException)
			{
				Response.StatusCode = 400;
				errorMsg = "Someone has updated the record. You must reload your page.";
			}
			else if (ex is NoContentException
				|| ex is InvalidOperationException
				|| ex is DbUpdateException
				|| ex is DbEntityValidationException
				|| ex is TemplateException)
			{
				Response.StatusCode = 400;
				if (ex is DbEntityValidationException && viewModel != null)
				{
					viewModel.AddErrors(Utilities.GetErrors((DbEntityValidationException)ex, viewModel));
					if (viewModel.ValidationErrors.Count > 0)
						errorMsg = string.Join("<br/>", viewModel.ValidationErrors.Select(ve => ve.Value).Where(e => !String.IsNullOrEmpty(e)).Distinct().ToArray());
				}
				else if (ex is InvalidOperationException)
				{
					errorMsg = ex.GetAllMessages();
				}
				else if (ex is DbUpdateException)
				{
					_logger.Error(ex, "Failed to update the datasource.");
					errorMsg = "An unexpected error occurred, please try again later or contact support.";
				}
				else if (ex is TemplateException)
				{
					var error = ex as TemplateException;
					_logger.Error(ex, "Failed to parse razor template for grant program templates.");
					errorMsg = $"Failed to parse the {error.DisplayName} document template.";
					viewModel?.AddError(error.PropertyName, error.GetErrorMessages());
				}
			}
			else if (ex is NotAuthorizedException)
			{
				_logger.Warn(ex);
				Response.StatusCode = 403;
			}
			else
			{
				_logger.Error(ex);
				Response.StatusCode = 500;
				errorMsg = "An unexpected error occurred, please try again later or contact support.";
			}

			Response.TrySkipIisCustomErrors = true;
			viewModel?.AddError("Summary", errorMsg);
			postback?.Invoke();
		}

		/// <summary>
		/// Provides a generic way to handle the majority of exceptions and ensure the response includes an error message.
		/// </summary>
		/// <param name="error"></param>
		/// <param name="delimiter"></param>
		/// <returns></returns>
		protected string HandleAjaxException(Exception error, string delimiter = "<br/>", Action postback = null)
		{
			string message;
			var validation_error = error as DbEntityValidationException;
			var not_authorized_error = error as NotAuthorizedException;

			var type = error.GetType();
			if (type == typeof(DbEntityValidationException))
			{
				message = validation_error.GetValidationMessages(delimiter);
			}
			else if (type == typeof(DbUpdateConcurrencyException))
			{
				message = "The record has been updated by someone else, please reload the page.";
			}
			else if (type == typeof(NotAuthorizedException))
			{
				message = "You are not authorized to access this page and/or information.";
			}
			else
			{
				message = error.GetAllMessages(delimiter);
				message = "An error has occured, please contact support.";
			}

			if (postback != null)
			{
				Response.TrySkipIisCustomErrors = true;
				postback?.Invoke();
			}

			return message;
		}
		#endregion
	}
}
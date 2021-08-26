using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Int.Models.ProgramNotifications;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Int.Controllers
{
	/// <summary>
	/// ProgramNotificationController class, provides endpoints to manage program notifications.
	/// </summary>
	[AuthorizeAction(Privilege.AM4, Privilege.SM)]
	[RouteArea("Int")]
	[RoutePrefix("Admin/Program")]
	public class ProgramNotificationController : BaseController
	{
		#region Variables
		private readonly IProgramNotificationService _programNotificationService;
		private readonly INotificationService _notificationService;
		private readonly IUserService _userService;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a ProgramNotificationController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="programNotificationService"></param>
		/// <param name="notificationService"></param>
		/// <param name="userService"></param>
		public ProgramNotificationController(
			IControllerService controllerService,
			IProgramNotificationService programNotificationService,
			INotificationService notificationService,
			IUserService userService
		   ) : base(controllerService.Logger)
		{
			_programNotificationService = programNotificationService;
			_notificationService = notificationService;
			_userService = userService;
		}
		#endregion

		#region Endpoints
		/// <summary>
		/// Returns a view to manage program notifications.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Notifications/View")]
		public ActionResult ProgramNotificationsView()
		{
			return View();
		}

		/// <summary>
		/// Returns an array of program notifications.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="quantity"></param>
		/// <param name="search"></param>
		/// <returns></returns>
		[HttpGet, Route("Notifications/Search/{page}/{quantity}")]
		[ValidateRequestHeader]
		public JsonResult GetProgramNotifications(int page, int quantity, string search)
		{
			var model = new BaseViewModel();
			try
			{
				var programNotifications = _programNotificationService.GetProgramNotifications(page, quantity, search);
				var result = new
				{
					RecordsFiltered = programNotifications.Items.Count(),
					RecordsTotal = programNotifications.Total,
					Data = programNotifications.Items.Select(o => new ProgramNotificationListViewModel(o)).ToArray()
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns the current user email.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Notification/User/Email")]
		public JsonResult GetUserEmail()
		{
			var model = new BaseViewModel();
			try
			{
				var user = _userService.GetInternalUser(User.GetUserId().Value);
				var result = user.Email;
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get the program notification modal view.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Modal/View")]
		public ActionResult ProgramNotificationModalView()
		{
			return PartialView();
		}

		/// <summary>
		/// Returns the data for the specified program notification.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Notification/{id}")]
		public JsonResult GetProgramNotification(int id)
		{
			var model = new ProgramNotificationViewModel();
			try
			{
				var programNotification = _programNotificationService.Get(id);
				model = new ProgramNotificationViewModel(programNotification);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns the number of applicants registered and per grant program.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Notification/Applicants")]
		public JsonResult GetGrantProgramApplicants()
		{
			var model = new BaseViewModel();
			try
			{
				var result = new
				{
					NumberOfApplicants = _programNotificationService.GetNumberOfApplicants(),
					NumberOfApplicantsPerGrantProgram = _programNotificationService.GetNumberOfApplicantsPerGrantProgram()
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Adds a new program notification to the datasource.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Notification")]
		public JsonResult AddProgramNotification(ProgramNotificationViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					if (_programNotificationService.Exists(model.Id, model.Caption))
						throw new InvalidOperationException($"A Program Notification with this name '{model.Caption}' already exists, you can't create a new one with the same name.");

					var programNotification = new ProgramNotification
					{
						NotificationTemplate = new NotificationTemplate()
					};
					model.SetProgramNotification(programNotification);
					_programNotificationService.Add(programNotification);
					model = new ProgramNotificationViewModel(programNotification);
				}
				else
				{
					HandleModelStateValidation(model);
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model);
		}

		/// <summary>
		/// Updates the program notification in the datasource.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Notification")]
		public JsonResult UpdateProgramNotification(ProgramNotificationViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					if (_programNotificationService.Exists(model.Id, model.Caption))
						throw new InvalidOperationException($"A Program Notification with this name '{model.Caption}' already exists, you can't create a new one with the same name.");

					var programNotification = _programNotificationService.Get(model.Id);
					model.SetProgramNotification(programNotification, _programNotificationService);
					_programNotificationService.Update(programNotification);
					model = new ProgramNotificationViewModel(programNotification);
				}
				else
				{
					HandleModelStateValidation(model);
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model);
		}

		/// <summary>
		/// Deletes the specified program notification from the datasource.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Notification/Delete")]
		public JsonResult DeleteProgramNotification(ProgramNotificationViewModel model)
		{
			try
			{
				var programNotification = _programNotificationService.Get(model.Id);
				programNotification.RowVersion = Convert.FromBase64String(model.RowVersion);
				_programNotificationService.Delete(programNotification);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model);
		}

		/// <summary>
		/// Add the program notification to the notification queue so that it will be emailed out the next time the notification service runs.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpPost]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Notification/Send/{id}")]
		public JsonResult SendProgramNotification(int id)
		{
			var model = new BaseViewModel();
			try
			{
				_programNotificationService.Send(id);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model);
		}

		/// <summary>
		/// Sends a test email of the program notification to the current user.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Notification/Test")]
		public JsonResult TestProgramNotification(ProgramNotificationPreviewModel data)
		{
			var model = new BaseViewModel();
			try
			{
				var sender = System.Configuration.ConfigurationManager.AppSettings["EmailFromAddress"];
				var name = System.Configuration.ConfigurationManager.AppSettings["EmailFromDisplayName"];
				var user = _userService.GetInternalUser(User.GetUserId().Value);
				var applicant = new User(Guid.NewGuid(), user.FirstName, user.LastName, user.Email, new Organization() { LegalName = name }, new Address());
				_notificationService.SendNotification(new NotificationQueue(new ProgramNotification(data.Name, new NotificationTemplate(data.Name, data.Subject, data.Body)), applicant, sender)
				{
					Organization = applicant.Organization
				}, true);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model);
		}

		/// <summary>
		/// Returns a view to display the specified program notification parsed template.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="description"></param>
		/// <param name="subject"></param>
		/// <param name="body"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("Notification/Preview/View")]
		public ActionResult ProgramNotificationsPreview(ProgramNotificationPreviewModel model)
		{
			TempData["_RemoveHeader"] = true;
			return View(model);
		}
		#endregion
	}
}

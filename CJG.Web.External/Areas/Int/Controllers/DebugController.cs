using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.GrantOpeningService;
using CJG.Infrastructure.Identity;
using CJG.Infrastructure.ReportingService;
using CJG.Web.External.Areas.Int.Models;
using CJG.Web.External.Areas.Int.Models.Debug;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;
using Microsoft.AspNet.Identity;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Int.Controllers
{
	/// <summary>
	/// DebugController class, provides endpoints for debugging the application.
	/// </summary>
	[RouteArea("Int")]
	[RoutePrefix("Debug")]
	[AuthorizeAction(Privilege.SM)]
	public class DebugController : BaseController
	{
		#region Variables
		private readonly ILogService _logService;
		private readonly ISiteMinderService _siteMinderService;
		private readonly IUserService _userService;
		private readonly ISettingService _settingService;
		private readonly ApplicationUserManager _userManager;
		private readonly ApplicationSignInManager _signInManager;
		private readonly IGrantOpeningService _grantOpeningService;
		private readonly IParticipantService _participantService;
		private readonly INotificationService _notificationService;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a DebugController object, and initializes with arguments.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="logService"></param>
		/// <param name="settingService"></param>
		/// <param name="userManager"></param>
		/// <param name="signInManager"></param>
		/// <param name="grantOpeningService"></param>
		/// <param name="participantService"></param>
		public DebugController(
			IControllerService controllerService,
			ILogService logService,
			ISettingService settingService,
			ApplicationUserManager userManager,
			ApplicationSignInManager signInManager,
			IGrantOpeningService grantOpeningService,
			IParticipantService participantService,
			INotificationService notificationService) : base(controllerService.Logger)
		{
			_siteMinderService = controllerService.SiteMinderService;
			_userService = controllerService.UserService;
			_logService = logService;
			_settingService = settingService;
			_userManager = userManager;
			_signInManager = signInManager;
			_grantOpeningService = grantOpeningService;
			_participantService = participantService;
			_notificationService = notificationService;
		}

		#endregion

		#region Endpoints
		/// <summary>
		///     Display the debug screen menu.
		/// </summary>
		/// <returns></returns>
		public ActionResult Index()
		{
			return View();
		}

		#region BCeID
		[HttpGet]
		public ActionResult SyncBCeIDAccounts()
		{
			return View();
		}

		public JsonResult GetAllBCeIDAccounts()
		{
			var model = new SyncBCeIDAccoutsViewModel();
			try
			{
				model.Skip = 0;
				model.Take = 1;
				model.Ids = _userService.GetBCeIDUsers();
				model.NumberOfAccounts = model.Ids.Count();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult UpdateAllBCeIDAccounts(SyncBCeIDAccoutsViewModel model)
		{
			try
			{
				if (model.Skip == model.NumberOfAccounts)
				{
					model.IsCompleted = true;
					return Json(model, JsonRequestBehavior.AllowGet);
				}

				foreach (var userId in model.Ids.Skip(model.Skip).Take(model.Take))
				{
					var user = _userService.GetUser(userId);
					try
					{
						if (_userService.SyncUserFromBCeIDAccount(user))
						{
							model.NumberOfUpdatedAccounts++;
							System.Threading.Thread.Sleep(500);
						}
					}
					catch (Exception e)
					{
						model.Failed.Add(new FailedBCeIDAccount
						{
							BCeID = user.BCeID,
							FirstName = user.FirstName,
							LastName = user.LastName,
							Reason = e.Message
						});
						continue;
					}
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		///     Display the BCeID debugger View.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public ActionResult BCeID()
		{
			return View(new BCeIDViewModel());
		}

		/// <summary>
		///     Make a request to BCeID for information.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult BCeID(BCeIDViewModel model)
		{
			try
			{
				var requester = model.Requester ?? _siteMinderService.CurrentUserGuid;
				var requesterAccountType = model.RequesterAccountType ?? _siteMinderService.CurrentUserType;
				User user;
				if (!string.IsNullOrEmpty(model.UserId?.Trim()))
				{
					user = _userService.GetBCeIDUser(model.UserId.Trim(), requester, requesterAccountType);
				}
				else if (model.BCeID.HasValue)
				{
					user = _userService.GetBCeIDUser(model.BCeID.Value, requester, requesterAccountType);
				}
				else
					user = new User();

				ModelState.Clear();
				model.User = user;
				model.UserId = string.IsNullOrEmpty(user.BCeID) ? model.UserId : user.BCeID;
				model.BCeID = user.BCeIDGuid;
				model.Requester = model.Requester;
				model.RequesterAccountType = model.RequesterAccountType;
			}
			catch (Exception ex)
			{
				this.SetAlert(ex);
			}
			return View(model);
		}

		/// <summary>
		///     Provides a way to debug the currently logged in user.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public ActionResult CurrentUser()
		{
			var model = new DebugUserViewModel();

			model.SiteMinderInfo = new SiteMinderInfoViewModel
			{
				CurrentUserGuid = _siteMinderService.CurrentUserGuid,
				CurrentUserType = _siteMinderService.CurrentUserType
			};

			try
			{
				var currentUser = _userManager.FindById(User.Identity.GetUserId());

				if (currentUser?.InternalUser != null)
					model.InternalUser = currentUser.InternalUser;

				return View(model);
			}
			catch (Exception ex)
			{
				this.SetAlert(ex);
				return View(model);
			}
		}

		/// <summary>
		///     A way to get the GUID for the IDIR account.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public ActionResult IDIR()
		{
			var model = new DebugIDIRViewModel();
			return View(model);
		}

		/// <summary>
		///     A way to get the GUID for the IDIR account.
		/// </summary>
		/// <param name="idir"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult IDIR(DebugIDIRViewModel model)
		{
			var user = _userService.GetIDIRUser(model.IDIR);

			if (user == null)
			{
				this.SetAlert($"The IDIR account '{model.IDIR}' does not exist.", AlertType.Warning, true);
				return RedirectToAction("Index");
			}

			ModelState.Clear();
			model.Guid = user.BCeIDGuid.ToString();
			model.IDIR = model.IDIR;

			return View(model);
		}

		/// <summary>
		///     A way to impersonate another internal user.
		/// </summary>
		/// <param name="idir"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("int/debug/impersonate/{idir}")]
		public ActionResult Impersonate(string idir)
		{
#if DEBUG
			var user = _userManager.Users.FirstOrDefault(u => u.InternalUser.IDIR == idir);
			if (user != null)
			{
				_signInManager.SignInAsync(user, true, false).Wait();
				Session[Application.Services.Constants.SiteMinderSimulator_UserGuid_SessionVariableName] = new Guid(user.Id);
				Session[Application.Services.Constants.SiteMinderSimulator_UserName_SessionVariableName] = user.UserName;
				Session[Application.Services.Constants.SiteMinderSimulator_UserType_SessionVariableName] = AccountTypes.Internal.ToString();
			}
#endif

			return RedirectToAction("Index", "Home");
		}
		#endregion

		#region Logging
		/// <summary>
		///     Display the log entries View.
		/// </summary>
		/// <param name="page"></param>
		/// <returns></returns>
		public ActionResult Log(int page = 1)
		{
			var numberOfItems = 25;
			var logs = _logService.GetLogs(page, numberOfItems);
			var model = new DebugLogViewModel(page, numberOfItems, logs, logs.Count());
			return View(model);
		}

		[HttpPost]
		public ActionResult Log(DebugLogViewModel model)
		{
			var logs = _logService.Filter(model.Level ?? "*", model.DateAdded, model.Message, model.UserName, model.Page, model.ItemsPerPage);

			ModelState.Clear();
			model.Logs = logs;
			model.Total = logs.Count();
			return View(model);
		}
		#endregion

		#region Application Date/Time
		/// <summary>
		/// Display the change application date/time view.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public ActionResult AppDateTimeView()
		{
			return View();
		}

		/// <summary>
		/// Get the current application date time.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("AppDateTime")]
		public JsonResult GetAppDateTime()
		{
			var model = new DebugAppDateTimeViewModel();
			try
			{
				model = new DebugAppDateTimeViewModel(AppDateTime.Now);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Set the application date/time.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("AppDateTime")]
		public JsonResult UpdateAppDateTime(DebugAppDateTimeViewModel model)
		{
			try
			{
				AppDateTime.SetNow(model.Year, model.Month, model.Day, model.Hour, model.Minute, model.Second);

				_settingService.AddOrUpdate(new Setting("AppDateTime", AppDateTime.Now));

				model = new DebugAppDateTimeViewModel(AppDateTime.Now);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}

			return Json(model);
		}

		/// <summary>
		/// Reset the application date to server time.
		/// </summary>
		/// <returns></returns>
		[HttpPut]
		[Route("AppDateTime/Reset")]
		public JsonResult ResetAppDateTime()
		{
			var model = new DebugAppDateTimeViewModel();
			try
			{
				AppDateTime.ResetNow();

				var setting = _settingService.Get("AppDateTime");
				_settingService.Delete(setting);

				model = new DebugAppDateTimeViewModel(AppDateTime.Now);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}

			return Json(model);
		}
		#endregion

		#region Information
		/// <summary>
		///     Display the version information.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public ActionResult Version()
		{
			return View(typeof(MvcApplication).Assembly.GetName().Version);
		}
		#endregion

		#region Notifications Service
		/// <summary>
		/// A view to debug the notification service.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Notifications/Service/View")]
		public ActionResult NotificationsServiceView()
		{
			return View();
		}

		/// <summary>
		/// Get the notifications service debug model.
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[Route("Notifications/Service/Queue")]
		public JsonResult GetNotificationQueue(NotificationQueueFilterViewModel filter)
		{
			var model = new DebugNotificationsViewModel();
			try
			{
				var queue = _notificationService.GetNotifications(filter.Page, filter.Quantity, filter.GenerateFilter());
				model = new DebugNotificationsViewModel(0, queue.Items.Select(n => new Models.Debug.NotificationQueueViewModel(n)))
				{
					NotificationsInQueue = queue.Total.Value
				};
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model);
		}

		/// <summary>
		/// Add scheduled notifications to the queue..
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("Notifications/Service/Queue/Scheduled/Notifications")]
		public JsonResult QueueScheduledNotifications(DebugNotificationsViewModel model)
		{
			try
			{
				var addToQueue = _notificationService.QueueScheduledNotifications(model.RunAsDate.Value);
				var queue = _notificationService.GetNotifications(1, 10, new NotificationFilter(null, "Queued,Failed", "State"));

				model = new DebugNotificationsViewModel(addToQueue, queue.Items.Select(q => new Models.Debug.NotificationQueueViewModel(q)));
			}
			catch (Exception ex)
			{
				_logger.Debug(ex, $"Error occured while adding the notification queue.");
				HandleAngularException(ex, model);
			}

			return Json(model);
		}

		/// <summary>
		/// Send the notifications in the queue.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("Notifications/Service/Send/Notifications")]
		public JsonResult SendNotifications(DebugNotificationsViewModel model)
		{
			try
			{
				var queue = _notificationService.SendScheduledNotifications(model.RunAsDate.Value);

				model = new DebugNotificationsViewModel(0, queue.Select(q => new Models.Debug.NotificationQueueViewModel(q)))
				{
					RunAsDate = model.RunAsDate
				};
			}
			catch (Exception ex)
			{
				_logger.Debug(ex, $"Error occured while sending the notifications.");
				HandleAngularException(ex, model);
			}

			return Json(model);
		}
		#endregion

		#region Grant Opening Service
		[HttpGet]
		public ActionResult GrantOpeningService()
		{
			var model = new DebugGrantOpeningViewModel { CurrentDate = DateTime.Now };
			return View(model);
		}

		[HttpPost]
		public ActionResult GrantOpeningService(DebugGrantOpeningViewModel model)
		{
			model.LogRecords = new List<string>();

			try
			{
				using (new DisposableListLogger(model.LogRecords, LogManager.Configuration))
				{
					new GrantOpeningJob(_grantOpeningService, _logger).Start(model.CurrentDate, model.NumberOfDaysBefore);
				}
			}
			catch (Exception e)
			{
				model.LogRecords.Add(e.Message);
			}
			return View(model);
		}
		#endregion

		#region Reporting Service
		[HttpGet]
		public ActionResult ReportingService()
		{
			return View(new DebugReportingViewModel { CurrentDate = DateTime.Now });
		}

		[HttpPost]
		public ActionResult ReportingService(DebugReportingViewModel model, string submit)
		{
			if (model == null)
			{
				model = new DebugReportingViewModel();
			}

			model.LogRecords = new List<string>();

			try
			{
				StartReportingService(model);
			}
			catch (Exception e)
			{
				model.LogRecords.Add(e.Message);
			}

			return View(model);
		}

		[HttpGet]
		public ActionResult ReportingServiceDownload(string filePath)
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(filePath))
				{
					var tempPath = Path.GetTempPath();
					var fileInfo = new FileInfo(Path.Combine(tempPath, filePath));
					Response.Clear();
					Response.AddHeader("Content-Disposition", "attachment; filename=" + fileInfo.Name);
					Response.AddHeader("Content-Length", fileInfo.Length.ToString(CultureInfo.InvariantCulture));
					Response.ContentType = "application/octet-stream";
					Response.WriteFile(fileInfo.FullName);
				}
				else
				{
					Response.Write("No files to download");
				}
			}
			catch (Exception e)
			{
				Response.ClearContent();
				Response.Write(e.Message);
			}

			return null;
		}

		private void StartReportingService(DebugReportingViewModel model)
		{
			const string fileNamePrefix = "participant-report";
			const string sdsiReportTemplateHtml = "SDSI-Report-Template.html";

			var tempPath = Path.GetTempPath();
			var tempSessionDir = Directory.CreateDirectory(Path.Combine(tempPath, Guid.NewGuid().ToString("N")));

			var csvFilePath = Path.Combine(tempSessionDir.FullName, fileNamePrefix + string.Format("-{0:yyyy-MM-dd}.csv", DateTime.Now));
			var htmlFilePathTemplate =
				Path.Combine(tempSessionDir.FullName,
					fileNamePrefix + string.Format("-{0:yyyy-MM-dd}-::ParticipantFormId::.html", DateTime.Now));

			using (new DisposableListLogger(model.LogRecords, LogManager.Configuration))
			{
				new SdsiReportJob(_participantService, _logger)
					.Start(model.CurrentDate, csvFilePath, htmlFilePathTemplate,
						model.NumberOfDaysBefore,
						Path.Combine(Server.MapPath(@"~/bin"), sdsiReportTemplateHtml),
						model.MaxParticipants, model.AddHeader);
			}

			var paths = Directory.EnumerateFiles(tempSessionDir.FullName, $"{fileNamePrefix}*.csv").ToList();
			paths.AddRange(Directory.EnumerateFiles(tempSessionDir.FullName, $"{fileNamePrefix}*.html"));

			model.FileNames = paths
				.Select(x => new SelectListItem { Value = x.Replace(tempPath, string.Empty), Text = Path.GetFileName(x) }).ToList();
		}
		#endregion
		#endregion
	}
}
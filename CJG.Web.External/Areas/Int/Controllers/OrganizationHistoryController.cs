using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using CJG.Application.Services;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Areas.Int.Models.Organizations;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Controllers
{
    /// <summary>
    /// <typeparamref name="OrganizationHistoryController"/> class, provides endpoints to manage organization History.
    /// </summary>
    [RouteArea("Int")]
	[RoutePrefix("Organization")]
	[AuthorizeAction(Privilege.AM1, Privilege.AM2, Privilege.AM3, Privilege.AM4, Privilege.AM5)]
	public class OrganizationHistoryController : BaseController
	{
		private readonly IOrganizationService _organizationService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IUserService _userService;
		private readonly IGrantProgramService _grantProgramService;
		private readonly IAttachmentService _attachmentService;

		/// <summary>
		/// Creates a new instance of a <typeparamref name="OrganizationHistoryController"/> object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="organizationService"></param>
		/// <param name="grantApplicationService"></param>
		/// <param name="grantProgramService"></param>
		/// <param name="userService"></param>
		public OrganizationHistoryController(
			IControllerService controllerService,
			IOrganizationService organizationService,
			IGrantApplicationService grantApplicationService,
			IGrantProgramService grantProgramService,
			IUserService userService,
			IAttachmentService attachmentService) : base(controllerService.Logger)
		{
			_organizationService = organizationService;
			_grantApplicationService = grantApplicationService;
			_userService = userService;
			_grantProgramService = grantProgramService;
			_attachmentService = attachmentService;
		}

		/// <summary>
		/// Returns the organization grant file history view.
		/// </summary>
		/// <param name="organizationId"></param>
		/// <returns></returns>
		[HttpGet, Route("History/View/{organizationId}")]
		public ActionResult OrganizationGrantFileHistoryView(int? organizationId)
		{
			ViewBag.OrganizationId = organizationId;
			//logic to handle return click - either return to organization page or application page
			ViewBag.Path = new List<string>();

			string path = Request.UrlReferrer.LocalPath;
			var listOfPath = path.Split('/').ToList();

			foreach (var item in listOfPath)
			{
				ViewBag.Path.Add(item);
			}

			return View();
		}

		/// <summary>
		/// Get the organization grant file history view data.
		/// </summary>
		/// <param name="organizationId"></param>
		/// <returns></returns>
		[HttpGet, Route("History/{organizationId}")]
		public JsonResult GetOrganizationGrantFileHistory(int organizationId)
		{
			var model = new OrganizationGrantFileHistoryViewModel();
			try
			{
				var organization = _organizationService.Get(organizationId);
				model = new OrganizationGrantFileHistoryViewModel(organization, _organizationService)
				{
					AllowDeleteOrganization = User.IsInRole("Director") || User.IsInRole("Assessor") || User.IsInRole("System Administrator"),
					UrlReferrer = Request.UrlReferrer?.AbsolutePath ??
					   new UrlHelper(this.ControllerContext.RequestContext).Action(nameof(OrganizationController.OrganizationsView), nameof(OrganizationController).Replace("Controller", ""))
				};
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		[HttpGet, Route("HistoryYTD/{organizationId}/{grantProgramId}")]
		public JsonResult GetOrganizationGrantHistoryYTD(int organizationId, int grantProgramId)
		{
			var model = new OrganizationGrantFileYTDViewModel();
			try
			{
				var result = _organizationService.GetOrganizationYTD(organizationId);
				model.TotalRequested = result.TotalRequested;
				model.TotalApproved = result.TotalApproved;
				model.TotalPaid = result.TotalPaid;
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get grant file histories for organization.
		/// </summary>
		/// <param name="organizationId"></param>
		/// <param name="page"></param>
		/// <param name="quantity"></param>
		/// <param name="grantProgramId"></param>
		/// <param name="search"></param>
		/// <returns></returns>
		[HttpGet, Route("History/Search/{organizationId}/{page}/{quantity}")]
		[ValidateRequestHeader]
		[AuthorizeAction(Privilege.IA1)]
		public JsonResult GetOrganizationGrantFileHistory(int organizationId,int page, int quantity, int grantProgramId, string search, string sortby, bool sortDesc)
		{
			var model = new BaseViewModel();
			try
			{
				if (grantProgramId == 0)
					grantProgramId = _grantProgramService.GetDefaultGrantProgramId();

				var grantApplications = _grantApplicationService.GetGrantApplicationsForOrg(organizationId, grantProgramId, search);

				var history = grantApplications.ToList().Select(o => new OrganizationGrantFileHistoryDataTableModel(o, _userService)).ToList();

				if (string.IsNullOrEmpty(sortby))
					sortby = "FileNumber";

				var prop = typeof(OrganizationGrantFileHistoryDataTableModel).GetProperty(sortby);
				history = sortDesc ? history.OrderByDescending(o => prop.GetValue(o, null)).ToList() : history.OrderBy(o => prop.GetValue(o, null)).ToList();

				var filtered = history
					.Skip((page - 1) * quantity)
					.Take(quantity)
					.ToArray();

				var result = new
				{
					RecordsFiltered = filtered.Count(),
					RecordsTotal = history.Count(),
					Data = filtered
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
		/// Update the organization notes/riskflag.
		/// </summary>
		/// <param name="organizationId"></param>
		/// <param name="notesText"></param>
		/// <param name="riskFlag"></param>
		/// <param name="rowVersion"></param>
		/// <returns></returns>
		[HttpPut, Route("History/Change/{organizationId}")]
		[AuthorizeAction(Privilege.TP1, Privilege.TP2)]
		public JsonResult UpdateNote(int organizationId, string notesText, bool riskFlag, string rowVersion)
		{
			var model = new OrganizationGrantFileHistoryViewModel();
			try
			{
				var organization = _organizationService.Get(organizationId);

				organization.RowVersion = Convert.FromBase64String(rowVersion.Replace(" ", "+"));
				organization.Notes = notesText;
				organization.RiskFlag = riskFlag;

				_organizationService.UpdateOrganization(organization);

				model.RowVersion = Convert.ToBase64String(organization.RowVersion);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get the business license listing for an organization
		/// </summary>
		/// <param name="organizationId"></param>
		/// <returns></returns>
		[HttpGet, Route("History/BusinessLicenses/{organizationId}")]
		public JsonResult GetOrganizationBusinessLicenses(int organizationId)
		{
			var model = new OrganizationBusinessLicensesModel();
			try
			{
				var organization = _organizationService.Get(organizationId);
				model = new OrganizationBusinessLicensesModel(organization);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Adds specified attachment
		/// </summary>
		/// <param name="organizationId"></param>
		/// <param name="file"></param>
		/// <param name="attachments"></param>
		/// <returns></returns>
		[HttpPost]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("History/BusinessLicenses/License")]
		public ActionResult AddBusinessLicense(int organizationId, HttpPostedFileBase file, string attachments)
		{
			var model = new BaseViewModel();
			try
			{
				var organization = _organizationService.Get(organizationId);

				var attachmentViewModel = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Attachments.UpdateAttachmentViewModel>(attachments);

				//organization.BusinessLicenseDocuments.Add(attachment);

				var attachment = file.UploadFile(attachmentViewModel.Description, attachmentViewModel.FileName);

				if (attachmentViewModel.Id == 0)
				{
					organization.BusinessLicenseDocuments.Add(attachment);
					_attachmentService.Add(attachment, true);
				}

				//_claimService.AddReceipt(claimId, claimVersion, attachment);
				//var claim = _claimService.Get(claimId, claimVersion);
				model = new OrganizationBusinessLicensesModel(organization);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Downloads specified attachment
		/// </summary>
		/// <param name="organizationId"></param>
		/// <param name="attachmentId"></param>
		/// <returns></returns>
		[HttpGet]
		[PreventSpam]
		[Route("History/BusinessLicense/Download/{organizationId}/{attachmentId}")]
		public ActionResult DownloadAttachment(int organizationId, int attachmentId)
		{
			var model = new BaseViewModel();
			try
			{
				var organization = _organizationService.Get(organizationId);
				var attachment = _attachmentService.Get(attachmentId);
				return File(attachment.AttachmentData, MediaTypeNames.Application.Octet, $"{attachment.FileName}");
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}
	}
}

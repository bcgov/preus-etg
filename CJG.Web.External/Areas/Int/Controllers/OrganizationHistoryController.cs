using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Int.Models.Organizations;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

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
		#region Variables
		private readonly IOrganizationService _organizationService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IUserService _userService;
		private readonly IGrantProgramService _grantProgramService;
		#endregion

		#region Constructors

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
			IUserService userService) : base(controllerService.Logger)
		{
			_organizationService = organizationService;
			_grantApplicationService = grantApplicationService;
			_userService = userService;
			_grantProgramService = grantProgramService;
		}
		#endregion

		#region Endpoints
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
				if (grantProgramId == 0)
					grantProgramId = _grantProgramService.GetDefaultGrantProgramId();

				var result = _organizationService.GetOrganizationYTD(organizationId, grantProgramId);
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

		[HttpGet]
		[Route("History/Program/Types")]
		public JsonResult GetProgramTypes()
		{
			IEnumerable<KeyValuePair<int, string>> results = new KeyValuePair<int, string>[0];
			try
			{
				var grantPrograms = _grantProgramService.GetAll();
				results = grantPrograms.Select(p => new KeyValuePair<int, string>(p.Id, p.Name)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(results, JsonRequestBehavior.AllowGet);
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

				List<OrganizationGrantFileHistoryDataTableModel> history = grantApplications.ToList().Select(o => new OrganizationGrantFileHistoryDataTableModel(o, _userService)).ToList();

				//SORT
				if (string.IsNullOrEmpty(sortby))
				{
					sortby = "FileNumber";
				}
				System.Reflection.PropertyInfo prop = typeof(OrganizationGrantFileHistoryDataTableModel).GetProperty(sortby);
				if (sortDesc)
				{
					history = history.OrderByDescending(o => prop.GetValue(o, null)).ToList();
				}
				else
				{
					history = history.OrderBy(o => prop.GetValue(o, null)).ToList();
				}

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
		
		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CJG.Core.Entities.Helpers;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;

namespace CJG.Web.External.Areas.Int.Controllers
{
    /// <summary>
    /// RegionalExceptionControl class, provides endpoints to select and search for regional exceptions.
    /// </summary>
    [RouteArea("Int")]
	[Authorize(Roles = "Assessor, System Administrator, Director, Financial Clerk")]
	public class RegionalExceptionController : BaseController
	{
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IFiscalYearService _fiscalYearService;
		private readonly IGrantProgramService _grantProgramService;
		private readonly IGrantStreamService _grantStreamService;
		private readonly IPrioritizationService _prioritizationService;

		public RegionalExceptionController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			IFiscalYearService fiscalYearService,
			IGrantProgramService grantProgramService,
			IGrantStreamService grantStreamService,
			IPrioritizationService prioritizationService
		   ) : base(controllerService.Logger)
		{
			_grantApplicationService = grantApplicationService;
			_fiscalYearService = fiscalYearService;
			_grantProgramService = grantProgramService;
			_grantStreamService = grantStreamService;
			_prioritizationService = prioritizationService;
		}

		[HttpGet]
		[Route("RegionalExceptions/View")]
		public ActionResult RegionalExceptionsView()
		{
			return View();
		}

        [HttpGet]
        [Route("RegionalExceptions/Regions")]
        public JsonResult GetRegions()
        {
            IEnumerable<KeyValuePair<int, string>> results = new KeyValuePair<int, string>[0];
            try
            {
                var regions = _prioritizationService.GetPrioritizationRegions();
                results = regions.Select(a => new KeyValuePair<int, string>(a.Id, a.Name)).ToArray();
            }
            catch (Exception ex)
            {
                HandleAngularException(ex);
            }
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
		[Route("RegionalExceptions/Fiscal/Years")]
		public JsonResult GetFiscalYears()
		{
			IEnumerable<KeyValuePair<int, string>> results = new KeyValuePair<int, string>[0];
			try
			{
				var fiscalYears = _fiscalYearService.GetFiscalYears();
				results = fiscalYears.Select(fy => new KeyValuePair<int, string>(fy.Id, fy.Caption)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(results, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Route("RegionalExceptions/Training/Periods/{fiscalYearId:int}/{grantStreamId:int?}")]
		public JsonResult GetTrainingPeriods(int fiscalYearId, int? grantStreamId)
		{
			IEnumerable<KeyValuePair<string, string>> results = new KeyValuePair<string, string>[0];
			try
			{
				results = _fiscalYearService.GetTrainingPeriodLabels(fiscalYearId, grantStreamId);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(results, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Route("RegionalExceptions/Grant/Programs")]
		public JsonResult GetGrantPrograms()
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

		[HttpGet]
		[Route("RegionalExceptions/Grant/Streams/{grantProgramId?}")]
		public JsonResult GetGrantStreams(int? grantProgramId)
		{
			IEnumerable<KeyValuePair<int, string>> results = new KeyValuePair<int, string>[0];
			try
			{
				var grantStreams = grantProgramId.HasValue ? _grantStreamService.GetGrantStreamsForProgram(grantProgramId.Value) : _grantStreamService.GetAll();
				results = grantStreams.Select(s => new KeyValuePair<int, string>(s.Id, s.Name)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(results, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[ValidateRequestHeader]
		[Route("RegionalExceptions/Queue")]
		public JsonResult GetGrantApplications(Models.Prioritization.PrioritizationFilterViewModel filter)
		{
			var model = new PageList<Models.Prioritization.GrantApplicationViewModel>();
			try
			{
				int.TryParse(Request.QueryString["page"], out int pageNumber);
				int.TryParse(Request.QueryString["quantity"], out int quantityNumber);

				var query = filter.GetFilter();
				var applications = _grantApplicationService.GetGrantApplications(pageNumber, quantityNumber, query);

				model.Page = applications.Page;
				model.Quantity = applications.Quantity;
				model.Total = applications.Total;
				model.Items = applications.Items.Select(a => new Models.Prioritization.GrantApplicationViewModel(a));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, filter);
				return Json(filter);
			}
			return Json(model);
		}

		[HttpGet]
		[Route("RegionalExceptions/SelectRegion/View")]
		public ActionResult SelectRegionView()
		{
			return PartialView();
		}

		/// <summary>
		/// Select for assessment the specified grant application.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="selectedRegionId"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[AuthorizeAction(Privilege.IA1)]
		[Route("RegionalExceptions/SelectRegion")]
		public JsonResult SelectRegion(int grantApplicationId, int selectedRegionId)
		{
			var model = new Models.Prioritization.GrantApplicationViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);

				_prioritizationService.SetRegionException(grantApplication, selectedRegionId);
				_prioritizationService.AddPostalCodeToRegion(grantApplication, selectedRegionId);

				_grantApplicationService.UpdateWithNoPermissionCheck(grantApplication);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model);
		}
	}
}
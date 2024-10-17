using CJG.Application.Business.Models;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;

namespace CJG.Web.External.Areas.Int.Controllers
{
	[RouteArea("Int")]
	public class StaticDataController : BaseController
	{
		#region Variables
		private readonly IStaticDataService _staticDataService;
		private readonly IExpenseTypeService _expenseTypeService;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a StaticDataController object, and initializes it with the specified services.
		/// </summary>
		/// <param name="expenseTypeService"></param>
		/// <param name="controllerService"></param>
		public StaticDataController(
			IExpenseTypeService expenseTypeService,
			IControllerService controllerService) : base(controllerService.Logger)
		{
			_staticDataService = controllerService.StaticDataService;
			_expenseTypeService = expenseTypeService;
;
		}
		#endregion

		#region Endpoints
		#region Summary
		/// <summary>
		/// Get an array of active organization types.
		/// </summary>
		/// <returns>JsonResult</returns>
		[OutputCache(CacheProfile = "StaticData", VaryByParam = "none", Location = OutputCacheLocation.ServerAndClient)]
		[Route("Application/Applicant/Organization/Types")]
		public JsonResult GetOrganizationTypes()
		{
			IEnumerable<KeyValuePair<int, string>> organizationTypeList = null;

			try
			{
				organizationTypeList = _staticDataService.GetOrganizationTypes().Select(t => new KeyValuePair<int, string>(t.Id, t.Caption)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}

			return Json(organizationTypeList, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get an array of active legal structures.
		/// </summary>
		/// <returns>JsonResult</returns>
		[OutputCache(CacheProfile = "StaticData", VaryByParam = "none", Location = OutputCacheLocation.ServerAndClient)]
		[Route("Application/Applicant/Legal/Structures")]
		public JsonResult GetLegalStructures()
		{
			IEnumerable<KeyValuePair<int, string>> legalStructureList = null;

			try
			{
				legalStructureList = _staticDataService.GetLegalStructures().Select(ls => new KeyValuePair<int, string>(ls.Id, ls.Caption)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}

			return Json(legalStructureList, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get an array of risk classifications.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[OutputCache(CacheProfile = "StaticData", VaryByParam = "none", Location = OutputCacheLocation.ServerAndClient)]
		[Route("Application/Summary/Risk/Classifications")]
		public JsonResult GetRiskClassifications()
		{
			RiskClassification[] riskClassifications = null;

			try
			{
				riskClassifications = _staticDataService.GetRiskClassifications().ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}

			return Json(riskClassifications, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region Training Program
		[HttpGet]
		[OutputCache(CacheProfile = "StaticData", VaryByParam = "none", Location = OutputCacheLocation.ServerAndClient)]
		[Route("Application/Training/Program/Delivery/Methods")]
		[Route("Application/Training/Provider/Delivery/Methods")]
		public JsonResult GetDeliveryMethods()
		{
			KeyValueListItem<int, string>[] model = null;
			try
			{
				var entity = _staticDataService.GetDeliveryMethods();
				model = entity.Select(o => new KeyValueListItem<int, string>(o.Id, o.Caption)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[OutputCache(CacheProfile = "StaticData", VaryByParam = "none", Location = OutputCacheLocation.ServerAndClient)]
		[Route("Application/Training/Program/Skill/Levels")]
		public JsonResult GetSkillLevels()
		{
			KeyValueListItem<int, string>[] model = null;
			try
			{
				var entity = _staticDataService.GetSkillLevels();
				model = entity.Select(o => new KeyValueListItem<int, string>(o.Id, o.Caption)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[OutputCache(CacheProfile = "StaticData", VaryByParam = "none", Location = OutputCacheLocation.ServerAndClient)]
		[Route("Application/Training/Program/Skill/Focuses")]
		public JsonResult GetSkillsFocuses()
		{
			KeyValueListItem<int, string>[] model = null;
			try
			{
				var entity = _staticDataService.GetSkillsFocuses();
				model = entity.Select(o => new KeyValueListItem<int, string>(o.Id, o.Caption)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[OutputCache(CacheProfile = "StaticData", VaryByParam = "none", Location = OutputCacheLocation.ServerAndClient)]
		[Route("Application/Training/Program/In/Demand/Occupations")]
		public JsonResult GetInDemandOccupations()
		{
			KeyValueListItem<int, string>[] model = null;
			try
			{
				var entity = _staticDataService.GetInDemandOccupations();
				model = entity.Select(o => new KeyValueListItem<int, string>(o.Id, o.Caption)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[OutputCache(CacheProfile = "StaticData", VaryByParam = "none", Location = OutputCacheLocation.ServerAndClient)]
		[Route("Application/Training/Program/Training/Levels")]
		public JsonResult GetTrainingLevels()
		{
			KeyValueListItem<int, string>[] model = null;

			try
			{
				var entity = _staticDataService.GetTrainingLevels();
				model = entity.Select(o => new KeyValueListItem<int, string>(o.Id, o.Caption)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[OutputCache(CacheProfile = "StaticData", VaryByParam = "none", Location = OutputCacheLocation.ServerAndClient)]
		[Route("Application/Training/Program/Under/Represented/Groups")]
		public JsonResult GetUnderRepresentedGroups()
		{
			KeyValueListItem<int, string>[] model = null;

			try
			{
				var entity = _staticDataService.GetUnderRepresentedGroups();
				model = entity.Select(o => new KeyValueListItem<int, string>(o.Id, o.Caption)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[OutputCache(CacheProfile = "StaticData", VaryByParam = "none", Location = OutputCacheLocation.ServerAndClient)]
		[Route("Application/Training/Program/Expected/Qualifications")]
		public JsonResult GetExpectedQualifications()
		{
			KeyValueListItem<int, string>[] model = null;

			try
			{
				var entity = _staticDataService.GetExpectedQualifications();
				model = entity.Select(o => new KeyValueListItem<int, string>(o.Id, o.Caption)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region Training Provider
		/// <summary>
		/// Get all training provider types.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[OutputCache(CacheProfile = "StaticData", VaryByParam = "none", Location = OutputCacheLocation.ServerAndClient)]
		[Route("Application/Training/Provider/Types")]
		public JsonResult GetTrainingProviderTypes()
		{
			KeyValueParent<int, string, TrainingProviderPrivateSectorValidationTypes>[] model = null;

			try
			{
				var entity = _staticDataService.GetTrainingProviderTypes();
				model = entity.Select(o => new KeyValueParent<int, string, TrainingProviderPrivateSectorValidationTypes>(o.Id, o.Caption, o.PrivateSectorValidationType)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region Configuration
		/// <summary>
		/// Get an array of program types.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Admin/Grant/Program/Types")]
		[OutputCache(CacheProfile = "StaticData", VaryByParam = "none", Location = OutputCacheLocation.ServerAndClient)]
		public JsonResult GetProgramTypes()
		{
			IEnumerable<KeyValuePair<ProgramTypes, string>> result = null;

			try
			{
				result = _staticDataService.GetProgramTypes().Select(t => new KeyValuePair<ProgramTypes, string>(t.Id, t.Caption)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns an array of expense types.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Admin/Expense/Types")]
		[OutputCache(CacheProfile = "StaticData", VaryByParam = "none", Location = OutputCacheLocation.ServerAndClient)]
		public JsonResult GetExpenseTypes()
		{
			IEnumerable<KeyValuePair<ExpenseTypes, string>> list = null;
			try
			{
				list = _expenseTypeService.GetAll().Select(t => new KeyValuePair<ExpenseTypes, string>(t.Id, t.Caption)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(list, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns an array of reimbursment rates.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Admin/Rates")]
		[OutputCache(CacheProfile = "StaticData", VaryByParam = "none", Location = OutputCacheLocation.ServerAndClient)]
		public JsonResult GetRates()
		{
			IEnumerable<KeyValuePair<double, string>> list = null;
			try
			{
				list = _staticDataService.GetRateFormats().Select(t => new KeyValuePair<double, string>(t.Rate, t.Format)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(list, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Return an array of claim types.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Admin/Claim/Types")]
		[OutputCache(CacheProfile = "StaticData", VaryByParam = "none", Location = OutputCacheLocation.ServerAndClient)]
		public JsonResult GetClaimTypes()
		{
			IEnumerable<KeyValuePair<ClaimTypes, string>> result = null;
			try
			{
				result = _staticDataService.GetClaimTypes().Select(t => new KeyValuePair<ClaimTypes, string>(t.Id, t.Caption)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		#endregion
		#endregion
	}
}
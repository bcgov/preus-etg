using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Int.Models.Organizations;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Controllers
{

    [RouteArea("Int")]
	[RoutePrefix("Organization")]
	[AuthorizeAction(Privilege.TP1)]
	public class OrganizationController : BaseController
	{
		public static class TextMessages
		{
			public const string NoPermissions = "You don't have permissions to change status of this organization";
		}

		private readonly IOrganizationService _organizationService;
		private readonly IStaticDataService _staticDataService;
		private readonly INaIndustryClassificationSystemService _naIndustryClassificationSystemService;

		public OrganizationController(
			IControllerService controllerService,
			IOrganizationService organizationService,
			IStaticDataService staticDataService,
			INaIndustryClassificationSystemService naIndustryClassificationSystemService) : base(controllerService.Logger)
		{
			_organizationService = organizationService;
			_staticDataService = staticDataService;
			_naIndustryClassificationSystemService = naIndustryClassificationSystemService;
		}

		[HttpGet, Route("View")]
		public ActionResult OrganizationsView()
		{
			return View();
		}

		[HttpGet, Route("List")]
		public JsonResult GetOrganizationList()
		{
			var model = new OrganizationsViewModel();
			try
			{
				model = new OrganizationsViewModel(User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		[HttpGet, Route("Form/View")]
		public ActionResult OrganizationView()
		{
			return PartialView("_OrganizationDetails");
		}

		/// <summary>
		/// Get Organizations from inventory.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="quantity"></param>
		/// <param name="search"></param>
		/// <param name="riskFlag"></param>
		/// <returns></returns>
		[HttpGet, Route("Search/{page}/{quantity}")]
		public JsonResult GetOrganization(int page, int quantity, string search, bool? riskFlag)
		{
			var model = new BaseViewModel();
			try
			{
				var orgList = _organizationService.GetOrganizationList(page, quantity, search, isRisk: riskFlag);

				var result = new
				{
					RecordsFiltered = orgList.Items.Count(),
					RecordsTotal = orgList.Total,
					Data = orgList.Items.Select(o => new OrganizationListViewModel(o, _organizationService, _naIndustryClassificationSystemService)).ToArray()
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
		/// Get the organization types.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Types")]
		public JsonResult GetOrganizationTypes()
		{
			var model = new BaseViewModel();

			try
			{
				var result = _staticDataService.GetOrganizationTypes().Select(t => new KeyValuePair<int, string>(t.Id, t.Caption)).ToArray();
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get the Legal Structures data.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Legal/Structures")]
		public JsonResult GetLegalStructures()
		{
			try
			{
				var model = _staticDataService.GetLegalStructures().Select(x => new KeyValuePair<int, string>(x.Id, x.Caption)).ToArray();
				return Json(model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				var model = new BaseViewModel();
				HandleAngularException(ex, model);
				return Json(model, JsonRequestBehavior.AllowGet);
			}
		}

		/// <summary>
		/// Get the Provinces data.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Provinces")]
		public JsonResult GetProvinces()
		{
			try
			{
				var model = _staticDataService.GetProvinces().Select(x => new KeyValuePair<string, string>(x.Id, x.Name)).ToArray();
				return Json(model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				var model = new BaseViewModel();
				HandleAngularException(ex, model);
				return Json(model, JsonRequestBehavior.AllowGet);
			}
		}


		/// <summary>
		/// Get the NAICS data.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("NAICS/{level}/{parentId?}")]
		public JsonResult GetNAICS(int level, int? parentId)
		{
			try
			{
				var model = _naIndustryClassificationSystemService.GetNaIndustryClassificationSystemChildren(parentId ?? 0, level).Select(n => new KeyValuePair<int, string>(n.Id, $"{n.Code} | {n.Description}")).ToArray();
				return Json(model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				var model = new BaseViewModel();
				HandleAngularException(ex, model);
				return Json(model, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost, Route("Upsert")]
		[AuthorizeAction(Privilege.TP1, Privilege.TP2)]
		public JsonResult CreateOrganization(OrganizationListViewModel model)
		{
			try
			{
				var organization = new Organization
				{
					BCeIDGuid = Guid.NewGuid(),
					LegalName = model.LegalName,
					DoingBusinessAs = model.DoingBusinessAs,
					YearEstablished = model.YearEstablished,
					NumberOfEmployeesWorldwide = model.NumberOfEmployeesWorldwide,
					NumberOfEmployeesInBC = model.NumberOfEmployeesInBC,
					AnnualTrainingBudget = model.AnnualTrainingBudget,
					AnnualEmployeesTrained = model.AnnualEmployeesTrained,
					OrganizationTypeId = model.OrganizationTypeId,
					LegalStructureId = model.LegalStructureId,
					NaicsId = model.NaicsId,
					HeadOfficeAddressId = 0
				};

				if (organization.HeadOfficeAddress == null)
					organization.HeadOfficeAddress = new Address();

				if (model.HeadOfficeAddress != null)
				{					
					organization.HeadOfficeAddress.AddressLine1 = model.HeadOfficeAddress.AddressLine1;
					organization.HeadOfficeAddress.AddressLine2 = model.HeadOfficeAddress.AddressLine2;
					organization.HeadOfficeAddress.City = model.HeadOfficeAddress.City;
					organization.HeadOfficeAddress.RegionId = model.HeadOfficeAddress.RegionId;
					organization.HeadOfficeAddress.PostalCode = model.HeadOfficeAddress.PostalCode;
				}

				_organizationService.AddOrganizationProfile(organization);

			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		[HttpPut, Route("Upsert")]
		[AuthorizeAction(Privilege.TP1, Privilege.TP2)]
		public JsonResult UpdateOrganization(OrganizationListViewModel model)
		{
			try
			{
				var organization = _organizationService.Get(model.Id);
				organization.RowVersion = Convert.FromBase64String(model.RowVersion);

				if (!User.HasPrivilege(Privilege.TP2) || !User.HasPrivilege(Privilege.TP1))
				{
					throw new NotAuthorizedException(TextMessages.NoPermissions);
				}

				organization.LegalName = model.LegalName;
				organization.DoingBusinessAs = model.DoingBusinessAs;
				organization.YearEstablished = model.YearEstablished;
				organization.NumberOfEmployeesWorldwide = model.NumberOfEmployeesWorldwide;
				organization.NumberOfEmployeesInBC = model.NumberOfEmployeesInBC;
				organization.AnnualTrainingBudget = model.AnnualTrainingBudget;
				organization.AnnualEmployeesTrained = model.AnnualEmployeesTrained;
				organization.OrganizationTypeId = model.OrganizationTypeId;
				organization.LegalStructureId = model.LegalStructureId;
				organization.NaicsId = model.NaicsId;
				organization.HeadOfficeAddressId = model.HeadOfficeAddressId;

				organization.HeadOfficeAddress = new Address
				{
					AddressLine1 = model.HeadOfficeAddress.AddressLine1,
					AddressLine2 = model.HeadOfficeAddress.AddressLine2,
					City = model.HeadOfficeAddress.City,
					RegionId = model.HeadOfficeAddress.RegionId,
					PostalCode = model.HeadOfficeAddress.PostalCode
				};

				_organizationService.UpdateOrganization(organization);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}
	}
}

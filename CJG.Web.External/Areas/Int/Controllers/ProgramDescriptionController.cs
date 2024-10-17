using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Int.Models.ProgramDescriptions;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Controllers
{
    [RouteArea("Int")]
	public class ProgramDescriptionController : BaseController
	{
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IStaticDataService _staticDataService;
		private readonly INaIndustryClassificationSystemService _naIndustryClassificationSystemService;
		private readonly INationalOccupationalClassificationService _nationalOccupationalClassificationService;
		private readonly IVulnerableGroupService _vulnerableGroupService;
		private readonly IUnderRepresentedPopulationService _underRepresentedPopulationService;
		private readonly ICommunityService _communityService;
		private readonly IParticipantEmploymentStatusService _participantEmploymentStatusService;

		public ProgramDescriptionController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			IStaticDataService staticDataService,
			INaIndustryClassificationSystemService naIndustryClassificationSystemService,
			INationalOccupationalClassificationService nationalOccupationalClassificationService,
			IVulnerableGroupService vulnerableGroupService,
			IUnderRepresentedPopulationService underRepresentedPopulationService,
			ICommunityService communityService,
			IParticipantEmploymentStatusService participantEmploymentStatusService) : base(controllerService.Logger)
		{
			_grantApplicationService = grantApplicationService;
			_staticDataService = staticDataService;
			_naIndustryClassificationSystemService = naIndustryClassificationSystemService;
			_nationalOccupationalClassificationService = nationalOccupationalClassificationService;
			_vulnerableGroupService = vulnerableGroupService;
			_underRepresentedPopulationService = underRepresentedPopulationService;
			_communityService = communityService;
			_participantEmploymentStatusService = participantEmploymentStatusService;
		}

		[HttpGet]
		[Route("Application/Program/Description/{grantApplicationId}")]
		public JsonResult GetProgramDescription(int grantApplicationId)
		{
			var model = new ProgramDescriptionViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new ProgramDescriptionViewModel(grantApplication.ProgramDescription, _naIndustryClassificationSystemService, _nationalOccupationalClassificationService);
			}
			catch (Exception ex)
			{
				 HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Application/Program/Description/")]
		public JsonResult UpdateProgramDescription(ProgramDescriptionViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var grantApplication = _grantApplicationService.Get(model.Id);
					grantApplication.ProgramDescription.RowVersion = Convert.FromBase64String(model.RowVersion);
					model.MapToEntity(grantApplication.ProgramDescription, _vulnerableGroupService, _underRepresentedPopulationService, _communityService);
					_grantApplicationService.Update(grantApplication, ApplicationWorkflowTrigger.EditProgramDescription);
					model = new ProgramDescriptionViewModel(grantApplication.ProgramDescription, _naIndustryClassificationSystemService, _nationalOccupationalClassificationService);
				}
				else
				{
					model.ValidationErrors = GetClientErrors();
					AddGenericError(model, ModelState.GetErrorMessages("<br />"));
				}
			}
			catch (Exception e)
			{
				HandleAngularException(e, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Route("Application/Program/Description/Applicant/Organization/Types")]
		public JsonResult GetApplicantOrganizationTypes()
		{
			try
			{
				var model = _staticDataService.GetApplicantOrganizationTypes().Select(x => new KeyValuePair<int, string>(x.Id, x.Caption)).ToArray();
				return Json(model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				var model = new BaseViewModel();
				HandleAngularException(ex, model);
				return Json(model, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		[Route("Application/Program/Description/Participant/Employment/Statuses")]
		public JsonResult GetParticipantEmploymentStatuses()
		{
			try
			{
				var model = _staticDataService.GetParticipantEmploymentStatuses().Select(x => new KeyValuePair<int, string>(x.Id, x.Caption)).ToArray();
				return Json(model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				var model = new BaseViewModel();
				HandleAngularException(ex, model);
				return Json(model, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		[Route("Application/Program/Description/Under/Represented/Populations")]
		public JsonResult GetUnderRepresentedPopulations()
		{
			try
			{
				var model = _staticDataService.GetUnderRepresentedPopulations().Select(x => new KeyValuePair<int, string>(x.Id, x.Caption)).ToArray();
				return Json(model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				var model = new BaseViewModel();
				HandleAngularException(ex, model);
				return Json(model, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		[Route("Application/Program/Description/Vulnerable/Groups")]
		public JsonResult GetVulnerableGroups()
		{
			try
			{
				var model = _staticDataService.GetVulnerableGroups().Select(x => new KeyValuePair<int, string>(x.Id, x.Caption)).ToArray();
				return Json(model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				var model = new BaseViewModel();
				HandleAngularException(ex, model);
				return Json(model, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		[Route("Application/Program/Description/NAICS/{level}/{parentId?}")]
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

		[HttpGet]
		[Route("Application/Program/Description/NOCs/{level}/{parentId?}")]
		public JsonResult GetNOCs(int level, int? parentId)
		{
			try
			{
				var model = _nationalOccupationalClassificationService.GetNationalOccupationalClassificationChildren(parentId ?? 0, level)
					.Select(n => new KeyValuePair<int, string>(n.Id, $"{n.Code} | {n.Description} ({n.NOCVersion})"))
					.ToArray();
				return Json(model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				var model = new BaseViewModel();
				HandleAngularException(ex, model);
				return Json(model, JsonRequestBehavior.AllowGet);
			}
		}
	}
}
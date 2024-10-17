using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;
using System;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Ext.Controllers
{
	/// <summary>
	/// ProgramDescriptionController class, provides endpoints for adding and editing program descriptions.
	/// </summary>
	[RouteArea("Ext")]
	[ExternalFilter]
	public class ProgramDescriptionController : BaseController
	{
		#region Variables
		private readonly IStaticDataService _staticDataService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly INaIndustryClassificationSystemService _naIndustryClassificationSystemService;
		private readonly INationalOccupationalClassificationService _nationalOccupationalClassificationService;
		private readonly IVulnerableGroupService _vulnerableGroupService;
		private readonly IUnderRepresentedPopulationService _underRepresentedPopulationService;
		private readonly ICommunityService _communityService;
		private readonly IParticipantEmploymentStatusService _participantEmploymentStatusService;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a ProgramDescriptionController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantApplicationService"></param>
		/// <param name="naIndustryClassificationSystemService"></param>
		/// <param name="nationalOccupationalClassificationService"></param>
		/// <param name="vulnerableGroupService"></param>
		/// <param name="underRepresentedPopulationService"></param>
		/// <param name="communityService"></param>
		public ProgramDescriptionController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			INaIndustryClassificationSystemService naIndustryClassificationSystemService,
			INationalOccupationalClassificationService nationalOccupationalClassificationService,
			IVulnerableGroupService vulnerableGroupService,
			IUnderRepresentedPopulationService underRepresentedPopulationService,
			ICommunityService communityService,
			IParticipantEmploymentStatusService participantEmploymentStatusService) : base(controllerService.Logger)
		{
			_staticDataService = controllerService.StaticDataService;
			_grantApplicationService = grantApplicationService;
			_naIndustryClassificationSystemService = naIndustryClassificationSystemService;
			_nationalOccupationalClassificationService = nationalOccupationalClassificationService;
			_vulnerableGroupService = vulnerableGroupService;
			_underRepresentedPopulationService = underRepresentedPopulationService;
			_communityService = communityService;
			_participantEmploymentStatusService = participantEmploymentStatusService;
		}
		#endregion

		#region Endpoints
		/// <summary>
		/// Display the Program Description View.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet, Route("Application/Program/Description/View/{grantApplicationId}")]
		public ActionResult ProgramDescriptionView(int grantApplicationId)
		{
			var grantApplication = _grantApplicationService.Get(grantApplicationId);

			if (!User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditApplication))
			{
				throw new NotAuthorizedException($"User does not have permission to edit the application id: {grantApplicationId}");
			}

			ViewBag.GrantApplicationId = grantApplicationId;
			return View(grantApplication);
		}

		/// <summary>
		/// Get the data for the program description view.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet, Route("Application/Program/Description/{grantApplicationId}")]
		public JsonResult GetProgramDescription(int grantApplicationId)
		{
			var model = new Models.ProgramDescriptions.ProgramDescriptionExtViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);

				if (!User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditApplication))
				{
					throw new NotAuthorizedException($"User does not have permission to edit the application id: {grantApplicationId}");
				}

				var programDescription = grantApplication.ProgramDescription ?? new ProgramDescription(grantApplication);
				model = new Models.ProgramDescriptions.ProgramDescriptionExtViewModel(programDescription, _staticDataService, _naIndustryClassificationSystemService, _nationalOccupationalClassificationService, _communityService);
			}
			catch (Exception e)
			{
				HandleAngularException(e, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get the data for the program description view.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Application/Program/Description")]
		public JsonResult UpdateProgramDescription(Models.ProgramDescriptions.ProgramDescriptionExtViewModel model)
		{
			try
			{
				if (model.ApplicantOrganizationTypeId != 21)
				{
					ModelState.Remove("ApplicantOrganizationTypeInfo");
				}

				if (ModelState.IsValid)
				{
					var grantApplication = _grantApplicationService.Get(model.Id);
					var programDescription = grantApplication.ProgramDescription ?? new ProgramDescription(grantApplication);
					model.MapToEntity(programDescription, _vulnerableGroupService, _underRepresentedPopulationService, _communityService, _participantEmploymentStatusService);
					grantApplication.TrainingCost.EstimatedParticipants = model.NumberOfParticipants.Value;
					programDescription.DescriptionState = ProgramDescriptionStates.Complete;

					var skillsTrainingMaxCostPerParticipant = grantApplication.GrantOpening.GrantStream.ProgramConfiguration.SkillsTrainingMaxEstimatedParticipantCosts;
					var ESSMaxParticipantCostPerParticipant = grantApplication.GrantOpening.GrantStream.ProgramConfiguration.ESSMaxEstimatedParticipantCost;

					grantApplication.TrainingCost.RecalculateEstimatedCostsFor(grantApplication.TrainingCost.EstimatedParticipants);

					_grantApplicationService.Update(grantApplication);

					model.RedirectURL = Url.ActionUrl(nameof(ApplicationController.ApplicationOverviewView), typeof(ApplicationController), new { grantApplicationId = grantApplication.Id });
				}
				else
				{
					HandleModelStateValidation(model);
				}
			}
			catch (Exception e)
			{
				HandleAngularException(e, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Check if the max ESS/SkillTraining per participant is exceeded with new participants.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Program/Description/Training/Max/Cost/{id}/{participants}")]
		public JsonResult GetMaxCosts(int id, int participants)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(id);
				var skillsTrainingMaxCostPerParticipant = grantApplication.GrantOpening.GrantStream.ProgramConfiguration.SkillsTrainingMaxEstimatedParticipantCosts;
				var ESSMaxParticipantCostPerParticipant = grantApplication.GrantOpening.GrantStream.ProgramConfiguration.ESSMaxEstimatedParticipantCost;

				if (grantApplication.TrainingCost.HasExceededServiceLimit(ServiceTypes.SkillsTraining, participants, skillsTrainingMaxCostPerParticipant)
					|| grantApplication.TrainingCost.HasExceededServiceLimit(ServiceTypes.EmploymentServicesAndSupports, participants, ESSMaxParticipantCostPerParticipant))
				{
					return Json(new { CostExceeded = true, SkillsTrainingMaxEstimatedParticipantCosts = skillsTrainingMaxCostPerParticipant, ESSMaxEstimatedParticipantCost = ESSMaxParticipantCostPerParticipant }, JsonRequestBehavior.AllowGet);
				}

				return Json(new { CostExceeded = false }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				var model = new BaseViewModel();
				HandleAngularException(ex, model);
				return Json(model, JsonRequestBehavior.AllowGet);
			}
		}
		#endregion
	}
}
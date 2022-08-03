using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Int.Models;
using CJG.Web.External.Areas.Int.Models.GrantStreams;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;

namespace CJG.Web.External.Areas.Int.Controllers
{
	/// <summary>
	/// <paramtyperef name="Applicantcontroller"/> class. Provides endpoints to manage the applicant block partial form.
	/// </summary>
	[RouteArea("Int")]
	[RoutePrefix("Application/Applicant")]
	[Authorize(Roles = "Assessor, System Administrator, Director, Financial Clerk")]
	public class ApplicantController : BaseController
	{
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IGrantStreamService _grantStreamService;
		private readonly IStaticDataService _staticDataService;
		private readonly INaIndustryClassificationSystemService _naIndustryClassificationSystemService;
		private readonly IApplicationAddressService _applicationAddressService;
		private readonly ICommunityService _communityService;

		#region Constructors

		/// <summary>
		/// Creates a new instance of the ApplicantController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantApplicationService"></param>
		/// <param name="grantStreamService"></param>
		/// <param name="staticDataService"></param>
		/// <param name="naIndustryClassificationSystemService"></param>
		/// <param name="applicationAddressService"></param>
		/// <param name="communityService"></param>
		public ApplicantController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			IGrantStreamService grantStreamService, 
			IStaticDataService staticDataService,
			INaIndustryClassificationSystemService naIndustryClassificationSystemService,
			IApplicationAddressService applicationAddressService,
			ICommunityService communityService) : base(controllerService.Logger)
		{
			_grantApplicationService = grantApplicationService;
			_grantStreamService = grantStreamService;
			_staticDataService = staticDataService;
			_naIndustryClassificationSystemService = naIndustryClassificationSystemService;
			_applicationAddressService = applicationAddressService;
			_communityService = communityService;
		}
		#endregion

		#region Endpoints
		/// <summary>
		/// Get the data for the specified grant application applicant for the partial view.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns>JsonResult</returns>
		[HttpGet]
		[Route("{grantApplicationId}")]
		public JsonResult GetApplicant(int grantApplicationId)
		{
			var model = new ApplicantViewModel();

			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new ApplicantViewModel(grantApplication, _naIndustryClassificationSystemService, _grantStreamService);
			} catch (Exception ex) {
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get all communities from the CommunityService.
		/// </summary>
		/// <returns>JsonResult</returns>
		[HttpGet]
		[Route("Communities")]
		public JsonResult GetCommunities()
		{
			IEnumerable<KeyValuePair<int, string>> communities = null;
			try
			{
				communities = _communityService.GetAll().Where(t => t.IsActive).Select(x => new KeyValuePair<int, string>(x.Id, x.Caption)).ToArray();

			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(communities, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get an array of active NAICS.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="parentId"></param>
		/// <returns>JsonResult</returns>
		[Route("Naics/{level}/{parentId?}")]
		public JsonResult GetNAICS(int level, int? parentId)
		{
			IEnumerable<dynamic> naicsList = null;

			try {
				naicsList = _naIndustryClassificationSystemService.GetNaIndustryClassificationSystemChildren(parentId ?? 0, level).Select(n => new {
					Key = n.Id,
					Value = n.Description,
					Code = n.Code
				}).ToArray();
			} catch (Exception ex) {
				HandleAngularException(ex);
			}

			return Json(naicsList, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get an array of Eligibility Questions and their Answers for an application.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns>JsonResult</returns>
		[Route("EligibilityQuestions/{grantApplicationId:int}")]
		public JsonResult GetApplicationEligibilityQuestions(int grantApplicationId)
		{
			IEnumerable<dynamic> questionList = null;
			var streamEligibilityQuestions = new List<GrantStreamQuestionViewModel>();
			try {
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				streamEligibilityQuestions = _grantStreamService.GetGrantStreamQuestions(grantApplication.GrantOpening.GrantStreamId)
					.Where(l => l.IsActive)
					.Select(n => new GrantStreamQuestionViewModel(n))
					.ToList();

				questionList = streamEligibilityQuestions.Select(n => new {
					Key = n.Id,
					Value = n.EligibilityAnswer.HasValue ? n.EligibilityAnswer.Value : false,
					Code = n.EligibilityQuestion
				}).ToArray();
			} catch (Exception ex) {
				HandleAngularException(ex);
			}

			return Json(streamEligibilityQuestions.ToList(), JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Updates the specified grant application applicant information in the datasource.
		/// </summary>
		/// <param name="model"></param>
		/// <returns>JsonResult</returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("")]
		public JsonResult UpdateApplicant(ApplicantViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var grantApplication = _grantApplicationService.Get(model.Id);
					model.MapTo(grantApplication, _staticDataService, _applicationAddressService);

					_grantApplicationService.Update(grantApplication, ApplicationWorkflowTrigger.EditApplicant);

					model = new ApplicantViewModel(grantApplication, _naIndustryClassificationSystemService, _grantStreamService);
				}
				else
				{
					HandleModelStateValidation(model, ModelState.GetErrorMessages("<br />"));
				}
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
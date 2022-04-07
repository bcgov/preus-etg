using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Int.Models;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CJG.Web.External.Areas.Int.Models.TrainingPrograms;
using CJG.Web.External.Areas.Int.Models.TrainingProviders;

namespace CJG.Web.External.Areas.Int.Controllers
{
	/// <summary>
	/// TrainingProgramController class, controller provides a way to update training programs.
	/// </summary>
	[RouteArea("Int")]
	[Authorize(Roles = "Assessor, System Administrator, Director, Financial Clerk")]
	public class TrainingProgramController : BaseController
	{
		#region Variables
		private readonly IStaticDataService _staticDataService;
		private readonly ITrainingProgramService _trainingProgramService;
		private readonly ICipsCodesService _cipsCodesService;
		#endregion

		#region Constructors
		/// <summary>
		/// Create a new instance of a TrainingProgramController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="trainingProgramService"></param>
		public TrainingProgramController(
			IControllerService controllerService,
			ITrainingProgramService trainingProgramService, ICipsCodesService cipsCodesService
		   ) : base(controllerService.Logger)
		{
			_staticDataService = controllerService.StaticDataService;
			_trainingProgramService = trainingProgramService;
			_cipsCodesService = cipsCodesService;

		}
		#endregion

		#region Endpoints
		/// <summary>
		/// Get the training program detail for the specified 'id'.
		/// </summary>
		/// <param name="trainingProgramId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Training/Program/{trainingProgramId}")]
		public JsonResult GetTrainingProgram(int trainingProgramId)
		{
			var model = new Models.TrainingPrograms.TrainingProgramViewModel();

			try
			{
				var trainingProgram = _trainingProgramService.Get(trainingProgramId);
				model = new Models.TrainingPrograms.TrainingProgramViewModel(trainingProgram, _cipsCodesService);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get the specified training program extra information. Mostly HTMl content we need to display, but not edit/post.
		/// </summary>
		/// <param name="trainingProgramId"></param>
		/// <returns></returns>
		[HttpGet, Route("Application/Training/Program/ExtraInfo/{trainingProgramId}")]
		public JsonResult GetTrainingProgramExtraInfo(int trainingProgramId)
		{
			var model = new TrainingProgramExtraInfoViewModel();
			try
			{
				var trainingProgram = _trainingProgramService.Get(trainingProgramId);
				model = new TrainingProgramExtraInfoViewModel(trainingProgram);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Gets the child CIPS record
		/// </summary>
		/// <param name="level"></param>
		/// <param name="parentId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Training/Program/CipsCodes/{level}/{parentId?}")]
		public JsonResult GetCipsCode(int level, int? parentId)
		{
			try
			{
				var model = _cipsCodesService.GetCipsCodeChildren(parentId ?? 0, level).Select(n => new KeyValuePair<int, string>(n.Id, $"{n.Code} | {n.Description}")).ToArray();
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
		/// Update the specified training program in the datasource.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Application/Training/Program")]
		public JsonResult UpdateTrainingProgram(Models.TrainingPrograms.TrainingProgramViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var trainingProgram = _trainingProgramService.Get(model.Id);
					model.MapTo(trainingProgram, _staticDataService);
					_trainingProgramService.Update(trainingProgram);

					model = new Models.TrainingPrograms.TrainingProgramViewModel(trainingProgram, _cipsCodesService);
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

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}

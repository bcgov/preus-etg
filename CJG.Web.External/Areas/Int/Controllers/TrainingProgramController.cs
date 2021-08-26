using CJG.Core.Interfaces.Service;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using System;
using System.Web.Mvc;

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
		#endregion

		#region Constructors
		/// <summary>
		/// Create a new instance of a TrainingProgramController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="trainingProgramService"></param>
		public TrainingProgramController(
			IControllerService controllerService,
			ITrainingProgramService trainingProgramService
		   ) : base(controllerService.Logger)
		{
			_staticDataService = controllerService.StaticDataService;
			_trainingProgramService = trainingProgramService;

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
				model = new Models.TrainingPrograms.TrainingProgramViewModel(trainingProgram);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
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

					model = new Models.TrainingPrograms.TrainingProgramViewModel(trainingProgram);
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

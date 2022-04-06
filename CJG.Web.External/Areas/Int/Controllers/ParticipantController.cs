using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Int.Models;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Int.Controllers
{
	/// <summary>
	/// <paramtyperef name="ParticipantController"/> class. Provides endpoints to manage the Participant partial form.
	/// </summary>
	[RouteArea("Int")]
	[RoutePrefix("Application")]
	public class ParticipantController : BaseController
	{
		#region Variables
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IUserService _userService;
		private readonly IParticipantService _participantService;
		private readonly INationalOccupationalClassificationService _nationalOccupationalClassificationService;
		private readonly IAttachmentService _attachmentService;
		#endregion

		#region Constructors
		public ParticipantController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			IUserService userService,
			IParticipantService participantService,
			INationalOccupationalClassificationService nationalOccupationalClassificationService,
			IAttachmentService attachmentService) : base(controllerService.Logger)
		{
			_grantApplicationService = grantApplicationService;
			_userService = userService;
			_participantService = participantService;
			_nationalOccupationalClassificationService = nationalOccupationalClassificationService;
			_attachmentService = attachmentService;
		}
		#endregion

		#region Endpoints
		/// <summary>
		/// Get participants data for the specified Grant Application.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Participants/{grantApplicationId}")]
		public JsonResult GetParticipants(int grantApplicationId)
		{
			var viewModel = new ParticipantListViewModel();

			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				viewModel = new ParticipantListViewModel(grantApplication, _participantService);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}

			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get the participant consent attachment.
		/// </summary>
		/// <param name="participantId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Participant/Consent/{participantId}")]
		public ActionResult DownloadAttachment(int participantId)
		{
			ParticipantForm participantForm = _participantService.Get(participantId);

			if (participantForm.ParticipantConsentAttachment != null)
			{
				var attachment = _attachmentService.Get(participantForm.ParticipantConsentAttachment.Id);

				return File(attachment.AttachmentData, System.Net.Mime.MediaTypeNames.Application.Octet, $"{attachment.FileName}{attachment.FileExtension}");
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Returns a view to display the participant information.
		/// </summary>
		/// <param name="participantId"></param>
		/// <returns></returns>
		[HttpGet]
		[AuthorizeAction(Privilege.IA2)]
		[Route("Participant/Info/View/{participantId}")]
		public ActionResult ParticipantInformationView(int participantId)
		{
			ParticipantInfoViewModel model;
			ParticipantForm participantForm = _participantService.Get(participantId);

			if (participantForm != null)
			{
				model = new ParticipantInfoViewModel(participantForm, _nationalOccupationalClassificationService, _userService, _participantService);
			}
			else
			{
				model = new ParticipantInfoViewModel();
			}

#if Training || Support
			 model.ContactInfo.SIN = string.Concat(model.ContactInfo.SIN?.Substring(0, 1), "** *** ***");
#endif

			// Mask the social insurance number for anyone without privilege IA3
			if (!User.HasPrivilege(Privilege.IA3))
			{
				model.ContactInfo.SIN = string.Concat(model.ContactInfo.SIN?.Substring(0, 1), "** *** ***");
			}

			return View(model);
		}


		/// <summary>
		/// Returns a view to display the participant information.
		/// </summary>
		/// <param name="participantId"></param>
		/// <returns></returns>
		[HttpGet]
		[AuthorizeAction(Privilege.IA2)]
		[Route("Participant/Info/View/{participantId}/TrainingHistory")]
		public JsonResult ParticipantTrainingHistory(int participantId)
		{
			ViewBag.ParticipantId = participantId;

			ParticipantInfoViewModel model;
			ParticipantForm participantForm = _participantService.Get(participantId);

			if (participantForm != null)
			{
				model = new ParticipantInfoViewModel(participantForm, _nationalOccupationalClassificationService, _userService, _participantService);
			}
			else
			{
				model = new ParticipantInfoViewModel();
			}


#if Training || Support
             model.ContactInfo.SIN = string.Concat(model.ContactInfo.SIN?.Substring(0, 1), "** *** ***");
#endif

			// Mask the social insurance number for anyone without privilege IA3
			if (!User.HasPrivilege(Privilege.IA3))
			{
				model.ContactInfo.SIN = string.Concat(model.ContactInfo.SIN?.Substring(0, 1), "** *** ***");
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Participants")]
		public JsonResult ApproveDenyParticipants(ParticipantListViewModel model)
		{
			//set the Approved property
			try
			{
				if (ModelState.IsValid)
				{
					int grantApplicationId = model.Id;

					var participantsApproved = model.ParticipantInfo.Where(w => w.ParticipantId != null).ToDictionary(d => d.ParticipantId, d => d.Approved);

					_participantService.ApproveDenyParticipants(grantApplicationId, participantsApproved);

					var grantApplication = _grantApplicationService.Get(grantApplicationId);

					model = new ParticipantListViewModel(grantApplication, _participantService);
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
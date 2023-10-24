using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Int.Models;
using CJG.Web.External.Areas.Int.Models.Participants;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Controllers
{
    /// <summary>
    /// IntakeController class, provides endpoints to select and search for grant applications.
    /// </summary>
    [RouteArea("Int")]
	[Authorize(Roles = "Assessor, System Administrator, Director, Financial Clerk")]
	public class ParticipantsController : BaseController
	{
		private readonly IParticipantsService _participantsService;
		private readonly IParticipantService _participantService;

		public ParticipantsController(
			IControllerService controllerService,
			IParticipantsService participantsService,
			IParticipantService participantService
		   ) : base(controllerService.Logger)
		{
			_participantsService = participantsService;
			_participantService = participantService;
		}

		[HttpGet]
		[Route("Participants/View")]
		public ActionResult ParticipantsView()
		{
			return View();
		}

		[HttpPost]
		[ValidateRequestHeader]
		[Route("Participants/Search")]
		public JsonResult GetParticipants(ParticipantsFilterViewModel filter)
		{
			var model = new PageList<ParticipantApplicationModel>();
			try
			{
				int.TryParse(Request.QueryString["page"], out int pageNumber);
				int.TryParse(Request.QueryString["quantity"], out int quantityNumber);

				var query = filter.GetFilter();
				var participants = _participantsService.GetParticipants(pageNumber, quantityNumber, query);

				model.Page = participants.Page;
				model.Quantity = participants.Quantity;
				model.Total = participants.Total;

				var hasIA3Privilege = User.HasPrivilege(Privilege.IA3);

				model.Items = participants.Items.Select(pf => new ParticipantApplicationModel
				{
					SIN = MaskSIN(pf.SIN, hasIA3Privilege),  // Hide SIN in page
					ParticipantFormId = pf.ParticipantFormId,
					ParticipantLastName = pf.ParticipantLastName,
					ParticipantMiddleName = pf.ParticipantMiddleName,
					ParticipantFirstName = pf.ParticipantFirstName,
					LastApplicationDateTime = pf.LastApplicationDateTime,
					FileNumber = pf.FileNumber,
					CourseName = pf.CourseName,
					EmployerName = pf.EmployerName,
                    PaidToDate = pf.PaidToDate
				});
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, filter);
				return Json(filter);
			}

			return Json(model);
		}

		[HttpGet, Route("Participants/Participant/{participantFormId}")]
		public ActionResult ParticipantHistoryView(int? participantFormId)
		{
			ViewBag.ParticipantFormId = participantFormId;
			return View();
		}

		[HttpGet, Route("Participants/Participant/Data/{participantFormId}")]
		public JsonResult GetParticipantData(int participantFormId)
		{
			var model = new ParticipantApplicationModel();
			try
			{
				var participantForm = _participantService.Get(participantFormId);
				var hasIA3Privilege = User.HasPrivilege(Privilege.IA3);
				model = new ParticipantApplicationModel
				{
					ParticipantFormId = participantFormId,
					ParticipantFirstName = participantForm.FirstName,
					ParticipantMiddleName = participantForm.MiddleName,
					ParticipantLastName = participantForm.LastName,
					SIN = MaskSIN(participantForm.SIN, hasIA3Privilege)
				};
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		[HttpGet, Route("Participants/Participant/Search/{participantFormId}/{page}/{quantity}")]
		[ValidateRequestHeader]
		[AuthorizeAction(Privilege.IA1)]
		public JsonResult GetParticipantFileHistory(int participantFormId, int page, int quantity, string search, string sortby, bool sortDesc)
		{
			var model = new BaseViewModel();
			try
			{
				var pif = _participantService.Get(participantFormId);

				var participantForms = _participantService.GetParticipantFormsBySIN(pif.SIN);

				if (!string.IsNullOrWhiteSpace(search))
				{
					search = search.ToLowerInvariant();
					participantForms = participantForms
						.Where(x => x.GrantApplication.FileNumber != null && x.GrantApplication.FileNumber.ToLowerInvariant().Contains(search)
						            || x.GrantApplication.TrainingPrograms.Any(tp => tp.CourseTitle.ToLowerInvariant().Contains(search))
						            || x.GrantApplication.TrainingPrograms.Any(tp => tp.TrainingProviders.Any(tpp => tpp.Name.ToLowerInvariant().Contains(search))));
				}

				var trainingHistory = new List<ParticipantTrainingHistory>();

				foreach (var p in participantForms)
				{
					var reimbursement = p.ParticipantCosts.Sum(s => s.AssessedReimbursement);
					var amtPaid = p.ParticipantCosts
						.Where(w => w.ClaimEligibleCost.Claim.ClaimState == ClaimState.ClaimApproved ||
						            w.ClaimEligibleCost.Claim.ClaimState == ClaimState.PaymentRequested ||
						            w.ClaimEligibleCost.Claim.ClaimState == ClaimState.ClaimPaid)
						.Sum(s => s.AssessedReimbursement);

					foreach (var t in p.GrantApplication.TrainingPrograms)
					{
						if (t.GrantApplication.FileNumber == null || t.GrantApplication.ApplicationStateInternal == ApplicationStateInternal.Draft)
							continue;

						trainingHistory.Add(new ParticipantTrainingHistory(t, reimbursement, amtPaid, p));
					}
				}

				// Sort
				var prop = typeof(ParticipantTrainingHistory).GetProperty(sortby);
				if (prop != null)
					trainingHistory = sortDesc
						? trainingHistory.OrderByDescending(o => prop.GetValue(o, null)).ToList()
						: trainingHistory.OrderBy(o => prop.GetValue(o, null)).ToList();

				// Filter
				var filtered = trainingHistory
					.Skip((page - 1) * quantity)
					.Take(quantity)
					.ToList();

				var result = new
				{
					RecordsFiltered = filtered.Count,
					RecordsTotal = trainingHistory.Count,
					Data = filtered.ToArray()
				};

				return Json(result, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		private string MaskSIN(string sin, bool hasIA3Privilege)
		{
#if Training || Support
				sin = string.Concat(sin?.Substring(0, 1), "** *** ***");
#endif

			// Mask the social insurance number for anyone without privilege IA3
			if (!hasIA3Privilege)
				return string.Concat(sin?.Substring(0, 1), "** *** ***");

			return sin;
		}
	}
}
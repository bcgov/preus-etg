using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CJG.Application.Business.Models;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Areas.Ext.Models.Applications;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;

namespace CJG.Web.External.Areas.Ext.Controllers
{
    [RouteArea("Ext")]
	public class AlternateContactController : BaseController
	{
		private readonly IStaticDataService _staticDataService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IUserService _userService;
		private readonly IApplicationAddressService _applicationAddressService;

		public AlternateContactController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			IApplicationAddressService applicationAddressService
		   ) : base(controllerService.Logger)
		{
			_userService = controllerService.UserService;
			_staticDataService = controllerService.StaticDataService;
			_grantApplicationService = grantApplicationService;
			_applicationAddressService = applicationAddressService;
		}

		[HttpGet]
		[Route("Application/AlternateContact/{grantApplicationId}")]
		public ActionResult AlternateContactView(int grantApplicationId)
		{
			ViewBag.GrantApplicationId = grantApplicationId;
			var grantApplication = _grantApplicationService.Get(grantApplicationId);
			return View(SidebarViewModelFactory.Create(grantApplication, ControllerContext));
		}

		/// <summary>
		/// Get the applicant contact information for the application details view.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/AlternateContact/Data/{id}")]
		public JsonResult GetAlternateContact(int id)
		{
			var model = new AlternateContactViewModel();

			try
			{
				var grantApplication = _grantApplicationService.Get(id);
				model = new AlternateContactViewModel(grantApplication);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get the applicant contact information for the application details view.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/ApplicantContact/{id}")]
		public JsonResult GetApplicantContact(int id)
		{
			var model = new AlternateContactViewModel();

			try
			{
				var grantApplication = _grantApplicationService.Get(id);
				model = new AlternateContactViewModel(grantApplication, _userService, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Update the alternate contact information in the datasource.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Application/AlternateContact/")]
		public JsonResult UpdateAlternateContact(AlternateContactViewModel model)
		{
			// Bypass the validation on the PhoneNumber field
			ModelState.Remove("PhoneNumberViewModel.Phone");
			try
			{
				if (ModelState.IsValid)
				{
					var grantApplication = _grantApplicationService.Get(model.Id);
					var contactModel = new AlternateContactModel
					{
						AlternateFirstName = model.AlternateFirstName,
						AlternateLastName = model.AlternateLastName,
						AlternateEmail = model.AlternateEmail,
						AlternateJobTitle = model.AlternateJobTitle,
						AlternatePhoneNumber = model.PhoneNumberViewModel.Phone,
						AlternatePhoneExtension = model.PhoneNumberViewModel.PhoneExtension
					};

					_grantApplicationService.ChangeAlternateContact(grantApplication, contactModel);

					model = new AlternateContactViewModel(grantApplication);

					this.SetAlert("Alternate contact updated.", AlertType.Success, true);
					model.RedirectURL = @Url.Action("AlternateContactView", "AlternateContact", new {grantApplicationId = model.Id});
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

		/// <summary>
		/// Update the applicant contact information in the datasource.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Application/ApplicantContact/")]
		public JsonResult UpdateApplicantContact(AlternateContactViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var grantApplication = _grantApplicationService.Get(model.Id);
					var originalMailingAddress = grantApplication.ApplicantMailingAddress;
					model.MapToGrantApplication(grantApplication, _staticDataService, _applicationAddressService);
					if (grantApplication.ApplicantMailingAddressId == 0 && originalMailingAddress?.Id != 0 && originalMailingAddress?.Id != grantApplication.ApplicantPhysicalAddressId)
					{
						// Delete the orphaned address.
						_applicationAddressService.RemoveAddressIfNotUsed(grantApplication.ApplicantMailingAddress);
					}
					_grantApplicationService.Update(grantApplication, ApplicationWorkflowTrigger.EditApplicantContact);
					model = new AlternateContactViewModel(grantApplication, _userService, User);
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

		/// <summary>
		/// Get all of the applicants in the organization for the specified grant application.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Applicant/Contacts/{grantApplicationId}")]
		public JsonResult GetApplicantsForOrganization(int grantApplicationId)
		{
			var model = new List<KeyValuePair<int, string>>();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = _grantApplicationService.GetAvailableApplicationContacts(grantApplication).Select(a => new KeyValuePair<int, string>(a.Id, $"{a.GetUserFullName()} | {a.JobTitle} | {a.EmailAddress} | {a.PhoneNumber}")).ToList();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Change the applicant who owns the specified grant application.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Application/Applicant/Contact/Change")]
		public JsonResult ChangeApplicant(Models.Applications.ChangeAlternateContactViewModel model)
		{
			var result = new AlternateContactViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(model.Id);
				grantApplication.RowVersion = Convert.FromBase64String(model.RowVersion);
				_grantApplicationService.ChangeApplicationAdministrator(grantApplication, model.ApplicantContactId);

				result = new AlternateContactViewModel(grantApplication, _userService, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, result);
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}
	}
}
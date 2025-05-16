using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Part.Models;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using Newtonsoft.Json;

namespace CJG.Web.External.Areas.Part.Controllers
{
    /// <summary>
    /// InformationController class, provides the controller for external participant enrollment.
    /// </summary>
    [ParticipantFilter]
	[RouteArea("Part")]
	[RoutePrefix("Information")]
	public class InformationController : BaseController
	{
		private const int EI_BENEFIT_NONE_OF_THE_ABOVE = 6;

		private readonly IStaticDataService _staticDataService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IParticipantService _participantService;
		private readonly INationalOccupationalClassificationService _nationalOccupationalClassificationService;
		private readonly IReCaptchaService _reCaptchaService;
		private readonly IGrantProgramService _grantProgramService;
		private readonly IAttachmentService _attachmentService;
		private readonly IParticipantInvitationService _participantInvitationService;

		/// <summary>
		/// Creates a new instance of a InformationController object, and initializes it with the specified arguments.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantApplicationService"></param>
		/// <param name="grantParticipantService"></param>
		/// <param name="nationalOccupationalClassificationService"></param>
		/// <param name="recaptchaService"></param>
		/// <param name="grantProgramService"></param>
		/// <param name="attachmentService"></param>
		/// <param name="participantInvitationService"></param>
		public InformationController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			IParticipantService grantParticipantService,
			INationalOccupationalClassificationService nationalOccupationalClassificationService,
			IReCaptchaService recaptchaService,
			IGrantProgramService grantProgramService,
			IAttachmentService attachmentService,
			IParticipantInvitationService participantInvitationService) : base(controllerService.Logger)
		{
			_staticDataService = controllerService.StaticDataService;
			_grantApplicationService = grantApplicationService;
			_participantService = grantParticipantService;
			_nationalOccupationalClassificationService = nationalOccupationalClassificationService;
			_reCaptchaService = recaptchaService;
			_grantProgramService = grantProgramService;
			_attachmentService = attachmentService;
			_participantInvitationService = participantInvitationService;
		}

		/// <summary>
		/// Return participant information form view.
		/// </summary>
		/// <param name="invitationKey"></param>
		/// <param name="individualKey"></param>
		/// <returns></returns>
		[Route("{invitationKey}/{individualKey?}")]
		public ActionResult Index(string invitationKey, string individualKey = null)
		{
			// Launched on page load - displays Step1 of the wizard
			try
			{
				var model = new ParticipantInfoViewModel();

				var expired = IsInvitationExpired(invitationKey, out Guid guidResult);
				if (expired != null)
					return expired;

				var individualGuid = GetIndividualKey(individualKey);
				var individualKeyValid = IsIndividualInviteValid(guidResult, individualGuid);
				if (individualKeyValid != null)
					return individualKeyValid;

				var maxParticipantEnrolled = IsMaxParticipantEnrolled(new Guid(invitationKey));
				if (maxParticipantEnrolled != null)
					return maxParticipantEnrolled;

				// Continue with the participant information collection process
				PrepareStep1(ref model, guidResult, individualGuid);

				return View(model);
			}
			catch (DbEntityValidationException e)
			{
				this.SetAlert(e);
			}
			catch (Exception e)
			{
				_logger.Error(e);
				this.SetAlert(e);
			}

			return RedirectToAction(nameof(SessionTimeout));
		}

		private static Guid GetIndividualKey(string individualKey)
		{
			if (string.IsNullOrWhiteSpace(individualKey))
				return Guid.Empty;

			var valid = Guid.TryParse(individualKey, out var key);
			return valid ? key : Guid.Empty;
		}

		/// <summary>
		/// Return the applicant view.
		/// </summary>
		/// <param name="invitationKey"></param>
		/// <param name="individualKey"></param>
		/// <returns></returns>
		[Route("Applicant/View/{invitationKey}/{individualKey?}")]
		public ActionResult ApplicantReportingView(string invitationKey, string individualKey = null)
		{
			// Launched on page load - displays Step1 of the wizard
			try
			{
				if (HasConsentForm())
				{
					var attachment = _attachmentService.Get(GetConsentFormId());
					_attachmentService.Delete(attachment);
					Session.Remove("consentFormId");
				}

				var model = new ParticipantInfoViewModel();
				var expired = IsInvitationExpired(invitationKey, out Guid guidResult);
				if (expired != null)
					return expired;

				var individualGuid = GetIndividualKey(individualKey);
				var individualKeyValid = IsIndividualInviteValid(guidResult, individualGuid);
				if (individualKeyValid != null)
					return individualKeyValid;

				// Continue with the participant information collection process
				PrepareStep1(ref model, guidResult, individualGuid);
				model.ParticipantInfoStep0ViewModel.ReportedByApplicant = false;
				return View(model);
			}
			catch (DbEntityValidationException e)
			{
				this.SetAlert(e);
			}
			catch (Exception e)
			{
				_logger.Error(e);
				this.SetAlert(e);
			}

			return RedirectToAction(nameof(SessionTimeout));
		}

		/// <summary>
		/// Submit the participant information form data.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="invitationKey"></param>
		/// <param name="individualKey"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("{invitationKey}/{individualKey?}")]
		public ActionResult Index(ParticipantInfoViewModel model, string invitationKey, string individualKey = null)
		{
			try
			{
				// need to check that the agreement has not been cancelled
				var expired = IsInvitationExpired(invitationKey, out Guid guidResult);
				if (expired != null)
					return expired;

				var individualGuid = GetIndividualKey(individualKey);
				var individualKeyValid = IsIndividualInviteValid(guidResult, individualGuid);
				if (individualKeyValid != null)
					return individualKeyValid;

				// Validate the reCAPTCHA
				var encodedResponse = Request.Form["g-recaptcha-response"];
				var errorCodes = "";

				if (_reCaptchaService.Validate(encodedResponse, ref errorCodes))
				{
					// Move forward to Step2
					TempData["CaptchaValidated"] = true;
					return RedirectToAction(nameof(Form), new { invitationKey });
				}

				ControllerContext.SetAlert("An error has occurred, please try again. If the error continues, please contact support. Additional info:" + errorCodes, AlertType.Error, false);
				PrepareStep1(ref model, guidResult, individualGuid);
			}
			catch (DbEntityValidationException e)
			{
				this.SetAlert(e);
			}
			catch (Exception e)
			{
				_logger.Error(e);
				this.SetAlert(e);
				return RedirectToAction(nameof(SessionTimeout));
			}

			return View(model);
		}

		/// <summary>
		/// Return the participant information form view.
		/// </summary>
		/// <param name="invitationKey"></param>
		/// <param name="individualKey"></param>
		/// <returns></returns>
		[Route("Form/{invitationKey}/{individualKey?}")]
		public ActionResult Form(string invitationKey, string individualKey = null)
		{
			if (TempData["CaptchaValidated"] as bool? != true)
			{
				this.SetAlert("Please complete the Captcha and confirm your attendance.<br />Note: Please do not refresh the page while completing the Participant Information forms.", AlertType.Error, true);
				return RedirectToAction(nameof(Index), new { invitationKey });
			}

			var model = new ParticipantInfoViewModel();

			var expired = IsInvitationExpired(invitationKey, out Guid guidResult);
			if (expired != null)
				return expired;

			var individualGuid = GetIndividualKey(individualKey);
			var individualKeyValid = IsIndividualInviteValid(guidResult, individualGuid);
			if (individualKeyValid != null)
				return individualKeyValid;

			try
			{
				PrepareStep1(ref model, guidResult, individualGuid);
				PrepareStep2(ref model);
				model.ParticipantInfoStep0ViewModel.Step = 2;
			}
			catch (Exception e)
			{
				_logger.Error(e);
				this.SetAlert(e, true);
				return RedirectToAction(nameof(SessionTimeout));
			}

			return View(model);
		}

		/// <summary>
		/// Return the participant information form view.
		/// </summary>
		/// <param name="invitationKey"></param>
		/// <param name="individualKey"></param>
		/// <returns></returns>
		[Route("Applicant/Form/{invitationKey}/{individualKey?}")]
		public ActionResult ApplicantForm(string invitationKey, string individualKey = null)
		{
			var model = new ParticipantInfoViewModel();

			var expired = IsInvitationExpired(invitationKey, out Guid guidResult);
			if (expired != null)
				return expired;

			var individualGuid = GetIndividualKey(individualKey);
			var individualKeyValid = IsIndividualInviteValid(guidResult, individualGuid);
			if (individualKeyValid != null)
				return individualKeyValid;

			try
			{
				PrepareStep1(ref model, guidResult, individualGuid);
				PrepareStep2(ref model);
				model.ParticipantInfoStep0ViewModel.ReportedByApplicant = false;
				model.ParticipantInfoStep0ViewModel.Step = 2;
			}
			catch (Exception e)
			{
				_logger.Error(e);
				this.SetAlert(e, true);
				return RedirectToAction(nameof(SessionTimeout));
			}

			return View("Form", model);
		}

		/// <summary>
		/// Submit the participant information form data.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("Step2")]
		public ActionResult Step2(ParticipantInfoViewModel model)
		{
			try
			{
				// need to check that the agreement has not been cancelled
				var expired = IsInvitationExpired(model.ParticipantInfoStep0ViewModel.InvitationKey, out Guid guidResult);
				if (expired != null)
					return expired;

				var individualKeyValid = IsIndividualInviteValid(guidResult, model.ParticipantInfoStep0ViewModel.IndividualKey ?? Guid.Empty);
				if (individualKeyValid != null)
					return individualKeyValid;

				var maxparticipation = IsMaxParticipantEnrolled(model.ParticipantInfoStep0ViewModel.InvitationKey);
				if (maxparticipation != null)
					return maxparticipation;

				ClearParticipantModelErrors(model);

				if (ModelState.IsValid)
				{
					// Assign data on the next step to the model
					PrepareStep3(ref model);
				}
				else
				{
					model.ValidationErrors = GetClientErrors();
					AddGenericError(model, ModelState.GetErrorMessages("<br />"));
				}
			}
			catch (DbEntityValidationException e)
			{
				this.SetAlert(e);
			}
			catch (Exception e)
			{
				_logger.Error(e);
				this.SetAlert(e, true);
				return RedirectToActionAjax(nameof(SessionTimeout));
			}

			PrimeStep2(ref model);

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Submit the participant information form data.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("Step3")]
		public ActionResult Step3(ParticipantInfoViewModel model)
		{
			try
			{
				// need to check that the agreement has not been cancelled
				var expired = IsInvitationExpired(model.ParticipantInfoStep0ViewModel.InvitationKey, out Guid guidResult);
				if (expired != null)
					return expired;

				var individualKeyValid = IsIndividualInviteValid(guidResult, model.ParticipantInfoStep0ViewModel.IndividualKey ?? Guid.Empty);
				if (individualKeyValid != null)
					return individualKeyValid;

				var maxparticipation = IsMaxParticipantEnrolled(model.ParticipantInfoStep0ViewModel.InvitationKey);
				if (maxparticipation != null)
					return maxparticipation;

				ClearParticipantModelErrors(model);

				if (ModelState.IsValid)
				{
					// Assign data on the next step to the model
					PrepareStep4(ref model);
				}
				else
				{
					model.ValidationErrors = GetClientErrors();
					AddGenericError(model, ModelState.GetErrorMessages("<br />"));
				}
			}
			catch (DbEntityValidationException e)
			{
				this.SetAlert(e);
			}
			catch (Exception e)
			{
				_logger.Error(e);
				this.SetAlert(e, true);
				return RedirectToActionAjax(nameof(SessionTimeout));
			}

			PrimeStep2(ref model);
			PrimeStep3(ref model);

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Submit the participant information form data.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("Step4")]
		public ActionResult Step4(ParticipantInfoViewModel model)
		{
			try
			{
				// need to check that the agreement has not been cancelled
				var expired = IsInvitationExpired(model.ParticipantInfoStep0ViewModel.InvitationKey, out Guid guidResult);
				if (expired != null)
					return expired;

				var individualKeyValid = IsIndividualInviteValid(guidResult, model.ParticipantInfoStep0ViewModel.IndividualKey ?? Guid.Empty);
				if (individualKeyValid != null)
					return individualKeyValid;

				var maxparticipation = IsMaxParticipantEnrolled(model.ParticipantInfoStep0ViewModel.InvitationKey);
				if (maxparticipation != null)
					return maxparticipation;

				ClearParticipantModelErrors(model);

				if (ModelState.IsValid)
				{
					// Assign data on the next step to the model
					PrepareStep5(ref model);
				}
				else
				{
					model.ValidationErrors = GetClientErrors();
					AddGenericError(model, ModelState.GetErrorMessages("<br />"));
				}
			}
			catch (DbEntityValidationException e)
			{
				this.SetAlert(e);
			}
			catch (Exception e)
			{
				_logger.Error(e);
				this.SetAlert(e, true);
				return RedirectToActionAjax(nameof(SessionTimeout));
			}

			PrimeStep2(ref model);
			PrimeStep3(ref model);
			PrimeStep4(ref model);

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Submit the participant information form data.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("Form")]
		public ActionResult Submit(ParticipantInfoViewModel model)
		{
			try
			{
				// need to check that the agreement has not been cancelled
				var expired = IsInvitationExpired(model.ParticipantInfoStep0ViewModel.InvitationKey, out Guid guidResult);
				if (expired != null)
					return expired;

				var individualKeyValid = IsIndividualInviteValid(guidResult, model.ParticipantInfoStep0ViewModel.IndividualKey ?? Guid.Empty);
				if (individualKeyValid != null)
					return individualKeyValid;

				var maxParticipantsEnrolled = IsMaxParticipantEnrolled(model.ParticipantInfoStep0ViewModel.InvitationKey);
				if (maxParticipantsEnrolled != null)
					return maxParticipantsEnrolled;

				if (ModelState.IsValid)
				{
					TempData["GrantProgramId"] = model.ParticipantInfoStep0ViewModel.GrantProgramId;
					var participantFullName = model.ParticipantInfoStep2ViewModel.FirstName.ToLower() + " " + model.ParticipantInfoStep2ViewModel.LastName.ToLower();

					if (model.ParticipantInfoStep0ViewModel.ReportedByApplicant)
					{
						TempData["ParticipantGrantApplicationId"] = model.ParticipantInfoStep0ViewModel.GrantApplicationId;
						TempData["ParticipantName"] = $"{model.ParticipantInfoStep2ViewModel.LastName}, {model.ParticipantInfoStep2ViewModel.FirstName} {model.ParticipantInfoStep2ViewModel.MiddleName}";
						TempData["ReportedByApplicant"] = model.ParticipantInfoStep0ViewModel.ReportedByApplicant;
						PrepareStep6(ref model);
						return RedirectToActionAjax(nameof(Confirmation));
					}
					else
					{
						if (participantFullName == Regex.Replace(model.ParticipantInfoStep5ViewModel.ConsentNameEntered.ToLower(), @"\s+", " "))
						{
							// Assign data on the next step to the model
							PrepareStep6(ref model);

							return RedirectToActionAjax(nameof(Confirmation));
						}

						AddGenericError(model, "The consent name entered does not match the First Name and Last Name entered on Step 2.");
					}
				}
				else
				{
					model.ValidationErrors = GetClientErrors();
					AddGenericError(model, ModelState.GetErrorMessages("<br />"));
				}
			}
			catch (DbEntityValidationException e)
			{
				this.SetAlert(e);
				AddGenericError(model, e.GetValidationMessages());
			}
			catch (Exception e)
			{
				_logger.Error(e);
				this.SetAlert(e, true);
				return RedirectToActionAjax(nameof(SessionTimeout));
			}

			PrimeStep2(ref model);
			PrimeStep3(ref model);
			PrimeStep4(ref model);

			// step #5 is not required if reported by applicant
			if (!model.ParticipantInfoStep0ViewModel.ReportedByApplicant)
			{
				PrimeStep5(ref model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Return the confirmation page view.
		/// </summary>
		/// <returns></returns>
		[Route("Confirmed")]
		public ActionResult Confirmation()
		{
			return View();
		}

		/// <summary>
		/// Return the declined page view.
		/// </summary>
		/// <returns></returns>
		[Route("Declined")]
		public ActionResult Declined()
		{
			return View();
		}

		/// <summary>
		/// Return the exit page view.
		/// </summary>
		/// <returns></returns>
		[Route("Exit")]
		public ActionResult Exit()
		{
			return View();
		}

		[Route("MaxParticipantEnrolled")]
		public ActionResult MaxParticipantEnrolled()
		{
			return View();
		}

		/// <summary>
		/// Return the session timed-out view.
		/// </summary>
		/// <returns></returns>
		[Route("Timeout")]
		public ActionResult SessionTimeout()
		{
			return View();
		}

		/// <summary>
		/// Return the invitation expired view.
		/// </summary>
		/// <returns></returns>
		[Route("Invitation/Expired")]
		public ActionResult InvitationExpired()
		{
			return View();
		}

		/// <summary>
		/// Download the specified attachment.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Download/Participant/Consent")]
		public ActionResult DownloadAttachment()
		{
			var filename = "ParticipantConsentandInformationForm.pdf";
			var fullPath = Server.MapPath($"~/App_Data/PDF/{filename}");

			if (System.IO.File.Exists(fullPath))
			{
				return File(fullPath, System.Net.Mime.MediaTypeNames.Application.Octet, $"{filename}");
			}
			else
			{
				this.SetAlert(string.Format("The sample {0} could not be found.", filename), AlertType.Warning, true);
				return Redirect(Request.UrlReferrer.ToString());
			}
		}

		/// <summary>
		/// Add the attachment to the participant information form.
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		[HttpPost]
		[PreventSpam]
		[Route("Attachment")]
		public ActionResult AddAttachment(HttpPostedFileBase[] files, string component)
		{
			var viewModel = new ParticipantInfoViewModel();

			if (files != null)
			{
				// If file was previously selected, remove old one.
				if (HasConsentForm())
				{
					var attachment = _attachmentService.Get(GetConsentFormId());
					_attachmentService.Delete(attachment);
					Session.Remove("consentFormId");
				}

				viewModel = JsonConvert.DeserializeObject<ParticipantInfoViewModel>(component);

				foreach (var file in files)
				{
					try
					{
						AttachmentModel attachmentType = new AttachmentModel();
						var fileToUpload = file;
						var attachment = fileToUpload.UploadPostedFile(attachmentType);
						_attachmentService.Add(attachment, true);
						viewModel.ParticipantInfoStep0ViewModel.HasConsentForm = true;
						Session["consentFormId"] = attachment.Id;
						Session.Remove("attachmentError");
					}
					catch (Exception ex)
					{
						HandleAngularException(ex, viewModel);
					}
				}
			}

			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}

		#region private helpers
		private ActionResult IsInvitationExpired(Guid invitationKey, out Guid guidResult)
		{
			guidResult = invitationKey;
			return IsInvitationExpired(guidResult, true);
		}

		private ActionResult IsInvitationExpired(string invitationKey, out Guid guidResult)
		{
			bool invitationIsValid = Guid.TryParse(invitationKey, out guidResult);
			if (!invitationIsValid)
			{
				_logger.Error($"The invitationKey is not a valid GUID: {invitationKey}");
				this.SetAlert("Access to the page is denied, request did not have a valid invitation key.", AlertType.Error, true);

				return RedirectToAction(nameof(InvitationExpired));
			}

			return IsInvitationExpired(guidResult, false);
		}

		private ActionResult IsInvitationExpired(Guid guidResult, bool ajax)
		{
			var grantApplication = _grantApplicationService.Get(guidResult);
			if (grantApplication == null || grantApplication.IsInvitationExpired())
			{
				var applicationNumber = grantApplication == null ? string.Empty : $"Grant application {grantApplication.Id}: ";
				
				var inviteExpiry = grantApplication?.InvitationExpiresOn == null
					? "Not set"
					: grantApplication.InvitationExpiresOn.Value.ToString("yyyy-MM-dd h:mm:ss tt");
				_logger.Error($"{applicationNumber}The invitation key is either invalid or expired: { guidResult.ToString() }. Expiry date: { inviteExpiry }");

				if (ajax)
					return RedirectToActionAjax(nameof(InvitationExpired));

				return RedirectToAction(nameof(InvitationExpired));
			}

			return null;
		}

		private ActionResult IsMaxParticipantEnrolled(Guid invitationKey)
		{
			var grantApplication = _grantApplicationService.Get(invitationKey);
			if (grantApplication.GetMaxParticipants() <= grantApplication.ParticipantForms.Count)
			{
				_logger.Info("The total number of allowed participants exceeded.");
				return RedirectToAction(nameof(MaxParticipantEnrolled));
			}

			return null;
		}

		private ActionResult IsIndividualInviteValid(Guid invitationKey, Guid individualKey)
		{
			var grantApplication = _grantApplicationService.Get(invitationKey);
			if (!grantApplication.ParticipantInvitations.Any())
				return null;

			// If we have active invitations, we require a key here
			if (individualKey == Guid.Empty)
				return RedirectToAction("InvitationExpired");

			var individualInvite = GetParticipantInvitation(invitationKey, individualKey);
			if (individualInvite != null)
				return null;

			_logger.Info("The individual invite was either found, or complete.");
			return RedirectToAction("InvitationExpired");
		}

		private ParticipantInvitation GetParticipantInvitation(Guid invitationKey, Guid individualKey)
		{
			var grantApplication = _grantApplicationService.Get(invitationKey);
			var individualInvite = grantApplication
				.ParticipantInvitations
				.Where(pi => pi.ParticipantInvitationStatus == ParticipantInvitationStatus.Sent)
				.FirstOrDefault(pi => pi.IndividualKey == individualKey);

			return individualInvite;
		}

		private void ClearParticipantModelErrors(ParticipantInfoViewModel data)
		{
			for (int i = data.ParticipantInfoStep0ViewModel.Step + 1; i <= 6; i++)
			{
				var parent = "ParticipantInfoStep" + i + "ViewModel";
				var model = data.GetType().GetProperty(parent)?.GetValue(data);
				if (model != null)
				{
					var properties = model.GetType().GetProperties();
					foreach (var property in properties)
					{
						ModelState.Remove($"{parent}.{property.Name}");
					}
				}
			}
		}

		private void PrepareStep1(ref ParticipantInfoViewModel model, Guid invitationKey, Guid? individualKey = null)
		{
			// Prepare to display Step1 (Confirmation)
			if (model == null)
				model = new ParticipantInfoViewModel();

			model.CanadaPostKey = new CanadaPostConfiguration().Key;

			// Create Step1
			var grantApplication = _grantApplicationService.Get(invitationKey);
			model.ParticipantInfoStep0ViewModel = new ParticipantInfoStep0ViewModel
			{
				GrantApplicationId = grantApplication.Id,
				ApplicationSubmissionDate = grantApplication.DateSubmitted ?? DateTime.Now,
				GrantProgramId = grantApplication.GrantOpening.GrantStream.GrantProgram.AccountCodeId,
				DataCollected = 0,
				InvitationKey = invitationKey,
				IndividualKey = individualKey
			};

			var employerNames = new[] { grantApplication.Organization.LegalName, grantApplication.Organization.DoingBusinessAs };

			model.ParticipantInfoStep1ViewModel = new ParticipantInfoStep1ViewModel
			{
				RecaptchaEnabled = _reCaptchaService.IsEnabled(),
				RecaptchaSiteKey = _reCaptchaService.GetSiteKey(),
				ProgramEmployerName = grantApplication.Organization.LegalName,
				ProgramEmployerFullName = string.Join(", doing business as ", employerNames.Where(n => !string.IsNullOrEmpty(n))),
				GrantProgramName = grantApplication.GrantOpening.GrantStream.GrantProgram.Name,
				GrantProgramId = grantApplication.GrantOpening.GrantStream.GrantProgram.AccountCodeId,
				ProgramSponsorName = grantApplication.ApplicantLastName + ", " + grantApplication.ApplicantFirstName,
				ProgramDescription = grantApplication.GetProgramDescription(),
				ProgramStartDate = grantApplication.StartDate.ToLocalMorning(),
				ProgramType = grantApplication.GetProgramType(),
				TimeoutPeriod = ConfigurationManager.AppSettings["ParticipantSessionDuration"].ToString()
			};
		}

		private void PrepareStep2(ref ParticipantInfoViewModel model)
		{
			// Prepare to display Step2 (Contact Info)
			if (model == null)
				model = new ParticipantInfoViewModel();

			model.CanadaPostKey = new CanadaPostConfiguration().Key;

			if (model.ParticipantInfoStep2ViewModel == null)
			{
				// Create Step2
				int participantOldestAge = 0;
				int participantYoungestAge = 0;

				try
				{
					participantOldestAge = int.Parse(ConfigurationManager.AppSettings["ParticipantOldestAge"]);
				}
				catch
				{
					participantOldestAge = 90;
				}

				try
				{
					participantYoungestAge = int.Parse(ConfigurationManager.AppSettings["ParticipantYoungestAge"]);
				}
				catch
				{
					participantYoungestAge = 15;
				}

				Guid guidResult = model.ParticipantInfoStep0ViewModel.InvitationKey;
				var grantApplication = _grantApplicationService.Get(guidResult);

				model.ParticipantInfoStep2ViewModel = new ParticipantInfoStep2ViewModel();
				model.ParticipantInfoStep2ViewModel.ProgramEmployerName = grantApplication.OrganizationLegalName;
				model.ParticipantInfoStep2ViewModel.RegionId = "BC";
				model.ParticipantInfoStep2ViewModel.ParticipantOldestAge = participantOldestAge;
				model.ParticipantInfoStep2ViewModel.ParticipantYoungestAge = participantYoungestAge;
				model.ParticipantInfoStep2ViewModel.EnteredSINs = grantApplication.ParticipantForms.Select(x => x.SIN.Replace("-", "")).ToList();
			}

			if (model.ParticipantInfoStep0ViewModel.IndividualKey != null
			    && model.ParticipantInfoStep0ViewModel.IndividualKey != Guid.Empty)
			{
				var individualInvite = GetParticipantInvitation(model.ParticipantInfoStep0ViewModel.InvitationKey, model.ParticipantInfoStep0ViewModel.IndividualKey.Value);
				if (individualInvite != null)
				{
					model.ParticipantInfoStep2ViewModel.FirstName = individualInvite.FirstName;
					model.ParticipantInfoStep2ViewModel.LastName = individualInvite.LastName;
					model.ParticipantInfoStep2ViewModel.EmailAddress = individualInvite.EmailAddress;
				}
			}

			PrimeStep2(ref model);
		}

		private void PrimeStep2(ref ParticipantInfoViewModel model)
		{
			// Prime objects such as dropdown lists for this step
			model.ParticipantInfoStep2ViewModel.Provinces = _staticDataService.GetProvinces()
				.OrderBy(x => x.Name)
				.Select(x => new KeyValuePair<string, string>(x.Id, x.Name))
				.ToList();
		}

		private void PrepareStep3(ref ParticipantInfoViewModel model)
		{
			// Prepare to display Step3 (Demographic Info)
			if (model == null)
				model = new ParticipantInfoViewModel();

			if (model.ParticipantInfoStep3ViewModel == null)
			{
				// Create Step3
				model.ParticipantInfoStep3ViewModel = new ParticipantInfoStep3ViewModel();
			}

			PrimeStep3(ref model);

			if (model.ParticipantInfoStep4ViewModel != null)
			{
				PrimeStep4(ref model);
			}
		}

		private void PrimeStep3(ref ParticipantInfoViewModel model)
		{
			// Prime objects such as dropdown lists for this step
			model.ParticipantInfoStep3ViewModel.CanadianStatuses = _staticDataService.GetCanadianStatuses().Select(x => new KeyValuePair<int, string>(x.Id, x.Caption)).ToArray();
			model.ParticipantInfoStep3ViewModel.AboriginalBands = _staticDataService.GetAboriginalBands().Select(x => new KeyValuePair<int, string>(x.Id, x.Caption)).ToArray();
			model.ParticipantInfoStep3ViewModel.MaritalStatuses = _staticDataService.GetMaritalStatuses().Select(x => new KeyValuePair<int, string>(x.Id, x.Caption)).ToArray();
			model.ParticipantInfoStep3ViewModel.FederalOfficialLanguages = _staticDataService.GetFederalOfficialLanguages().Select(x => new KeyValuePair<int, string>(x.Id, x.Caption)).ToArray();
			model.ParticipantInfoStep3ViewModel.EducationLevels = _staticDataService.GetEducationLevels().Select(x => new KeyValuePair<int, string>(x.Id, x.Caption)).ToArray();
			model.ParticipantInfoStep3ViewModel.ImmigrationCountries = _staticDataService.GetImmigrationCountries().Select(x => new KeyValuePair<string, string>(x.Id, x.Name)).ToArray();

			// Calculate participant age
			var today = AppDateTime.UtcNow;
			var dateOfBirth = new AppDateTime(
				model.ParticipantInfoStep2ViewModel.DateOfBirth.Year,
				model.ParticipantInfoStep2ViewModel.DateOfBirth.Month,
				model.ParticipantInfoStep2ViewModel.DateOfBirth.Day);
			var age = today.Year - dateOfBirth.Year;

			// If participant's birthday has not yet occurred this year, subtract a year from age
			if (dateOfBirth > today.AddYears(-age)) age--;

			// Assign to Step3ViewModel required to display YouthInCare controls
			model.ParticipantInfoStep3ViewModel.Age = age;

			// Assign to Step3ViewModel required to compare YearToCanada with BirthYear (cannot immigrate before your birth year)
			model.ParticipantInfoStep3ViewModel.BirthYear = model.ParticipantInfoStep2ViewModel.DateOfBirth.Year;
		}

		private void PrepareStep4(ref ParticipantInfoViewModel model)
		{
			// Prepare to display Step4 (Employment Info)
			if (model == null)
				model = new ParticipantInfoViewModel();

			if (model.ParticipantInfoStep4ViewModel == null)
			{
				// Create Step4
				model.ParticipantInfoStep4ViewModel = new ParticipantInfoStep4ViewModel();
			}

			PrimeStep4(ref model);
		}

		private void PrimeStep4(ref ParticipantInfoViewModel model)
		{
			// Prime objects such as dropdown lists for this step
			LookupNocCodes(ref model);

			var kvpEmploymentStatuses = _staticDataService.GetEmploymentStatuses().Select(x => new KeyValuePair<int, string>(x.Id, x.Caption)).ToList();
			model.ParticipantInfoStep4ViewModel.EmploymentStatuses = kvpEmploymentStatuses;

			var kvpEmploymentTypes = _staticDataService.GetEmploymentTypes().Select(x => new KeyValuePair<int, string>(x.Id, x.Caption)).ToList();
			model.ParticipantInfoStep4ViewModel.EmploymentTypes = kvpEmploymentTypes;

			var kvpTrainingResults = _staticDataService.GetTrainingResults().Select(x => new KeyValuePair<int, string>(x.Id, x.Caption)).ToList();
			model.ParticipantInfoStep4ViewModel.TrainingResults = kvpTrainingResults;

			model.ParticipantInfoStep4ViewModel.ProgramType = model.ParticipantInfoStep1ViewModel.ProgramType;
		}

		private void PrepareStep5(ref ParticipantInfoViewModel model)
		{
			// Prepare to display Step5 (Consent)
			if (model == null)
				model = new ParticipantInfoViewModel();

			if (model.ParticipantInfoStep5ViewModel == null)
			{
				// Create Step5
				model.ParticipantInfoStep5ViewModel = new ParticipantInfoStep5ViewModel();
			}
			PrimeStep5(ref model);
		}

		private void PrimeStep5(ref ParticipantInfoViewModel model)
		{
			Guid guidResult = model.ParticipantInfoStep0ViewModel.InvitationKey;
			var grantApplication = _grantApplicationService.Get(guidResult);
			// Prime objects such as dropdown lists for this step
			model.ParticipantInfoStep5ViewModel.ParticipantConsentBody = _grantProgramService.GenerateParticipantConsentBody(grantApplication);
			model.ParticipantInfoStep5ViewModel.ConsentNameEntered = $"{model.ParticipantInfoStep2ViewModel.FirstName} {model.ParticipantInfoStep2ViewModel.LastName}";
		}

		private void PrepareStep6(ref ParticipantInfoViewModel model)
		{
			// Prepare to display Step6 (Completion)
			if (model == null)
				model = new ParticipantInfoViewModel();

			if (model.ParticipantInfoStep6ViewModel == null
			    && model.ParticipantInfoStep0ViewModel.DataCollected == 0)
			{
				// Create Step6
				model.ParticipantInfoStep6ViewModel = new ParticipantInfoStep6ViewModel();

				try
				{
					SaveParticipantInfo(model);
					// Process saved data successfully - display Step6
					model.ParticipantInfoStep0ViewModel.DataCollected = 1;

				}
				catch (Exception)
				{
					// An error has occurred - display Step5
					model.ParticipantInfoStep6ViewModel = null;
					PrimeStep5(ref model);
					throw;
				}
			}
			else
			{
				this.SetAlert("You have already accepted or declined the collection process.", AlertType.Warning);
			}
		}

		private ActionResult RedirectToActionAjax(string action)
		{
			Response.StatusCode = 200;
			Response.TrySkipIisCustomErrors = true;
			var data = new
			{
				RedirectUrl = Url.Action(action)
			};
			return Json(data, JsonRequestBehavior.AllowGet);
		}

		private void LookupNocCodes(ref ParticipantInfoViewModel model)
		{
			#region Current NOC
			model.ParticipantInfoStep4ViewModel.CurrentNoc1Codes = _nationalOccupationalClassificationService.GetNationalOccupationalClassificationLevel(1).Select(n => new KeyValuePair<int, string>(n.Id, $"{n.Code} | {n.Description}")).ToList();
			model.ParticipantInfoStep4ViewModel.CurrentNoc2Codes = GetNocCodes(2);
			model.ParticipantInfoStep4ViewModel.CurrentNoc3Codes = GetNocCodes(3);
			model.ParticipantInfoStep4ViewModel.CurrentNoc4Codes = GetNocCodes(4);
			model.ParticipantInfoStep4ViewModel.CurrentNoc5Codes = GetNocCodes(5);
			#endregion

			#region Future NOC
			model.ParticipantInfoStep4ViewModel.FutureNoc1Codes = _nationalOccupationalClassificationService.GetNationalOccupationalClassificationLevel(1).Select(n => new KeyValuePair<int, string>(n.Id, $"{n.Code} | {n.Description}")).ToList();
			model.ParticipantInfoStep4ViewModel.FutureNoc2Codes = GetNocCodes(2);
			model.ParticipantInfoStep4ViewModel.FutureNoc3Codes = GetNocCodes(3);
			model.ParticipantInfoStep4ViewModel.FutureNoc4Codes = GetNocCodes(4);
			model.ParticipantInfoStep4ViewModel.FutureNoc5Codes = GetNocCodes(5);
			#endregion
		}

		private List<KeyValueParent<int, string, int>> GetNocCodes(int level)
		{
			return _nationalOccupationalClassificationService
				.GetNationalOccupationalClassificationLevel(level)
				.Select(n => new KeyValueParent<int, string, int>(n.Id, $"{n.Code} | {n.Description}", n.ParentId ?? 0))
				.ToList();
		}

		private void SaveParticipantInfo(ParticipantInfoViewModel model)
		{
			try
			{
				if ((model.ParticipantInfoStep0ViewModel == null
				 || model.ParticipantInfoStep1ViewModel == null
				 || model.ParticipantInfoStep2ViewModel == null
				 || model.ParticipantInfoStep3ViewModel == null
				 || model.ParticipantInfoStep4ViewModel == null
				 || model.ParticipantInfoStep5ViewModel == null
				 || model.ParticipantInfoStep6ViewModel == null)
				 && !model.ParticipantInfoStep0ViewModel.ReportedByApplicant)
				{
					// Set error message - most likely the browser back button was used
					ControllerContext.SetAlert("All steps must be performed in sequence. Please close your browser and start again.", AlertType.Error, false);
					return;
				}

				if ((model.ParticipantInfoStep0ViewModel == null
				 || model.ParticipantInfoStep1ViewModel == null
				 || model.ParticipantInfoStep2ViewModel == null
				 || model.ParticipantInfoStep3ViewModel == null
				 || model.ParticipantInfoStep4ViewModel == null)
				 && model.ParticipantInfoStep0ViewModel.ReportedByApplicant)
				{
					// Set error message - most likely the browser back button was used
					ControllerContext.SetAlert("All steps must be performed in sequence. Please close your browser and start again.", AlertType.Error, false);
					return;
				}

				var newParticipantForm = new ParticipantForm
				{
					// Common
					GrantApplicationId = model.ParticipantInfoStep0ViewModel.GrantApplicationId,
					InvitationKey = model.ParticipantInfoStep0ViewModel.InvitationKey,

					// Step 1 of 6
					ProgramSponsorName = model.ParticipantInfoStep1ViewModel.ProgramSponsorName,
					ProgramDescription = model.ParticipantInfoStep1ViewModel.ProgramDescription,
					ProgramStartDate = model.ParticipantInfoStep1ViewModel.ProgramStartDate,

					// Step 2 of 6
					RegionId = model.ParticipantInfoStep2ViewModel.RegionId,
					CountryId = "CA",
					FirstName = model.ParticipantInfoStep2ViewModel.FirstName,
					MiddleName = model.ParticipantInfoStep2ViewModel.MiddleName,
					LastName = model.ParticipantInfoStep2ViewModel.LastName,
					SIN = $"{model.ParticipantInfoStep2ViewModel.SIN1}-{model.ParticipantInfoStep2ViewModel.SIN2}-{model.ParticipantInfoStep2ViewModel.SIN3}",
					PhoneNumber1 = $"{model.ParticipantInfoStep2ViewModel.Phone1AreaCode}{model.ParticipantInfoStep2ViewModel.Phone1Exchange}{model.ParticipantInfoStep2ViewModel.Phone1Number}".FormatPhoneNumber(),
					PhoneExtension1 = !String.IsNullOrWhiteSpace(model.ParticipantInfoStep2ViewModel.Phone1Extension) ? $"{model.ParticipantInfoStep2ViewModel.Phone1Extension}" : null,
					PhoneNumber2 = $"{model.ParticipantInfoStep2ViewModel.Phone2AreaCode}{model.ParticipantInfoStep2ViewModel.Phone2Exchange}{model.ParticipantInfoStep2ViewModel.Phone2Number}".FormatPhoneNumber(),
					PhoneExtension2 = !String.IsNullOrWhiteSpace(model.ParticipantInfoStep2ViewModel.Phone2Extension) ? $"{model.ParticipantInfoStep2ViewModel.Phone2Extension}" : null,
					EmailAddress = model.ParticipantInfoStep2ViewModel.EmailAddress.Trim(),
					AddressLine1 = model.ParticipantInfoStep2ViewModel.AddressLine1,
					AddressLine2 = model.ParticipantInfoStep2ViewModel.AddressLine2,
					City = model.ParticipantInfoStep2ViewModel.City,
					PostalCode = model.ParticipantInfoStep2ViewModel.PostalCode,
					BirthDate = DateTime.SpecifyKind(DateTime.Parse(
					model.ParticipantInfoStep2ViewModel.DateOfBirth.Year.ToString() + "/" +
					model.ParticipantInfoStep2ViewModel.DateOfBirth.Month.ToString() + "/" +
					model.ParticipantInfoStep2ViewModel.DateOfBirth.Day.ToString()), DateTimeKind.Local).ToUniversalTime(),

					// Step 3 of 6
					CanadianStatusId = model.ParticipantInfoStep3ViewModel.CanadianStatus,
					AboriginalBandId = model.ParticipantInfoStep3ViewModel.AboriginalBand != 0 ? model.ParticipantInfoStep3ViewModel.AboriginalBand : null,
					MartialStatusId = model.ParticipantInfoStep3ViewModel.MaritalStatusId != 0 ? model.ParticipantInfoStep3ViewModel.MaritalStatusId : null,
					FederalOfficialLanguageId = model.ParticipantInfoStep3ViewModel.FederalOfficialLanguageId != 0 ? model.ParticipantInfoStep3ViewModel.FederalOfficialLanguageId : null,
					EducationLevelId = model.ParticipantInfoStep3ViewModel.EducationLevel != 0 ? (int?)model.ParticipantInfoStep3ViewModel.EducationLevel : null,
					NumberOfDependents = model.ParticipantInfoStep3ViewModel.NumberOfDependents,
					CanadaImmigrant = model.ParticipantInfoStep3ViewModel.CanadaImmigrant ?? null,
					YearToCanada = model.ParticipantInfoStep3ViewModel.YearToCanada,
					CanadaRefugee = model.ParticipantInfoStep3ViewModel.CanadaRefugee ?? null,
					FromCountry = model.ParticipantInfoStep3ViewModel.CanadaRefugee.HasValue && !string.IsNullOrWhiteSpace(model.ParticipantInfoStep3ViewModel.FromCountry) ? _staticDataService.GetCountry(model.ParticipantInfoStep3ViewModel.FromCountry).Name : null,
					Gender = model.ParticipantInfoStep3ViewModel.Gender,
					YouthInCare = model.ParticipantInfoStep3ViewModel.YouthInCare ?? false,
					PersonDisability = model.ParticipantInfoStep3ViewModel.PersonDisability,
					PersonAboriginal = model.ParticipantInfoStep3ViewModel.PersonAboriginal,
					LiveOnReserve = model.ParticipantInfoStep3ViewModel.LiveOnReserve ?? false,
					VisibleMinority = model.ParticipantInfoStep3ViewModel.VisibleMinority,

					// Step 4 of 6
					EmploymentStatusId = model.ParticipantInfoStep4ViewModel.EmploymentStatus,
					EmploymentTypeId = model.ParticipantInfoStep4ViewModel.EmploymentType != 0 ? model.ParticipantInfoStep4ViewModel.EmploymentType : null,
					TrainingResultId = model.ParticipantInfoStep4ViewModel.TrainingResult != 0 ? (int?)model.ParticipantInfoStep4ViewModel.TrainingResult : null,
					EIBenefitId = model.ParticipantInfoStep4ViewModel.EIBenefit != 0 ? model.ParticipantInfoStep4ViewModel.EIBenefit : EI_BENEFIT_NONE_OF_THE_ABOVE,
					MaternalPaternal = model.ParticipantInfoStep4ViewModel.MaternalPaternal ?? false,
					ReceivingEIBenefit = model.ParticipantInfoStep4ViewModel.CurrentReceiveEI ?? false,
					BceaClient = model.ParticipantInfoStep4ViewModel.BceaClient ?? false,
					EmployedBySupportEmployer = model.ParticipantInfoStep4ViewModel.EmployedBySupportEmployer ?? false,
					HowLongYears = model.ParticipantInfoStep4ViewModel.HowLongYears,
					HowLongMonths = model.ParticipantInfoStep4ViewModel.HowLongMonths,
					BusinessOwner = model.ParticipantInfoStep4ViewModel.BusinessOwner ?? false,
					AvgHoursPerWeek = model.ParticipantInfoStep4ViewModel.AvgHoursPerWeek,
					HourlyWage = model.ParticipantInfoStep4ViewModel.HourlyWage,
					PrimaryCity = model.ParticipantInfoStep4ViewModel.PrimaryCity,
					Apprentice = model.ParticipantInfoStep4ViewModel.Apprentice ?? false,
					ItaRegistered = model.ParticipantInfoStep4ViewModel.ItaRegistered ?? false,
					OtherPrograms = model.ParticipantInfoStep4ViewModel.OtherPrograms ?? false,
					OtherProgramDesc = model.ParticipantInfoStep4ViewModel.OtherProgramDesc,
					JobTitleBefore = model.ParticipantInfoStep4ViewModel.JobTitleBefore,
					JobTitleFuture = model.ParticipantInfoStep4ViewModel.JobTitleFuture,

					// Step 5 of 6
					ConsentDateEntered = AppDateTime.UtcNow
				};

				#region Current NOC
				if (model.ParticipantInfoStep4ViewModel.CurrentNoc5Id.HasValue)
					newParticipantForm.CurrentNoc = _nationalOccupationalClassificationService.GetNationalOccupationalClassification(model.ParticipantInfoStep4ViewModel.CurrentNoc5Id.Value);
				else if (model.ParticipantInfoStep4ViewModel.CurrentNoc4Id.HasValue)
					newParticipantForm.CurrentNoc = _nationalOccupationalClassificationService.GetNationalOccupationalClassification(model.ParticipantInfoStep4ViewModel.CurrentNoc4Id.Value);
				else if (model.ParticipantInfoStep4ViewModel.CurrentNoc3Id.HasValue)
					newParticipantForm.CurrentNoc = _nationalOccupationalClassificationService.GetNationalOccupationalClassification(model.ParticipantInfoStep4ViewModel.CurrentNoc3Id.Value);
				else if (model.ParticipantInfoStep4ViewModel.CurrentNoc2Id.HasValue)
					newParticipantForm.CurrentNoc = _nationalOccupationalClassificationService.GetNationalOccupationalClassification(model.ParticipantInfoStep4ViewModel.CurrentNoc2Id.Value);
				else if (model.ParticipantInfoStep4ViewModel.CurrentNoc1Id.HasValue)
					newParticipantForm.CurrentNoc = _nationalOccupationalClassificationService.GetNationalOccupationalClassification(model.ParticipantInfoStep4ViewModel.CurrentNoc1Id.Value);
				#endregion

				#region Future NOC
				if (model.ParticipantInfoStep4ViewModel.FutureNoc5Id.HasValue)
					newParticipantForm.FutureNoc = _nationalOccupationalClassificationService.GetNationalOccupationalClassification(model.ParticipantInfoStep4ViewModel.FutureNoc5Id.Value);
				else if (model.ParticipantInfoStep4ViewModel.FutureNoc4Id.HasValue)
					newParticipantForm.FutureNoc = _nationalOccupationalClassificationService.GetNationalOccupationalClassification(model.ParticipantInfoStep4ViewModel.FutureNoc4Id.Value);
				else if (model.ParticipantInfoStep4ViewModel.FutureNoc3Id.HasValue)
					newParticipantForm.FutureNoc = _nationalOccupationalClassificationService.GetNationalOccupationalClassification(model.ParticipantInfoStep4ViewModel.FutureNoc3Id.Value);
				else if (model.ParticipantInfoStep4ViewModel.FutureNoc2Id.HasValue)
					newParticipantForm.FutureNoc = _nationalOccupationalClassificationService.GetNationalOccupationalClassification(model.ParticipantInfoStep4ViewModel.FutureNoc2Id.Value);
				else if (model.ParticipantInfoStep4ViewModel.FutureNoc1Id.HasValue)
					newParticipantForm.FutureNoc = _nationalOccupationalClassificationService.GetNationalOccupationalClassification(model.ParticipantInfoStep4ViewModel.FutureNoc1Id.Value);
				#endregion

				var individualInvitation = GetParticipantInvitation(model.ParticipantInfoStep0ViewModel.InvitationKey, model.ParticipantInfoStep0ViewModel.IndividualKey ?? Guid.Empty);

				if (individualInvitation != null)
					newParticipantForm.ExpectedParticipantOutcome = individualInvitation.ExpectedParticipantOutcome;

				if (HasConsentForm())
				{
					newParticipantForm.ParticipantConsentAttachmentId = GetConsentFormId();
					Session["consentFormId"] = null;
				}

				var participantForm = _participantService.Add(newParticipantForm);

				if (individualInvitation != null)
					_participantInvitationService.CompleteIndividualInvitation(participantForm, individualInvitation);

				// Set result message
				this.SetAlert("Thank you. Your participant information has been saved successfully.", AlertType.Success);
			}
			catch (DbEntityValidationException e)
			{
				this.SetAlert(e);
				throw;
			}
			catch (Exception e)
			{
				_logger.Error(e);
				this.SetAlert(e);
				throw;
			}
		}

		private bool HasConsentForm()
		{
			return Session["consentFormId"] != null;
		}

		private int GetConsentFormId()
		{
			if (HasConsentForm())
			{
				return int.Parse(Session["consentFormId"].ToString());
			}

			return 0;
		}

		#endregion
	}
}

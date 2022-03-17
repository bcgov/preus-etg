using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Int.Models.Applications;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Int.Controllers
{
	[RouteArea("Int")]
	public class ApplicationSummaryController : BaseController
	{
		#region Variables
		private readonly IStaticDataService _staticDataService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IAuthorizationService _authorizationService;
		private readonly IRiskClassificationService _riskClassificationService;
		private readonly IGrantAgreementService _grantAgreementService;
		private readonly IDeliveryPartnerService _deliveryPartnerService;
		private readonly IAttachmentService _attachmentService;
		#endregion

		#region Constructors
		public ApplicationSummaryController(
			IControllerService controllerService,
			IAuthorizationService authorizationService,
			IGrantApplicationService grantApplicationService,
			IRiskClassificationService riskClassificationService,
			IGrantAgreementService grantAgreementService,
			IDeliveryPartnerService deliveryPartnerService,
			IAttachmentService attachmentService

		   ) : base(controllerService.Logger)
		{
			_staticDataService = controllerService.StaticDataService;
			_grantApplicationService = grantApplicationService;
			_authorizationService = authorizationService;
			_riskClassificationService = riskClassificationService;
			_grantAgreementService = grantAgreementService;
			_deliveryPartnerService = deliveryPartnerService;
			_attachmentService = attachmentService;
		}
		#endregion

		#region Endpoints
		/// <summary>
		/// Get the summary information for the application details view.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Summary/{grantApplicationId}")]
		public JsonResult GetSummary(int grantApplicationId)
		{
			var model = new ApplicationSummaryViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new ApplicationSummaryViewModel(grantApplication, _deliveryPartnerService, _authorizationService, _grantApplicationService, _riskClassificationService, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get an array of delivery programs.
		/// </summary>
		/// <param name="grantProgramId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Summary/Delivery/Partners/{grantProgramId}")]
		public JsonResult GetDeliveryPartners(int grantProgramId)
		{
			var deliveryPartners = new KeyValuePair<int, string>[] { };
			try
			{
				deliveryPartners = _deliveryPartnerService.GetDeliveryPartners(grantProgramId).Select(x => new KeyValuePair<int, string>(x.Id, x.Caption)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(deliveryPartners, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get an array of delivery partner services.
		/// </summary>
		/// <param name="grantProgramId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Summary/Delivery/Partner/Services/{grantProgramId}")]
		public JsonResult GetDeliveryPartnerServices(int grantProgramId)
		{
			var deliveryPartnerServices = new KeyValuePair<int, string>[] { };
			try
			{
				deliveryPartnerServices = _deliveryPartnerService.GetDeliveryPartnerServices(grantProgramId).Select(x => new KeyValuePair<int, string>(x.Id, x.Caption)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(deliveryPartnerServices, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Assign the specified assessor to the grant application.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Application/Summary/Assign")]
		public JsonResult Assign(ApplicationSummaryViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.Id);
				grantApplication.RowVersion = Convert.FromBase64String(model.RowVersion);
				_grantApplicationService.AssignAssessor(grantApplication, model.AssessorId);
				model = new ApplicationSummaryViewModel(grantApplication, _deliveryPartnerService, _authorizationService, _grantApplicationService, _riskClassificationService, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Update the summary information in the datasource.
		/// </summary>
		/// <param name="summary"></param>
		/// <param name="file"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Application/Summary")]
		public JsonResult UpdateSummary(string summary, System.Web.HttpPostedFileBase file)
		{
			var model = new ApplicationSummaryViewModel();
			try
			{
				model = Newtonsoft.Json.JsonConvert.DeserializeObject<ApplicationSummaryViewModel>(summary);

				var grantApplication = _grantApplicationService.Get(model.Id);
				grantApplication.RowVersion = Convert.FromBase64String(model.RowVersion);

				bool deliveryDatesModified = (model.DeliveryStartDate.ToLocalTime().Date != grantApplication.StartDate.ToLocalTime().Date || model.DeliveryEndDate.ToLocalTime().Date != grantApplication.EndDate.ToLocalTime().Date);
				if (deliveryDatesModified && grantApplication.ApplicationStateInternal.In(ApplicationStateInternal.NewClaim, ApplicationStateInternal.ClaimAssessEligibility, ApplicationStateInternal.ClaimAssessReimbursement, ApplicationStateInternal.ClaimApproved))
				{
					ModelState.AddModelError(nameof(model.DeliveryStartDate) + " + " + nameof(model.DeliveryEndDate), "You cannot change the Delivery Dates when a Claim is currently submitted or a previous Claim has been approved.");
				}

				if (model.DeliveryPartnerId == null)
				{
					model.SelectedDeliveryPartnerServiceIds = new List<int>();
				}

				if ((!model.HasRequestedAdditionalFunding) ?? true)
				{
					ModelState.Remove("DescriptionOfFundingRequested");
				}

				if (ModelState.IsValid)
				{
					// when delivery start/end dates changed
					if (deliveryDatesModified)
					{
						// delivery date range must cover all training programs date ranges
						var earliest = grantApplication.DateSubmitted.Value.ToLocalMorning();
						var latest = model.DeliveryStartDate.AddYears(1);
						if (model.DeliveryStartDate < earliest || latest < model.DeliveryEndDate)
						{
							throw new InvalidOperationException($"Delivery dates must be within {earliest:yyyy-MM-dd} and {latest:yyyy-MM-dd}");
						}

						// Update Delivery dates in the Grant Application.
						grantApplication.StartDate = model.DeliveryStartDate.ToUtcMorning();
						grantApplication.EndDate = model.DeliveryEndDate.ToUtcMidnight();

						if (grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId != ProgramTypes.EmployerGrant)
						{
							DateTime? latestStartTime = null;
							DateTime? earliestEndTime = null;
							foreach (TrainingProgram tp in grantApplication.TrainingPrograms)
							{
								if (latestStartTime == null)
								{
									latestStartTime = tp.StartDate;
								}
								if (earliestEndTime == null)
								{
									earliestEndTime = tp.EndDate;
								}
								if (latestStartTime > tp.StartDate)
								{
									latestStartTime = tp.StartDate;
								}
								if (earliestEndTime < tp.EndDate)
								{
									earliestEndTime = tp.EndDate;
								}
							}

							if (grantApplication.StartDate > latestStartTime || grantApplication.EndDate < earliestEndTime)
							{
								throw new InvalidOperationException("Skills training dates do not fall within your delivery period and will need to be rescheduled. Make sure all your skills training dates are accurate to your plan.");
							}
						}
					}

					grantApplication.Organization.DoingBusinessAsMinistry = model.DoingBusinessAsMinistry;
					grantApplication.RowVersion = Convert.FromBase64String(model.RowVersion);
					grantApplication.RiskClassificationId = model.RiskClassificationId;

					if (_grantAgreementService.AgreementUpdateRequired(grantApplication))
					{
						_grantAgreementService.UpdateAgreement(grantApplication);
					}

					if (file != null)
					{
						var attachment = file.UploadFile(grantApplication.BusinessCaseDocument?.Description ?? string.Empty, file.FileName);
						if (grantApplication.BusinessCaseDocument == null || grantApplication.BusinessCaseDocument.Id == 0)
						{
							grantApplication.BusinessCaseDocument = attachment;
							_attachmentService.Add(attachment,true);
						}
						else
						{
							attachment.Id = grantApplication.BusinessCaseDocument.Id;
							attachment.RowVersion = grantApplication.BusinessCaseDocument.RowVersion;
							_attachmentService.Update(attachment,true);
						}
					}
					if (grantApplication.GrantOpening.GrantStream.IncludeDeliveryPartner)
					{
						_grantApplicationService.UpdateDeliveryPartner(grantApplication, model.DeliveryPartnerId, model.SelectedDeliveryPartnerServiceIds);
						_grantApplicationService.Update(grantApplication, ApplicationWorkflowTrigger.EditSummary);
					}
					else
					{
						_grantApplicationService.Update(grantApplication, ApplicationWorkflowTrigger.EditSummary);
					}

					model = new ApplicationSummaryViewModel(grantApplication, _deliveryPartnerService, _authorizationService, _grantApplicationService, _riskClassificationService, User);
				}
				else
				{
					HandleModelStateValidation(model, ModelState.GetErrorMessages("<br />"));
				}
			}
			catch (Exception e)
			{
				HandleAngularException(e, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}
		#endregion
	}
}
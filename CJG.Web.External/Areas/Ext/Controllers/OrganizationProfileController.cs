using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Areas.Ext.Models.Attachments;
using CJG.Web.External.Areas.Ext.Models.OrganizationProfile;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;
using DataAnnotationsExtensions;
using Newtonsoft.Json;

namespace CJG.Web.External.Areas.Ext.Controllers
{
    /// <summary>
    /// <typeparamref name="OrganizationProfileController"/> class, MVC Controller provides endpoints for managing external organization information.
    /// </summary>
    [ExternalFilter]
	[RouteArea("Ext")]
	public class OrganizationProfileController : BaseController
	{
		private readonly IAuthenticationService _authenticationService;
		private readonly ISiteMinderService _siteMinderService;
		private readonly IUserService _userService;
		private readonly IStaticDataService _staticDataService;
		private readonly IOrganizationService _organizationService;
		private readonly INaIndustryClassificationSystemService _naIndustryClassificationSystemService;
		private readonly IAttachmentService _attachmentService;
		private readonly IApplicationAddressService _applicationAddressService;

		public OrganizationProfileController(
			IControllerService controllerService,
			IAuthenticationService authenticationService,
			IOrganizationService organizationService,
			INaIndustryClassificationSystemService naIndustryClassificationSystem,
			IAttachmentService attachmentService,
			IApplicationAddressService applicationAddressService) : base(controllerService.Logger)
		{
			_authenticationService = authenticationService;
			_userService = controllerService.UserService;
			_siteMinderService = controllerService.SiteMinderService;
			_staticDataService = controllerService.StaticDataService;
			_organizationService = organizationService;
			_naIndustryClassificationSystemService = naIndustryClassificationSystem;
			_attachmentService = attachmentService;
			_applicationAddressService = applicationAddressService;
		}

		/// <summary>
		/// Return a view for organization.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Organization/Profile/View")]
		public ActionResult OrganizationProfileView()
		{
			var backUrl = Request.UrlReferrer?.ToString();
			if (TempData["ProfileBackUrl"] != null)
				backUrl = (string)TempData["ProfileBackUrl"];

			TempData["BackURL"] = backUrl;
			return View();
		}

		/// <summary>
		/// Get the data for the organization view.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Organization/Profile")]
		public JsonResult GetOrganization()
		{
			var model = new OrganizationProfileViewNewModel();

			try
			{
				var currentUser = _userService.GetUser(_siteMinderService.CurrentUserGuid);
				var createOrganizationProfile = currentUser.Organization == null || _organizationService.GetOrganizationProfileAdminUserId(currentUser.Organization.Id) == 0;
				var userCanEditOrganizationProfile = currentUser.Organization != null || createOrganizationProfile;
				var previouslySubmittedApplications = _organizationService.SubmittedGrantApplications(currentUser.OrganizationId);

				if (_userService.SyncOrganizationFromBCeIDAccount(currentUser))
					_authenticationService.Refresh(currentUser);

				model = new OrganizationProfileViewNewModel(currentUser.Organization, _naIndustryClassificationSystemService)
				{
					BackURL = TempData["BackURL"]?.ToString() ?? "/Ext/Home/",
					CreateOrganizationProfile = createOrganizationProfile,
					IsOrganizationProfileAdministrator = currentUser.IsOrganizationProfileAdministrator || model.CreateOrganizationProfile,
					CanEditOrganizationProfile = userCanEditOrganizationProfile,
					HasPreviouslySubmittedApplications = previouslySubmittedApplications > 0,
					RequiresBusinessLicenseDocuments = _organizationService.RequiresBusinessLicenseDocuments(currentUser.OrganizationId)
				};
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Update and save the specified organization.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Organization/Profile")]
		public JsonResult CreateOrganization(OrganizationProfileViewNewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					model.UpdateOrganization(_userService, _siteMinderService, _organizationService);
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
		/// Update and save the specified organization.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="files"></param>
		/// <param name="attachments"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Organization/Profile")]
		public JsonResult UpdateOrganization(OrganizationProfileViewNewModel model, HttpPostedFileBase[] files, string attachments)
		{
			// Have to clear the default validation state since the address sub-model doesn't come over with the ajax 'datatype' set to 'file'.
			ModelState.Clear();

			model.HeadOfficeAddress = JsonConvert.DeserializeObject<AddressViewModel>(model.HeadOfficeAddressBlob);
			model.BusinessLicenseDocumentAttachments = GetBusinessAttachmentsToValidate(attachments);
			model.BusinessWebsite = PrefixBusinessWebsite(model);

			// Have to validate the sub-model since we cleared the original errors
			TryValidateModel(model.HeadOfficeAddress, "HeadOfficeAddress");
			TryValidateModel(model);

			model.RequiresBusinessLicenseDocuments = false;  // Don't force the warning message. Let the validation pick up the warning instead. 

			try
			{
				// Deserialize attachments model. This is required because it isn't easy to deserialize an array when including files in a multipart data form.
				var attachmentsModel = JsonConvert.DeserializeObject<IEnumerable<UpdateAttachmentViewModel>>(attachments);

				if (ModelState.IsValid)
				{
					model.UpdateOrganization(_userService, _siteMinderService, _organizationService);
					model.UpdateOrganizationBusinessLicenses(_userService, _siteMinderService, _attachmentService, files, attachmentsModel);

					model.UpdateOrganizationAddressOnApplications(_userService, _siteMinderService, _applicationAddressService);

					this.SetAlert("Organization Profile has been updated successfully.", AlertType.Success, true);
					model.RedirectURL = Url.Action("OrganizationProfileView");
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

		private string PrefixBusinessWebsite(OrganizationProfileViewNewModel model)
		{
			if (string.IsNullOrWhiteSpace(model.BusinessWebsite))
				return string.Empty;

			var urlAttribute = new UrlAttribute();
			if (urlAttribute.IsValid(model.BusinessWebsite))
				return model.BusinessWebsite;

			var url = model.BusinessWebsite.ToLower();

			if (!url.StartsWith("http://") || !url.StartsWith("https://"))
				return $"http://{model.BusinessWebsite}";

			return model.BusinessWebsite;
		}

		private List<AttachmentViewModel> GetBusinessAttachmentsToValidate(string attachments)
		{
			if (string.IsNullOrWhiteSpace(attachments))
				return new List<AttachmentViewModel>();

			var currentUser = _userService.GetUser(_siteMinderService.CurrentUserGuid);
			var currentOrganization = currentUser.Organization;
			var currentBusinessDocs = currentOrganization?.BusinessLicenseDocuments.Select(a => new AttachmentViewModel(a)).ToList();

			var postedAttachments = JsonConvert.DeserializeObject<IEnumerable<UpdateAttachmentViewModel>>(attachments).ToList();
			var docIdsToRemove = postedAttachments.Where(a => a.Delete).Select(d => d.Id).ToList();
			var docsToAdd = postedAttachments.Where(a => !a.Delete).ToList();

			var documentsToValidate = new List<AttachmentViewModel>();
			documentsToValidate.AddRange(docsToAdd);
			documentsToValidate.AddRange(currentBusinessDocs.Where(c => !docIdsToRemove.Contains(c.Id)));

			return documentsToValidate;
		}

		/// <summary>
		/// Downloads specified business license attachment
		/// </summary>
		/// <param name="attachmentId"></param>
		/// <returns></returns>
		[HttpGet]
		[PreventSpam]
		[Route("Organization/BusinessLicense/Download/{attachmentId}")]
		public ActionResult DownloadAttachment(int attachmentId)
		{
			var model = new BaseViewModel();
			try
			{
				var currentUser = _userService.GetUser(_siteMinderService.CurrentUserGuid);
				var organization = currentUser.Organization;

				var attachment = _attachmentService.GetBusinessLicenseAttachment(organization.Id, attachmentId);
				return File(attachment.AttachmentData, System.Net.Mime.MediaTypeNames.Application.Octet, $"{attachment.FileName}{attachment.FileExtension}");
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get the organization types.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Organization/Types")]
		public JsonResult GetOrganizationTypes()
		{
			var model = new BaseViewModel();

			try
			{
				var result = _staticDataService.GetOrganizationTypes().Select(t => new KeyValuePair<int, string>(t.Id, t.Caption)).ToArray();
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get the Legal Structures data.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Organization/Legal/Structures")]
		public JsonResult GetLegalStructures()
		{
			try
			{
				var model = _staticDataService.GetLegalStructures().Select(x => new KeyValuePair<int, string>(x.Id, x.Caption)).ToArray();
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
		/// Get the Provinces data.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Organization/Provinces")]
		public JsonResult GetProvinces()
		{
			try
			{
				var model = _staticDataService.GetProvinces().Select(x => new KeyValuePair<string, string>(x.Id, x.Name)).ToArray();
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
		/// Get the NAICS data.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Organization/NAICS/{level}/{parentId?}")]
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
	}
}

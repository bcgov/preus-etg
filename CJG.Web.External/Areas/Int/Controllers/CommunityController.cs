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

    [AuthorizeAction(Privilege.GM1, Privilege.SM)]
    [RouteArea("Int")]
    public class CommunityController : BaseController
    {
        #region Variables
        private readonly ICommunityService _communityService;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <paramtyperef name="CommunityController"/> object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="communityService"></param>
		public CommunityController (
			IControllerService controllerService,
            ICommunityService communityService
           ) : base(controllerService.Logger)
		{
            _communityService = communityService;
        }
        #endregion

        #region Endpoints
		/// <summary>
		/// Display the Community Management view.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Admin/Community/View")]
        public ActionResult CommunityManagementView()
        {
            return View();
        }

		/// <summary>
		/// Returns the Modal view.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Admin/Community/View/{id}")]
		public PartialViewResult CommunityView(int id)
		{
			return PartialView("_CommunityView");
		}

		/// <summary>
		/// The data for the specified community 'id'.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Admin/Community/{id}")]
		public JsonResult GetCommunity(int id)
		{
			var community = new CommunityViewModel();
			try {
				community = new CommunityViewModel(_communityService.Get(id));
			} catch (Exception ex) {
				HandleAngularException(ex, community);
			}
			return Json(community, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns a Json result of all the Communities.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
        [PreventSpam]
        [Route("Admin/Communities")]
        public ActionResult GetCommunities()
        {
			var communities = new CommunityManagementViewModel();
			try {
				communities = new CommunityManagementViewModel(_communityService.GetAll());
			} catch (Exception ex) {
				HandleAngularException(ex, communities);
			}
			return Json(communities, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Adds a Community.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[PreventSpam, ValidateRequestHeader]
		[Route("Admin/Community")]
		public JsonResult AddCommunity(CommunityViewModel model)
		{
			try
			 {
				if (ModelState.IsValid) {
					if (_communityService.Get(model.Caption) == null) {
						var community = new Community();

						community.Caption = model.Caption;

						community = _communityService.Add(community);

						model.Id = community.Id;
					} else {
						throw new InvalidOperationException("A community by that name already exists.");
					}
				} else {
					HandleModelStateValidation(model);
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		/// <summary>
		/// Update a Community.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
        [PreventSpam, ValidateRequestHeader]
        [Route("Admin/Community")]
        public JsonResult UpdateCommunity(CommunityViewModel model)
        {
            try
            {
				if (ModelState.IsValid) {
					var community = _communityService.Get(model.Id);
					if (community != null) {
						var communityWithSameCaptionAsModel = _communityService.Get(model.Caption);
						if (communityWithSameCaptionAsModel == null || community.Id == communityWithSameCaptionAsModel.Id) {
							community.Caption = model.Caption;
							community.IsActive = model.Active;

							_communityService.Update(community);
						} else { 
							throw new InvalidOperationException("A community by that name already exists.");
						}
					} else {
						throw new InvalidOperationException("A community by that Id could not be found.");
					}
				} else {
					HandleModelStateValidation(model);
				}
            }
            catch (Exception ex)
            {
                HandleAngularException(ex, model);
            }
            return Json(model);
        }
        #endregion
    }
}

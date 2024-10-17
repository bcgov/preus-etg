using CJG.Application.Business.Models;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
namespace CJG.Web.External.Areas.Int.Controllers
{
	[RouteArea("Int")]
	public class AddressController : BaseController
	{
		#region Variables
		private readonly IStaticDataService _staticDataService;
		#endregion

		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="staticDataService"></param>
		public AddressController(IControllerService controllerService,
								 IStaticDataService staticDataService) : base(controllerService.Logger)
		{
			_staticDataService = controllerService.StaticDataService;
		}
		#endregion

		#region Method
		/// <summary>
		/// Get an array of countries.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[OutputCache(CacheProfile = "StaticData", VaryByParam = "none", Location = OutputCacheLocation.ServerAndClient)]
		[Route("address/countries")]
		public JsonResult GetCountries()
		{
			CollectionItemModel[] results = null;
			try
			{
				results = _staticDataService.GetCountries().Select(x => new CollectionItemModel()
				{
					Key = x.Id,
					Caption = x.Name,
					RowSequence = x.RowSequence
				}).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}

			return Json(results, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get an array of provinces.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[OutputCache(CacheProfile = "StaticData", VaryByParam = "none", Location = OutputCacheLocation.ServerAndClient)]
		[Route("address/provinces")]
		public JsonResult GetProvinces()
		{
			CollectionItemModel[] results = null;
			try
			{
				results = _staticDataService.GetProvinces().Select(x => new CollectionItemModel()
				{
					Key = x.Id,
					Caption = x.Name,
					RowSequence = x.RowSequence
				}).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}

			return Json(results, JsonRequestBehavior.AllowGet);
		}
		#endregion
	}
}
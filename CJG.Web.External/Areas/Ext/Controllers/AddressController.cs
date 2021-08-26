using CJG.Application.Business.Models;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Ext.Controllers
{
	/// <summary>
	/// AddressController class, provides API endpoints for addresses.
	/// </summary>
	[RouteArea("Ext")]
	public class AddressController : BaseController
	{
		#region Variables
		private readonly IStaticDataService _staticDataService;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a AddressController object, and initializes it with the specified services.
		/// </summary>
		/// <param name="controllerService"></param>
		public AddressController(IControllerService controllerService) : base(controllerService.Logger)
		{
			_staticDataService = controllerService.StaticDataService;
		}
		#endregion

		#region Method
		/// <summary>
		/// Return an array of countries.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Address/Countries")]
		public JsonResult GetCountries()
		{
			var _dataSource = new List<CollectionItemModel>();
			try
			{
				_dataSource = _staticDataService.GetCountries().Select(x => new CollectionItemModel()
				{
					Key = x.Id,
					Caption = x.Name,
					RowSequence = x.RowSequence
				}).ToList();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(_dataSource, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Return an array of provinces.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Address/Provinces")]
		public JsonResult GetProvinces()
		{
			var _dataSource = new List<CollectionItemModel>();
			try
			{
				_dataSource = _staticDataService.GetProvinces().Select(x => new CollectionItemModel()
				{
					Key = x.Id,
					Caption = x.Name,
					RowSequence = x.RowSequence
				}).ToList();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(_dataSource, JsonRequestBehavior.AllowGet);
		}
		#endregion
	}
}
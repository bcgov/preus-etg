using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Ext.Controllers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Testing.UnitTests.Controllers.Ext
{
	[TestClass]
	public class AddressControllerTest
	{

		#region GetCountries
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(AddressController))]
		public void GetCountries()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<AddressController>(user);
			var controller = helper.Create();

			var countries = new List<Country>(new[] { new Country("CA", "Canada") });
			helper.GetMock<IStaticDataService>().Setup(m => m.GetCountries()).Returns(countries);

			// Act
			var result = controller.GetCountries();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<CollectionItemModel>>();
			var data = result.Data as List<CollectionItemModel>;
			data.Count().Should().Be(1);
			data.First().Key.Should().Be(countries.First().Id);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetCountries(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(AddressController))]
		public void GetCountries_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<AddressController>(user);

			helper.GetMock<IStaticDataService>().Setup(m => m.GetCountries()).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetCountries();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<CollectionItemModel>>();
			var data = result.Data as List<CollectionItemModel>;
			data.Count().Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region GetProvinces
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(AddressController))]
		public void GetProvinces()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<AddressController>(user);
			var controller = helper.Create();

			var provinces = new List<Region>(new[] { new Region("BC", "British Columbia", new Country("CA", "Canada")) });
			helper.GetMock<IStaticDataService>().Setup(m => m.GetProvinces()).Returns(provinces);

			// Act
			var result = controller.GetProvinces();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<CollectionItemModel>>();
			var data = result.Data as List<CollectionItemModel>;
			data.Count().Should().Be(1);
			data.First().Key.Should().Be(provinces.First().Id);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetProvinces(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(AddressController))]
		public void GetProvinces_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<AddressController>(user);

			helper.GetMock<IStaticDataService>().Setup(m => m.GetProvinces()).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetProvinces();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<CollectionItemModel>>();
			var data = result.Data as List<CollectionItemModel>;
			data.Count().Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion
	}
}

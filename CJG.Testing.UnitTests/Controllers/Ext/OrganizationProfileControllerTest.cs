using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Ext.Controllers;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Areas.Ext.Models.OrganizationProfile;
using CJG.Web.External.Models.Shared;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using CJG.Web.External.Areas.Ext.Models.Attachments;
using Newtonsoft.Json;

namespace CJG.Testing.UnitTests.Controllers.Ext
{
	[TestClass]
	public class OrganizationProfileControllerTest
	{
		#region OrganizationProfileView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(OrganizationProfileController))]
		public void OrganizationProfileView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<OrganizationProfileController>(user);

			var controller = helper.Create();

			// Act
			var result = controller.OrganizationProfileView();

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
		}
		#endregion

		#region GetOrganization
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(OrganizationProfileController))]
		public void GetOrganization()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<OrganizationProfileController>(user);
			var controller = helper.Create();

			helper.GetMock<IUserService>().Setup(m => m.GetUser(It.IsAny<Guid>())).Returns(user);
			helper.GetMock<IOrganizationService>().Setup(m => m.GetOrganizationProfileAdminUserId(It.IsAny<int>())).Returns(user.Id);

			// Act
			var result = controller.GetOrganization();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<OrganizationProfileViewNewModel>();
			var data = result.Data as OrganizationProfileViewNewModel;
			data.Id.Should().Be(user.Organization.Id);
			helper.GetMock<IUserService>().Verify(m => m.GetUser(It.IsAny<Guid>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(OrganizationProfileController))]
		public void GetOrganization_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<OrganizationProfileController>(user);

			helper.GetMock<IUserService>().Setup(m => m.GetUser(It.IsAny<Guid>())).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetOrganization();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<OrganizationProfileViewNewModel>();
			var data = result.Data as OrganizationProfileViewNewModel;
			data.ValidationErrors.Count().Should().NotBe(0);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region CreateOrganization
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(OrganizationProfileController))]
		public void CreateOrganization()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<OrganizationProfileController>(user);
			var controller = helper.Create();

			helper.GetMock<IUserService>().Setup(m => m.GetUser(It.IsAny<Guid>())).Returns(user);
			helper.GetMock<IUserService>().Setup(m => m.GetBCeIDUser(It.IsAny<Guid>())).Returns(user);

			// Act
			var model = new OrganizationProfileViewNewModel();
			Utilities.MapProperties(user.Organization, model);
			model.HeadOfficeAddress = new AddressViewModel(user.Organization.HeadOfficeAddress);
			model.RowVersion = Convert.ToBase64String(user.Organization.RowVersion);
			var result = controller.CreateOrganization(model);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<OrganizationProfileViewNewModel>();
			var data = result.Data as OrganizationProfileViewNewModel;
			data.RowVersion.Should().Be(Convert.ToBase64String(user.Organization.RowVersion));
			helper.GetMock<IUserService>().Verify(m => m.Update(It.IsAny<User>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(OrganizationProfileController))]
		public void CreateOrganization_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<OrganizationProfileController>(user);

			helper.GetMock<IUserService>().Setup(m => m.GetUser(It.IsAny<Guid>())).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.CreateOrganization(new OrganizationProfileViewNewModel());

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<OrganizationProfileViewNewModel>();
			var data = result.Data as OrganizationProfileViewNewModel;
			data.ValidationErrors.Count().Should().NotBe(0);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region UpdateOrganization
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(OrganizationProfileController))]
		public void UpdateOrganization()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<OrganizationProfileController>(user);
			var controller = helper.Create();

			helper.GetMock<IUserService>().Setup(m => m.GetUser(It.IsAny<Guid>())).Returns(user);

			// Act
			var model = new OrganizationProfileViewNewModel();
			Utilities.MapProperties(user.Organization, model);
			model.HeadOfficeAddress = new AddressViewModel(user.Organization.HeadOfficeAddress);
			model.HeadOfficeAddressBlob = JsonConvert.SerializeObject(model.HeadOfficeAddress);
			model.RowVersion = Convert.ToBase64String(user.Organization.RowVersion);
			model.Naics3Id = user.Organization.NaicsId;
			model.Naics2Id = user.Organization.NaicsId;
			model.Naics1Id = user.Organization.NaicsId;
			// Need to stub out the attachments to stop this test from failing.
			//model.BusinessLicenseDocumentAttachments = new List<AttachmentViewModel>();
			var result = controller.UpdateOrganization(model, new HttpPostedFileBase[0], string.Empty);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<OrganizationProfileViewNewModel>();
			
			var data = result.Data as OrganizationProfileViewNewModel;
			Assert.AreEqual(data.ValidationErrors.Count, 0);
			data.RowVersion.Should().Be(Convert.ToBase64String(user.Organization.RowVersion));
			helper.GetMock<IUserService>().Verify(m => m.Update(It.IsAny<User>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(OrganizationProfileController))]
		public void UpdateOrganization_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<OrganizationProfileController>(user);

			helper.GetMock<IUserService>().Setup(m => m.GetUser(It.IsAny<Guid>())).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.UpdateOrganization(new OrganizationProfileViewNewModel(), new HttpPostedFileBase[0], string.Empty);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<OrganizationProfileViewNewModel>();
			var data = result.Data as OrganizationProfileViewNewModel;
			data.ValidationErrors.Count().Should().NotBe(0);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region GetOrganizationTypes
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(OrganizationProfileController))]
		public void GetOrganizationTypes()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<OrganizationProfileController>(user);
			var controller = helper.Create();

			var list = new List<OrganizationType>(new[] { new OrganizationType("Organization Type") });
			helper.GetMock<IStaticDataService>().Setup(m => m.GetOrganizationTypes()).Returns(list);

			// Act
			var result = controller.GetOrganizationTypes();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<KeyValuePair<int, string>[]>();
			var data = result.Data as KeyValuePair<int, string>[];
			data.Count().Should().Be(1);
			data.First().Key.Should().Be(list.First().Id);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetOrganizationTypes(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(OrganizationProfileController))]
		public void GetOrganizationTypes_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<OrganizationProfileController>(user);

			helper.GetMock<IStaticDataService>().Setup(m => m.GetOrganizationTypes()).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetOrganizationTypes();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<BaseViewModel>();
			var data = result.Data as BaseViewModel;
			data.ValidationErrors.Count().Should().NotBe(0);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region GetLegalStructures
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(OrganizationProfileController))]
		public void GetLegalStructures()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<OrganizationProfileController>(user);
			var controller = helper.Create();

			var list = new List<LegalStructure>(new[] { new LegalStructure("Legal Structure") });
			helper.GetMock<IStaticDataService>().Setup(m => m.GetLegalStructures()).Returns(list);

			// Act
			var result = controller.GetLegalStructures();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<KeyValuePair<int, string>[]>();
			var data = result.Data as KeyValuePair<int, string>[];
			data.Count().Should().Be(1);
			data.First().Key.Should().Be(list.First().Id);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetLegalStructures(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(OrganizationProfileController))]
		public void GetLegalStructures_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<OrganizationProfileController>(user);

			helper.GetMock<IStaticDataService>().Setup(m => m.GetLegalStructures()).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetLegalStructures();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<BaseViewModel>();
			var data = result.Data as BaseViewModel;
			data.ValidationErrors.Count().Should().NotBe(0);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region GetProvinces
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(OrganizationProfileController))]
		public void GetProvinces()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<OrganizationProfileController>(user);
			var controller = helper.Create();

			var provinces = new List<Region>(new[] { new Region("BC", "British Columbia", new Country("CA", "Canada")) });
			helper.GetMock<IStaticDataService>().Setup(m => m.GetProvinces()).Returns(provinces);

			// Act
			var result = controller.GetProvinces();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<KeyValuePair<string, string>[]>();
			var data = result.Data as KeyValuePair<string, string>[];
			data.Count().Should().Be(1);
			data.First().Key.Should().Be(provinces.First().Id);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetProvinces(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(OrganizationProfileController))]
		public void GetProvinces_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<OrganizationProfileController>(user);

			helper.GetMock<IStaticDataService>().Setup(m => m.GetProvinces()).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetProvinces();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<BaseViewModel>();
			var data = result.Data as BaseViewModel;
			data.ValidationErrors.Count().Should().NotBe(0);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region GetNAICS
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(OrganizationProfileController))]
		public void GetNAICS()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<OrganizationProfileController>(user);
			var controller = helper.Create();

			var list = new List<NaIndustryClassificationSystem>(new[] { new NaIndustryClassificationSystem("0001", "NAICS", 1, 0, 0, 0, 2012) });
			helper.GetMock<INaIndustryClassificationSystemService>().Setup(m => m.GetNaIndustryClassificationSystemChildren(0, 1)).Returns(list);

			// Act
			var result = controller.GetNAICS(1, 0);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<KeyValuePair<int, string>[]>();
			var data = result.Data as KeyValuePair<int, string>[];
			data.Count().Should().Be(1);
			data.First().Key.Should().Be(list.First().Id);
			helper.GetMock<INaIndustryClassificationSystemService>().Verify(m => m.GetNaIndustryClassificationSystemChildren(0, 1), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(OrganizationProfileController))]
		public void GetNAICS_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<OrganizationProfileController>(user);

			helper.GetMock<INaIndustryClassificationSystemService>().Setup(m => m.GetNaIndustryClassificationSystemChildren(0, 1)).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetNAICS(1, 0);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<BaseViewModel>();
			var data = result.Data as BaseViewModel;
			data.ValidationErrors.Count().Should().NotBe(0);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion
	}
}

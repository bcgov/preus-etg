using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Int.Controllers;
using CJG.Web.External.Areas.Int.Models.Organizations;
using CJG.Web.External.Models.Shared;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static CJG.Testing.Core.ServiceHelper;

namespace CJG.Testing.UnitTests.Controllers.Int
{
	[TestClass]
	public class OrganizationProfileControllerTest
	{
		#region OrganizationView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(OrganizationProfileController))]
		public void OrganizationView()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<OrganizationProfileController>(user, Roles.Assessor);

			var controller = helper.Create();

			// Act
			var result = controller.OrganizationView();

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
		}
		#endregion

		#region GetOrganization
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(OrganizationProfileController))]
		public void GetOrganization()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<OrganizationProfileController>(user, Roles.Assessor);

			var controller = helper.Create();

			// Act
			var result = controller.GetOrganization();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<OrganizationProfileViewModel>();
		}
		#endregion

		#region UpdateOrganization
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(OrganizationProfileController))]
		public void UpdateOrganization()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<OrganizationProfileController>(user, Roles.Assessor);
			var tempUser = EntityHelper.CreateExternalUser();
			tempUser.OrganizationId = 2;
			helper.GetMock<IUserService>().Setup(m => m.GetUser(It.IsAny<int>())).Returns(tempUser);
			helper.GetMock<IUserService>().Setup(m => m.GetUsersForOrganization(It.IsAny<int>())).Returns(new List<User>() { tempUser });

			var controller = helper.Create();

			// Act
			var organizationProfileViewModel = new OrganizationProfileViewModel()
			{
				SelectedUserId = 1,
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var result = controller.UpdateOrganization(organizationProfileViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			helper.GetMock<IUserService>().Verify(m => m.Update(It.IsAny<User>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(OrganizationProfileController))]
		public void UpdateOrganization_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<OrganizationProfileController>(user, Roles.Assessor);
			var tempUser = EntityHelper.CreateExternalUser();

			helper.GetMock<IUserService>().Setup(m => m.GetUser(It.IsAny<int>())).Throws<NoContentException>();

			var controller = helper.Create();

			// Act
			var organizationProfileViewModel = new OrganizationProfileViewModel()
			{
				SelectedUserId = 1,
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var result = controller.UpdateOrganization(organizationProfileViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			((JsonResult)result).Data.Should().NotBeNull().And.BeOfType<OrganizationProfileViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region GetOrganizations
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(OrganizationProfileController))]
		public void GetOrganizations()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<OrganizationProfileController>(user, Roles.Assessor);
			var controller = helper.Create();
			var organization = EntityHelper.CreateOrganization();
			helper.GetMock<IOrganizationService>()
				.Setup(m => m.Search(It.IsAny<string>()))
				.Returns(new List<Organization>() { organization });

			// Act
			var result = controller.GetOrganizations("test");

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<KeyValuePair<int, string>[]>();
			var data = result.Data as KeyValuePair<int, string>[];
			data.Count().Should().Be(1);
			data[0].Key.Should().Be(organization.Id);
			data[0].Value.Should().Be(organization.LegalName);
			helper.GetMock<IOrganizationService>().Verify(m => m.Search(It.IsAny<string>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(OrganizationProfileController))]
		public void GetOrganizations_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<OrganizationProfileController>(user, Roles.Assessor);
			var controller = helper.Create();
			var organization = EntityHelper.CreateOrganization();
			helper.GetMock<IOrganizationService>()
				.Setup(m => m.Search(It.IsAny<string>()))
				.Throws<NoContentException>();

			// Act
			var result = controller.GetOrganizations("test");

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<BaseViewModel>();
			helper.GetMock<IOrganizationService>().Verify(m => m.Search(It.IsAny<string>()), Times.Once);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region GetUsers
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(OrganizationProfileController))]
		public void GetUsers()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<OrganizationProfileController>(user, Roles.Assessor);
			var controller = helper.Create();
			var tempUser = EntityHelper.CreateExternalUser();
			var organization = EntityHelper.CreateOrganization();
			helper.GetMock<IOrganizationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(organization);
			helper.GetMock<IUserService>().Setup(m => m.GetUsersForOrganization(It.IsAny<int>()))
				.Returns(new List<User>() { tempUser });
			helper.GetMock<IUserService>().Setup(m => m.GetBCeIDUser(It.IsAny<Guid>()))
				.Returns(tempUser);

			// Act
			var result = controller.GetUsers(organization.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<OrganizationProfileUserListViewModel>();
			var data = result.Data as OrganizationProfileUserListViewModel;
			data.OrganizationName.Should().Be(organization.LegalName);
			helper.GetMock<IUserService>().Verify(m => m.GetUsersForOrganization(It.IsAny<int>()), Times.Once);
			helper.GetMock<IOrganizationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IUserService>().Verify(m => m.GetBCeIDUser(It.IsAny<Guid>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(OrganizationProfileController))]
		public void GetUsers_Exception()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<OrganizationProfileController>(user, Roles.Assessor);
			var controller = helper.Create();
			var organization = EntityHelper.CreateOrganization();
			helper.GetMock<IUserService>().Setup(m => m.GetUsersForOrganization(It.IsAny<int>()))
				.Throws<NoContentException>();

			// Act
			var result = controller.GetUsers(organization.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<BaseViewModel>();
			helper.GetMock<IUserService>().Verify(m => m.GetUsersForOrganization(It.IsAny<int>()), Times.Once);
			helper.GetMock<IOrganizationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<IUserService>().Verify(m => m.GetBCeIDUser(It.IsAny<Guid>()), Times.Never);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion
	}
}

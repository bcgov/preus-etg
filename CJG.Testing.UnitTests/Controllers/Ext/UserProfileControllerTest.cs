using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.BCeID.WebService;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Ext.Controllers;
using CJG.Web.External.Areas.Ext.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Testing.UnitTests.Controllers.Ext
{
	[TestClass]
	public class UserProfileControllerTest
	{

		#region CreateUserProfileView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(UserProfileController))]
		public void CreateUserProfileView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<UserProfileController>(user);
			helper.GetMock<IUserService>().Setup(m => m.GetUser(It.IsAny<Guid>())).Returns(user);
			var controller = helper.Create();

			// Act
			var result = controller.CreateUserProfileView();

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			user.Id.Should().Be(controller.ViewBag.UserId);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(UserProfileController))]
		public void CreateUserProfileView_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<UserProfileController>(user);
			helper.GetMock<IUserService>().Setup(m => m.GetUser(It.IsAny<Guid>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NotAuthorizedException>(() => controller.CreateUserProfileView());
		}
		#endregion

		#region GetUserProfile
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(UserProfileController))]
		public void GetUserProfile()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<UserProfileController>(user);
			helper.GetMock<IUserService>().Setup(m => m.GetUser(It.IsAny<Guid>())).Returns(user);
			helper.GetMock<IUserService>().Setup(m => m.GetUserProfileDetails(It.IsAny<int>())).Returns(new UserProfileDetailModel(user));
			helper.GetMock<IUserService>().Setup(m => m.SyncOrganizationFromBCeIDAccount(It.IsAny<User>())).Returns(false);

			var controller = helper.Create();

			// Act
			var result = controller.GetUserProfile(user.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<UserProfileViewModel>();
			var model = result.Data as UserProfileViewModel;
			model.UserId.Should().Be(user.Id);
			model.UserProfileDetails.Title.Should().Be(user.JobTitle);
			model.UserProfileDetails.Subscribe.Should().Be(user.IsSubscriberToEmail);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(UserProfileController))]
		public void GetUserProfile_GetBCeIDUser()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<UserProfileController>(user);

			User userTemp = null;
			helper.GetMock<IUserService>().Setup(m => m.GetUser(It.IsAny<Guid>())).Returns(userTemp);
			helper.GetMock<IUserService>().Setup(m => m.GetBCeIDUser(It.IsAny<Guid>())).Returns(user);
			helper.GetMock<IUserService>().Setup(m => m.GetUserProfileDetails(It.IsAny<int>())).Returns(new UserProfileDetailModel(user));
			helper.GetMock<IUserService>().Setup(m => m.SyncOrganizationFromBCeIDAccount(It.IsAny<User>())).Returns(false);

			var controller = helper.Create();

			// Act
			var result = controller.GetUserProfile(user.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<UserProfileViewModel>();
			var model = result.Data as UserProfileViewModel;
			model.UserId.Should().Be(user.Id);
			model.UserProfileDetails.Title.Should().Be(user.JobTitle);
			model.UserProfileDetails.Subscribe.Should().Be(user.IsSubscriberToEmail);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(UserProfileController))]
		public void GetUserProfile_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<UserProfileController>(user);
			User temp = null;
			helper.GetMock<IUserService>().Setup(m => m.GetUser(It.IsAny<Guid>())).Returns(temp);
			helper.GetMock<IUserService>().Setup(m => m.GetBCeIDUser(It.IsAny<Guid>())).Returns(temp);
			var controller = helper.Create();

			// Act
			var result = controller.GetUserProfile(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<UserProfileViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(UserProfileController))]
		public void GetUserProfile_InvalidOperationException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<UserProfileController>(user);
			helper.GetMock<IUserService>().Setup(m => m.GetUser(It.IsAny<Guid>())).Returns(user);
			helper.GetMock<IUserService>().Setup(m => m.SyncOrganizationFromBCeIDAccount(It.IsAny<User>())).Throws<InvalidOperationException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetUserProfile(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<UserProfileViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region CreateUserProfile
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(UserProfileController))]
		public void CreateUserProfile()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<UserProfileController>(user);
			var province = EntityHelper.CreateRegion();
			helper.GetMock<IStaticDataService>().Setup(m => m.GetProvinces()).Returns(new List<Region>() { province });
			var userProfileDetails = new UserProfileDetailModel(user)
			{
				MailingAddressSameAsBusiness = true
			};
			var userProfileViewModel = new UserProfileViewModel()
			{
				UserId = user.Id,
				RowVersion = "AgQGCAoMDhASFA==",
				UserProfileDetails = userProfileDetails
			};
			var controller = helper.Create();

			// Act
			var result = controller.CreateUserProfile(userProfileViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<UserProfileViewModel>();
			var model = result.Data as UserProfileViewModel;
			model.UserId.Should().Be(user.Id);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetProvinces(), Times.Once);
			helper.GetMock<IUserService>()
				.Verify(m => m.CreateUserProfile(It.IsAny<Guid>(), It.IsAny<UserProfileDetailModel>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(UserProfileController))]
		public void CreateUserProfile_Invalid()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<UserProfileController>(user);
			var province = EntityHelper.CreateRegion();
			helper.GetMock<IStaticDataService>().Setup(m => m.GetProvinces()).Returns(new List<Region>() { province });
			var userProfileDetails = new UserProfileDetailModel(user)
			{
				MailingAddressSameAsBusiness = false
			};
			var userProfileViewModel = new UserProfileViewModel()
			{
				UserId = user.Id,
				RowVersion = "AgQGCAoMDhASFA==",
				UserProfileDetails = userProfileDetails
			};
			var controller = helper.Create();

			// Act
			var result = controller.CreateUserProfile(userProfileViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<UserProfileViewModel>();
			var model = result.Data as UserProfileViewModel;
			model.UserId.Should().Be(user.Id);
			var validationErrors = model.ValidationErrors;
			validationErrors.Count().Should().Be(1);
			validationErrors.Any(l => l.Key == "PreferencePrograms").Should().BeTrue();
			helper.GetMock<IStaticDataService>().Verify(m => m.GetProvinces(), Times.Once);
			helper.GetMock<IUserService>()
				.Verify(m => m.CreateUserProfile(It.IsAny<Guid>(), It.IsAny<UserProfileDetailModel>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(UserProfileController))]
		public void CreateUserProfile_NoContentException()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<UserProfileController>(user);
			var province = EntityHelper.CreateRegion();
			helper.GetMock<IStaticDataService>().Setup(m => m.GetProvinces()).Throws<NoContentException>();
			var userProfileViewModel = new UserProfileViewModel()
			{
				UserId = user.Id,
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var controller = helper.Create();

			// Act
			var result = controller.CreateUserProfile(userProfileViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<UserProfileViewModel>();
			var model = result.Data as UserProfileViewModel;
			model.UserId.Should().Be(user.Id);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetProvinces(), Times.Once);
			helper.GetMock<IUserService>()
				.Verify(m => m.CreateUserProfile(It.IsAny<Guid>(), It.IsAny<UserProfileDetailModel>()), Times.Never);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region UpdateUserProfileView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(UserProfileController))]
		public void UpdateUserProfileView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<UserProfileController>(user);
			helper.GetMock<IUserService>().Setup(m => m.GetUser(It.IsAny<Guid>())).Returns(user);
			var controller = helper.Create();

			// Act
			var result = controller.UpdateUserProfileView();

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			user.Id.Should().Be(controller.ViewBag.UserId);
			"/Ext/Home".Should().Be(controller.ViewBag.BackURL);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(UserProfileController))]
		public void UpdateUserProfileView_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<UserProfileController>(user);
			helper.GetMock<IUserService>().Setup(m => m.GetUser(It.IsAny<Guid>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NotAuthorizedException>(() => controller.UpdateUserProfileView());
		}
		#endregion

		#region UpdateUserProfile
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(UserProfileController))]
		public void UpdateUserProfile()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<UserProfileController>(user);
			var province = EntityHelper.CreateRegion();
			helper.GetMock<IStaticDataService>().Setup(m => m.GetProvinces()).Returns(new List<Region>() { province });
			var userProfileDetails = new UserProfileDetailModel(user)
			{
				MailingAddressSameAsBusiness = true
			};
			var userProfileViewModel = new UserProfileViewModel()
			{
				UserId = user.Id,
				RowVersion = "AgQGCAoMDhASFA==",
				UserProfileDetails = userProfileDetails
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateUserProfile(userProfileViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<UserProfileViewModel>();
			var model = result.Data as UserProfileViewModel;
			model.UserId.Should().Be(user.Id);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetProvinces(), Times.Once);
			helper.GetMock<IUserService>()
				.Verify(m => m.UpdateUserProfile(It.IsAny<int>(), It.IsAny<UserProfileDetailModel>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(UserProfileController))]
		public void UpdateUserProfile_Invalid()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<UserProfileController>(user);
			var province = EntityHelper.CreateRegion();
			helper.GetMock<IStaticDataService>().Setup(m => m.GetProvinces()).Returns(new List<Region>() { province });
			var userProfileDetails = new UserProfileDetailModel(user)
			{
				MailingAddressSameAsBusiness = false
			};
			var userProfileViewModel = new UserProfileViewModel()
			{
				UserId = user.Id,
				RowVersion = "AgQGCAoMDhASFA==",
				UserProfileDetails = userProfileDetails
			};
			var controller = helper.Create();
			controller.ModelState.AddModelError("PreferencePrograms", "PreferencePrograms is required");
			// Act
			var result = controller.UpdateUserProfile(userProfileViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<UserProfileViewModel>();
			var model = result.Data as UserProfileViewModel;
			model.UserId.Should().Be(user.Id);
			var validationErrors = model.ValidationErrors;
			validationErrors.Count().Should().Be(2);
			validationErrors.Any(l => l.Key == "Summary").Should().BeTrue();
			validationErrors.Any(l => l.Key == "PreferencePrograms").Should().BeTrue();
			helper.GetMock<IStaticDataService>().Verify(m => m.GetProvinces(), Times.Once);
			helper.GetMock<IUserService>()
				.Verify(m => m.UpdateUserProfile(It.IsAny<int>(), It.IsAny<UserProfileDetailModel>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(UserProfileController))]
		public void UpdateUserProfile_NoContentException()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<UserProfileController>(user);
			var province = EntityHelper.CreateRegion();
			helper.GetMock<IStaticDataService>().Setup(m => m.GetProvinces()).Throws<NoContentException>();
			var userProfileViewModel = new UserProfileViewModel()
			{
				UserId = user.Id,
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateUserProfile(userProfileViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<UserProfileViewModel>();
			var model = result.Data as UserProfileViewModel;
			model.UserId.Should().Be(user.Id);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetProvinces(), Times.Once);
			helper.GetMock<IUserService>()
				.Verify(m => m.UpdateUserProfile(It.IsAny<int>(), It.IsAny<UserProfileDetailModel>()), Times.Never);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region ConfirmDetailsView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(UserProfileController))]
		public void ConfirmDetailsView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<UserProfileController>(user);
			helper.GetMock<IUserService>().Setup(m => m.GetBCeIDUser(It.IsAny<Guid>())).Returns(user);
			var controller = helper.Create();

			// Act
			var result = controller.ConfirmDetailsView();

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(UserProfileController))]
		public void ConfirmDetailsView_BCeIDException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<UserProfileController>(user);
			User temp = null;
			helper.GetMock<IUserService>().Setup(m => m.GetUser(It.IsAny<Guid>())).Returns(temp);
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<BCeIDException>(() => controller.ConfirmDetailsView());
		}
		#endregion

		#region GetConfirmDetails
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(UserProfileController))]
		public void GetConfirmDetails()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<UserProfileController>(user);
			var userProfileViewModel = new UserProfileViewModel()
			{
				UserId = user.Id,
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var userProfileConfirmationModel = new UserProfileConfirmationModel()
			{
				FirstName = "FirstName",
				LastName = "LastName",
				EmailAddress = "test@test.ca"
			};
			helper.GetMock<IUserService>().Setup(m => m.GetConfirmationDetails(It.IsAny<Guid>())).Returns(userProfileConfirmationModel);
			var controller = helper.Create();

			// Act
			var result = controller.GetConfirmDetails();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<UserProfileViewModel>();
			var model = result.Data as UserProfileViewModel;
			model.UserProfileConfirmation.FirstName.Should().Be(userProfileConfirmationModel.FirstName);
			model.UserProfileConfirmation.LastName.Should().Be(userProfileConfirmationModel.LastName);
			model.UserProfileConfirmation.EmailAddress.Should().Be(userProfileConfirmationModel.EmailAddress);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(UserProfileController))]
		public void GetConfirmDetails_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<UserProfileController>(user);
			helper.GetMock<IUserService>().Setup(m => m.GetConfirmationDetails(It.IsAny<Guid>())).Throws<NotAuthorizedException>();

			helper.GetMock<IUserService>().Setup(m => m.SyncOrganizationFromBCeIDAccount(It.IsAny<User>())).Throws<InvalidOperationException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetConfirmDetails();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<UserProfileViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion
	}
}

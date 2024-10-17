using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Int.Controllers;
using CJG.Web.External.Areas.Int.Models;
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
	public class ApplicantContactControllerTest
	{

		#region GetApplicantContact
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicantContactController))]
		public void GetApplicantContact()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicantContactController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);

			// Act
			var result = controller.GetApplicantContact(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicantContactViewModel>();
			var model = result.Data as ApplicantContactViewModel;
			model.Id.Should().Be(grantApplication.Id);
			model.ApplicantJobTitle.Should().Be(grantApplication.ApplicantJobTitle);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicantContactController))]
		public void GetApplicantContact_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicantContactController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();

			// Act
			var result = controller.GetApplicantContact(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicantContactViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			controller.ControllerContext.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion

		#region UpdateApplicantContact
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicantContactController))]
		public void UpdateApplicantContact()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicantContactController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication();
			var region = EntityHelper.CreateRegion("test");
			var country = EntityHelper.CreateCountry("country test");

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IStaticDataService>().Setup(m => m.GetRegion(It.IsAny<string>(), It.IsAny<string>())).Returns(region);
			helper.GetMock<IStaticDataService>().Setup(m => m.GetCountry(It.IsAny<string>())).Returns(country);

			var phoneNumberViewModel = new PhoneViewModel("604-000-0000", "454");
			var physicalAddress = new AddressViewModel()
			{
				RowVersion = "AgQGCAoMDhASFA==",
				CountryId = CJG.Core.Entities.Constants.CanadaCountryId,
				IsCanadianAddress = true
			};
			var applicantContactViewModel = new ApplicantContactViewModel()
			{
				Id = grantApplication.Id,
				ApplicantFirstName = "ApplicantFirstName",
				ApplicantLastName = "ApplicantLastName",
				PhoneNumberViewModel = phoneNumberViewModel,
				MailingAddressSameAsPhysical = true,
				RowVersion = "AgQGCAoMDhASFA==",
				PhysicalAddress = physicalAddress
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateApplicantContact(applicantContactViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicantContactViewModel>();
			var model = result.Data as ApplicantContactViewModel;
			model.Id.Should().Be(grantApplication.Id);
			helper.GetMock<IGrantApplicationService>()
				.Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), It.IsAny<Func<ApplicationWorkflowTrigger, bool>>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicantContactController))]
		public void UpdateApplicantContact_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicantContactController>(user, Roles.Assessor);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();

			var phoneNumberViewModel = new PhoneViewModel("604-000-0000", "454");
			var physicalAddress = new AddressViewModel()
			{
				RowVersion = "AgQGCAoMDhASFA==",
				CountryId = CJG.Core.Entities.Constants.CanadaCountryId,
				IsCanadianAddress = true
			};
			var applicantContactViewModel = new ApplicantContactViewModel()
			{
				Id = 1,
				ApplicantFirstName = "ApplicantFirstName",
				ApplicantLastName = "ApplicantLastName",
				PhoneNumberViewModel = phoneNumberViewModel,
				MailingAddressSameAsPhysical = true,
				RowVersion = "AgQGCAoMDhASFA==",
				PhysicalAddress = physicalAddress
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateApplicantContact(applicantContactViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicantContactViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IGrantApplicationService>()
				.Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), It.IsAny<Func<ApplicationWorkflowTrigger, bool>>()), Times.Never);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicantContactController))]
		public void UpdateApplicantContact_InvalidModel()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicantContactController>(user, Roles.Assessor);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var phoneNumberViewModel = new PhoneViewModel("604-000-0000", "454");
			var physicalAddress = new AddressViewModel()
			{
				RowVersion = "AgQGCAoMDhASFA==",
				CountryId = CJG.Core.Entities.Constants.CanadaCountryId,
				IsCanadianAddress = true
			};
			var applicantContactViewModel = new ApplicantContactViewModel()
			{
				Id = 1,
				ApplicantLastName = "ApplicantLastName",
				PhoneNumberViewModel = phoneNumberViewModel,
				MailingAddressSameAsPhysical = true,
				RowVersion = "AgQGCAoMDhASFA==",
				PhysicalAddress = physicalAddress
			};
			var controller = helper.Create();
			controller.ModelState.AddModelError("ApplicantFirstName", "ApplicantFirstName is required");
			// Act
			var result = controller.UpdateApplicantContact(applicantContactViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicantContactViewModel>();
			var validationErrors = (result.Data as ApplicantContactViewModel).ValidationErrors;
			validationErrors.Count().Should().Be(1);
			validationErrors.Any(l => l.Key == "ApplicantFirstName").Should().BeTrue();
			helper.GetMock<IGrantApplicationService>()
				.Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), It.IsAny<Func<ApplicationWorkflowTrigger, bool>>()), Times.Never);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
		}
		#endregion

		#region GetApplicantsForOrganization
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicantContactController))]
		public void GetApplicantsForOrganization()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicantContactController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			var externalUser = EntityHelper.CreateExternalUser();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.GetAvailableApplicationContacts(It.IsAny<GrantApplication>()))
				.Returns(new List<User>() { externalUser });
			// Act
			var result = controller.GetApplicantsForOrganization(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<KeyValuePair<int, string>>>();
			var model = result.Data as List<KeyValuePair<int, string>>;
			model.Count().Should().Be(1);
			model[0].Key.Should().Be(externalUser.Id);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.GetAvailableApplicationContacts(It.IsAny<GrantApplication>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicantContactController))]
		public void GetApplicantsForOrganization_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicantContactController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();

			// Act
			var result = controller.GetApplicantsForOrganization(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<KeyValuePair<int, string>>>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.GetAvailableApplicationContacts(It.IsAny<GrantApplication>()), Times.Never);
			controller.ControllerContext.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion

		#region ChangeApplicant
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicantContactController))]
		public void ChangeApplicant()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicantContactController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			var externalUser = EntityHelper.CreateExternalUser();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var changeApplicantContactViewModel = new Web.External.Areas.Int.Models.Applicants.ChangeApplicantContactViewModel()
			{
				Id = grantApplication.Id,
				RowVersion = "AgQGCAoMDhASFA==",
				ApplicantContactId = 1
			};
			// Act
			var result = controller.ChangeApplicant(changeApplicantContactViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicantContactViewModel>();
			var model = result.Data as ApplicantContactViewModel;
			model.Id.Should().Be(grantApplication.Id);
			model.ApplicantFirstName.Should().Be(grantApplication.ApplicantFirstName);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>()
				.Verify(m => m.ChangeApplicationAdministrator(It.IsAny<GrantApplication>(), It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicantContactController))]
		public void ChangeApplicant_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicantContactController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var changeApplicantContactViewModel = new Web.External.Areas.Int.Models.Applicants.ChangeApplicantContactViewModel()
			{
				Id = grantApplication.Id,
				RowVersion = "AgQGCAoMDhASFA==",
				ApplicantContactId = 1
			};
			// Act
			var result = controller.ChangeApplicant(changeApplicantContactViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicantContactViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>()
							.Verify(m => m.ChangeApplicationAdministrator(It.IsAny<GrantApplication>(), It.IsAny<int>()), Times.Never);
			controller.ControllerContext.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicantContactController))]
		public void ChangeAlternateContact()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicantContactController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);

			var changeApplicantContactViewModel = new ApplicantContactViewModel
			{
				Id = grantApplication.Id,
				RowVersion = "AgQGCAoMDhASFA==",
				AlternateFirstName = "Fred",
				AlternateLastName = "Smith",
				AlternatePhoneNumberViewModel = new PhoneViewModel()
			};

			// Act
			var result = controller.ChangeAlternateContact(changeApplicantContactViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicantContactViewModel>();

			var model = result.Data as ApplicantContactViewModel;
			model.Id.Should().Be(grantApplication.Id);
			//model.AlternateFirstName.Should().Be(changeApplicantContactViewModel.AlternateFirstName);   // Not testing the result of the ChangeAlternateContact method

			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IGrantApplicationService>()
				.Verify(m => m.ChangeAlternateContact(It.IsAny<GrantApplication>(), It.IsAny<AlternateContactModel>()), Times.Once);
		}
	}
}

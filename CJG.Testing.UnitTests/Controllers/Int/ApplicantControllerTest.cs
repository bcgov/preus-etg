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
using System.Web.Mvc;
using static CJG.Testing.Core.ServiceHelper;

namespace CJG.Testing.UnitTests.Controllers.Int
{
	[TestClass]
	public class ApplicantControllerTest
	{
		#region GetApplicant
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicantController))]
		public void GetApplicant()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicantController>(user, Roles.Assessor);
			var controller = helper.Create();
			var naics = EntityHelper.CreateNaIndustryClassificationSystem();
			var grantApplication = EntityHelper.CreateGrantApplication();
			grantApplication.OrganizationTypeId = 1;
			grantApplication.OrganizationLegalStructureId = 1;
			grantApplication.NAICSId = naics.Id;
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<INaIndustryClassificationSystemService>().Setup(m => m.GetNaIndustryClassificationSystems(It.IsAny<int>())).Returns(new List<NaIndustryClassificationSystem> { naics });

			// Act
			var result = controller.GetApplicant(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicantViewModel>();
			var model = result.Data as ApplicantViewModel;
			model.Id.Should().Be(grantApplication.Id);
			model.OrganizationLegalName.Should().Be(grantApplication.OrganizationLegalName);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicantController))]
		public void GetApplicant_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicantController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();

			// Act
			var result = controller.GetApplicant(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicantViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			controller.ControllerContext.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion

		#region UpdateApplicant
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicantController))]
		public void UpdateApplicant()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicantController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication();
			var region = EntityHelper.CreateRegion("BC");
			var country = EntityHelper.CreateCountry("CA");
			var naics = EntityHelper.CreateNaIndustryClassificationSystem();

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IStaticDataService>().Setup(m => m.GetRegion(It.IsAny<string>(), It.IsAny<string>())).Returns(region);
			helper.GetMock<IStaticDataService>().Setup(m => m.GetCountry(It.IsAny<string>())).Returns(country);
			helper.GetMock<INaIndustryClassificationSystemService>().Setup(m => m.GetNaIndustryClassificationSystems(It.IsAny<int>())).Returns(new List<NaIndustryClassificationSystem> { naics });

			var applicantViewModel = new ApplicantViewModel()
			{
				Id = grantApplication.Id,
				OrganizationLegalName = grantApplication.OrganizationLegalName,
				OrganizationTypeId = 0,
				LegalStructureId = 0,
				OrganizationYearEstablished = 0,
				OrganizationNumberOfEmployeesWorldwide = 0,
				OrganizationNumberOfEmployeesBC = 0,
				OrganizationAnnualTrainingBudget = 0,
				OrganizationAnnualEmployeesTrained = 0,
				BusinessLicenseNumber = "",
				BusinessNumber = "",
				BusinessNumberVerified = false,
				StatementOfRegistrationNumber = "",
				JurisdictionOfIncorporation = "",
				IncorporationNumber = "",
				AddressLine1 = "",
				AddressLine2 = "",
				City = "",
				RegionId = "",
				Region = "",
				PostalCode = "",
				CountryId = "",
				Country = "",
				HasAppliedForGrantBefore = false,
				WouldTrainEmployeesWithoutGrant = false,
				NAICSLevel1Id = 0,
				NAICSLevel2Id = 0,
				NAICSLevel3Id = 0,
				NAICSLevel4Id = 0,
				NAICSLevel5Id = 0,
				GrantProgramName = "",
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateApplicant(applicantViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicantViewModel>();
			var model = result.Data as ApplicantViewModel;
			model.Id.Should().Be(grantApplication.Id);
			helper.GetMock<IGrantApplicationService>()
				.Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), It.IsAny<Func<ApplicationWorkflowTrigger, bool>>()), Times.Once);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicantController))]
		public void UpdateApplicant_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicantController>(user, Roles.Assessor);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();

			var phoneNumberViewModel = new PhoneViewModel("604-000-0000", "454");
			var physicalAddress = new AddressViewModel()
			{
				RowVersion = "AgQGCAoMDhASFA==",
				CountryId = CJG.Core.Entities.Constants.CanadaCountryId,
				IsCanadianAddress = true
			};
			var applicantViewModel = new ApplicantViewModel()
			{
				Id = 1,
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateApplicant(applicantViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicantViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IGrantApplicationService>()
				.Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), It.IsAny<Func<ApplicationWorkflowTrigger, bool>>()), Times.Never);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicantController))]
		public void UpdateApplicant_InvalidModel()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<ApplicantController>(user, Roles.Assessor);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var phoneNumberViewModel = new PhoneViewModel("604-000-0000", "454");
			var physicalAddress = new AddressViewModel()
			{
				RowVersion = "AgQGCAoMDhASFA==",
				CountryId = CJG.Core.Entities.Constants.CanadaCountryId,
				IsCanadianAddress = true
			};
			var applicantViewModel = new ApplicantViewModel()
			{
				Id = 1,
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var controller = helper.Create();
			controller.ModelState.AddModelError("ApplicantFirstName", "ApplicantFirstName is required");
			// Act
			var result = controller.UpdateApplicant(applicantViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<ApplicantViewModel>();
			helper.GetMock<IGrantApplicationService>()
				.Verify(m => m.Update(It.IsAny<GrantApplication>(), It.IsAny<ApplicationWorkflowTrigger>(), It.IsAny<Func<ApplicationWorkflowTrigger, bool>>()), Times.Never);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
		}
		#endregion
	}
}

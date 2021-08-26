using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Ext.Controllers;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Models.Shared.SkillsTrainings;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CJG.Testing.UnitTests.Controllers.Ext
{
	[TestClass]
	public class SkillsTrainingControllerTest
	{

		#region SkillsTrainingView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void SkillsTrainingView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.SkillsTrainingView(grantApplication.Id, eligibleExpenseType.Id, trainingProgram.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			eligibleExpenseType.Id.Should().Be(controller.ViewBag.EligibleExpenseTypeId);
			trainingProgram.Id.Should().Be(controller.ViewBag.TrainingProgramId);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void SkillsTrainingView_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NotAuthorizedException>(() => controller.SkillsTrainingView(1, 1, 1));
		}
		#endregion

		#region GetSkillsTraining
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void GetSkillsTraining()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetSkillsTraining(grantApplication.Id, eligibleExpenseType.Id, 0);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillTrainingViewModel>();
			var model = result.Data as SkillTrainingViewModel;
			model.GrantApplicationId.Should().Be(grantApplication.Id);
			model.EligibleExpenseTypeId.Should().Be(eligibleExpenseType.Id);
			model.SkillTrainingDetails.EligibleExpenseTypeId.Should().Be(eligibleExpenseType.Id);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void GetSkillsTraining_ExistTrainingProgram()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var eligibleExpenseBreakdown = EntityHelper.CreateEligibleExpenseBreakdown(eligibleExpenseType);
			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var eligibleCostBreakdown = EntityHelper.CreateEligibleCostBreakdown(eligibleCost, eligibleExpenseBreakdown);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			trainingProgram.EligibleCostBreakdown = eligibleCostBreakdown;
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<ITrainingProgramService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProgram);
			var controller = helper.Create();

			// Act
			var result = controller.GetSkillsTraining(grantApplication.Id, eligibleExpenseType.Id, trainingProgram.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillTrainingViewModel>();
			var model = result.Data as SkillTrainingViewModel;
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void GetSkillsTraining_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetSkillsTraining(1, 1, 0);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillTrainingViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void GetSkillsTraining_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NoContentException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetSkillsTraining(1, 1, 1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillTrainingViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
		}
		#endregion

		#region AddSkillsTraining
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void AddSkillsTraining()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var eligibleExpenseBreakdown = EntityHelper.CreateEligibleExpenseBreakdown(eligibleExpenseType);
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("Test", 1);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<IEligibleExpenseBreakdownService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(eligibleExpenseBreakdown);
			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.Get<TrainingProviderType>(It.IsAny<int>()))
				.Returns(trainingProviderType);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetTrainingProviderType(It.IsAny<int>()))
				.Returns(trainingProviderType);
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();
			var serviceProviderViewModel = new SkillTrainingProviderDetailsViewModel()
			{
				Id = 0,
				RowVersion = "AgQGCAoMDhASFA==",
				EligibleCostBreakdownId = 1,
				ContactPhone = "6041234567",
				ContactPhoneAreaCode = "604",
				ContactPhoneExchange = "123",
				ContactPhoneNumber = "4567",
				Name = "test",
				ChangeRequestReason = "reason",
				GrantApplicationId = grantApplication.Id,
				TrainingProviderInventory = trainingProviderInventory,
				TrainingProviderInventoryId = trainingProviderInventory.Id,
				PrivateSectorValidationType = TrainingProviderPrivateSectorValidationTypes.Never,
				TrainingProviderTypeId = 1,
				TrainingOutsideBC = false,
				ContactFirstName = "first name",
				ContactLastName = "Last Name",
				ContactEmail = "test@dfad.ca",
				AddressLine1 = "address",
				City = "city",
				PostalCode = "V9C9C9"
			};
			var skillTrainingDetailsViewModel = new SkillTrainingDetailsViewModel()
			{
				Id = 0,
				RowVersion = "AgQGCAoMDhASFA==",
				GrantApplicationId = grantApplication.Id,
				DeliveryStartDate = AppDateTime.UtcNow,
				DeliveryEndDate = AppDateTime.UtcNow,
				StartDate = AppDateTime.UtcNow,
				StartYear = AppDateTime.UtcNow.Year,
				StartMonth = AppDateTime.UtcNow.Month,
				StartDay = AppDateTime.UtcNow.Day,
				EndDate = AppDateTime.UtcNow,
				EndYear = AppDateTime.UtcNow.Year,
				EndMonth = AppDateTime.UtcNow.Month,
				EndDay = AppDateTime.UtcNow.Day,
				CourseTitle = "CourseTitle Test",
				TotalTrainingHours = 23,
				TitleOfQualification = "TitleOfQualification test",
				SelectedDeliveryMethodIds = new int[] { 1 },
				ExpectedQualificationId = 1,
				SkillLevelId = 1,
				ServiceLineBreakdownCaption = "ServiceLineBreakdownCaption Test",
				TrainingProvider = serviceProviderViewModel,
				TotalCost = 4324,
				EligibleExpenseBreakdownId = 1,
				EligibleExpenseTypeId = eligibleExpenseType.Id,
			};
			var skillTrainingViewModel = new SkillTrainingViewModel()
			{
				GrantApplicationId = grantApplication.Id,
				SkillTrainingDetails = skillTrainingDetailsViewModel,
				RowVersion = "AgQGCAoMDhASFA==",
				EligibleExpenseTypeId = eligibleExpenseType.Id,
			};
			var controller = helper.Create();

			// Act
			var result = controller.AddSkillsTraining(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(skillTrainingViewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillTrainingViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Add(It.IsAny<TrainingProgram>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<IEligibleExpenseBreakdownService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderType(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void AddSkillsTraining_InValidModel()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var eligibleExpenseBreakdown = EntityHelper.CreateEligibleExpenseBreakdown(eligibleExpenseType);
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("Test", 1);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<IEligibleExpenseBreakdownService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(eligibleExpenseBreakdown);
			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.Get<TrainingProviderType>(It.IsAny<int>()))
				.Returns(trainingProviderType);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetTrainingProviderType(It.IsAny<int>()))
				.Returns(trainingProviderType);
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();
			var serviceProviderViewModel = new SkillTrainingProviderDetailsViewModel()
			{
				Id = 0,
				RowVersion = "AgQGCAoMDhASFA==",
				GrantApplicationId = grantApplication.Id
			};
			var skillTrainingDetailsViewModel = new SkillTrainingDetailsViewModel()
			{
				Id = 0,
				RowVersion = "AgQGCAoMDhASFA==",
				GrantApplicationId = grantApplication.Id,
				TrainingProvider = serviceProviderViewModel
			};
			var skillTrainingViewModel = new SkillTrainingViewModel()
			{
				GrantApplicationId = grantApplication.Id,
				SkillTrainingDetails = skillTrainingDetailsViewModel,
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var controller = helper.Create();

			// Act
			var result = controller.AddSkillsTraining(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(skillTrainingViewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillTrainingViewModel>();
			var validationErrors = (result.Data as SkillTrainingViewModel).ValidationErrors;
			validationErrors.Count().Should().Be(21);
			validationErrors.Any(l => l.Key == "EligibleExpenseBreakdownId").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ServiceLineBreakdownId").Should().BeTrue();
			validationErrors.Any(l => l.Key == "StartDate").Should().BeTrue();
			validationErrors.Any(l => l.Key == "EndDate").Should().BeTrue();
			validationErrors.Any(l => l.Key == "CourseTitle").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TotalTrainingHours").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TitleOfQualification").Should().BeTrue();
			validationErrors.Any(l => l.Key == "SelectedDeliveryMethodIds").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ExpectedQualificationId").Should().BeTrue();
			validationErrors.Any(l => l.Key == "SkillLevelId").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TrainingOutsideBC").Should().BeTrue();
			validationErrors.Any(l => l.Key == "Name").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TrainingProviderTypeId").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactFirstName").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactLastName").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactEmail").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactPhone").Should().BeTrue();
			validationErrors.Any(l => l.Key == "AddressLine1").Should().BeTrue();
			validationErrors.Any(l => l.Key == "City").Should().BeTrue();
			validationErrors.Any(l => l.Key == "PostalCode").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TotalCost").Should().BeTrue();
			validationErrors.Any(l => l.Key == "OtherZipCode").Should().BeFalse();
			validationErrors.Any(l => l.Key == "OtherRegion").Should().BeFalse();

			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(1));
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Add(It.IsAny<TrainingProgram>()), Times.Never);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<IEligibleExpenseBreakdownService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderType(It.IsAny<int>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void AddSkillsTraining_OtherInValidModel()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var eligibleExpenseBreakdown = EntityHelper.CreateEligibleExpenseBreakdown(eligibleExpenseType);
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("Test", 1);
			trainingProviderType.PrivateSectorValidationType = TrainingProviderPrivateSectorValidationTypes.Always;
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<IEligibleExpenseBreakdownService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(eligibleExpenseBreakdown);
			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.Get<TrainingProviderType>(It.IsAny<int>()))
				.Returns(trainingProviderType);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetTrainingProviderType(It.IsAny<int>()))
				.Returns(trainingProviderType);
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();
			var serviceProviderViewModel = new SkillTrainingProviderDetailsViewModel()
			{
				Id = 0,
				RowVersion = "AgQGCAoMDhASFA==",
				GrantApplicationId = grantApplication.Id,
				IsCanadianAddress = false,
				TrainingOutsideBC = true,
				TrainingProviderTypeId = 3
			};
			var skillTrainingDetailsViewModel = new SkillTrainingDetailsViewModel()
			{
				Id = 0,
				RowVersion = "AgQGCAoMDhASFA==",
				GrantApplicationId = grantApplication.Id,
				TrainingProvider = serviceProviderViewModel,
				ExpectedQualificationId = 5
			};
			var skillTrainingViewModel = new SkillTrainingViewModel()
			{
				GrantApplicationId = grantApplication.Id,
				SkillTrainingDetails = skillTrainingDetailsViewModel,
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var controller = helper.Create();

			// Act
			var result = controller.AddSkillsTraining(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(skillTrainingViewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillTrainingViewModel>();
			var validationErrors = (result.Data as SkillTrainingViewModel).ValidationErrors;
			validationErrors.Count().Should().Be(21);
			validationErrors.Any(l => l.Key == "EligibleExpenseBreakdownId").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ServiceLineBreakdownId").Should().BeTrue();
			validationErrors.Any(l => l.Key == "StartDate").Should().BeTrue();
			validationErrors.Any(l => l.Key == "EndDate").Should().BeTrue();
			validationErrors.Any(l => l.Key == "CourseTitle").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TotalTrainingHours").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TitleOfQualification").Should().BeFalse();
			validationErrors.Any(l => l.Key == "ProofOfQualificationsDocument").Should().BeTrue();
			validationErrors.Any(l => l.Key == "SelectedDeliveryMethodIds").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ExpectedQualificationId").Should().BeFalse();
			validationErrors.Any(l => l.Key == "SkillLevelId").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TrainingOutsideBC").Should().BeFalse();
			validationErrors.Any(l => l.Key == "Name").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TrainingProviderTypeId").Should().BeFalse();
			validationErrors.Any(l => l.Key == "ContactFirstName").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactLastName").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactEmail").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactPhone").Should().BeTrue();
			validationErrors.Any(l => l.Key == "AddressLine1").Should().BeTrue();
			validationErrors.Any(l => l.Key == "City").Should().BeTrue();
			validationErrors.Any(l => l.Key == "BusinessCaseDocument").Should().BeTrue();
			validationErrors.Any(l => l.Key == "PostalCode").Should().BeFalse();
			validationErrors.Any(l => l.Key == "TotalCost").Should().BeTrue();
			validationErrors.Any(l => l.Key == "OtherZipCode").Should().BeTrue();
			validationErrors.Any(l => l.Key == "OtherRegion").Should().BeTrue();
			validationErrors.Any(l => l.Key == "CourseOutlineDocument").Should().BeTrue();

			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(1));
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Add(It.IsAny<TrainingProgram>()), Times.Never);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<IEligibleExpenseBreakdownService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderType(It.IsAny<int>()), Times.Never);
		}
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void AddSkillsTraining_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();

			var viewModel = new TrainingServiceProviderViewModel()
			{
				GrantApplicationId = grantApplication.Id
			};
			var controller = helper.Create();

			// Act
			var result = controller.AddSkillsTraining(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(viewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillTrainingViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion

		#region UpdateSkillsTraining
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void UpdateSkillsTraining()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("Test", 1);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			var trainingProvider = EntityHelper.CreateTrainingProvider(trainingProgram);
			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var eligibleExpenseBreakdown = EntityHelper.CreateEligibleExpenseBreakdown(eligibleExpenseType);
			var eligibleCostBreakdown = EntityHelper.CreateEligibleCostBreakdown(eligibleCost, eligibleExpenseBreakdown);
			trainingProgram.EligibleCostBreakdown = eligibleCostBreakdown;
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<IEligibleExpenseBreakdownService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(eligibleExpenseBreakdown);
			helper.GetMock<ITrainingProgramService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProgram);
			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProvider);
			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.Get<TrainingProviderType>(It.IsAny<int>()))
				.Returns(trainingProviderType);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetTrainingProviderType(It.IsAny<int>()))
				.Returns(trainingProviderType);
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();
			var serviceProviderViewModel = new SkillTrainingProviderDetailsViewModel(trainingProvider)
			{
				ContactPhone = "6041234567",
				ContactPhoneAreaCode = "604",
				ContactPhoneExchange = "123",
				ContactPhoneNumber = "4567",
				Name = "test",
				ChangeRequestReason = "reason",
				GrantApplicationId = grantApplication.Id,
				TrainingProviderInventory = trainingProviderInventory,
				TrainingProviderInventoryId = trainingProviderInventory.Id,
				PrivateSectorValidationType = TrainingProviderPrivateSectorValidationTypes.Never,
				TrainingProviderTypeId = 1,
				TrainingOutsideBC = false,
				ContactFirstName = "first name",
				ContactLastName = "Last Name",
				ContactEmail = "test@dfad.ca",
				AddressLine1 = "address",
				City = "city",
				PostalCode = "V9C9C9"
			};
			var skillTrainingDetailsViewModel = new SkillTrainingDetailsViewModel(trainingProgram)
			{
				DeliveryStartDate = AppDateTime.UtcNow,
				DeliveryEndDate = AppDateTime.UtcNow,
				StartDate = AppDateTime.UtcNow,
				StartYear = AppDateTime.UtcNow.Year,
				StartMonth = AppDateTime.UtcNow.Month,
				StartDay = AppDateTime.UtcNow.Day,
				EndDate = AppDateTime.UtcNow,
				EndYear = AppDateTime.UtcNow.Year,
				EndMonth = AppDateTime.UtcNow.Month,
				EndDay = AppDateTime.UtcNow.Day,
				CourseTitle = "CourseTitle Test",
				TotalTrainingHours = 23,
				TitleOfQualification = "TitleOfQualification test",
				SelectedDeliveryMethodIds = new int[] { 1 },
				ExpectedQualificationId = 1,
				SkillLevelId = 1,
				ServiceLineBreakdownCaption = "ServiceLineBreakdownCaption Test",
				TrainingProvider = serviceProviderViewModel,
				TotalCost = 4324,
				EligibleExpenseBreakdownId = 1,
				EligibleExpenseTypeId = eligibleExpenseType.Id,
			};
			var skillTrainingViewModel = new SkillTrainingViewModel(trainingProgram);
			var controller = helper.Create();

			// Act
			var result = controller.UpdateSkillsTraining(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(skillTrainingViewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillTrainingViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Update(It.IsAny<TrainingProgram>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IEligibleExpenseBreakdownService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(2));
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderType(It.IsAny<int>()), Times.Once);
			var model = result.Data as SkillTrainingViewModel;
			model.GrantApplicationId.Should().Be(grantApplication.Id);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void UpdateSkillsTraining_InValidModel()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var eligibleExpenseBreakdown = EntityHelper.CreateEligibleExpenseBreakdown(eligibleExpenseType);
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("Test", 1);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<IEligibleExpenseBreakdownService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(eligibleExpenseBreakdown);
			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.Get<TrainingProviderType>(It.IsAny<int>()))
				.Returns(trainingProviderType);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetTrainingProviderType(It.IsAny<int>()))
				.Returns(trainingProviderType);
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();
			var serviceProviderViewModel = new SkillTrainingProviderDetailsViewModel()
			{
				Id = 0,
				RowVersion = "AgQGCAoMDhASFA==",
				GrantApplicationId = grantApplication.Id
			};
			var skillTrainingDetailsViewModel = new SkillTrainingDetailsViewModel()
			{
				Id = 0,
				RowVersion = "AgQGCAoMDhASFA==",
				GrantApplicationId = grantApplication.Id,
				TrainingProvider = serviceProviderViewModel
			};
			var skillTrainingViewModel = new SkillTrainingViewModel()
			{
				GrantApplicationId = grantApplication.Id,
				SkillTrainingDetails = skillTrainingDetailsViewModel,
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateSkillsTraining(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(skillTrainingViewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillTrainingViewModel>();
			var validationErrors = (result.Data as SkillTrainingViewModel).ValidationErrors;
			validationErrors.Count().Should().Be(21);
			validationErrors.Any(l => l.Key == "EligibleExpenseBreakdownId").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ServiceLineBreakdownId").Should().BeTrue();
			validationErrors.Any(l => l.Key == "StartDate").Should().BeTrue();
			validationErrors.Any(l => l.Key == "EndDate").Should().BeTrue();
			validationErrors.Any(l => l.Key == "CourseTitle").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TotalTrainingHours").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TitleOfQualification").Should().BeTrue();
			validationErrors.Any(l => l.Key == "SelectedDeliveryMethodIds").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ExpectedQualificationId").Should().BeTrue();
			validationErrors.Any(l => l.Key == "SkillLevelId").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TrainingOutsideBC").Should().BeTrue();
			validationErrors.Any(l => l.Key == "Name").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TrainingProviderTypeId").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactFirstName").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactLastName").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactEmail").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactPhone").Should().BeTrue();
			validationErrors.Any(l => l.Key == "AddressLine1").Should().BeTrue();
			validationErrors.Any(l => l.Key == "City").Should().BeTrue();
			validationErrors.Any(l => l.Key == "PostalCode").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TotalCost").Should().BeTrue();
			validationErrors.Any(l => l.Key == "OtherZipCode").Should().BeFalse();
			validationErrors.Any(l => l.Key == "OtherRegion").Should().BeFalse();

			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(1));
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Update(It.IsAny<TrainingProgram>()), Times.Never);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<IEligibleExpenseBreakdownService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderType(It.IsAny<int>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void UpdateSkillsTraining_OtherInValidModel()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var eligibleExpenseBreakdown = EntityHelper.CreateEligibleExpenseBreakdown(eligibleExpenseType);
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("Test", 1);
			trainingProviderType.PrivateSectorValidationType = TrainingProviderPrivateSectorValidationTypes.Always;
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<IEligibleExpenseBreakdownService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(eligibleExpenseBreakdown);
			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.Get<TrainingProviderType>(It.IsAny<int>()))
				.Returns(trainingProviderType);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetTrainingProviderType(It.IsAny<int>()))
				.Returns(trainingProviderType);
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();
			var serviceProviderViewModel = new SkillTrainingProviderDetailsViewModel()
			{
				Id = 0,
				RowVersion = "AgQGCAoMDhASFA==",
				GrantApplicationId = grantApplication.Id,
				IsCanadianAddress = false,
				TrainingOutsideBC = true,
				TrainingProviderTypeId = 3
			};
			var skillTrainingDetailsViewModel = new SkillTrainingDetailsViewModel()
			{
				Id = 0,
				RowVersion = "AgQGCAoMDhASFA==",
				GrantApplicationId = grantApplication.Id,
				TrainingProvider = serviceProviderViewModel,
				ExpectedQualificationId = 5
			};
			var skillTrainingViewModel = new SkillTrainingViewModel()
			{
				GrantApplicationId = grantApplication.Id,
				SkillTrainingDetails = skillTrainingDetailsViewModel,
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateSkillsTraining(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(skillTrainingViewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillTrainingViewModel>();
			var validationErrors = (result.Data as SkillTrainingViewModel).ValidationErrors;
			validationErrors.Count().Should().Be(21);
			validationErrors.Any(l => l.Key == "EligibleExpenseBreakdownId").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ServiceLineBreakdownId").Should().BeTrue();
			validationErrors.Any(l => l.Key == "StartDate").Should().BeTrue();
			validationErrors.Any(l => l.Key == "EndDate").Should().BeTrue();
			validationErrors.Any(l => l.Key == "CourseTitle").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TotalTrainingHours").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TitleOfQualification").Should().BeFalse();
			validationErrors.Any(l => l.Key == "ProofOfQualificationsDocument").Should().BeTrue();
			validationErrors.Any(l => l.Key == "SelectedDeliveryMethodIds").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ExpectedQualificationId").Should().BeFalse();
			validationErrors.Any(l => l.Key == "SkillLevelId").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TrainingOutsideBC").Should().BeFalse();
			validationErrors.Any(l => l.Key == "Name").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TrainingProviderTypeId").Should().BeFalse();
			validationErrors.Any(l => l.Key == "ContactFirstName").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactLastName").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactEmail").Should().BeTrue();
			validationErrors.Any(l => l.Key == "ContactPhone").Should().BeTrue();
			validationErrors.Any(l => l.Key == "AddressLine1").Should().BeTrue();
			validationErrors.Any(l => l.Key == "City").Should().BeTrue();
			validationErrors.Any(l => l.Key == "BusinessCaseDocument").Should().BeTrue();
			validationErrors.Any(l => l.Key == "PostalCode").Should().BeFalse();
			validationErrors.Any(l => l.Key == "TotalCost").Should().BeTrue();
			validationErrors.Any(l => l.Key == "OtherZipCode").Should().BeTrue();
			validationErrors.Any(l => l.Key == "OtherRegion").Should().BeTrue();
			validationErrors.Any(l => l.Key == "CourseOutlineDocument").Should().BeTrue();

			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Exactly(1));
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Update(It.IsAny<TrainingProgram>()), Times.Never);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<IEligibleExpenseBreakdownService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingProviderType(It.IsAny<int>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void UpdateSkillsTraining_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();

			var viewModel = new TrainingServiceProviderViewModel()
			{
				GrantApplicationId = grantApplication.Id
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateSkillsTraining(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(viewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillTrainingViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion

		#region GetServiceLines
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void GetServiceLines()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var eligibleExpenseBreakdown = EntityHelper.CreateEligibleExpenseBreakdown(eligibleExpenseType);
			var serviceLine = EntityHelper.CreateServiceLine();
			eligibleExpenseBreakdown.ServiceLine = serviceLine;
			helper.GetMock<IEligibleExpenseBreakdownService>()
				.Setup(m => m.GetAllActiveForEligibleExpenseType(It.IsAny<int>()))
				.Returns(new List<EligibleExpenseBreakdown>() { eligibleExpenseBreakdown });

			var controller = helper.Create();

			// Act
			var result = controller.GetServiceLines(eligibleExpenseType.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<ServiceLineListModel>>();
			var model = result.Data as List<ServiceLineListModel>;
			model[0].Id.Should().Be(eligibleExpenseBreakdown.Id);
			model[0].Caption.Should().Be(eligibleExpenseBreakdown.Caption);
			helper.GetMock<IEligibleExpenseBreakdownService>().Verify(m => m.GetAllActiveForEligibleExpenseType(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void GetServiceLines_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			helper.GetMock<IEligibleExpenseBreakdownService>()
				.Setup(m => m.GetAllActiveForEligibleExpenseType(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetServiceLines(eligibleExpenseType.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<ServiceLineListModel>>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IEligibleExpenseBreakdownService>().Verify(m => m.GetAllActiveForEligibleExpenseType(It.IsAny<int>()), Times.Once);
		}
		#endregion

		#region GetServiceLineBreakdowns
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void GetServiceLineBreakdowns()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var eligibleExpenseBreakdown = EntityHelper.CreateEligibleExpenseBreakdown(eligibleExpenseType);
			var serviceLineBreakdown = EntityHelper.CreateServiceLineBreakdown();
			var serviceLine = EntityHelper.CreateServiceLine();
			eligibleExpenseBreakdown.ServiceLine = serviceLine;
			eligibleExpenseBreakdown.ServiceLineId = serviceLine.Id;

			helper.GetMock<IEligibleExpenseBreakdownService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(eligibleExpenseBreakdown);
			helper.GetMock<IServiceLineBreakdownService>()
				.Setup(m => m.GetAllForServiceLine(It.IsAny<int>(), It.IsAny<bool>()))
				.Returns(new List<ServiceLineBreakdown>() { serviceLineBreakdown });

			var controller = helper.Create();

			// Act
			var result = controller.GetServiceLineBreakdowns(eligibleExpenseBreakdown.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<CollectionItemModel>>();
			helper.GetMock<IEligibleExpenseBreakdownService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IServiceLineBreakdownService>().Verify(m => m.GetAllForServiceLine(It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void GetServiceLineBreakdowns_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var eligibleExpenseBreakdown = EntityHelper.CreateEligibleExpenseBreakdown(eligibleExpenseType);
			helper.GetMock<IEligibleExpenseBreakdownService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();

			var controller = helper.Create();

			// Act
			var result = controller.GetServiceLineBreakdowns(eligibleExpenseBreakdown.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<CollectionItemModel>>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion
	}
}

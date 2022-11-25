using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Ext.Controllers;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Areas.Ext.Models.TrainingPrograms;
using CJG.Web.External.Areas.Ext.Models.Applications;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace CJG.Testing.UnitTests.Controllers.Ext
{
	[TestClass]
	public class TrainingProgramControllerTest
	{

		#region TrainingProgramView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void TrainingProgramView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProgramController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.TrainingProgramView(grantApplication.Id, trainingProgram.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			trainingProgram.Id.Should().Be(controller.ViewBag.TrainingProgramId);
		}
		#endregion

		#region GetTrainingProgram
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void GetTrainingProgram()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProgramController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.TrainingPrograms = new List<TrainingProgram>();
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingProgram(grantApplication.Id, 0);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingProgramViewModel>();
			var model = result.Data as TrainingProgramViewModel;
			model.GrantApplicationId.Should().Be(grantApplication.Id);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void GetTrainingProgram_ExistTrainingProgram()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProgramController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<ITrainingProgramService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProgram);
			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingProgram(grantApplication.Id, trainingProgram.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingProgramViewModel>();
			var model = result.Data as TrainingProgramViewModel;
			model.Id.Should().Be(trainingProgram.Id);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void GetTrainingProgram_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProgramController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingProgram(1, 0);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingProgramViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void GetTrainingProgram_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProgramController>(user);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NoContentException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingProgram(1, 1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingProgramViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
		}
		#endregion

		#region AddTrainingProgram
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void AddTrainingProgram()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProgramController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<ITrainingProgramService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProgram);
			var trainingProgramViewModel = new TrainingProgramViewModel()
			{
				GrantApplicationRowVersion = "AgQGCAoMDhASFA==",
				RowVersion = "AgQGCAoMDhASFA==",
				GrantApplicationId = grantApplication.Id,
				DeliveryStartDate = AppDateTime.Now,
				DeliveryEndDate = AppDateTime.Now.AddMonths(6),
				StartDate = AppDateTime.Now,
				StartYear = AppDateTime.Now.Year,
				StartMonth = AppDateTime.Now.Month,
				StartDay = AppDateTime.Now.Day,
				EndDate = AppDateTime.Now.AddMonths(6),
				EndYear = AppDateTime.Now.AddMonths(6).Year,
				EndMonth = AppDateTime.Now.AddMonths(6).Month,
				EndDay = AppDateTime.Now.AddMonths(6).Day,
				CourseTitle = "Test",
				TotalTrainingHours = 1,
				SelectedDeliveryMethodIds = new int[] { 1 },
				ExpectedQualificationId = 1,
				SkillLevelId = 1,
				InDemandOccupationId = 1,
				SkillFocusId = 1,
				TrainingLevelId = 1,
				TrainingBusinessCase = "",
				HasOfferedThisTypeOfTrainingBefore = false,
				HasRequestedAdditionalFunding = false,
				DescriptionOfFundingRequested = "",
				MemberOfUnderRepresentedGroup = false,
				SelectedUnderRepresentedGroupIds = new int[] { 1 }
			};
			var controller = helper.Create();

			var jsonModel = Json.Encode(trainingProgramViewModel);
			// Act
			var result = controller.AddTrainingProgram(new HttpPostedFileBase[]{}, jsonModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingProgramViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Add(It.IsAny<TrainingProgram>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void AddTrainingProgram_InvalidModel()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProgramController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<ITrainingProgramService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProgram);
			var trainingProgramViewModel = new TrainingProgramViewModel()
			{
				GrantApplicationRowVersion = "AgQGCAoMDhASFA==",
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var controller = helper.Create();

			var jsonModel = Json.Encode(trainingProgramViewModel);
			// Act
			var result = controller.AddTrainingProgram(new HttpPostedFileBase[] { }, jsonModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingProgramViewModel>();
			var validationErrors = (result.Data as TrainingProgramViewModel).ValidationErrors;
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Add(It.IsAny<TrainingProgram>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void AddSkillsTraining_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var helper = new ControllerHelper<TrainingProgramController>(user);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();

			var trainingProgramViewModel = new TrainingProgramViewModel()
			{
				GrantApplicationRowVersion = "AgQGCAoMDhASFA==",
				RowVersion = "AgQGCAoMDhASFA==",
				GrantApplicationId = grantApplication.Id,
				DeliveryStartDate = AppDateTime.Now,
				DeliveryEndDate = AppDateTime.Now.AddMonths(6),
				StartDate = AppDateTime.Now,
				StartYear = AppDateTime.Now.Year,
				StartMonth = AppDateTime.Now.Month,
				StartDay = AppDateTime.Now.Day,
				EndDate = AppDateTime.Now.AddMonths(6),
				EndYear = AppDateTime.Now.AddMonths(6).Year,
				EndMonth = AppDateTime.Now.AddMonths(6).Month,
				EndDay = AppDateTime.Now.AddMonths(6).Day,
				CourseTitle = "Test",
				TotalTrainingHours = 1,
				SelectedDeliveryMethodIds = new int[] { 1 },
				ExpectedQualificationId = 1,
				SkillLevelId = 1,
				InDemandOccupationId = 1,
				SkillFocusId = 1,
				TrainingLevelId = 1,
				TrainingBusinessCase = "",
				HasOfferedThisTypeOfTrainingBefore = false,
				HasRequestedAdditionalFunding = false,
				DescriptionOfFundingRequested = "",
				MemberOfUnderRepresentedGroup = false,
				SelectedUnderRepresentedGroupIds = new int[] { 1 }
			};
			var controller = helper.Create();

			var jsonModel = Json.Encode(trainingProgramViewModel);
			// Act
			var result = controller.AddTrainingProgram(new HttpPostedFileBase[] { }, jsonModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingProgramViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion

		#region UpdateTrainingProgram
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void UpdateTrainingProgram()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProgramController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			var deliveryMethod = EntityHelper.CreateDeliveryMethod("test");

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<ITrainingProgramService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProgram);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetDeliveryMethod(It.IsAny<int>()))
				.Returns(deliveryMethod);
			var trainingProgramViewModel = new TrainingProgramViewModel()
			{
				Id = trainingProgram.Id,
				GrantApplicationRowVersion = "AgQGCAoMDhASFA==",
				RowVersion = "AgQGCAoMDhASFA==",
				GrantApplicationId = grantApplication.Id,
				DeliveryStartDate = AppDateTime.Now,
				DeliveryEndDate = AppDateTime.Now.AddMonths(6),
				StartDate = AppDateTime.Now,
				StartYear = AppDateTime.Now.Year,
				StartMonth = AppDateTime.Now.Month,
				StartDay = AppDateTime.Now.Day,
				EndDate = AppDateTime.Now.AddMonths(6),
				EndYear = AppDateTime.Now.AddMonths(6).Year,
				EndMonth = AppDateTime.Now.AddMonths(6).Month,
				EndDay = AppDateTime.Now.AddMonths(6).Day,
				CourseTitle = "Test",
				TotalTrainingHours = 1,
				SelectedDeliveryMethodIds = new int[] { deliveryMethod.Id },
				ExpectedQualificationId = 1,
				SkillLevelId = 1,
				InDemandOccupationId = 1,
				SkillFocusId = 1,
				TrainingLevelId = 1,
				TrainingBusinessCase = "",
				HasOfferedThisTypeOfTrainingBefore = false,
				HasRequestedAdditionalFunding = false,
				DescriptionOfFundingRequested = "",
				MemberOfUnderRepresentedGroup = false,
				SelectedUnderRepresentedGroupIds = new int[] { 1 }
			};
			var controller = helper.Create();

			var jsonModel = Json.Encode(trainingProgramViewModel);

			// Act
			var result = controller.UpdateTrainingProgram(new HttpPostedFileBase[] { }, jsonModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingProgramViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Update(It.IsAny<TrainingProgram>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void UpdateTrainingProgram_InvalidModel()
		{
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProgramController>(user);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);

			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			helper.GetMock<ITrainingProgramService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProgram);
			var trainingProgramViewModel = new TrainingProgramViewModel()
			{
				GrantApplicationRowVersion = "AgQGCAoMDhASFA==",
				RowVersion = "AgQGCAoMDhASFA=="
			};
			var controller = helper.Create();

			var jsonModel = Json.Encode(trainingProgramViewModel);

			// Act
			var result = controller.UpdateTrainingProgram(new HttpPostedFileBase[] { }, jsonModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingProgramViewModel>();
			var validationErrors = (result.Data as TrainingProgramViewModel).ValidationErrors;
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Update(It.IsAny<TrainingProgram>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void UpdateTrainingProgram_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var helper = new ControllerHelper<TrainingProgramController>(user);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();

			var trainingProgramViewModel = new TrainingProgramViewModel()
			{
				GrantApplicationRowVersion = "AgQGCAoMDhASFA==",
				RowVersion = "AgQGCAoMDhASFA==",
				GrantApplicationId = grantApplication.Id,
				DeliveryStartDate = AppDateTime.Now,
				DeliveryEndDate = AppDateTime.Now.AddMonths(6),
				StartDate = AppDateTime.Now,
				StartYear = AppDateTime.Now.Year,
				StartMonth = AppDateTime.Now.Month,
				StartDay = AppDateTime.Now.Day,
				EndDate = AppDateTime.Now.AddMonths(6),
				EndYear = AppDateTime.Now.AddMonths(6).Year,
				EndMonth = AppDateTime.Now.AddMonths(6).Month,
				EndDay = AppDateTime.Now.AddMonths(6).Day,
				CourseTitle = "Test",
				TotalTrainingHours = 1,
				SelectedDeliveryMethodIds = new int[] { 1 },
				ExpectedQualificationId = 1,
				SkillLevelId = 1,
				InDemandOccupationId = 1,
				SkillFocusId = 1,
				TrainingLevelId = 1,
				TrainingBusinessCase = "",
				HasOfferedThisTypeOfTrainingBefore = false,
				HasRequestedAdditionalFunding = false,
				DescriptionOfFundingRequested = "",
				MemberOfUnderRepresentedGroup = false,
				SelectedUnderRepresentedGroupIds = new int[] { 1 }
			};
			var controller = helper.Create();

			var jsonModel = Json.Encode(trainingProgramViewModel);

			// Act
			var result = controller.UpdateTrainingProgram(new HttpPostedFileBase[] { }, jsonModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingProgramViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion

		#region GetSkillLevels
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void GetSkillLevels()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProgramController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var skillLevel = EntityHelper.CreateSkillLevel("test");
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetSkillLevels())
				.Returns(new List<SkillLevel>() { skillLevel });
			var controller = helper.Create();

			// Act
			var result = controller.GetSkillLevels();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<CollectionItemModel>>();
			var model = result.Data as List<CollectionItemModel>;
			model.Count().Should().Be(1);
			model[0].Caption.Should().Be(skillLevel.Caption);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetSkillLevels(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void GetSkillLevels_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProgramController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetSkillLevels())
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetSkillLevels();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<CollectionItemModel>>();
			var model = result.Data as List<CollectionItemModel>;
			model.Count().Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetSkillLevels(), Times.Once);
		}
		#endregion

		#region GetSkillsFocuses
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void GetSkillsFocuses()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProgramController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var skillsFocuses = EntityHelper.CreateSkillFocus("test");
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetSkillsFocuses())
				.Returns(new List<SkillsFocus>() { skillsFocuses });
			var controller = helper.Create();

			// Act
			var result = controller.GetSkillsFocuses();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<CollectionItemModel>>();
			var model = result.Data as List<CollectionItemModel>;
			model.Count().Should().Be(1);
			model[0].Caption.Should().Be(skillsFocuses.Caption);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetSkillsFocuses(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void GetSkillsFocuses_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProgramController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetSkillsFocuses())
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetSkillsFocuses();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<CollectionItemModel>>();
			var model = result.Data as List<CollectionItemModel>;
			model.Count().Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetSkillsFocuses(), Times.Once);
		}
		#endregion

		#region GetExpectedQualifications
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void GetExpectedQualifications()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProgramController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var expectedQualification = EntityHelper.CreateExpectedQualification("test");
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetExpectedQualifications())
				.Returns(new List<ExpectedQualification>() { expectedQualification });
			var controller = helper.Create();

			// Act
			var result = controller.GetExpectedQualifications();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<CollectionItemModel>>();
			var model = result.Data as List<CollectionItemModel>;
			model.Count().Should().Be(1);
			model[0].Caption.Should().Be(expectedQualification.Caption);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetExpectedQualifications(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void GetExpectedQualifications_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProgramController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetExpectedQualifications())
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetExpectedQualifications();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<CollectionItemModel>>();
			var model = result.Data as List<CollectionItemModel>;
			model.Count().Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetExpectedQualifications(), Times.Once);
		}
		#endregion

		#region GetInDemandOccupations
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void GetInDemandOccupations()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProgramController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var inDemandOccupation = EntityHelper.CreateInDemandOccupation("test");
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetInDemandOccupations())
				.Returns(new List<InDemandOccupation>() { inDemandOccupation });
			var controller = helper.Create();

			// Act
			var result = controller.GetInDemandOccupations();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<CollectionItemModel>>();
			var model = result.Data as List<CollectionItemModel>;
			model.Count().Should().Be(1);
			model[0].Caption.Should().Be(inDemandOccupation.Caption);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetInDemandOccupations(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void GetInDemandOccupations_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProgramController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetInDemandOccupations())
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetInDemandOccupations();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<CollectionItemModel>>();
			var model = result.Data as List<CollectionItemModel>;
			model.Count().Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetInDemandOccupations(), Times.Once);
		}
		#endregion

		#region GetTrainingLevels
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void GetTrainingLevels()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProgramController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingLevel = EntityHelper.CreateTrainingLevel("test");
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetTrainingLevels())
				.Returns(new List<TrainingLevel>() { trainingLevel });
			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingLevels();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<CollectionItemModel>>();
			var model = result.Data as List<CollectionItemModel>;
			model.Count().Should().Be(1);
			model[0].Caption.Should().Be(trainingLevel.Caption);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingLevels(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void GetTrainingLevels_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProgramController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetTrainingLevels())
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingLevels();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<CollectionItemModel>>();
			var model = result.Data as List<CollectionItemModel>;
			model.Count().Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetTrainingLevels(), Times.Once);
		}
		#endregion

		#region GetUnderrepresentedGroups
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void GetUnderrepresentedGroups()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProgramController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var underRepresentedGroup = EntityHelper.CreateUnderRepresentedGroup("test");
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetUnderRepresentedGroups())
				.Returns(new List<UnderRepresentedGroup>() { underRepresentedGroup });
			var controller = helper.Create();

			// Act
			var result = controller.GetUnderrepresentedGroups();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<CollectionItemModel>>();
			var model = result.Data as List<CollectionItemModel>;
			model.Count().Should().Be(1);
			model[0].Caption.Should().Be(underRepresentedGroup.Caption);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetUnderRepresentedGroups(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void GetUnderrepresentedGroups_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProgramController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetUnderRepresentedGroups())
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetUnderrepresentedGroups();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<CollectionItemModel>>();
			var model = result.Data as List<CollectionItemModel>;
			model.Count().Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetUnderRepresentedGroups(), Times.Once);
		}
		#endregion

		#region GetDeliveryMethods
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void GetDeliveryMethods()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProgramController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var deliveryMethod = EntityHelper.CreateDeliveryMethod("test");
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetDeliveryMethods())
				.Returns(new List<DeliveryMethod>() { deliveryMethod });
			var controller = helper.Create();

			// Act
			var result = controller.GetDeliveryMethods();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<CollectionItemModel>>();
			var model = result.Data as List<CollectionItemModel>;
			model.Count().Should().Be(1);
			model[0].Caption.Should().Be(deliveryMethod.Caption);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetDeliveryMethods(), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void GetDeliveryMethods_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<TrainingProgramController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IStaticDataService>()
				.Setup(m => m.GetDeliveryMethods())
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetDeliveryMethods();

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<List<CollectionItemModel>>();
			var model = result.Data as List<CollectionItemModel>;
			model.Count().Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetDeliveryMethods(), Times.Once);
		}
		#endregion
	}
}
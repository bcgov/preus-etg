using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Areas.Int.Controllers;
using CJG.Web.External.Areas.Int.Models.SkillsTraining;
using CJG.Web.External.Areas.Int.Models.TrainingProviders;
using CJG.Web.External.Models.Shared;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static CJG.Testing.Core.ServiceHelper;

namespace CJG.Testing.UnitTests.Controllers.Int
{
	[TestClass]
	public class SkillsTrainingControllerTest
	{
		#region Training Program
		#region GetTrainingProgram
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void GetTrainingProgram()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user, Roles.Assessor);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var eligibleExpenseBreakdown = EntityHelper.CreateEligibleExpenseBreakdown(eligibleExpenseType);
			var eligibleCostBreakdown = EntityHelper.CreateEligibleCostBreakdown(eligibleCost, eligibleExpenseBreakdown);
			trainingProgram.ServiceLineId = 1;
			trainingProgram.EligibleCostBreakdown = eligibleCostBreakdown;
			trainingProgram.EligibleCostBreakdownId = eligibleCostBreakdown.Id;

			helper.GetMock<ITrainingProgramService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProgram);
			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingProgram(trainingProgram.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillsTrainingProgramViewModel>();
			var model = result.Data as SkillsTrainingProgramViewModel;
			model.GrantApplicationId.Should().Be(trainingProgram.GrantApplication.Id);
			model.CourseTitle.Should().Be(trainingProgram.CourseTitle);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void GetTrainingProgram_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user, Roles.Assessor);

			helper.GetMock<ITrainingProgramService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingProgram(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillsTrainingProgramViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void GetTrainingProgram_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user, Roles.Assessor);

			helper.GetMock<ITrainingProgramService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NoContentException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingProgram(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillsTrainingProgramViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}
		#endregion

		#region UpdateTrainingProgram
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void UpdateTrainingProgram()
		{
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var serviceLine = EntityHelper.CreateServiceLine();
			var serviceLineBreakdown = EntityHelper.CreateServiceLineBreakdown();
			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var eligibleExpenseBreakdown = EntityHelper.CreateEligibleExpenseBreakdown(eligibleExpenseType);
			eligibleExpenseBreakdown.ServiceLine = serviceLine;
			eligibleExpenseBreakdown.ServiceLineId = serviceLine.Id;
			eligibleCost.EligibleExpenseType = eligibleExpenseType;
			var eligibleCostBreakdown = EntityHelper.CreateEligibleCostBreakdown(eligibleCost, eligibleExpenseBreakdown);
			eligibleExpenseType.Breakdowns = new List<EligibleExpenseBreakdown>() { eligibleExpenseBreakdown };
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			trainingProgram.ServiceLineId = serviceLine.Id;
			trainingProgram.EligibleCostBreakdown = eligibleCostBreakdown;
			trainingProgram.EligibleCostBreakdownId = eligibleCostBreakdown.Id;

			helper.GetMock<ITrainingProgramService>()
							.Setup(m => m.Get(It.IsAny<int>()))
							.Returns(trainingProgram);

			var skillsTrainingProgramViewModel = new SkillsTrainingProgramViewModel()
			{
				CanEdit = true,
				CanRemove = true,
				RowVersion = "AgQGCAoMDhASFA==",
				GrantApplicationId = grantApplication.Id,
				ServiceLineId = serviceLine.Id,
				ServiceLineBreakdownId = serviceLineBreakdown.Id,
				EligibleCostId = eligibleCost.Id,
				EligibleCostBreakdownId = eligibleCostBreakdown.Id,
				EligibleCostBreakdownRowVersion = Convert.ToBase64String(eligibleCostBreakdown.RowVersion),
				StartDate = DateTime.UtcNow,
				EndDate = DateTime.UtcNow.AddMonths(3),
				CourseTitle = "CourseTitle",
				TotalTrainingHours = 10,
				TitleOfQualification = "TitleOfQualification",
				SelectedDeliveryMethodIds = new int[] { 1, 2, 3},
				ExpectedQualificationId = 1,
				SkillLevelId = 1,
				EstimatedCost = 23,
				AgreedCost = 23
			};

			var controller = helper.Create();

			// Act
			var result = controller.UpdateTrainingProgram(skillsTrainingProgramViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillsTrainingProgramViewModel>();
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.UpdateDeliveryMethods(It.IsAny<TrainingProgram>(), It.IsAny<int[]>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Update(It.IsAny<TrainingProgram>()), Times.Once);

		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void UpdateTrainingProgram_InvalidModel()
		{
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			helper.GetMock<ITrainingProgramService>()
							.Setup(m => m.Get(It.IsAny<int>()))
							.Returns(trainingProgram);

			var skillsTrainingProgramViewModel = new SkillsTrainingProgramViewModel()
			{
				Id = 1
			};
			var controller = helper.Create();
			controller.ModelState.AddModelError("EstimatedCost", "EstimatedCost is required");
			// Act
			var result = controller.UpdateTrainingProgram(skillsTrainingProgramViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillsTrainingProgramViewModel>();
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.UpdateDeliveryMethods(It.IsAny<TrainingProgram>(), It.IsAny<int[]>()), Times.Never);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Update(It.IsAny<TrainingProgram>()), Times.Never);
			var validationErrors = (result.Data as SkillsTrainingProgramViewModel).ValidationErrors;
			validationErrors.Count().Should().Be(1);
			validationErrors.Any(l => l.Key == "EstimatedCost").Should().BeTrue();
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void UpdateTrainingProgram_NotAuthorized()
		{
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			helper.GetMock<ITrainingProgramService>()
							.Setup(m => m.Get(It.IsAny<int>()))
							.Throws<NotAuthorizedException>();

			var skillsTrainingProgramViewModel = new SkillsTrainingProgramViewModel()
			{
				Id = 1
			};
			var controller = helper.Create();
			// Act
			var result = controller.UpdateTrainingProgram(skillsTrainingProgramViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillsTrainingProgramViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.UpdateDeliveryMethods(It.IsAny<TrainingProgram>(), It.IsAny<int[]>()), Times.Never);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Update(It.IsAny<TrainingProgram>()), Times.Never);
		}
		#endregion

		#region IsEligible
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void IsEligible()
		{
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var serviceLine = EntityHelper.CreateServiceLine();
			var serviceLineBreakdown = EntityHelper.CreateServiceLineBreakdown();
			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var eligibleExpenseBreakdown = EntityHelper.CreateEligibleExpenseBreakdown(eligibleExpenseType);
			eligibleExpenseBreakdown.ServiceLine = serviceLine;
			eligibleExpenseBreakdown.ServiceLineId = serviceLine.Id;
			eligibleCost.EligibleExpenseType = eligibleExpenseType;
			var eligibleCostBreakdown = EntityHelper.CreateEligibleCostBreakdown(eligibleCost, eligibleExpenseBreakdown);
			eligibleExpenseType.Breakdowns = new List<EligibleExpenseBreakdown>() { eligibleExpenseBreakdown };
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			trainingProgram.ServiceLineId = serviceLine.Id;
			trainingProgram.EligibleCostBreakdown = eligibleCostBreakdown;
			trainingProgram.EligibleCostBreakdownId = eligibleCostBreakdown.Id;

			helper.GetMock<ITrainingProgramService>()
							.Setup(m => m.Get(It.IsAny<int>()))
							.Returns(trainingProgram);


			var controller = helper.Create();

			// Act
			var result = controller.IsEligible(trainingProgram.Id, "AgQGCAoMDhASFA==");

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillsTrainingProgramViewModel>();
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.ChangeEligibility(It.IsAny<TrainingProgram>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void IsEligible_NotAuthorized()
		{
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			helper.GetMock<ITrainingProgramService>()
							.Setup(m => m.Get(It.IsAny<int>()))
							.Throws<NotAuthorizedException>();

			var controller = helper.Create();
			// Act
			var result = controller.IsEligible(trainingProgram.Id, "AgQGCAoMDhASFA==");

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillsTrainingProgramViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.ChangeEligibility(It.IsAny<TrainingProgram>()), Times.Never);
		}
		#endregion

		#region AddComponent
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void AddComponent()
		{
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var serviceLine = EntityHelper.CreateServiceLine();
			var serviceLineBreakdown = EntityHelper.CreateServiceLineBreakdown();
			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var eligibleExpenseBreakdown = EntityHelper.CreateEligibleExpenseBreakdown(eligibleExpenseType);
			eligibleExpenseBreakdown.ServiceLine = serviceLine;
			eligibleExpenseBreakdown.ServiceLineId = serviceLine.Id;
			eligibleCost.EligibleExpenseType = eligibleExpenseType;
			var eligibleCostBreakdown = EntityHelper.CreateEligibleCostBreakdown(eligibleCost, eligibleExpenseBreakdown);
			eligibleExpenseType.Breakdowns = new List<EligibleExpenseBreakdown>() { eligibleExpenseBreakdown };
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			trainingProgram.ServiceLineId = serviceLine.Id;
			trainingProgram.EligibleCostBreakdown = eligibleCostBreakdown;
			trainingProgram.EligibleCostBreakdownId = eligibleCostBreakdown.Id;
			var trainigProviderType = EntityHelper.CreateTrainingProviderType("TrainingProviderType");
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();
			var region = EntityHelper.CreateRegion("test");
			var country = EntityHelper.CreateCountry("country test");
			grantApplication.TrainingPrograms.Add(trainingProgram);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			helper.GetMock<IEligibleCostService>().Setup(m => m.Get(It.IsAny<int>())).Returns(eligibleCost);
			helper.GetMock<IEligibleExpenseBreakdownService>().Setup(m => m.GetForServiceLine(It.IsAny<int>())).Returns(eligibleExpenseBreakdown);
			helper.GetMock<ITrainingProgramService>().Setup(m => m.Get(It.IsAny<int>())).Returns(trainingProgram);
			helper.GetMock<IStaticDataService>().Setup(m => m.GetRegion(It.IsAny<string>(), It.IsAny<string>())).Returns(region);
			helper.GetMock<IStaticDataService>().Setup(m => m.GetCountry(It.IsAny<string>())).Returns(country);

			var updateSkillsTrainingProviderViewModel = new UpdateSkillsTrainingProviderViewModel()
			{
				RowVersion = "AgQGCAoMDhASFA==",
				EligibleCostId = eligibleCost.Id,
				Name = "Name",
				ChangeRequestReason = "ChangeRequestReason",
				TrainingProgramId = trainingProgram.Id,
				TrainingProviderTypeId = trainigProviderType.Id,
				TrainingProviderInventoryId = trainingProviderInventory.Id,
				TrainingProviderState = TrainingProviderStates.Complete,
				ContactFirstName = "first name",
				ContactLastName = "Last Name",
				ContactEmail = "test@dfad.ca",
				ContactPhoneAreaCode = "604",
				ContactPhoneExchange = "123",
				ContactPhoneNumber = "4567",
				ContactPhoneExtension = "4567",
				AddressLine1 = "address",
				City = "city",
				PostalCode = "V9C9C9",
				TrainingAddressId = 1,
				CountryId = CJG.Core.Entities.Constants.CanadaCountryId
			};
			var updateSkillsTrainingProgramViewModel = new UpdateSkillsTrainingProgramViewModel()
			{
				GrantApplicationId = grantApplication.Id,
				ServiceLineId = serviceLine.Id,
				ServiceLineBreakdownId = serviceLineBreakdown.Id,
				EligibleCostId = eligibleCost.Id,
				StartDate = DateTime.UtcNow,
				EndDate = DateTime.UtcNow.AddMonths(3),
				CourseTitle = "CourseTitle",
				TotalTrainingHours = 10,
				TitleOfQualification = "TitleOfQualification",
				SelectedDeliveryMethodIds = new int[] { 1, 2, 3 },
				ExpectedQualificationId = 1,
				SkillLevelId = 1,
				AgreedCost = 23,
				TrainingProvider = updateSkillsTrainingProviderViewModel
			};
			var controller = helper.Create();

			// Act
			var result = controller.AddComponent(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(updateSkillsTrainingProgramViewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillsTrainingProgramViewModel>();
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IEligibleCostService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.UpdateDeliveryMethods(It.IsAny<TrainingProgram>(), It.IsAny<int[]>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Add(It.IsAny<TrainingProgram>()), Times.Once);

		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void AddComponent_NotAuthorized()
		{
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var updateSkillsTrainingProviderViewModel = new UpdateSkillsTrainingProviderViewModel()
			{
				RowVersion = "AgQGCAoMDhASFA==",
				Name = "Name",
				ChangeRequestReason = "ChangeRequestReason",
				TrainingProgramId = trainingProgram.Id,
				TrainingProviderState = TrainingProviderStates.Complete,
				ContactFirstName = "first name",
				ContactLastName = "Last Name",
				ContactEmail = "test@dfad.ca",
				ContactPhoneAreaCode = "604",
				ContactPhoneExchange = "123",
				ContactPhoneNumber = "4567",
				ContactPhoneExtension = "4567",
				AddressLine1 = "address",
				City = "city",
				PostalCode = "V9C9C9",
				TrainingAddressId = 1,
				CountryId = CJG.Core.Entities.Constants.CanadaCountryId
			};
			var updateSkillsTrainingProgramViewModel = new UpdateSkillsTrainingProgramViewModel()
			{
				GrantApplicationId = grantApplication.Id,
				TrainingProvider = updateSkillsTrainingProviderViewModel
			};
			var controller = helper.Create();
			// Act
			var result = controller.AddComponent(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(updateSkillsTrainingProgramViewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillsTrainingProgramViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IEligibleCostService>().Verify(m => m.Get(It.IsAny<int>()), Times.Never);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.UpdateDeliveryMethods(It.IsAny<TrainingProgram>(), It.IsAny<int[]>()), Times.Never);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Add(It.IsAny<TrainingProgram>()), Times.Never);
		}
		#endregion

		#region DeleteComponent
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void DeleteComponent()
		{
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var serviceLine = EntityHelper.CreateServiceLine();
			var serviceLineBreakdown = EntityHelper.CreateServiceLineBreakdown();
			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var eligibleExpenseBreakdown = EntityHelper.CreateEligibleExpenseBreakdown(eligibleExpenseType);
			eligibleExpenseBreakdown.ServiceLine = serviceLine;
			eligibleExpenseBreakdown.ServiceLineId = serviceLine.Id;
			eligibleCost.EligibleExpenseType = eligibleExpenseType;
			var eligibleCostBreakdown = EntityHelper.CreateEligibleCostBreakdown(eligibleCost, eligibleExpenseBreakdown);
			eligibleCostBreakdown.AddedByAssessor = true;
			eligibleExpenseType.Breakdowns = new List<EligibleExpenseBreakdown>() { eligibleExpenseBreakdown };
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			trainingProgram.ServiceLineId = serviceLine.Id;
			trainingProgram.EligibleCostBreakdown = eligibleCostBreakdown;
			trainingProgram.EligibleCostBreakdownId = eligibleCostBreakdown.Id;

			helper.GetMock<ITrainingProgramService>()
							.Setup(m => m.Get(It.IsAny<int>()))
							.Returns(trainingProgram);


			var controller = helper.Create();

			// Act
			var result = controller.DeleteComponent(trainingProgram.Id, "AgQGCAoMDhASFA==");

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Delete(It.IsAny<TrainingProgram>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void DeleteComponent_NotAuthorized()
		{
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			helper.GetMock<ITrainingProgramService>()
							.Setup(m => m.Get(It.IsAny<int>()))
							.Throws<NotAuthorizedException>();

			var controller = helper.Create();
			// Act
			var result = controller.DeleteComponent(trainingProgram.Id, "AgQGCAoMDhASFA==");

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Delete(It.IsAny<TrainingProgram>()), Times.Never);
		}
		#endregion

		#region GetServiceLines
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void GetServiceLines()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user, Roles.Assessor);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var eligibleExpenseBreakdown = EntityHelper.CreateEligibleExpenseBreakdown(eligibleExpenseType);
			var eligibleCostBreakdown = EntityHelper.CreateEligibleCostBreakdown(eligibleCost, eligibleExpenseBreakdown);
			var serviceLine = EntityHelper.CreateServiceLine();
			serviceLine.BreakdownCaption = "BreakdownCaption";
			eligibleExpenseBreakdown.ServiceLineId = serviceLine.Id;
			eligibleExpenseBreakdown.Caption = "TEST";
			eligibleExpenseBreakdown.ServiceLine = serviceLine;
			eligibleExpenseType.Breakdowns = new List<EligibleExpenseBreakdown>() { eligibleExpenseBreakdown };
			helper.GetMock<IEligibleExpenseTypeService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(eligibleExpenseType);
			var controller = helper.Create();

			// Act
			var result = controller.GetServiceLines(eligibleExpenseType.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			var list = result.Data as Object[];
			list.Count().Should().Be(eligibleExpenseType.Breakdowns.Count());
			helper.GetMock<IEligibleExpenseTypeService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void GetServiceLines_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user, Roles.Assessor);

			helper.GetMock<IEligibleExpenseTypeService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetServiceLines(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IEligibleExpenseTypeService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}
		#endregion

		#region GetServiceLineBreakdowns
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void GetServiceLineBreakdowns()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user, Roles.Assessor);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			var serviceLine = EntityHelper.CreateServiceLine();
			serviceLine.BreakdownCaption = "BreakdownCaption";
			var serviceLineBreakdown = EntityHelper.CreateServiceLineBreakdown();
			serviceLineBreakdown.ServiceLineId = serviceLine.Id;
			helper.GetMock<IServiceLineBreakdownService>()
				.Setup(m => m.GetAllForServiceLine(It.IsAny<int>(), It.IsAny<bool>()))
				.Returns(new List<ServiceLineBreakdown>() { serviceLineBreakdown });
			var controller = helper.Create();

			// Act
			var result = controller.GetServiceLineBreakdowns(serviceLine.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			var list = result.Data as Object[];
			list.Count().Should().Be(1);
			helper.GetMock<IServiceLineBreakdownService>().Verify(m => m.GetAllForServiceLine(It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void GetServiceLineBreakdowns_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user, Roles.Assessor);

			helper.GetMock<IServiceLineBreakdownService>()
				.Setup(m => m.GetAllForServiceLine(It.IsAny<int>(), It.IsAny<bool>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetServiceLineBreakdowns(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<IServiceLineBreakdownService>().Verify(m => m.GetAllForServiceLine(It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
		}
		#endregion
		#endregion

		#region Training Provider
		#region GetTrainingProvider
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void GetTrainingProvider()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user, Roles.Assessor);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			var region = EntityHelper.CreateRegion("test");
			trainingProvider.TrainingPrograms.Add(trainingProgram);
			helper.GetMock<ITrainingProviderService>().Setup(m => m.Get(It.IsAny<int>())).Returns(trainingProvider);
			helper.GetMock<IStaticDataService>().Setup(m => m.GetRegion(It.IsAny<string>(), It.IsAny<string>())).Returns(region);
			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingProvider(trainingProvider.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillsTrainingProviderViewModel>();
			var model = result.Data as SkillsTrainingProviderViewModel;
			model.Id.Should().Be(trainingProvider.Id);
			model.ContactEmail.Should().Be(trainingProvider.ContactEmail);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void GetTrainingProvider_NoContentException()
		{
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user, Roles.Assessor);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);

			helper.GetMock<ITrainingProviderService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingProvider(trainingProvider.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillsTrainingProviderViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}
		#endregion

		#region UpdateTrainingProvider
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void UpdateTrainingProvider()
		{
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var serviceLine = EntityHelper.CreateServiceLine();
			var serviceLineBreakdown = EntityHelper.CreateServiceLineBreakdown();
			var eligibleCost = EntityHelper.CreateEligibleCost(grantApplication);
			var eligibleExpenseType = EntityHelper.CreateEligibleExpenseType();
			var eligibleExpenseBreakdown = EntityHelper.CreateEligibleExpenseBreakdown(eligibleExpenseType);
			eligibleExpenseBreakdown.ServiceLine = serviceLine;
			eligibleExpenseBreakdown.ServiceLineId = serviceLine.Id;
			eligibleCost.EligibleExpenseType = eligibleExpenseType;
			var eligibleCostBreakdown = EntityHelper.CreateEligibleCostBreakdown(eligibleCost, eligibleExpenseBreakdown);
			eligibleExpenseType.Breakdowns = new List<EligibleExpenseBreakdown>() { eligibleExpenseBreakdown };
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			trainingProgram.ServiceLineId = serviceLine.Id;
			trainingProgram.EligibleCostBreakdown = eligibleCostBreakdown;
			trainingProgram.EligibleCostBreakdownId = eligibleCostBreakdown.Id;
			var trainigProviderType = EntityHelper.CreateTrainingProviderType("TrainingProviderType");
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			var region = EntityHelper.CreateRegion("test");
			var country = EntityHelper.CreateCountry("country test");
			grantApplication.TrainingPrograms.Add(trainingProgram);
			helper.GetMock<ITrainingProviderService>().Setup(m => m.Get(It.IsAny<int>())).Returns(trainingProvider);
		

			var updateSkillsTrainingProviderViewModel = new UpdateSkillsTrainingProviderViewModel()
			{
				RowVersion = "AgQGCAoMDhASFA==",
				EligibleCostId = eligibleCost.Id,
				Name = "Name",
				ChangeRequestReason = "ChangeRequestReason",
				TrainingProgramId = trainingProgram.Id,
				TrainingProviderTypeId = trainigProviderType.Id,
				TrainingProviderInventoryId = trainingProviderInventory.Id,
				TrainingProviderState = TrainingProviderStates.Complete,
				ContactFirstName = "first name",
				ContactLastName = "Last Name",
				ContactEmail = "test@dfad.ca",
				ContactPhoneAreaCode = "604",
				ContactPhoneExchange = "123",
				ContactPhoneNumber = "4567",
				ContactPhoneExtension = "4567",
				AddressLine1 = "address",
				City = "city",
				PostalCode = "V9C9C9",
				TrainingAddressId = 1,
				CountryId = CJG.Core.Entities.Constants.CanadaCountryId
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateTrainingProvider(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(updateSkillsTrainingProviderViewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillsTrainingProviderViewModel>();
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Update(It.IsAny<TrainingProvider>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void UpdateTrainingProvider_NotAuthorized()
		{
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			helper.GetMock<ITrainingProviderService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var updateSkillsTrainingProviderViewModel = new UpdateSkillsTrainingProviderViewModel()
			{
				RowVersion = "AgQGCAoMDhASFA==",
				Name = "Name",
				ChangeRequestReason = "ChangeRequestReason",
				TrainingProgramId = trainingProgram.Id,
				TrainingProviderState = TrainingProviderStates.Complete,
				ContactFirstName = "first name",
				ContactLastName = "Last Name",
				ContactEmail = "test@dfad.ca",
				ContactPhoneAreaCode = "604",
				ContactPhoneExchange = "123",
				ContactPhoneNumber = "4567",
				ContactPhoneExtension = "4567",
				AddressLine1 = "address",
				City = "city",
				PostalCode = "V9C9C9",
				TrainingAddressId = 1,
				CountryId = CJG.Core.Entities.Constants.CanadaCountryId
			};
			var controller = helper.Create();
			// Act
			var result = controller.UpdateTrainingProvider(new HttpPostedFileBase[] { }, JsonConvert.SerializeObject(updateSkillsTrainingProviderViewModel));

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillsTrainingProviderViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Update(It.IsAny<TrainingProvider>()), Times.Never);
		}
		#endregion

		#region ApproveProvider
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void ApproveProvider()
		{
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("TrainingProviderType");
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			trainingProvider.TrainingPrograms.Add(trainingProgram);
			trainingProvider.TrainingProviderType = trainingProviderType;
			var region = EntityHelper.CreateRegion("test");
			helper.GetMock<ITrainingProviderService>().Setup(m => m.Get(It.IsAny<int>())).Returns(trainingProvider);
			helper.GetMock<IStaticDataService>().Setup(m => m.GetRegion(It.IsAny<string>(), It.IsAny<string>())).Returns(region);

			var skillsTrainingProviderViewModel = new SkillsTrainingProviderViewModel()
			{
				RowVersion = "AgQGCAoMDhASFA==",
				Id = 1
			};
			var controller = helper.Create();

			// skillsTrainingProviderViewModel
			var result = controller.ApproveProvider(skillsTrainingProviderViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillsTrainingProviderViewModel>();
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Update(It.IsAny<TrainingProvider>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void ApproveProvider_NotAuthorized()
		{

			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("TrainingProviderType");
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			trainingProvider.TrainingPrograms.Add(trainingProgram);
			trainingProvider.TrainingProviderType = trainingProviderType;

			helper.GetMock<ITrainingProviderService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();


			var skillsTrainingProviderViewModel = new SkillsTrainingProviderViewModel()
			{
				RowVersion = "AgQGCAoMDhASFA==",
				Id = 1
			};
			var controller = helper.Create();

			// skillsTrainingProviderViewModel
			var result = controller.ApproveProvider(skillsTrainingProviderViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillsTrainingProviderViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Update(It.IsAny<TrainingProvider>()), Times.Never);
		}
		#endregion

		#region DenyProvider
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void DenyProvider()
		{
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("TrainingProviderType");
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			trainingProvider.TrainingPrograms.Add(trainingProgram);
			trainingProvider.TrainingProviderType = trainingProviderType;
			var region = EntityHelper.CreateRegion("test");
			helper.GetMock<ITrainingProviderService>().Setup(m => m.Get(It.IsAny<int>())).Returns(trainingProvider);
			helper.GetMock<IStaticDataService>().Setup(m => m.GetRegion(It.IsAny<string>(), It.IsAny<string>())).Returns(region);

			var skillsTrainingProviderViewModel = new SkillsTrainingProviderViewModel()
			{
				RowVersion = "AgQGCAoMDhASFA==",
				Id = 1
			};
			var controller = helper.Create();

			// skillsTrainingProviderViewModel
			var result = controller.DenyProvider(skillsTrainingProviderViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillsTrainingProviderViewModel>();
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Update(It.IsAny<TrainingProvider>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(SkillsTrainingController))]
		public void DenyProvider_NotAuthorized()
		{

			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<SkillsTrainingController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("TrainingProviderType");
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			trainingProvider.TrainingPrograms.Add(trainingProgram);
			trainingProvider.TrainingProviderType = trainingProviderType;

			helper.GetMock<ITrainingProviderService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();


			var skillsTrainingProviderViewModel = new SkillsTrainingProviderViewModel()
			{
				RowVersion = "AgQGCAoMDhASFA==",
				Id = 1
			};
			var controller = helper.Create();

			// skillsTrainingProviderViewModel
			var result = controller.DenyProvider(skillsTrainingProviderViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<SkillsTrainingProviderViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Update(It.IsAny<TrainingProvider>()), Times.Never);
		}
		#endregion
		#endregion
	}
}

using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Int.Controllers;
using CJG.Web.External.Areas.Int.Models.Organizations;
using CJG.Web.External.Areas.Int.Models.TrainingPrograms;
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
	public class TrainingProgramControllerTest
	{
		#region GetTrainingProgram
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void GetTrainingProgram()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<TrainingProgramController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			helper.GetMock<ITrainingProgramService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProgram);
			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingProgram(trainingProgram.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingProgramViewModel>();
			var model = result.Data as TrainingProgramViewModel;
			model.Id.Should().Be(trainingProgram.Id);
			model.CourseTitle.Should().Be(trainingProgram.CourseTitle);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void GetTrainingProgram_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<TrainingProgramController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			helper.GetMock<ITrainingProgramService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingProgram(trainingProgram.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingProgramViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}
		#endregion

		#region UpdateTrainingProgram
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void UpdateTrainingProgram()
		{
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<TrainingProgramController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			var deliveryMethod = EntityHelper.CreateDeliveryMethod("deliveryMethod");
			trainingProgram.DeliveryMethods = new List<DeliveryMethod>() { deliveryMethod };
			var underRepresentedGroup = EntityHelper.CreateUnderRepresentedGroup("UnderRepresentedGroup");
			trainingProgram.UnderRepresentedGroups = new List<UnderRepresentedGroup>() { underRepresentedGroup };
			helper.GetMock<ITrainingProgramService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProgram);
			var controller = helper.Create();
			var trainingProgramViewModel = new TrainingProgramViewModel()
			{
				Id = trainingProgram.Id,
				RowVersion = "AgQGCAoMDhASFA==",
				StartDate = DateTime.UtcNow,
				EndDate = DateTime.UtcNow.AddMonths(3),
				CourseTitle = "CourseTitle"
			};
			// Act
			var result = controller.UpdateTrainingProgram(trainingProgramViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingProgramViewModel>();
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Update(It.IsAny<TrainingProgram>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProgramController))]
		public void UpdateTrainingProgram_Exception()
		{
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<TrainingProgramController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProgram = EntityHelper.CreateTrainingProgram(grantApplication);
			helper.GetMock<ITrainingProgramService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();
			var trainingProgramViewModel = new TrainingProgramViewModel()
			{
				Id = trainingProgram.Id,
			};
			// Act
			var result = controller.UpdateTrainingProgram(trainingProgramViewModel);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<TrainingProgramViewModel>();
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProgramService>().Verify(m => m.Update(It.IsAny<TrainingProgram>()), Times.Never);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}
		#endregion
	}
}

using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Areas.Int.Controllers;
using CJG.Web.External.Areas.Int.Models.TrainingProviders;
using CJG.Web.External.Models.Shared;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static CJG.Testing.Core.ServiceHelper;

namespace CJG.Testing.UnitTests.Controllers.Int
{
	[TestClass]
	public class TrainingProviderControllerTest
	{
		#region GetTrainingProvider
		[Ignore, TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void GetTrainingProvider()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user, Roles.Assessor);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProvider);
			helper.GetMock<IGrantApplicationService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingProvider(trainingProvider.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<Web.External.Areas.Int.Models.TrainingProviders.TrainingProviderViewModel>();
			var model = result.Data as Web.External.Areas.Int.Models.TrainingProviders.TrainingProviderViewModel;
			model.GrantApplicationId.Should().Be(grantApplication.Id);
			model.Name.Should().Be(trainingProvider.Name);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void GetTrainingProvider_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user, Roles.Assessor);

			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingProvider(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<Web.External.Areas.Int.Models.TrainingProviders.TrainingProviderViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void GetTrainingProvider_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user, Roles.Assessor);

			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NoContentException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetTrainingProvider(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<Web.External.Areas.Int.Models.TrainingProviders.TrainingProviderViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}
		#endregion

		#region UpdateTrainingProvider
		[Ignore, TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void UpdateTrainingProvider()
		{
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("test");
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();
			var region = EntityHelper.CreateRegion("test");
			var country = EntityHelper.CreateCountry("country test");

			helper.GetMock<ITrainingProviderService>().Setup(m => m.Get(It.IsAny<int>())).Returns(trainingProvider);
			helper.GetMock<IStaticDataService>().Setup(m => m.GetRegion(It.IsAny<string>(), It.IsAny<string>())).Returns(region);
			helper.GetMock<IStaticDataService>().Setup(m => m.GetCountry(It.IsAny<string>())).Returns(country);

			var trainingTrainerDetailsListViewModel = new TrainingTrainerDetailsListViewModel()
			{
				ContactFirstName = "first name",
				ContactLastName = "Last Name",
				ContactEmail = "test@dfad.ca",
				ContactNumberNumber = "604",
				ContactNumberAreaCode = "123",
				ContactNumberExchange = "4567",
				ContactNumberExtension = "4567"
			};
			var trainingOutsideBCListViewModel = new TrainingOutsideBCListViewModel()
			{
				TrainingOutsideBC = false
			};
			var trainingLocationListViewModel = new CJG.Web.External.Areas.Int.Models.AddressViewModel()
			{
				AddressLine1 = "address",
				City = "city",
				PostalCode = "V9C9C9"
			};
			var trainingProviderViewModel = new UpdateTrainingProviderViewModel()
			{
				Name = "Name Test",
				RowVersion = "AgQGCAoMDhASFA==",
				TrainingProviderType = trainingProviderType,
				TrainingProviderInventoryId = trainingProviderInventory.Id,
				TrainingTrainerDetailsListViewModel = trainingTrainerDetailsListViewModel,
				TrainingLocationListViewModel = trainingLocationListViewModel,
				TrainingOutsideBcListViewModel = trainingOutsideBCListViewModel
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateTrainingProvider(JsonConvert.SerializeObject(trainingProviderViewModel), new HttpPostedFileBase[] { });

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<CJG.Web.External.Areas.Int.Models.TrainingProviders.TrainingProviderViewModel>();
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Update(It.IsAny<TrainingProvider>()), Times.Once);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetRegion(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetCountry(It.IsAny<string>()), Times.Once);
		}

		[Ignore, TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void UpdateTrainingProvider_InvalidModel()
		{
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user, Roles.Assessor);
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			var trainingProviderType = EntityHelper.CreateTrainingProviderType("test");
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();
			var region = EntityHelper.CreateRegion("test");
			var country = EntityHelper.CreateCountry("country test");

			helper.GetMock<ITrainingProviderService>().Setup(m => m.Get(It.IsAny<int>())).Returns(trainingProvider);
			helper.GetMock<IStaticDataService>().Setup(m => m.GetRegion(It.IsAny<string>(), It.IsAny<string>())).Returns(region);
			helper.GetMock<IStaticDataService>().Setup(m => m.GetCountry(It.IsAny<string>())).Returns(country);

			var trainingTrainerDetailsListViewModel = new TrainingTrainerDetailsListViewModel();
			var trainingOutsideBCListViewModel = new TrainingOutsideBCListViewModel()
			{
				TrainingOutsideBC = false
			};
			var trainingLocationListViewModel = new CJG.Web.External.Areas.Int.Models.AddressViewModel();
			var trainingProviderViewModel = new UpdateTrainingProviderViewModel()
			{
				Id = 1,
				TrainingProviderType = trainingProviderType,
				TrainingProviderInventoryId = trainingProviderInventory.Id,
				TrainingTrainerDetailsListViewModel = trainingTrainerDetailsListViewModel,
				TrainingLocationListViewModel = trainingLocationListViewModel,
				TrainingOutsideBcListViewModel = trainingOutsideBCListViewModel
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateTrainingProvider(JsonConvert.SerializeObject(trainingProviderViewModel), new HttpPostedFileBase[] { });

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<CJG.Web.External.Areas.Int.Models.TrainingProviders.TrainingProviderViewModel>();
			var validationErrors = (result.Data as CJG.Web.External.Areas.Int.Models.TrainingProviders.TrainingProviderViewModel).ValidationErrors;
			validationErrors.Count().Should().Be(7);
			validationErrors.Any(l => l.Key == "TrainingLocationListViewModel.AddressLine1").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TrainingLocationListViewModel.City").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TrainingLocationListViewModel.PostalCode").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TrainingTrainerDetailsListViewModel.ContactFirstName").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TrainingTrainerDetailsListViewModel.ContactLastName").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TrainingTrainerDetailsListViewModel.ContactEmail").Should().BeTrue();
			validationErrors.Any(l => l.Key == "TrainingTrainerDetailsListViewModel.PhoneNumberNoExtension").Should().BeTrue();

			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Update(It.IsAny<TrainingProvider>()), Times.Never);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetRegion(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetCountry(It.IsAny<string>()), Times.Never);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void UpdateTrainingProvider_NotAuthorized()
		{
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user, Roles.Assessor);
			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Throws<NotAuthorizedException>();

			var trainingProviderViewModel = new UpdateTrainingProviderViewModel()
			{
				Id = 1
			};
			var controller = helper.Create();

			// Act
			var result = controller.UpdateTrainingProvider(JsonConvert.SerializeObject(trainingProviderViewModel), new HttpPostedFileBase[] { });

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<CJG.Web.External.Areas.Int.Models.TrainingProviders.TrainingProviderViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Update(It.IsAny<TrainingProvider>()), Times.Never);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetRegion(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
			helper.GetMock<IStaticDataService>().Verify(m => m.GetCountry(It.IsAny<string>()), Times.Never);
		}
		#endregion

		#region DownloadAttachment
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void DownloadAttachment()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user, Roles.Assessor);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			var attachment = EntityHelper.CreateAttachment();
			trainingProvider.BusinessCaseDocumentId = attachment.Id;
			helper.GetMock<IAttachmentService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(attachment);
			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProvider);
			var controller = helper.Create();

			// Act
			var result = controller.DownloadAttachment(trainingProvider.Id, attachment.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<FileContentResult>();
			var model = result as FileContentResult;
			model.FileDownloadName.Should().Be(attachment.FileName + attachment.FileExtension);
			helper.GetMock<IAttachmentService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void DownloadAttachment_InvalidOperationException()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user, Roles.Assessor);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			var attachment = EntityHelper.CreateAttachment();
			attachment.Id = 5;
			helper.GetMock<IAttachmentService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(attachment);
			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.Get(It.IsAny<int>()))
				.Returns(trainingProvider);
			var controller = helper.Create();

			// Act
			var result = controller.DownloadAttachment(trainingProvider.Id, attachment.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
			helper.GetMock<IAttachmentService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}
		#endregion

		#region GetInventory
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void GetInventory()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user, Roles.Assessor);
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();
			var trainingProviderInventories = new PageList<TrainingProviderInventory>()
			{
				Items = new List<TrainingProviderInventory>() { trainingProviderInventory }
			};
			helper.GetMock<ITrainingProviderInventoryService>()
				.Setup(m => m.GetInventory(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool?>()))
				.Returns(trainingProviderInventories);
			var controller = helper.Create();

			// Act
			var result = controller.GetInventory(1, 1, "");

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			var list = result.Data.GetReflectedProperty("Data") as TrainingProviderInventoryViewModel[];
			list.Count().Should().Be(trainingProviderInventories.Items.Count());
			list[0].Name.Should().Be(trainingProviderInventory.Name);
			helper.GetMock<ITrainingProviderInventoryService>()
				.Verify(m => m.GetInventory(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool?>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void GetInventory_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user, Roles.Assessor);
			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();
			var trainingProviderInventories = new PageList<TrainingProviderInventory>()
			{
				Items = new List<TrainingProviderInventory>() { trainingProviderInventory }
			};
			helper.GetMock<ITrainingProviderInventoryService>()
				.Setup(m => m.GetInventory(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool?>()))
				.Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetInventory(1, 1, "");

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<ITrainingProviderInventoryService>()
				.Verify(m => m.GetInventory(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool?>()), Times.Once);
		}
		#endregion

		#region GetValidationView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void GetValidationView()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user, Roles.Assessor);

			var grantApplication = EntityHelper.CreateGrantApplication();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			helper.GetMock<ITrainingProviderService>().Setup(m => m.Get(It.IsAny<int>())).Returns(trainingProvider);
			var controller = helper.Create();

			// Act
			var result = controller.GetValidationView(trainingProvider.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<PartialViewResult>();
			var model = result.GetReflectedProperty("Model") as TrainingProviderInventoryViewModel;
			model.Name.Should().Be(trainingProvider.Name);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void GetValidationView_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user, Roles.Assessor);

			var grantApplication = EntityHelper.CreateGrantApplication();
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			helper.GetMock<ITrainingProviderService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NotAuthorizedException>(() => controller.GetValidationView(1));
		}
		#endregion

		#region Validate
		[Ignore, TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void Validate()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			helper.GetMock<ITrainingProviderService>().Setup(m => m.Get(It.IsAny<int>())).Returns(trainingProvider);
			helper.GetMock<ITrainingProviderService>()
				.Setup(m => m.ValidateTrainingProvider(It.IsAny<TrainingProvider>(), It.IsAny<int>()))
				.Returns(trainingProvider);

			// Act
			var result = controller.Validate(trainingProvider.Id, 1, "AgQGCAoMDhASFA==");

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<Web.External.Areas.Int.Models.TrainingProviders.TrainingProviderViewModel>();
			var model = result.Data as Web.External.Areas.Int.Models.TrainingProviders.TrainingProviderViewModel;
			model.GrantApplicationId.Should().Be(grantApplication.Id);
			helper.GetMock<ITrainingProviderService>()
				.Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>()
				.Verify(m => m.ValidateTrainingProvider(It.IsAny<TrainingProvider>(), It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void Validate_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user, Roles.Assessor);
			var controller = helper.Create();
			var grantApplication = EntityHelper.CreateGrantApplication(user);
			var trainingProvider = EntityHelper.CreateTrainingProvider(grantApplication);
			helper.GetMock<ITrainingProviderService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();

			// Act
			var result = controller.Validate(trainingProvider.Id, 1, "AgQGCAoMDhASFA==");

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<Web.External.Areas.Int.Models.TrainingProviders.TrainingProviderViewModel>();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
			helper.GetMock<ITrainingProviderService>()
				.Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<ITrainingProviderService>()
				.Verify(m => m.ValidateTrainingProvider(It.IsAny<TrainingProvider>(), It.IsAny<int>()), Times.Never);
		}
		#endregion

		#region AddTrainingProviderToInventory
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void AddTrainingProviderToInventory()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user, Roles.Assessor);
			var controller = helper.Create();

			// Act
			var result = controller.AddTrainingProviderToInventory("Name", true, true, "no");

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			var model = result.Data as BaseViewModel;
			helper.GetMock<ITrainingProviderInventoryService>()
				.Verify(m => m.GetTrainingProviderFromInventory(It.IsAny<string>()), Times.Once);
			helper.GetMock<ITrainingProviderInventoryService>()
				.Verify(m => m.Add(It.IsAny<TrainingProviderInventory>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(TrainingProviderController))]
		public void AddTrainingProviderToInventory_InvalidOperationException()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ControllerHelper<TrainingProviderController>(user, Roles.Assessor);

			var trainingProviderInventory = EntityHelper.CreateTrainingProviderInventory();
			helper.GetMock<ITrainingProviderInventoryService>()
				.Setup(m => m.GetTrainingProviderFromInventory(It.IsAny<string>()))
				.Returns(trainingProviderInventory);
			var controller = helper.Create();

			// Act
			var result = controller.AddTrainingProviderToInventory(trainingProviderInventory.Name, true, true, "no");

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
			var model = result.Data as BaseViewModel;
			helper.GetMock<ITrainingProviderInventoryService>()
				.Verify(m => m.GetTrainingProviderFromInventory(It.IsAny<string>()), Times.Once);
			helper.GetMock<ITrainingProviderInventoryService>()
				.Verify(m => m.Add(It.IsAny<TrainingProviderInventory>()), Times.Never);
		}
		#endregion
	}
}

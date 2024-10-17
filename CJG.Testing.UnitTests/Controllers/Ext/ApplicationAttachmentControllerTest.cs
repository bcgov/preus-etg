using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Testing.Core;
using CJG.Web.External.Areas.Ext.Controllers;
using CJG.Web.External.Areas.Ext.Models.Attachments;
using CJG.Web.External.Models.Shared;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CJG.Testing.UnitTests.Controllers.Ext
{
	[TestClass]
	public class ApplicationAttachmentControllerTest
	{
		#region AttachmentsView
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationAttachmentController))]
		public void AttachmentsView()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationAttachmentController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.AttachmentsView(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<ViewResult>();
			grantApplication.Id.Should().Be(controller.ViewBag.GrantApplicationId);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationAttachmentController))]
		public void AttachmentsView_NotAuthorized()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser(2);
			var helper = new ControllerHelper<ApplicationAttachmentController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act + Assert
			var result = CoreAssert.Throws<NotAuthorizedException>(() => controller.AttachmentsView(1));
		}
		#endregion

		#region GetAttachments
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationAttachmentController))]
		public void GetAttachments()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationAttachmentController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.Attachments.Clear();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);
			var controller = helper.Create();

			// Act
			var result = controller.GetAttachments(grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantApplicationAttachmentsViewModel>();
			var model = result.Data as GrantApplicationAttachmentsViewModel;
			model.Id.Should().Be(grantApplication.Id);
			model.Attachments.Count().Should().Be(0);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationAttachmentController))]
		public void GetAttachments_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationAttachmentController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetAttachments(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantApplicationAttachmentsViewModel>();
			var model = result.Data as GrantApplicationAttachmentsViewModel;
			model.Id.Should().Be(0);
			model.Attachments.Should().BeNull();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationAttachmentController))]
		public void GetAttachments_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationAttachmentController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetAttachments(1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantApplicationAttachmentsViewModel>();
			var model = result.Data as GrantApplicationAttachmentsViewModel;
			model.Id.Should().Be(0);
			model.Attachments.Should().BeNull();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region GetAttachment
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationAttachmentController))]
		public void GetAttachment()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationAttachmentController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);

			var attachment = EntityHelper.CreateAttachment();
			helper.GetMock<IAttachmentService>().Setup(m => m.Get(It.IsAny<int>())).Returns(attachment);
			var controller = helper.Create();

			// Act
			var result = controller.GetAttachment(attachment.Id, grantApplication.Id);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantApplicationAttachmentViewModel>();
			var model = result.Data as GrantApplicationAttachmentViewModel;
			model.GrantApplicationId.Should().Be(grantApplication.Id);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationAttachmentController))]
		public void GetAttachment_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationAttachmentController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetAttachment(1, 1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantApplicationAttachmentViewModel>();
			var model = result.Data as GrantApplicationAttachmentViewModel;
			model.Id.Should().Be(0);
			model.GrantApplicationId.Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationAttachmentController))]
		public void GetAttachment_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationAttachmentController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var controller = helper.Create();

			// Act
			var result = controller.GetAttachment(1, 1);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantApplicationAttachmentViewModel>();
			var model = result.Data as GrantApplicationAttachmentViewModel;
			model.Id.Should().Be(0);
			model.GrantApplicationId.Should().Be(0);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region UpdateAttachment
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationAttachmentController))]
		public void UpdateAttachment_NoFile()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationAttachmentController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.Attachments.Clear();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);

			var attachment = EntityHelper.CreateAttachment();
			helper.GetMock<IAttachmentService>().Setup(m => m.Get(It.IsAny<int>())).Returns(attachment);
			var controller = helper.Create();

			grantApplication.Attachments.Add(attachment);
			var data = new UpdateAttachmentViewModel(attachment);
			var json = Newtonsoft.Json.JsonConvert.SerializeObject(new[] { data });

			// Act
			var result = controller.UpdateAttachment(attachment.Id, null, json);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantApplicationAttachmentsViewModel>();
			var model = result.Data as GrantApplicationAttachmentsViewModel;
			model.Id.Should().Be(grantApplication.Id);
			model.Attachments.Count().Should().Be(1);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IAttachmentService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IAttachmentService>().Verify(m => m.Update(It.IsAny<Attachment>(), It.IsAny<bool>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationAttachmentController))]
		public void UpdateAttachment_Add_WithFile()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationAttachmentController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.Attachments.Clear();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);

			var attachment = EntityHelper.CreateAttachment(0);
			helper.GetMock<IAttachmentService>().Setup(m => m.Get(It.IsAny<int>())).Returns(attachment);
			helper.GetMock<IAttachmentService>().Setup(m => m.Add(It.IsAny<Attachment>(), It.IsAny<bool>())).Callback<Attachment, bool>((a, b) => {
				a.Id = 1;
				a.SetRowVersion();
			});
			var controller = helper.Create();

			var data = new UpdateAttachmentViewModel(attachment) { Index = 0 };
			var json = Newtonsoft.Json.JsonConvert.SerializeObject(new[] { data });

			var file = FileHelper.CreateFile($"{attachment.FileName}{attachment.FileExtension}");

			// Act
			var result = controller.UpdateAttachment(attachment.Id, new[] { file }, json);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantApplicationAttachmentsViewModel>();
			var model = result.Data as GrantApplicationAttachmentsViewModel;
			model.Id.Should().Be(grantApplication.Id);
			model.Attachments.Count().Should().Be(1);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IAttachmentService>().Verify(m => m.Add(It.IsAny<Attachment>(), It.IsAny<bool>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationAttachmentController))]
		public void UpdateAttachment_Update_WithFile()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationAttachmentController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.Attachments.Clear();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);

			var attachment = EntityHelper.CreateAttachment(1);
			helper.GetMock<IAttachmentService>().Setup(m => m.Get(It.IsAny<int>())).Returns(attachment);
			var controller = helper.Create();

			grantApplication.Attachments.Add(attachment);
			var data = new UpdateAttachmentViewModel(attachment) { Index = 0 };
			var json = Newtonsoft.Json.JsonConvert.SerializeObject(new[] { data });

			var file = FileHelper.CreateFile($"{attachment.FileName}{attachment.FileExtension}");

			// Act
			var result = controller.UpdateAttachment(attachment.Id, new[] { file }, json);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantApplicationAttachmentsViewModel>();
			var model = result.Data as GrantApplicationAttachmentsViewModel;
			model.Id.Should().Be(grantApplication.Id);
			model.Attachments.Count().Should().Be(1);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IAttachmentService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IAttachmentService>().Verify(m => m.Update(It.IsAny<Attachment>(), It.IsAny<bool>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationAttachmentController))]
		public void UpdateAttachment_Delete()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationAttachmentController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			grantApplication.Attachments.Clear();
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);

			var attachment = EntityHelper.CreateAttachment();
			helper.GetMock<IAttachmentService>().Setup(m => m.Get(It.IsAny<int>())).Returns(attachment);
			var controller = helper.Create();

			grantApplication.Attachments.Add(attachment);
			var data = new UpdateAttachmentViewModel(attachment) { Delete = true };
			var json = Newtonsoft.Json.JsonConvert.SerializeObject(new[] { data });

			// Act
			var result = controller.UpdateAttachment(attachment.Id, null, json);

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantApplicationAttachmentsViewModel>();
			var model = result.Data as GrantApplicationAttachmentsViewModel;
			model.Id.Should().Be(grantApplication.Id);
			model.Attachments.Count().Should().Be(0);
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IAttachmentService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IAttachmentService>().Verify(m => m.Delete(It.IsAny<Attachment>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationAttachmentController))]
		public void UpdateAttachment_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationAttachmentController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.UpdateAttachment(1, null, "{Id:1}");

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantApplicationAttachmentsViewModel>();
			var model = result.Data as GrantApplicationAttachmentsViewModel;
			model.Id.Should().Be(0);
			model.Attachments.Should().BeNull();
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationAttachmentController))]
		public void UpdateAttachment_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationAttachmentController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var controller = helper.Create();

			// Act
			var result = controller.UpdateAttachment(1, null, "{Id:1}");

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<GrantApplicationAttachmentsViewModel>();
			var model = result.Data as GrantApplicationAttachmentsViewModel;
			model.Id.Should().Be(0);
			model.Attachments.Should().BeNull();
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region DownloadAttachment
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationAttachmentController))]
		public void DownloadAttachment()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationAttachmentController>(user);

			var grantApplication = EntityHelper.CreateGrantApplication(user);
			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Returns(grantApplication);

			var attachment = EntityHelper.CreateAttachment();
			helper.GetMock<IAttachmentService>().Setup(m => m.Get(It.IsAny<int>())).Returns(attachment);
			grantApplication.Attachments.Add(attachment);
			var controller = helper.Create();

			// Act
			var result = controller.DownloadAttachment(grantApplication.Id, attachment.Id) as FileContentResult;

			// Assert
			result.Should().NotBeNull().And.BeOfType<FileContentResult>();
			result.ContentType.Should().Be("application/octet-stream");
			helper.GetMock<IGrantApplicationService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
			helper.GetMock<IAttachmentService>().Verify(m => m.Get(It.IsAny<int>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationAttachmentController))]
		public void DownloadAttachment_NotAuthorizedException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationAttachmentController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NotAuthorizedException>();
			var controller = helper.Create();

			// Act
			var result = controller.DownloadAttachment(1, 1) as JsonResult;

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<BaseViewModel>();
			var model = result.Data as BaseViewModel;
			model.Id.Should().Be(0);
			model.ValidationErrors.Should().HaveCount(1);
			controller.HttpContext.Response.StatusCode.Should().Be(403);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationAttachmentController))]
		public void DownloadAttachment_NoContentException()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationAttachmentController>(user);

			helper.GetMock<IGrantApplicationService>().Setup(m => m.Get(It.IsAny<int>())).Throws<NoContentException>();
			var controller = helper.Create();

			// Act
			var result = controller.DownloadAttachment(1, 1) as JsonResult;

			// Assert
			result.Should().NotBeNull().And.BeOfType<JsonResult>();
			result.Data.Should().NotBeNull().And.BeOfType<BaseViewModel>();
			var model = result.Data as BaseViewModel;
			model.Id.Should().Be(0);
			model.ValidationErrors.Should().HaveCount(1);
			controller.HttpContext.Response.StatusCode.Should().Be(400);
		}
		#endregion

		#region DownloadResource
		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationAttachmentController))]
		public void DownloadResource()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationAttachmentController>(user);

			helper.GetMock<HttpServerUtilityBase>().Setup(m => m.MapPath(It.IsAny<string>())).Returns("thefullpath/test.pdf");
			helper.GetMock<Web.External.Helpers.IFileWrapper>().Setup(m => m.Exists(It.IsAny<string>())).Returns(true);
			var controller = helper.Create();

			// Act
			var result = controller.DownloadResource("filename.pdf", "test message") as FilePathResult;

			// Assert
			result.Should().NotBeNull().And.BeOfType<FilePathResult>();
			result.ContentType.Should().Be("application/octet-stream");
			helper.GetMock<HttpServerUtilityBase>().Verify(m => m.MapPath(It.IsAny<string>()), Times.Once);
			helper.GetMock<Web.External.Helpers.IFileWrapper>().Verify(m => m.Exists(It.IsAny<string>()), Times.Once);
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationAttachmentController))]
		public void DownloadResource_Redirect()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationAttachmentController>(user);
			var controller = helper.Create();

			var message = "test message";

			// Act
			var result = controller.DownloadResource("filename.pdf", message) as RedirectResult;

			// Assert
			var mockHttpRequest = helper.GetMock<HttpRequestBase>();

			result.Should().NotBeNull().And.BeOfType<RedirectResult>();
			result.Url.Should().Be(mockHttpRequest.Object.UrlReferrer.ToString());
			controller.TempData["message"].Should().Be($"The sample {message} could not be found.");
			controller.TempData["messageType"].Should().Be("warning");
		}

		[TestMethod, TestCategory("Controller"), TestCategory(nameof(ApplicationAttachmentController))]
		public void DownloadResource_BadRequest()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ControllerHelper<ApplicationAttachmentController>(user);

			helper.GetMock<HttpServerUtilityBase>().Setup(m => m.MapPath(It.IsAny<string>())).Throws<Exception>();
			var controller = helper.Create();

			// Act
			var result = controller.DownloadResource("filename.pdf", "test message") as HttpStatusCodeResult;

			// Assert
			result.Should().NotBeNull().And.BeOfType<HttpStatusCodeResult>();
			result.StatusCode.Should().Be(400);
		}
		#endregion
	}
}

using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Infrastructure.Entities;
using CJG.Testing.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
    public class AttachmentServiceTest : ServiceUnitTestBase
	{
		[TestInitialize]
        public void Setup()
		{
			
		}

        [TestMethod, TestCategory("Attachment"), TestCategory("Service")]
        public void GetAttachment_AttachmentIDAsParameter_CanFindAttachment()
        {
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(AttachmentService), applicationAdministrator);
			var service = helper.Create<AttachmentService>();
			helper.MockDbSet( new Attachment() { Id = 1 } );

            // Act
            var results = service.Get(1);

            // Assert
            results.Id.Should().Be(1);
        }

        [TestMethod, TestCategory("Attachment"), TestCategory("Service")]
        public void UpdateAttachment_AttachmentObjectAsParameter_ShouldUpdateAttachment()
        {
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(AttachmentService), applicationAdministrator);
			var service = helper.Create<AttachmentService>();
			var attachment = new Attachment()
            {
                Id = 1,
                VersionNumber = 1,
                FileName = "filename",
                FileExtension = "pdf",
                AttachmentData = System.Text.Encoding.ASCII.GetBytes("some data")
            };
            helper.MockDbSet( attachment );
            helper.MockDbSet<VersionedAttachment>();

            // Act
            var result = service.Update(attachment);

            // Assert
            result.VersionNumber.Should().Be(2);
        }

        [TestMethod, TestCategory("Attachment"), TestCategory("Service")]
        public void Add_AttachmentObjectAsParameter_ShouldAddAttachment()
        {
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(AttachmentService), applicationAdministrator);
			var service = helper.Create<AttachmentService>();
			var attachment = new Attachment()
            {
                Id = 1,
                FileName = "filename",
                FileExtension = "pdf",
                AttachmentData = System.Text.Encoding.ASCII.GetBytes("some data")
            };
            helper.MockDbSet( attachment );
            var dbContextMock = helper.GetMock<IDataContext>();
            dbContextMock.Setup(m => m.Attachments.Add(It.IsAny<Attachment>()));

            // Act
            service.Add(attachment);

            // Assert
            dbContextMock.Verify(x => x.Attachments.Add(It.IsAny<Attachment>()), Times.Exactly(1));
        }

        [TestMethod, TestCategory("Attachment"), TestCategory("Service")]
        public void DeleteAttachment_AttachmentIDAsParameter_ShouldRemoveAttachment()
        {
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(AttachmentService), applicationAdministrator);
			var service = helper.Create<AttachmentService>();
			var attachment = new Attachment()
            {
                Id = 1,
                FileName = "filename",
                FileExtension = "pdf",
                AttachmentData = System.Text.Encoding.ASCII.GetBytes("some data")
            };
            helper.MockDbSet( attachment );
            var dbContextMock = helper.GetMock<IDataContext>();
            dbContextMock.Setup(m => m.Attachments.Remove(It.IsAny<Attachment>()));

            // Act
            service.Delete(attachment);

            // Assert
            dbContextMock.Verify(m => m.Attachments.Remove(It.IsAny<Attachment>()), Times.Exactly(1));
        }
    }
}

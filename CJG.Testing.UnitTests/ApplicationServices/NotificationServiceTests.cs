using System;
using System.Collections.Generic;
using System.Linq;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Helpers.Settings;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CJG.Testing.Core;
using CJG.Core.Interfaces.Service.Settings;

using Moq;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
	public class NotificationServiceTests : ServiceUnitTestBase
	{
		[TestInitialize]
		public void Setup()
		{
		}

		[TestMethod]
		[TestCategory("Notification"), TestCategory("Service")]
		[TestCategory("Templates")]
		public void ParseTemplate_WithSinglePlaceholder_ReturnsContentWithReplacedPlaceholder()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(NotificationService), identity, 1);
			var service = helper.Create<NotificationService>(CreateNotificationSettings());
			var grantApplication = EntityHelper.CreateGrantApplication(user);

			var data = new NotificationService.PlaceholderData(grantApplication, new User { FirstName = "TEST_FIRST", LastName = "TEST_LAST" });

			// Act
			var content = service.ParseDocumentTemplate(data, " @Model.RecipientFirstName - @Model.RecipientLastName ");

			// Assert
			content.Should().Be(" TEST_FIRST - TEST_LAST ");
		}

		[TestMethod]
		[TestCategory("Notification"), TestCategory("Service")]
		[TestCategory("Templates")]
		public void ParseTemplate_WithAddress_ReturnsContentWithReplacedPlaceholder()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(NotificationService), identity, 1);
			var service = helper.Create<NotificationService>(CreateNotificationSettings());
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(user);
			grantApplication.ApplicantPhysicalAddress = new ApplicationAddress()
			{
				AddressLine1 = "addr1",
				City = "city1",
				Region = new Region() { Name = "British Columbia" },
				Country = new Country() { Name = "Canada" },
				PostalCode = "A1B 2C3"

			};

			var data = new NotificationService.PlaceholderData(grantApplication, new User());

			// Act
			var content = service.ParseDocumentTemplate(data, "@Model.ApplicationAdministratorAddress");

			// Assert
			content.Should().Contain("addr1");
			content.Should().Contain("city1");
			content.Should().Contain("British Columbia");
			content.Should().Contain("Canada");
			content.Should().Contain("A1B 2C3");
		}

		[TestMethod]
		[TestCategory("Notification"), TestCategory("Service")]
		[TestCategory("Templates")]
		public void ParseTemplate_WithoutPlaceholder_ReturnsOriginalTemplate()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(NotificationService), identity, 1);
			var service = helper.Create<NotificationService>(CreateNotificationSettings());
			var grantApplication = EntityHelper.CreateGrantApplication(user);

			// Act
			var content = service.ParseDocumentTemplate(new NotificationService.PlaceholderData(grantApplication, new User()), "EMPTY");

			// Assert
			content.Should().Be("EMPTY");
		}

		[TestMethod]
		[TestCategory("Notification"), TestCategory("Service")]
		public void HandleWorkflowNotificationType_WithNotificationType_ReturnsTrue()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(NotificationService), identity, 1);
			helper.GetMock<INotificationSettings>().Setup(m => m.EnableEmails).Returns(true);
			helper.MockDbSet<ParticipantForm>();
			helper.MockDbSet<User>();
			var service = helper.Create<NotificationService>(CreateNotificationSettings());

			var notificationType = NotificationTypes.SubmissionConfirmation;
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(user, ApplicationStateInternal.Draft);
			var programNotification = new GrantProgramNotificationType(
				grantApplication.GrantOpening.GrantStream.GrantProgram,
				new NotificationType(notificationType, "test"),
				new NotificationTemplate(" @Model.FileNumber ", " @Model.FileNumber ", " @Model.FileNumber ")
			);
			grantApplication.GrantOpening.GrantStream.GrantProgram.GrantProgramNotificationTypes.Add(programNotification);
			helper.MockDbSet<NotificationScheduleQueue>();
			helper.MockDbSet(grantApplication.BusinessContactRoles);
			helper.MockDbSet(grantApplication);
			var dbSetMock = helper.MockDbSet<Notification>();

			// Act
			var result = service.HandleWorkflowNotificationType(grantApplication, programNotification.NotificationType);

			// Assert
#if Training || Support
			result.Should().BeFalse();
#else
			result.Should().BeTrue();
#endif
		}

		[TestMethod]
		[TestCategory("Notification"), TestCategory("Service")]
		public void HandleWorkflowNotification_WithNotificationType_ReturnsTrue()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(NotificationService), identity, 1);
			helper.GetMock<INotificationSettings>().Setup(m => m.EnableEmails).Returns(true);
			helper.MockDbSet<ParticipantForm>();
			helper.MockDbSet<User>();
			var service = helper.Create<NotificationService>(CreateNotificationSettings());

			var notificationType = NotificationTypes.SubmissionConfirmation;
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(user, ApplicationStateInternal.Draft);
			var programNotification = new GrantProgramNotificationType(
				grantApplication.GrantOpening.GrantStream.GrantProgram,
				new NotificationType(notificationType, "test"),
				new NotificationTemplate(" @Model.FileNumber ", " @Model.FileNumber ", " @Model.FileNumber ")
			);
			grantApplication.GrantOpening.GrantStream.GrantProgram.GrantProgramNotificationTypes.Add(programNotification);
			helper.MockDbSet<NotificationScheduleQueue>();
			helper.MockDbSet(grantApplication.BusinessContactRoles);
			helper.MockDbSet(new[] { grantApplication });
			helper.MockDbSet<Notification>();

			// Act
			var result = service.HandleWorkflowNotification(grantApplication, NotificationTypes.SubmissionConfirmation);

			// Assert
#if Training || Support
			result.Should().BeFalse();
#else
			result.Should().BeTrue();
#endif
		}

		[TestMethod]
		[TestCategory("Notification"), TestCategory("Service")]
		public void HandleScheduledNotification_WithNotificationType_ReturnsTrue()
		{
			// Arrange
			var settings = new NotificationSettings();
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(NotificationService), identity, 1);
			var service = helper.Create<NotificationService>(CreateNotificationSettings());
			helper.MockDbSet<ParticipantForm>();
			helper.MockDbSet<User>();
			helper.GetMock<INotificationSettings>().Setup(m => m.EnableEmails).Returns(true);

			var notificationType = NotificationTypes.SubmissionConfirmation;
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(user, ApplicationStateInternal.Draft);
			var programNotification = new GrantProgramNotificationType(
				grantApplication.GrantOpening.GrantStream.GrantProgram,
				new NotificationType(notificationType, "test"),
				new NotificationTemplate(" @Model.FileNumber ", " @Model.FileNumber ", " @Model.FileNumber "));
			grantApplication.GrantOpening.GrantStream.GrantProgram.GrantProgramNotificationTypes.Add(programNotification);
			helper.MockDbSet<NotificationScheduleQueue>();
			helper.MockDbSet(grantApplication.BusinessContactRoles);
			helper.MockDbSet(new[] { grantApplication });
			helper.MockDbSet<Notification>();

			// Act
			var result = service.HandleScheduledNotification(new NotificationScheduleQueue
			{
				GrantApplication = grantApplication,
				NotificationType = programNotification.NotificationType
			});

			// Assert
#if Training || Support
			result.Should().BeFalse();
#else
			result.Should().Be(settings.EnableEmails);
#endif
		}

		[TestMethod]
		[TestCategory("Notification"), TestCategory("Service")]
		public void GetRecipients_WithApplicationAdministrator_ReturnsUser()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(NotificationService), identity, 1);
			var service = helper.Create<NotificationService>(CreateNotificationSettings());

			helper.MockDbSet( new[]
			{
				new BusinessContactRole {User = new User {Id = 1}, GrantApplicationId = 1}
			});

			// Act
			var result = service.GetRecipients(1);

			// Assert
			result.First().Id.Should().Be(1);
		}

		[TestMethod]
		[TestCategory("Notification"), TestCategory("Service")]
		public void GetRecipients_WithEmployerAdministrator_ReturnsUser()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(NotificationService), identity, 1);
			var service = helper.Create<NotificationService>(CreateNotificationSettings());

			helper.MockDbSet( new[]
			{
				new BusinessContactRole {User = new User {Id = 1}, GrantApplicationId = 1}
			});

			// Act
			var result = service.GetRecipients(1);

			// Assert
			result.Should().HaveCount(1);
		}

		private NotificationSettings CreateNotificationSettings()
		{
			return new NotificationSettings(
					allowEmails: true,
					smtpServer: null,
					timeout: TimeSpan.FromSeconds(1),
					fromAddress: "fromAddress@a.com",
					fromName: "fromName",
					throwOnSendEmailError: false);
		}
	}
}
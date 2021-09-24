using System;
using System.Collections.Generic;
using System.Diagnostics;
using CJG.Application.Services;
using CJG.Core.Entities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CJG.Testing.Core;
using CJG.Infrastructure.Entities;
using System.Data.Entity;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
	public class NotificationQueueServiceTests : ServiceUnitTestBase
	{
		[TestInitialize]
		public void Setup()
		{
		}

		[TestMethod]
		[TestCategory("Notification Queue"), TestCategory("Service")]
		public void CreateNotificationTicketId_WithvalidParams_ReturnsTicketId()
		{
			var ticket = NotificationQueueService.CreateNotificationTicketId(
				new GrantApplication { Id = 1 },
				new NotificationType { Id = NotificationTypes.SubmissionConfirmation });

			Debug.WriteLine("NotificationTicketId: " + ticket);
		}

		[TestMethod]
		[TestCategory("Notification Queue"), TestCategory("Service")]
		public void AddNotificationEvent_WithValidParams_AddedNewNotificationToQueue()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(NotificationQueueService), identity);

			var notificationType = new NotificationType(NotificationTypes.GrantAgreementAcceptancePastDue, "test");
			var grantApplication = EntityHelper.CreateGrantApplication(user, ApplicationStateInternal.OfferIssued);
			var programNotification = new GrantProgramNotificationType(grantApplication.GrantOpening.GrantStream.GrantProgram, notificationType, new NotificationTemplate());
			grantApplication.GrantOpening.GrantStream.GrantProgram.GrantProgramNotificationTypes.Add(programNotification);
			helper.MockDbSet(notificationType);
			helper.MockDbSet(grantApplication.GrantOpening.GrantStream.GrantProgram.GrantProgramNotificationTypes);
			var mockDbSet = helper.MockDbSet<NotificationScheduleQueue>();
			var service = helper.Create<NotificationQueueService>();

			// Act
			service.AddNotificationEvent(grantApplication, NotificationTypes.GrantAgreementAcceptancePastDue);

			// Assert
			mockDbSet.Verify(
				x => x.Add(It.Is<NotificationScheduleQueue>(n => n.GrantApplicationId == 1 &&
																 n.NotificationTypeId == notificationType.Id &&
																 n.NotificationTicketId.StartsWith("NG000001-T0" + (int)notificationType.Id))));
		}

		[TestMethod]
		[TestCategory("Notification Queue"), TestCategory("Service")]
		public void IsValid_WithNotificationScheduleQueue_ReturnsTrue()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(NotificationQueueService), identity);
			var notificationType = new NotificationType(NotificationTypes.GrantAgreementAcceptancePastDue, "test");
			helper.MockDbSet(new[] { notificationType });
			var service = helper.Create<NotificationQueueService>();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(user, ApplicationStateInternal.OfferIssued);
			grantApplication.GrantAgreement.DateAccepted = AppDateTime.UtcNow;

			// Act
			var result = service.IsValid(new NotificationScheduleQueue(grantApplication, notificationType, new NotificationTemplate(), "test"), out string msg);

			// Assert
			result.Should().BeTrue();
			msg.Should().BeNullOrEmpty();
		}

		[TestMethod]
		[TestCategory("Notification Queue"), TestCategory("Service")]
		public void TryExecute_WithThrownException_ReturnsFalse()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(NotificationQueueService), identity);
			var notificationType = new NotificationType(NotificationTypes.GrantAgreementAcceptancePastDue, "test");
			helper.MockDbSet(new[] { notificationType });
			var service = helper.Create<NotificationQueueService>();

			// Act
			var isSuccess = service.TryExecute(() => { throw new ApplicationException("TEST"); });

			// Assert
			isSuccess.Should().BeFalse();
		}

		[TestMethod]
		[TestCategory("Notification Queue"), TestCategory("Service")]
		public void PushNotificationEvents_WithDate_ReturnsTrue()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(NotificationQueueService), identity);

			var grantApplication = new GrantApplication
			{
				GrantAgreement = new GrantAgreement { StartDate = new DateTime(2017, 1, 1) }
			};
			var notificationType = new NotificationType(NotificationTypes.AgreementAcceptanceConfirmation, "test")
			{
				MilestoneDateName = NotificationQueueService.StringConstants.OfferIssueDate,
				MilestoneDateOffset = -1
			};
			helper.MockDbSet(notificationType);
			var dbSetMock = helper.MockDbSet(
				new NotificationScheduleQueue(grantApplication, notificationType, new NotificationTemplate(), "test")
			);
			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(x => x.NotificationScheduleQueue.AsNoTracking()).Returns(dbSetMock.Object);

			var service = helper.Create<NotificationQueueService>();

			// Act
			var notificationResults = service.PushNotificationEvents(new DateTime(2017, 1, 1), true);

			// Assert
			notificationResults.Should().NotBeNullOrEmpty();
		}

		[TestMethod]
		[TestCategory("Notification Queue"), TestCategory("Service")]
		public void ApprovedAfterStartRule_WithApprovedDateBeforeNotificationDate_ReturnsNull()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(NotificationQueueService), identity);
			var notificationType = new NotificationType(NotificationTypes.GrantAgreementAcceptancePastDue, "test");
			helper.MockDbSet(new[] { notificationType });
			var service = helper.Create<NotificationQueueService>();

			var grantApplication = new GrantApplication()
			{
				GrantAgreement = new GrantAgreement { DateAccepted = new DateTime(2016, 12, 16) },
			};
			grantApplication.TrainingPrograms.Add(new TrainingProgram
			{
				StartDate = new DateTime(2017, 1, 1)
			});

			// Act
			var notificationResults = service
				.ApprovedAfterStartRule(new NotificationScheduleQueue
				{
					GrantApplication = grantApplication,
					NotificationType = new NotificationType()
					{
						MilestoneDateOffset = -15
					}
				});

			// Assert
			notificationResults.Should().BeNullOrEmpty();
		}

		[TestMethod]
		[TestCategory("Notification Queue"), TestCategory("Service")]
		public void ApprovedAfterEndRule_WithApprovedDateBeforeNotificationDate_ReturnsNull()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(NotificationQueueService), identity);
			var notificationType = new NotificationType(NotificationTypes.GrantAgreementAcceptancePastDue, "test");
			helper.MockDbSet(new[] { notificationType });
			var service = helper.Create<NotificationQueueService>();

			var grantApplication = new GrantApplication()
			{
				StartDate = new DateTime(2017, 1, 1),
				EndDate = new DateTime(2017, 1, 2),
				GrantAgreement = new GrantAgreement { DateAccepted = new DateTime(2016, 12, 16) },
			};
			grantApplication.TrainingProviders.Add(new TrainingProvider(grantApplication));
			grantApplication.TrainingPrograms.Add(new TrainingProgram(grantApplication)
			{
				StartDate = new DateTime(2017, 1, 1),
				EndDate = new DateTime(2017, 1, 2)
			});

			// Act
			var notificationResults = service.ApprovedAfterEndRule(new NotificationScheduleQueue
				{
					GrantApplication = grantApplication,
					NotificationType = new NotificationType()
					{
						MilestoneDateOffset = -15
					}
				});

			// Assert
			notificationResults.Should().BeNullOrEmpty();
		}

		[TestMethod]
		[TestCategory("Notification Queue"), TestCategory("Service")]
		public void AddTrainingNotificationEvents_WithAgreementAccepted_CreatesNewNotificationScheduleQueue()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(NotificationQueueService), identity);

			var notificationType = new NotificationType(NotificationTypes.AgreementAcceptanceConfirmation, "test")
			{
				MilestoneDateName = NotificationQueueService.StringConstants.TrainingStartDate
			};
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(user, ApplicationStateInternal.AgreementAccepted);
			var programNotification = new GrantProgramNotificationType(grantApplication.GrantOpening.GrantStream.GrantProgram, notificationType, new NotificationTemplate());
			grantApplication.GrantOpening.GrantStream.GrantProgram.GrantProgramNotificationTypes.Add(programNotification);
			helper.MockDbSet(notificationType);
			helper.MockDbSet(grantApplication.GrantOpening.GrantStream.GrantProgram.GrantProgramNotificationTypes);
			helper.MockDbSet<NotificationScheduleQueue>();
			var service = helper.Create<NotificationQueueService>();

			// Act
			service.AddTrainingNotificationEvents(grantApplication);

			// Assert
			helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Once);
		}

		[TestMethod]
		[TestCategory("Notification Queue"), TestCategory("Service")]
		public void UpdateTrainingDateNotifications_WithSentPastNotifications_CreatesNewScheduledEvents()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(NotificationQueueService), identity);

			var grantApplication = new GrantApplication
			{
				Id = 1,
				StartDate = AppDateTime.UtcNow.AddDays(3),
				EndDate = AppDateTime.UtcNow.AddMonths(3)
			};

			var notificationType1 = new NotificationType(NotificationTypes.GrantAgreementAcceptancePastDue, "test")
			{
				MilestoneDateOffset = 1,
				MilestoneDateName = NotificationQueueService.StringConstants.TrainingStartDate
			};
			var notificationType2 = new NotificationType(NotificationTypes.GrantAgreementAcceptancePastDue, "test")
			{
				MilestoneDateOffset = -4,
				MilestoneDateName = NotificationQueueService.StringConstants.TrainingStartDate
			};
			helper.MockDbSet(new[] { notificationType1, notificationType2 });
			helper.MockDbSet(new List<NotificationScheduleQueue>
			{
				new NotificationScheduleQueue
				{
					GrantApplicationId = grantApplication.Id,
					GrantApplication = grantApplication,
					SendStatus = NotificationSendStatus.Sent,
					NotificationType = notificationType1,
					NotificationTemplate = new NotificationTemplate("test", "test", "test")
				},
				new NotificationScheduleQueue
				{
					GrantApplicationId = grantApplication.Id,
					GrantApplication = grantApplication,
					SendStatus = NotificationSendStatus.Sent,
					NotificationType = notificationType2,
					NotificationTemplate = new NotificationTemplate("test", "test", "test")
				}
			});
			var service = helper.Create<NotificationQueueService>();

			// Act
			service.UpdateTrainingDateNotifications(grantApplication);

			// Assert
			helper.GetMock<DbSet<NotificationScheduleQueue>>().Verify(x => x.Add(It.Is<NotificationScheduleQueue>(nsq => nsq.GrantApplication.Id == 1)), Times.Once);
		}

		[TestMethod]
		[TestCategory("Notification Queue"), TestCategory("Service")]
		public void UpdateTrainingDateNotifications_WithSentNotifications_CreatesNewScheduledEvents()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(NotificationQueueService), identity);

			var grantApplication = new GrantApplication
			{
				Id = 1,
				StartDate = AppDateTime.UtcNow.AddDays(3),
				EndDate = AppDateTime.UtcNow.AddMonths(3)
			};
			var notificationType1 = new NotificationType(NotificationTypes.GrantAgreementAcceptancePastDue, "test")
			{
				MilestoneDateOffset = 1,
				MilestoneDateName = NotificationQueueService.StringConstants.TrainingStartDate
			};
			var notificationType2 = new NotificationType(NotificationTypes.GrantAgreementAcceptancePastDue, "test")
			{
				MilestoneDateOffset = 5,
				MilestoneDateName = NotificationQueueService.StringConstants.TrainingStartDate
			};
			helper.MockDbSet(new[] { notificationType1, notificationType2 });
			helper.MockDbSet(new List<NotificationScheduleQueue>
			{
				new NotificationScheduleQueue
				{
					GrantApplicationId = grantApplication.Id,
					GrantApplication = grantApplication,
					SendStatus = NotificationSendStatus.Sent,
					NotificationType = notificationType1,
					NotificationTemplate = new NotificationTemplate("test", "test", "test")
				},
				new NotificationScheduleQueue
				{
					GrantApplicationId = grantApplication.Id,
					GrantApplication = grantApplication,
					SendStatus = NotificationSendStatus.Sent,
					NotificationType = notificationType2,
					NotificationTemplate = new NotificationTemplate("test", "test", "test")
				}
			});
			var service = helper.Create<NotificationQueueService>();

			// Act
			service.UpdateTrainingDateNotifications(grantApplication);

			// Assert
			helper.GetMock<DbSet<NotificationScheduleQueue>>().Verify(x => x.Add(It.Is<NotificationScheduleQueue>(nsq => nsq.GrantApplication.Id == 1)), Times.Exactly(2));
		}

		[TestMethod]
		[TestCategory("Notification Queue"), TestCategory("Service")]
		public void GetActiveScheduledNotificationEvents_WithDateTheSameASAgreementStart_ReturnsScheduledNotification()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(NotificationQueueService), identity);

			var notificationType = new NotificationType(NotificationTypes.AgreementAcceptanceConfirmation, "test")
			{
				MilestoneDateName = NotificationQueueService.StringConstants.OfferIssueDate,
				MilestoneDateOffset = -1
			};
			helper.MockDbSet(notificationType);
			helper.MockDbSet<NotificationTemplate>();
			var dbSetMock = helper.MockDbSet(
				new NotificationScheduleQueue
				{
					GrantApplication = new GrantApplication
					{
						GrantAgreement = new GrantAgreement {StartDate = new DateTime(2017, 1, 1)}
					},
					NotificationType = notificationType,
					NotificationTypeId = (NotificationTypes)notificationType.Id,
					NotificationTemplate = new NotificationTemplate(),
				}
			);
			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(x => x.NotificationScheduleQueue.AsNoTracking()).Returns(dbSetMock.Object);

			var service = helper.Create<NotificationQueueService>();

			// Act
			var schedueldNotifications = service.GetActiveScheduledNotificationEvents(new DateTime(2017, 1, 1));

			// Assert
			schedueldNotifications.Should().HaveCount(1);
		}
	}
}
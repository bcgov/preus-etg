using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Infrastructure.Entities;
using CJG.Testing.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
    public class GrantOpeningServiceTests : ServiceUnitTestBase
    {
        [TestInitialize]
        public void Setup()
        {
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void AddGrantOpening_WithGrantOpening_CommitedAndDatesSet()
        {
            // Arrange
            AppDateTime.SetNow(DateTime.Now);
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();
            var dateAdded = AppDateTime.UtcNow.Date;
            var grantOpening = EntityHelper.CreateGrantOpening();
            var dbSetMock = helper.MockDbSet<GrantOpening>(grantOpening);

            // Act
            service.Add(grantOpening);

			// Assert
			dbSetMock.Verify(
				x =>
					x.Add(
						It.Is<GrantOpening>(
							g =>
								g.DateAdded.Date == dateAdded && g.GrantOpeningFinancial.DateAdded.Date == dateAdded &&
								g.GrantOpeningIntake.DateAdded.Date == dateAdded)), Times.Once);

			helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Once);
        }

        [TestMethod, TestCategory("Grant Opening"), TestCategory("Service")]
        public void UpdateGrantOpening_WithGrantOpening_Commited()
        {
            // Arrange
            AppDateTime.SetNow(DateTime.Now);
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            var grantOpening = new GrantOpening() { Id = 1, State = GrantOpeningStates.Scheduled };

            helper.MockDbSet(grantOpening);
			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(m => m.OriginalValue(It.IsAny<GrantOpening>(), It.IsAny<string>())).Returns(1);

			// Act
			service.Update(grantOpening);

            // Assert
            helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Once);
        }

        [TestMethod, TestCategory("Grant Opening"), TestCategory("Service")]
        public void UpdateGrantOpening_When_Original_GrantOpening_State_Is_Different_Then_Current_GrantOpening_State_Throws_InvalidOperationException()
        {
            // Arrange
            AppDateTime.SetNow(DateTime.Now);
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();
            var grantOpening = new GrantOpening() { Id = 1, State = GrantOpeningStates.Scheduled };
			helper.MockDbSet(grantOpening);
			var grantOpeningOpenState = new GrantOpening() { Id = 1, State = GrantOpeningStates.Open };
			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(m => m.OriginalValue(It.IsAny<GrantOpening>(), It.IsAny<string>())).Returns(1);

			// Act
			Action action = ()=> service.Update(grantOpeningOpenState);

            // Assert
            action.Should().Throw<InvalidOperationException>();
        }

        [TestMethod, TestCategory("Grant Opening"), TestCategory("Service")]
        public void UpdateGrantOpening_GrantOpeningState_Scheduled_Updated_To_GrantOpeningState_Published()
        {
            // Arrange
            AppDateTime.SetNow(DateTime.Now);
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            var grantOpening = new GrantOpening() { Id = 1, State = GrantOpeningStates.Scheduled, PublishDate = AppDateTime.UtcNow.AddDays(-1), OpeningDate = AppDateTime.UtcNow.AddDays(1) };

            helper.MockDbSet(grantOpening);
			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(m => m.OriginalValue(It.IsAny<GrantOpening>(), It.IsAny<string>())).Returns(1);

			// Act
			service.Update(grantOpening);

            // Assert
            grantOpening.State.Should().Be(GrantOpeningStates.Published);
        }

        [TestMethod, TestCategory("Grant Opening"), TestCategory("Service")]
        public void UpdateGrantOpening_GrantOpeningState_Scheduled_Updated_To_GrantOpeningState_Open()
        {
            // Arrange
            AppDateTime.SetNow(DateTime.Now);
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            var grantOpening = new GrantOpening() {
                Id = 1,
                State = GrantOpeningStates.Scheduled,
                OpeningDate = AppDateTime.UtcNow.AddDays(-1),
                ClosingDate = AppDateTime.UtcNow.AddDays(5)
            };

            helper.MockDbSet(grantOpening);
			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(m => m.OriginalValue(It.IsAny<GrantOpening>(), It.IsAny<string>())).Returns(1);

			// Act
			service.Update(grantOpening);

            // Assert
            grantOpening.State.Should().Be(GrantOpeningStates.Open);
        }

        [TestMethod, TestCategory("Grant Opening"), TestCategory("Service")]
        public void UpdateGrantOpening_GrantOpeningState_Scheduled_Updated_To_GrantOpeningState_Closed()
        {
            // Arrange
            AppDateTime.SetNow(DateTime.Now);
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var grantOpening = EntityHelper.CreateGrantOpening();
            grantOpening.State = GrantOpeningStates.Scheduled;
            grantOpening.ClosingDate = AppDateTime.UtcNow.AddDays(-5);
            grantOpening.State = GrantOpeningStates.Closed;

            helper.MockDbSet(grantOpening);
			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(m => m.OriginalValue(It.IsAny<GrantOpening>(), It.IsAny<string>())).Returns(4);
			var service = helper.Create<GrantOpeningService>();

            // Act
            service.Update(grantOpening);

            // Assert
            grantOpening.State.Should().Be(GrantOpeningStates.Closed);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void ScheduleGrantOpening_ClosingDate_Is_Less_Then_AppDateTime_UtcNow_ToLocalMidnightd_Throws_InvalidOperationException()
        {
            // Arrange
            AppDateTime.SetNow(DateTime.Now);
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            var grantOpening = new GrantOpening() { Id = 1, State = GrantOpeningStates.Scheduled, ClosingDate = AppDateTime.UtcNow.AddDays(-5) };

            helper.MockDbSet(grantOpening);

            // Act
            Action action = () => service.Schedule(grantOpening);

            // Assert
            action.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void ScheduleGrantOpening_GrantOpeningState_Scheduled_Updated_To_GrantOpeningState_Published()
        {
            // Arrange
            AppDateTime.SetNow(DateTime.Now);
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            var grantOpening = new GrantOpening() { Id = 1, State = GrantOpeningStates.Scheduled,
                PublishDate = AppDateTime.UtcNow.AddDays(-5),
                OpeningDate = AppDateTime.UtcNow.AddDays(5),
                ClosingDate = AppDateTime.UtcNow.AddDays(5)};

            helper.MockDbSet(grantOpening);

            // Act
            service.Schedule(grantOpening);

            // Assert
            grantOpening.State.Should().Be(GrantOpeningStates.Published);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void ScheduleGrantOpening_GrantOpeningState_Scheduled_Updated_To_GrantOpeningState_Open()
        {
            // Arrange
            AppDateTime.SetNow(DateTime.Now);
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            var grantOpening = new GrantOpening()
            {
                Id = 1,
                State = GrantOpeningStates.Scheduled,
                OpeningDate = AppDateTime.UtcNow.AddDays(-5),
                ClosingDate = AppDateTime.UtcNow.AddDays(5)
            };

            helper.MockDbSet(grantOpening);

            // Act
            service.Schedule(grantOpening);

            // Assert
            grantOpening.State.Should().Be(GrantOpeningStates.Open);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void ScheduleGrantOpening_GrantOpeningState_Scheduled_Updated_To_GrantOpeningState_Closed()
        {
            // Arrange
            AppDateTime.SetNow(DateTime.Now.ToLocalMidnight());
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            var grantOpening = new GrantOpening()
            {
                Id = 1,
                State = GrantOpeningStates.Scheduled,
                ClosingDate = AppDateTime.UtcNow.AddMinutes(-1)
            };

            helper.MockDbSet(grantOpening);

            // Act
            service.Schedule(grantOpening);

            // Assert
            grantOpening.State.Should().Be(GrantOpeningStates.Closed);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void UnscheduleGrantOpening_When_GrantOpening_State_Is_Not_Scheduled_Throws_InvalidOperationException()
        {
            // Arrange
            AppDateTime.SetNow(DateTime.Now);
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            var grantOpening = new GrantOpening() { Id = 1, State = GrantOpeningStates.Unscheduled };

            helper.MockDbSet(grantOpening);

            // Act
            Action action = () => service.Unschedule(grantOpening);

            // Assert
            action.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void UnscheduleGrantOpening_GrantOpeningState_Scheduled_Updated_To_GrantOpeningState_Unscheduled()
        {
            // Arrange
            AppDateTime.SetNow(DateTime.Now);
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            var grantOpening = new GrantOpening()
            {
                Id = 1,
                State = GrantOpeningStates.Scheduled
            };

            helper.MockDbSet(grantOpening);
			var dbContextMock = helper.GetMock<IDataContext>();

			// Act
			service.Unschedule(grantOpening);

            // Assert
            grantOpening.State.Should().Be(GrantOpeningStates.Unscheduled);
            dbContextMock.Verify(x => x.Update(It.Is<GrantOpening>(y => y.Id == 1)), Times.Once);
            helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Once);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void CloseGrantOpening_When_GrantOpening_State_Is_Unscheduled_Throws_InvalidOperationException()
        {
            // Arrange
            AppDateTime.SetNow(DateTime.Now);
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            var grantOpening = new GrantOpening() { Id = 1, State = GrantOpeningStates.Unscheduled };

            // Act
            Action action = () => service.Close(grantOpening);

            // Assert
            action.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void CloseGrantOpening_GrantOpeningState_Scheduled_Updated_To_GrantOpeningState_Closed()
        {
            // Arrange
            AppDateTime.SetNow(DateTime.Now);
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            var grantOpening = new GrantOpening()
            {
                Id = 1,
                State = GrantOpeningStates.Scheduled,
                ClosingDate = AppDateTime.UtcNow.AddDays(1)
            };

            var dbSetMock = helper.MockDbSet(grantOpening);
			var dbContextMock = helper.GetMock<IDataContext>();

			// Act
			service.Close(grantOpening);

            // Assert
            grantOpening.State.Should().Be(GrantOpeningStates.Closed);
			dbContextMock.Verify(x => x.Update(It.Is<GrantOpening>(y => y.Id == 1)), Times.Once);
            helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Once);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void ReopenGrantOpening_When_GrantOpening_State_Is_Not_Closed_Throws_InvalidOperationException()
        {
            // Arrange
            AppDateTime.SetNow(DateTime.Now);
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            var grantOpening = new GrantOpening() { Id = 1, State = GrantOpeningStates.Scheduled };

            // Act
            Action action = () => service.Reopen(grantOpening);

            // Assert
            action.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void ReopenGrantOpening_GrantOpeningState_Closed_To_GrantOpeningState_Reopen()
        {
            // Arrange
            AppDateTime.SetNow(DateTime.Now);
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            var grantOpening = new GrantOpening()
            {
                Id = 1,
                State = GrantOpeningStates.Closed
            };

            var dbSetMock = helper.MockDbSet(grantOpening);
			var dbContextMock = helper.GetMock<IDataContext>();

			// Act
			service.Reopen(grantOpening);

            // Assert
            grantOpening.State.Should().Be(GrantOpeningStates.Open);
			dbContextMock.Verify(x => x.Update(It.Is<GrantOpening>(y => y.Id == 1)), Times.Once);
            helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Once);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void OpenForSubmitGrantOpening_When_GrantOpening_State_Is_Not_Closed_Throws_InvalidOperationException()
        {
            // Arrange
            AppDateTime.SetNow(DateTime.Now);
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            var grantOpening = new GrantOpening() { Id = 1, State = GrantOpeningStates.Scheduled };

            // Act
            Action action = () => service.OpenForSubmit(grantOpening);

            // Assert
            action.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void OpenForSubmitGrantOpening_GrantOpeningState_Closed_Updated_To_GrantOpeningState_OpenForSubmit()
        {
            // Arrange
            AppDateTime.SetNow(DateTime.Now);
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            var grantOpening = new GrantOpening()
            {
                Id = 1,
                State = GrantOpeningStates.Closed
            };

            helper.MockDbSet(grantOpening);
			var dbContextMock = helper.GetMock<IDataContext>();

			// Act
			service.OpenForSubmit(grantOpening);

            // Assert
            grantOpening.State.Should().Be(GrantOpeningStates.OpenForSubmit);
			dbContextMock.Verify(x => x.Update(It.Is<GrantOpening>(y => y.Id == 1)), Times.Once);
            helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Once);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void AssociatedWithAGrantStream_WithGrantStream_ReturnsOpeningsWithMatchedStreamId()
        {
			// Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
			var dbSetMock = helper.MockDbSet(new GrantOpening { GrantStreamId = 1 });
			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(x => x.GrantOpenings.AsNoTracking()).Returns(dbSetMock.Object);

			var service = helper.Create<GrantOpeningService>();

			// Act and Assert
			service.AssociatedWithAGrantStream(new GrantStream { Id = 1 }).Should().BeTrue();
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void GetGrantOpenings_WithGrantStream_ReturnsOpeningsWithMatchedStreamId()
        {
			// Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();
			var dbSetMock = helper.MockDbSet(new GrantOpening { GrantStreamId = 1 });
			var dbContextMock = helper.GetMock<IDataContext>();

			dbContextMock.Setup(x => x.GrantOpenings.AsNoTracking()).Returns(dbSetMock.Object);

			// Act and Assert
			service.GetGrantOpenings(new GrantStream { Id = 1 }).Should().HaveCount(1);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void GetGrantOpeningsInStates_WithSpecifiedStates_ReturnsOpeningsWithMatchedStatesAndActive()
        {
			// Act
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var dbSetMock = helper.MockDbSet(new[]
            {
                new GrantOpening
                {
                    Id = 1,
                    State = GrantOpeningStates.Open,
                    GrantStream = new GrantStream {IsActive = true}
                },
                new GrantOpening
                {
                    Id = 2,
                    State = GrantOpeningStates.Published,
                    GrantStream = new GrantStream {IsActive = true}
                },
                new GrantOpening
                {
                    Id = 3,
                    State = GrantOpeningStates.Scheduled,
                    GrantStream = new GrantStream {IsActive = true}
                },
                new GrantOpening
                {
                    Id = 4,
                    State = GrantOpeningStates.Scheduled,
                    GrantStream = new GrantStream {IsActive = false}
                }
            });

			var service = helper.Create<GrantOpeningService>();
			
			// Act
			var result =
                helper.Create<GrantOpeningService>()
                    .GetGrantOpeningsInStates(new[] { GrantOpeningStates.Open, GrantOpeningStates.Scheduled });

			// Assert
            result.Should().HaveCount(2);
            result.Where(x => x.Id == 1).Should().HaveCount(1);
            result.Where(x => x.Id == 3).Should().HaveCount(1);
        }

		//[TestMethod]
		//[TestCategory("Grant Opening"), TestCategory("Service")]
		//public void GetGrantOpenings()
		//{
		//	// Arrange
		//	AppDateTime.SetNow(DateTime.Now);
		//	var user = EntityHelper.CreateExternalUser();
		//	var identity = HttpHelper.CreateIdentity(user);
		//	var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
		//	var grantOpening = EntityHelper.CreateGrantOpeningWithGrantApplications();
		//	var dbSetMock = helper.MockDbSet(new List<GrantOpening>
		//	{
		//		new GrantOpening
		//		{
		//			Id = 1,
		//			State = GrantOpeningStates.Open,
		//			//ClosingDate = new DateTime(2020, 1, 1),
		//			ClosingDate = new DateTime(2021, 1, 1),
		//			GrantStream = new GrantStream
		//			{
		//				Id = 1,
		//				IsActive = true,
		//				Name = "Grant STream Name",
		//				GrantProgram = new GrantProgram
		//				{
		//					State = GrantProgramStates.Implemented,
		//					Name = "Grant Program Name"
		//				},
		//				GrantProgramId = 2
		//			},
		//			TrainingPeriod = new TrainingPeriod {StartDate = AppDateTime.UtcNow }
		//		}
		//	});
		//	var grantId = 2;
		//	var chkDate = AppDateTime.UtcNow;
		//	var dbContextMock = helper.GetMock<IDataContext>();
		//	dbContextMock.Setup(x => x.GrantOpenings.AsNoTracking()).Returns(dbSetMock.Object);

		//	var service = helper.Create<GrantOpeningService>();

		//	// Act
		//	var result = service.GetGrantOpenings(chkDate, grantId).ToList();

		//	// Assert
		//	result.Count().Should().Be(1);
		//}

		[TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void GetPublishedGrantOpenings_WithSpecifiedDate_ReturnsPublishedAndNotClosedOnTheDate()
        {
            // Arrange
            AppDateTime.SetNow(DateTime.Now);
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            var dbSetMock = helper.MockDbSet(new[]
            {
                new GrantOpening
                {
                    Id = 1,
                    State = GrantOpeningStates.Open,
                    ClosingDate = new AppDateTime(2017, 1, 1),
                    GrantStream = new GrantStream {IsActive = true}
                },
                new GrantOpening
                {
                    Id = 2,
                    State = GrantOpeningStates.Published,
                    ClosingDate = new AppDateTime(2017, 1, 29),
                    GrantStream = new GrantStream {IsActive = true}
                },
                new GrantOpening
                {
                    Id = 3,
                    State = GrantOpeningStates.Published,
                    ClosingDate = new AppDateTime(2017, 2, 1),
                    GrantStream = new GrantStream {IsActive = true}
                },
                new GrantOpening
                {
                    Id = 4,
                    State = GrantOpeningStates.Scheduled,
                    ClosingDate = new AppDateTime(2017, 3, 1),
                    GrantStream = new GrantStream {IsActive = false}
                }
            });
			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(x => x.GrantOpenings.AsNoTracking()).Returns(dbSetMock.Object);


			// Act
			var result = service.GetPublishedGrantOpenings(new AppDateTime(2017, 1, 31));

            // Assert
            result.Should().HaveCount(1);
            result.First().Id.Should().Be(3);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void GetGrantOpenings_WithSpecifiedDate_ReturnsPublishedAndNotClosedOnTheDate()
        {
            AppDateTime.SetNow(DateTime.Now);
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet(new[]
            {
                new GrantOpening
                {
                    Id = 1,
                    State = GrantOpeningStates.Open,
                    ClosingDate = new DateTime(2017, 1, 1),
                    GrantStream = new GrantStream("name", "criteria", new GrantProgram(), new AccountCode()) { IsActive = true },
                    TrainingPeriod = new TrainingPeriod()
                },
                new GrantOpening
                {
                    Id = 2,
                    State = GrantOpeningStates.Published,
                    ClosingDate = new DateTime(2017, 1, 29),
                    GrantStream = new GrantStream("name", "criteria", new GrantProgram(), new AccountCode()) { IsActive = true },
                    TrainingPeriod = new TrainingPeriod()
                },
                new GrantOpening
                {
                    Id = 3,
                    State = GrantOpeningStates.Published,
                    ClosingDate = new DateTime(2017, 2, 1),
                    GrantStream = new GrantStream("name", "criteria", new GrantProgram(), new AccountCode()) { IsActive = true },
                    TrainingPeriod = new TrainingPeriod()
                },
                new GrantOpening
                {
                    Id = 4,
                    State = GrantOpeningStates.Scheduled,
                    ClosingDate = new DateTime(2017, 3, 1),
                    GrantStream = new GrantStream("name", "criteria", new GrantProgram(), new AccountCode()) { IsActive = true },
                    TrainingPeriod = new TrainingPeriod()
                }
            });

            var result = service.GetGrantOpenings(new DateTime(2017, 1, 31));

            result.Should().HaveCount(1);
            result.First().Id.Should().Be(3);
        }


        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void GetGrantOpenings_WithSpecifiedDateAndStream_ReturnsPublishedAndNotClosedOnTheDate()
        {
			// Arrange
            AppDateTime.SetNow(DateTime.Now);
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var dbSetMock = helper.MockDbSet(new[]
            {
                new GrantOpening
                {
                    Id = 1,
                    State = GrantOpeningStates.Open,
                    ClosingDate = new DateTime(2017, 1, 1),
                    GrantStream = new GrantStream("name", "criteria", new GrantProgram(), new AccountCode()) { IsActive = true },
                    TrainingPeriod = new TrainingPeriod()
                },
                new GrantOpening
                {
                    Id = 2,
                    State = GrantOpeningStates.Published,
                    ClosingDate = new DateTime(2017, 1, 29),
                    GrantStream = new GrantStream("name", "criteria", new GrantProgram(), new AccountCode()) { IsActive = true },
                    TrainingPeriod = new TrainingPeriod()
                },
                new GrantOpening
                {
                    Id = 3,
                    State = GrantOpeningStates.Published,
                    ClosingDate = new DateTime(2017, 2, 1),
                    GrantStream = new GrantStream("name", "criteria", new GrantProgram(), new AccountCode()) { IsActive = true },
                    GrantStreamId = 1,
                    TrainingPeriodId = 2,
                    TrainingPeriod = new TrainingPeriod()
                },
                new GrantOpening
                {
                    Id = 4,
                    State = GrantOpeningStates.OpenForSubmit,
                    GrantStream = new GrantStream("name", "criteria", new GrantProgram(), new AccountCode()) { IsActive = true },
                    GrantStreamId = 1,
                    TrainingPeriodId = 2,
                    TrainingPeriod = new TrainingPeriod()
                },
                new GrantOpening
                {
                    Id = 5,
                    State = GrantOpeningStates.OpenForSubmit,
                    GrantStream = new GrantStream("name", "criteria", new GrantProgram(), new AccountCode()) { IsActive = true },
                    GrantStreamId = 4,
                    TrainingPeriodId = 2,
                    TrainingPeriod = new TrainingPeriod()
                },
                new GrantOpening
                {
                    Id = 6,
                    State = GrantOpeningStates.Scheduled,
                    ClosingDate = new DateTime(2017, 3, 1),
                    GrantStream = new GrantStream("name", "criteria", new GrantProgram(), new AccountCode()) { IsActive = true },
                    TrainingPeriod = new TrainingPeriod()
                }
            });

			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(x => x.GrantOpenings.AsNoTracking()).Returns(dbSetMock.Object);

			var service = helper.Create<GrantOpeningService>();

			// Act
			var result = service.GetGrantOpenings(new DateTime(2017, 1, 31), 1, 2)?.ToList();

			// Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result?.Where(x => x.Id == 4).Should().HaveCount(1);
            result?.Where(x => x.Id == 3).Should().HaveCount(1);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void DeleteGrantOpening_WithExistingGrantOpeningId_Deletes()
        {
			// Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            var grantOpening = new GrantOpening
            {
                Id = 1,
                State = GrantOpeningStates.Unscheduled,
                GrantApplications = new List<GrantApplication>()
            };

			var dbSetMock = helper.MockDbSet<GrantOpening>(grantOpening);

			// Act
            service.Delete(1);

			// Assert
            dbSetMock.Verify(x => x.Remove(It.Is<GrantOpening>(go => go.Id == 1)), Times.Once);
            helper.GetMock<IDataContext>().Verify(x => x.Commit(), Times.Once);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void DeleteGrantOpening_WithScheduledGrantOpeningId_ThrowsError()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            var grantOpening = new GrantOpening
            {
                Id = 1,
                State = GrantOpeningStates.Scheduled,
                GrantApplications = new List<GrantApplication>()
            };

			helper.MockDbSet<GrantOpening>(grantOpening);

			// Act
			Action action = () => service.Delete(1);

            // Assert
            action.Should().Throw<InvalidOperationException>().Where(x => x.Message.Contains("Scheduled"));
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void DeleteGrantOpening_WithGrantOpeningLinkedApplications_ThrowsError()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            var grantOpening = new GrantOpening
            {
                Id = 1,
                State = GrantOpeningStates.Unscheduled,
                GrantApplications = new List<GrantApplication> { new GrantApplication() }
            };

			helper.MockDbSet<GrantOpening>(grantOpening);

			// Act
			Action action = () => service.Delete(1);

            // Assert
            action.Should().Throw<InvalidOperationException>().Where(x => x.Message == "Cannot delete grant opening with linked applications");
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void MakeReservation_WithGrantOpeningLinkedApplications_ReturnsReservedApplicationIds()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            var grantOpening = EntityHelper.CreateGrantOpening();
            grantOpening.State = GrantOpeningStates.Unscheduled;
            grantOpening.IntakeTargetAmt = 80;
            grantOpening.GrantOpeningIntake = new GrantOpeningIntake()
            {
                UnderAssessmentAmt = 30,
                UnderAssessmentCount = 10
            };
            grantOpening.GrantOpeningFinancial = new GrantOpeningFinancial()
            {
                CurrentReservations = 0,
                AssessedCommitments = 0
            };

            var grantApplications = new List<GrantApplication>()
            {
                new GrantApplication()
                {
                    Id = 1,
                    GrantOpeningId = 1,
                    GrantOpening = grantOpening,
                    DateSubmitted = AppDateTime.UtcNow.AddMinutes(2),
                    TrainingCost = new TrainingCost()
                    {
                        TotalEstimatedReimbursement = 10
                    },
                    ApplicationStateInternal = ApplicationStateInternal.New
                },
                new GrantApplication()
                {
                    Id = 3,
                    GrantOpeningId = 1,
                    GrantOpening = grantOpening,
                    DateSubmitted = AppDateTime.UtcNow,
                    TrainingCost = new TrainingCost()
                    {
                        TotalEstimatedReimbursement = 20
                    },
                    ApplicationStateInternal = ApplicationStateInternal.New
                },
            };

            helper.MockDbSet(grantOpening);
            helper.MockDbSet(grantApplications);
			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(x => x.Validate<GrantApplication>(It.IsAny<GrantApplication>())).Returns(new List<ValidationResult>());

			// Act
			var results = service.MakeReservation(grantApplications[0]);

            // Assert
            results.Count().Should().Be(2);
            grantOpening.GrantOpeningIntake.NewCount.Should().Be(-2);
            grantOpening.GrantOpeningIntake.NewAmt.Should().Be(-30);
            grantOpening.GrantOpeningIntake.PendingAssessmentCount.Should().Be(2);
            grantOpening.GrantOpeningIntake.PendingAssessmentAmt.Should().Be(30);
            grantOpening.GrantOpeningFinancial.CurrentReservations.Should().Be(30);

        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void MakeReservation_WithGrantOpeningLinkedApplications_ThrowsError()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            var grantOpening = EntityHelper.CreateGrantOpening();
            grantOpening.State = GrantOpeningStates.Unscheduled;
            grantOpening.IntakeTargetAmt = 0;
            grantOpening.GrantOpeningFinancial = new GrantOpeningFinancial()
            {
                CurrentReservations = 0,
                AssessedCommitments = 0
            };
            var grantApplications = new List<GrantApplication>()
            {
                new GrantApplication()
                {
                    Id = 1,
                    GrantOpeningId = 1,
                    GrantOpening = grantOpening,
                    DateSubmitted =DateTime.Now,
                    TrainingCost = new TrainingCost()
                    {
                        TotalEstimatedReimbursement = 10
                    },
                    TrainingPrograms = new List<TrainingProgram>()
                    {
                        new TrainingProgram(){ },
                        new TrainingProgram(){ }
                    },
                    ApplicationStateInternal = ApplicationStateInternal.New
                },
                new GrantApplication()
                {
                    Id =2,
                    GrantOpeningId = 2,
                    GrantOpening = grantOpening,
                    DateSubmitted =DateTime.Now,
                    TrainingPrograms = new List<TrainingProgram>(),
                    TrainingCost = new TrainingCost()
                    {
                        TotalEstimatedReimbursement = 10
                    }
                },
                new GrantApplication()
                {
                    Id = 3,
                    GrantOpeningId = 1,
                    GrantOpening = grantOpening,
                    DateSubmitted =DateTime.Now,
                    TrainingCost = new TrainingCost()
                    {
                        TotalEstimatedReimbursement = 10
                    },
                    TrainingPrograms = new List<TrainingProgram>()
                    {
                        new TrainingProgram(){ },
                        new TrainingProgram(){ }
                    },
                },
            };

            helper.MockDbSet(grantOpening);
            helper.MockDbSet(grantApplications);

            // Act
            Action action = () => service.MakeReservation(grantApplications[0]);

            // Assert
            action.Should().Throw<Exception>().Where(x => x.Message == "Insufficient funds to make the reservation requested");
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void DeleteGrantOpening_WithInvalidGrantOpeningId_ThrowsError()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            var grantOpening = new GrantOpening
            {
                Id = 0,
                State = GrantOpeningStates.Unscheduled,
                GrantApplications = new List<GrantApplication> { new GrantApplication() }
            };

            helper.MockDbSet<GrantOpening>(grantOpening);

            // Assert
            Action action = () => service.Delete(grantOpening.Id);

            // Act
            action.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void DeleteGrantOpening_FindingGrantOpening_ById_Returns_Null_And_ThrowsError()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();
			var grantOpening = new GrantOpening
            {
                Id = 1,
                State = GrantOpeningStates.Unscheduled,
                GrantApplications = new List<GrantApplication> { new GrantApplication() }
            };

			helper.MockDbSet<GrantOpening>();

            // Act
            Action action = () => service.Delete(1);

            // Assert
            action.Should().Throw<NoContentException>();
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void DeleteGrantOpening_WithGrantOpeningIntake_NotNull_Remove_Is_Called()
        {

            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            var grantOpening = new GrantOpening
            {
                Id = 1,
                State = GrantOpeningStates.Unscheduled,
                GrantApplications = new List<GrantApplication>(),
                GrantOpeningIntake = new GrantOpeningIntake()

            };

            helper.MockDbSet<GrantOpening>(grantOpening);
            var dbSetMock = helper.MockDbSet<GrantOpeningIntake>();

            // Act
            service.Delete(1);

			// Assert
			dbSetMock.Verify(x => x.Remove(It.Is<GrantOpeningIntake>(go => grantOpening.GrantOpeningIntake == grantOpening.GrantOpeningIntake)), Times.Once);
            helper.GetMock<IDataContext>().Verify(x => x.Commit(), Times.Once);
        }


        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void DeleteGrantOpening_WithGrantOpeningFinancials_NotNull_Remove_Is_Called()
        {

            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            var grantOpening = new GrantOpening
            {
                Id = 1,
                State = GrantOpeningStates.Unscheduled,
                GrantApplications = new List<GrantApplication>(),
                GrantOpeningFinancial = new GrantOpeningFinancial()

            };

            helper.MockDbSet<GrantOpening>(grantOpening);
			var dbSetMock = helper.MockDbSet<GrantOpeningFinancial>();

            // Act
            service.Delete(1);

			// Assert
			dbSetMock.Verify(x => x.Remove(It.Is<GrantOpeningFinancial>(go => grantOpening.GrantOpeningFinancial == grantOpening.GrantOpeningFinancial)), Times.Once);
            helper.GetMock<IDataContext>().Verify(x => x.Commit(), Times.Once);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void GetGrantOpening_By_GrantStreamId_And_TrainingPeriodId_Returns_Result()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var dbSetMock = helper.MockDbSet(new[]
            {
                new GrantOpening
                {
                    Id = 1,
                    GrantStreamId = 2,
                    TrainingPeriodId = 3,
                    GrantStream = new GrantStream {Id = 2 },
                    TrainingPeriod = new TrainingPeriod {Id = 3 }
                },
                new GrantOpening
                {
                    Id = 2,
                    GrantStreamId = 3,
                    TrainingPeriodId = 4,
                    GrantStream = new GrantStream {Id = 3 },
                    TrainingPeriod = new TrainingPeriod {Id = 4 }
                }
            });
			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(x => x.GrantOpenings.AsNoTracking()).Returns(dbSetMock.Object);

            var service = helper.Create<GrantOpeningService>();

			// Act
			var result = service.GetGrantOpening(2, 3);

            // Assert
            result.GrantStreamId.Should().Be(2);
            result.TrainingPeriodId.Should().Be(3);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void GetGrantOpening_By_GrantStreamId_And_TrainingPeriodId_Returns_Null()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var dbSetMock = helper.MockDbSet(new[]
            {
                new GrantOpening
                {
                    Id = 1,
                    GrantStreamId = 2,
                    TrainingPeriodId = 3,
                    GrantStream = new GrantStream {Id = 2 },
                    TrainingPeriod = new TrainingPeriod {Id = 3 }
                },
                new GrantOpening
                {
                    Id = 2,
                    GrantStreamId = 3,
                    TrainingPeriodId = 4,
                    GrantStream = new GrantStream {Id = 3 },
                    TrainingPeriod = new TrainingPeriod {Id = 4 }
                }
            });
			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(x => x.GrantOpenings.AsNoTracking()).Returns(dbSetMock.Object);
			
			var service = helper.Create<GrantOpeningService>();

            // Act
            var result = service.GetGrantOpening(22, 33);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void Get_GrantOpening_By_Id()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            var grantOpening = new GrantOpening
            {
                Id = 1,
                State = GrantOpeningStates.Open,
                GrantApplications = new List<GrantApplication> { new GrantApplication() }
            };
            helper.MockDbSet(grantOpening);

            // Act
            var result = service.Get(grantOpening.Id);


            // Assert
            result.Should().BeOfType<GrantOpening>();
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.State.Should().Be(GrantOpeningStates.Open);

        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void Get_GrantOpening_By_Id_Throws_Exception()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            var grantOpening = new GrantOpening
            {
                Id = 1,
                State = GrantOpeningStates.Open,
                GrantApplications = new List<GrantApplication> { new GrantApplication() }
            };
			helper.MockDbSet<GrantOpening>();

            // Act
            Action action = () => service.Get(grantOpening.Id);

            // Assert
            action.Should().Throw<Exception>();

        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void MakeReservation_Insufficient_Funds_In_GrantOpening_To_Cover_Request_Throws_Insufficient_Funds_Exception()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var grantOpening = EntityHelper.CreateGrantOpeningWithGrantApplications();
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet(grantOpening.GrantApplications);
            helper.MockDbSet(grantOpening);

			// Act
			Action action = () => service.MakeReservation(grantOpening.GrantApplications.Where(x => x.ApplicationStateInternal == ApplicationStateInternal.New).FirstOrDefault());

            // Assert
            action.Should().Throw<DbEntityValidationException>().Where(x => x.Message.Contains("Insufficient funds to make the reservation requested"));
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void MakeReservation_Reserved_Application_Id_Returns_One()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var grantOpening = EntityHelper.CreateGrantOpeningWithGrantApplications(100);//pass IntakeAmt
            var grantApplication = EntityHelper.CreateGrantApplicationWithClaim(grantOpening);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet(grantOpening.GrantApplications);
            helper.MockDbSet(grantOpening);

            grantApplication.Id = 2;
            grantApplication.Claims.FirstOrDefault().GrantApplication = grantApplication;
			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(x => x.Validate<GrantApplication>(It.IsAny<GrantApplication>())).Returns(new List<ValidationResult>());

            // Act
            var result = service.MakeReservation(grantApplication);

            // Assert
            result.Count.Should().Be(2);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void MakeReservation_Reserved_Application_Id_Returns_Zero()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var grantOpening = EntityHelper.CreateGrantOpeningWithGrantApplications(100); //pass IntakeAmt
            var grantApplication = EntityHelper.CreateGrantApplicationWithClaim(grantOpening);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet(grantOpening);
            helper.MockDbSet(grantOpening.GrantApplications);

            grantApplication.Claims.FirstOrDefault().GrantApplication = grantApplication;
			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(x => x.Validate<GrantApplication>(It.IsAny<GrantApplication>())).Returns(new List<ValidationResult>());

			// Act
			var result = service.MakeReservation(grantApplication);

            // Assert
            result.Count.Should().Be(1);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void AdjustFinancialStatements_WithGrantApplication()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var grantApplication = EntityHelper.CreateGrantApplicationWithClaim();
            var grantOpening = EntityHelper.CreateGrantOpeningWithGrantApplications();
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet<GrantOpening>(grantOpening);

            grantOpening.GrantOpeningIntake.UnderAssessmentCount = 10;
            grantOpening.GrantOpeningIntake.UnderAssessmentAmt = 10;
            grantOpening.GrantOpeningFinancial.CurrentReservations = 10;
            grantOpening.GrantOpeningIntake.WithdrawnCount = 10;
            grantOpening.GrantOpeningIntake.WithdrawnAmt = 10;

            var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();
            grantApplication.Claims.FirstOrDefault().GrantApplication = grantApplication;

            // Act
            service.AdjustFinancialStatements(grantApplication,ApplicationStateInternal.ReturnedToAssessment,ApplicationWorkflowTrigger.WithdrawApplication);

            // Assert
            grantOpening.GrantOpeningIntake.UnderAssessmentCount.Should().Be(9);
            grantOpening.GrantOpeningIntake.UnderAssessmentAmt.Should().Be(8);
            grantOpening.GrantOpeningFinancial.CurrentReservations.Should().Be(8);
            grantOpening.GrantOpeningIntake.WithdrawnCount.Should().Be(11);
            grantOpening.GrantOpeningIntake.WithdrawnAmt.Should().Be(12);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void AdjustFinancialStatements_Null_GrantOpening_Throws_Invalid_Operation_Exception()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var grantApplication = EntityHelper.CreateGrantApplicationWithClaim();
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet((GrantOpening)null);

            var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();
            grantApplication.Claims.FirstOrDefault().GrantApplication = grantApplication;

            // Act
            Action action = () => service.AdjustFinancialStatements(grantApplication, ApplicationStateInternal.ReturnedToAssessment, ApplicationWorkflowTrigger.WithdrawApplication);

            // Assert
            action.Should().Throw<NoContentException>();
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void AdjustFinancialStatements_ApplicationWorkflowTrigger_Case_SubmitApplication()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var grantOpening = EntityHelper.CreateGrantOpeningWithGrantApplications();
            var grantApplication = EntityHelper.CreateGrantApplicationWithClaim();
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet<GrantOpening>(grantOpening);

            grantOpening.GrantOpeningIntake.NewCount = 10;
            grantOpening.GrantOpeningIntake.NewAmt = 10;

            var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();
            grantApplication.Claims.FirstOrDefault().GrantApplication = grantApplication;

            // Act
            service.AdjustFinancialStatements(grantApplication, ApplicationStateInternal.New, ApplicationWorkflowTrigger.SubmitApplication);

            // Assert
            grantOpening.GrantOpeningIntake.NewCount.Should().Be(11);
            grantOpening.GrantOpeningIntake.NewAmt.Should().Be(12);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void AdjustFinancialStatements_ApplicationWorkflowTrigger_Case_SelectForAssessment()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var grantOpening = EntityHelper.CreateGrantOpeningWithGrantApplications();
            var grantApplication = EntityHelper.CreateGrantApplicationWithClaim();
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet<GrantOpening>(grantOpening);

            grantOpening.GrantOpeningIntake.NewCount = 10;
            grantOpening.GrantOpeningIntake.NewAmt = 10;

            grantOpening.GrantOpeningIntake.PendingAssessmentCount = 10;
            grantOpening.GrantOpeningIntake.PendingAssessmentAmt = 10;

            grantOpening.GrantOpeningFinancial.CurrentReservations = 10;

            var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();
            grantApplication.Claims.FirstOrDefault().GrantApplication = grantApplication;

            // Act
            service.AdjustFinancialStatements(grantApplication, ApplicationStateInternal.New, ApplicationWorkflowTrigger.SelectForAssessment);

            // Assert
            grantOpening.GrantOpeningIntake.NewCount.Should().Be(9);
            grantOpening.GrantOpeningIntake.NewAmt.Should().Be(8);

            grantOpening.GrantOpeningIntake.PendingAssessmentCount.Should().Be(11);
            grantOpening.GrantOpeningIntake.PendingAssessmentAmt.Should().Be(12);

            grantOpening.GrantOpeningFinancial.CurrentReservations.Should().Be(12);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void AdjustFinancialStatements_ApplicationWorkflowTrigger_Case_RemoveFromAssessment()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var grantOpening = EntityHelper.CreateGrantOpeningWithGrantApplications();
            var grantApplication = EntityHelper.CreateGrantApplicationWithClaim();
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet<GrantOpening>(grantOpening);

            grantOpening.GrantOpeningIntake.PendingAssessmentCount = 10;
            grantOpening.GrantOpeningIntake.PendingAssessmentAmt = 10;

            grantOpening.GrantOpeningIntake.NewCount = 10;
            grantOpening.GrantOpeningIntake.NewAmt = 10;

            grantOpening.GrantOpeningFinancial.CurrentReservations = 10;

            var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();
            grantApplication.Claims.FirstOrDefault().GrantApplication = grantApplication;

            // Act
            service.AdjustFinancialStatements(grantApplication, ApplicationStateInternal.New, ApplicationWorkflowTrigger.RemoveFromAssessment);

            // Assert
            grantOpening.GrantOpeningIntake.PendingAssessmentCount.Should().Be(9);
            grantOpening.GrantOpeningIntake.PendingAssessmentAmt.Should().Be(8);

            grantOpening.GrantOpeningIntake.NewCount.Should().Be(11);
            grantOpening.GrantOpeningIntake.NewAmt.Should().Be(12);

            grantOpening.GrantOpeningFinancial.CurrentReservations.Should().Be(8);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void AdjustFinancialStatements_ApplicationWorkflowTrigger_Case_BeginAssessment()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var grantOpening = EntityHelper.CreateGrantOpeningWithGrantApplications();
            var grantApplication = EntityHelper.CreateGrantApplicationWithClaim();
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet<GrantOpening>(grantOpening);

            grantOpening.GrantOpeningIntake.PendingAssessmentCount = 10;
            grantOpening.GrantOpeningIntake.PendingAssessmentAmt = 10;

            grantOpening.GrantOpeningIntake.UnderAssessmentCount = 10;
            grantOpening.GrantOpeningIntake.UnderAssessmentAmt = 10;

            var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();
            grantApplication.Claims.FirstOrDefault().GrantApplication = grantApplication;

            // Act
            service.AdjustFinancialStatements(grantApplication, ApplicationStateInternal.New, ApplicationWorkflowTrigger.BeginAssessment);

            // Assert
            grantOpening.GrantOpeningIntake.PendingAssessmentCount.Should().Be(9);
            grantOpening.GrantOpeningIntake.PendingAssessmentAmt.Should().Be(8);

            grantOpening.GrantOpeningIntake.UnderAssessmentCount.Should().Be(11);
            grantOpening.GrantOpeningIntake.UnderAssessmentAmt.Should().Be(12);
        }
        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void AdjustFinancialStatements_ApplicationWorkflowTrigger_Case_DenyApplication()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var grantOpening = EntityHelper.CreateGrantOpeningWithGrantApplications();
            var grantApplication = EntityHelper.CreateGrantApplicationWithClaim();
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet<GrantOpening>(grantOpening);

            grantOpening.GrantOpeningIntake.UnderAssessmentCount = 10;
            grantOpening.GrantOpeningIntake.UnderAssessmentAmt = 10;

            grantOpening.GrantOpeningIntake.DeniedCount = 10;
            grantOpening.GrantOpeningIntake.DeniedAmt = 10;

            grantOpening.GrantOpeningFinancial.CurrentReservations = 10;

            var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();
            grantApplication.Claims.FirstOrDefault().GrantApplication = grantApplication;


            // Act
            service.AdjustFinancialStatements(grantApplication, ApplicationStateInternal.New, ApplicationWorkflowTrigger.DenyApplication);

            // Assert
            grantOpening.GrantOpeningIntake.UnderAssessmentCount.Should().Be(9);
            grantOpening.GrantOpeningIntake.UnderAssessmentAmt.Should().Be(8);

            grantOpening.GrantOpeningIntake.DeniedCount.Should().Be(11);
            grantOpening.GrantOpeningIntake.DeniedAmt.Should().Be(12);

            grantOpening.GrantOpeningFinancial.CurrentReservations.Should().Be(8);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void AdjustFinancialStatements_ApplicationWorkflowTrigger_Case_RejectGrantAgreement_Triggers_WithdrawOffer()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var grantOpening = EntityHelper.CreateGrantOpeningWithGrantApplications();
            var grantApplication = EntityHelper.CreateGrantApplicationWithClaim();
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet<GrantOpening>(grantOpening);

            grantOpening.GrantOpeningIntake.UnderAssessmentCount = 10;
            grantOpening.GrantOpeningIntake.UnderAssessmentAmt = 10;

            grantOpening.GrantOpeningIntake.WithdrawnCount = 10;
            grantOpening.GrantOpeningIntake.WithdrawnAmt = 10;

            grantOpening.GrantOpeningFinancial.CurrentReservations = 10;

            var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();
            grantApplication.Claims.FirstOrDefault().GrantApplication = grantApplication;


            // Act
            service.AdjustFinancialStatements(grantApplication, ApplicationStateInternal.New, ApplicationWorkflowTrigger.RejectGrantAgreement);

            // Assert
            grantOpening.GrantOpeningIntake.UnderAssessmentCount.Should().Be(9);
            grantOpening.GrantOpeningIntake.UnderAssessmentAmt.Should().Be(8);

            grantOpening.GrantOpeningIntake.WithdrawnCount.Should().Be(11);
            grantOpening.GrantOpeningIntake.WithdrawnAmt.Should().Be(12);

            grantOpening.GrantOpeningFinancial.CurrentReservations.Should().Be(8);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void AdjustFinancialStatements_ApplicationWorkflowTrigger_Case_WithdrawApplication_With_New_Internal_State()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var grantOpening = EntityHelper.CreateGrantOpeningWithGrantApplications();
            var grantApplication = EntityHelper.CreateGrantApplicationWithClaim();
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet<GrantOpening>(grantOpening);

            grantOpening.GrantOpeningIntake.NewCount = 10;
            grantOpening.GrantOpeningIntake.NewAmt = 10;

            var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();
            grantApplication.Claims.FirstOrDefault().GrantApplication = grantApplication;

            // Act
            service.AdjustFinancialStatements(grantApplication, ApplicationStateInternal.New, ApplicationWorkflowTrigger.WithdrawApplication);

            // Assert
            grantOpening.GrantOpeningIntake.NewCount.Should().Be(9);
            grantOpening.GrantOpeningIntake.NewAmt.Should().Be(8);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void AdjustFinancialStatements_ApplicationWorkflowTrigger_Case_WithdrawApplication_With_PendingAssessment_Internal_State()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var grantOpening = EntityHelper.CreateGrantOpeningWithGrantApplications();
            var grantApplication = EntityHelper.CreateGrantApplicationWithClaim();
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet<GrantOpening>(grantOpening);

            grantOpening.GrantOpeningIntake.PendingAssessmentCount = 10;
            grantOpening.GrantOpeningIntake.PendingAssessmentAmt = 10;

            grantOpening.GrantOpeningFinancial.CurrentReservations = 10;

            grantOpening.GrantOpeningIntake.WithdrawnCount = 10;
            grantOpening.GrantOpeningIntake.WithdrawnAmt = 10;

            var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();
            grantApplication.Claims.FirstOrDefault().GrantApplication = grantApplication;

            // Act
            service.AdjustFinancialStatements(grantApplication, ApplicationStateInternal.PendingAssessment, ApplicationWorkflowTrigger.WithdrawApplication);

            // Assert
            grantOpening.GrantOpeningIntake.PendingAssessmentCount.Should().Be(9);
            grantOpening.GrantOpeningIntake.PendingAssessmentAmt.Should().Be(8);

            grantOpening.GrantOpeningFinancial.CurrentReservations.Should().Be(8);

            grantOpening.GrantOpeningIntake.WithdrawnCount.Should().Be(11);
            grantOpening.GrantOpeningIntake.WithdrawnAmt.Should().Be(12);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void AdjustFinancialStatements_ApplicationWorkflowTrigger_Case_WithdrawApplication_With_UnderAssessment_Internal_State()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var grantOpening = EntityHelper.CreateGrantOpeningWithGrantApplications();
            var grantApplication = EntityHelper.CreateGrantApplicationWithClaim();
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet<GrantOpening>(grantOpening);

            grantOpening.GrantOpeningIntake.UnderAssessmentCount = 10;
            grantOpening.GrantOpeningIntake.UnderAssessmentAmt = 10;

            grantOpening.GrantOpeningFinancial.CurrentReservations = 10;

            grantOpening.GrantOpeningIntake.WithdrawnCount = 10;
            grantOpening.GrantOpeningIntake.WithdrawnAmt = 10;

            var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();
            grantApplication.Claims.FirstOrDefault().GrantApplication = grantApplication;

            // Act
            service.AdjustFinancialStatements(grantApplication, ApplicationStateInternal.UnderAssessment, ApplicationWorkflowTrigger.WithdrawApplication);

            // Assert

            grantOpening.GrantOpeningIntake.UnderAssessmentCount.Should().Be(9);
            grantOpening.GrantOpeningIntake.UnderAssessmentAmt.Should().Be(8);

            grantOpening.GrantOpeningFinancial.CurrentReservations.Should().Be(8);

            grantOpening.GrantOpeningIntake.WithdrawnCount.Should().Be(11);
            grantOpening.GrantOpeningIntake.WithdrawnAmt.Should().Be(12);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void AdjustFinancialStatements_ApplicationWorkflowTrigger_Case_AcceptGrantAgreement()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var grantOpening = EntityHelper.CreateGrantOpeningWithGrantApplications();
            var grantApplication = EntityHelper.CreateGrantApplicationWithClaim();
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet<GrantOpening>(grantOpening);

            grantOpening.GrantOpeningIntake.UnderAssessmentCount = 10;
            grantOpening.GrantOpeningIntake.UnderAssessmentAmt = 10;

            grantOpening.GrantOpeningFinancial.CurrentReservations = 10;

            grantOpening.GrantOpeningIntake.ReductionsAmt = 10;

            grantOpening.GrantOpeningFinancial.AssessedCommitmentsCount = 10;
            grantOpening.GrantOpeningFinancial.AssessedCommitments = 10;

            grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount = 10;
            grantOpening.GrantOpeningFinancial.OutstandingCommitments = 10;

            var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();
            grantApplication.Claims.FirstOrDefault().GrantApplication = grantApplication;

            // Act
            service.AdjustFinancialStatements(grantApplication, ApplicationStateInternal.New, ApplicationWorkflowTrigger.AcceptGrantAgreement);

            // Assert
            grantOpening.GrantOpeningIntake.UnderAssessmentCount.Should().Be(9);
            grantOpening.GrantOpeningIntake.UnderAssessmentAmt.Should().Be(8);

            grantOpening.GrantOpeningFinancial.CurrentReservations.Should().Be(8);

            grantOpening.GrantOpeningIntake.ReductionsAmt.Should().Be(11);

            grantOpening.GrantOpeningFinancial.AssessedCommitmentsCount.Should().Be(11);
            grantOpening.GrantOpeningFinancial.AssessedCommitments.Should().Be(11);

            grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount.Should().Be(11);
            grantOpening.GrantOpeningFinancial.OutstandingCommitments.Should().Be(11);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void AdjustFinancialStatements_ApplicationWorkflowTrigger_Case_CancelAgreementHolder()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var grantOpening = EntityHelper.CreateGrantOpeningWithGrantApplications();
            var grantApplication = EntityHelper.CreateGrantApplicationWithClaim();
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet<GrantOpening>(grantOpening);

            grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount = 10;
            grantOpening.GrantOpeningFinancial.OutstandingCommitments = 10;

            grantOpening.GrantOpeningFinancial.CancellationsCount = 10;
            grantOpening.GrantOpeningFinancial.Cancellations = 10;

            var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();
            grantApplication.Claims.FirstOrDefault().GrantApplication = grantApplication;

            // Act
            service.AdjustFinancialStatements(grantApplication, ApplicationStateInternal.New, ApplicationWorkflowTrigger.CancelAgreementHolder);

            // Assert
            grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount.Should().Be(9);
            grantOpening.GrantOpeningFinancial.OutstandingCommitments.Should().Be(9);

            grantOpening.GrantOpeningFinancial.CancellationsCount.Should().Be(11);
            grantOpening.GrantOpeningFinancial.Cancellations.Should().Be(11);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void AdjustFinancialStatements_ApplicationWorkflowTrigger_Case_SubmitClaim_With_No_Prior_Claims_And_ClaimVersion_Equal_To_One()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var grantOpening = EntityHelper.CreateGrantOpeningWithGrantApplications();
            var grantApplication = EntityHelper.CreateGrantApplicationWithClaim();
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet<GrantOpening>(grantOpening);

            grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount = 10;
            grantOpening.GrantOpeningFinancial.OutstandingCommitments = 10;

            grantOpening.GrantOpeningFinancial.CurrentClaimCount = 10;
            grantOpening.GrantOpeningFinancial.CurrentClaims = 10;

            var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();
            grantApplication.Claims.FirstOrDefault().GrantApplication = grantApplication;

            // Act
            service.AdjustFinancialStatements(grantApplication, ApplicationStateInternal.New, ApplicationWorkflowTrigger.SubmitClaim);

            // Assert
            grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount.Should().Be(10);
            grantOpening.GrantOpeningFinancial.OutstandingCommitments.Should().Be(10);

            grantOpening.GrantOpeningFinancial.CurrentClaimCount.Should().Be(10);
            grantOpening.GrantOpeningFinancial.CurrentClaims.Should().Be(10);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void AdjustFinancialStatements_ApplicationWorkflowTrigger_Case_SubmitClaim_With_Prior_Claims_And_ClaimVersion_Greater_Then_One()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var grantOpening = EntityHelper.CreateGrantOpeningWithGrantApplications();
            var grantApplication = EntityHelper.CreateGrantApplicationWithClaim();
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet<GrantOpening>(grantOpening);

            grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount = 10;
            grantOpening.GrantOpeningFinancial.OutstandingCommitments = 10;

            grantOpening.GrantOpeningFinancial.CurrentClaimCount = 10;
            grantOpening.GrantOpeningFinancial.CurrentClaims = 10;

            //Use a different claimversion and add prior claim
            var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();

            grantApplication.Claims.FirstOrDefault().ClaimVersion = 2;
            grantApplication.Claims.FirstOrDefault().GrantApplication = grantApplication;

            grantApplication.Claims.Add(new Claim
            {
                ClaimVersion = 3,
                GrantApplication = grantApplication,
                ClaimState = ClaimState.ClaimDenied
            });
            grantApplication.Claims.Add(new Claim
            {
                ClaimVersion = 1,
                GrantApplication = grantApplication,
                ClaimState = ClaimState.ClaimApproved
            });

            // Act
            service.AdjustFinancialStatements(grantApplication, ApplicationStateInternal.New, ApplicationWorkflowTrigger.SubmitClaim);

            // Assert
            grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount.Should().Be(10);
            grantOpening.GrantOpeningFinancial.OutstandingCommitments.Should().Be(10);

            grantOpening.GrantOpeningFinancial.CurrentClaimCount.Should().Be(10);
            grantOpening.GrantOpeningFinancial.CurrentClaims.Should().Be(10);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void AdjustFinancialStatements_ApplicationWorkflowTrigger_Case_WithdrawClaim_With_No_Prior_Claims_And_ClaimVersion_Equal_To_One()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var grantOpening = EntityHelper.CreateGrantOpeningWithGrantApplications();
            var grantApplication = EntityHelper.CreateGrantApplicationWithClaim();
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet<GrantOpening>(grantOpening);

            grantOpening.GrantOpeningFinancial.CurrentClaimCount = 10;
            grantOpening.GrantOpeningFinancial.CurrentClaims = 10;

            grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount = 10;
            grantOpening.GrantOpeningFinancial.OutstandingCommitments = 10;

            var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();
            grantApplication.Claims.FirstOrDefault().GrantApplication = grantApplication;

            // Act
            service.AdjustFinancialStatements(grantApplication, ApplicationStateInternal.New, ApplicationWorkflowTrigger.WithdrawClaim);

            // Assert
            grantOpening.GrantOpeningFinancial.CurrentClaimCount.Should().Be(10);
            grantOpening.GrantOpeningFinancial.CurrentClaims.Should().Be(10);

            grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount.Should().Be(10);
            grantOpening.GrantOpeningFinancial.OutstandingCommitments.Should().Be(10);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void AdjustFinancialStatements_ApplicationWorkflowTrigger_Case_WithdrawClaim_With_Prior_Claims_And_ClaimVersion_Greater_Then_One()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var grantOpening = EntityHelper.CreateGrantOpeningWithGrantApplications();
            var grantApplication = EntityHelper.CreateGrantApplicationWithClaim();
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet<GrantOpening>(grantOpening);

            grantOpening.GrantOpeningFinancial.CurrentClaimCount = 10;
            grantOpening.GrantOpeningFinancial.CurrentClaims = 10;

            grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount = 10;
            grantOpening.GrantOpeningFinancial.OutstandingCommitments = 10;

            //Use a different claimversion and add prior claim
            var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();

            grantApplication.Claims.FirstOrDefault().ClaimVersion = 2;
            grantApplication.Claims.FirstOrDefault().GrantApplication = grantApplication;

            grantApplication.Claims.Add(new Claim
            {
                ClaimVersion = 3,
                GrantApplication = grantApplication,
                ClaimState = ClaimState.ClaimDenied
            });
            grantApplication.Claims.Add(new Claim
            {
                ClaimVersion = 1,
                GrantApplication = grantApplication,
                ClaimState = ClaimState.ClaimApproved
            });
            // Act
            service.AdjustFinancialStatements(grantApplication, ApplicationStateInternal.New, ApplicationWorkflowTrigger.WithdrawClaim);

            // Assert
            grantOpening.GrantOpeningFinancial.CurrentClaimCount.Should().Be(10);
            grantOpening.GrantOpeningFinancial.CurrentClaims.Should().Be(10);

            grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount.Should().Be(10);
            grantOpening.GrantOpeningFinancial.OutstandingCommitments.Should().Be(10);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void AdjustFinancialStatements_ApplicationWorkflowTrigger_Case_ApproveClaim_With_No_Prior_Claims_And_ClaimVersion_Equal_To_One()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var grantOpening = EntityHelper.CreateGrantOpeningWithGrantApplications();
            var grantApplication = EntityHelper.CreateGrantApplicationWithClaim();
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet<GrantOpening>(grantOpening);

            grantOpening.GrantOpeningFinancial.CurrentClaimCount = 10;
            grantOpening.GrantOpeningFinancial.CurrentClaims = 10;

            grantOpening.GrantOpeningFinancial.ClaimsAssessedCount = 10;
            grantOpening.GrantOpeningFinancial.ClaimsAssessed = 10;

            var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();
            grantApplication.Claims.FirstOrDefault().GrantApplication = grantApplication;

            // Act
            service.AdjustFinancialStatements(grantApplication, ApplicationStateInternal.New, ApplicationWorkflowTrigger.ApproveClaim);

            // Assert
            grantOpening.GrantOpeningFinancial.CurrentClaimCount.Should().Be(10);
            grantOpening.GrantOpeningFinancial.CurrentClaims.Should().Be(10);

            grantOpening.GrantOpeningFinancial.ClaimsAssessedCount.Should().Be(10);
            grantOpening.GrantOpeningFinancial.ClaimsAssessed.Should().Be(10);
        }


        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void AdjustFinancialStatements_ApplicationWorkflowTrigger_Case_ApproveClaim_With_Prior_Claims_And_ClaimVersion_Greater_Then_One()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var grantOpening = EntityHelper.CreateGrantOpeningWithGrantApplications();
            var grantApplication = EntityHelper.CreateGrantApplicationWithClaim();
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet<GrantOpening>(grantOpening);

            grantOpening.GrantOpeningFinancial.ClaimsAssessedCount = 10;
            grantOpening.GrantOpeningFinancial.ClaimsAssessed = 10;

            //Use a different claimversion and add prior claim
            var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();

            grantApplication.Claims.FirstOrDefault().ClaimVersion = 2;
            grantApplication.Claims.FirstOrDefault().GrantApplication = grantApplication;

            grantApplication.Claims.Add(new Claim
            {
                ClaimVersion = 3,
                GrantApplication = grantApplication,
                ClaimState = ClaimState.ClaimDenied
            });
            grantApplication.Claims.Add(new Claim
            {
                ClaimVersion = 1,
                GrantApplication = grantApplication,
                ClaimState = ClaimState.ClaimApproved
            });
            // Act
            service.AdjustFinancialStatements(grantApplication, ApplicationStateInternal.New, ApplicationWorkflowTrigger.ApproveClaim);

            // Assert
            grantOpening.GrantOpeningFinancial.ClaimsAssessedCount.Should().Be(10);
            grantOpening.GrantOpeningFinancial.ClaimsAssessed.Should().Be(10);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void AdjustFinancialStatements_ApplicationWorkflowTrigger_Case_DenyClaim_With_No_Prior_Claims_And_ClaimVersion_Equal_To_One()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var grantOpening = EntityHelper.CreateGrantOpeningWithGrantApplications();
            var grantApplication = EntityHelper.CreateGrantApplicationWithClaim();
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet<GrantOpening>(grantOpening);

            grantOpening.GrantOpeningFinancial.CurrentClaimCount = 10;
            grantOpening.GrantOpeningFinancial.CurrentClaims = 10;

            grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount = 10;
            grantOpening.GrantOpeningFinancial.OutstandingCommitments = 10;

            grantOpening.GrantOpeningFinancial.ClaimsDeniedCount = 10;
            grantOpening.GrantOpeningFinancial.ClaimsDenied = 10;

            var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();
            grantApplication.Claims.FirstOrDefault().GrantApplication = grantApplication;

            // Act
            service.AdjustFinancialStatements(grantApplication, ApplicationStateInternal.New, ApplicationWorkflowTrigger.DenyClaim);

            // Assert
            grantOpening.GrantOpeningFinancial.CurrentClaimCount.Should().Be(10);
            grantOpening.GrantOpeningFinancial.CurrentClaims.Should().Be(10);

            grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount.Should().Be(10);
            grantOpening.GrantOpeningFinancial.OutstandingCommitments.Should().Be(10);

            grantOpening.GrantOpeningFinancial.ClaimsDeniedCount.Should().Be(10);
            grantOpening.GrantOpeningFinancial.ClaimsDenied.Should().Be(10);
        }


        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void AdjustFinancialStatements_ApplicationWorkflowTrigger_Case_DenyClaim_With_Prior_Claims_And_ClaimVersion_Greater_Then_One()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var grantOpening = EntityHelper.CreateGrantOpeningWithGrantApplications();
            var grantApplication = EntityHelper.CreateGrantApplicationWithClaim();
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet<GrantOpening>(grantOpening);

            grantOpening.GrantOpeningFinancial.CurrentClaimCount = 10;
            grantOpening.GrantOpeningFinancial.CurrentClaims = 10;

            grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount = 10;
            grantOpening.GrantOpeningFinancial.OutstandingCommitments = 10;

            grantOpening.GrantOpeningFinancial.ClaimsDeniedCount = 10;
            grantOpening.GrantOpeningFinancial.ClaimsDenied = 10;

            //Use a different claimversion and add prior claim
            var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();

            grantApplication.Claims.FirstOrDefault().ClaimVersion = 2;
            grantApplication.Claims.FirstOrDefault().GrantApplication = grantApplication;

            grantApplication.Claims.Add(new Claim
            {
                ClaimVersion = 3,
                GrantApplication = grantApplication,
                ClaimState = ClaimState.ClaimDenied,
                EligibleCosts = new List<ClaimEligibleCost> { new ClaimEligibleCost {
                    AssessedCost = 10,
                    AssessedParticipants = 10,
                    AssessedMaxParticipantCost = 10,
                    AssessedMaxParticipantReimbursementCost = 10,
                    AssessedParticipantEmployerContribution = 10

        } }
            });
            grantApplication.Claims.Add(new Claim
            {
                ClaimVersion = 1,
                GrantApplication = grantApplication,
                ClaimState = ClaimState.ClaimApproved
            });
            // Act
            service.AdjustFinancialStatements(grantApplication, ApplicationStateInternal.New, ApplicationWorkflowTrigger.DenyClaim);

            // Assert
            var eligibleCost = grantApplication.Claims.FirstOrDefault().EligibleCosts.FirstOrDefault();
            eligibleCost.AssessedCost.Should().Be(0);
            eligibleCost.AssessedParticipants.Should().Be(0);
            eligibleCost.AssessedMaxParticipantCost.Should().Be(0);
            eligibleCost.AssessedMaxParticipantReimbursementCost.Should().Be(0);
            eligibleCost.AssessedParticipantEmployerContribution.Should().Be(0);

            grantOpening.GrantOpeningFinancial.CurrentClaimCount.Should().Be(10);
            grantOpening.GrantOpeningFinancial.CurrentClaims.Should().Be(10);

            grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount.Should().Be(10);
            grantOpening.GrantOpeningFinancial.OutstandingCommitments.Should().Be(10);

            grantOpening.GrantOpeningFinancial.ClaimsDeniedCount.Should().Be(10);
            grantOpening.GrantOpeningFinancial.ClaimsDenied.Should().Be(10);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void AdjustFinancialStatements_ApplicationWorkflowTrigger_Case_ReturnClaimToApplicant_With_No_Prior_Claims_And_ClaimVersion_Equal_To_One()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var grantOpening = EntityHelper.CreateGrantOpeningWithGrantApplications();
            var grantApplication = EntityHelper.CreateGrantApplicationWithClaim();
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet<GrantOpening>(grantOpening);

            grantOpening.GrantOpeningFinancial.CurrentClaimCount = 10;
            grantOpening.GrantOpeningFinancial.CurrentClaims = 10;

            grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount = 10;
            grantOpening.GrantOpeningFinancial.OutstandingCommitments = 10;

            var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();
            grantApplication.Claims.FirstOrDefault().GrantApplication = grantApplication;

            // Act
            service.AdjustFinancialStatements(grantApplication, ApplicationStateInternal.New, ApplicationWorkflowTrigger.ReturnClaimToApplicant);

            // Assert
            grantOpening.GrantOpeningFinancial.CurrentClaimCount.Should().Be(10);
            grantOpening.GrantOpeningFinancial.CurrentClaims.Should().Be(10);

            grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount.Should().Be(10);
            grantOpening.GrantOpeningFinancial.OutstandingCommitments.Should().Be(10);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void AdjustFinancialStatements_ApplicationWorkflowTrigger_Case_ReturnClaimToApplicant_With_Prior_Claims_And_ClaimVersion_Greater_Then_One()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var grantOpening = EntityHelper.CreateGrantOpeningWithGrantApplications();
            var grantApplication = EntityHelper.CreateGrantApplicationWithClaim();
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet<GrantOpening>(grantOpening);

            grantOpening.GrantOpeningFinancial.CurrentClaimCount = 10;
            grantOpening.GrantOpeningFinancial.CurrentClaims = 10;

            grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount = 10;
            grantOpening.GrantOpeningFinancial.OutstandingCommitments = 10;

            //Use a different claimversion and add prior claim
            var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();

            grantApplication.Claims.FirstOrDefault().ClaimVersion = 2;
            grantApplication.Claims.FirstOrDefault().GrantApplication = grantApplication;

            grantApplication.Claims.Add(new Claim
            {
                ClaimVersion = 3,
                GrantApplication = grantApplication,
                ClaimState = ClaimState.ClaimDenied
            });
            grantApplication.Claims.Add(new Claim
            {
                ClaimVersion = 1,
                GrantApplication = grantApplication,
                ClaimState = ClaimState.ClaimApproved
            });

            // Act
            service.AdjustFinancialStatements(grantApplication, ApplicationStateInternal.New, ApplicationWorkflowTrigger.ReturnClaimToApplicant);

            // Assert
            grantOpening.GrantOpeningFinancial.CurrentClaimCount.Should().Be(10);
            grantOpening.GrantOpeningFinancial.CurrentClaims.Should().Be(10);

            grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount.Should().Be(10);
            grantOpening.GrantOpeningFinancial.OutstandingCommitments.Should().Be(10);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Service")]
        public void AdjustFinancialStatements_ApplicationWorkflowTrigger_Case_SubmitClaim_With_Prior_Claims_And_ClaimVersion_Greater_Then_One_Temp()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
            var grantOpening = EntityHelper.CreateGrantOpeningWithGrantApplications();
            var grantApplication = EntityHelper.CreateGrantApplicationWithClaim();
            var service = helper.Create<GrantOpeningService>();

            helper.MockDbSet<GrantOpening>(grantOpening);

            grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount = 10;
            grantOpening.GrantOpeningFinancial.OutstandingCommitments = 10;

            grantOpening.GrantOpeningFinancial.CurrentClaimCount = 10;
            grantOpening.GrantOpeningFinancial.CurrentClaims = 10;

            //Use a different claimversion and add prior claim
            var trainingProgram = grantApplication.TrainingPrograms.FirstOrDefault();

            grantApplication.Claims.FirstOrDefault().ClaimVersion = 2;
            grantApplication.Claims.FirstOrDefault().GrantApplication = grantApplication;

            grantApplication.Claims.Add ( new Claim {
                ClaimVersion = 3,
                GrantApplication = grantApplication,
                ClaimState = ClaimState.ClaimDenied
            });
            grantApplication.Claims.Add(new Claim
            {
                ClaimVersion = 1,
                GrantApplication = grantApplication,
                ClaimState = ClaimState.ClaimApproved
            });

            // Act
            service.AdjustFinancialStatements(grantApplication, ApplicationStateInternal.New, ApplicationWorkflowTrigger.SubmitClaim);

            // Assert
            grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount.Should().Be(10);
            grantOpening.GrantOpeningFinancial.OutstandingCommitments.Should().Be(10);

            grantOpening.GrantOpeningFinancial.CurrentClaimCount.Should().Be(10);
            grantOpening.GrantOpeningFinancial.CurrentClaims.Should().Be(10);
        }
	}
}
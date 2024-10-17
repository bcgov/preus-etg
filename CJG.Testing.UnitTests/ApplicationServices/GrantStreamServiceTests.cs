using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Infrastructure.Entities;
using CJG.Testing.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	/// <summary>
	///     Unit tests for GrantStreamService
	/// </summary>
	[TestClass]
	public class GrantStreamServiceTests : ServiceUnitTestBase
	{
		[TestInitialize]
		public void Setup()
		{
		}

		[TestMethod, TestCategory("Grant Stream"), TestCategory("Service")]
		public void AddGrantStream_WithGrantStream_AddsNewGrantStream()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(GrantStreamService), identity);
			var dbSetMock = helper.MockDbSet<GrantStream>();
			var service = helper.Create<GrantStreamService>();
			var grantStream = new GrantStream()
			{
				Id = 1
			};

			// Act
			service.Add(grantStream);

			// Assert
			dbSetMock.Verify(x => x.Add(It.IsAny<GrantStream>()), Times.Exactly(1));
			var result = service.Get(grantStream.Id);
			result.Id.Should().Be(1);
		}

		[TestMethod, TestCategory("Grant Stream"), TestCategory("Service")]
		public void SaveGrantStream_WithGrantStream_AddsNewGrantStream()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(GrantStreamService), identity);
			helper.MockDbSet<GrantStream>();
			var service = helper.Create<GrantStreamService>();

			// Act
			service.Update(new GrantStream());

			// Assert
			helper.GetMock<IDataContext>().Verify(x => x.Update(It.IsAny<GrantStream>()), Times.Exactly(1));
			helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Once);
		}

		[TestMethod, TestCategory("Grant Stream"), TestCategory("Service")]
		public void SaveGrantStream_WithGrantStream_UpdateGrantStream_HasParticipantOutcomeReporting()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(GrantStreamService), identity);
			helper.MockDbSet<GrantStream>();
			var service = helper.Create<GrantStreamService>();

			var grantStream = new GrantStream()
			{
				Id = 1
			};
			service.Add(grantStream);
			grantStream.HasParticipantOutcomeReporting = true;
			service.Update(grantStream);

			// Assert
			helper.GetMock<IDataContext>().Verify(x => x.Update(It.IsAny<GrantStream>()), Times.Exactly(1));
			helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Exactly(2));
			var result = service.Get(grantStream.Id);
			result.HasParticipantOutcomeReporting.Should().Be(true);
		}

		[TestMethod, TestCategory("Grant Stream"), TestCategory("Service")]
		public void DeleteGrantStream_WithGrantStream_DeletesGrantStreamAndDependencies()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(GrantStreamService), identity);
			var service = helper.Create<GrantStreamService>();
			var grantProgram = new GrantProgram() { Id = 3 };
			var payment = new AccountCode() { Id = 2 };
			var grantStream = new GrantStream("name", "criteria", grantProgram, payment) {Id = 4};

			var dbSetMock = helper.MockDbSet(grantStream);
			helper.MockDbSet<AccountCode>();

			// Act
			service.Delete(grantStream);

			// Assert
			dbSetMock.Verify(x => x.Remove(It.Is<GrantStream>(gs => gs.Id == grantStream.Id)));
		}

		[TestMethod, TestCategory("Grant Stream"), TestCategory("Service")]
		public void GetAllGrantStreams()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var identity = HttpHelper.CreateIdentity(user, "Assessor");
			var helper = new ServiceHelper(typeof(GrantStreamService), identity);

			var grantProgram = new GrantProgram
			{
				Id = 3,
				ProgramCode = "ETG"
			};

			helper.MockDbSet(grantProgram);

			helper.MockDbSet( new[] {
				new GrantStream
				{
					Id = 1,
					GrantProgram = grantProgram
				},
				new GrantStream
				{
					Id = 2,
					GrantProgram = grantProgram
				},
				new GrantStream
				{
					Id = 3,
					GrantProgram = grantProgram
				}
			});
			var service = helper.Create<GrantStreamService>();

			// Act
			var result = service.GetAll();

			// Assert
			result.Should().HaveCount(3);
		}

		[TestMethod, TestCategory("Grant Stream"), TestCategory("Service")]
		public void GetGrantStream_WithGrantStreamId_ReturnsStreamWithMatchedStreamId()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(GrantStreamService), identity);
			helper.MockDbSet( new[] {
				new GrantStream
				{
					Id = 1,
					Name = "Name 1"
				}
			});
			var service = helper.Create<GrantStreamService>();

			// Act
			var grantStream = service.Get(1);

			// Assert
			grantStream.Name.Should().Be("Name 1");
		}

		[TestMethod, TestCategory("Grant Stream"), TestCategory("Service")]
		public void GetGrantStream_WithGrantStreamName_ReturnsStreamWithMatchedStreamName()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(GrantStreamService), identity);
			var dbSetMock = helper.MockDbSet( new[] {
				new GrantStream
				{
					Name = "stream 1"
				},
				new GrantStream
				{
					 Name = "stream 2"
				},
				new GrantStream
				{
					 Name = "stream 3"
				}
			});
			var service = helper.Create<GrantStreamService>();
			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(x => x.GrantStreams.AsNoTracking()).Returns(dbSetMock.Object);

			// Act
			var result = service.Get("stream 3");

			// Assert
			service.Get("stream 1").Should().NotBeNull();
			service.Get("stream 2").Should().NotBeNull();
			service.Get("stream 3").Should().NotBeNull();
		}

		[TestMethod, TestCategory("Grant Stream"), TestCategory("Service")]
		public void GetReportRate_WithGrantStreamId_ReturnsReportRateWithMatchedStreamId()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(GrantStreamService), identity);
			var dbSetMock = helper.MockDbSet( new[] {
				new ReportRate
				{
					GrantStreamId = 1,
				},
				new ReportRate
				{
					GrantStreamId = 2,
				},
				new ReportRate
				{
					GrantStreamId = 1,
				},
				new ReportRate
				{
					GrantStreamId = 3,
				}

			});
			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(x => x.ReportRates.AsNoTracking()).Returns(dbSetMock.Object);

			// Act
			var service = helper.Create<GrantStreamService>();

			// Assert
			service.GetReportRates(1).Should().HaveCount(2);
			service.GetReportRates(2).Should().HaveCount(1);
		}
	}
}
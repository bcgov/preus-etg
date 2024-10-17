using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using CJG.Core.Entities;
using FluentAssertions;
using CJG.Application.Services;
using CJG.Testing.Core;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using System.Collections.Generic;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
	public class NoteServiceTest : ServiceUnitTestBase
	{
		[TestInitialize]
		public void Setup()
		{
		}

		[TestMethod, TestCategory("Note"), TestCategory("Service")]
		public void AddSystemNoteWithAttachment_PassInParameters_ShouldAddNoteToDataStore()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ServiceHelper(typeof(NoteService), user, "Assessor");

			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(user, ApplicationStateInternal.AgreementAccepted);
			string noteContent = "This is a test note";
			Attachment attachment = null;

			helper.MockDbSet(new[] { user });
			helper.MockDbSet(new[] { grantApplication });
			var noteDbSetMock = helper.MockDbSet<Note>(new List<Note>() { new Note() } );
			var dataContextMock = helper.GetMock<IDataContext>();
			var staticServiceMock = helper.GetMock<IStaticDataService>().Setup(m => m.GetNoteType(It.IsAny<NoteTypes>())).Returns(new NoteType(NoteTypes.ED, "description", true));
			var service = helper.Create<NoteService>();
			// Act
			service.AddSystemNote(grantApplication, noteContent, attachment);

			// Assert
			noteDbSetMock.Verify(x => x.Add(It.IsAny<Note>()), Times.Exactly(1));
			dataContextMock.Verify(x => x.Commit(), Times.Never);
			dataContextMock.Verify(x => x.CommitTransaction(), Times.Never);
		}

		[TestMethod, TestCategory("Note"), TestCategory("Service")]
		public void AddValueChangeNote_PassInParameters_ShouldAddNoteToDataStore()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ServiceHelper(typeof(NoteService), user, "Assessor");

			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(user, ApplicationStateInternal.AgreementAccepted);
			string itemName = "This is a test note";
			string oldValue = "This is old value";
			string newValue = "This is new value";

			helper.MockDbSet(new[] { grantApplication });
			helper.MockDbSet(new[] { user });
			var noteDbSetMock = helper.MockDbSet<Note>(new List<Note>() { new Note() });
			var staticServiceMock = helper.GetMock<IStaticDataService>().Setup(m => m.GetNoteType(It.IsAny<NoteTypes>())).Returns(new NoteType(NoteTypes.ED, "description", true));
			var dataContextMock = helper.GetMock<IDataContext>();
			var service = helper.Create<NoteService>();

			// Act
			service.AddValueChangedNote(grantApplication, itemName, oldValue, newValue);

			// Assert
			noteDbSetMock.Verify(x => x.Add(It.IsAny<Note>()), Times.Exactly(1));
			dataContextMock.Verify(x => x.Commit(), Times.Never);
			dataContextMock.Verify(x => x.CommitTransaction(), Times.Never);
		}

		[TestMethod, TestCategory("Note"), TestCategory("Service")]
		public void AddNote_PassInNoteAsParameter_ShouldAddNoteToDataStore()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ServiceHelper(typeof(NoteService), user, "Assessor");

			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(user, ApplicationStateInternal.AgreementAccepted);
			var note = new Note()
			{
				Id = 1,
				Content = "note content",
				GrantApplicationId = grantApplication.Id,
				GrantApplication = grantApplication
			};

			helper.MockDbSet(new[] { grantApplication });
			helper.MockDbSet<Attachment>();
			helper.MockDbSet<InternalUser>();
			var noteDbSetMock = helper.MockDbSet<Note>();
			var dataContextMock = helper.GetMock<IDataContext>();
			var service = helper.Create<NoteService>();

			// Act
			service.Add(note);

			// Assert
			noteDbSetMock.Verify(x => x.Add(It.IsAny<Note>()), Times.Exactly(1));
			dataContextMock.Verify(x => x.Commit(), Times.Never);
			dataContextMock.Verify(x => x.CommitTransaction(), Times.Once);
		}

		[TestMethod, TestCategory("Note"), TestCategory("Service")]
		public void UpdateNote_PassInNoteAsParameter_ShouldUpdateNoteInDataStore()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ServiceHelper(typeof(NoteService), user, "Assessor");

			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(user, ApplicationStateInternal.AgreementAccepted);
			var note = new Note()
			{
				Id = 1,
				Content = "note content",
				GrantApplicationId = grantApplication.Id,
				GrantApplication = grantApplication,
				NoteTypeId = NoteTypes.AS,
				CreatorId = 1
			};

			helper.MockDbSet(grantApplication);
			var grantApplicationService = helper.MockDbSet<GrantApplication>();
			var noteDbSetMock = helper.MockDbSet<Note>(note);
			var dbContextMock = helper.GetMock<IDataContext>();
			var service = helper.Create<NoteService>();

			// Act
			service.Update(note);

			// Assert
			noteDbSetMock.Verify(x => x.Add(It.IsAny<Note>()), Times.Exactly(1));
			dbContextMock.Verify(x => x.Commit(), Times.Never);
			dbContextMock.Verify(x => x.CommitTransaction(), Times.Once);
		}

		[TestMethod, TestCategory("Note"), TestCategory("Service")]
		public void Remove_PassInNoteAsParameter_ShouldUpdateNoteInDataStore()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ServiceHelper(typeof(NoteService), user, "Assessor");

			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(user, ApplicationStateInternal.AgreementAccepted);
			var note = new Note()
			{
				Id = 1,
				CreatorId = user.Id,
				NoteTypeId = NoteTypes.AS,
				Content = "note content"
			};

			helper.MockDbSet(new[] { grantApplication });
			var noteDbSetMock = helper.MockDbSet<Note>();
			var dataContextMock = helper.GetMock<IDataContext>();
			var service = helper.Create<NoteService>();

			// Act
			service.Remove(note);

			// Assert
			noteDbSetMock.Verify(x => x.Remove(It.IsAny<Note>()), Times.Exactly(1));
			dataContextMock.Verify(x => x.Commit(), Times.Never);
			dataContextMock.Verify(x => x.CommitTransaction(), Times.Once);
		}

		[TestMethod, TestCategory("Note"), TestCategory("Service")]
		public void CreateNote_PassIn5Parameters_ShouldReturnANote()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ServiceHelper(typeof(NoteService), user, "Assessor");

			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(user, ApplicationStateInternal.AgreementAccepted);
			string itemName = "This is a test note";
			string oldValue = "This is old value";
			string newValue = "This is new value";

			helper.MockDbSet(new[] { user });
			helper.MockDbSet(new[] { grantApplication });
			var staticServiceMock = helper.GetMock<IStaticDataService>().Setup(m => m.GetNoteType(It.IsAny<NoteTypes>())).Returns(new NoteType(NoteTypes.ED, "description", true));
			var dataContextMock = helper.GetMock<IDataContext>();
			var service = helper.Create<NoteService>();

			// Act
			var result = service.CreateNote(grantApplication, itemName, oldValue, newValue);

			// Assert
			result.GrantApplicationId.Should().Be(1);
			result.Creator.LastName.Should().Be(user.LastName);
			result.Creator.FirstName.Should().Be(user.FirstName);
			result.Content.Should().Contain(itemName);
			result.Content.Should().Contain(oldValue);
			result.Content.Should().Contain(newValue);
		}

		[TestMethod, TestCategory("Note"), TestCategory("Service")]
		public void CreateNote_PassIn3Parameters_ShouldReturnASystemNote()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ServiceHelper(typeof(NoteService), user, "Assessor");

			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(user, ApplicationStateInternal.AgreementAccepted);
			string noteContent = "This is a test note";

			helper.MockDbSet(new[] { grantApplication });
			helper.MockDbSet(new[] { user });
			var staticServiceMock = helper.GetMock<IStaticDataService>().Setup(m => m.GetNoteType(It.IsAny<NoteTypes>())).Returns(new NoteType(NoteTypes.ED, "description", true));
			var dataContextMock = helper.GetMock<IDataContext>();
			var service = helper.Create<NoteService>();

			// Act
			var result = service.CreateNote(grantApplication, NoteTypes.ED, noteContent);

			// Assert
			result.GrantApplicationId.Should().Be(1);
			result.Creator.LastName.Should().Be(user.LastName);
			result.Creator.FirstName.Should().Be(user.FirstName);
			result.Content.Should().Be(noteContent);
		}

		[TestMethod, TestCategory("Note"), TestCategory("Service")]
		public void CreateWorkflowNote_PassIn3Parameters_ShouldReturnAWorkFlowNoteNote()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ServiceHelper(typeof(NoteService), user, "Assessor");

			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(user, ApplicationStateInternal.AgreementAccepted);
			string noteContent = "This is a test note";

			helper.MockDbSet(new[] { grantApplication });
			helper.MockDbSet(new[] { user });
			var staticServiceMock = helper.GetMock<IStaticDataService>().Setup(m => m.GetNoteType(It.IsAny<NoteTypes>())).Returns(new NoteType(NoteTypes.ED, "description", true));
			var dataContextMock = helper.GetMock<IDataContext>();
			var service = helper.Create<NoteService>();

			// Act
			var result = service.CreateWorkflowNote(grantApplication, noteContent);

			// Assert
			result.GrantApplicationId.Should().Be(1);
			result.Creator.LastName.Should().Be(user.LastName);
			result.Creator.FirstName.Should().Be(user.FirstName);
			result.Content.Should().Be(noteContent);
		}

		[TestMethod, TestCategory("Note"), TestCategory("Service")]
		public void CreateNote_PassIn4Parameters_ShouldReturnANote()
		{
			// Arrange
			var user = EntityHelper.CreateInternalUser();
			var helper = new ServiceHelper(typeof(NoteService), user, "Assessor");

			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(user, ApplicationStateInternal.AgreementAccepted);
			string message = "This is new value";

			helper.MockDbSet(new[] { grantApplication });
			helper.MockDbSet(new[] { user });
			var staticServiceMock = helper.GetMock<IStaticDataService>().Setup(m => m.GetNoteType(It.IsAny<NoteTypes>())).Returns(new NoteType(NoteTypes.ED, "description", true));
			var dataContextMock = helper.GetMock<IDataContext>();
			var service = helper.Create<NoteService>();

			// Act
			var result = service.CreateNote(grantApplication, NoteTypes.AS, message);

			// Assert
			result.GrantApplicationId.Should().Be(1);
			result.Creator.LastName.Should().Be(user.LastName);
			result.Creator.FirstName.Should().Be(user.FirstName);
			result.Content.Should().Be(message);
		}
	}
}

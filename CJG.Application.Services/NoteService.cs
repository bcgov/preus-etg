using System;
using System.Text;
using System.Web;
using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using CJG.Core.Interfaces;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;

namespace CJG.Application.Services
{
	public class NoteService : Service, INoteService
	{
		private readonly IStaticDataService _staticDataService;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="staticDataService"></param>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public NoteService(IStaticDataService staticDataService, IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
			_staticDataService = staticDataService;
		}

		/// <summary>
		/// Get the note for the specified Id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Note Get(int id)
		{
			var note = Get<Note>(id);
			Get<GrantApplication>(note.GrantApplicationId);

			return note;
		}

		/// <summary>
		/// Add the note to the datasource.
		/// </summary>
		/// <param name="note"></param>
		/// <returns></returns>
		public Note Add(Note note)
		{
			_dbContext.Notes.Add(note);
			_dbContext.CommitTransaction();
			return note;
		}

		/// <summary>
		/// Update the note in the datasource.
		/// </summary>
		/// <param name="note"></param>
		/// <returns></returns>
		public Note Update(Note note)
		{
			if (note.CreatorId != _httpContext.User.GetUserId() || !note.NoteTypeId.In(NoteTypes.AS, NoteTypes.PD, NoteTypes.PM, NoteTypes.QA, NoteTypes.NR))
			{
				throw new NotAuthorizedException($"User does not have permission to update note: '{note.Id}'.");
			}
			_dbContext.Update(note);
			_dbContext.CommitTransaction();

			return note;
		}

		/// <summary>
		/// Delete the note from the datasource.
		/// </summary>
		/// <param name="note"></param>
		public void Remove(Note note)
		{
			if (note.CreatorId != _httpContext.User.GetUserId() || !note.NoteTypeId.In(NoteTypes.AS, NoteTypes.PD, NoteTypes.PM, NoteTypes.QA, NoteTypes.NR))
			{
				throw new NotAuthorizedException($"User does not have permission to update note: '{note.Id}'.");
			}

			// Delete attachments.
			if (note.AttachmentId != null)
			{
				var attachment = Get<Attachment>(note.AttachmentId);
				attachment.Versions.ForEach(a => _dbContext.VersionedAttachments.Remove(a));
				_dbContext.Attachments.Remove(attachment);
			}

			_dbContext.Notes.Remove(note);
			_dbContext.CommitTransaction();
		}

		#region Pre-generated notes
		/// <summary>
		/// Add a note to the specified grant application.
		/// This does not update the datasource.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="type"></param>
		/// <param name="message"></param>
		/// <param name="attachment"></param>
		/// <returns></returns>
		public Note AddNote(GrantApplication grantApplication, NoteTypes type, string message, Attachment attachment = null)
		{
			var note = CreateNote(grantApplication, type, message, attachment);
			grantApplication.Notes.Add(note);
			_dbContext.Notes.Add(note);
			return note;
		}

		/// <summary>
		/// Add a note the specified grant application which reflects a workflow change event.
		/// This does not update the datasource.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="message"></param>
		/// <param name="attachment"></param>
		/// <returns></returns>
		public Note AddWorkflowNote(GrantApplication grantApplication, string message, Attachment attachment = null)
		{
			var note = CreateWorkflowNote(grantApplication, message);
			_dbContext.Notes.Add(note);
			return note;
		}

		/// <summary>
		/// Add a note the specified grant application which reflects a system change event.
		/// This does not update the datasource.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="message"></param>
		/// <param name="attachment"></param>
		/// <returns></returns>
		public Note AddSystemNote(GrantApplication grantApplication, string message, Attachment attachment = null)
		{
			var note = CreateSystemNote(grantApplication, message, attachment);
			_dbContext.Notes.Add(note);
			return note;
		}

		/// <summary>
		/// Add a note to the specified grant application which reflects a system change event with old and new values.
		/// This does not update the datasource.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="itemName"></param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		/// <returns></returns>
		public Note AddValueChangedNote(GrantApplication grantApplication, string itemName, string oldValue, string newValue)
		{
			var note = CreateNote(grantApplication, itemName, oldValue, newValue);
			grantApplication.Notes.Add(note);
			_dbContext.Notes.Add(note);
			return note;
		}

		/// <summary>
		/// Create a note containing all of the current changes being made in the data context.
		/// This does not update the datasource.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public Note AddValueChangedNote(GrantApplication grantApplication)
		{
			var changes = new EntityChanges(_dbContext.Context);
			var message = new StringBuilder();
			foreach (var change in changes)
			{
				message.AppendLine(change.ToString());
			}
			var note = CreateSystemNote(grantApplication, message.ToString());
			grantApplication.Notes.Add(note);
			return note;
		}

		/// <summary>
		/// Add a note to the specified grant application which reflects a date change event.
		/// This does not update the datasource.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="itemName"></param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		/// <returns></returns>
		public Note AddDateChangedNote(GrantApplication grantApplication, string itemName, DateTime oldValue, DateTime newValue)
		{
			var note = CreateNote(grantApplication, NoteTypes.TD, $"{itemName} from \'{oldValue.ToLocalTime().ToString("yyyy-MM-dd")}\' to \'{newValue.ToLocalTime().ToString("yyyy-MM-dd")}\'.");
			grantApplication.Notes.Add(note);
			_dbContext.Notes.Add(note);
			return note;
		}

		/// <summary>
		/// Generate a system note that contains all of the changes made.
		/// This does not update the datasource.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <returns></returns>
		public Note GenerateUpdateNote(GrantApplication grantApplication)
		{
			if (_httpContext.User.GetAccountType() == AccountTypes.External)
				return null;

			var tracker = new EntityChanges(_dbContext.Context);
			if (!tracker.IsChanged)
				return null;

			return AddSystemNote(grantApplication, tracker.ToString());
		}

		/// <summary>
		/// Generate a system note that contains all of the changes made.
		/// This does not update the datasource.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="allowExternal"></param>
		/// <returns></returns>
		public Note GenerateUpdateNote(GrantApplication grantApplication, bool allowExternal)
		{
			if (!allowExternal && _httpContext.User.GetAccountType() == AccountTypes.External)
				return null;

			var tracker = new EntityChanges(_dbContext.Context);
			if (tracker.IsChanged)
			{
				var note = AddSystemNote(grantApplication, tracker.ToString());
				return note;
			}

			return null;
		}
		#endregion

		#region Create notes
		/// <summary>
		/// Create a Note object and initialize it.
		/// This does not add it to the datasource.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="type"></param>
		/// <param name="message"></param>
		/// <param name="attachment"></param>
		/// <returns></returns>
		public Note CreateNote(GrantApplication grantApplication, NoteTypes type, string message, Attachment attachment = null)
		{
			var isInternal = _httpContext.User.GetAccountType() == AccountTypes.Internal;
			var user = isInternal ? Get<InternalUser>(_httpContext.User.GetUserId()) : null;
			var noteType = _staticDataService.GetNoteType(type);
			return new Note(grantApplication, noteType, message, attachment)
			{
				Creator = user
			};
		}

		/// <summary>
		/// Create a Note object and initialize it for a note type of ED (File Change).
		/// Create a standard message to capture a value change.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="itemName"></param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		/// <returns></returns>
		public Note CreateNote(GrantApplication grantApplication, string itemName, string oldValue, string newValue)
		{
			return CreateNote(grantApplication, NoteTypes.ED, $"{itemName} from \'{oldValue}\' to \'{newValue}\'.");
		}

		/// <summary>
		/// Create a Note object and initialize it for a note type of ED (File Change).
		/// A system note is generated by the system and is not linked to a user's account.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="message"></param>
		/// <param name="attachment"></param>
		/// <returns></returns>
		public Note CreateSystemNote(GrantApplication grantApplication, string message, Attachment attachment = null)
		{
			var isInternal = _httpContext.User.GetAccountType() == AccountTypes.Internal;
			var user = isInternal ? Get<InternalUser>(_httpContext.User.GetUserId()) : null;
			var noteType = _staticDataService.GetNoteType(NoteTypes.ED);

			return new Note(grantApplication, noteType, message, attachment)
			{
				Creator = user
			}; ;
		}

		/// <summary>
		/// Create a Note object and initialize it for a note type of WF (Workflow Change).
		/// This note is used when state changes in the application.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		public Note CreateWorkflowNote(GrantApplication grantApplication, string message)
		{
			return CreateNote(grantApplication, NoteTypes.WF, message);
		}
		#endregion
	}
}

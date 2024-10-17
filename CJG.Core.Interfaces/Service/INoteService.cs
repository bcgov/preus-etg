using CJG.Core.Entities;
using System;

namespace CJG.Core.Interfaces.Service
{
	public interface INoteService
	{
		Note Get(int id);

		Note Add(Note note);

		Note Update(Note note);

		void Remove(Note note);

		Note AddNote(GrantApplication grantApplication, NoteTypes type, string message, Attachment attachment = null);

		Note AddWorkflowNote(GrantApplication grantApplication, string message, Attachment attachment = null);

		Note AddSystemNote(GrantApplication grantApplication, string message, Attachment noteAttachment = null);

		Note AddValueChangedNote(GrantApplication grantApplication, string itemName, string oldValue, string newValue);

		Note AddDateChangedNote(GrantApplication grantApplication, string itemName, DateTime oldValue, DateTime newValue);

		Note AddValueChangedNote(GrantApplication grantApplication);

		Note GenerateUpdateNote(GrantApplication grantApplication);

		Note GenerateUpdateNote(GrantApplication grantApplication, bool allowExternal);

		#region Create Notes
		Note CreateNote(GrantApplication grantApplication, NoteTypes noteType, string message, Attachment attachment = null);

		Note CreateNote(GrantApplication grantApplication, string itemName, string oldValue, string newValue);

		Note CreateSystemNote(GrantApplication grantApplication, string message, Attachment attachment = null);

		Note CreateWorkflowNote(GrantApplication grantApplication, string message);
		#endregion
	}
}
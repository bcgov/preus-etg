using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Web.Mvc;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.Notes
{
	public class NoteViewModel : BaseViewModel
	{
		public string RowVersion { get; set; }
		public int GrantApplicationId { get; set; }

		[Required(ErrorMessage = "Note type is required")]
		public NoteTypes? NoteTypeId { get; set; }

		public string NoteTypeCaption { get; set; }
		public string NoteTypeDescription { get; set; }

		public int CreatorId { get; set; }
		public string CreatorName { get; set; }
		public DateTime DateAdded { get; set; }

		[AllowHtml]
		[Required(ErrorMessage = "Message is required.")]
		public string Content { get; set; }

		public string Caption { get; set; }

		public int? AttachmentId { get; set; }
		public string AttachmentDescription { get; set; }
		public string AttachmentFileName { get; set; }

		public bool IsCreator { get; set; }
		public bool AllowEdit { get; set; }
		public bool ShowNote { get; set; }  // Used in Front End

		public NoteViewModel()
		{
		}

		public NoteViewModel(Note note, IPrincipal user, bool includeAttachment = true, bool replaceNewLines = false)
		{
			if (note == null)
				throw new ArgumentNullException(nameof(note));

			GrantApplicationId = note?.GrantApplicationId ?? throw new ArgumentNullException(nameof(note));
			Id = note.Id;
			CreatorId = note.CreatorId ?? 0;
			CreatorName = note.CreatorId == 0 ? "Applicant" : note.Creator != null ? $"{note.Creator.FirstName} {note.Creator.LastName}" : null;
			Content = replaceNewLines ? note.Content.Replace(Environment.NewLine, "<br />") : note.Content;
			Caption = note.GetCaption();
			AttachmentId = note.AttachmentId;
			if (includeAttachment)
			{
				AttachmentFileName = note.Attachment?.FileName;
				AttachmentDescription = note.Attachment?.Description;
			}
			DateAdded = note.DateAdded.ToLocalTime();
			NoteTypeId = note.NoteTypeId;
			NoteTypeCaption = note.NoteType.Caption;
			NoteTypeDescription = note.NoteType.Description;
			RowVersion = Convert.ToBase64String(note.RowVersion);
			IsCreator = user.GetUserId() == CreatorId;
			AllowEdit = IsCreator && (NoteTypeId?.In(NoteTypes.AS, NoteTypes.PD, NoteTypes.PM, NoteTypes.QA, NoteTypes.NR) ?? false);
		}
	}
}

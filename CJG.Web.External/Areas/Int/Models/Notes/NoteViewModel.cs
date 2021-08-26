using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Web;

namespace CJG.Web.External.Areas.Int.Models.Notes
{
	public class NoteViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }
		public int GrantApplicationId { get; set; }

		[Required(ErrorMessage = "Note type is required")]
		public NoteTypes? NoteTypeId { get; set; }

		public string NoteTypeCaption { get; set; }
		public string NoteTypeDescription { get; set; }

		public int CreatorId { get; set; }
		public string CreatorName { get; set; }
		public DateTime DateAdded { get; set; }

		[Required(ErrorMessage = "Message is required.")]
		public string Content { get; set; }

		public string Caption { get; set; }

		public int? AttachmentId { get; set; }
		public string AttachmentDescription { get; set; }
		public string AttachmentFileName { get; set; }

		public bool IsCreator { get; set; }
		public bool AllowEdit { get; set; }

		#endregion

		#region Constructor
		public NoteViewModel()
		{
		}

		public NoteViewModel(Note note, IPrincipal user, bool includeAttachment = true)
		{
			if (note == null) throw new ArgumentNullException(nameof(note));

			this.GrantApplicationId = note?.GrantApplicationId ?? throw new ArgumentNullException(nameof(note));
			this.Id = note.Id;
			this.CreatorId = note.CreatorId ?? 0;
			this.CreatorName = note.CreatorId == 0 ? "Applicant" : note.Creator != null ? $"{note.Creator.FirstName} {note.Creator.LastName}" : null;
			this.Content = note.Content;
			this.Caption = note.GetCaption();
			this.AttachmentId = note.AttachmentId;
			if (includeAttachment)
			{
				this.AttachmentFileName = note.Attachment?.FileName;
				this.AttachmentDescription = note.Attachment?.Description;
			}
			this.DateAdded = note.DateAdded.ToLocalTime();
			this.NoteTypeId = note.NoteTypeId;
			this.NoteTypeCaption = note.NoteType.Caption;
			this.NoteTypeDescription = note.NoteType.Description;
			this.RowVersion = Convert.ToBase64String(note.RowVersion);
			this.IsCreator = user.GetUserId() == this.CreatorId;
			this.AllowEdit = this.IsCreator && (this.NoteTypeId?.In(NoteTypes.AS, NoteTypes.PD, NoteTypes.PM, NoteTypes.QA, NoteTypes.NR) ?? false);
		}
		#endregion
	}
}

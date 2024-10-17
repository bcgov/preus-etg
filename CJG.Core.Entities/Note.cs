using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="Note"/> class, provides the ORM with a way to manage notes in the system.
	/// </summary>
	public class Note : EntityBase
	{
		/// <summary>
		/// get/set - The primary key identity.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - The foreign key for the type of note this is.
		/// </summary>
		[Index("IX_Note", 1)]
		public NoteTypes NoteTypeId { get; set; }

		/// <summary>
		/// get/set - The type of note this is.
		/// </summary>
		[ForeignKey(nameof(NoteTypeId))]
		public virtual NoteType NoteType { get; set; }

		/// <summary>
		/// get/set - The foreign key to the grant application.
		/// </summary>
		[Index("IX_Note", 2)]
		public int GrantApplicationId { get; set; }

		/// <summary>
		/// get/set - The grant application this note belongs to.
		/// </summary>
		[ForeignKey(nameof(GrantApplicationId))]
		public virtual GrantApplication GrantApplication { get; set; }

		/// <summary>
		/// get/set - The foreign key to the attachment.
		/// </summary>
		public int? AttachmentId { get; set; }

		/// <summary>
		/// get/set - The attachment.
		/// </summary>
		[ForeignKey(nameof(AttachmentId))]
		public virtual Attachment Attachment { get; set; }

		/// <summary>
		/// get/set - The foreign key to the internal User who created this note.  If NULL then it's a system note.
		/// </summary>
		[Index("IX_Note", 3)]
		public int? CreatorId { get; set; }

		/// <summary>
		/// get/set - The internal User who created this note.
		/// </summary>
		[ForeignKey(nameof(CreatorId))]
		public virtual InternalUser Creator { get; set; }

		/// <summary>
		/// get/set - The note content or body.
		/// </summary>
		[Required]
		public string Content { get; set; }

		/// <summary>
		/// Creates a new instance of a <typeparamref name="Note"/> object.
		/// </summary>
		public Note()
		{ }

		/// <summary>
		/// Creates a new instance of a <typeparamref name="Note"/> object and initializes it with the specified property values.
		/// Use this for user notes.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="noteType"></param>
		/// <param name="creator"></param>
		/// <param name="content"></param>
		/// <param name="attachment"></param>
		public Note(GrantApplication grantApplication, NoteType noteType, InternalUser creator, string content, Attachment attachment = null) : this(grantApplication, noteType, content, attachment)
		{
			Creator = creator ?? throw new ArgumentNullException(nameof(creator));
			CreatorId = creator.Id;
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="Note"/> object and initializes it with the specified property values.
		/// Use this for system notes.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="noteType"></param>
		/// <param name="content"></param>
		/// <param name="attachment"></param>
		public Note(GrantApplication grantApplication, NoteType noteType, string content, Attachment attachment = null)
		{
			if (string.IsNullOrEmpty(content))
				throw new ArgumentNullException(nameof(content));

			GrantApplication = grantApplication ?? throw new ArgumentNullException(nameof(grantApplication));
			GrantApplicationId = grantApplication.Id;
			NoteType = noteType ?? throw new ArgumentNullException(nameof(noteType));
			NoteTypeId = (NoteTypes)Enum.Parse(typeof(NoteTypes), noteType.Id.ToString());
			Content = content;
			Attachment = attachment;
			AttachmentId = attachment?.Id;
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="Note"/> object and initializes it with the specified property values.
		/// Use this for user notes.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="noteType"></param>
		/// <param name="creator"></param>
		/// <param name="content"></param>
		/// <param name="attachment"></param>
		public Note(GrantApplication grantApplication, NoteTypes noteType, InternalUser creator, string content, Attachment attachment = null) : this(grantApplication, noteType, content, attachment)
		{
			Creator = creator ?? throw new ArgumentNullException(nameof(creator));
			CreatorId = creator.Id;
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="Note"/> object and initializes it with the specified property values.
		/// Use this for system notes.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="noteType"></param>
		/// <param name="content"></param>
		/// <param name="attachment"></param>
		public Note(GrantApplication grantApplication, NoteTypes noteType, string content, Attachment attachment = null)
		{
			if (string.IsNullOrEmpty(content))
				throw new ArgumentNullException(nameof(content));

			GrantApplication = grantApplication ?? throw new ArgumentNullException(nameof(grantApplication));
			GrantApplicationId = grantApplication.Id;
			NoteTypeId = noteType;
			Content = content;
			Attachment = attachment;
			AttachmentId = attachment?.Id;
		}

		/// <summary>
		/// Returns a note in the following format - $"NoteTypeId:{NoteTypeId}|{NoteType?.ToString()} Creator: {Creator} GrantApplicationId:{GrantApplicationId}: {Content}"
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"NoteTypeId:{NoteTypeId}|{NoteType} Creator: {Creator} GrantApplicationId:{GrantApplicationId}: {Content}";
		}

		/// <summary>
		/// Parse the note content and return a friendly non-json string.
		/// </summary>
		/// <returns></returns>
		public string GetCaption()
		{
			try
			{
				if (Content.StartsWith("{"))
				{
					var entity = JsonConvert.DeserializeObject<EntityChange>(Content);
					return entity.ToString();
				}

				if (Content.StartsWith("["))
				{
					var entities = JsonConvert.DeserializeObject<IEnumerable<EntityChange>>(Content);
					return string.Join("\n", entities.Select(e => e.ToString()));
				}
			}
			catch
			{
				// Ignore a parsing error and just return the raw data.
			}
			return Content;
		}

		/// <summary>
		/// Parse the note content and return a friendly non-json string.
		/// </summary>
		/// <returns></returns>
		public string GetOldValueFromNewValue(string propertyName, string newValue)
		{
			try
			{
				if (Content.StartsWith("["))
				{
					var entities = JsonConvert.DeserializeObject<IEnumerable<EntityChange>>(Content).ToList();
					if (!entities.Any())
						return string.Empty;

					var change = entities
						.SelectMany(n => n.Changes)
						.Where(e => e.Name == propertyName)
						.FirstOrDefault(e => e.NewValue == newValue);

					return change.OldValue;
				}
			}
			catch
			{
				// Ignore a parsing error and just return the raw data.
			}

			return string.Empty;
		}
	}

	internal struct EntityChange
	{
		public string Name { get; set; }
		public IEnumerable<PropertyChange> Changes { get; set; }
		public string State { get; set; }

		public override string ToString()
		{
			return $"{Name} ({State}): \n{(State == "modified" ? string.Join("\n", Changes.Select(c => c.ToString())) : "")}";
		}
	}

	internal struct PropertyChange
	{
		public string Name { get; set; }
		public string OldValue { get; set; }
		public string NewValue { get; set; }

		public override string ToString()
		{
			return $"{this.Name}: changed from '{this.OldValue}' to '{this.NewValue}'";
		}
	}
}

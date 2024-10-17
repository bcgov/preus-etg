using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="Attachment"/> class, provides the ORM with a way to manage attachments.
	/// </summary>
	public class Attachment : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - Primary key uses IDENTITY.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - The current version of the attachment.
		/// </summary>
		[Required, DefaultValue(1)]
		public int VersionNumber { get; set; } = 1;

		/// <summary>
		/// get/set - The file name of the binary data.
		/// </summary>
		[Required, MaxLength(500)]
		public string FileName { get; set; }

		/// <summary>
		/// get/set - A description for the file.
		/// </summary>
		[MaxLength(500)]
		public string Description { get; set; }

		/// <summary>
		/// get/set - The file extension.
		/// </summary>
		[Required, MaxLength(50)]
		public string FileExtension { get; set; }

		/// <summary>
		/// get/set - The binary data of the file.
		/// </summary>
		[Required]
		public byte[] AttachmentData { get; set; }

		/// <summary>
		/// get/set - The type of attachment. Applicants upload attachments. Admins can upload Attachments or Documents. Applicant cannot affect Documents.
		/// </summary>
		[DefaultValue(AttachmentType.Attachment)]
		public AttachmentType AttachmentType { get; set; }

		/// <summary>
		/// get - All of the versions of this attachment.
		/// </summary>
		public ICollection<VersionedAttachment> Versions { get; set; } = new List<VersionedAttachment>();

		/// <summary>
		/// get - All of the grant applications associated with this attachment.
		/// </summary>
		public ICollection<GrantApplication> GrantApplications { get; set; } = new List<GrantApplication>();

		#endregion

		/// <summary>
		/// Creates a new instance of a <typeparamref name="Attachment"/> object.
		/// </summary>
		public Attachment()
		{
			VersionNumber = 1;
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="Attachment"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="description"></param>
		/// <param name="fileExtension"></param>
		/// <param name="data"></param>
		/// <param name="attachmentType"></param>
		public Attachment(string fileName, string description, string fileExtension, byte[] data, AttachmentType attachmentType = AttachmentType.Attachment)
		{
			if (string.IsNullOrEmpty(fileName))
				throw new ArgumentNullException(nameof(fileName));

			if (string.IsNullOrEmpty(fileExtension))
				throw new ArgumentNullException(nameof(fileExtension));

			if (data == null || data.Length == 0)
				throw new ArgumentException("The file data cannot be null or empty.", nameof(data));

			FileName = fileName;
			Description = description;
			FileExtension = fileExtension;
			AttachmentData = data;
			AttachmentType = attachmentType;
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="Attachment"/> object and initializes it with the specified property values.
		/// Copies the specified attachment information.
		/// </summary>
		/// <param name="attachment"></param>
		public Attachment(Attachment attachment) : this()
		{
			if (attachment == null)
				throw new ArgumentNullException(nameof(attachment));

			FileName = attachment.FileName;
			Description = attachment.Description;
			FileExtension = attachment.FileExtension;
			AttachmentData = attachment.AttachmentData;
			AttachmentType = attachment.AttachmentType;
			Versions.Clear();
		}

		/// <summary>
		/// Creates a new <typeparamref name="VersionedAttachment"/> of the current attachment.
		/// Updates the current attachment properties with the specified values.
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="description"></param>
		/// <param name="fileExtension"></param>
		/// <param name="data"></param>
		/// <param name="attachmentType"></param>
		public void CreateNewVersion(string fileName, string description, string fileExtension, byte[] data, AttachmentType attachmentType = AttachmentType.Attachment)
		{
			if (string.IsNullOrEmpty(fileName))
				throw new ArgumentNullException(nameof(fileName));

			if (string.IsNullOrEmpty(fileExtension))
				throw new ArgumentNullException(nameof(fileExtension));

			if (data == null || data.Length == 0)
				throw new ArgumentException("The file data cannot be null or empty.", nameof(data));

			Versions.Add(new VersionedAttachment(this));
			VersionNumber++;
			FileName = fileName;
			Description = description;
			FileExtension = fileExtension;
			AttachmentData = data;
			AttachmentType = attachmentType;
		}
	}
}
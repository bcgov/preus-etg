using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="VersionedAttachment"/> class, provides ORM with a way to manage versioning of attachments.
    /// </summary>
    public class VersionedAttachment : EntityBase
    {
        #region Properties
        /// <summary>
        /// get/set - The primary key and foreign key to parent attachment.
        /// </summary>
        [Key, Column(Order = 0)]
        public int AttachmentId { get; set; }

        /// <summary>
        /// get/set - The parent attachment.
        /// </summary>
        [ForeignKey(nameof(AttachmentId))]
        public virtual Attachment Attachment { get; set; }

        /// <summary>
        /// get/set - The primary key and version number of the attachment.
        /// </summary>
        [Key, Column(Order = 1)]
        public int VersionNumber { get; set; }

        /// <summary>
        /// get/set - The file name.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The file name is required"), MaxLength(500)]
        public string FileName { get; set; }

        /// <summary>
        /// get/set - A description of the attachment.
        /// </summary>
        [MaxLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// get/set - The file extension.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The file extension is required"), MaxLength(50)]
        public string FileExtension { get; set; }

        /// <summary>
        /// get/set - The attachment data.
        /// </summary>
        [Required]
        public byte[] AttachmentData { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates new instance of a <typeparamref name="VersionedAttachment"/> object.
        /// </summary>
        public VersionedAttachment()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="VersionedAttachment"/> object and initializes with the specified property values.
        /// </summary>
        /// <param name="attachment"></param>
        public VersionedAttachment(Attachment attachment)
        {
            this.Attachment = attachment ?? throw new ArgumentNullException(nameof(attachment));
            this.AttachmentId = attachment.Id;
            this.VersionNumber = attachment.VersionNumber;
            this.FileName = attachment.FileName;
            this.Description = attachment.Description;
            this.FileExtension = attachment.FileExtension;
            this.AttachmentData = attachment.AttachmentData;
        }
        #endregion
    }
}
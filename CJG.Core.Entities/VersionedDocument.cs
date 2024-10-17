using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="VersionedDocument"/> class, provides the ORM with a way to managed versioned documents.
    /// </summary>
    public class VersionedDocument : EntityBase
    {
        #region Properties
        /// <summary>
        /// get/set - The primary key and foreign key to the parent document.
        /// </summary>
        [Key, ForeignKey(nameof(Document)), Column(Order = 0)]
        public int DocumentId { get; set; }

        /// <summary>
        /// get/set - The parent document.
        /// </summary>
        public Document Document { get; set; }

        /// <summary>
        /// get/set - The primary key and version number of the document.
        /// </summary>
        [Key, Column(Order = 1)]
        public int VersionNumber { get; set; } = 1;

        /// <summary>
        /// get/set - The foreign key to the parent document template.
        /// </summary>
        public int? DocumentTemplateId { get; set; }

        /// <summary>
        /// get/set - The parent document template used to generate the body.
        /// </summary>
        [ForeignKey(nameof(DocumentTemplateId))]
        public DocumentTemplate DocumentTemplate { get; set; }

        /// <summary>
        /// get/set - The title of the document.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "A title is required"), MaxLength(500)]
        public string Title { get; set; }

        /// <summary>
        /// get/set - The body of the document.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The body is required")]
        public string Body { get; set; }
        #endregion  

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="VersionedDocument"/> object.
        /// </summary>
        public VersionedDocument()
        {
            this.VersionNumber = 1;
        }

        /// <summary>
        /// Creates new instance of a <typeparamref name="VersionedDocument"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="document"></param>
        public VersionedDocument(Document document)
        {
            this.Document = document ?? throw new ArgumentNullException(nameof(document));
            this.DocumentId = document.Id;
            this.VersionNumber = document.VersionNumber;
            this.DocumentTemplate = document.DocumentTemplate;
            this.DocumentTemplateId = document.DocumentTemplateId;
            this.Title = document.Title;
            this.Body = document.Body;
        }
        #endregion

    }
}
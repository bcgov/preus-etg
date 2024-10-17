using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="Document"/> class, provides ORM with a way to manage documents.
    /// </summary>
    public class Document : EntityBase
    {
        #region Properties
        /// <summary>
        /// get/set - The primary key usees IDENTITY.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// get/set - The current version of the document.
        /// </summary>
        [Required, DefaultValue(1)]
        public int VersionNumber { get; set; } = 1;

        /// <summary>
        /// get/set - The foreign key to the document template.
        /// </summary>
        public int? DocumentTemplateId { get; set; }

        /// <summary>
        /// get/set - The document template used to generate this document.
        /// </summary>
        [ForeignKey(nameof(DocumentTemplateId))]
        public DocumentTemplate DocumentTemplate { get; set; }

        /// <summary>
        /// get/set - The title of the document.
        /// </summary>
        [Required, MaxLength(500)]
        public string Title { get; set; }

        /// <summary>
        /// get/set - The body of the document.
        /// </summary>
        [Required]
        public string Body { get; set; }

        /// <summary>
        /// get - All the versions of this document.
        /// </summary>
        public virtual ICollection<VersionedDocument> Versions { get; set; } = new List<VersionedDocument>();
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="Document"/> object.
        /// </summary>
        public Document()
        {
            this.VersionNumber = 1;
        }

        /// <summary>
        /// Creates a new intance of a <typeparamref name="Document"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="body"></param>
        public Document(string title, string body) : this()
        {
            this.Title = title ?? throw new ArgumentNullException(nameof(title));
            this.Body = body ?? throw new ArgumentNullException(nameof(body));
        }

        /// <summary>
        /// Creates a new intance of a <typeparamref name="Document"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="body"></param>
        /// <param name="template"></param>
        public Document(string title, string body, DocumentTemplate template) : this(title, body)
        {
            this.DocumentTemplate = template ?? throw new ArgumentNullException(nameof(template));
            this.DocumentTemplateId = template.Id;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates a new <typeparamref name="VersionedDocument"/> of the current document.
        /// Updates the current document properties with the specified values.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="body"></param>
        public void CreateNewVersion(string title, string body)
        {
            this.Versions.Add(new VersionedDocument(this));
            this.VersionNumber++;
            this.Title = title ?? throw new ArgumentNullException(nameof(title));
            this.Body = body ?? throw new ArgumentNullException(nameof(body));
        }

        /// <summary>
        /// Creatse a new <typeparamref name="VersionedDocument"/> of the current document.
        /// Update the current document properties with the specified document values.
        /// </summary>
        /// <param name="document"></param>
        public void CreateNewVersion(Document document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            this.Versions.Add(new VersionedDocument(this));
            this.VersionNumber++;
            this.Title = document.Title;
            this.Body = document.Body;
        }
        #endregion
    }
}

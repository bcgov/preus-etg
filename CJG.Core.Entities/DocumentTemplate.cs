using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="DocumentTemplate"/> class, provides the ORM a way to manage the document templates.
	/// A document template provides dynamically generated text.
	/// </summary>
	public class DocumentTemplate : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - The primary key uses IDENTITY.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - The document type.
		/// </summary>
		[Required]
		[Index("IX_DocumentTemplate", 1)]
		public DocumentTypes DocumentType { get; set; }

		/// <summary>
		/// get/set - The title of the template.
		/// </summary>
		[Required, MaxLength(250)]
		[Index("IX_DocumentTemplate", 3)]
		public string Title { get; set; }

		/// <summary>
		/// get/set - A description of the template.
		/// </summary>
		[MaxLength(500)]
		public string Description { get; set; }

		/// <summary>
		/// get/set - The body of the template.
		/// </summary>
		[Required]
		public string Body { get; set; }

		/// <summary>
		/// get/set - Whether the template is active.
		/// </summary>
		[Index("IX_DocumentTemplate", 2), DefaultValue(false)]
		public bool IsActive { get; set; }

		/// <summary>
		/// get - All the grant program applicant declaration templates.
		/// </summary>
		public virtual ICollection<GrantProgram> ApplicantDeclarationTemplates { get; set; }

		/// <summary>
		/// get - All the grant program applicant coverletter templates.
		/// </summary>
		public virtual ICollection<GrantProgram> ApplicantCoverLetterTemplates { get; set; }

		/// <summary>
		/// get - All the grant pgoram applicant schedule A templates.
		/// </summary>
		public virtual ICollection<GrantProgram> ApplicantScheduleATemplates { get; set; }

		/// <summary>
		/// get - All the grant program applicant schedule B templates.
		/// </summary>
		public virtual ICollection<GrantProgram> ApplicantScheduleBTemplates { get; set; }

		/// <summary>
		/// get - All the grant program participant consent templates.
		/// </summary>
		public virtual ICollection<GrantProgram> ParticipantConsentTemplates { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="DocumentTemplate"/> object.
		/// </summary>
		public DocumentTemplate()
		{ }

		/// <summary>
		/// Creates a new instance of a <typeparamref name="DocumentTemplate"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="documentType"></param>
		/// <param name="title"></param>
		/// <param name="body"></param>
		/// <param name="isActive"></param>
		public DocumentTemplate(DocumentTypes documentType, string title, string body, bool isActive = true)
		{	
			this.DocumentType = documentType;
			this.Title = title ?? throw new ArgumentNullException(nameof(title));
			this.Body = body ?? throw new ArgumentNullException(nameof(body));
			this.IsActive = isActive;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Validates the <typeparamref name="DocumentTemplate"/> property values.
		/// </summary>
		/// <param name="validationContext"></param>
		/// <returns></returns>
		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var context = validationContext.GetDbContext();

			if (context == null)
				yield break;

			if (!this.IsActive)
			{
				if (context.Set<GrantProgram>().Any(dt => dt.State == GrantProgramStates.Implemented &&
														  (dt.ApplicantCoverLetterTemplateId == dt.Id ||
														   dt.ApplicantDeclarationTemplateId == dt.Id ||
														   dt.ApplicantScheduleATemplateId == dt.Id ||
														   dt.ApplicantScheduleBTemplateId == dt.Id ||
														   dt.ParticipantConsentTemplateId == dt.Id)))
					yield return new ValidationResult($"Document cannot set inactive due to there is at least one Grant Program associated to it.", new[] { nameof(this.IsActive) });
			}
			foreach (var validation in base.Validate(validationContext))
			{
				yield return validation;
			}
		}
		#endregion
	}
}

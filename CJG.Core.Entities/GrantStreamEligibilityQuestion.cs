using CJG.Core.Entities.Attributes;
using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="GrantStreamEligibilityQuestion"/> class, provides ORM a way to manage stream grants.
	/// </summary>
	public class GrantStreamEligibilityQuestion : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - Primary key uses IDENTITY.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - Foreign key to the parent grant program.
		/// </summary>
		[Index("IX_GrantStreamEligibilityQuestions", 1)]
		public int GrantStreamId { get; set; }

		/// <summary>
		/// get/set - The parent grant program.
		/// </summary>
		[ForeignKey(nameof(GrantStreamId))]
		public virtual GrantStream GrantStream { get; set; }

		/// <summary>
		/// get/set - The grant stream Eligibility Requirements text.
		/// </summary>
		[MaxLength(2000)]
		public string EligibilityRequirements { get; set; }

		/// <summary>
		/// get/set - The grant stream Eligibility Question.
		/// </summary>
		[MaxLength(2000)]
		[Required]
		public string EligibilityQuestion { get; set; }

		/// <summary>
		/// get/set - Whether question is enabled.
		/// </summary>
		[Required]
		public bool IsActive { get; set; }

		/// <summary>
		/// get/set - Whether question is required to be answered YES.
		/// </summary>
		[Required]
		public bool EligibilityPositiveAnswerRequired { get; set; }

		/// <summary>
		/// get/set - The order questions will be presented.
		/// </summary>
		[Required]
		public int RowSequence { get; set; }

		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="GrantStream"/> object.
		/// </summary>
		public GrantStreamEligibilityQuestion()
		{
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="GrantStreamEligibilityQuestions"/> object.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="objective"></param>
		/// <param name="program"></param>
		/// <param name="payment"></param>
		public GrantStreamEligibilityQuestion(string EligibilityRequirements, string EligibilityQuestion, bool IsActive,
			bool EligibilityPositiveAnswerRequired, int RowSequence, GrantStream GrantStream)
		{
			this.EligibilityRequirements = EligibilityRequirements;
			this.EligibilityQuestion = EligibilityQuestion ?? throw new ArgumentNullException(nameof(EligibilityQuestion));
			this.IsActive = IsActive;
			this.EligibilityPositiveAnswerRequired = EligibilityPositiveAnswerRequired;
			this.RowSequence = RowSequence;
			this.GrantStreamId = GrantStream.Id;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Validate this grant stream before making changes to the datasource.
		/// </summary>
		/// <param name="validationContext"></param>
		/// <returns></returns>
		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var context = validationContext.GetDbContext();
			var entry = validationContext.GetDbEntityEntry();

			// This is done to stop errors from being thrown when developers use EF entities in ViewModels.
			if (entry == null || context == null)
				yield break;

			if (this.GrantStreamId > 0 && this.GrantStream == null)
				this.GrantStream = context.Set<GrantStream>().Find(this.GrantStreamId);

			// Must be linked to an existing grant program.
			if (this.GrantStreamId == 0)
				yield return new ValidationResult("The eligibiility question must be associated to an existing grant stream.", new[] { nameof(this.GrantStreamId) });

			// EligibilityQuestion must have a value.
			if (string.IsNullOrEmpty(EligibilityQuestion))
				yield return new ValidationResult("The Eligibility Question must be defined.", new[] { nameof(this.EligibilityQuestion) });

			foreach (var validation in base.Validate(validationContext))
			{
				yield return validation;
			}
		}
		#endregion
	}
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="GrantStreamEligibilityQuestion"/> class, provides ORM a way to manage stream grants.
	/// </summary>
	public class GrantStreamEligibilityQuestion : EntityBase
	{
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

		[Required]
		public bool EligibilityRationaleAnswerAllowed { get; set; }

		public string EligibilityRationaleAnswerLabel { get; set; }

		/// <summary>
		/// get/set - The order questions will be presented.
		/// </summary>
		[Required]
		public int RowSequence { get; set; }

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

			if (GrantStreamId > 0 && GrantStream == null)
				GrantStream = context.Set<GrantStream>().Find(GrantStreamId);

			// Must be linked to an existing grant program.
			if (GrantStreamId == 0)
				yield return new ValidationResult("The eligibility question must be associated to an existing grant stream.", new[] { nameof(GrantStreamId) });

			// EligibilityQuestion must have a value.
			if (string.IsNullOrEmpty(EligibilityQuestion))
				yield return new ValidationResult("The Eligibility Question must be defined.", new[] { nameof(EligibilityQuestion) });

			// EligibilityQuestion must have a value.
			//if (EligibilityRationaleAnswerAllowed && !string.IsNullOrWhiteSpace(EligibilityRationaleAnswerLabel))
			//	yield return new ValidationResult("The Eligibility Question rationale label must be defined.", new[] { nameof(EligibilityRationaleAnswerLabel) });

			foreach (var validation in base.Validate(validationContext))
			{
				yield return validation;
			}
		}
	}
}

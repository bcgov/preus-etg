using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="GrantStreamEligibilityAnswer"/> class, provides ORM a way to manage stream grants.
	/// </summary>
	public class GrantStreamEligibilityAnswer : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - Primary key uses IDENTITY.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - Foreign key to the parent grant application.
		/// </summary>
		[Index("IX_GrantStreamEligibilityAnswers", 1)]
		public int GrantApplicationId { get; set; }

		/// <summary>
		/// get/set - The parent grant program.
		/// </summary>
		[ForeignKey(nameof(GrantApplicationId))]
		public virtual GrantApplication GrantApplication { get; set; }

		/// <summary>
		/// get/set - Foreign key to the parent question.
		/// </summary>
		[Index("IX_GrantStreamEligibilityAnswers", 2)]
		public int GrantStreamEligibilityQuestionId { get; set; }

		/// <summary>
		/// get/set - The parent question.
		/// </summary>
		[ForeignKey(nameof(GrantStreamEligibilityQuestionId))]
		public virtual GrantStreamEligibilityQuestion GrantStreamEligibilityQuestions { get; set; }

		/// <summary>
		/// get/set - Answer to question
		/// </summary>
		[Required]
		public bool EligibilityAnswer { get; set; }

		/// <summary>
		/// get/set - Optional Rationale when EligibilityAnswer is 'yes'. Will contain HTML.
		/// </summary>
		public string RationaleAnswer { get; set; }

		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="GrantStream"/> object.
		/// </summary>
		public GrantStreamEligibilityAnswer()
		{
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="GrantStreamEligibilityQuestions"/> object.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="objective"></param>
		/// <param name="program"></param>
		/// <param name="payment"></param>
		public GrantStreamEligibilityAnswer(int GrantApplicationId, int GrantStreamEligibilityQuestionId, bool EligibilityAnswer)
		{
			this.GrantApplicationId = GrantApplicationId;
			this.GrantStreamEligibilityQuestionId = GrantStreamEligibilityQuestionId;
			this.EligibilityAnswer = EligibilityAnswer;
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

			this.GrantApplicationId = GrantApplicationId;
			this.GrantStreamEligibilityQuestionId = GrantStreamEligibilityQuestionId;
			this.EligibilityAnswer = EligibilityAnswer;


			if (this.GrantApplicationId > 0 && this.GrantApplication == null)
				this.GrantApplication = context.Set<GrantApplication>().Find(this.GrantApplicationId);

			if (this.GrantStreamEligibilityQuestionId > 0 && this.GrantStreamEligibilityQuestions == null)
				this.GrantStreamEligibilityQuestions = context.Set<GrantStreamEligibilityQuestion>().Find(this.GrantStreamEligibilityQuestionId);

			// Must be linked to an existing grant program.
			if (this.GrantStreamEligibilityQuestionId == 0)
				yield return new ValidationResult("The eligibility answer must be associated to a stream eligibility question.", new[] { nameof(this.GrantStreamEligibilityQuestionId) });


			foreach (var validation in base.Validate(validationContext))
			{
				yield return validation;
			}
		}
		#endregion
	}
}

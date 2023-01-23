using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	public class PrioritizationScoreBreakdownAnswer : EntityBase
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Column("GrantStreamEligibilityQuestionId")]
		public virtual GrantStreamEligibilityQuestion QuestionedAnswered { get; set; }
		public int QuestionScore { get; set; }
	}
}
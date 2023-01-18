using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CJG.Core.Entities
{
	public class PrioritizationScoreBreakdown : EntityBase
	{
		/// <summary>
		/// get/set - Primary key uses IDENTITY.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public int IndustryScore { get; set; }
		public string IndustryName { get; set; }
		public string IndustryCode { get; set; }

		public int RegionalScore { get; set; }
		public string RegionalName { get; set; }

		public int SmallBusinessScore { get; set; }
		public int FirstTimeApplicantScore { get; set; }

		[NotMapped]
		public int QuestionScoreTotal
		{
			get { return EligibilityAnswerScores.Sum(q => q.QuestionScore); }
		}

		public virtual ICollection<PrioritizationScoreBreakdownAnswer> EligibilityAnswerScores { get; set; } = new List<PrioritizationScoreBreakdownAnswer>();

		public int GetTotalScore()
		{
			return IndustryScore + RegionalScore + SmallBusinessScore + FirstTimeApplicantScore + QuestionScoreTotal;
		}

		/// <summary>
		/// Does this breakdown encounter an Exception with it's region lookup? Ie: No matching postal code
		/// </summary>
		/// <returns></returns>
		public bool HasRegionalException()
		{
			return string.IsNullOrWhiteSpace(RegionalName);
		}
	}
}
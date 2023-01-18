using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	public class PrioritizationIndustryScore : EntityBase
	{
		/// <summary>
		/// get/set - Primary key uses IDENTITY.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public string Name { get; set; } // Assuming we're using the 2017 Coding right now
		public string NaicsCode { get; set; } // Assuming we're using the 2017 Coding right now

		public int IndustryScore { get; set; } // (1 to 4) 1 = Greater Priority, 4 = Lower Priority
	}
}
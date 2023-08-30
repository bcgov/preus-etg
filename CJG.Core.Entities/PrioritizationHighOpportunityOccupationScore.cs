using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	public class PrioritizationHighOpportunityOccupationScore : EntityBase
	{
		/// <summary>
		/// get/set - Primary key uses IDENTITY.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public string Name { get; set; } 
		public string NOCCode { get; set; } // Assuming we're using the 2021 5-digit noc coding

		public int HighOpportunityOccupationScore { get; set; } // (1 to 4) 1 = Greater Priority, 4 = Lower Priority
	}
}
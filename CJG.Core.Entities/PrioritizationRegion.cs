using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	public class PrioritizationRegion : EntityBase
	{
		/// <summary>
		/// get/set - Primary key uses IDENTITY.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public string Name { get; set; } // Region Name
		public decimal RegionalScore { get; set; } // (1 to 4) 1 = Less Good, 4 = More good

		public virtual ICollection<PrioritizationPostalCode> PostalCodes { get; set; } = new List<PrioritizationPostalCode>();
	}
}
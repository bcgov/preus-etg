using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	public class PrioritizationPostalCode : EntityBase
	{
		/// <summary>
		/// get/set - Primary key uses IDENTITY.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[ForeignKey("Region")]
		public int RegionId { get; set; }
		public virtual PrioritizationRegion Region { get; set; }

		[MaxLength(10)]
		public string PostalCode { get; set; }
	}
}
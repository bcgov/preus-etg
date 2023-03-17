using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	public class PrioritizationThreshold : EntityBase
	{
		/// <summary>
		/// get/set - Primary key uses IDENTITY.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public int IndustryThreshold { get; set; }
		public decimal RegionalThreshold { get; set; }
		public int EmployeeCountThreshold { get; set; }

		public int IndustryAssignedScore { get; set; }
		public int RegionalThresholdAssignedScore { get; set; }
		public int EmployeeCountAssignedScore { get; set; }
		public int FirstTimeApplicantAssignedScore { get; set; }
	}
}
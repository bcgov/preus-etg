using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	/// <summary>
	/// A ServiceLineBreakdown class, provides a way to manage related lists for a service line.
	/// </summary>
	public class ServiceLineBreakdown : LookupTable<int>
	{
		#region Properties
		/// <summary>
		/// get/set - A description of this service line breakdown.
		/// </summary>
		[MaxLength(1000)]
		public string Description { get; set; }
	   
		/// <summary>
		/// get/set - The foreign key to the parent service line.
		/// </summary>
		public int ServiceLineId { get; set; }

		/// <summary>
		/// get/set - The parent service line.
		/// </summary>
		[ForeignKey(nameof(ServiceLineId))]
		public virtual ServiceLine ServiceLine { get; set; }

		/// <summary>
		/// get - The training programs associated with this service line breakdown.
		/// </summary>
		public virtual ICollection<TrainingProgram> TrainingPrograms { get; set; } = new List<TrainingProgram>();
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a ServiceLineBreakdown object.
		/// </summary>
		public ServiceLineBreakdown()
		{

		}

		/// <summary>
		/// Creates a new instance of a ServiceLineBreakdown object.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="serviceLineId"></param>
		public ServiceLineBreakdown(string caption, int serviceLineId) : base(caption)
		{
			this.ServiceLineId = serviceLineId;
		}
		#endregion
	}
}

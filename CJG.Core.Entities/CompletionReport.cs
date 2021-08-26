using CJG.Core.Entities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	/// <summary>
	/// A CompletionReport class, provides a way to manage different types of completion reports.
	/// </summary>
	public class CompletionReport : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - The primary key uses IDENTITY.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - The caption for the completion report.
		/// </summary>
		[Required, MaxLength(250)]
		public string Caption { get; set; }

		/// <summary>
		/// get/set - The description for the completion report.
		/// </summary>
		[MaxLength(500)]
		public string Description { get; set; }

		/// <summary>
		/// get/set - Indicates whether or not the completion report is active.
		/// </summary>
		[DefaultValue(true)]
		public bool IsActive { get; set; } = true;

		/// <summary>
		/// get/set - The date on which this completion report becomes effective.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Column(TypeName = "DATETIME2")]
		public DateTime EffectiveDate { get; set; }

		/// <summary>
		/// get - All of the question groups associated with this completion report.
		/// </summary>
		public virtual ICollection<CompletionReportGroup> Groups { get; set; } = new List<CompletionReportGroup>();

		/// <summary>
		/// get - All of the questions associated with this completion report.
		/// </summary>
		public virtual ICollection<CompletionReportQuestion> Questions { get; set; } = new List<CompletionReportQuestion>();

		/// <summary>
		/// get - All of the grantApplications associated with this completion report.
		/// </summary>
		public virtual ICollection<GrantApplication> GrantApplications { get; set; } = new List<GrantApplication>();
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a CompletionReport object.
		/// </summary>
		public CompletionReport()
		{

		}

		/// <summary>
		/// Creates a new instance of a CompletionReport object and initializes it.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="effectiveDate"></param>
		public CompletionReport(string caption, DateTime effectiveDate)
		{
			this.Caption = caption;
			this.EffectiveDate = effectiveDate;
		}
		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	/// <summary>
	/// A CompletionReportGroup class, provides a way to group questions within a completion report.
	/// </summary>
	public class CompletionReportGroup : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - The primary key uses IDENTITY.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - The title of the question group.
		/// </summary>
		[Required, MaxLength(500)]
		public string Title { get; set; }

		/// <summary>
		/// get/set - The sequence to display the question group.
		/// </summary>
		public int RowSequence { get; set; }

		/// <summary>
		/// get/set - The foreign key to the program type.
		/// </summary>
		public ProgramTypes? ProgramTypeId { get; set; }

		/// <summary>
		/// get/set - The program type that controls the application process.
		/// </summary>
		[ForeignKey(nameof(ProgramTypeId))]
		public virtual ProgramType ProgramType { get; set; }

		/// <summary>
		/// get/set - Foreign key to the completion report.
		/// </summary>
		public int CompletionReportId { get; set; }

		/// <summary>
		/// get/set - The completion report.
		/// </summary>
		[ForeignKey(nameof(CompletionReportId))]
		public virtual CompletionReport CompletionReport { get; set; }

		/// <summary>
		/// get - All the questions within this group.
		/// </summary>
		public virtual ICollection<CompletionReportQuestion> Questions { get; set; } = new List<CompletionReportQuestion>();
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a CompletionReportGroup object.
		/// </summary>
		public CompletionReportGroup()
		{

		}

		/// <summary>
		/// Creates a new instance of a CompletionReportGroup object and initializes it.
		/// </summary>
		/// <param name="title"></param>
		public CompletionReportGroup(string title)
		{
			this.Title = string.IsNullOrEmpty(title) ? throw new ArgumentNullException(nameof(title)) : title;
		}
		#endregion
	}
}

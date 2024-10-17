using CJG.Application.Services;
using CJG.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace CJG.Web.External.Models.Shared
{
	public class LookupTableViewModel : BaseViewModel
	{
		#region Properties
		[Required(ErrorMessage = "The Caption field is required.")]
		public string Caption { get; set; }

		public string Description { get; set; }

		public bool IsActive { get; set; }

		public int RowSequence { get; set; }

		public bool Selected { get; set; }

		public string RowVersion { get; set; }

		public bool Deleted { get; set; }

		public bool AllowDelete { get; set; } = true;
		#endregion

		#region Constructors
		public LookupTableViewModel() { }

		public LookupTableViewModel(LookupTable<int> lookupTable)
		{
			Utilities.MapProperties(lookupTable, this);
		}
		#endregion
	}
}
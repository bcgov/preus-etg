using CJG.Core.Entities;

namespace CJG.Application.Business.Models
{
	/// <summary>
	/// <typeparamref name="CollectionItemModel" class, provides a generic way to pass collection information in/out of the services.
	/// </summary>
	public class CollectionItemModel
	{
		#region Properties
		public string Key { get; set; }
		public int Id { get; set; }
		public string Caption { get; set; }
		public string Description { get; set; }
		public bool IsActive { get; set; } = true;
		public bool IsSelected { get; set; }
		public bool CanEdit { get; set; } = true;
		public int RowSequence { get; set; }
		public int? Parent { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="CollectionItemModel"/> object.
		/// </summary>
		public CollectionItemModel()
		{

		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="CollectionItemModel"/> object and initalizes it.
		/// </summary>
		/// <param name="lookup"></param>
		/// <param name="description"></param>
		/// <param name="selected"></param>
		/// <param name="canEdit"></param>
		public CollectionItemModel(LookupTable<int> lookup, string description, bool selected, bool canEdit = true)
		{
			this.Id = lookup.Id;
			this.Caption = lookup.Caption;
			this.IsActive = lookup.IsActive;
			this.RowSequence = lookup.RowSequence;
			this.Description = description;
			this.IsSelected = selected;
			this.CanEdit = canEdit;
		}
		#endregion
	}
}

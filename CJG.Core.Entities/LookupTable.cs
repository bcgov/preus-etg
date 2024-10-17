using DataAnnotationsExtensions;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="LookupTable"/> abstract class, provides us a way to define a new Lookup Table without
	/// needing to repeat the monotony of defining the same two fields every time.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class LookupTable<T> : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - The primary key.
		/// </summary>
		[Key]
		public T Id { get; set; }

		/// <summary>
		/// get/set - The unique caption.
		/// </summary>
		[Required, MaxLength(250, ErrorMessage = "The caption cannot be longer than 250 characters."), Index("IX_Caption", IsUnique = true)]
		public string Caption { get; set; }

		/// <summary>
		/// get/set - Whether this item is active.
		/// </summary>
		[DefaultValue(true), Index("IX_Active")]
		public bool IsActive { get; set; }

		/// <summary>
		/// get/set - The sequence to display this item.
		/// </summary>
		[Min(0)]
		public int RowSequence { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="LookupTable"/> object.
		/// </summary>
		public LookupTable() : this(null, 0)
		{
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="LookupTable"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="caption"></param>
		/// <param name="rowSequence"></param>
		public LookupTable(T id, string caption, int rowSequence = 0) : this(caption, rowSequence)
		{
			if (id == null)
				throw new ArgumentNullException(nameof(id));

			this.Id = id;
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="LookupTable"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="rowSequence"></param>
		public LookupTable(string caption = null, int rowSequence = 0)
		{
			this.Caption = caption;
			this.IsActive = true;
			this.RowSequence = rowSequence;
		}
		#endregion
	}
}

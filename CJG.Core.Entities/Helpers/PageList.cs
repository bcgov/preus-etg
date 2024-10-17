using System.Collections.Generic;
using System.Linq;

namespace CJG.Core.Entities.Helpers
{
	/// <summary>
	/// PageList class, provides a standardize way to return paged content.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class PageList<T>
		where T : class
	{
		#region Properties
		/// <summary>
		/// get/set - The page number.
		/// </summary>
		public int Page { get; set; }

		/// <summary>
		/// get/set - The number of items per page.
		/// </summary>
		public int Quantity { get; set; }

		/// <summary>
		/// get/set - The total number of items in the datasource for the specified query.
		/// </summary>
		public int? Total { get; set; }

		/// <summary>
		/// get/set - A collection of items on this page.
		/// </summary>
		public IEnumerable<T> Items { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a PageList object.
		/// </summary>
		public PageList()
		{
			this.Page = 1;
		}

		/// <summary>
		/// Creates a new instance of a PageList object and initializes it with the specified items.
		/// </summary>
		/// <param name="items"></param>
		public PageList(IEnumerable<T> items) : this()
		{
			this.Items = items;
			this.Quantity = items.Count();
		}

		/// <summary>
		/// Creates a new instance of a PageList object and initializes it with the specified properties.
		/// </summary>
		/// <param name="page">The page number</param>
		/// <param name="quantity">The number of items per page</param>
		/// <param name="total">The total number of items in the datasource for the specified filter</param>
		/// <param name="items">The items on the page</param>
		public PageList(int page, int quantity, int total, IEnumerable<T> items) : this()
		{
			this.Page = page;
			this.Quantity = quantity;
			this.Total = total;
			this.Items = items;
		}
		#endregion
	}
}

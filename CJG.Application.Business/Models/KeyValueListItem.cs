namespace CJG.Application.Business.Models
{
	public class KeyValueListItem<TKey, TValue>
	{
		#region Properties
		public TKey Key { get; set; }
		public TValue Value { get; set; }
		public bool Selected { get; set; }
		#endregion

		#region Constructors
		public KeyValueListItem()
		{
		}

		public KeyValueListItem(TKey key, TValue value)
		{
			Key = key;
			Value = value;
		}

		public KeyValueListItem(TKey key, TValue value, bool selected)
		{
			Key = key;
			Value = value;
			Selected = selected;
		}
		#endregion
	}
}

namespace CJG.Core.Entities
{
	/// <summary>
	/// MartialStatus class, provides a way to manage all martial status.
	/// </summary>
	public class MartialStatus : LookupTable<int>
    {
		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="MartialStatus"/> object.
		/// </summary>
		public MartialStatus()
        {
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="MartialStatus"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="rowSequence"></param>
		public MartialStatus(string caption, int rowSequence = 0) : base(caption, rowSequence)
		{
		}
		#endregion
	}
}

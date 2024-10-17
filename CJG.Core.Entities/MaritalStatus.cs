namespace CJG.Core.Entities
{
	/// <summary>
	/// MaritalStatus class, provides a way to manage all marital status.
	/// </summary>
	
	public class MaritalStatus : LookupTable<int>
    {
		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="MaritalStatus"/> object.
		/// </summary>
		public MaritalStatus()
        {
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="MaritalStatus"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="rowSequence"></param>
		public MaritalStatus(string caption, int rowSequence = 0) : base(caption, rowSequence)
		{
		}
		#endregion
	}
}

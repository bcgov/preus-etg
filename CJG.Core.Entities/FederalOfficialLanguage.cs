namespace CJG.Core.Entities
{
	/// <summary>
	/// FederalOfficialLanguage class, provides a way to manage all federal official language.
	/// </summary>
	public class FederalOfficialLanguage : LookupTable<int>
    {
		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="FederalOfficialLanguage"/> object.
		/// </summary>
		public FederalOfficialLanguage()
        {
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="FederalOfficialLanguage"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="rowSequence"></param>
		public FederalOfficialLanguage(string caption, int rowSequence = 0) : base(caption, rowSequence)
		{
		}
		#endregion
	}
}

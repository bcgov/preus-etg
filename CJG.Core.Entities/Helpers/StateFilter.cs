namespace CJG.Core.Entities.Helpers
{
	public struct StateFilter<T>
	{
		#region Properties
		public T State { get; }
		public bool Include { get; }
		#endregion

		#region Constructors
		public StateFilter(T state, bool include = true)
		{
			this.State = state;
			this.Include = include;
		}
		#endregion
	}
}

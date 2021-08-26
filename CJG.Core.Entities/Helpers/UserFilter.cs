namespace CJG.Core.Entities.Helpers
{
	public struct UserFilter
	{
		#region Properties
		public string SearchCriteria { get; }
		public string[] OrderBy { get; }
		#endregion

		#region Constructors
		public UserFilter(string searchCriteria, string[] orderBy = null)
		{
			this.SearchCriteria = searchCriteria;
			this.OrderBy = orderBy;
		}
		#endregion
	}
}

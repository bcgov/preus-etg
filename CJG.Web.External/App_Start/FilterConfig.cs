using CJG.Web.External.Helpers.Filters;
using System.Web.Mvc;

namespace CJG.Web.External
{
	/// <summary>
	/// <typeparamref name="FilterConfig"/> class, provides a way to configure filters.
	/// </summary>
	public class FilterConfig
	{
		/// <summary>
		/// Register global filters.
		/// </summary>
		/// <param name="filters"></param>
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			//filters.Add(new HandleErrorAttribute());
			filters.Add(new ResponseTimeFilterAttribute());
			filters.Add(new IdentitySignOnFilterAttribute(), -1);
			filters.Add(new MessageFilter());
			//filters.Add(new PermissionsFilter());
		}
	}
}

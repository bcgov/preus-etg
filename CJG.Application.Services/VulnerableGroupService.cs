using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	public class VulnerableGroupService : Service, IVulnerableGroupService
	{
		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public VulnerableGroupService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}
		#endregion

		#region Methods
		public IEnumerable<VulnerableGroup> GetVulnerableGroups(int[] ids)
		{
			return _dbContext.VulnerableGroups.Where(e => e.IsActive && ids.Contains(e.Id));
		}
		#endregion
	}
}

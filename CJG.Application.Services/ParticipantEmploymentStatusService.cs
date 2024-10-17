using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	public class ParticipantEmploymentStatusService : Service, IParticipantEmploymentStatusService
	{
		#region Constructors
		/// <summary>
		///
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public ParticipantEmploymentStatusService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}

		#endregion

		#region Methods
		public IEnumerable<ParticipantEmploymentStatus> GetParticipantEmploymentStatuses(int[] ids)
		{
			return _dbContext.ParticipantEmploymentStatuses.Where(e => e.IsActive && ids.Contains(e.Id));
		}
		#endregion
	}
}

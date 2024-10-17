using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System.Web;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="ReportRateService"/> class, provides a way to manage <typeparamref name="ReportRate"/> objects.
	/// </summary>
	public class ReportRateService : Service, IReportRateService
	{
		#region Variables
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="ReportRateService"/> object.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public ReportRateService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}
		#endregion

		#region Methods
		public ReportRate Get(int fiscalYearId, int grantProgramId, int grantStreamId)
		{
			return _dbContext.ReportRates.Find(fiscalYearId, grantProgramId, grantStreamId);
		}

		public ReportRate Add(ReportRate reportRate)
		{
			_dbContext.ReportRates.Add(reportRate);

			return reportRate;
		}
		#endregion
	}
}

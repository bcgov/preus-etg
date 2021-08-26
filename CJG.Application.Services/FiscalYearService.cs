using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="FiscalYearService"/> class, provides a way to manage <typeparamref name="FiscalYear"/> objects.
	/// </summary>
	public class FiscalYearService : Service, IFiscalYearService
	{
		#region Variables
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="FiscalYearService"/> object.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public FiscalYearService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}
		#endregion

		#region Methods
		public FiscalYear Get(int id)
		{
			return Get<FiscalYear>(id);
		}

		public FiscalYear GetFiscalYear(DateTime date)
		{
			return _dbContext.FiscalYears.AsNoTracking().Where(fy => fy.StartDate <= date && fy.EndDate >= date).OrderBy(fy => fy.StartDate).FirstOrDefault();
		}

		public IEnumerable<ReportRate> GetReportRates(int fiscalYearId)
		{
			return _dbContext.ReportRates.AsNoTracking().Where(rr => rr.FiscalYearId == fiscalYearId);
		}

		public IEnumerable<FiscalYear> GetFiscalYears()
		{
			return _dbContext.FiscalYears.ToArray();
		}

		public IEnumerable<TrainingPeriod> GetTrainingPeriods(int? fiscalYearId)
		{
			return _dbContext.TrainingPeriods.AsNoTracking().Where(o => fiscalYearId == null || o.FiscalYearId == fiscalYearId).ToArray();
		}
		#endregion
	}
}

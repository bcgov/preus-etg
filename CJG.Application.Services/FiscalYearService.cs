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
		/// <summary>
		/// Creates a new instance of a <typeparamref name="FiscalYearService"/> object.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public FiscalYearService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}

		public FiscalYear Get(int id)
		{
			return Get<FiscalYear>(id);
		}

		public FiscalYear GetFiscalYear(DateTime date)
		{
			return _dbContext.FiscalYears
				.AsNoTracking()
				.Where(fy => fy.StartDate <= date && fy.EndDate >= date)
				.OrderBy(fy => fy.StartDate)
				.FirstOrDefault();
		}

		public FiscalYear GetCurrentFiscalYear()
		{
			return GetFiscalYear(AppDateTime.UtcNow);
		}

		public IEnumerable<ReportRate> GetReportRates(int fiscalYearId)
		{
			return _dbContext.ReportRates
				.AsNoTracking()
				.Where(rr => rr.FiscalYearId == fiscalYearId);
		}

		public IEnumerable<FiscalYear> GetFiscalYears()
		{
			return _dbContext.FiscalYears
				.OrderBy(fy => fy.StartDate)
				.ThenBy(fy => fy.Caption)
				.ToArray();
		}

		// TODO: Look at moving this to the TrainingPeriodService class
		public IEnumerable<KeyValuePair<string, string>> GetTrainingPeriodLabels(int? fiscalYearId, int? grantStreamId)
		{
			var periods = GetTrainingPeriods(fiscalYearId, grantStreamId);

			return periods
				.OrderBy(t => t.Caption)
				.Select(tp => tp.Caption)
				.Distinct()
				.ToList()
				.Select(caption => new KeyValuePair<string, string>(caption, caption));
		}

		public TrainingPeriod GetCurrentTrainingPeriodFor(int? fiscalYearId, int? grantStreamId)
		{
			var now = AppDateTime.UtcNow;
			return GetTrainingPeriods(fiscalYearId, grantStreamId)
				.Where(fy => fy.StartDate <= now && fy.EndDate >= now)
				.OrderBy(t => t.Caption)
				.FirstOrDefault();
		}

		private IQueryable<TrainingPeriod> GetTrainingPeriods(int? fiscalYearId, int? grantStreamId)
		{
			var periods = _dbContext
				.TrainingPeriods
				.AsNoTracking()
				.AsQueryable();

			if (fiscalYearId.HasValue && fiscalYearId > 0)
				periods = periods.Where(o => fiscalYearId == null || o.FiscalYearId == fiscalYearId);

			if (grantStreamId.HasValue)
				periods = periods.Where(o => o.GrantStreamId == grantStreamId);
			return periods;
		}
	}
}

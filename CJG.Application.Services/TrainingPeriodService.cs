using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CJG.Application.Business.Models;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;

namespace CJG.Application.Services
{
	public class TrainingPeriodService : Service, ITrainingPeriodService
	{
		public TrainingPeriodService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}

		public IEnumerable<TrainingPeriod> GetAllFor(int fiscalYearId, int grantProgramId)
		{
			return _dbContext.TrainingPeriods
				.Where(tp => tp.FiscalYearId == fiscalYearId)
				.Where(tp => tp.GrantStream.GrantProgramId == grantProgramId)
				.OrderBy(fy => fy.StartDate)
				.ThenBy(fy => fy.Caption);
		}

		public IEnumerable<TrainingPeriod> GetAllFor(int fiscalYearId, int grantProgramId, int grantStreamId)
		{
			return _dbContext.TrainingPeriods
				.Where(tp => tp.FiscalYearId == fiscalYearId)
				.Where(tp => tp.GrantStream.GrantProgramId == grantProgramId)
				.Where(tp => tp.GrantStream.Id == grantStreamId)
				.OrderBy(fy => fy.StartDate)
				.ThenBy(fy => fy.Caption);
		}

		public TrainingPeriod Get(int id)
		{
			return _dbContext.TrainingPeriods.Find(id);
		}

		public void SetDefaultDates(TrainingPeriod trainingPeriod)
		{
			trainingPeriod.DefaultPublishDate = trainingPeriod.StartDate.AddMonths(-3);  // Have to make sure not to UTC these dates here as they already have been.
			trainingPeriod.DefaultOpeningDate = trainingPeriod.StartDate.AddMonths(-2);
		}

		public TrainingPeriod Add(TrainingPeriod trainingPeriod)
		{
			SetDefaultDates(trainingPeriod);

			_dbContext.TrainingPeriods.Add(trainingPeriod);
			CommitTransaction();

			return trainingPeriod;
		}

		public TrainingPeriod Update(TrainingPeriod trainingPeriod)
		{
			SetDefaultDates(trainingPeriod);

			var entry = _dbContext.Entry(trainingPeriod);
			var originalStartDate = (DateTime)entry.OriginalValues[nameof(TrainingPeriod.StartDate)];
			var originalEndDate = (DateTime)entry.OriginalValues[nameof(TrainingPeriod.EndDate)];

			if (originalEndDate != trainingPeriod.EndDate)
			{
				var hasOpen = trainingPeriod.HasOpenGrantOpenings();
				if (hasOpen)
				{
					foreach (var grantOpening in trainingPeriod.GrantOpenings)
						grantOpening.ClosingDate = trainingPeriod.EndDate;
				}
			}

			_dbContext.Update(trainingPeriod);

			CommitTransaction();

			return trainingPeriod;
		}

		public TrainingPeriod ToggleStatus(TrainingPeriod trainingPeriod)
		{
			trainingPeriod.IsActive = !trainingPeriod.IsActive;

			_dbContext.Update(trainingPeriod);
			CommitTransaction();

			return trainingPeriod;
		}

		/// <summary>
		/// Delete the specified community from the datasource.
		/// </summary>
		/// <param name="trainingPeriod"></param>
		public void Delete(TrainingPeriod trainingPeriod)
		{
			if (trainingPeriod == null)
				throw new ArgumentNullException(nameof(trainingPeriod));

			var entity = Get(trainingPeriod.Id);
			_dbContext.TrainingPeriods.Remove(entity);

			CommitTransaction();
		}

		public void SaveBudgetRates(TrainingBudgetModel trainingBudget)
		{
			var budgetEntry = _dbContext.TrainingPeriodBudgetRates
				.Where(b => b.TrainingPeriod.Id == trainingBudget.TrainingPeriodId)
				.Where(b => b.BudgetType == trainingBudget.BudgetType)
				.FirstOrDefault();

			if (budgetEntry == null)
			{
				var tp = _dbContext.TrainingPeriods.Find(trainingBudget.TrainingPeriodId);
				budgetEntry = new TrainingPeriodBudgetRate
				{
					TrainingPeriod = tp,
					BudgetType = trainingBudget.BudgetType
				};

				_dbContext.TrainingPeriodBudgetRates.Add(budgetEntry);
			}

			budgetEntry.WithdrawnRate = trainingBudget.WithdrawnRate / 100;
			budgetEntry.RefusalRate = trainingBudget.RefusalRate / 100;
			budgetEntry.ApprovedSlippageRate = trainingBudget.ApprovedSlippageRate / 100;
			budgetEntry.ClaimedSlippageRate = trainingBudget.ClaimedSlippageRate / 100;

			_dbContext.SaveChanges();
		}

		public TrainingBudgetModel GetBudget(TrainingPeriod trainingPeriod, TrainingPeriodBudgetType budgetType)
		{
			var budgetEntry = _dbContext.TrainingPeriodBudgetRates
				                  .Where(b => b.TrainingPeriod.Id == trainingPeriod.Id)
				                  .Where(b => b.BudgetType == budgetType)
				                  .FirstOrDefault() ?? new TrainingPeriodBudgetRate();

			return new TrainingBudgetModel
			{
				WithdrawnRate = budgetEntry.WithdrawnRate,
				RefusalRate = budgetEntry.RefusalRate,
				ApprovedSlippageRate = budgetEntry.ApprovedSlippageRate,
				ClaimedSlippageRate = budgetEntry.ClaimedSlippageRate
			};
		}
	}
}
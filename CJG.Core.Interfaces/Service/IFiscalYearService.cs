using CJG.Core.Entities;
using System;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
    public interface IFiscalYearService : IService
    {
        FiscalYear Get(int id);
        FiscalYear GetFiscalYear(DateTime date);

        IEnumerable<ReportRate> GetReportRates(int fiscalYearId);
        IEnumerable<FiscalYear> GetFiscalYears();

        IEnumerable<TrainingPeriod> GetTrainingPeriods(int? fiscalYearId);
    }
}

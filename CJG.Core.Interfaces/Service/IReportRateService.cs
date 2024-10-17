using CJG.Core.Entities;
using System;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
    public interface IReportRateService : IService
    {
        ReportRate Get(int fiscalYearId, int grantProgramId, int grantStreamId);

        ReportRate Add(ReportRate reportRate);
    }
}

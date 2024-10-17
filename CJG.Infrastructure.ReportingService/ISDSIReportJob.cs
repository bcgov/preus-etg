using System;

namespace CJG.Infrastructure.ReportingService
{
    internal interface ISdsiReportJob
    {
        void Start(DateTime currentDate, 
            string csvFilePath,
            string htmlFilePathTemplate, 
            int daysBefore,
            string templatePath,
			DateTime cutoffDate,
            int maxParticipants = 1000,
            bool addHeader = false);
    }
}
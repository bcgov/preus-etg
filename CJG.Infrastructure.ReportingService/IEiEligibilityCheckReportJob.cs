using System;

namespace CJG.Infrastructure.ReportingService
{
	internal interface IEiEligibilityCheckReportJob
	{
		void Start(DateTime currentDate, string csvFilePath, DateTime cutoffDate, int maxParticipants = 1000);
	}
}
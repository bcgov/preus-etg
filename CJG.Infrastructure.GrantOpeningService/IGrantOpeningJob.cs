using System;

namespace CJG.Infrastructure.GrantOpeningService
{
    internal interface IGrantOpeningJob
    {
        void Start(DateTime currentDate, int optionsNumberOfDaysBefore);
    }
}
using System;
using Fclp;
using NLog;

namespace CJG.Infrastructure.GrantOpeningService
{
    // -p (Pause) - to pause will be waiting key press before exit, convenient for F5 debugging
    // -d (current Date) explicitly specify current date. By default, it uses DateTime.UtcNow. 
    //    It is convenient to execute the process and pretend that it was run on different date. 
    //    It will parse text in any format compatible with DateTime.Parse. For example: 2017-11-25. 
    // -x (date range with number of days before check date). By default, it will verify if GrantOpening milestone date is 'expired' or not. 
    //    It will process only if [milestone date] >= [check date] - [expiry days] AND [milestone date] < [current date] 
    //    When "-x" option specified specifies number of 'expiry days', 10 by default
    /// <summary>
    ///     Command line options:
    /// </summary>
    internal class Program
    {
        private static int Main(string[] args)
        {
            var resultCode = SystemExitCode.FatalError;

            ILogger logger = null;
            Options options = null;

            try
            {
                options = ParseArgs(args);

                var appFactory = new AppFactory();
                logger = appFactory.GetLogger();
                var job = appFactory.GetGrantOpeningJob();
                job.Start(options.CurrentDate, options.NumberOfDaysBefore);
                resultCode = SystemExitCode.Success;
            }
            catch (Exception e)
            {
                logger?.Fatal(e);
                Console.WriteLine(e);
            }

            if (options?.PauseBeforeExit ?? false)
            {
                Console.WriteLine("Press any key to exit");
                Console.Read();
            }
            return (int) resultCode;
        }

        private static Options ParseArgs(string[] args)
        {
            var options = new Options();

            var p = new FluentCommandLineParser();

            p.Setup<bool>('p', "pause")
                .Callback(pause => options.PauseBeforeExit = pause);

            p.Setup<DateTime>('d', "date")
                .SetDefault(DateTime.UtcNow)
                .Callback(date => options.CurrentDate = date);

            p.Setup<int>('x', "expired")
                .SetDefault(10)
                .Callback(i => options.NumberOfDaysBefore = i);

            var parseResult = p.Parse(args);

            if (parseResult.HasErrors)
                throw new ApplicationException("Can't parse command line arguments: " + parseResult.ErrorText);

            return options;
        }
    }

    internal enum SystemExitCode
    {
        Success = 0,
        FatalError = 1
    }

    public class Options
    {
        public bool PauseBeforeExit { get; set; }
        public DateTime CurrentDate { get; set; }
        public int NumberOfDaysBefore { get; set; }
    }
}
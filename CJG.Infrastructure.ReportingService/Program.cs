using System;
using CJG.Infrastructure.ReportingService.Properties;
using Fclp;
using NLog;

namespace CJG.Infrastructure.ReportingService
{
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
                job.Start(options.CurrentDate, 
                    string.Format(Settings.Default.CsvFilePathTemplate, options.CurrentDate),
                    string.Format(Settings.Default.HtmlFilePathTemplate, options.CurrentDate),
                    Settings.Default.NumDaysBefore,
                    $"{AppDomain.CurrentDomain.BaseDirectory}\\SDSI-Report-Template.html",
					Settings.Default.ReportCutoffDate,
                    Settings.Default.MaxParticipants, 
                    Settings.Default.CsvAddReportHeader);
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
            return (int)resultCode;
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
    }
}

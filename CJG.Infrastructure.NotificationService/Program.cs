using Fclp;
using NLog;
using System;

namespace CJG.Infrastructure.NotificationService
{
	/// <summary>
	/// Command line options:
	// -p (Pause) - to pause will be waiting key press before exit, convenient for F5 debugging
	// -d (current Date) explicitly specify current date. By default, it uses DateTime.UtcNow. 
	//    It is convenient to execute the process and pretend that it was run on different date. 
	//    It will parse text in any format compatible with DateTime.Parse. For example: 2017-11-25. 
	/// </summary>
	internal class Program
	{
		/// <summary>
		/// Principal starting point for Windows console applications.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		private static int Main(string[] args)
		{
			ILogger logger = null;
			Options options = null;
			
			try
			{
				options = ParseArgs(args);
				
				var appFactory = new AppFactory();
				logger = appFactory.GetLogger();
				var job = appFactory.GetNotificationJob();

				return (int)job.StartNotificationService(appFactory.GetAppDateTime() ?? options.CurrentDate);
			}
			catch (Exception e)
			{
				logger?.Fatal(e);
				Console.WriteLine(e);
			}

			if (options?.PauseBeforeExit ?? false) {
				Console.WriteLine("Press any key to exit");
				Console.Read();
			}

			return (int)SystemExitCode.FatalError;
		}

		/// <summary>
		/// Extract the arguments from the command line.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
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
			if (parseResult.HasErrors) throw new ApplicationException("Can't parse command line arguments: " + parseResult.ErrorText);

			return options;
		}        
	}
}

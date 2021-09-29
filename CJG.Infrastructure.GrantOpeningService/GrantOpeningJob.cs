using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using NLog;
using System;

namespace CJG.Infrastructure.GrantOpeningService
{
	public class GrantOpeningJob : IGrantOpeningJob
	{
		public class GrantOpeningStateUpdateResult
		{
			public GrantOpeningStates PreviousState { get; set; }
			public GrantOpeningStates? NewState { get; set; }
			public Exception Error { get; set; }
			public bool WasUpdated => NewState.HasValue;
			public bool HasError => Error != null;
		}

		private readonly IGrantOpeningService _grantOpeningService;
		private readonly ILogger _logger;

		public GrantOpeningJob(IGrantOpeningService grantOpeningService, ILogger logger)
		{
			_grantOpeningService = grantOpeningService;
			_logger = logger;
		}

		public void Start(DateTime currentDate, int numberOfDaysBefore)
		{
			_logger.Info($"Starting '{nameof(GrantOpeningJob)}' on {currentDate:G}");

			int succeedCount = 0, failedCount = 0, ignoredCount = 0;
			foreach (var grantOpening in _grantOpeningService.GetGrantOpeningsInStates(new[] { GrantOpeningStates.Scheduled, GrantOpeningStates.Published, GrantOpeningStates.Open }))
			{
				var grantOpeningTitle = FormatGrantOpeningTitle(grantOpening);
				var result = TryVerifyAndUpdateGrantOpening(grantOpening, currentDate.ToUniversalTime(), numberOfDaysBefore);

				if (!result.HasError)
				{
					string message;

					if (result.WasUpdated)
					{
						succeedCount++;
						message = $"Grant opening: '{grantOpeningTitle}' was successfully updated from state {result.PreviousState} to {result.NewState}";
					}
					else
					{
						ignoredCount++;
						message = $"Grant opening: '{grantOpeningTitle}' didn't match criteria to update from state: {result.PreviousState}";
					}

					_logger.Info(message);
				}
				else
				{
					failedCount++;
					_logger.Error(result.Error, $"Grant opening: '{grantOpeningTitle}' failed to update from state: {result.PreviousState} ");
				}
			}

			_logger.Info($"Processed {succeedCount + failedCount + ignoredCount} grant opening records. " +
						 $"Updated: {succeedCount} Failed: {failedCount} Ignored: {ignoredCount} ");
		}

		internal GrantOpeningStateUpdateResult TryVerifyAndUpdateGrantOpening(GrantOpening grantOpening,
			DateTime checkDateUtc, int numberOfDaysToExpiry = 10)
		{
			var result = new GrantOpeningStateUpdateResult();

			try
			{
				result.PreviousState = grantOpening.State;

				if (grantOpening.State == GrantOpeningStates.Scheduled && IsDateMatched(grantOpening.PublishDate, checkDateUtc, numberOfDaysToExpiry))
					grantOpening.State = GrantOpeningStates.Published;

				if (grantOpening.State == GrantOpeningStates.Published &&
					IsDateMatched(grantOpening.OpeningDate, checkDateUtc, numberOfDaysToExpiry))
					grantOpening.State = GrantOpeningStates.Open;

				if (grantOpening.State == GrantOpeningStates.Open && IsDateMatched(grantOpening.ClosingDate, checkDateUtc, numberOfDaysToExpiry))
					grantOpening.State = GrantOpeningStates.Closed;

				if (result.PreviousState != grantOpening.State)
				{
					_grantOpeningService.Update(grantOpening);
					result.NewState = grantOpening.State;
				}
			}
			catch (Exception e)
			{
				result.Error = e;
			}

			return result;
		}

		private static bool IsDateMatched(DateTime sourceDate, DateTime checkDate, int numberOfDaysToExpiry)
		{
			return sourceDate >= checkDate.AddDays(-numberOfDaysToExpiry) && sourceDate <= checkDate;
		}

		private static string FormatGrantOpeningTitle(GrantOpening grantOpening)
		{
			return $"{grantOpening.Id}-{grantOpening.GrantStream?.Name} from {grantOpening.OpeningDate.ToLocalMorning():yyyy-MM-dd} to {grantOpening.ClosingDate.ToLocalMidnight():yyyy-MM-dd}";
		}
	}
}
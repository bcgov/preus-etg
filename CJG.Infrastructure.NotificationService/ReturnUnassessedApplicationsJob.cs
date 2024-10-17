using System;
using System.Linq;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using NLog;

namespace CJG.Infrastructure.NotificationService
{
	public class ReturnUnassessedApplicationsJob : IReturnUnassessedApplicationsJob
	{
		public class GrantApplicationReturnStatus
		{
			public ApplicationStateInternal PreviousState { get; set; }
			public ApplicationStateInternal? NewState { get; set; }
			public Exception Error { get; set; }
			public bool WasUpdated => NewState.HasValue;
			public bool HasError => Error != null;
		}

		private readonly IGrantApplicationJobService _grantApplicationJobService;
		private readonly ISettingService _settingService;
		private readonly ILogger _logger;

		public ReturnUnassessedApplicationsJob(
			IGrantApplicationJobService grantApplicationJobService,
			ISettingService settingService,
			ILogger logger)
		{
			_grantApplicationJobService = grantApplicationJobService;
			_settingService = settingService;
			_logger = logger;
		}

		public void Start()
		{
			var currentDate = AppDateTime.UtcNow;

			const string jobName = nameof(ReturnUnassessedApplicationsJob);
			_logger.Info($"Starting '{jobName}' on {currentDate:G}");

			var jobSetting = _settingService.Get(SettingServiceKeys.ReturnUnassessedSettingKey);
			var isThisOn = jobSetting?.GetValue<bool>() ?? false;

			if (!isThisOn)
			{
				_logger.Info($"Job '{jobName}' is currently turned off. No applications will be processed. Exiting.");
				return;
			}

			var succeedCount = 0;
			var failedCount = 0;
			var ignoredCount = 0;

			var unassessedGrantApplications = _grantApplicationJobService
				.GetUnassessedGrantApplications()
				.ToList();

			_logger.Info($"Job '{jobName}' - Found {unassessedGrantApplications.Count} applications to process.");

			foreach (var grantApplication in unassessedGrantApplications)
			{
				var grantApplicationTitle = grantApplication.FileNumber;
				var result = TryToReturnGrantApplication(grantApplication);

				if (!result.HasError)
				{
					string message;

					if (result.WasUpdated)
					{
						succeedCount++;
						message = $"Grant application: '{grantApplicationTitle}' was successfully updated from state {result.PreviousState} to {result.NewState}";
					}
					else
					{
						ignoredCount++;
						message = $"Grant application: '{grantApplicationTitle}' didn't match criteria to update from state: {result.PreviousState}";
					}

					_logger.Info(message);
				}
				else
				{
					failedCount++;
					_logger.Error(result.Error, $"Grant application: '{grantApplicationTitle}' failed to update from state: {result.PreviousState} ");
				}
			}

			var processed = succeedCount + failedCount + ignoredCount;
			_logger.Info($"Job '{jobName}' finished. Processed {processed} grant application records. Updated: {succeedCount} Failed: {failedCount} Ignored: {ignoredCount} ");
		}

		internal GrantApplicationReturnStatus TryToReturnGrantApplication(GrantApplication grantApplication)
		{
			var result = new GrantApplicationReturnStatus();

			try
			{
				result.PreviousState = grantApplication.ApplicationStateInternal;

				_grantApplicationJobService.ReturnUnassessed(grantApplication);

				if (result.PreviousState != grantApplication.ApplicationStateInternal)
					result.NewState = grantApplication.ApplicationStateInternal;
			}
			catch (Exception e)
			{
				result.Error = e;
			}

			return result;
		}
	}
}
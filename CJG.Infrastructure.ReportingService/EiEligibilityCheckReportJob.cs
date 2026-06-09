using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using NLog;

namespace CJG.Infrastructure.ReportingService
{
	internal class EiEligibilityCheckReportJob : IEiEligibilityCheckReportJob
	{
		private readonly ILogger _logger;
		private readonly IParticipantService _participantService;
		private readonly ISettingService _settingService;

		public EiEligibilityCheckReportJob(IParticipantService participantService, ISettingService settingService, ILogger logger)
		{
			_participantService = participantService;
			_settingService = settingService;
			_logger = logger;
		}

		public void Start(DateTime currentDate, string csvFilePath, DateTime cutoffDate, int maxParticipants = 1000)
		{
			const string jobName = nameof(EiEligibilityCheckReportJob);
			_logger.Info($"Starting '{jobName}' on {currentDate:G}");

			var jobSetting = _settingService.Get(SettingServiceKeys.EiEligibilityCheckServiceSettingKey);
			var isThisOn = jobSetting?.GetValue<bool>() ?? false;

			if (!isThisOn)
			{
				_logger.Info($"Job '{jobName}' is currently turned off. No participants will be processed. Exiting.");
				return;
			}

			_logger.Debug($"ETG EI Eligibility Check Report Job: Retrieving participants... [current date: {currentDate.ToStringLocalTime()}] [maxParticipants: {maxParticipants}]");

			var participants = _participantService.GetParticipantsEnrollmentsForEiCheck(currentDate, maxParticipants, cutoffDate)
				.ToList();

			_logger.Debug("Converting and formatting data...");

			var header = CreateCsvHeaderString();

			var placeholders = participants.Select(ConvertParticipantFormToPlaceholders).ToList();

			_logger.Debug("Exporting data into csv file...");

			var participantCount = ExportToCsvFile(placeholders.Select(ConvertPlaceholdersToCsvString), csvFilePath, header);

			_logger.Info($"Exported {participantCount} participant records into csv file: {csvFilePath}");

			_participantService.UpdateEiEligibilityReportedDate(participants, currentDate);

			_logger.Info($"All {participants.Count} exported participant records marked with date {currentDate.ToUniversalTime()}");
		}

		private string CreateCsvHeaderString()
		{
			return new CsvLineBuilder()
				.AppendColumn("SIN")
				.AppendColumn("File Number")
				.AppendColumn("First Name")
				.AppendColumn("Last Name")
				.ToString();
		}

		private static IDictionary<string, string> ConvertParticipantFormToPlaceholders(ParticipantForm participant)
		{
			var ga = participant.GrantApplication;
			var placeholders = new Dictionary<string, string>
			{
				{"SocialInsuranceNumber", participant.SIN},
				{"FileNumber", ga.FileNumber},
				{"FirstName", participant.FirstName},
				{"LastName", participant.LastName},
			};

			return placeholders;
		}

		internal static double GetTrainingHoursPerWeek(int totalTrainingHours, DateTime startDate, DateTime endDate)
		{
			var dateDiffInWeeks = DateDiffInWeeks(startDate, endDate, true);
			return dateDiffInWeeks > 1 ? totalTrainingHours / dateDiffInWeeks : totalTrainingHours;
		}

		private static string ConvertPlaceholdersToCsvString(IDictionary<string, string> placeholders)
		{
			var builder = new CsvLineBuilder();

			foreach (var placeholder in placeholders)
				builder.AppendColumn(placeholder.Value);

			return builder.ToString();
		}

		internal static double DateDiffInWeeks(DateTime startDate, DateTime endDate, bool isEndDateInclusive = false)
		{
			return ((endDate.Date - startDate.Date).TotalDays + (isEndDateInclusive ? 1 : 0)) / 7;
		}

		private static int ExportToCsvFile(IEnumerable<string> lines, string filePath, string headerLine = null)
		{
			var index = 0;
			var path = new FileInfo(filePath).DirectoryName;

			// Create the directory if doesn't exist.
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			using (var w = new StreamWriter(filePath))
			{
				if (!string.IsNullOrWhiteSpace(headerLine))
					w.WriteLine(headerLine);

				foreach (var line in lines)
				{
					w.WriteLine(line);
					w.Flush();
					index++;
				}
			}

			return index;
		}
	}
}
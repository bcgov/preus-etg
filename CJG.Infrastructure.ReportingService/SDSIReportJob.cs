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
	internal class SdsiReportJob : ISdsiReportJob
	{
		private readonly ILogger _logger;
		private readonly IParticipantService _participantService;

		public SdsiReportJob(IParticipantService participantService, ILogger logger)
		{
			_participantService = participantService;
			_logger = logger;
		}

		public void Start(DateTime currentDate, string csvFilePath, string htmlFilePathTemplate, int daysBefore, string templatePath, int maxParticipants = 1000, bool addHeader = false)
		{
			_logger.Debug("Retrieving participants...");

			var participants = _participantService.GetUnemployedParticipantEnrollments(
				currentDate.AddDays(-daysBefore), maxParticipants).ToList();

			_logger.Debug("Converting and formatting data...");

			var header = addHeader ? CreateCsvHeaderString() : null;

			var placeholders = participants.Select(ConvertParticipantFormToPlaceholders).ToList();

			_logger.Debug("Exporting data into csv file...");

			var participantCount = ExportToCsvFile(placeholders.Select(ConvertPlaceholdersToCsvString), csvFilePath, header);

			_logger.Info($"Exported {participantCount} participant records into csv file: {csvFilePath}");

			_logger.Debug("Exporting data into html files...");

			var htmlTemplateText = File.ReadAllText(templatePath);

			foreach (var placeholder in placeholders)
			{
				var htmlFile = ExportToHtmlFile(htmlTemplateText, placeholder, htmlFilePathTemplate);
				_logger.Info($"Exporting participant data into html files {htmlFile}");
			}

			_participantService.UpdateReportedDate(participants, currentDate);

			_logger.Info($"All {participants.Count} exported participant records  marked with date {currentDate.ToUniversalTime()}");

		}

		private string CreateCsvHeaderString()
		{
			return new CsvLineBuilder()
				.AppendColumn("ID")
				.AppendColumn("CJG Application Number")
				.AppendColumn("Participant")
				.AppendColumn("SIN")
				.AppendColumn("Contact Info")
				.AppendColumn("Employer")
				.AppendColumn("Course Name")
				.AppendColumn("School")
				.AppendColumn("Start Date")
				.AppendColumn("End Date")
				.AppendColumn("Hours per week")
				.AppendColumn("Referring  Adjudicator")
				.AppendColumn("Phone Number")
				.AppendColumn("Email")
				.AppendColumn("Notes")
				.ToString();
		}

		private static IDictionary<string, string> ConvertParticipantFormToPlaceholders(ParticipantForm participant)
		{
			var tp = participant.GrantApplication.TrainingPrograms.FirstOrDefault();
			var ga = tp.GrantApplication;
			var total_hours = ga.TrainingPrograms.Sum(t => t.TotalTrainingHours);
			var placeholders = new Dictionary<string, string>
			{
				{"ParticipantFormId", participant.Id.ToString()},
				{"FileNumber", ga.FileNumber},
				{"ParticipantFullName", ValueFormatters.FormatName(participant.FirstName, participant.LastName, participant.MiddleName)},
				{"SocialInsuranceNumber", participant.SIN},
				{"ParticipantContactInfo", ValueFormatters.FormatContactInfo(participant)},
				{"OrganizationLegalName", ga.OrganizationLegalName},
				{"TrainingProgramTitle", tp.CourseTitle},
				{"TrainingProviderName", tp.TrainingProvider.Name},
				{"TrainingStartDate", ga.StartDate.ToLocalTime().ToString("yyyy-MM-dd")},
				{"TrainingEndDate", ga.EndDate.ToLocalTime().ToString("yyyy-MM-dd")},
				{"HoursPerWeek", GetTrainingHoursPerWeek(total_hours, ga.StartDate, ga.EndDate).ToString("F1")},
				{"AssignedAssessorName", ValueFormatters.FormatName(ga.Assessor?.FirstName, ga.Assessor?.LastName)},
				{"AssignedAssessorPhoneNumber", ValueFormatters.FormatPhone(ga.Assessor?.PhoneNumber, ga.Assessor?.PhoneNumberExt)},
				{"AssignedAssessorEmail", ga.Assessor?.Email},
				{"AssessorNotes", ga.AssessorNote},
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

			foreach (var placeholder in placeholders.Where(x=>x.Key != "ParticipantEnrollmentId"))
			{
				builder.AppendColumn(placeholder.Value);
			}

			return builder.ToString();
		}

		internal static double DateDiffInWeeks(DateTime startDate, DateTime endDate, bool isEndDateInclusive = false)
		{
			return ((endDate.Date - startDate.Date).TotalDays + (isEndDateInclusive ? 1 : 0))/ 7;
		}

		private static string ExportToHtmlFile(string sourceTemplateText, IDictionary<string, string> placeholders, string targetFilePathTemplate)
		{
			var fileContent = Utilities.ParseTemplate(placeholders, sourceTemplateText).ToString();

			var filePath = Utilities.ParseTemplate(placeholders, targetFilePathTemplate).ToString();

			File.WriteAllText(filePath, fileContent);

			return filePath;
		}

		private static int ExportToCsvFile(IEnumerable<string> lines, string filePath, string headerLine = null)
		{
			var index = 0;

			var path = new FileInfo(filePath).DirectoryName;
			// Create the directory if doesn't exist.
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			using (var w = new StreamWriter(filePath))
			{
				if(!string.IsNullOrWhiteSpace(headerLine))
				{
					w.WriteLine(headerLine);
				}

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
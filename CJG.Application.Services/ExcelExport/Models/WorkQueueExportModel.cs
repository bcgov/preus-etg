using System;
using ClosedXML.Attributes;

namespace CJG.Application.Services.ExcelExport.Models
{
	public class WorkQueueExportModel
	{
		[XLColumn(Header = "File Number", Order = 1)]
		public string FileNumber { get; set; }

		[XLColumn(Header = "Organization", Order = 2)]
		public string Applicant { get; set; }

		[XLColumn(Header = "Date Submitted", Order = 3)]
		public DateTime DateSubmitted { get; set; }

		[XLColumn(Header = "Training Start", Order = 4)]
		public DateTime StartDate { get; set; }

		[XLColumn(Header = "Priority Score", Order = 5)]
		public int PrioritizationScore { get; set; }

		[XLColumn(Header = "Status", Order = 6)]
		public string Status { get; set; }

		[XLColumn(Header = "Status Changed", Order = 7)]
		public DateTime StatusChanged { get; set; }

		[XLColumn(Header = "Stream", Order = 8)]
		public string GrantStreamName { get; set; }

		[XLColumn(Header = "Is Risk", Order = 9)]
		public string IsRisk { get; set; }

		[XLColumn(Header = "Requested Government Contribution", Order = 10)]
		public string RequestedGovernmentContribution { get; set; }
	}
}
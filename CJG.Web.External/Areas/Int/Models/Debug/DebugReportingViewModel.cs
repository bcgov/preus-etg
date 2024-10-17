using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Int.Models
{
    public class DebugReportingViewModel
    {
        public DateTime CurrentDate { get; set; }
        public List<string> LogRecords { get; set; }
        public int NumberOfDaysBefore { get; set; } = 100;
        public int MaxParticipants { get; set; } = 10;
        public bool AddHeader { get; set; } = true;
        public string[] SelectedFilePaths { get; set; }
        public List<SelectListItem> FileNames { get; set; }

        public DebugReportingViewModel()
        {
            LogRecords = new List<string>();
        }
    }
}
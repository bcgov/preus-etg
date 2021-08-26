using System;
using System.Collections.Generic;

namespace CJG.Web.External.Areas.Int.Models
{
    public class DebugGrantOpeningViewModel
    {
        public DateTime CurrentDate { get; set; }
        public int NumberOfDaysBefore { get; set; }
        public List<string> LogRecords { get; set; }
        
        public DebugGrantOpeningViewModel()
        {
            LogRecords = new List<string>();
        }
    }
}
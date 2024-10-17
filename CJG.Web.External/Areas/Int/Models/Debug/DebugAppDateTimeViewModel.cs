using CJG.Web.External.Models.Shared;
using System;

namespace CJG.Web.External.Areas.Int.Models
{
    public class DebugAppDateTimeViewModel : BaseViewModel
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
        public DebugAppDateTimeViewModel() { }

        public DebugAppDateTimeViewModel(DateTime date)
        {
            this.Day = date.Day;
            this.Month = date.Month;
            this.Year = date.Year;
            this.Hour = date.Hour;
            this.Minute = date.Minute;
            this.Second = date.Second;
        }
    }
}